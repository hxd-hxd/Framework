using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.LocalizationSimple
{
    /// <summary>本地化数据</summary>
    [Serializable]
    public class LocalizationData<Data> : LocalizationDataBase
    {
        /// <summary>数据</summary>
        public Data _data;

        public override T GetData<T>()
        {
            if(_data is T t) return t;
            return default;
        }

        public override void SetData<T>(T data)
        {
            if(data is Data t) _data = t;
            else _data = default;
        }
    }
}