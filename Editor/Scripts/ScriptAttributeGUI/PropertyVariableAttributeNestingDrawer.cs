using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Framework.Runtime;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    /// <summary>
    /// Unity 会把字段上的 PropertyAttribute Drawer 排在类型 Drawer 之前。
    /// 若直接对 PropertyVariable（Generic）调用 Range/TextArea 等，只会显示 “Use xxx with …” 错误行。
    /// 本代理在绘制 PropertyVariable 外壳时下沉到下一层 Drawer（类型 Drawer），再由转发逻辑作用到 _value。
    /// </summary>
    internal sealed class PropertyVariableAttributeNestingDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (ShouldNest(property, fieldInfo))
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            if (!TryCreateOriginalDrawer(out var drawer))
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            drawer.OnGUI(position, property, label);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (ShouldNest(property, fieldInfo))
                return EditorGUI.GetPropertyHeight(property, label, true);

            if (!TryCreateOriginalDrawer(out var drawer))
                return EditorGUI.GetPropertyHeight(property, label, true);

            // TextArea/Multiline 的 GetPropertyHeight 会无条件读 stringValue；
            // 类型不对时 Unity 在抛异常前就已 LogError，try/catch 拦不住控制台报错。
            if (!CanSafelyQueryOriginalHeight(property))
                return EditorGUIUtility.singleLineHeight;

            return drawer.GetPropertyHeight(property, label);
        }

        bool TryCreateOriginalDrawer(out PropertyDrawer drawer)
        {
            drawer = null;
            var attrType = attribute != null ? attribute.GetType() : null;
            var originalType = PropertyVariableAttributeNesting.GetOriginalDrawerType(attrType);
            if (originalType == null || originalType == typeof(PropertyVariableAttributeNestingDrawer))
                return false;

            drawer = PropertyVariableAttributeNesting.CreateDrawer(originalType, fieldInfo, attribute);
            return drawer != null;
        }

        /// <summary>
        /// 原 Drawer 的 GetPropertyHeight 是否可在当前属性类型上安全调用。
        /// </summary>
        bool CanSafelyQueryOriginalHeight(SerializedProperty property)
        {
            if (property == null)
                return false;

            if (attribute is TextAreaAttribute || attribute is MultilineAttribute)
                return property.propertyType == SerializedPropertyType.String;

            return true;
        }

        /// <summary>
        /// 仅当正在绘制 PropertyVariable 外壳（Generic）时下沉；
        /// 转发到 _value（int/string 等）时走原 Drawer。
        /// </summary>
        internal static bool ShouldNest(SerializedProperty property, FieldInfo fieldInfo)
        {
            return property != null
                && property.propertyType == SerializedPropertyType.Generic
                && IsPropertyVariableType(fieldInfo?.FieldType);
        }

        internal static bool IsPropertyVariableType(Type type)
        {
            while (type != null)
            {
                if (type.IsGenericType)
                {
                    var def = type.GetGenericTypeDefinition();
                    if (def == typeof(PropertyVariable<>)
                        || def == typeof(Framework.Core.PropertyVariable<>))
                        return true;
                }

                type = type.BaseType;
            }

            return false;
        }
    }

    /// <summary>
    /// 将 ScriptAttributeUtility 中 PropertyAttribute 的 PropertyDrawer 替换为嵌套代理。
    /// </summary>
    [InitializeOnLoad]
    internal static class PropertyVariableAttributeNesting
    {
        private static readonly Dictionary<Type, Type> OriginalDrawerTypes = new Dictionary<Type, Type>();
        private static readonly FieldInfo DrawerFieldInfoField;
        private static readonly FieldInfo DrawerAttributeField;
        private static bool _installed;
        private static int _installAttempts;

        static PropertyVariableAttributeNesting()
        {
            DrawerFieldInfoField = typeof(PropertyDrawer).GetField(
                "m_FieldInfo", BindingFlags.Instance | BindingFlags.NonPublic);
            DrawerAttributeField = typeof(PropertyDrawer).GetField(
                "m_Attribute", BindingFlags.Instance | BindingFlags.NonPublic);

            // 等程序集与 TypeCache 就绪后再安装；立即尝试一次，失败则 delayCall 重试
            try
            {
                Install();
            }
            catch
            {
                // ignore
            }

            if (!_installed)
                EditorApplication.delayCall += Install;
        }

        public static Type GetOriginalDrawerType(Type attributeType)
        {
            if (attributeType == null)
                return null;
            return OriginalDrawerTypes.TryGetValue(attributeType, out var type) ? type : null;
        }

        public static PropertyDrawer CreateDrawer(Type drawerType, FieldInfo fieldInfo, PropertyAttribute attribute)
        {
            if (drawerType == null || !typeof(PropertyDrawer).IsAssignableFrom(drawerType))
                return null;

            var drawer = (PropertyDrawer)Activator.CreateInstance(drawerType);
            DrawerFieldInfoField?.SetValue(drawer, fieldInfo);
            DrawerAttributeField?.SetValue(drawer, attribute);
            return drawer;
        }

        static void Install()
        {
            if (_installed)
                return;

            _installAttempts++;
            try
            {
                // 强制构建 Drawer 缓存
                ForceBuildDrawerCache();

                if (TryInstallModernCache() || TryInstallLegacyCache())
                {
                    _installed = true;
                    ClearHandlerCaches();
                    return;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[PropertyVariable] Attribute nesting install failed: {e.Message}");
            }

            if (_installAttempts < 10)
                EditorApplication.delayCall += Install;
        }

        static void ForceBuildDrawerCache()
        {
            var utilityType = GetScriptAttributeUtilityType();
            if (utilityType == null)
                return;

            // 新版签名可能带多个参数，尽量触发缓存构建
            foreach (var method in utilityType.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
            {
                if (method.Name != "GetDrawerTypeForType" && method.Name != "GetDrawerTypeForPropertyAndType")
                    continue;

                var ps = method.GetParameters();
                try
                {
                    if (ps.Length == 1 && ps[0].ParameterType == typeof(Type))
                    {
                        method.Invoke(null, new object[] { typeof(RangeAttribute) });
                        return;
                    }

                    if (ps.Length == 3
                        && ps[0].ParameterType == typeof(Type)
                        && ps[1].ParameterType == typeof(Type[]))
                    {
                        method.Invoke(null, new object[] { typeof(RangeAttribute), null, false });
                        return;
                    }
                }
                catch
                {
                    // try next overload
                }
            }
        }

        static Type GetScriptAttributeUtilityType()
        {
            return typeof(EditorGUI).Assembly.GetType("UnityEditor.ScriptAttributeUtility");
        }

        /// <summary>Unity 新版：Lazy&lt;Dictionary&lt;Type, CustomPropertyDrawerContainer[]&gt;&gt;</summary>
        static bool TryInstallModernCache()
        {
            var utilityType = GetScriptAttributeUtilityType();
            if (utilityType == null)
                return false;

            var lazyField = utilityType.GetField(
                "k_DrawerTypeForType",
                BindingFlags.Static | BindingFlags.NonPublic);
            if (lazyField == null)
                return false;

            var lazy = lazyField.GetValue(null);
            if (lazy == null)
                return false;

            var valueProp = lazy.GetType().GetProperty("Value");
            var dictObj = valueProp?.GetValue(lazy);
            if (!(dictObj is IDictionary dict))
                return false;

            var containerType = utilityType.GetNestedType(
                "CustomPropertyDrawerContainer",
                BindingFlags.NonPublic);
            if (containerType == null)
                return false;

            var drawerTypeField = containerType.GetField(
                "drawerType", BindingFlags.Instance | BindingFlags.Public);
            if (drawerTypeField == null)
                return false;

            ConstructorInfo containerCtor = null;
            foreach (var ctor in containerType.GetConstructors(
                         BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var ps = ctor.GetParameters();
                if (ps.Length >= 1 && ps[0].ParameterType == typeof(Type))
                {
                    containerCtor = ctor;
                    break;
                }
            }

            if (containerCtor == null)
                return false;

            var keys = new List<Type>();
            foreach (var key in dict.Keys)
            {
                if (key is Type t)
                    keys.Add(t);
            }

            bool changed = false;
            foreach (var attrType in keys)
            {
                if (!typeof(PropertyAttribute).IsAssignableFrom(attrType))
                    continue;

                var containers = dict[attrType] as Array;
                if (containers == null || containers.Length == 0)
                    continue;

                var first = containers.GetValue(0);
                var originalDrawer = drawerTypeField.GetValue(first) as Type;
                if (originalDrawer == null)
                    continue;
                if (!typeof(PropertyDrawer).IsAssignableFrom(originalDrawer))
                    continue;
                if (typeof(DecoratorDrawer).IsAssignableFrom(originalDrawer))
                    continue;
                if (originalDrawer == typeof(PropertyVariableAttributeNestingDrawer))
                    continue;

                if (!OriginalDrawerTypes.ContainsKey(attrType))
                    OriginalDrawerTypes[attrType] = originalDrawer;

                object newContainer;
                var ps = containerCtor.GetParameters();
                if (ps.Length == 3)
                    newContainer = containerCtor.Invoke(new object[]
                    {
                        typeof(PropertyVariableAttributeNestingDrawer),
                        null,
                        false
                    });
                else if (ps.Length == 2)
                    newContainer = containerCtor.Invoke(new object[]
                    {
                        typeof(PropertyVariableAttributeNestingDrawer),
                        false
                    });
                else
                    newContainer = containerCtor.Invoke(new object[]
                    {
                        typeof(PropertyVariableAttributeNestingDrawer)
                    });

                var newArray = Array.CreateInstance(containerType, 1);
                newArray.SetValue(newContainer, 0);
                dict[attrType] = newArray;
                changed = true;
            }

            if (changed)
            {
                var staticCache = utilityType.GetField(
                    "k_DrawerStaticTypesCache",
                    BindingFlags.Static | BindingFlags.NonPublic);
                if (staticCache?.GetValue(null) is IDictionary cacheDict)
                    cacheDict.Clear();
            }

            return changed;
        }

        /// <summary>Unity 旧版：Dictionary&lt;Type, DrawerKeySet&gt;</summary>
        static bool TryInstallLegacyCache()
        {
            var utilityType = GetScriptAttributeUtilityType();
            if (utilityType == null)
                return false;

            var dictField = utilityType.GetField(
                "s_DrawerTypeForType",
                BindingFlags.Static | BindingFlags.NonPublic);
            if (dictField == null)
                return false;

            // 触发构建
            var getDrawer = utilityType.GetMethod(
                "GetDrawerTypeForType",
                BindingFlags.Static | BindingFlags.NonPublic,
                null,
                new[] { typeof(Type) },
                null);
            getDrawer?.Invoke(null, new object[] { typeof(RangeAttribute) });

            if (!(dictField.GetValue(null) is IDictionary dict))
                return false;

            var keySetType = utilityType.GetNestedType("DrawerKeySet", BindingFlags.NonPublic);
            if (keySetType == null)
                return false;

            var drawerField = keySetType.GetField("drawer", BindingFlags.Instance | BindingFlags.Public);
            var typeField = keySetType.GetField("type", BindingFlags.Instance | BindingFlags.Public);
            if (drawerField == null)
                return false;

            var keys = new List<Type>();
            foreach (var key in dict.Keys)
            {
                if (key is Type t)
                    keys.Add(t);
            }

            bool changed = false;
            foreach (var attrType in keys)
            {
                if (!typeof(PropertyAttribute).IsAssignableFrom(attrType))
                    continue;

                var keySet = dict[attrType];
                var originalDrawer = drawerField.GetValue(keySet) as Type;
                if (originalDrawer == null)
                    continue;
                if (!typeof(PropertyDrawer).IsAssignableFrom(originalDrawer))
                    continue;
                if (typeof(DecoratorDrawer).IsAssignableFrom(originalDrawer))
                    continue;
                if (originalDrawer == typeof(PropertyVariableAttributeNestingDrawer))
                    continue;

                if (!OriginalDrawerTypes.ContainsKey(attrType))
                    OriginalDrawerTypes[attrType] = originalDrawer;

                var newKeySet = Activator.CreateInstance(keySetType);
                drawerField.SetValue(newKeySet, typeof(PropertyVariableAttributeNestingDrawer));
                typeField?.SetValue(newKeySet, attrType);
                dict[attrType] = newKeySet;
                changed = true;
            }

            return changed;
        }

        static void ClearHandlerCaches()
        {
            var utilityType = GetScriptAttributeUtilityType();
            var clear = utilityType?.GetMethod(
                "ClearGlobalCache",
                BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            clear?.Invoke(null, null);
        }
    }
}
