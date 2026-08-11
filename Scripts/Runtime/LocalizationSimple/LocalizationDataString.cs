using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.LocalizationSimple
{
    /// <summary>本地化数据</summary>
    [Serializable]
    public class LocalizationDataString : LocalizationDataBase
    {
        /// <summary>文本</summary>
        [TextArea(1, 10)]
        public string _text;

        public override T GetData<T>()
        {
            return (T)(object)_text;
        }

        public override void SetData<T>(T data)
        {
            _text = (string)(object)data;
        }
    }
}