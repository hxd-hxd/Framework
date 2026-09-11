using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Framework.Runtime
{
    /// <summary>位标器信息</summary>
    [Serializable]
    public class PositionMarkerInfo
    {
        /// <summary>清理周期，默认10分钟</summary>
        public float clearTime = 10 * 60;

        /// <summary>采样时间</summary>
        public float sampleTime = 1;

        public UnityEvent<Type> onSample;
        public UnityEvent<Type> onClear;
        public UnityEvent<Type, int> onSampleArray;
        public UnityEvent<Type, int> onClearArray;

        public void Clear()
        {
            clearTime = 10 * 60;
            sampleTime = 1;

            onSample?.RemoveAllListeners();
            onClear?.RemoveAllListeners();
            onSampleArray?.RemoveAllListeners();
            onClearArray?.RemoveAllListeners();
        }
    }
}
