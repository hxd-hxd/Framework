using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>Unity 可序列化字典</summary>
    [Serializable]
    public class DictionarySerializable<TKey, TValue> :
        ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, IDictionary<TKey, TValue>, IReadOnlyCollection<KeyValuePair<TKey, TValue>>, IReadOnlyDictionary<TKey, TValue>, ICollection, IDictionary,
        ISerializationCallbackReceiver
    {
        // 不可序列化字典
        [NonSerialized]
        private Dictionary<TKey, TValue> _dict;

        /// <summary>代码改过字典后，下次序列化才把字典回写到列表。</summary>
        [NonSerialized]
        private bool _dictMutated;

        // 可序列化列表
        [SerializeField]
        [HideInInspector]
        private List<TKey> _keys;
        [SerializeField]
        [HideInInspector]
        private List<TValue> _values;

        [SerializeField]
        [HideInInspector]
        private string _comparerClassName;

        private static readonly HashSet<string> s_unresolvedComparers = new HashSet<string>();

        #region 构造函数
        public DictionarySerializable()
        {
            _dict = new Dictionary<TKey, TValue>();
        }
        public DictionarySerializable(IDictionary<TKey, TValue> dictionary)
        {
            _dict = new Dictionary<TKey, TValue>(dictionary);
            Touch();
        }
        public DictionarySerializable(IEnumerable<KeyValuePair<TKey, TValue>> collection)
        {
            _dict = new Dictionary<TKey, TValue>(collection);
            Touch();
        }
        public DictionarySerializable(IEqualityComparer<TKey> comparer)
        {
            _dict = new Dictionary<TKey, TValue>(comparer);
            RememberComparer(comparer);
        }
        public DictionarySerializable(int capacity)
        {
            _dict = new Dictionary<TKey, TValue>(capacity);
        }
        public DictionarySerializable(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
        {
            _dict = new Dictionary<TKey, TValue>(dictionary, comparer);
            RememberComparer(comparer);
            Touch();
        }
        public DictionarySerializable(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer)
        {
            _dict = new Dictionary<TKey, TValue>(collection, comparer);
            RememberComparer(comparer);
            Touch();
        }
        public DictionarySerializable(int capacity, IEqualityComparer<TKey> comparer)
        {
            _dict = new Dictionary<TKey, TValue>(capacity, comparer);
            RememberComparer(comparer);
        }
        #endregion

        #region 属性
        public TValue this[TKey key]
        {
            get => _dict[key];
            set
            {
                _dict[key] = value;
                Touch();
            }
        }

        public Dictionary<TKey, TValue>.KeyCollection Keys => _dict.Keys;
        public Dictionary<TKey, TValue>.ValueCollection Values => _dict.Values;
        public IEqualityComparer<TKey> Comparer => _dict.Comparer;
        public int Count => _dict.Count;

        #endregion

        #region 方法
        public void Add(TKey key, TValue value)
        {
            _dict.Add(key, value);
            Touch();
        }
        public void Clear()
        {
            _dict.Clear();
            Touch();
        }
        public bool ContainsKey(TKey key) => _dict.ContainsKey(key);
        public bool ContainsValue(TValue value) => _dict.ContainsValue(value);
        public int EnsureCapacity(int capacity) => _dict.EnsureCapacity(capacity);
        public Dictionary<TKey, TValue>.Enumerator GetEnumerator() => _dict.GetEnumerator();
        public bool Remove(TKey key, out TValue value)
        {
            var result = _dict.Remove(key, out value);
            if (result)
                Touch();
            return result;
        }
        public bool Remove(TKey key)
        {
            var result = _dict.Remove(key);
            if (result)
                Touch();
            return result;
        }
        public void TrimExcess()
        {
            _dict.TrimExcess(_dict.Count);
            _keys?.TrimExcess();
            _values?.TrimExcess();
        }
        public void TrimExcess(int capacity)
        {
            _dict.TrimExcess(capacity);
            SetListCapacity(_keys, capacity);
            SetListCapacity(_values, capacity);
        }
        public bool TryAdd(TKey key, TValue value)
        {
            bool result = _dict.TryAdd(key, value);
            if (result)
                Touch();
            return result;
        }
        public bool TryGetValue(TKey key, out TValue value) => _dict.TryGetValue(key, out value);
        #endregion

        /// <summary>标记字典已被代码修改，序列化前需要回写列表。</summary>
        private void Touch() => _dictMutated = true;

        private static bool IsAbsentKey(TKey key)
        {
            if (Equals(key, null))
                return true;

            return key is UnityEngine.Object unityObject && unityObject == null;
        }

        private void RememberComparer(IEqualityComparer<TKey> comparer)
        {
            if (comparer != null)
                _comparerClassName = comparer.GetType().AssemblyQualifiedName;
        }

        private static Type ResolveType(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type != null)
                return type;

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                type = assemblies[i].GetType(typeName);
                if (type != null)
                    return type;
            }

            return null;
        }

        private void SetComparer()
        {
            if (string.IsNullOrWhiteSpace(_comparerClassName))
                return;

            try
            {
                var comparerType = ResolveType(_comparerClassName);
                if (Activator.CreateInstance(comparerType) is IEqualityComparer<TKey> comparer)
                {
                    _dict = new Dictionary<TKey, TValue>(comparer);
                    return;
                }
            }
            catch (Exception)
            {
                // 类型找不到、没有公有无参构造，或被 IL2CPP 裁掉
            }

            if (s_unresolvedComparers.Add(_comparerClassName))
                Debug.LogWarning($"DictionarySerializable 无法还原比较器 \"{_comparerClassName}\"，已使用默认比较器。");
        }

        private static void SetListCapacity<T>(List<T> list, int capacity)
        {
            if (list == null)
                return;

            int target = Math.Max(capacity, list.Count);
            if (list.Capacity != target)
                list.Capacity = target;
        }

        private void SyncToDict()
        {
            _dict ??= new Dictionary<TKey, TValue>();
            _dict.Clear();

            if (_keys == null || _values == null) return;

            int count = Math.Min(_keys.Count, _values.Count);
            for (int i = 0; i < count; i++)
            {
                var key = _keys[i];
                if (IsAbsentKey(key) || _dict.ContainsKey(key))
                    continue;
                _dict.Add(key, _values[i]);
            }
        }

        private void SyncToList()
        {
            _keys ??= new List<TKey>(_dict.Count);
            _values ??= new List<TValue>(_dict.Count);
            _keys.Clear();
            _values.Clear();

            foreach (var item in _dict)
            {
                _keys.Add(item.Key);
                _values.Add(item.Value);
            }
        }

        #region Unity 序列化处理
        // 反序列化之后。列表是检视面板编辑的数据源。
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            try
            {
                SetComparer();
            }
            finally
            {
                SyncToDict();
                _dictMutated = false;
            }
        }

        // 序列化之前。只有代码改过字典才回写列表，避免刷新检视面板时覆盖列表。
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (!_dictMutated || _dict == null)
                return;

            SyncToList();
            _dictMutated = false;
        }
        #endregion

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => (_dict as ICollection<KeyValuePair<TKey, TValue>>).IsReadOnly;

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;

        ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

        bool ICollection.IsSynchronized => (_dict as ICollection).IsSynchronized;

        object ICollection.SyncRoot => (_dict as ICollection).SyncRoot;

        bool IDictionary.IsFixedSize => (_dict as IDictionary).IsFixedSize;

        bool IDictionary.IsReadOnly => (_dict as IDictionary).IsReadOnly;

        ICollection IDictionary.Keys => Keys;

        ICollection IDictionary.Values => Values;

        object IDictionary.this[object key]
        {
            get => (_dict as IDictionary)[key];
            set
            {
                (_dict as IDictionary)[key] = value;
                Touch();
            }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return (_dict as ICollection<KeyValuePair<TKey, TValue>>).Contains(item);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            (_dict as ICollection<KeyValuePair<TKey, TValue>>).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            var result = (_dict as ICollection<KeyValuePair<TKey, TValue>>).Remove(item);
            if (result)
                Touch();
            return result;
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void ICollection.CopyTo(Array array, int index)
        {
            (_dict as ICollection).CopyTo(array, index);
        }

        void IDictionary.Add(object key, object value)
        {
            (_dict as IDictionary).Add(key, value);
            Touch();
        }

        void IDictionary.Remove(object key)
        {
            var dict = (IDictionary)_dict;
            if (!dict.Contains(key))
                return;

            dict.Remove(key);
            Touch();
        }

        bool IDictionary.Contains(object key)
        {
            return (_dict as IDictionary).Contains(key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
