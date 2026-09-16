using System;
using System.Collections;
using System.Collections.Generic;
using Framework;
using Framework.Runtime;
using UnityEngine;

namespace Framework.ObjectPool
{

    [Serializable]
    public class TypeMarkerInfo
    {
        [TypeNameSelect]
        public string typeName = typeof(object).FullName;

        public PositionMarkerInfo info;
    }

}
