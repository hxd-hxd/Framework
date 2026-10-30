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
    /// Unity 内置特性按类型匹配：TextArea/Multiline 仅字符串，Range/Delayed 仅对应数值或字符串，
    /// Min 仅数值和向量，ColorUsage 仅颜色，GradientUsage 仅渐变。同一侧多个特性时靠下的生效。
    /// </summary>
    internal static class PropertyAttributeForwardingUtility
    {
        private struct FieldDrawerSpec
        {
            public Type DrawerType;
            public PropertyAttribute Attribute;
        }

        private struct DrawerInstanceKey : IEquatable<DrawerInstanceKey>
        {
            public FieldInfo Field;
            public Type AttributeType;

            public bool Equals(DrawerInstanceKey other)
            {
                return Field == other.Field && AttributeType == other.AttributeType;
            }

            public override bool Equals(object obj)
            {
                return obj is DrawerInstanceKey other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = Field != null ? Field.GetHashCode() : 0;
                    return (hash * 397) ^ (AttributeType != null ? AttributeType.GetHashCode() : 0);
                }
            }
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

        /// <summary>字段上可转发的特性，靠下的排在前面；同类型只保留靠下的一个。</summary>
        private static readonly Dictionary<FieldInfo, FieldDrawerSpec[]> s_fieldSpecCache =
            new Dictionary<FieldInfo, FieldDrawerSpec[]>();

        /// <summary>按字段和特性类型缓存 Drawer 实例，保证同字段测高与绘制共用。</summary>
        private static readonly Dictionary<DrawerInstanceKey, PropertyDrawer> s_drawerInstanceCache =
            new Dictionary<DrawerInstanceKey, PropertyDrawer>();

        static readonly FieldDrawerSpec[] EmptySpecs = Array.Empty<FieldDrawerSpec>();

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

            if (drawer.attribute is ColorUsageAttribute colorUsage)
            {
                DrawColorUsage(position, property, label, colorUsage);
                return true;
            }

            if (drawer.attribute is GradientUsageAttribute gradientUsage)
            {
                DrawGradientUsage(position, property, label, gradientUsage);
                return true;
            }

            if (drawer.attribute is MinAttribute minAttribute)
            {
                DrawMin(position, property, label, minAttribute);
                return true;
            }

            if (drawer.attribute is DelayedAttribute)
            {
                DrawDelayed(position, property, label);
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

            if (attribute is MinAttribute)
                return IsMinType(property.propertyType);

            if (attribute is DelayedAttribute)
            {
                return property.propertyType == SerializedPropertyType.Integer
                    || property.propertyType == SerializedPropertyType.Float
                    || property.propertyType == SerializedPropertyType.String;
            }

            if (attribute is ColorUsageAttribute)
                return property.propertyType == SerializedPropertyType.Color;

            if (attribute is GradientUsageAttribute)
                return property.propertyType == SerializedPropertyType.Gradient;

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

        static void DrawColorUsage(Rect position, SerializedProperty property, GUIContent label, ColorUsageAttribute colorUsage)
        {
            if (property.propertyType != SerializedPropertyType.Color)
                return;

            label = EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            Color newColor = EditorGUI.ColorField(
                position,
                label,
                property.colorValue,
                true,
                colorUsage.showAlpha,
                colorUsage.hdr);
            if (EditorGUI.EndChangeCheck())
                property.colorValue = newColor;
            EditorGUI.EndProperty();
        }

        static void DrawGradientUsage(Rect position, SerializedProperty property, GUIContent label, GradientUsageAttribute gradientUsage)
        {
            if (property.propertyType != SerializedPropertyType.Gradient)
                return;

            label = EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            Gradient gradient = GetGradientValue(property) ?? new Gradient();
#if UNITY_2020_2_OR_NEWER
            gradient = EditorGUI.GradientField(position, label, gradient, gradientUsage.hdr, gradientUsage.colorSpace);
#else
            gradient = EditorGUI.GradientField(position, label, gradient, gradientUsage.hdr);
#endif
            if (EditorGUI.EndChangeCheck())
                SetGradientValue(property, gradient);
            EditorGUI.EndProperty();
        }

        /// <summary>
        /// SerializedProperty.gradientValue 从 2022.1 起才公开。更早版本同名属性是 internal，用反射读写。
        /// </summary>
#if UNITY_2022_1_OR_NEWER
        static Gradient GetGradientValue(SerializedProperty property)
        {
            return property.gradientValue;
        }

        static void SetGradientValue(SerializedProperty property, Gradient value)
        {
            property.gradientValue = value;
        }
#else
        static readonly PropertyInfo GradientValueProperty = typeof(SerializedProperty).GetProperty(
            "gradientValue",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        static Gradient GetGradientValue(SerializedProperty property)
        {
            return GradientValueProperty != null
                ? GradientValueProperty.GetValue(property, null) as Gradient
                : null;
        }

        static void SetGradientValue(SerializedProperty property, Gradient value)
        {
            if (GradientValueProperty != null)
                GradientValueProperty.SetValue(property, value, null);
        }
#endif

        static void DrawMin(Rect position, SerializedProperty property, GUIContent label, MinAttribute minAttribute)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, property, label, true);
            if (EditorGUI.EndChangeCheck())
                ApplyMin(property, minAttribute.min);
        }

        static void DrawDelayed(Rect position, SerializedProperty property, GUIContent label)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    EditorGUI.DelayedIntField(position, property, label);
                    break;
                case SerializedPropertyType.Float:
                    EditorGUI.DelayedFloatField(position, property, label);
                    break;
                case SerializedPropertyType.String:
                    EditorGUI.DelayedTextField(position, property, label);
                    break;
            }
        }

        static bool IsMinType(SerializedPropertyType type)
        {
            switch (type)
            {
                case SerializedPropertyType.Integer:
                case SerializedPropertyType.Float:
                case SerializedPropertyType.Vector2:
                case SerializedPropertyType.Vector3:
                case SerializedPropertyType.Vector4:
                case SerializedPropertyType.Vector2Int:
                case SerializedPropertyType.Vector3Int:
                    return true;
                default:
                    return false;
            }
        }

        static void ApplyMin(SerializedProperty property, float min)
        {
            int minInt = Mathf.RoundToInt(min);
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    property.intValue = Mathf.Max(minInt, property.intValue);
                    break;
                case SerializedPropertyType.Float:
                    property.floatValue = Mathf.Max(min, property.floatValue);
                    break;
                case SerializedPropertyType.Vector2:
                    var vector2 = property.vector2Value;
                    property.vector2Value = new Vector2(Mathf.Max(min, vector2.x), Mathf.Max(min, vector2.y));
                    break;
                case SerializedPropertyType.Vector2Int:
                    var vector2Int = property.vector2IntValue;
                    property.vector2IntValue = new Vector2Int(Mathf.Max(minInt, vector2Int.x), Mathf.Max(minInt, vector2Int.y));
                    break;
                case SerializedPropertyType.Vector3:
                    var vector3 = property.vector3Value;
                    property.vector3Value = new Vector3(
                        Mathf.Max(min, vector3.x),
                        Mathf.Max(min, vector3.y),
                        Mathf.Max(min, vector3.z));
                    break;
                case SerializedPropertyType.Vector3Int:
                    var vector3Int = property.vector3IntValue;
                    property.vector3IntValue = new Vector3Int(
                        Mathf.Max(minInt, vector3Int.x),
                        Mathf.Max(minInt, vector3Int.y),
                        Mathf.Max(minInt, vector3Int.z));
                    break;
                case SerializedPropertyType.Vector4:
                    var vector4 = property.vector4Value;
                    property.vector4Value = new Vector4(
                        Mathf.Max(min, vector4.x),
                        Mathf.Max(min, vector4.y),
                        Mathf.Max(min, vector4.z),
                        Mathf.Max(min, vector4.w));
                    break;
            }
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
            if (sourceFieldInfo == null || property == null)
                return false;

            if (!TryGetFieldDrawerSpecs(sourceFieldInfo, out var specs))
                return false;

            for (int i = 0; i < specs.Length; i++)
            {
                var spec = specs[i];
                if (!CanSafelyUseAttribute(spec.Attribute, property))
                    continue;

                drawer = GetOrCreateDrawer(sourceFieldInfo, spec);
                return drawer != null;
            }

            return false;
        }

        static PropertyDrawer GetOrCreateDrawer(FieldInfo sourceFieldInfo, FieldDrawerSpec spec)
        {
            var key = new DrawerInstanceKey
            {
                Field = sourceFieldInfo,
                AttributeType = spec.Attribute != null ? spec.Attribute.GetType() : null,
            };
            if (s_drawerInstanceCache.TryGetValue(key, out var drawer))
                return drawer;

            drawer = CreateDrawer(spec.DrawerType, sourceFieldInfo, spec.Attribute);
            if (drawer != null)
                s_drawerInstanceCache[key] = drawer;
            return drawer;
        }

        /// <summary>收集字段上全部可转发特性。靠下的优先，便于同一侧多个特性时仍沿用原来的覆盖规则。</summary>
        private static bool TryGetFieldDrawerSpecs(
            FieldInfo sourceFieldInfo,
            out FieldDrawerSpec[] specs)
        {
            if (s_fieldSpecCache.TryGetValue(sourceFieldInfo, out specs))
                return specs != null && specs.Length > 0;

            bool allAttributesResolved = true;
            var attributes = sourceFieldInfo.GetCustomAttributes(typeof(PropertyAttribute), true);
            var resolved = new List<FieldDrawerSpec>(attributes.Length);
            var seen = new HashSet<Type>();
            for (int i = attributes.Length - 1; i >= 0; i--)
            {
                var attribute = (PropertyAttribute)attributes[i];
                var attrType = attribute.GetType();
                if (SkipAttributeTypes.Contains(attrType) || !seen.Add(attrType))
                    continue;

                var drawerType = GetDrawerType(attrType);
                if (drawerType == null)
                {
                    if (!s_drawerTypeByAttribute.ContainsKey(attrType))
                        allAttributesResolved = false;
                    continue;
                }

                resolved.Add(new FieldDrawerSpec
                {
                    DrawerType = drawerType,
                    Attribute = attribute,
                });
            }

            specs = resolved.Count > 0 ? resolved.ToArray() : EmptySpecs;
            if (allAttributesResolved)
                s_fieldSpecCache[sourceFieldInfo] = specs;
            return specs.Length > 0;
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
