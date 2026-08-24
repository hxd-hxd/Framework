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
    /// 非 PV 字段转发给安装当下的下一层 Drawer（原版或其它代理）；重入时改走 TrueOriginal，避免包装链成环。
    /// </summary>
    internal sealed class PropertyVariableAttributeNestingDrawer : PropertyDrawer
    {
        private static readonly Dictionary<Type, bool> s_propertyVariableTypeCache = new Dictionary<Type, bool>();
        private static int s_invokeDepth;

        private PropertyDrawer _cachedDrawer;
        private Type _cachedDrawerType;
        private int _boundGeneration = -1;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (ShouldNestCurrent(property))
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            if (!TryGetShellDrawer(s_invokeDepth > 0, out var drawer)
                || !PropertyAttributeForwardingUtility.CanSafelyUseDrawer(drawer, property))
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            s_invokeDepth++;
            try
            {
                drawer.OnGUI(position, property, label);
            }
            finally
            {
                s_invokeDepth--;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (ShouldNestCurrent(property))
                return EditorGUI.GetPropertyHeight(property, label, true);

            if (!TryGetShellDrawer(s_invokeDepth > 0, out var drawer)
                || !PropertyAttributeForwardingUtility.CanSafelyUseDrawer(drawer, property))
                return EditorGUI.GetPropertyHeight(property, label, true);

            s_invokeDepth++;
            try
            {
                return drawer.GetPropertyHeight(property, label);
            }
            finally
            {
                s_invokeDepth--;
            }
        }

        bool TryGetShellDrawer(bool reentered, out PropertyDrawer drawer)
        {
            drawer = null;
            var attrType = attribute != null ? attribute.GetType() : null;
            var drawerType = reentered
                ? PropertyVariableAttributeNesting.GetOriginalDrawerType(attrType)
                : PropertyVariableAttributeNesting.GetNextDrawerType(attrType);

            if (!PropertyVariableAttributeNesting.IsUsableDrawerType(drawerType))
                drawerType = PropertyVariableAttributeNesting.GetOriginalDrawerType(attrType);

            if (!PropertyVariableAttributeNesting.IsUsableDrawerType(drawerType))
                return false;

            int generation = PropertyVariableAttributeNesting.InstallGeneration;
            if (_boundGeneration != generation
                || _cachedDrawerType != drawerType
                || _cachedDrawer == null)
            {
                _cachedDrawer = PropertyVariableAttributeNesting.CreateDrawer(drawerType, fieldInfo, attribute);
                _cachedDrawerType = drawerType;
                _boundGeneration = generation;
            }

            drawer = _cachedDrawer;
            return drawer != null;
        }

        bool ShouldNestCurrent(SerializedProperty property)
        {
            return ShouldNest(property, fieldInfo);
        }

        /// <summary>
        /// 仅当正在绘制 PropertyVariable 外壳（Generic，且带 <c>_value</c>）时下沉；
        /// 转发到 _value（int/string/List 等）时走原 Drawer。
        /// List 等 Generic _value 不能只凭 fieldInfo 判断，否则会当成外壳
        /// <c>PropertyField(none)</c> 画出空标题折页。
        /// </summary>
        internal static bool ShouldNest(SerializedProperty property, FieldInfo fieldInfo)
        {
            return property != null
                && property.propertyType == SerializedPropertyType.Generic
                && IsPropertyVariableType(fieldInfo?.FieldType)
                && property.FindPropertyRelative("_value") != null;
        }

        internal static bool IsPropertyVariableType(Type type)
        {
            if (type == null)
                return false;
            if (s_propertyVariableTypeCache.TryGetValue(type, out var cached))
                return cached;

            bool result = false;
            var current = type;
            while (current != null)
            {
                if (current.IsGenericType)
                {
                    var def = current.GetGenericTypeDefinition();
                    if (def == typeof(PropertyVariable<>)
                        || def == typeof(Framework.Core.PropertyVariable<>))
                    {
                        result = true;
                        break;
                    }
                }

                current = current.BaseType;
            }

            s_propertyVariableTypeCache[type] = result;
            return result;
        }
    }

    /// <summary>
    /// 将 ScriptAttributeUtility 中 PropertyAttribute 的 PropertyDrawer 包一层嵌套代理。
    /// 可重入、可链式、哨兵自愈；不把 Container[] 压成长度 1。
    /// </summary>
    [InitializeOnLoad]
    internal static class PropertyVariableAttributeNesting
    {
        private const int DelayInstallCount = 8;
        private const double WatchdogIntervalSeconds = 2.0;
        private const string LogPrefix = "[PropertyVariable] ";

        /// <summary>安装当下注册表中的下一层（原版或其它代理）。非 PV 外壳走这条链。</summary>
        private static readonly Dictionary<Type, Type> NextDrawerTypes = new Dictionary<Type, Type>();

        /// <summary>TypeCache 解析出的真正 Drawer，供转发到 _value。</summary>
        private static readonly Dictionary<Type, Type> TypeCacheTrueOriginals = new Dictionary<Type, Type>();

        private static readonly FieldInfo DrawerFieldInfoField;
        private static readonly FieldInfo DrawerAttributeField;
        private static readonly FieldInfo CustomPropertyDrawerTypeField;

        private static readonly Type NestingDrawerType = typeof(PropertyVariableAttributeNestingDrawer);

        private static bool _typeCacheBuilt;
        private static bool _installInProgress;
        private static bool _loggedInstallFailure;
        private static bool _loggedWatchdogRecover;
        private static int _delayInstallsRemaining;
        private static double _lastWatchdogTime;
        private static int _installGeneration;

        public static int InstallGeneration => _installGeneration;

        static PropertyVariableAttributeNesting()
        {
            DrawerFieldInfoField = typeof(PropertyDrawer).GetField(
                "m_FieldInfo", BindingFlags.Instance | BindingFlags.NonPublic);
            DrawerAttributeField = typeof(PropertyDrawer).GetField(
                "m_Attribute", BindingFlags.Instance | BindingFlags.NonPublic);
            CustomPropertyDrawerTypeField = typeof(CustomPropertyDrawer).GetField(
                "m_Type", BindingFlags.NonPublic | BindingFlags.Instance);

            EditorApplication.delayCall += DelayedInstall;
            EditorApplication.update -= Watchdog;
            EditorApplication.update += Watchdog;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            try
            {
                Install();
            }
            catch (Exception e)
            {
                LogInstallFailure(e.Message);
            }
        }

        /// <summary>真正的特性 Drawer，供转发到 _value；不是包装链上的代理。</summary>
        public static Type GetOriginalDrawerType(Type attributeType)
        {
            if (attributeType == null)
                return null;

            EnsureTypeCacheTrueOriginals();
            if (TypeCacheTrueOriginals.TryGetValue(attributeType, out var cached)
                && IsUsableDrawerType(cached))
                return cached;

            if (NextDrawerTypes.TryGetValue(attributeType, out var next)
                && IsUsableDrawerType(next)
                && !NameLooksLikeProxy(next.Name))
                return next;

            if (NextDrawerTypes.TryGetValue(attributeType, out next)
                && IsUsableDrawerType(next))
                return next;

            return null;
        }

        /// <summary>非 PV 外壳应调用的下一层 Drawer。</summary>
        public static Type GetNextDrawerType(Type attributeType)
        {
            if (attributeType == null)
                return null;
            return NextDrawerTypes.TryGetValue(attributeType, out var type) ? type : null;
        }

        public static bool IsUsableDrawerType(Type drawerType)
        {
            return drawerType != null
                && drawerType != NestingDrawerType
                && typeof(PropertyDrawer).IsAssignableFrom(drawerType)
                && !typeof(DecoratorDrawer).IsAssignableFrom(drawerType);
        }

        public static PropertyDrawer CreateDrawer(Type drawerType, FieldInfo fieldInfo, PropertyAttribute attribute)
        {
            if (!IsUsableDrawerType(drawerType))
                return null;

            var drawer = (PropertyDrawer)Activator.CreateInstance(drawerType);
            DrawerFieldInfoField?.SetValue(drawer, fieldInfo);
            DrawerAttributeField?.SetValue(drawer, attribute);
            return drawer;
        }

        static void DelayedInstall()
        {
            Install();
            _delayInstallsRemaining++;
            if (_delayInstallsRemaining < DelayInstallCount)
                EditorApplication.delayCall += DelayedInstall;
        }

        static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode
                || state == PlayModeStateChange.EnteredEditMode)
                Install();
        }

        static void Watchdog()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
                return;

            double now = EditorApplication.timeSinceStartup;
            if (now - _lastWatchdogTime < WatchdogIntervalSeconds)
                return;
            _lastWatchdogTime = now;

            if (IsSentinelOurs())
                return;

            Install();
            if (IsSentinelOurs())
            {
                if (!_loggedWatchdogRecover)
                {
                    _loggedWatchdogRecover = true;
                    Debug.LogWarning(LogPrefix + "Attribute nesting sentinel lost; registry re-wrapped.");
                }
            }
            else
            {
                LogInstallFailure("sentinel is not nesting drawer");
            }
        }

        static void Install()
        {
            if (_installInProgress)
                return;

            _installInProgress = true;
            try
            {
                ForceBuildDrawerCache();
                bool changed = TryInstallModernCache();
                changed |= TryInstallLegacyCache();
                if (changed)
                {
                    _installGeneration++;
                    ClearHandlerCaches();
                    ClearStaticTypeCache();
                    _loggedInstallFailure = false;
                    _loggedWatchdogRecover = false;
                }
            }
            catch (Exception e)
            {
                LogInstallFailure(e.Message);
            }
            finally
            {
                _installInProgress = false;
            }
        }

        static bool IsSentinelOurs()
        {
            return PeekRegisteredDrawerType(typeof(RangeAttribute)) == NestingDrawerType;
        }

        static void LogInstallFailure(string message)
        {
            if (_loggedInstallFailure)
                return;
            _loggedInstallFailure = true;
            Debug.LogWarning(LogPrefix + "Attribute nesting install failed: " + message);
        }

        static void ForceBuildDrawerCache()
        {
            var utilityType = GetScriptAttributeUtilityType();
            if (utilityType == null)
                return;

            foreach (var method in utilityType.GetMethods(
                         BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
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

                    if (ps.Length >= 1 && ps[0].ParameterType == typeof(Type))
                    {
                        var args = new object[ps.Length];
                        args[0] = typeof(RangeAttribute);
                        for (int i = 1; i < ps.Length; i++)
                        {
                            if (ps[i].ParameterType == typeof(bool))
                                args[i] = false;
                            else if (ps[i].ParameterType == typeof(Type[]))
                                args[i] = null;
                            else
                                args[i] = ps[i].ParameterType.IsValueType
                                    ? Activator.CreateInstance(ps[i].ParameterType)
                                    : null;
                        }

                        method.Invoke(null, args);
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

        static Type PeekRegisteredDrawerType(Type attributeType)
        {
            if (attributeType == null)
                return null;

            Type modern = null;
            if (TryGetModernDictionary(out var modernDict, out var containerDrawerField)
                && modernDict.Contains(attributeType)
                && modernDict[attributeType] is Array containers
                && containers.Length > 0)
            {
                modern = containerDrawerField.GetValue(containers.GetValue(0)) as Type;
            }

            Type legacy = null;
            if (TryGetLegacyDictionary(out var legacyDict, out var keySetDrawerField, out _)
                && legacyDict.Contains(attributeType))
            {
                legacy = keySetDrawerField.GetValue(legacyDict[attributeType]) as Type;
            }

            if (modern == NestingDrawerType || legacy == NestingDrawerType)
                return NestingDrawerType;
            return modern ?? legacy;
        }

        /// <summary>Unity 新版：Lazy&lt;Dictionary&lt;Type, CustomPropertyDrawerContainer[]&gt;&gt;</summary>
        static bool TryInstallModernCache()
        {
            if (!TryGetModernDictionary(out var dict, out var drawerTypeField))
                return false;

            var keys = CopyAttributeKeys(dict);
            bool changed = false;
            foreach (var attrType in keys)
            {
                if (!(dict[attrType] is Array containers) || containers.Length == 0)
                    continue;

                for (int i = 0; i < containers.Length; i++)
                {
                    var boxed = containers.GetValue(i);
                    if (boxed == null)
                        continue;

                    var current = drawerTypeField.GetValue(boxed) as Type;
                    if (!IsUsableDrawerType(current))
                        continue;

                    RememberNextDrawer(attrType, current);
                    drawerTypeField.SetValue(boxed, NestingDrawerType);
                    containers.SetValue(boxed, i);
                    changed = true;
                }
            }

            return changed;
        }

        /// <summary>Unity 旧版：Dictionary&lt;Type, DrawerKeySet&gt;</summary>
        static bool TryInstallLegacyCache()
        {
            if (!TryGetLegacyDictionary(out var dict, out var drawerField, out var typeField))
                return false;

            var keys = CopyAttributeKeys(dict);
            bool changed = false;
            foreach (var attrType in keys)
            {
                var keySet = dict[attrType];
                if (keySet == null)
                    continue;

                var current = drawerField.GetValue(keySet) as Type;
                if (!IsUsableDrawerType(current))
                    continue;

                RememberNextDrawer(attrType, current);
                drawerField.SetValue(keySet, NestingDrawerType);
                typeField?.SetValue(keySet, attrType);
                dict[attrType] = keySet;
                changed = true;
            }

            return changed;
        }

        static bool TryGetModernDictionary(out IDictionary dict, out FieldInfo drawerTypeField)
        {
            dict = null;
            drawerTypeField = null;

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

            object dictObj = lazy;
            if (!(dictObj is IDictionary))
            {
                var valueProp = lazy.GetType().GetProperty("Value");
                dictObj = valueProp?.GetValue(lazy);
            }

            if (!(dictObj is IDictionary found))
                return false;

            var containerType = utilityType.GetNestedType(
                "CustomPropertyDrawerContainer",
                BindingFlags.NonPublic);
            if (containerType == null)
                return false;

            drawerTypeField = FindDrawerTypeField(containerType);
            if (drawerTypeField == null)
                return false;

            dict = found;
            return true;
        }

        static bool TryGetLegacyDictionary(
            out IDictionary dict,
            out FieldInfo drawerField,
            out FieldInfo typeField)
        {
            dict = null;
            drawerField = null;
            typeField = null;

            var utilityType = GetScriptAttributeUtilityType();
            if (utilityType == null)
                return false;

            var dictField = utilityType.GetField(
                "s_DrawerTypeForType",
                BindingFlags.Static | BindingFlags.NonPublic);
            if (dictField == null)
                return false;

            if (!(dictField.GetValue(null) is IDictionary found))
                return false;

            var keySetType = utilityType.GetNestedType("DrawerKeySet", BindingFlags.NonPublic);
            if (keySetType == null)
                return false;

            drawerField = FindDrawerTypeField(keySetType);
            typeField = keySetType.GetField("type", BindingFlags.Instance | BindingFlags.Public)
                ?? keySetType.GetField("type", BindingFlags.Instance | BindingFlags.NonPublic);
            if (drawerField == null)
                return false;

            dict = found;
            return true;
        }

        static FieldInfo FindDrawerTypeField(Type entryType)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            return entryType.GetField("drawerType", flags)
                ?? entryType.GetField("drawer", flags);
        }

        static List<Type> CopyAttributeKeys(IDictionary dict)
        {
            var keys = new List<Type>();
            foreach (var key in dict.Keys)
            {
                if (key is Type t && typeof(PropertyAttribute).IsAssignableFrom(t))
                    keys.Add(t);
            }

            return keys;
        }

        static void RememberNextDrawer(Type attributeType, Type currentDrawer)
        {
            NextDrawerTypes[attributeType] = currentDrawer;
        }

        static void EnsureTypeCacheTrueOriginals()
        {
            if (_typeCacheBuilt)
                return;
            _typeCacheBuilt = true;

            var types = TypeCache.GetTypesWithAttribute<CustomPropertyDrawer>();
            for (int i = 0; i < types.Count; i++)
            {
                var type = types[i];
                if (!IsUsableDrawerType(type))
                    continue;

                var customDrawerAttributes = type.GetCustomAttributes(typeof(CustomPropertyDrawer), false);
                for (int j = 0; j < customDrawerAttributes.Length; j++)
                {
                    var target = GetCustomPropertyDrawerType((CustomPropertyDrawer)customDrawerAttributes[j]);
                    if (target == null || !typeof(PropertyAttribute).IsAssignableFrom(target))
                        continue;

                    if (!TypeCacheTrueOriginals.TryGetValue(target, out var existing)
                        || ScoreTrueOriginal(type) > ScoreTrueOriginal(existing))
                        TypeCacheTrueOriginals[target] = type;
                }
            }
        }

        static int ScoreTrueOriginal(Type drawerType)
        {
            if (drawerType == null)
                return int.MinValue;

            int score = 0;
            string asm = drawerType.Assembly.GetName().Name;
            if (asm == "UnityEditor" || asm.StartsWith("UnityEditor."))
                score += 100;

            string ns = drawerType.Namespace ?? string.Empty;
            if (ns.StartsWith("UnityEditor") || ns == "UnityEngine")
                score += 50;

            if (NameLooksLikeProxy(drawerType.Name))
                score -= 80;

            return score;
        }

        static bool NameLooksLikeProxy(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;
            return name.IndexOf("Proxy", StringComparison.OrdinalIgnoreCase) >= 0
                || name.IndexOf("Wrapper", StringComparison.OrdinalIgnoreCase) >= 0
                || name.IndexOf("Nesting", StringComparison.OrdinalIgnoreCase) >= 0
                || name.IndexOf("Harmony", StringComparison.OrdinalIgnoreCase) >= 0
                || name.IndexOf("Patch", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        static Type GetCustomPropertyDrawerType(CustomPropertyDrawer customDrawer)
        {
            return CustomPropertyDrawerTypeField?.GetValue(customDrawer) as Type;
        }

        static void ClearHandlerCaches()
        {
            var utilityType = GetScriptAttributeUtilityType();
            var clear = utilityType?.GetMethod(
                "ClearGlobalCache",
                BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            clear?.Invoke(null, null);
        }

        static void ClearStaticTypeCache()
        {
            var utilityType = GetScriptAttributeUtilityType();
            var staticCache = utilityType?.GetField(
                "k_DrawerStaticTypesCache",
                BindingFlags.Static | BindingFlags.NonPublic);
            if (staticCache?.GetValue(null) is IDictionary cacheDict)
                cacheDict.Clear();
        }
    }
}
