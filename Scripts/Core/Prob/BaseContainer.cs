using System;
using System.Collections;
using System.Collections.Generic;

namespace Framework.Prob
{
    /// <summary>容器基类</summary>
    public abstract class BaseContainer<TValue, TProbBranch, TProbItem> :
        IContainer<TValue, TProbBranch, TProbItem>, ITypePoolObject
        where TProbBranch : BaseProbBranch<TValue, TProbBranch, TProbItem>
        where TProbItem : BaseProbItem<TValue, TProbBranch, TProbItem>
    {
        private string _containerName = "Node";

        private List<TProbItem> _items;
        private List<TProbBranch> _branchs;

        internal List<IProb> _validProbs = new List<IProb>();
        internal List<float> _probRanges = new List<float>();

        public abstract IRandomProvider randomProvider { get; set; }
        public abstract bool isDirty { get; set; }

        List<IProb> IContainer<TValue, TProbBranch, TProbItem>.validProbs { get => _validProbs; set => _validProbs = value; }

        List<float> IContainer<TValue, TProbBranch, TProbItem>.probRanges { get => _probRanges; set => _probRanges = value; }

        public virtual List<TProbItem> items
        {
            get => _items ??= new List<TProbItem>();
            set
            {
                if (!Equals(_items, value)) isDirty = true;
                _items = value;
            }
        }
        public virtual List<TProbBranch> branchs
        {
            get => _branchs ??= new List<TProbBranch>();
            set
            {
                if (!Equals(_branchs, value)) isDirty = true;
                _branchs = value;
            }
        }
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
                isDirty = true;
            }
            return add;
        }

        /// <summary>添加分支</summary>
        public TProbBranch AddBranch(string containerName, float provValue)
        {
            TProbBranch branch = TypePool.root.Get<TProbBranch>();
            branch.containerName = containerName;
            branch.probValue = provValue;
            if (!AddBranch(branch))
            {
                TypePool.root.Return(branch);
                branch = null;
            }
            return branch;
        }

        public bool AddItem(TProbItem item)
        {
            bool add = AddNotContains(items, item);
            if (add)
            {
                item.owner = this;
                isDirty = true;
            }
            return add;
        }

        /// <summary>添加概率项</summary>
        public TProbItem AddItem(float provValue, TValue value)
        {
            TProbItem item = TypePool.root.Get<TProbItem>();
            item.value = value;
            item.probValue = provValue;
            if (!AddItem(item))
            {
                TypePool.root.Return(item);
                item = null;
            }
            return item;
        }

        public bool RemoveItem(TProbItem item)
        {
            bool remove = items.Remove(item);
            if (remove)
            {
                item.owner = null;
                isDirty = true;
            }
            return remove;
        }

        /// <summary>移除概率项</summary>
        public bool RemoveItem(TValue value)
        {
            var index = items.FindIndex((i) => Equals(i.value, value));
            if (index < 0) return false;
            var item = items[index];

            items.RemoveAt(index);
            item.owner = null;
            isDirty = true;
            TypePool.root.Return(item);
            return true;
        }

        public bool RemoveBranch(TProbBranch branch)
        {
            bool remove = branchs.Remove(branch);
            if (remove)
            {
                branch.owner = null;
                isDirty = true;
            }
            return remove;
        }

        /// <summary>移除分支</summary>
        public bool RemoveBranch(string containerName)
        {
            var index = branchs.FindIndex((tpb) => tpb.containerName == containerName);
            if (index < 0) return false;
            var branch = branchs[index];

            branchs.RemoveAt(index);
            branch.owner = null;
            isDirty = true;
            TypePool.root.Return(branch);
            return true;
        }

        public TProbItem GetRandmoDirectlyItem()
        {
            TProbItem p = items.RandomProb(randomProvider);
            return p;
        }

        public TProbBranch GetRandmoDirectlyBranch()
        {
            return branchs.RandomProb(randomProvider);
        }

        public TProbItem GetRandmoDirectlyBranchItem()
        {
            TProbBranch p = GetRandmoDirectlyBranch();
            return p?.GetRandmoDirectlyItem();
        }

        public TProbBranch GetRandmoBranch()
        {
            var b = GetRandmoDirectlyBranch();
            while (b != null)
            {
                bool next = randomProvider.RandomProbValue() <= 50 ? false : true;
                if (next)
                {
                    var b1 = b.GetRandmoDirectlyBranch();
                    if (b == b1) break;
                    b = b1;
                }
            }
            return b;
        }

        /// <summary>随机获取一个概率项的结果</summary>
        public virtual TProbItem GetRandomItem()
        {
            // 因为分支既可以有项也可以有下级分支，所以要在项和分支里共同随机

            TProbItem r = default;
            IContainer<TValue, TProbBranch, TProbItem> b = this;

            while (b != null)
            {
                b.UpdateValidProbs();

                var p = b.validProbs.InternalRandomProb(randomProvider, b.probRanges);
                // 随机到容器继续
                if (p is IContainer<TValue, TProbBranch, TProbItem> c)
                {
                    b = c;
                    continue;
                }

                r = p as TProbItem;
                break;
            }
            return r;
        }

        /// <summary>随机获取一个概率项的结果</summary>
        public virtual TValue GetRandomValue()
        {
            TProbItem p = GetRandomItem();
            return p == null ? default : p.value;
        }

        public float GetRealProb(TValue value)
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
        public bool HasValidItem()
        {
            foreach (var item in items)
            {
                if (item.IsValid()) return true;
            }
            foreach (var b1 in branchs)
            {
                if (b1.HasValidItem()) return true;
            }
            return false;
        }

        public void UpdateValidProbs()
        {
            if (!isDirty) return;

            List<IProb> validProbs = _validProbs;
            validProbs.Clear();
            // 添加项
            foreach (var item in items)
            {
                if (item.IsValid()) validProbs.Add(item);
            }
            // 添加容器
            foreach (var b1 in branchs)
            {
                // 没有项的分支不可参与随机
                if (b1.IsValid() && b1.HasValidItem()) validProbs.Add(b1);
            }

            validProbs.GetProbRanges(_probRanges);

            isDirty = false;
        }

        public virtual void Clear()
        {
            if (items.Count > 0 || branchs.Count > 0) isDirty = true;

            foreach (var item in items)
            {
                if (item != null)
                    item.owner = null;
            }
            foreach (var item in branchs)
            {
                if (item != null)
                    item.owner = null;
            }
            //items.Clear();
            //branchs.Clear();
            TypePool.root.ReturnE(items);
            TypePool.root.ReturnE(branchs);
        }

        void ITypePoolObject.Clear()
        {
            OnPoolClear();
        }

        protected virtual void OnPoolClear()
        {
            containerName = "Node";
            Clear();
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
