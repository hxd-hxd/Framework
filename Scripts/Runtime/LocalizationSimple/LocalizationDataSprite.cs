using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.LocalizationSimple
{
    /// <summary>本地化数据</summary>
    [Serializable]
    public class LocalizationDataSprite : LocalizationDataBase
    {
        /// <summary>精灵</summary>
        public Sprite _sprite;
    }
}