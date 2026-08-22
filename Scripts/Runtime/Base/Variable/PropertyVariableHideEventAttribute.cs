using System;
using UnityEngine;

namespace Framework.Runtime
{
    /// <summary>
    /// 标在 <see cref="PropertyVariable{T}"/> 字段上，Inspector 中不绘制 <c>onChangeCallback</c>。
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class PropertyVariableHideEventAttribute : PropertyAttribute
    {
    }
}
