// -------------------------
// 创建日期：2024/10/14 14:53:10
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;
using UnityEngine.AddressableAssets;

namespace Framework.AddressableExpress
{
    public class AddressableHandler : IResourcesHandler
    {
        public T GetObject<T>(string path) where T : UnityEngine.Object
        {
            //Addressables.DownloadDependenciesAsync
            return null;
        }

        public UnityEngine.Object GetObject(string path)
        {
            throw new NotImplementedException();
        }
    }
}