// -------------------------
// 创建日期：2026/9/15 17:10:00
// -------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor;

namespace Framework.Editor
{
    /// <summary>类型选择器的类型收集与解析缓存。</summary>
    public static class TypeSelectCache
    {
        public struct Entry
        {
            public Type type;
            public string fullName;
            public string name;
            public string ns;
            public bool isSystem;
        }

        static readonly Type[] AlwaysVisibleTypes =
        {
            typeof(object),
            typeof(string),
            typeof(bool),
            typeof(byte),
            typeof(sbyte),
            typeof(char),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(decimal),
        };

        static readonly HashSet<Type> AlwaysVisible = new HashSet<Type>(AlwaysVisibleTypes);

        static List<Entry> s_entries;

        public static IReadOnlyList<Entry> GetEntries()
        {
            if (s_entries == null)
                Build();
            return s_entries;
        }

        public static bool IsAlwaysVisible(Type type)
        {
            return type != null && AlwaysVisible.Contains(type);
        }

        public static bool IsSystemType(Type type)
        {
            if (type == null)
                return false;
            return IsSystemAssembly(type.Assembly);
        }

        public static Type Find(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
                return null;

            Type type = Type.GetType(fullName);
            if (type != null)
                return type;

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                try
                {
                    type = assemblies[i].GetType(fullName);
                }
                catch (ReflectionTypeLoadException)
                {
                    type = null;
                }

                if (type != null)
                    return type;
            }

            if (fullName.EndsWith("[]", StringComparison.Ordinal))
            {
                Type element = Find(fullName.Substring(0, fullName.Length - 2));
                if (element != null)
                    return element.MakeArrayType();
            }

            return null;
        }

        static void Build()
        {
            var set = new HashSet<Type>();
            var list = new List<Entry>(2048);

            for (int i = 0; i < AlwaysVisibleTypes.Length; i++)
                TryAdd(AlwaysVisibleTypes[i], set, list);

            AddTypes(TypeCache.GetTypesDerivedFrom(typeof(object)), set, list);
            AddTypes(TypeCache.GetTypesDerivedFrom(typeof(ValueType)), set, list);

            list.Sort(CompareEntries);
            s_entries = list;
        }

        static void AddTypes(IEnumerable<Type> types, HashSet<Type> set, List<Entry> list)
        {
            foreach (Type type in types)
                TryAdd(type, set, list);
        }

        static void TryAdd(Type type, HashSet<Type> set, List<Entry> list)
        {
            if (type == null || !set.Add(type))
                return;
            if (!IsSelectable(type))
                return;

            list.Add(new Entry
            {
                type = type,
                fullName = type.FullName,
                name = type.Name,
                ns = type.Namespace ?? string.Empty,
                isSystem = IsSystemAssembly(type.Assembly),
            });
        }

        static bool IsSelectable(Type type)
        {
            if (string.IsNullOrEmpty(type.FullName))
                return false;
            if (type.IsPointer || type.IsByRef)
                return false;
            if (type.IsGenericTypeDefinition || type.IsArray)
                return false;
            if (type == typeof(void))
                return false;
            if (IsCompilerGenerated(type))
                return false;
            return true;
        }

        static bool IsCompilerGenerated(Type type)
        {
            if (type.Name.IndexOf('<') >= 0)
                return true;
            return type.IsDefined(typeof(CompilerGeneratedAttribute), false);
        }

        static bool IsSystemAssembly(Assembly assembly)
        {
            if (assembly == null)
                return true;

            string name = assembly.GetName().Name;
            if (string.IsNullOrEmpty(name))
                return true;

            return name == "mscorlib"
                || name == "netstandard"
                || name == "System"
                || name.StartsWith("System.", StringComparison.Ordinal)
                || name.StartsWith("Microsoft.", StringComparison.Ordinal)
                || name.StartsWith("Unity", StringComparison.Ordinal)
                || name.StartsWith("Mono.", StringComparison.Ordinal);
        }

        static int CompareEntries(Entry a, Entry b)
        {
            int ns = string.CompareOrdinal(a.ns, b.ns);
            if (ns != 0)
                return ns;
            int name = string.CompareOrdinal(a.name, b.name);
            return name != 0 ? name : string.CompareOrdinal(a.fullName, b.fullName);
        }
    }
}
