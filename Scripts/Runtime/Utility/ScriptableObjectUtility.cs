// -------------------------
// 创建日期：2023/5/11 18:31:00
// -------------------------

#pragma warning disable 0414
#pragma warning disable 0219

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Framework
{
    public static partial class ScriptableObjectUtility
    {

#if UNITY_EDITOR
        public static T Create<T>(bool select = true) where T : ScriptableObject
        {
            return Create<T>("Assets", select);
        }
        public static T Create<T>(string path, bool select = true) where T : ScriptableObject
        {
            return Create<T>(path, typeof(T).Name, select);
        }
        /// <summary>
        /// 创建指定的 <see cref="ScriptableObject"/> 资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_path"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T Create<T>(string _path, string name, bool select = true) where T : ScriptableObject
        {
            T so = ScriptableObject.CreateInstance<T>();

            string path = _path;

            if (File.Exists(path) && !Directory.Exists(path))
            {
                path = Path.GetDirectoryName(path);
            }
            if (string.IsNullOrEmpty(path)) path = "Assets";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            // 转换一下路径，确保是 unity 可用的资源路径
            path = PathUtility.GetUnityAssetPath(path);

            string assetPath = $"{path}/{name}.asset";
            if (File.Exists(assetPath))
                for (int i = 1; i < int.MaxValue; i++)
                {
                    assetPath = $"{path}/{name} {i}.asset";
                    if (!File.Exists(assetPath)) break;
                }
            AssetDatabase.CreateAsset(so, assetPath);

            if(select) 
                Selection.activeObject = so;

            AssetDatabase.Refresh();

            return so;
        }
#endif
    }
}