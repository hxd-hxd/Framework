using System.Collections;
using System.Collections.Generic;

namespace Framework.Prob
{
    /// <summary>
    /// 容器接口
    /// </summary>
    /// <typeparam name="TValue">包含类型</typeparam>
    /// <typeparam name="TProbBranch">实现的分支类型</typeparam>
    /// <typeparam name="TProbItem">实现的概率类型</typeparam>
    public interface IContainer<TValue, TProbBranch, TProbItem> 
        where TProbBranch : IBranch<TValue, TProbBranch, TProbItem>
        where TProbItem : IProb
    {
        /// <summary>容器名称</summary>
        string containerName { get; set; }

        /// <summary>此概率所包含的直属列表</summary>
        List<TProbItem> items { get; set; }

        /// <summary>此概率所包含的分支概率列表</summary>
        List<TProbBranch> branchs { get; set; }

        /// <summary>随机提供者</summary>
        IRandomProvider randomProvider { get; set; }

        /// <summary>容器内容是否被修改
        /// <para>标脏基础规则：容器内的项和分支变化时，标记所属容器；添加和移除容器内容时，标记容器本身</para>
        /// </summary>
        bool isDirty { get; set; }

        /// <summary>内部维护的有效概率列表</summary>
        internal List<IProb> validProbs { get; set; }

        /// <summary>概率范围列表，应当是根据基数 100 计算过的累加值</summary>
        internal List<float> probRanges { get; set; }

        /// <summary>添加直属项</summary>
        bool AddItem(TProbItem item);

        /// <summary>
        /// 添加直属分支
        /// <para>注意：不可添加同名分支</para>
        /// </summary>
        bool AddBranch(TProbBranch branch);

        /// <summary>移除直属项</summary>
        bool RemoveItem(TProbItem item);

        /// <summary>移除直属分支</summary>
        bool RemoveBranch(TProbBranch branch);

        /// <summary>随机获取一个直属概率项</summary>
        TProbItem GetRandmoDirectlyItem();

        /// <summary>随机获取一个直属分支</summary>
        TProbBranch GetRandmoDirectlyBranch();

        /// <summary>随机获取一个直属分支的直属概率项</summary>
        TProbItem GetRandmoDirectlyBranchItem();

        /// <summary>在所有内容中获取随机分支</summary>
        TProbBranch GetRandmoBranch();

        /// <summary>在所有内容中获取随机项</summary>
        TProbItem GetRandomItem();

        /// <summary>
        /// 查找一个符合条件的 概率项
        /// <para>注意：除非你确定其所有分支都没有相同项，否则不同分支有相同的，只会找到第一个</para>
        /// </summary>
        TProbItem FindItem(TValue value);

        /// <summary>
        /// 查找一个符合条件的 分支
        /// <para>注意：除非你确定其所有分支都没有相同项，否则不同分支有相同的，只会找到第一个</para>
        /// </summary>
        TProbBranch FindBranch(System.Func<TProbBranch, bool> condition);

        /// <summary>
        /// 获取对应直属真实概率
        /// <para>注意：除非你确定其所有分支都没有相同项，否则不同分支有相同的，只会找到第一个</para>
        /// </summary>
        float GetRealProb(TValue value);

        /// <summary>存在有效项</summary>
        bool HasValidItem();

        /// <summary>更新有效概率列表</summary>
        void UpdateValidProbs();

        /// <summary>清空此容器</summary>
        void Clear();
    }
}
