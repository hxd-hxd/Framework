using System;
using System.Collections.Generic;

namespace Framework.Prob
{
    /// <summary>
    /// 分支
    /// </summary>
    /// <typeparam name="TValue">包含类型</typeparam>
    /// <typeparam name="TProbBranch">实现的分支类型</typeparam>
    /// <typeparam name="TProbItem">实现的概率类型</typeparam>
    public abstract class BaseProbBranch<TValue, TProbBranch, TProbItem> :
        BaseContainer<TValue, TProbBranch, TProbItem>,
        IBranch<TValue, TProbBranch, TProbItem>
        where TProbBranch : BaseProbBranch<TValue, TProbBranch, TProbItem>
        where TProbItem : BaseProbItem<TValue, TProbBranch, TProbItem>
    {

        public abstract float probValue { get; set; }

        public abstract bool enable { get; set; }

        public abstract bool allProb { get; set; }

        public abstract IContainer<TValue, TProbBranch, TProbItem> owner { get; set; }

        /// <summary>根节点</summary>
        public IContainer<TValue, TProbBranch, TProbItem> root
        {
            get
            {
                IContainer<TValue, TProbBranch, TProbItem> parent = owner;
                while (true)
                {
                    if (parent is TProbBranch parentBranch)
                    {
                        parent = parentBranch.owner;
                        continue;
                    }
                    break;
                }
                return parent;
            }
        }

        public BaseProbBranch()
        {
            Init();
        }

        protected virtual void Init()
        {
            enable = true;
            probValue = -1;
            containerName = default;
            items = new List<TProbItem>();
            branchs = new List<TProbBranch>();
        }

        public virtual float RealProb()
        {
            if (owner == null) return probValue;

            return RealProb(allProb);
        }

        /// <summary>
        /// 真实概率（返回百分比值<para>例：概率是 20%，即返回 20</para>）
        /// <para>ps：计算公式：真实概率 = 上级真实概率 * 本级百分比转换概率</para>
        /// </summary>
        /// <returns>百分比值</returns>
        public float RealProb(bool AllProb)
        {
            if (owner == null) return probValue;

            float prob = 0;
            // 所属者是否分支
            if (owner is IBranch<TValue, TProbBranch, TProbItem> branch)
            {
                // 获取所属分支地真实概率
                prob = branch.RealProb();

                // 计算百分比概率
                float branchSumProb = branch.branchs.GetSumProbValue();// 上级分支的总概率
                if (AllProb)
                    branchSumProb += branch.items.GetSumProbValue();// 是否包含所有概率项

                float prob1 = probValue.FormatProb1(branchSumProb);

                // 真实概率
                prob *= prob1;
            }
            else
            {
                // 这里直接计算
                // 计算百分比概率
                float branchSumProb = owner.branchs.GetSumProbValue();// 上级分支的总概率
                if (AllProb)
                    branchSumProb += owner.items.GetSumProbValue();// 是否包含所有概率项

                prob = probValue.FormatProb100(branchSumProb);// 真实概率
            }

            return prob;
        }

        public string GetPath()
        {
            string cn = string.Empty;
            if (owner != null)
            {
                if (owner is IBranch<TValue, TProbBranch, TProbItem> branch)
                {
                    cn = branch.GetPath() + "/" + owner.containerName;
                }
                else
                {
                    cn = owner.containerName;
                }
            }

            return cn;
        }

        public virtual bool IsValid(bool ignoreEnable = false)
        {
            if (ignoreEnable)
            {
                if (probValue > 0) return true;
            }
            else if (enable)
            {
                if (probValue > 0) return true;
            }
            return false;
        }
    }

}