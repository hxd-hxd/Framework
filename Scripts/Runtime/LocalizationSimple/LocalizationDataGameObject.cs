using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.LocalizationSimple
{
    /// <summary>本地化数据</summary>
    [Serializable]
    public class LocalizationDataGameObject : LocalizationDataBase
    {
        /// <summary>游戏对象</summary>
        public GameObject _gameObject;

        //public override T GetData<T>()
        //{
        //    return (T)(object)_gameObject;
        //}

        //public override void SetData<T>(T data)
        //{
        //    _gameObject = (GameObject)(object)data;
        //}

        public override T GetData<T>()
        {
            if (_gameObject is T t) return t;
            return default;
        }

        public override void SetData<T>(T data)
        {
            if (data is GameObject t) _gameObject = t;
            else _gameObject = default;
        }
    }
}