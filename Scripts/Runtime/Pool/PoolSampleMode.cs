
namespace Framework.ObjectPool
{
    /// <summary>池采样方式</summary>
    public enum PoolSampleMode
    {
        /// <summary>默认</summary>
        Default = 0,

        /// <summary>最小采样，只记录最小值</summary>
        Min,

        /// <summary>全量采样，记录每次采样的值</summary>
        Full,
    }
}
