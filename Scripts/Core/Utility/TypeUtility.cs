using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Core
{
    /// <summary>类型工具类</summary>
    public class TypeUtility
    {
        public static bool As<Tin, Tout>(Tin value, ref Tout result)
        {
            if (value is Tout)
            {
                result = (Tout)(object)value;
                return true;
            }
            return false;
        }
    }
}
