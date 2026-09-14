using System;
using UnityEngine;
using UnityEngine.Events;

namespace Framework.Runtime
{
    /// <summary>位标器 Unity 版</summary>
    [Serializable]
    public class PositionMarker : Core.IPositionMarker, ITypePoolObject
    {
        [SerializeField]
        private PositionMarkerInfo _info = new PositionMarkerInfo();
        [SerializeReference]
        private PositionMarkerInfo _overrideInfo;

        [SerializeField]
        private int _sampleCount;// 采样次数

        [SerializeField]
        private float _clearTimeer;// 清理周期计时器
        [SerializeField]
        private float _sampleTimeer;// 采样时间计时器

        /// <summary>采样事件</summary>
        public UnityEvent<PositionMarker> onSample = new UnityEvent<PositionMarker>();
        /// <summary>清理事件</summary>
        public UnityEvent<PositionMarker> onClear = new UnityEvent<PositionMarker>();

        public float clearTime { get => GetInfo().clearTime; set => GetInfo().clearTime = value; }

        public float sampleTime { get => GetInfo().sampleTime; set => GetInfo().sampleTime = value; }

        public int sampleCount { get => _sampleCount; set => _sampleCount = value; }
        public PositionMarkerInfo overrideInfo { get => _overrideInfo; set => _overrideInfo = value; }

        public virtual void Update(float elapseTime, float realElapseTime)
        {
            _clearTimeer += realElapseTime;
            _sampleTimeer += realElapseTime;

            if (_sampleTimeer >= sampleTime)
            {
                _sampleTimeer = 0;
                Sample();
            }

            if (_clearTimeer >= clearTime)
            {
                _clearTimeer = 0;
                Clear();
            }
        }

        public virtual void Sample()
        {
            _sampleCount++;
            onSample?.Invoke(this);
        }

        public virtual void Reset()
        {
            _sampleCount = 0;
            _clearTimeer = _sampleTimeer = 0;
        }

        public virtual void Clear()
        {
            _sampleCount = 0;
            onClear?.Invoke(this);
        }

        private PositionMarkerInfo GetInfo()
        {
            return _overrideInfo ?? _info;
        }

        void ITypePoolObject.Clear()
        {
            _info.Clear();
            _overrideInfo = null;

            _sampleCount = 0;
            _clearTimeer = _sampleTimeer = 0;
            onSample?.RemoveAllListeners();
            onClear?.RemoveAllListeners();
        }
    }
}
