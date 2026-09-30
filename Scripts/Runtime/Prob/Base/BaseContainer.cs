using System;
using System.Collections;
using System.Collections.Generic;
using static Codice.CM.Common.BranchExplorerData;
using static UnityEditor.Progress;

namespace Framework.Prob
{
    /// <summary>容器基类</summary>
    public abstract class BaseContainer<TValue, TProbBranch, TProbItem> :
        IContainer<TValue, TProbBranch, TProbItem>
        where TProbBranch : BaseProbBranch<TValue, TProbBranch, TProbItem>
        where TProbItem : BaseProbItem<TValue, TProbBranch, TProbItem>
    {
        private string _containerName = "Node";

        private List<TProbItem> _items;
        private List<TProbBranch> _branchs;

        //public abstract List<TProbItem> items { get; set; }
        //public abstract List<TProbBranch> branchs { get; set; }
        //public abstract string containerName { get; set; }

        public abstract IRandomProvider randomProvider { get; set; }

        public virtual List<TProbItem> items { get => _items ??= new List<TProbItem>(); set => _items = value; }
        public virtual List<TProbBranch> branchs { get => _branchs ??= new List<TProbBranch>(); set => _branchs = value; }
        public virtual string containerName { get => _containerName; set => _containerName = value; }

        public bool AddBranch(TProbBranch branch)
        {
            // 不可添加同名分支
            if (branchs.Find((tpb) => { return tpb.containerName == branch.containerName; }) != null)
                return false;

            bool add = AddNotContains(branchs, branch);
            if (add)
            {
                branch.owner = this;
            }
            return add;
        }

        /// <summary>添加分支</summary>
        public TProbBranch AddBranch(string containerName, float provValue)
        {
            TProbBranch branch = Activator.CreateInstance<TProbBranch>();
            branch.containerName = containerName;
            branch.probValue = provValue;
            branch = AddBranch(branch) ? branch : null;
            return branch;
        }

        public bool AddItem(TProbItem type)
        {
            bool add = AddNotContains(items, type);
            if (add)
            {
                type.owner = this;
                //type.ContainerVariable = this;
            }
            return add;
        }

        /// <summary>添加概率项</summary>
        public TProbItem AddItem(float provValue, TValue value)
        {
            TProbItem item = Activator.CreateInstance<TProbItem>();
            item.value = value;
            item.probValue = provValue;
            item = AddItem(item) ? item : null;
            return item;
        }

        public TProbItem GetRandmoDirectlyItem(bool ignoreEnable = false)
        {
            TProbItem p = items.RandomProb(randomProvider, ignoreEnable);
            return p;
        }

        public TProbBranch GetRandmoDirectlyBranch(bool ignoreEnable = false)
        {
            return branchs.RandomProb(randomProvider, ignoreEnable);
        }

        public TProbItem GetRandmoDirectlyBranchItem(bool ignoreEnable = false)
        {
            TProbBranch p = GetRandmoDirectlyBranch(ignoreEnable);
            return p?.GetRandmoDirectlyItem(ignoreEnable);
        }

        public TProbBranch GetRandmoBranch(bool ignoreEnable = false)
        {
            var b = GetRandmoDirectlyBranch(ignoreEnable);
            while (b != null)
            {
                bool next = randomProvider.RandomProbValue() <= 50 ? false : true;
                if (next)
                {
                    var b1 = b.GetRandmoDirectlyBranch(ignoreEnable);
                    if (b == b1) break;
                    b = b1;
                }
            }
            return b;
        }

        /// <summary>随机获取一个概率项的结果</summary>
        public virtual TProbItem GetRandomItem(bool ignoreEnable = false)
        {
            // TODO：因为分支既可以有项也可以有下级分支，所以要在项和分支里共同随机
            //return GetRandmoDirectlyBranchItem(ignoreEnable);

            TProbItem r = default;
            IContainer<TValue, TProbBranch, TProbItem> b = this;

            var ps = TypePool.root.GetList<IProb>();

            while (b != null)
            {
                ps.Clear();
                // 添加项
                //ps.AddRange(b.items);
                foreach (var item in b.items)
                {
                    if (item.IsValid(ignoreEnable)) ps.Add(item);
                }
                // 添加容器
                foreach (var b1 in b.branchs)
                {
                    // 没有项的分支不可参与随机
                    if (b1.IsValid(ignoreEnable) && b1.HasValidItem()) ps.Add(b1);
                }

                var p = ps.InternalRandomProb(randomProvider);
                // 随机到容器继续
                if (p is IContainer<TValue, TProbBranch, TProbItem> c)
                {
                    b = c;
                    continue;
                }

                r = p as TProbItem;
                break;
            }
            TypePool.root.Return(ps);
            return r;
        }

        /// <summary>随机获取一个概率项的结果</summary>
        public virtual TValue GetRandomValue(bool ignoreEnable = false)
        {
            TProbItem p = GetRandomItem(ignoreEnable);
            return p == null ? default : p.value;
        }

        public float GetRealProb(TValue value, bool ignoreEnable = false)
        {
            TProbItem probItem = FindItem(value);
            return probItem == null ? 0 : probItem.RealProb();
        }

        public TProbItem FindItem(TValue value)
        {
            TProbItem p = items.Find((tpi) => tpi.value.Equals(value));

            if (p == null)
                for (int i = 0; i < branchs.Count; i++)
                {
                    p = branchs[i].FindItem(value);
                    if (p != null) break;
                }

            return p;
        }

        public TProbBranch FindBranch(Func<TProbBranch, bool> condition)
        {
            TProbBranch branch = branchs.Find((b) => { return (bool)(condition?.Invoke(b)); });
            for (int i = 0; i < branchs.Count; i++)
            {
                branch = branchs[i].FindBranch(condition);
                if (branch != null) break;
            }

            return branch;
        }

        /// <summary>查找节点
        /// <para>path - 相对路径，格式：Node/a/b/c，其中 Node、a、b 等都是 <see cref="containerName"/></para>
        /// </summary>
        public virtual TProbBranch FindBranch(string path)
        {
            string[] strs = path.Split('/');
            TProbBranch branch = this as TProbBranch;

            int start = strs[0] == containerName ? 1 : 0;// 排除自己

            for (int i = 0; i < strs.Length; i++)
            {
                string name = strs[i];
                branch = branch.FindBranch((TProbBranch item) =>
                {
                    return item.containerName == name;
                });
            }
            return branch;
        }

        /// <summary>如要读取保存后的数据，请在读取完之后调用此函数</summary>
        public virtual void OnReadAfter()
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] != null)
                {
                    items[i].owner = this;
                }
            }

            for (int i = 0; i < branchs.Count; i++)
            {
                if (branchs[i] != null)
                {
                    branchs[i].owner = this;
                    branchs[i].OnReadAfter();
                }
            }
        }

        /// <summary>存在有效项，会递归查找子分支，有任意一个子分支存在有效项即可</summary>
        public bool HasValidItem(bool ignoreEnable = false)
        {
            foreach (var item in items)
            {
                if (item.IsValid(ignoreEnable)) return true;
            }
            foreach (var b1 in branchs)
            {
                if (b1.HasValidItem(ignoreEnable)) return true;
            }
            return false;
        }

        public void Clear()
        {
            items.Clear();
            branchs.Clear();
        }

        /// <summary>添加时检查是否包含，已包含则不添加</summary>
        private static bool AddNotContains<T>(List<T> value, T t)
        {
            if (value.Contains(t)) return false;
            value.Add(t);

            return true;
        }
    }
}
