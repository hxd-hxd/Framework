// -------------------------
// 创建日期：2023/5/11 17:47:58
// -------------------------

#pragma warning disable 0414
#pragma warning disable 0219

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 处理路径
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
            string l_path = ToStandardPath(path);
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
            string l_path = ToStandardPath(path);
            return l_path.Contains(Application.dataPath);
        }

        /// <summary>
        /// 获取路径上级目录
        /// </summary>
        public static string GetDirectoryName(string path)
        {
            string r = path;
            r = Path.GetDirectoryName(r);
            r = ToStandardPath(r);
            return r;
        }

        /// <summary>
        /// 转换成标准路径
        /// </summary>
        public static string ToStandardPath(string path)
        {
            string r = path;
            r = r.Replace("\\", "/");
            return r;
        }

        /// <summary>
        /// 拼接路径
        /// </summary>
        public static string Combine(string path1, string path2)
        {
            string r = string.Empty;
            string p1 = path1;
            string p2 = path2;
            bool p1HasSymbol = path1.EndsWith("\\") || path1.EndsWith("/");
            bool p2HasSymbol = path2.StartsWith("\\") || path2.StartsWith("/");
            if (p1HasSymbol && p2HasSymbol)
            {
                // 都有分隔符，则留一个
                p2 = path2.Substring(1);
            }
            else
            if (!p1HasSymbol && !p2HasSymbol)
            {
                // 都没有分隔符，则加一个
                p2 = $"/{p2}";
            }
            // 其中一个有不用管，直接拼接
            r = $"{p1}{p2}";
            r = ToStandardPath(r);
            return r;
        }

        /// <summary>
        /// 拼接路径
        /// </summary>
        public static string Combine(string path1, string path2, string path3)
        {
            string r = string.Empty;
            r = Combine(path1, path2);
            r = Combine(r, path3);
            return r;
        }

        /// <summary>
        /// 拼接路径
        /// </summary>
        public static string Combine(string path1, string path2, string path3, string path4)
        {
            string r = string.Empty;
            r = Combine(path1, path2);
            r = Combine(r, path3);
            r = Combine(r, path4);
            return r;
        }

        /// <summary>
        /// 拼接路径
        /// </summary>
        public static string Combine(string path1, string path2, string path3, string path4, string path5)
        {
            string r = string.Empty;
            r = Combine(path1, path2);
            r = Combine(r, path3);
            r = Combine(r, path4);
            r = Combine(r, path5);
            return r;
        }

        /// <summary>
        /// 拼接路径
        /// </summary>
        public static string Combine(string path1, string path2, string path3, string path4, string path5, string path6)
        {
            string r = string.Empty;
            r = Combine(path1, path2);
            r = Combine(r, path3);
            r = Combine(r, path4);
            r = Combine(r, path5);
            r = Combine(r, path6);
            return r;
        }

        /// <summary>
        /// 拼接路径
        /// </summary>
        public static string Combine(params string[] paths)
        {
            string r = string.Empty;
            if (paths != null)
            {
                if (paths.Length > 0)
                {
                    r = paths[0];
                    for (int i = 1; i < paths.Length; i++)
                    {
                        r = Combine(r, paths[i]);
                    }
                }

            }
            return r;
        }
    }
}