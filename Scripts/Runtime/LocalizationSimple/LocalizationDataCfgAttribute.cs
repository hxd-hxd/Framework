using System;
using UnityEngine;

namespace Framework.LocalizationSimple
{
    /// <summary>
    /// 控制 <see cref="LocalizationDataBase"/>（及其子类）在 Inspector 中 id / 语言相关字段的显示。
    /// 可标在直接字段或 List/数组等容器字段上（元素绘制时通过 fieldInfo 读取）。
    /// <para>仅作元数据，不要再为此特性编写 PropertyDrawer，否则会与类型抽屉叠绘。</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class LocalizationDataCfgAttribute : PropertyAttribute
    {
        public readonly LocalizationDataCfgMode mode;

        public LocalizationDataCfgAttribute(LocalizationDataCfgMode mode)
        {
            this.mode = mode;
        }
    }
}
