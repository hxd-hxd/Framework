// -------------------------
// 创建日期：2023/5/11 15:47:57
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

namespace Framework.HybridCLRExpress
{
    [CreateAssetMenu(menuName = "Framework HybridCLRExpress/Assemblies Cfg", fileName = "Assemblies Cfg")]
    public class AssembliesCfg : ScriptableObject
    {
        [Header("保存 HybridCLR 生成程序集相关操作的设置")]
        [Header("热更新程序集作为资源存放的路径")]

        /// <summary>
        /// 要拷贝到的目标路径 
        /// </summary>
        [TextArea(2, 10)]
        public string copyPath = "Assets/HotUpdateAssemblies";
        /// <summary>
        /// 当前要使用的 dll 存放的路径名
        /// </summary>
        [Header("当前要使用的 dll 存放的路径名")]
        public string currentUsePathName = "Use";

        /// <summary>
        /// 当前要使用的 dll 存放的完整路径（<see cref="copyPath"/>/<see cref="currentUsePathName"/>）
        /// </summary>
        public string currentUsePath => Path.Combine(copyPath, currentUsePathName);

    }
}

#pragma warning restore 0414
#pragma warning restore 0219
