using System;
using System.Collections.Generic;
using System.Reflection;
using Framework.Runtime;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    /// <summary>
    /// 测高依赖实际绘制宽度的 PropertyDrawer（如 HelpBox 换行）。
    /// </summary>
    public interface IWidthAwarePropertyDrawer
    {
        float GetPropertyHeight(SerializedProperty property, GUIContent label, float width);
    }

    /// <summary>
    /// 将外层字段上的 <see cref="PropertyAttribute"/>（已注册 PropertyDrawer）转发到子 <see cref="SerializedProperty"/> 绘制。
    /// </summary>
    internal static class PropertyAttributeForwardingUtility
    {
        private struct FieldDrawerSpec
        {
            public Type DrawerType;
            public PropertyAttribute Attribute;
        }

        private static readonly HashSet<Type> SkipAttributeTypes = new HashSet<Type>
        {
            typeof(PropertyVariableHideEventAttribute),
            typeof(PropertyVariableValueAttribute),
        };

        private static readonly FieldInfo DrawerFieldInfoField;
        private static readonly FieldInfo DrawerAttributeField;

        /// <summary>特性 → 真正的 Drawer 类型；value 为 null 表示确认没有 PropertyDrawer（负缓存）。</summary>
        private static readonly Dictionary<Type, Type> s_drawerTypeByAttribute = new Dictionary<Type, Type>();

        /// <summary>字段 → 转发规格；null 表示该字段没有可转发 Drawer。</summary>
        private static readonly Dictionary<FieldInfo, FieldDrawerSpec?> s_fieldSpecCache =
            new Dictionary<FieldInfo, FieldDrawerSpec?>();

        /// <summary>按 FieldInfo 缓存 Drawer 实例，保证同字段 GetPropertyHeight / OnGUI 共用宽度状态。</summary>
        private static readonly Dictionary<FieldInfo, PropertyDrawer> s_drawerInstanceCache =
            new Dictionary<FieldInfo, PropertyDrawer>();

        static PropertyAttributeForwardingUtility()
        {
            var drawerType = typeof(PropertyDrawer);
            DrawerFieldInfoField = drawerType.GetField("m_FieldInfo", BindingFlags.NonPublic | BindingFlags.Instance);
            DrawerAttributeField = drawerType.GetField("m_Attribute", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public static void DrawProperty(
            Rect position,
            SerializedProperty property,
            GUIContent label,
            FieldInfo sourceFieldInfo,
            bool includeChildren = true)
        {
            if (!TryGetPropertyDrawer(sourceFieldInfo, property, out var drawer)
                || !CanSafelyUseDrawer(drawer, property))
            {
                EditorGUI.PropertyField(position, property, label, includeChildren);
                return;
            }

            if (!TryDrawForwarded(position, property, label, drawer))
                drawer.OnGUI(position, property, label);
        }

        public static float GetPropertyHeight(
            SerializedProperty property,
            GUIContent label,
            FieldInfo sourceFieldInfo,
            bool includeChildren = true)
        {
            if (!TryGetPropertyDrawer(sourceFieldInfo, property, out var drawer)
                || !CanSafelyUseDrawer(drawer, property))
                return EditorGUI.GetPropertyHeight(property, label, includeChildren);

            return drawer.GetPropertyHeight(property, label);
        }

        /// <summary>
        /// 获取转发 Drawer，供高度与绘制共用同一实例。
        /// </summary>
        public static bool TryGetForwardedDrawer(
            FieldInfo sourceFieldInfo,
            SerializedProperty property,
            out PropertyDrawer drawer)
        {
            return TryGetPropertyDrawer(sourceFieldInfo, property, out drawer);
        }

        /// <summary>
        /// Unity 的 TextArea/Multiline 内置 Drawer 嵌套在自定义 PropertyDrawer 中会把标题画两次
        /// （HandlePrefixLabel 一次 + IndentedRect 错位一次）。由这里自绘并返回 true。
        /// </summary>
        public static bool TryDrawForwarded(
            Rect position,
            SerializedProperty property,
            GUIContent label,
            PropertyDrawer drawer)
        {
            if (drawer == null || property == null)
                return false;

            if (drawer.attribute is TextAreaAttribute)
            {
                DrawTextArea(position, property, label);
                return true;
            }

            if (drawer.attribute is MultilineAttribute)
            {
                DrawMultiline(position, property, label);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 原 Drawer 在当前属性类型上是否可安全调用（避免 TextArea 读 stringValue 等每帧 LogError）。
        /// </summary>
        public static bool CanSafelyUseDrawer(PropertyDrawer drawer, SerializedProperty property)
        {
            if (drawer == null || property == null)
                return false;
            return CanSafelyUseAttribute(drawer.attribute, property);
        }

        public static bool CanSafelyUseAttribute(PropertyAttribute attribute, SerializedProperty property)
        {
            if (property == null)
                return false;
            if (attribute == null)
                return true;

            if (attribute is TextAreaAttribute || attribute is MultilineAttribute)
                return property.propertyType == SerializedPropertyType.String;

            if (attribute is RangeAttribute)
            {
                return property.propertyType == SerializedPropertyType.Float
                    || property.propertyType == SerializedPropertyType.Integer;
            }

            return true;
        }

        static void DrawTextArea(Rect position, SerializedProperty property, GUIContent label)
        {
            float line = EditorGUIUtility.singleLineHeight;
            var indented = EditorGUI.IndentedRect(position);
            indented.height = position.height;

            int oldIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var textRect = indented;
            if (label != null && !string.IsNullOrEmpty(label.text))
            {
                EditorGUI.LabelField(new Rect(indented.x, indented.y, indented.width, line), label);
                textRect = new Rect(
                    indented.x,
                    indented.y + line,
                    indented.width,
                    Mathf.Max(0f, indented.height - line));
            }

            EditorGUI.BeginChangeCheck();
            string newValue = EditorGUI.TextArea(textRect, property.stringValue ?? string.Empty);
            if (EditorGUI.EndChangeCheck())
                property.stringValue = newValue;

            EditorGUI.indentLevel = oldIndent;
        }

        static void DrawMultiline(Rect position, SerializedProperty property, GUIContent label)
        {
            var indented = EditorGUI.IndentedRect(position);
            indented.height = position.height;

            int oldIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var textRect = indented;
            if (label != null && !string.IsNullOrEmpty(label.text))
            {
                float labelWidth = Mathf.Max(0f, EditorGUIUtility.labelWidth - (indented.x - position.x));
                EditorGUI.LabelField(
                    new Rect(indented.x, indented.y, labelWidth, EditorGUIUtility.singleLineHeight),
                    label);
                textRect = new Rect(
                    indented.x + labelWidth,
                    indented.y,
                    Mathf.Max(0f, indented.width - labelWidth),
                    indented.height);
            }

            EditorGUI.BeginChangeCheck();
            string newValue = EditorGUI.TextArea(textRect, property.stringValue ?? string.Empty);
            if (EditorGUI.EndChangeCheck())
                property.stringValue = newValue;

            EditorGUI.indentLevel = oldIndent;
        }

        /// <summary>偏窄估算宽：无 pos 时宁可略高，避免 Hint 叠到下一控件。</summary>
        public static float EstimateLayoutWidth()
        {
            return Mathf.Max(
                EditorGUIUtility.currentViewWidth - 70f - EditorGUI.indentLevel * 15f,
                80f);
        }

        private static bool TryGetPropertyDrawer(
            FieldInfo sourceFieldInfo,
            SerializedProperty property,
            out PropertyDrawer drawer)
        {
            drawer = null;
            if (sourceFieldInfo == null)
                return false;

            if (s_drawerInstanceCache.TryGetValue(sourceFieldInfo, out drawer))
                return true;

            if (!TryGetFieldDrawerSpec(sourceFieldInfo, out var spec))
                return false;

            drawer = CreateDrawer(spec.DrawerType, sourceFieldInfo, spec.Attribute);
            if (drawer == null)
                return false;

            s_drawerInstanceCache[sourceFieldInfo] = drawer;
            return true;
        }

        private static bool TryGetFieldDrawerSpec(
            FieldInfo sourceFieldInfo,
            out FieldDrawerSpec spec)
        {
            if (s_fieldSpecCache.TryGetValue(sourceFieldInfo, out var cached))
            {
                if (cached.HasValue)
                {
                    spec = cached.Value;
                    return true;
                }

                spec = default;
                return false;
            }

            spec = default;
            FieldDrawerSpec? resolved = null;
            bool allAttributesResolved = true;
            var attributes = sourceFieldInfo.GetCustomAttributes(typeof(PropertyAttribute), true);
            for (int i = attributes.Length - 1; i >= 0; i--)
            {
                var attribute = (PropertyAttribute)attributes[i];
                var attrType = attribute.GetType();
                if (SkipAttributeTypes.Contains(attrType))
                    continue;

                var drawerType = GetDrawerType(attrType);
                if (drawerType == null)
                {
                    if (!s_drawerTypeByAttribute.ContainsKey(attrType))
                        allAttributesResolved = false;
                    continue;
                }

                resolved = new FieldDrawerSpec
                {
                    DrawerType = drawerType,
                    Attribute = attribute,
                };
                break;
            }

            if (resolved.HasValue)
            {
                s_fieldSpecCache[sourceFieldInfo] = resolved;
                spec = resolved.Value;
                return true;
            }

            if (allAttributesResolved)
                s_fieldSpecCache[sourceFieldInfo] = null;

            return false;
        }

        private static PropertyDrawer CreateDrawer(
            Type drawerType,
            FieldInfo sourceFieldInfo,
            PropertyAttribute attribute)
        {
            if (!PropertyVariableAttributeNesting.IsUsableDrawerType(drawerType))
                return null;

            var drawer = (PropertyDrawer)Activator.CreateInstance(drawerType);
            DrawerFieldInfoField?.SetValue(drawer, sourceFieldInfo);
            DrawerAttributeField?.SetValue(drawer, attribute);
            return drawer;
        }

        private static Type GetDrawerType(Type attributeType)
        {
            if (attributeType == null)
                return null;

            if (s_drawerTypeByAttribute.TryGetValue(attributeType, out var cached))
                return cached;

            var original = PropertyVariableAttributeNesting.GetOriginalDrawerType(attributeType);
            var usable = PropertyVariableAttributeNesting.IsUsableDrawerType(original) ? original : null;
            s_drawerTypeByAttribute[attributeType] = usable;
            return usable;
        }
    }
}
