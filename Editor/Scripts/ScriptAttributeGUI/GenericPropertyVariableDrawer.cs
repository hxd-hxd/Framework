using System;
using System.Reflection;

using UnityEditor;
using UnityEngine;
using Framework.Runtime;

namespace Framework.Editor
{
    [CustomPropertyDrawer(typeof(PropertyVariable<>))]
    public class GenericPropertyVariableDrawer : LineCountPropertyDrawer
    {
        private const float FieldSpacing = 2f;

        public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(pos, property, label);
            label = EditorGUI.BeginProperty(pos, label, property);

            var value = property.FindPropertyRelative("_value");
            var onChangeCallback = property.FindPropertyRelative("_onChangeCallback");

            float y = pos.y;
            if (ShouldSplitFoldoutHeader(value, label))
            {
                float headerHeight = singleLineHeight;
                value.isExpanded = EditorGUI.Foldout(
                    new Rect(pos.x, y, pos.width, headerHeight),
                    value.isExpanded,
                    label,
                    true);
                y += headerHeight;

                if (value.isExpanded)
                {
                    float contentHeight = GetSplitValueContentHeight(value, pos.width);
                    DrawSplitValueContent(new Rect(pos.x, y, pos.width, contentHeight), value);
                    y += contentHeight;
                }
            }
            else
            {
                float valueHeight = GetValueHeight(value, label, pos.width);
                DrawValue(new Rect(pos.x, y, pos.width, valueHeight), value, label);
                y += valueHeight;
            }

            if (onChangeCallback != null && ShouldDrawOnChangeCallback())
            {
                y += FieldSpacing;
                float callbackHeight = EditorGUI.GetPropertyHeight(onChangeCallback, true);
                EditorGUI.indentLevel++;
                PropertyField(new Rect(pos.x, y, pos.width, callbackHeight), onChangeCallback, true);
                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            lineCount = 0;
            // 传 0：优先复用转发 Drawer 上一帧 OnGUI 缓存的真实宽度，避免估算宽覆盖导致换行不一致
            propertyHeight = MeasureStackHeight(property, label, 0f);
            return totalHeight;
        }

        /// <summary>
        /// 垂直栈：可选 Foldout 标题行 + value 内容 + 可选 callback。
        /// 拆标题时总高按「标题一行 + none 口径内容」分配，与绘制一致，避免 hWith 多算一行。
        /// </summary>
        protected float MeasureStackHeight(SerializedProperty property, GUIContent label, float width)
        {
            var value = property.FindPropertyRelative("_value");
            var onChangeCallback = property.FindPropertyRelative("_onChangeCallback");

            float height;
            if (ShouldSplitFoldoutHeader(value, label))
            {
                height = singleLineHeight;
                if (value.isExpanded)
                    height += GetSplitValueContentHeight(value, width);
            }
            else
            {
                height = GetValueHeight(value, label, width);
            }

            if (onChangeCallback != null && ShouldDrawOnChangeCallback())
            {
                height += FieldSpacing;
                height += EditorGUI.GetPropertyHeight(onChangeCallback, true);
            }

            return height;
        }

        /// <summary>
        /// 拆出 Foldout 标题后的内容高度：转发 Drawer 以 none 测高，否则只计子属性。
        /// </summary>
        protected float GetSplitValueContentHeight(SerializedProperty value, float width)
        {
            if (PropertyAttributeForwardingUtility.TryGetForwardedDrawer(fieldInfo, value, out _))
                return GetValueHeight(value, GUIContent.none, width);
            return GetChildrenOnlyHeight(value);
        }

        protected void DrawSplitValueContent(Rect rect, SerializedProperty value)
        {
            if (PropertyAttributeForwardingUtility.TryGetForwardedDrawer(fieldInfo, value, out var valueDrawer))
            {
                valueDrawer.OnGUI(rect, value, GUIContent.none);
                return;
            }

            DrawChildrenOnly(rect, value);
        }

        protected float GetValueHeight(SerializedProperty value, GUIContent valueLabel, float width)
        {
            if (PropertyAttributeForwardingUtility.TryGetForwardedDrawer(fieldInfo, value, out var valueDrawer))
            {
                if (valueDrawer is IWidthAwarePropertyDrawer widthAware)
                    return widthAware.GetPropertyHeight(value, valueLabel, width);
                return valueDrawer.GetPropertyHeight(value, valueLabel);
            }

            return EditorGUI.GetPropertyHeight(value, valueLabel, true);
        }

        protected void DrawValue(Rect rect, SerializedProperty value, GUIContent valueLabel)
        {
            if (PropertyAttributeForwardingUtility.TryGetForwardedDrawer(fieldInfo, value, out var valueDrawer))
            {
                valueDrawer.OnGUI(rect, value, valueLabel);
                return;
            }

            PropertyField(rect, value, valueLabel);
        }

        /// <summary>仅绘制子属性，不对父级 PropertyField(none)，避免第二层无标题折页。</summary>
        protected void DrawChildrenOnly(Rect rect, SerializedProperty value)
        {
            if (value == null || !value.hasVisibleChildren)
                return;

            var child = value.Copy();
            var end = value.GetEndProperty();
            if (!child.NextVisible(true))
                return;

            float y = rect.y;
            float spacing = EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.indentLevel++;
            while (!SerializedProperty.EqualContents(child, end))
            {
                float h = EditorGUI.GetPropertyHeight(child, null, true);
                EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, h), child, true);
                y += h + spacing;
                if (!child.NextVisible(false))
                    break;
            }
            EditorGUI.indentLevel--;
        }

        /// <summary>展开后仅子属性的高度（不含折页标题行）。</summary>
        protected float GetChildrenOnlyHeight(SerializedProperty value)
        {
            if (value == null || !value.hasVisibleChildren)
                return 0f;

            var child = value.Copy();
            var end = value.GetEndProperty();
            if (!child.NextVisible(true))
                return 0f;

            float height = 0f;
            float spacing = EditorGUIUtility.standardVerticalSpacing;
            while (!SerializedProperty.EqualContents(child, end))
            {
                height += EditorGUI.GetPropertyHeight(child, null, true) + spacing;
                if (!child.NextVisible(false))
                    break;
            }

            if (height > 0f)
                height -= spacing;
            return height;
        }

        /// <summary>是否绘制 <c>onChangeCallback</c></summary>
        protected virtual bool ShouldDrawOnChangeCallback()
        {
            return fieldInfo == null
                || fieldInfo.GetCustomAttribute<PropertyVariableHideEventAttribute>(true) == null;
        }

        /// <summary>
        /// 折页类 <c>_value</c> 同时传外层 label 时，Unity 会多算约一行；
        /// 拆成一次 Foldout（箭头+标题同行）+ 仅内容。无可见子属性则不拆，避免误伤单行类型。
        /// </summary>
        protected virtual bool ShouldSplitFoldoutHeader(SerializedProperty value, GUIContent outerLabel)
        {
            if (value == null || outerLabel == null || string.IsNullOrEmpty(outerLabel.text))
                return false;
            if (!value.hasVisibleChildren)
                return false;

            float hWith = EditorGUI.GetPropertyHeight(value, outerLabel, true);
            float hWithout = EditorGUI.GetPropertyHeight(value, GUIContent.none, true);
            float extra = hWith - hWithout;
            if (extra < singleLineHeight * 0.9f)
                return false;

            if (hWithout > singleLineHeight + 0.5f)
                return true;

            // 折叠后 hWithout 只剩约一行，仍按折页类型拆，避免标题与事件之间空行
            return IsFoldoutLikeProperty(value);
        }

        /// <summary>
        /// 真正走折页绘制的类型。Vector2/3 等虽有子字段，但是多字段单行，不能当折页拆。
        /// </summary>
        protected static bool IsFoldoutLikeProperty(SerializedProperty value)
        {
            if (value.isArray && value.propertyType != SerializedPropertyType.String)
                return true;

            var t = value.propertyType;
            return t == SerializedPropertyType.Generic
                || t == SerializedPropertyType.ManagedReference
                || t == SerializedPropertyType.Vector4;
        }

        protected virtual void PropertyField(Rect pos, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(pos, property, label, true);
        }

        protected virtual void PropertyField(Rect pos, SerializedProperty property, bool includeChildren)
        {
            EditorGUI.PropertyField(pos, property, includeChildren);
        }
    }
}
