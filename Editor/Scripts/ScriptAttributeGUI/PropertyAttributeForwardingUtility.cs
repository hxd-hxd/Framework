using System;
using System.Collections.Generic;
using System.Linq;
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
        private static readonly HashSet<Type> SkipAttributeTypes = new HashSet<Type>
        {
            typeof(PropertyVariableHideEventAttribute),
            typeof(PropertyVariableValueAttribute),
        };

        private static readonly FieldInfo DrawerFieldInfoField;
        private static readonly FieldInfo DrawerAttributeField;
        private static readonly MethodInfo GetDrawerTypeForPropertyMethod;
        private static readonly MethodInfo GetDrawerTypeForPropertyWithSerializedPropertyMethod;

        /// <summary>按路径缓存 Drawer，保证同字段 GetPropertyHeight / OnGUI 共用实例与宽度状态。</summary>
        private static readonly Dictionary<string, PropertyDrawer> s_drawerCache = new Dictionary<string, PropertyDrawer>();
        private const int DrawerCacheCapacity = 48;

        static PropertyAttributeForwardingUtility()
        {
            var drawerType = typeof(PropertyDrawer);
            DrawerFieldInfoField = drawerType.GetField("m_FieldInfo", BindingFlags.NonPublic | BindingFlags.Instance);
            DrawerAttributeField = drawerType.GetField("m_Attribute", BindingFlags.NonPublic | BindingFlags.Instance);

            var scriptAttributeUtility = typeof(EditorGUI).Assembly.GetType("UnityEditor.ScriptAttributeUtility");
            if (scriptAttributeUtility == null)
                return;

            const BindingFlags flags = BindingFlags.Static | BindingFlags.NonPublic;
            GetDrawerTypeForPropertyMethod = scriptAttributeUtility.GetMethod(
                "GetDrawerTypeForProperty",
                flags,
                null,
                new[] { typeof(Type) },
                null);

            GetDrawerTypeForPropertyWithSerializedPropertyMethod = scriptAttributeUtility.GetMethod(
                "GetDrawerTypeForProperty",
                flags,
                null,
                new[] { typeof(SerializedProperty), typeof(Type) },
                null);
        }

        public static void DrawProperty(
            Rect position,
            SerializedProperty property,
            GUIContent label,
            FieldInfo sourceFieldInfo,
            bool includeChildren = true)
        {
            if (!TryGetPropertyDrawer(sourceFieldInfo, property, out var drawer))
            {
                EditorGUI.PropertyField(position, property, label, includeChildren);
                return;
            }

            drawer.OnGUI(position, property, label);
        }

        public static float GetPropertyHeight(
            SerializedProperty property,
            GUIContent label,
            FieldInfo sourceFieldInfo,
            bool includeChildren = true)
        {
            if (!TryGetPropertyDrawer(sourceFieldInfo, property, out var drawer))
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

        /// <summary>偏窄估算宽：无 pos 时宁可略高，避免 Hint 叠到下一控件。</summary>
        public static float EstimateLayoutWidth()
        {
            return Mathf.Max(
                EditorGUIUtility.currentViewWidth - 70f - EditorGUI.indentLevel * 15f,
                80f);
        }

        private static void EnsureDrawerCacheCapacity()
        {
            // 跨帧保留实例以复用 _layoutWidth；超限清空，避免选中大量对象时无限增长
            if (s_drawerCache.Count > DrawerCacheCapacity)
                s_drawerCache.Clear();
        }

        private static bool TryGetPropertyDrawer(
            FieldInfo sourceFieldInfo,
            SerializedProperty property,
            out PropertyDrawer drawer)
        {
            drawer = null;
            if (sourceFieldInfo == null)
                return false;

            EnsureDrawerCacheCapacity();

            // 从后往前：与 Unity 多特性时取最后一个有 Drawer 的行为一致
            var attributes = sourceFieldInfo.GetCustomAttributes<PropertyAttribute>(true).ToArray();
            for (int i = attributes.Length - 1; i >= 0; i--)
            {
                var attribute = attributes[i];
                if (SkipAttributeTypes.Contains(attribute.GetType()))
                    continue;

                var drawerType = GetDrawerType(property, attribute.GetType());
                if (drawerType == null)
                    continue;

                string cacheKey = MakeCacheKey(sourceFieldInfo, property, attribute.GetType());
                if (s_drawerCache.TryGetValue(cacheKey, out drawer) && drawer != null)
                    return true;

                drawer = (PropertyDrawer)Activator.CreateInstance(drawerType);
                DrawerFieldInfoField?.SetValue(drawer, sourceFieldInfo);
                DrawerAttributeField?.SetValue(drawer, attribute);
                s_drawerCache[cacheKey] = drawer;
                return true;
            }

            return false;
        }

        private static string MakeCacheKey(FieldInfo fieldInfo, SerializedProperty property, Type attributeType)
        {
            return string.Concat(
                property.propertyPath, "\n",
                fieldInfo.DeclaringType != null ? fieldInfo.DeclaringType.FullName : "",
                ".", fieldInfo.Name, "\n",
                attributeType.FullName);
        }

        private static Type GetDrawerType(SerializedProperty property, Type attributeType)
        {
            // 优先使用安装器保存的原始 Drawer（避开嵌套代理，直接画 _value）
            var original = PropertyVariableAttributeNesting.GetOriginalDrawerType(attributeType);
            if (original != null && original != typeof(PropertyVariableAttributeNestingDrawer))
                return original;

            if (GetDrawerTypeForPropertyWithSerializedPropertyMethod != null)
            {
                var drawerType = GetDrawerTypeForPropertyWithSerializedPropertyMethod.Invoke(
                    null,
                    new object[] { property, attributeType }) as Type;
                if (drawerType != null && drawerType != typeof(PropertyVariableAttributeNestingDrawer))
                    return drawerType;
                if (drawerType == typeof(PropertyVariableAttributeNestingDrawer))
                {
                    original = PropertyVariableAttributeNesting.GetOriginalDrawerType(attributeType);
                    if (original != null)
                        return original;
                }
            }

            if (GetDrawerTypeForPropertyMethod != null)
            {
                var drawerType = GetDrawerTypeForPropertyMethod.Invoke(null, new object[] { attributeType }) as Type;
                if (drawerType != null && drawerType != typeof(PropertyVariableAttributeNestingDrawer))
                    return drawerType;
            }

            return FindDrawerType(attributeType);
        }

        private static Type FindDrawerType(Type attributeType)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] types;
                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    types = ex.Types;
                }

                if (types == null)
                    continue;

                foreach (var type in types)
                {
                    if (type == null || !typeof(PropertyDrawer).IsAssignableFrom(type))
                        continue;

                    var customDrawerAttributes = type.GetCustomAttributes(typeof(CustomPropertyDrawer), false);
                    foreach (CustomPropertyDrawer customDrawer in customDrawerAttributes)
                    {
                        var drawerTarget = GetCustomPropertyDrawerType(customDrawer);
                        if (drawerTarget == attributeType
                            && type != typeof(PropertyVariableAttributeNestingDrawer))
                            return type;
                    }
                }
            }

            return null;
        }

        private static Type GetCustomPropertyDrawerType(CustomPropertyDrawer customDrawer)
        {
            var field = typeof(CustomPropertyDrawer).GetField(
                "m_Type",
                BindingFlags.NonPublic | BindingFlags.Instance);
            return field?.GetValue(customDrawer) as Type;
        }
    }
}
