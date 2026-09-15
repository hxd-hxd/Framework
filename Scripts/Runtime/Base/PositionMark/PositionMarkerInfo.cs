using System;
using System.Collections.Generic;
using UnityEngine;
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

        public UnityEvent<Type> onSample = new UnityEvent<Type>();
        public UnityEvent<Type> onClear = new UnityEvent<Type>();
        public UnityEvent<Type, int> onSampleArray = new UnityEvent<Type, int>();
        public UnityEvent<Type, int> onClearArray = new UnityEvent<Type, int>();

        public UnityEvent<GameObject> onSampleGO = new UnityEvent<GameObject>();
        public UnityEvent<GameObject> onClearGO = new UnityEvent<GameObject>();

        public void Clear()
        {
            clearTime = 10 * 60;
            sampleTime = 1;

            onSample?.RemoveAllListeners();
            onClear?.RemoveAllListeners();
            onSampleArray?.RemoveAllListeners();
            onClearArray?.RemoveAllListeners();

            onSampleGO?.RemoveAllListeners();
            onClearGO?.RemoveAllListeners();
        }
    }
}
