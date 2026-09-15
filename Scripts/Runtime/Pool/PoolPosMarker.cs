using System.Collections;
using Framework.Runtime;

namespace Framework.ObjectPool
{
    /// <summary>内部位标器定义</summary>
    internal class PoolPosMarker : ITypePoolObject
    {
        /// <summary>专用位标器</summary>
        public PositionMarker marker;

        /// <summary>采样器</summary>
        public PoolSampler sampler = new PoolSampler();

        public void Reset()
        {
            if (marker != null) marker.Reset();
            sampler.Clear();
        }

        public void RemoveMarker()
        {
            if (marker != null)
            {
                InternalTypePool.root.Return(marker);
                marker = null;
            }
        }

        public void Clear()
        {
            RemoveMarker();

            sampler.Clear();
        }
    }
}