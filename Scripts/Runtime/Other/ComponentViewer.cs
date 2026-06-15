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

        // 目标设置折叠
        [HideInInspector]
        public bool cTargetSettingsFoldout = true;
        // 字段折叠
        [HideInInspector]
        public bool fieldFoldout = true;
        // 属性折叠
        [HideInInspector]
        public bool propertyFoldout = true;
        // 显示不支持的成员
        [HideInInspector]
        public bool showNonsupportMember = true;
        // 显示不支持的成员
        [HideInInspector]
        public bool showInheritRelation = true;
        // GUI 最大深度
        [HideInInspector]
        public int maxDepth = 10;
        // 最小、最大文本显示行数
        [HideInInspector]
        public int minTextLine = 1, maxTextLine = 5;

        public Component target { get => _target; set => _target = value; }

        //public T GetFieldValue<T>(string name)
        //{
        //    return default;
        //}
    }
}