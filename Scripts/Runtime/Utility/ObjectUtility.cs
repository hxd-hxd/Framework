// -------------------------
// 创建日期：2024/11/15 16:25:04
// -------------------------

using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 处理对象的实用程序
    /// </summary>
    public static class ObjectUtility
    {
        /// <summary>
        /// 对象是否为空引用
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNull<T>(T obj) where T : class
        {
            if (obj is Object uobj)
            {
                return uobj == null;
            }
            return obj == null;
        }
    }
}