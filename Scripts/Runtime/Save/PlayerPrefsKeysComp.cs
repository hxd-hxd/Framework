using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Core;
using UnityEngine;

namespace Framework.Save
{
    /// <summary>提供 <see cref="PlayerPrefs"/> 所需键名</summary>
    public class PlayerPrefsKeysComp : MonoBehaviour
    {
        [SerializeField]
        private List<Key> _keys;

        public List<Key> keys { get => _keys; set => _keys = value; }

        [Serializable]
        public class Key
        {
            /// <summary>键名</summary>
            public string key;

            /// <summary>值的类型名，用于序列化</summary>
            [TypeNameSelect]
            public string valueTypeName = string.Empty;

            /// <summary>值的类型</summary>
            public Type valueType
            { 
                get => TypeCache.Get(valueTypeName);
                set => valueTypeName = (value != null ? value.FullName : string.Empty);
            }
        }
    }
}
