using System;
using System.Collections.Generic;

namespace Framework.Core
{
    // 用于处理位标法采样
    /// <summary>位标器接口</summary>
    public interface IPositionMarker
    {
        /// <summary>清理周期</summary>
        float clearTime { get; set; }

        /// <summary>采样时间</summary>
        float sampleTime { get; set; }

        /// <summary>采样次数</summary>
        int sampleCount { get; }

        /// <summary>更新
        /// <para></para><paramref name="elapseTime"/>：流逝的时间
        /// <para></para><paramref name="realElapseTime"/>：真实流失的时间
        /// </summary>
        void Update(float elapseTime, float realElapseTime);

        /// <summary>采样</summary>
        void Sample();

        /// <summary>清理</summary>
        void Clear();
    }
}
