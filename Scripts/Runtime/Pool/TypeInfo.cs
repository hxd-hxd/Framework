using System;
using System.Collections;
using System.Collections.Generic;
using Framework;
using Framework.Runtime;

namespace Framework.ObjectPool
{

    [Serializable]
    public class TypeInfo
    {
        [TypeNameSelect]
        public string typeName = typeof(object).FullName;

        public PositionMarkerInfo info;
    }

}
