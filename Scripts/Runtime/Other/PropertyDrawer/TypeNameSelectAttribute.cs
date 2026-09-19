using System;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 字符串字段的类型选择：提供按钮打开类型选择窗口，将选中类型的 <see cref="Type.FullName"/> 写入字段。
    /// <para>注意：不可与 <see cref="TextAreaAttribute"/> 一起使用</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class TypeNameSelectAttribute : PropertyAttribute
    {
        /// <summary>为 true 时按元素类型选择，写入对应的一维数组类型全名。</summary>
        public readonly bool arrayOnly;

        public TypeNameSelectAttribute() : this(false)
        {
        }

        public TypeNameSelectAttribute(bool arrayOnly)
        {
            this.arrayOnly = arrayOnly;
        }
    }
}
