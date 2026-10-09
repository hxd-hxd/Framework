
namespace Framework.Prob
{
    /// <summary>
    /// 概率项接口
    /// </summary>
    /// <typeparam name="TValue">包含类型</typeparam>
    /// <typeparam name="TProbBranch">实现的分支类型</typeparam>
    /// <typeparam name="TProbItem">实现的概率类型</typeparam>
    public interface IItem<TValue, TProbBranch, TProbItem> : IProb
        where TProbBranch : IBranch<TValue, TProbBranch, TProbItem> 
        where TProbItem : IProb
    {
        /// <summary>所属容器</summary>
        IContainer<TValue, TProbBranch, TProbItem> owner { get; set; }
    }

}