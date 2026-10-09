
namespace Framework.Prob
{
    /// <summary>随机提供者接口</summary>
    public interface IRandomProvider
    {
        /// <summary>随机数种子</summary>
        int seed { get; set; }

        /// <summary>获取随机概率值，0 到 100 之间</summary>
        float RandomProbValue();
    }
}
