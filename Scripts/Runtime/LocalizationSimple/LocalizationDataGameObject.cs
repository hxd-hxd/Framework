using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Framework.LocalizationSimple.LocalizationDataSelectableSprite;

namespace Framework.LocalizationSimple
{
    /// <summary>本地化数据</summary>
    [Serializable]
    public class LocalizationDataGameObject : LocalizationDataBase
    {
        /// <summary>游戏对象</summary>
        public GameObject _gameObject;

        public override T GetData<T>()
        {
            return (T)(object)_gameObject;
        }

        public override void SetData<T>(T data)
        {
            _gameObject = (GameObject)(object)data;
        }

    }
}