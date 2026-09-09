using System;
using System.Collections.Generic;

namespace Framework.Core
{
    // 用于处理位标法采样
    /// <summary>位标器</summary>
    [Serializable]
    public class PositionMarker : IPositionMarker
    {
        private float _clearTime = 10 * 60;// 清理周期，默认10分钟
        private float _sampleTime = 1;// 采样时间

        private int _sampleCount;// 采样次数

        private float _clearTimeer;// 清理周期计时器
        private float _sampleTimeer;// 采样时间计时器

        /// <summary>采样事件</summary>
        public event Action onSample;
        /// <summary>清理事件</summary>
        public event Action onClear;

        public float clearTime { get => _clearTime; set => _clearTime = value; }

        public float sampleTime { get => _sampleTime; set => _sampleTime = value; }

        public int sampleCount { get => _sampleCount; set => _sampleCount = value; }

        public virtual void Update(float elapseTime, float realElapseTime)
        {
            _clearTimeer += realElapseTime;
            _sampleTimeer += realElapseTime;

            if (_sampleTimeer >= _sampleTime)
            {
                _sampleTimeer = 0;
                Sample();
            }

            if (_clearTimeer >= _clearTime)
            {
                _clearTimeer = 0;
                Clear();
            }
        }

        public virtual void Sample()
        {
            _sampleCount++;
            onSample?.Invoke();
        }

        public virtual void Clear()
        {
            _sampleCount = 0;
            onClear?.Invoke();
        }
    }
}
