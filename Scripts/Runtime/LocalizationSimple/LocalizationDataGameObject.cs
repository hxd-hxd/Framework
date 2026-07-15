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
    }
}