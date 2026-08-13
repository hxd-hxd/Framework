using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Framework.LocalizationSimple.LocalizationDataSelectableSprite;

namespace Framework.LocalizationSimple
{
    /// <summary>本地化数据</summary>
    [Serializable]
    public class LocalizationDataSprite : LocalizationDataBase
    {
        /// <summary>精灵</summary>
        public Sprite _sprite;

        //public override T GetData<T>()
        //{
        //    return (T)(object)_sprite;
        //}

        //public override void SetData<T>(T data)
        //{
        //    _sprite = (Sprite)(object)data;
        //}

        public override T GetData<T>()
        {
            if (_sprite is T t) return t;
            return default;
        }

        public override void SetData<T>(T data)
        {
            if (data is Sprite t) _sprite = t;
            else _sprite = default;
        }

    }
}