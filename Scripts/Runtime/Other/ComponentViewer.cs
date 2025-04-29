// -------------------------
// 创建日期：2023/4/11 15:27:24
// -------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 组件查看器
    /// </summary>
    public class ComponentViewer : MonoBehaviour
    {
        [SerializeField]
        private Component _target;
        [NonSerialized]
        public int _num;

        [HideInInspector]
        public bool cTargetSettingsFoldout = true;
        [HideInInspector]
        public bool showNonsupportMember = true;
        [HideInInspector]
        public bool showInheritRelation = true;
        [HideInInspector]
        public int maxDepth = 10, minTextLine = 1, maxTextLine = 5;

        public Component target { get => _target; set => _target = value; }

        //public T GetFieldValue<T>(string name)
        //{
        //    return default;
        //}
    }
}