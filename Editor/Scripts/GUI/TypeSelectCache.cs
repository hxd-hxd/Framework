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
            typeof(List<>),
            typeof(Dictionary<,>),
            typeof(HashSet<>),
            typeof(Queue<>),
            typeof(Stack<>),
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

        static readonly Dictionary<string, Type> s_findCache = new Dictionary<string, Type>();

        public static Type Find(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
                return null;

            if (s_findCache.TryGetValue(fullName, out Type cached))
                return cached;

            Type type = FindUncached(fullName);
            s_findCache[fullName] = type;
            return type;
        }

        static Type FindUncached(string fullName)
        {
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

            if (fullName.IndexOf('`') >= 0)
            {
                for (int i = 0; i < assemblies.Length; i++)
                {
                    try
                    {
                        type = Type.GetType(fullName + ", " + assemblies[i].FullName, false);
                    }
                    catch (Exception)
                    {
                        type = null;
                    }

                    if (type != null)
                        return type;
                }
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
            if (type.IsArray)
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

        public static string GetDisplayName(Type type)
        {
            if (type == null)
                return "未选择";

            var sb = new System.Text.StringBuilder(64);
            AppendDisplayName(type, sb);
            return sb.ToString();
        }

        static void AppendDisplayName(Type type, System.Text.StringBuilder sb)
        {
            if (type.IsArray)
            {
                AppendDisplayName(type.GetElementType(), sb);
                sb.Append("[]");
                return;
            }

            if (type.IsGenericType)
            {
                if (!type.IsGenericTypeDefinition
                    && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    AppendDisplayName(type.GetGenericArguments()[0], sb);
                    sb.Append('?');
                    return;
                }

                string name = type.Name;
                int tick = name.IndexOf('`');
                if (tick >= 0)
                    name = name.Substring(0, tick);
                sb.Append(name);
                sb.Append('<');
                Type[] args = type.GetGenericArguments();
                if (type.IsGenericTypeDefinition)
                {
                    for (int i = 1; i < args.Length; i++)
                        sb.Append(',');
                }
                else
                {
                    for (int i = 0; i < args.Length; i++)
                    {
                        if (i > 0)
                            sb.Append(", ");
                        AppendDisplayName(args[i], sb);
                    }
                }
                sb.Append('>');
                return;
            }

            sb.Append(type.Name);
        }

        public static bool MatchesGenericParameter(Type candidate, Type genericParameter)
        {
            if (candidate == null)
                return false;
            if (genericParameter == null || !genericParameter.IsGenericParameter)
                return true;

            GenericParameterAttributes special =
                genericParameter.GenericParameterAttributes & GenericParameterAttributes.SpecialConstraintMask;

            if ((special & GenericParameterAttributes.ReferenceTypeConstraint) != 0
                && candidate.IsValueType)
                return false;

            if ((special & GenericParameterAttributes.NotNullableValueTypeConstraint) != 0)
            {
                if (!candidate.IsValueType)
                    return false;
                if (candidate.IsGenericType && candidate.GetGenericTypeDefinition() == typeof(Nullable<>))
                    return false;
            }

            if ((special & GenericParameterAttributes.DefaultConstructorConstraint) != 0
                && !HasPublicParameterlessCtor(candidate))
                return false;

            Type[] constraints = genericParameter.GetGenericParameterConstraints();
            for (int i = 0; i < constraints.Length; i++)
            {
                Type constraint = constraints[i];
                if (constraint == null || constraint.IsGenericParameter)
                    continue;
                if (candidate.IsGenericTypeDefinition)
                    continue;
                if (!constraint.IsAssignableFrom(candidate))
                    return false;
            }

            return true;
        }

        public static bool TryMakeClosedType(Type definition, Type[] args, out Type result, out string error)
        {
            result = null;
            error = null;

            if (definition == null)
            {
                error = "未选择类型";
                return false;
            }

            if (!definition.IsGenericTypeDefinition)
            {
                result = definition;
                return true;
            }

            Type[] parameters = definition.GetGenericArguments();
            if (args == null || args.Length != parameters.Length)
            {
                error = "泛型参数数量不匹配";
                return false;
            }

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == null)
                {
                    error = "请选择 " + parameters[i].Name;
                    return false;
                }

                if (!MatchesGenericParameter(args[i], parameters[i]))
                {
                    error = GetDisplayName(args[i]) + " 不满足参数 " + parameters[i].Name + " 的约束";
                    return false;
                }
            }

            try
            {
                result = definition.MakeGenericType(args);
                return true;
            }
            catch (ArgumentException e)
            {
                error = e.Message;
                return false;
            }
        }

        static bool HasPublicParameterlessCtor(Type type)
        {
            if (type.IsValueType)
                return true;
            if (type.IsAbstract || type.IsInterface)
                return false;
            return type.GetConstructor(Type.EmptyTypes) != null;
        }
    }
}
