using System;
using System.Collections.Generic;
using System.Reflection;

namespace Framework.Core
{
    /// <summary>
    /// 按需缓存类型与成员查找结果（含未找到）。仅主线程使用。
    /// </summary>
    public static class TypeCache
    {
        const BindingFlags DeclaredMemberFlags =
            BindingFlags.Public
            | BindingFlags.NonPublic
            | BindingFlags.Instance
            | BindingFlags.Static
            | BindingFlags.DeclaredOnly;

        static readonly MemberInfo[] EmptyMembers = new MemberInfo[0];

        static readonly Dictionary<string, Type> s_types = new Dictionary<string, Type>();
        static readonly Dictionary<Type, Dictionary<string, MemberInfo[]>> s_membersByName =
            new Dictionary<Type, Dictionary<string, MemberInfo[]>>();
        static readonly Dictionary<Type, MemberInfo[]> s_allMembers = new Dictionary<Type, MemberInfo[]>();

        /// <summary>按全名查找类型。未找到返回 null，结果会缓存。</summary>
        public static Type Get(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
                return null;

            if (s_types.TryGetValue(fullName, out var type))
                return type;

            type = ResolveType(fullName);
            s_types[fullName] = type;
            return type;
        }

        /// <summary>获取类型的全部成员（含基类，派生类在前）。返回缓存数组，请勿修改。</summary>
        public static MemberInfo[] GetMembers(Type type)
        {
            if (type == null)
                return EmptyMembers;

            EnsureMembers(type);
            return s_allMembers[type];
        }

        /// <summary>按名获取成员（含重载）。找不到返回空数组，请勿修改返回值。</summary>
        public static MemberInfo[] GetMembers(Type type, string name)
        {
            if (type == null || string.IsNullOrEmpty(name))
                return EmptyMembers;

            var byName = EnsureMembers(type);
            if (byName.TryGetValue(name, out var members))
                return members;

            byName[name] = EmptyMembers;
            return EmptyMembers;
        }

        /// <summary>按类型全名与成员名查找。类型未解析到时返回空数组。</summary>
        public static MemberInfo[] GetMembers(string typeFullName, string name)
        {
            return GetMembers(Get(typeFullName), name);
        }

        /// <summary>按名取第一个字段。</summary>
        public static FieldInfo GetField(Type type, string name)
        {
            return First<FieldInfo>(GetMembers(type, name));
        }

        /// <summary>按类型全名与字段名取第一个字段。</summary>
        public static FieldInfo GetField(string typeFullName, string name)
        {
            return GetField(Get(typeFullName), name);
        }

        /// <summary>按名取第一个属性。</summary>
        public static PropertyInfo GetProperty(Type type, string name)
        {
            return First<PropertyInfo>(GetMembers(type, name));
        }

        /// <summary>按类型全名与属性名取第一个属性。</summary>
        public static PropertyInfo GetProperty(string typeFullName, string name)
        {
            return GetProperty(Get(typeFullName), name);
        }

        /// <summary>按名取方法。重载数量不是恰好 1 个时返回 null。</summary>
        public static MethodInfo GetMethod(Type type, string name)
        {
            return SingleMethod(GetMembers(type, name));
        }

        /// <summary>按类型全名与方法名取方法。重载数量不是恰好 1 个时返回 null。</summary>
        public static MethodInfo GetMethod(string typeFullName, string name)
        {
            return GetMethod(Get(typeFullName), name);
        }

        /// <summary>按名取第一个事件。</summary>
        public static EventInfo GetEvent(Type type, string name)
        {
            return First<EventInfo>(GetMembers(type, name));
        }

        /// <summary>按类型全名与事件名取第一个事件。</summary>
        public static EventInfo GetEvent(string typeFullName, string name)
        {
            return GetEvent(Get(typeFullName), name);
        }

        /// <summary>清空全部缓存。</summary>
        public static void Clear()
        {
            s_types.Clear();
            s_membersByName.Clear();
            s_allMembers.Clear();
        }

        static Dictionary<string, MemberInfo[]> EnsureMembers(Type type)
        {
            if (s_membersByName.TryGetValue(type, out var byName))
                return byName;

            var all = new List<MemberInfo>();
            var lists = new Dictionary<string, List<MemberInfo>>();

            for (var t = type; t != null; t = t.BaseType)
            {
                var declared = t.GetMembers(DeclaredMemberFlags);
                for (int i = 0; i < declared.Length; i++)
                {
                    var member = declared[i];
                    all.Add(member);

                    if (!lists.TryGetValue(member.Name, out var list))
                    {
                        list = new List<MemberInfo>();
                        lists[member.Name] = list;
                    }

                    list.Add(member);
                }
            }

            s_allMembers[type] = all.Count == 0 ? EmptyMembers : all.ToArray();

            byName = new Dictionary<string, MemberInfo[]>(lists.Count);
            foreach (var pair in lists)
                byName[pair.Key] = pair.Value.ToArray();

            s_membersByName[type] = byName;
            return byName;
        }

        static Type ResolveType(string fullName)
        {
            var type = Type.GetType(fullName);
            if (type != null)
                return type;

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
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
                var element = Get(fullName.Substring(0, fullName.Length - 2));
                if (element != null)
                    return element.MakeArrayType();
            }

            return null;
        }

        static T First<T>(MemberInfo[] members) where T : MemberInfo
        {
            for (int i = 0; i < members.Length; i++)
            {
                if (members[i] is T typed)
                    return typed;
            }

            return null;
        }

        static MethodInfo SingleMethod(MemberInfo[] members)
        {
            MethodInfo found = null;
            int count = 0;
            for (int i = 0; i < members.Length; i++)
            {
                if (members[i] is MethodInfo method)
                {
                    found = method;
                    count++;
                    if (count > 1)
                        return null;
                }
            }

            return found;
        }
    }
}
