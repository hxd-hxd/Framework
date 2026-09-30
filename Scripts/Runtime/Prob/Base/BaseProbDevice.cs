
namespace Framework.Prob
{
    /// <summary>
    /// 概率生成器
    /// <para>可以这么理解原理：<see cref="ProbDevice"/> 生成器类似磁盘，<see cref="ProbBranch"/> 分支类似文件夹，<see cref="ProbItem"/> 类似各种文件</para>
    /// <para>就是随机在文件夹里找一个文件，根据你设定的 probValue 大小，其被随机到的概率也不同</para>
    /// <para><see cref="IProb.probValue"/> 概率值可以是任何大于零的数，但是推荐自己提前计算好值，然后填入，以免出现概率问题而难以追踪</para>
    /// </summary>
    public abstract class BaseProbDevice<TValue, TProbBranch, TProbItem> : 
        BaseContainer<TValue, TProbBranch, TProbItem>
        where TProbBranch : BaseProbBranch<TValue, TProbBranch, TProbItem>
        where TProbItem : BaseProbItem<TValue, TProbBranch, TProbItem>
    {
        
    }
}
