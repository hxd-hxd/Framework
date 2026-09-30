using System;
using System.Collections;
using System.Collections.Generic;
using Framework;
using Framework.Runtime;
using UnityEngine;

namespace Framework.ObjectPool
{

    [Serializable]
    public class ArrayMarkerInfo
    {
        /// <summary><see cref="Array"/> 类型名</summary>
        [TypeNameSelect(true)]
        public string typeName = typeof(object[]).FullName;

        public int length;

        public PositionMarkerInfo info;
    }

}
