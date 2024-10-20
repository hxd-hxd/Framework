// -------------------------
// 创建日期：2023/5/11 17:47:58
// -------------------------

#pragma warning disable 0414
#pragma warning disable 0219

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

namespace Framework
{
    /// <summary>
    /// 处理文件路径
    /// </summary>
    public static class PathUtility
    {
        /// <summary>
        /// 将路径转换成 Unity Assets 资源路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetUnityAssetPath(string path)
        {
            string l_path = path.Replace("\\", "/");
            //string[] strs = Regex.Split(l_path, "Assets/");
            //string assetsPath = $"Assets/{strs[strs.Length - 1]}";
            string assetsPath = IsUnityAssetPath(path) ?
                $"Assets{l_path.Replace(Application.dataPath, null)}" :
                l_path;
            return assetsPath;
        }
        /// <summary>
        /// 判断是否 Unity Assets 资源路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsUnityAssetPath(string path)
        {
            string l_path = path.Replace("\\", "/");
            return l_path.Contains(Application.dataPath);
        }


    }
}