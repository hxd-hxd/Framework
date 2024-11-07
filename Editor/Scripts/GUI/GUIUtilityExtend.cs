// -------------------------
// 创建日期：2023/5/15 18:42:33
// -------------------------

#pragma warning disable 0414
#pragma warning disable 0219

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Framework.Editor
{
    /// <summary>
    /// Unity GUI 图标
    /// </summary>
    public static class GUIUtilityExtend
    {
        #region 图标

        /// <summary>
        /// 获取图标
        /// </summary>
        public static class IconNew
        {
            /// <summary>默认资产</summary>
            public static GUIContent DefaultAsset => GetIcon("d_DefaultAsset Icon");

            // 文本
            public static GUIContent TextAsset => GetIcon("d_TextAsset Icon");

            // 文件夹
            public static GUIContent Folder => GetIcon("d_Folder Icon");
            public static GUIContent FolderOpened => GetIcon("d_FolderOpened Icon");
            public static GUIContent FolderEmpty => GetIcon("d_FolderEmpty Icon");
            public static GUIContent FolderEmptyOn => GetIcon("d_FolderEmpty On Icon");

            // 脚本
            public static GUIContent ScriptableObject => GetIcon("d_ScriptableObject Icon");
            public static GUIContent ScriptableObjectOn => GetIcon("d_ScriptableObject On Icon");
            public static GUIContent TextScript => GetIcon("d_TextScriptImporter Icon");
            public static GUIContent CSScript => GetIcon("d_cs Script Icon");

            /// <summary>程序集动态库</summary>
            public static GUIContent Assembly => GetIcon("d_Assembly Icon");
            /// <summary>程序集定义资产</summary>
            public static GUIContent AssemblyDefinitionAsset => GetIcon("d_AssemblyDefinitionAsset Icon");
            /// <summary>程序集定义引用资产</summary>
            public static GUIContent AssemblyDefinitionReferenceAsset => GetIcon("d_AssemblyDefinitionReferenceAsset Icon");

            public static GUIContent GetIcon(string name)
            {
                //Debug.Log("获取图标");
                return GUIUtilityExtend.GetUnityIconNew(name);
            }
        }

        ///// <summary>
        ///// 图标新实例
        ///// </summary>
        //public static class IconNew
        //{
        //    /// <summary>默认资产</summary>
        //    public static GUIContent DefaultAsset => GetIcon("d_DefaultAsset Icon");

        //    // 文本
        //    public static GUIContent TextAsset => GetIcon("d_TextAsset Icon");

        //    // 文件夹
        //    public static GUIContent Folder => GetIcon("d_Folder Icon");
        //    public static GUIContent FolderOpened => GetIcon("d_FolderOpened Icon");
        //    public static GUIContent FolderEmpty => GetIcon("d_FolderEmpty Icon");
        //    public static GUIContent FolderEmptyOn => GetIcon("d_FolderEmpty On Icon");

        //    // 脚本
        //    public static GUIContent ScriptableObject => GetIcon("d_ScriptableObject Icon");
        //    public static GUIContent ScriptableObjectOn => GetIcon("d_ScriptableObject On Icon");
        //    public static GUIContent TextScript => GetIcon("d_TextScriptImporter Icon");
        //    public static GUIContent CSScript => GetIcon("d_cs Script Icon");

        //    /// <summary>程序集定义资产</summary>
        //    public static GUIContent AssemblyDefinitionAsset => GetIcon("d_AssemblyDefinitionAsset Icon");

        //    public static GUIContent GetIcon(string name)
        //    {
        //        //Debug.Log("获取图标新实例");
        //        return GUIUtilityExtend.GetUnityIconNew(name);
        //    }
        //}

        #endregion

        private static List<GUIContent> _unityIcons;
        private static Dictionary<string, GUIContent> _unityIconDic;
        /// <summary>
        /// Unity 内置图标的缓存列表
        /// <para>等同于 <see cref="GetUnityIcons"/> ，不过第一次获取后会缓存下来</para>
        /// </summary>
        public static List<GUIContent> unityIcons
        {
            get
            {
                if (_unityIcons == null) _unityIcons = GetUnityIcons();
                return _unityIcons;
            }
        }
        /// <summary>
        /// Unity 内置图标的缓存字典
        /// <para>等同于 <see cref="GetUnityIconDic"/> ，不过第一次获取后会缓存下来</para>
        /// </summary>
        public static Dictionary<string, GUIContent> unityIconDic
        {
            get
            {
                if (_unityIconDic == null) _unityIconDic = GetUnityIconDic();
                return _unityIconDic;
            }
        }

        /// <summary>
        /// 获取 GUI 图标列表
        /// </summary>
        /// <returns></returns>
        public static List<GUIContent> GetUnityIcons()
        {
            var icons = new List<GUIContent>();
            Texture2D[] textures = Resources.FindObjectsOfTypeAll<Texture2D>();
            foreach (Texture2D texture in textures)
            {
                GUIContent icon = null;
                //icon = EditorGUIUtility.IconContent(texture.name, $"|{texture.name}");
                //icon = EditorGUIUtility.TrIconContent(texture.name);
                icon = new GUIContent(null, texture, texture.name);

                if (icon != null && icon.image != null)
                {
                    if (icons.Find(item => item.tooltip == texture.name
                    //|| (item.tooltip.StartsWith("d_") && item.tooltip.Substring(2) == texture.name)
                    ) == null)
                        icons.Add(icon);
                }
            }

            // 按照英文字母排序 
            icons.Sort((x, y) => string.Compare(x.tooltip, y.tooltip));
            return icons;
        }
        /// <summary>
        /// 获取 GUI 图标字典
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, GUIContent> GetUnityIconDic()
        {
            var icons = new Dictionary<string, GUIContent>();
            Texture2D[] textures = Resources.FindObjectsOfTypeAll<Texture2D>();
            foreach (Texture2D texture in textures)
            {
                GUIContent icon = null;
                //icon = EditorGUIUtility.IconContent(texture.name, $"|{texture.name}");
                //icon = EditorGUIUtility.TrIconContent(texture.name);
                icon = new GUIContent(null, texture, texture.name);

                if (icon != null && icon.image != null)
                {
                    //icon.text = texture.name;
                    if (!icons.ContainsKey(texture.name))
                        icons.Add(texture.name, icon);
                }
            }
            return icons;
        }

        /// <summary>
        /// 获取对应名字的 Unity 内置 <see cref="GUIContent"/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static GUIContent GetUnityIcon(string name)
        {
            GUIContent icon = null;
            if (unityIconDic.ContainsKey(name))
            {
                icon = unityIconDic[name];
            }
            else
            {
                icon = new GUIContent();
            }
            return icon;
        }
        /// <summary>
        /// 获取对应名字 <see cref="GUIContent"/> 新实例
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static GUIContent GetUnityIconNew(string name)
        {
            GUIContent icon = GetUnityIcon(name);
            GUIContent newIcon = new GUIContent(icon);
            return newIcon;
        }

        public static void Clear()
        {
            //_unityIcons.Clear();
            //_unityIconDic.Clear();

            _unityIcons = null;
            _unityIconDic = null;
        }

    }
}