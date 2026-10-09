

namespace Framework.Prob
{
    /// <summary>概率项</summary>
    public abstract class BaseProbItem<TValue, TProbBranch, TProbItem> : 
        IItem<TValue, TProbBranch, TProbItem>, ITypePoolObject
        where TProbBranch : BaseProbBranch<TValue, TProbBranch, TProbItem>
        where TProbItem : BaseProbItem<TValue, TProbBranch, TProbItem>
    {
        public abstract TValue value { get; set; }
        public abstract float probValue { get; set; }
        public abstract bool enable { get; set; }

        public abstract IContainer<TValue, TProbBranch, TProbItem> owner { get; set; }

        /// <summary>根节点</summary>
        public IContainer<TValue, TProbBranch, TProbItem> root
        {
            get
            {
                var parent = owner;
                while (parent is TProbBranch branch)
                {
                    parent = branch?.owner;
                }
                return parent;
            }
        }

        public float RealProb()
        {
            if (owner == null) return probValue;
            float prob = 0;
            // 所属者是否分支
            if (owner is IBranch<TValue, TProbBranch, TProbItem> branch)
            {
                // 获取所属分支地真实概率
                prob = branch.RealProb();

                // 计算百分比概率
                float branchSumProb = branch.items.GetSumProbValue();// 上级分支的总概率
                branchSumProb += branch.branchs.GetSumProbValue();// 是否包含所有概率项
                float prob1 = probValue.FormatProb1(branchSumProb);

                // 真实概率
                prob = prob * prob1;
            }
            else
            {
                // 这里直接计算
                // 计算百分比概率
                float branchSumProb = owner.items.GetSumProbValue();// 上级分支的总概率
                branchSumProb += owner.branchs.GetSumProbValue();// 是否包含所有概率项

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

        public virtual bool IsValid()
        {
            if (enable)
            {
                if (probValue > 0) return true;
            }
            return false;
        }

        void ITypePoolObject.Clear()
        {
            owner = null;
            value = default;
            probValue = -1;
            enable = true;
        }
    }
}