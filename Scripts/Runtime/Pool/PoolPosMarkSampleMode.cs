
namespace Framework.ObjectPool
{
    /// <summary>池采样方式</summary>
    public enum PoolPosMarkSampleMode
    {
        /// <summary>默认</summary>
        Default = 0,

        /// <summary>只记录最小值</summary>
        Min,

        /// <summary>记录每次采样的值</summary>
        Full,
    }
}
