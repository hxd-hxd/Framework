// -------------------------
// 创建日期：2024/3/19 10:46:29
// -------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.Editor
{
    /// <summary>
    /// 加载编辑器资源文件夹中的资源
    /// <para>
    /// 功能和 <see cref="Resources"/> 类似，但只能加载包含在 Editor 文件夹中的下级 Resources 文件夹里的资源，
    /// </para>
    /// <para>
    /// 例如："Editor/Resources/Config/text.txt"、"Editor/Test/Resources/Config/text.txt"，都可以加载到 "Config/text.txt"
    /// </para>
    /// </summary>
    public class EditorResources
    {
        static List<string> _tempPaths = new List<string>();

        //static Dictionary<string, List<Object>> _resDic = new Dictionary<string, List<Object>>();

        ///// <summary>
        ///// 卸载资源
        ///// </summary>
        ///// <param name="resName"></param>
        ///// <returns></returns>
        //public static bool Unload(string resName)
        //{
        //    return _resDic.Remove(resName);
        //}
        ///// <summary>
        ///// 卸载所有资源
        ///// </summary>
        //public static void UnloadAll()
        //{
        //    _resDic.Clear();
        //}

        /// <summary>
        /// 加载第一个对应名称的资源
        /// <para>资源路径与 <see cref="Resources"/> 格式一样，需要完整名称（text.txt）</para>
        /// </summary>
        public static T Load<T>(string resName) where T : Object
        {
            Object obj = Load(resName, typeof(T));
            return obj as T;
        }

        /// <summary>
        /// 加载第一个对应名称的资源
        /// <para>资源路径与 <see cref="Resources"/> 格式一样，需要完整名称（text.txt）</para>
        /// </summary>
        public static Object Load(string resName, Type type)
        {
            Object result = null;

            //List<Object> ress = null;// 已加载并缓存的资源
            //if (_resDic.TryGetValue(resName, out ress))
            //{
            //    for (int i = 0; i < ress.Count; i++)
            //    {
            //        var item = ress[i];
            //        string p = AssetDatabase.GetAssetPath(item);
            //        // 检查资产是否 被移除 或 修改路径
            //        if (!item && p.EndsWith(resName))
            //        {
            //            result = item;
            //        }
            //    }
            //}

            if (!result)
            {
                if (GetFullPath(resName, out string resFileUnityPath))
                {
                    result = AssetDatabase.LoadAssetAtPath(resFileUnityPath, type);
                    //ress ??= new List<Object>();
                    //ress.Add(result);
                    //_resDic[resName] = ress;
                }
            }

            return result;
        }

        /// <summary>加载第一个对应类型的资源</summary>
        public static T Load<T>() where T : Object
        {
            T result = default;

            if (GetFullPath(typeof(T), out string resFileUnityPath))
            {
                result = AssetDatabase.LoadAssetAtPath<T>(resFileUnityPath);
            }
            return result;
        }

        /// <summary>加载第一个对应类型的资源</summary>
        public static Object Load(Type type)
        {
            Object result = null;

            if (GetFullPath(type, out string resFileUnityPath))
            {
                result = AssetDatabase.LoadAssetAtPath(resFileUnityPath, type);
            }
            return result;
        }

        /// <summary>加载所有同名资产
        /// <para>资源路径与 <see cref="Resources"/> 格式一样，需要完整名称（text.txt）</para>
        /// </summary>
        /// <returns>是否加载成功</returns>
        public static bool LoadAll<T>(string resName, ref List<T> assets) where T : Object
            => _LoadAll(resName, typeof(T), ref assets);

        /// <summary>加载所有同名资产
        /// <para>资源路径与 <see cref="Resources"/> 格式一样，需要完整名称（text.txt）</para>
        /// </summary>
        /// <returns>是否加载成功</returns>
        public static bool LoadAll(string resName, Type type, ref List<Object> assets)
            => _LoadAll(resName, type, ref assets);

        /// <summary>加载所有同类型资产</summary>
        /// <returns>是否加载成功</returns>
        public static bool LoadAll<T>(ref List<T> assets) where T : Object
            => _LoadAll(typeof(T), ref assets);

        /// <summary>加载所有同类型资产</summary>
        /// <returns>是否加载成功</returns>
        public static bool LoadAll(Type type, ref List<Object> assets)
            => _LoadAll(type, ref assets);

        /// <summary>加载指定目录下所有同类型资产</summary>
        /// <returns>是否加载成功</returns>
        public static bool LoadAllAtDir<T>(string resDir, ref List<T> assets) where T : Object
            => _LoadAllAtDir(resDir, typeof(T), ref assets);

        /// <summary>加载指定目录下所有同类型资产</summary>
        /// <returns>是否加载成功</returns>
        public static bool LoadAllAtDir(string resDir, Type type, ref List<Object> assets)
            => _LoadAllAtDir(resDir, type, ref assets);

        /// <summary>加载文件夹下的所有资产</summary>
        /// <remarks>Assets 文件夹路径，非 Editor/../Resources 相对路径</remarks>
        internal static List<T> LoadAssetsAtPath<T>(string folderPath) where T : Object
        {
            string[] assetGUIDs = AssetDatabase.FindAssets("t:Object", new string[] { folderPath });
            var assets = new List<T>();
            foreach (var guid in assetGUIDs)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset)
                {
                    assets.Add(asset);
                }
            }
            return assets;
        }

        /// <summary>加载文件夹下的所有资产路径</summary>
        /// <remarks>Assets 文件夹路径，非 Editor/../Resources 相对路径</remarks>
        internal static List<string> LoadAssetsPathAtPath<T>(string folderPath) where T : Object
        {
            string[] assetGUIDs = AssetDatabase.FindAssets("t:Object", new string[] { folderPath });
            var assets = new List<string>();
            foreach (var guid in assetGUIDs)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset)
                {
                    assets.Add(assetPath);
                }
            }
            return assets;
        }

        /// <summary>
        /// 获取 Unity 资产文件的全路径
        /// </summary>
        public static bool GetFullPath(string resName, out string assetPath)
        {
            _tempPaths.Clear();
            var paths = _tempPaths;
            bool result = _GetFullPath(resName, false, ref paths);
            assetPath = null;
            if (result)
            {
                assetPath = paths[0];
            }

            //TypePool.root.Return(paths);
            return result;
        }

        /// <summary>
        /// 获取 Unity 资产文件的全路径，将所有找到的路径存储在列表中
        /// </summary>
        /// <param name="assetsPath">找到的所有 <paramref name="resName"/> 资产路径</param>
        public static bool GetFullPath(string resName, ref List<string> assetsPath) => _GetFullPath(resName, true, ref assetsPath);

        /// <summary>获取指定类型 Unity 资产文件的全路径</summary>
        public static bool GetFullPath(Type type, out string assetPath)
        {
            _tempPaths.Clear();
            var paths = _tempPaths;
            bool result = _GetFullPath(Application.dataPath, null, type, false, ref paths);
            assetPath = null;
            if (result)
            {
                assetPath = paths[0];
            }

            return result;
        }

        /// <summary>获取指定类型 Unity 资产文件的全路径，将所有找到的路径存储在列表中</summary>
        public static bool GetFullPath(Type type, ref List<string> assetsPath) => _GetFullPath(Application.dataPath, null, type, true, ref assetsPath);

        /// <summary>获取指定类型 Unity 资产文件的全路径，将所有找到的路径存储在列表中</summary>
        public static bool GetFullPath(string resDir, Type type, ref List<string> assetsPath) => _GetFullPath(Application.dataPath, resDir, type, true, ref assetsPath);

        // 内部使用
        static bool _LoadAll<T>(string resName, Type type, ref List<T> assets) where T : Object
        {
            _tempPaths.Clear();
            var r = GetFullPath(resName, ref _tempPaths);
            if (r)
            {
                assets ??= new List<T>();
                foreach (var resFileUnityPath in _tempPaths)
                {
                    var obj = AssetDatabase.LoadAssetAtPath(resFileUnityPath, type);
                    if (obj) assets.Add(obj as T);
                }

                return assets.Count > 0;
            }

            return false;
        }
        static bool _LoadAll<T>(Type type, ref List<T> assets) where T : Object
        {
            return _LoadAllAtDir(null, type, ref assets);
        }
        static bool _LoadAllAtDir<T>(string resDir, Type type, ref List<T> assets) where T : Object
        {
            _tempPaths.Clear();
            var r = GetFullPath(resDir, type, ref _tempPaths);
            if (r)
            {
                assets ??= new List<T>();
                foreach (var resFileUnityPath in _tempPaths)
                {
                    var obj = AssetDatabase.LoadAssetAtPath(resFileUnityPath, type);
                    if (obj) assets.Add(obj as T);
                }

                return assets.Count > 0;
            }

            return false;
        }

        /// <summary>
        /// 获取指定 Unity 资产文件的全路径，将所有找到的路径存储在列表中
        /// </summary>
        /// <param name="findAll">true：找到所有资源，在不同目录下的相同资源  false：找到第一个</param>
        /// <returns>是否找到</returns>
        static bool _GetFullPath(string resName, bool findAll, ref List<string> assetsPath)
        {
            return _GetFullPath(Application.dataPath, resName, findAll, ref assetsPath);
        }
        // 递归查找
        static bool _GetFullPath(string dir, string resName, bool findAll, ref List<string> assetsPath)
        {
            bool result = false;
            //if (assetsPath != null) assetsPath.Clear();
            assetsPath ??= new List<string>(1);

            // 查找 Editor 文件夹
            var dirs = Directory.GetDirectories(dir);
            foreach (var ldir in dirs)
            {
                string dirName = Path.GetFileName(ldir);
                if (dirName == "Editor")
                {
                    // 查找 Resources 文件夹
                    func_res(ldir, ref assetsPath);
                }
                else
                {
                    result = result || _GetFullPath(ldir, resName, findAll, ref assetsPath);
                }

                if (result && !findAll) break;
            }

            return result;

            // 本地方法，查找 Resources 文件夹下的指定资源
            void func_res(string dir, ref List<string> assetsPath)
            {
                var edirs = Directory.GetDirectories(dir);
                foreach (var ledir in edirs)
                {
                    string edirName = Path.GetFileName(ledir);
                    if (edirName == "Resources")
                    {
                        // 资源文件的完整路径
                        string resFileFullPath = Path.Combine(ledir, resName);
                        // 查看指定资源文件是否存在
                        if (!File.Exists(resFileFullPath))
                        {
                            /* TODO：是否处理嵌套型文件
                                例如：
                                有以下两个文件路径
                                    1、Editor/Resources/Config/text.txt
                                    2、Editor/Resources/Resources/Config/text.txt
                                要找到文件 Config/text.txt，
                                其中 2 是否被算作资源，
                                还是在顶级 Editor/Resources/ 就被截断，
                                即 
                                    Config/text.txt
                                    Resources/Config/text.txt
                                是两个资源。

                            如果不处理嵌套型文件，就取消以下代码的注释
                            */

                            //result = _GetFullPath(ledir, resName, ref paths);
                            continue;
                        }
                        // 获取 Unity Assets 文件夹的路径
                        string resPath = resFileFullPath.Replace(Application.dataPath, "Assets");
                        assetsPath.Add(resPath);
                        result = true;
                    }
                    else
                    {
                        func_res(ledir, ref assetsPath);
                    }

                    if (result && !findAll) break;
                }
            }
        }
        // 查找指定文件夹下的所有指定类型的编辑器资源文件
        static bool _GetFullPath(string dir, string resDir, Type type, bool findAll, ref List<string> assetsPath)
        {
            bool result = false;
            //if (assetsPath != null) assetsPath.Clear();
            assetsPath ??= new List<string>(1);

            // 查找 Editor 文件夹
            var dirs = Directory.GetDirectories(dir);
            foreach (var ldir in dirs)
            {
                string dirName = Path.GetFileName(ldir);
                if (dirName == "Editor")
                {
                    // 查找 Resources 文件夹
                    func_res(ldir, ref assetsPath);
                }
                else
                {
                    result = result || _GetFullPath(ldir, resDir, type, findAll, ref assetsPath);
                }

                if (result && !findAll) break;
            }

            return result;

            // 本地方法，查找 Resources 文件夹下的指定类型资源
            void func_res(string dir, ref List<string> assetsPath)
            {
                var edirs = Directory.GetDirectories(dir);
                foreach (var ledir in edirs)
                {
                    string edirName = Path.GetFileName(ledir);
                    if (edirName == "Resources")
                    {
                        string tdir = ledir;
                        // 有没有指定文件夹
                        if (!string.IsNullOrEmpty(resDir))
                        {
                            tdir = Path.Combine(tdir, resDir);
                            if (!Directory.Exists(tdir)) continue;
                        }

                        // 按类型查找所有文件
                        func_type(tdir, ref assetsPath);
                    }
                    else
                    {
                        func_res(ledir, ref assetsPath);
                    }

                    if (result && !findAll) break;
                }
            }

            // 本地方法，查找目录下的类型资源
            void func_type(string dir, ref List<string> assetsPath)
            {
                // 按类型查找所有文件
                var rfiles = Directory.GetFiles(dir);
                foreach (var resFileFullPath in rfiles)
                {
                    // 获取 Unity Assets 文件夹的路径
                    string resPath = resFileFullPath.Replace(Application.dataPath, "Assets");
                    var rType = AssetDatabase.GetMainAssetTypeAtPath(resPath);
                    if (!type.IsAssignableFrom(rType)) continue;

                    assetsPath.Add(resPath);
                    result = true;
                    if (!findAll) return;
                }

                // 查找所有子文件夹
                var rdirs = Directory.GetDirectories(dir);
                foreach (var rdir in rdirs)
                {
                    func_type(rdir, ref assetsPath);
                }
            }
        }
    }
}