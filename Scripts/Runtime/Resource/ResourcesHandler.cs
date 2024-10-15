// -------------------------
// 创建日期：2022/10/27 10:12:12
// -------------------------

using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Framework
{
    /// <summary>
    /// Resources 资源管理器
    /// </summary>
    public class ResourcesHandler : IResourcesHandler
    {
        public T GetObject<T>(string path) where T : Object
        {
            throw new System.NotImplementedException();
        }

        public Object GetObject(string path)
        {
            throw new System.NotImplementedException();
        }
    }
}