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

        /// <summary>
        /// 添加一个分支
        /// <para>ps：不可添加同名分支</para>
        /// </summary>
        bool AddBranch(TProbBranch son);

        /// <summary>添加直属</summary>
        bool AddItem(TProbItem item);

        /// <summary>
        /// 随机获取一个直属概率项
        /// <para><paramref name="ignoreEnable"/>：忽视启用的影响</para>
        /// </summary>
        TProbItem GetRandmoDirectlyItem(bool ignoreEnable = false);

        /// <summary>
        /// 随机获取一个直属分支
        /// <para><paramref name="ignoreEnable"/>：忽视启用的影响</para>
        /// </summary>
        TProbBranch GetRandmoDirectlyBranch(bool ignoreEnable = false);

        /// <summary>
        /// 随机获取一个直属分支的直属概率项
        /// <para><paramref name="ignoreEnable"/>：忽视启用的影响</para>
        /// </summary>
        TProbItem GetRandmoDirectlyBranchItem(bool ignoreEnable = false);

        /// <summary>
        /// 随机获取一个分支，在所有下级分支中
        /// <para><paramref name="ignoreEnable"/>：忽视启用的影响</para>
        /// </summary>
        TProbBranch GetRandmoBranch(bool ignoreEnable = false);

        /// <summary>在所有分支中获取随机项</summary>
        TProbItem GetRandomItem(bool ignoreEnable = false);

        /// <summary>
        /// 查找一个符合条件的 概率项
        /// <para>ps：除非你确定其所有分支都没有相同项，否则不同分支有相同的，只会找到第一个</para>
        /// </summary>
        TProbItem FindItem(TValue include);

        /// <summary>
        /// 查找一个符合条件的 节点
        /// <para>ps：除非你确定其所有分支都没有相同项，否则不同分支有相同的，只会找到第一个</para>
        /// </summary>
        TProbBranch FindBranch(System.Func<TProbBranch, bool> condition);

        /// <summary>
        /// 获取对应直属真实概率
        /// <para><paramref name="ignoreEnable"/>：忽视启用的影响</para>
        /// <para>ps：除非你确定其所有分支都没有相同项，否则不同分支有相同的，只会找到第一个</para>
        /// </summary>
        float GetRealProb(TValue include, bool ignoreEnable = false);

        /// <summary>存在有效项</summary>
        bool HasValidItem(bool ignoreEnable = false);

        /// <summary>清空此容器</summary>
        void Clear();
    }
}
