using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Framework.Core;
using UnityEngine;

namespace Framework.Save
{
    /// <summary>Unity <see cref="PlayerPrefs"/> 存档数据提供者组件</summary>
    public class PlayerPrefsSaveDataProviderComp : SaveDataProviderCompBase
    {
        [Header("存档槽专用键")]
        [Tooltip("已有存档槽列表的键。挂在根名称下，不包含槽号和版本，否则无法枚举其他槽")]
        [SerializeField]
        private string _saveSlotKey = "_SaveSlotKey_";

        [Tooltip("每个存档槽已写入的完整键。挂在根名称下并带槽号，不包含版本号。PlayerPrefs 无法枚举键，删除整个存档时靠这份列表定位")]
        [SerializeField]
        private string _saveSlotDataKeysKey = "_SaveSlotDataKeys_";

        [Tooltip("存档键的根名称")]
        [SerializeField]
        private string _rootName = "Save";
        [Tooltip("存档槽")]
        [SerializeField]
        private int _saveSlot;
        [Tooltip("存档键的版本号。\r\n关于版本管理的逻辑需要玩家自行实现，一般是做数据迁移，自行完成数据适配。\r\n例如按当前版本读取不到数据时，可以按历史版本回溯，找到有数据的旧版本后迁移到新版本并删除旧版本")]
        [SerializeField]
        private string _version = string.Empty;
        [Tooltip("存档键，由于 PlayerPrefs 没有加载所有数据的功能，可主动配置键名用于预加载数据，和主动查看数据等")]
        [SerializeField]
        private PlayerPrefsKeysComp _keys;

        private Dictionary<string, object> _datas = new Dictionary<string, object>();

        /// <summary>已经提示过不可持久化的 Unity 对象类型。同一类型只警告一次。</summary>
        private static readonly HashSet<Type> _unityObjectWarningTypes = new HashSet<Type>();

        /// <summary>存档槽</summary>
        public override int saveSlot { get => _saveSlot; set => _saveSlot = value; }

        /// <summary>存档键</summary>
        public PlayerPrefsKeysComp keys { get => _keys; set => _keys = value; }

        /// <summary>存档键的根名称</summary>
        public string rootName { get => _rootName; set => _rootName = value; }

        /// <summary>存档键的版本号
        /// <para>关于版本管理的逻辑需要玩家自行实现，一般是做数据迁移，自行完成数据适配</para>
        /// </summary>
        public string version { get => _version; set => _version = value; }

        /// <summary>命名空间，会附加到键的前面。
        /// <para>由 <see cref="rootName"/> 和 <see cref="version"/> 拼接而成，
        /// 示例：Save/0/1.0 或 Save/0/，其中版本号是可选的。</para>
        /// </summary>
        public string nameSpace
        {
            get
            {
                var sb = TypePool.root.Get<StringBuilder>();
                string fullKey = sb.Append(_rootName)
                    .Append("/").Append(_saveSlot)
                    .Append("/").Append(_version)
                    .ToString();
                TypePool.root.Return(sb);
                return fullKey;
            }
        }

        public override T GetData<T>(string key)
        {
            key = GetKey(key);
            TrackSlotKeyIfStored(key);
            if (TryReadDirect(key, typeof(T), out object direct))
            {
                return (T)direct;
            }
            if (NeedsWrapper(typeof(T)))
            {
                return (T)ReadWrapped(key, typeof(T));
            }

            // 自定义对象通过json读取
            if (_datas.TryGetValue(key, out var obj))
            {
                if (obj is T typed)
                    return typed;
                return default;
            }

            if (PlayerPrefs.HasKey(key))
            {
                var json = PlayerPrefs.GetString(key);
                T result = JsonUtility.FromJson<T>(json);
                _datas[key] = result;
                return result;
            }
            return default;
        }

        public override object GetData(string key, Type type)
        {
            key = GetKey(key);
            TrackSlotKeyIfStored(key);
            if (TryReadDirect(key, type, out object direct))
            {
                return direct;
            }
            if (NeedsWrapper(type))
            {
                return ReadWrapped(key, type);
            }

            // 自定义对象通过json读取
            if (_datas.TryGetValue(key, out var obj))
            {
                if (obj != null && type.IsInstanceOfType(obj))
                    return obj;
                return null;
            }

            if (PlayerPrefs.HasKey(key))
            {
                var json = PlayerPrefs.GetString(key);
                object result = JsonUtility.FromJson(json, type);
                _datas[key] = result;
                return result;
            }
            return null;
        }

        /// <summary>覆写自定义类型对象</summary>
        public void GetObject(string key, object objectToOverwrite)
        {
            if (objectToOverwrite == null)
            {
                Debug.LogError("对象实例不能为空");
                return;
            }

            Type type = objectToOverwrite.GetType();
            if (type == typeof(int)
                || type == typeof(float)
                || type == typeof(string)
                || NeedsWrapper(type)
                || type.IsValueType
                )
            {
                Debug.Log("对象覆写仅支持自定义引用类型");
                return;
            }

            key = GetKey(key);
            TrackSlotKeyIfStored(key);
            // 自定义对象通过json读取。缓存命中时按序列化字段拷贝，与磁盘上的 FromJsonOverwrite 同一套规则。
            if (_datas.TryGetValue(key, out var obj))
            {
                if (obj != null)
                    CopySerializedObject(obj, objectToOverwrite);
                return;
            }

            if (PlayerPrefs.HasKey(key))
            {
                var json = PlayerPrefs.GetString(key);
                JsonUtility.FromJsonOverwrite(json, objectToOverwrite);
            }
        }

        public override bool TryGetData<T>(string key, out T value)
        {
            if (!HasKey(key))
            {
                value = default;
                return false;
            }

            value = GetData<T>(key);
            return true;
        }

        public override bool TryGetData(string key, Type type, out object value)
        {
            if (!HasKey(key))
            {
                value = null;
                return false;
            }

            value = GetData(key, type);
            return true;
        }

        public override void SetData<T>(string key, T value)
        {
            if (value == null) return;
            WarnIfUnityObjectNotPersistable(value);
            key = GetKey(key);
            TrackSlotKey(_saveSlot, key);
            if (value is int intV)
            {
                _datas.Remove(key);
                PlayerPrefs.SetInt(key, intV);
                return;
            }
            else if (value is float floatV)
            {
                _datas.Remove(key);
                PlayerPrefs.SetFloat(key, floatV);
                return;
            }
            else if (value is string stringV)
            {
                _datas.Remove(key);
                PlayerPrefs.SetString(key, stringV);
                return;
            }
            // PlayerPrefs 不直接支持的标量、数组、列表，以及 UnityEngine.Object 引用，用 SaveValue<T> 转成 json。
            // UnityEngine.Object 只含 instanceID，不能跨启动还原，写入时会警告。
            else if (NeedsWrapper(value.GetType()))
            {
                WriteWrapped(key, value);
                return;
            }

            _datas[key] = value;
        }

        public override void RemoveData(string key)
        {
            key = GetKey(key);
            _datas.Remove(key);
            PlayerPrefs.DeleteKey(key);
            UntrackSlotKey(_saveSlot, key);
        }

        public override void RemoveSaveSlot(int saveSlot)
        {
            DeleteSlotData(saveSlot);
            DeleteSlotDataKeyIndex(saveSlot);

            List<int> slots = LoadSaveSlots();
            if (slots.Remove(saveSlot))
                WriteSaveSlotRegistry(slots);

            PlayerPrefs.Save();
        }

        public override void RemoveAll()
        {
            _datas.Clear();
            PlayerPrefs.DeleteAll();
        }

        public override bool HasKey(string key)
        {
            key = GetKey(key);
            return _datas.ContainsKey(key) || PlayerPrefs.HasKey(key);
        }

        public override void Save()
        {
            TrackCachedSlotKeys();
            RecordCurrentSaveSlot();

            // object数据存成json字符串
            foreach (var data in _datas)
            {
                if (data.Value == null)
                {
                    continue;
                }

                // data.Key 在写入缓存时已经是完整键
                object payload = data.Value;
                string json = NeedsWrapper(payload.GetType())
                    ? ToJsonWrapped(payload)
                    : JsonUtility.ToJson(payload);
                PlayerPrefs.SetString(data.Key, json);
            }

            PlayerPrefs.Save();
        }

        public override void GetAllSaveSlots(List<int> saveSlots)
        {
            if (saveSlots == null)
            {
                return;
            }

            List<int> stored = LoadSaveSlots();
            if (stored.Count > 0)
            {
                saveSlots.AddRange(stored);
            }
        }

        public override void ClearCache()
        {
            _datas.Clear();
        }

        /// <summary>获取完整键
        /// <para>完整键名示例：Save/0/1.0/name 或 Save/0//name，其中版本号是可选的。</para>
        /// </summary>
        public string GetKey(string key)
        {
            return BuildFullKey(_saveSlot, _version, key);
        }

        public void ForEach(Action<string, object> callback)
        {
            if (_keys)
            {
                foreach (var key in _keys.keys)
                {
                    Type type = key.valueType;
                    object value = default;
                    if (HasKey(key.key))
                    {
                        value = GetData(key.key, type);
                    }
                    callback?.Invoke(GetKey(key.key), value);
                }
            }
        }

        /// <summary>把当前 <see cref="saveSlot"/> 记入存档槽列表。列表键不走 <see cref="GetKey"/>，避免带上槽号后只能看到当前槽。</summary>
        private void RecordCurrentSaveSlot()
        {
            List<int> slots = LoadSaveSlots();
            if (slots.Contains(_saveSlot))
            {
                return;
            }

            slots.Add(_saveSlot);
            slots.Sort();
            _datas[GetSaveSlotRegistryKey()] = new SaveValue<List<int>> { value = slots };
        }

        private List<int> LoadSaveSlots()
        {
            string key = GetSaveSlotRegistryKey();
            if (_datas.TryGetValue(key, out var cached) && cached is SaveValue<List<int>> cachedWrapped)
            {
                if (cachedWrapped.value == null)
                {
                    cachedWrapped.value = new List<int>();
                }
                return cachedWrapped.value;
            }

            if (PlayerPrefs.HasKey(key))
            {
                SaveValue<List<int>> wrapped = JsonUtility.FromJson<SaveValue<List<int>>>(PlayerPrefs.GetString(key));
                if (wrapped != null)
                {
                    if (wrapped.value == null)
                    {
                        wrapped.value = new List<int>();
                    }
                    _datas[key] = wrapped;
                    return wrapped.value;
                }
            }

            return new List<int>();
        }

        /// <summary>存档槽列表的完整键，示例：Save/_SaveSlotKey_</summary>
        private string GetSaveSlotRegistryKey()
        {
            var sb = TypePool.root.Get<StringBuilder>();
            string key = sb.Append(_rootName)
                .Append("/").Append(_saveSlotKey)
                .ToString();
            TypePool.root.Return(sb);
            return key;
        }

        /// <summary>读取 PlayerPrefs 直接支持的 int、float、string</summary>
        private static bool TryReadDirect(string key, Type type, out object value)
        {
            if (type == typeof(int))
            {
                value = PlayerPrefs.GetInt(key);
                return true;
            }
            if (type == typeof(float))
            {
                value = PlayerPrefs.GetFloat(key);
                return true;
            }
            if (type == typeof(string))
            {
                value = PlayerPrefs.GetString(key);
                return true;
            }

            value = null;
            return false;
        }

        /// <summary>PlayerPrefs 不能直接保存的标量、数组、列表，以及 <see cref="UnityEngine.Object"/>。对象引用不能跨启动还原。</summary>
        private static bool NeedsWrapper(Type type)
        {
            if (type == null)
            {
                return false;
            }

            return type == typeof(bool)
                || type == typeof(short)
                || type == typeof(ushort)
                || type == typeof(uint)
                || type == typeof(long)
                || type == typeof(ulong)
                || type == typeof(double)
                || type.IsEnum
                || type.IsArray
                || IsList(type)
                || typeof(UnityEngine.Object).IsAssignableFrom(type);
        }

        private static bool IsList(Type type)
        {
            Type current = type;
            while (current != null && current != typeof(object))
            {
                if (current.IsGenericType && current.GetGenericTypeDefinition() == typeof(List<>))
                {
                    return true;
                }
                current = current.BaseType;
            }
            return false;
        }

        private void WriteWrapped(string key, object value)
        {
            _datas[key] = value;
            PlayerPrefs.SetString(key, ToJsonWrapped(value));
        }

        private object ReadWrapped(string key, Type type)
        {
            if (_datas.TryGetValue(key, out var cached))
            {
                if (cached != null && type.IsInstanceOfType(cached))
                {
                    return cached;
                }
                return DefaultOf(type);
            }

            if (!PlayerPrefs.HasKey(key))
            {
                return DefaultOf(type);
            }

            // 旧数据按 int/float 写入时，GetString 为空，仍按原来的方式读取
            string json = PlayerPrefs.GetString(key);
            object result = string.IsNullOrEmpty(json)
                ? ReadLegacyScalar(key, type)
                : UnwrapSaveValue(json, type);

            // 列表和数组放进缓存。之后改的是同一个实例，Save 才会把修改写回去。
            if (result != null && (type.IsArray || IsList(type)) && type.IsInstanceOfType(result))
            {
                _datas[key] = result;
            }
            return result;
        }

        private static object ReadLegacyScalar(string key, Type type)
        {
            if (type == typeof(bool))
            {
                return PlayerPrefs.GetInt(key) == 1;
            }
            if (type == typeof(double))
            {
                return (double)PlayerPrefs.GetFloat(key);
            }
            if (type.IsEnum)
            {
                return Enum.ToObject(type, PlayerPrefs.GetInt(key));
            }

            int intValue = PlayerPrefs.GetInt(key);
            if (type == typeof(short))
            {
                return (short)intValue;
            }
            if (type == typeof(ushort))
            {
                return (ushort)intValue;
            }
            if (type == typeof(uint))
            {
                return (uint)intValue;
            }
            if (type == typeof(long))
            {
                return (long)intValue;
            }
            if (type == typeof(ulong))
            {
                return (ulong)intValue;
            }
            return DefaultOf(type);
        }

        /// <summary>按值的实际类型构造 <see cref="SaveValue{T}"/>。数组和列表不能作为 JsonUtility 的根对象。Unity 对象只保存 instanceID。</summary>
        private static object CreateSaveValue(object value)
        {
            Type wrapperType = typeof(SaveValue<>).MakeGenericType(value.GetType());
            object wrapper = TypePool.root.Get(wrapperType);
            wrapperType.GetField("value").SetValue(wrapper, value);
            return wrapper;
        }

        /// <summary>把临时包装序列化成 JSON，然后归还对象池。缓存里长期持有的包装不要走这里。</summary>
        private static string ToJsonWrapped(object value)
        {
            object wrapper = CreateSaveValue(value);
            try
            {
                return JsonUtility.ToJson(wrapper);
            }
            finally
            {
                ReleaseSaveValue(wrapper);
            }
        }

        private static void ReleaseSaveValue(object wrapper)
        {
            if (wrapper == null)
                return;

            FieldInfo field = wrapper.GetType().GetField("value");
            if (field != null && !field.FieldType.IsValueType)
                field.SetValue(wrapper, null);
            TypePool.root.Return(wrapper);
        }

        private static object UnwrapSaveValue(string json, Type type)
        {
            Type wrapperType = typeof(SaveValue<>).MakeGenericType(type);
            object wrapper = JsonUtility.FromJson(json, wrapperType);
            if (wrapper == null)
            {
                return DefaultOf(type);
            }

            FieldInfo field = wrapperType.GetField("value");
            object result = field.GetValue(wrapper);
            return result ?? DefaultOf(type);
        }

        private static object DefaultOf(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        /// <summary>按 <see cref="JsonUtility"/> 的字段规则把缓存对象写入目标。覆盖公共字段和带 <see cref="SerializeField"/> 的非公共字段，不包含静态字段和属性。</summary>
        private static void CopySerializedObject(object source, object target)
        {
            string json = JsonUtility.ToJson(source);
            if (string.IsNullOrEmpty(json))
                return;

            JsonUtility.FromJsonOverwrite(json, target);
        }

        /// <summary><see cref="UnityEngine.Object"/> 写入的是 instanceID，重新启动后无法还原。同一具体类型只提示一次。</summary>
        private void WarnIfUnityObjectNotPersistable(object value)
        {
            Type type = GetPersistableUnityObjectType(value);
            if (type == null || !_unityObjectWarningTypes.Add(type))
                return;

            Debug.LogWarning(
                "存档不能持久化 " + type.FullName
                + "。UnityEngine.Object 只会记下 instanceID，重新启动后引用会丢失。请改为保存资源路径或自定义标识。",
                this);
        }

        /// <summary>值本身是 Unity 对象，或数组、列表的元素是 Unity 对象时，返回该对象类型。</summary>
        private static Type GetPersistableUnityObjectType(object value)
        {
            if (value == null)
                return null;

            Type type = value.GetType();
            if (typeof(UnityEngine.Object).IsAssignableFrom(type))
                return type;

            Type element = null;
            if (type.IsArray)
                element = type.GetElementType();
            else
                element = GetListElementType(type);

            if (element != null && typeof(UnityEngine.Object).IsAssignableFrom(element))
                return element;
            return null;
        }

        private static Type GetListElementType(Type type)
        {
            Type current = type;
            while (current != null && current != typeof(object))
            {
                if (current.IsGenericType && current.GetGenericTypeDefinition() == typeof(List<>))
                    return current.GetGenericArguments()[0];
                current = current.BaseType;
            }
            return null;
        }

        /// <summary>完整键，示例：Save/0/1.0/name。版本为空时是 Save/0//name。</summary>
        private string BuildFullKey(int slot, string version, string key)
        {
            var sb = TypePool.root.Get<StringBuilder>();
            string fullKey = sb.Append(_rootName)
                .Append("/").Append(slot)
                .Append("/").Append(version)
                .Append("/").Append(key)
                .ToString();
            TypePool.root.Return(sb);
            return fullKey;
        }

        /// <summary>槽位数据的键前缀，示例：Save/0/。末尾带分隔符，避免槽 1 匹配到槽 10。</summary>
        private string GetSlotDataPrefix(int slot)
        {
            var sb = TypePool.root.Get<StringBuilder>();
            string prefix = sb.Append(_rootName)
                .Append("/").Append(slot)
                .Append("/")
                .ToString();
            TypePool.root.Return(sb);
            return prefix;
        }

        /// <summary>某个存档槽的键列表，示例：Save/_SaveSlotDataKeys_/0</summary>
        private string GetSlotDataKeysRegistryKey(int slot)
        {
            var sb = TypePool.root.Get<StringBuilder>();
            string key = sb.Append(_rootName)
                .Append("/").Append(_saveSlotDataKeysKey)
                .Append("/").Append(slot)
                .ToString();
            TypePool.root.Return(sb);
            return key;
        }

        /// <summary>从完整数据键解析槽号。键列表和槽登记本身的键会返回 false。</summary>
        private bool TryGetSlotFromDataKey(string fullKey, out int slot)
        {
            slot = 0;
            if (string.IsNullOrEmpty(fullKey) || string.IsNullOrEmpty(_rootName))
                return false;

            string prefix = _rootName + "/";
            if (!fullKey.StartsWith(prefix, StringComparison.Ordinal))
                return false;

            int start = prefix.Length;
            int end = fullKey.IndexOf('/', start);
            if (end <= start)
                return false;

            return int.TryParse(fullKey.Substring(start, end - start), out slot);
        }

        /// <summary>键已经在缓存或 PlayerPrefs 里时，补进槽位键列表。用来把索引建立前写入的旧键纳进来。</summary>
        private void TrackSlotKeyIfStored(string fullKey)
        {
            if (string.IsNullOrEmpty(fullKey))
                return;
            if (!_datas.ContainsKey(fullKey) && !PlayerPrefs.HasKey(fullKey))
                return;
            if (!TryGetSlotFromDataKey(fullKey, out int slot))
                return;

            TrackSlotKey(slot, fullKey);
        }

        /// <summary>把缓存里各槽的数据键补进键列表。遍历期间不能改 <see cref="_datas"/>。</summary>
        private void TrackCachedSlotKeys()
        {
            List<string> keys = TypePool.root.GetList<string>();
            try
            {
                foreach (var data in _datas)
                {
                    if (!TryGetSlotFromDataKey(data.Key, out _))
                        continue;
                    keys.Add(data.Key);
                }

                for (int i = 0; i < keys.Count; i++)
                {
                    if (TryGetSlotFromDataKey(keys[i], out int slot))
                        TrackSlotKey(slot, keys[i]);
                }
            }
            finally
            {
                TypePool.root.Return(keys);
            }
        }

        private void TrackSlotKey(int slot, string fullKey)
        {
            if (string.IsNullOrEmpty(fullKey))
                return;

            List<string> keys = LoadSlotDataKeys(slot);
            if (keys.Contains(fullKey))
                return;

            keys.Add(fullKey);
            WriteSlotDataKeys(slot, keys);
        }

        private void UntrackSlotKey(int slot, string fullKey)
        {
            List<string> keys = LoadSlotDataKeys(slot);
            if (!keys.Remove(fullKey))
                return;

            WriteSlotDataKeys(slot, keys);
        }

        private List<string> LoadSlotDataKeys(int slot)
        {
            string key = GetSlotDataKeysRegistryKey(slot);
            if (_datas.TryGetValue(key, out var cached) && cached is SaveValue<List<string>> cachedWrapped)
            {
                if (cachedWrapped.value == null)
                    cachedWrapped.value = new List<string>();
                return cachedWrapped.value;
            }

            if (PlayerPrefs.HasKey(key))
            {
                SaveValue<List<string>> wrapped = JsonUtility.FromJson<SaveValue<List<string>>>(PlayerPrefs.GetString(key));
                if (wrapped != null)
                {
                    if (wrapped.value == null)
                        wrapped.value = new List<string>();
                    _datas[key] = wrapped;
                    return wrapped.value;
                }
            }

            return new List<string>();
        }

        private void WriteSlotDataKeys(int slot, List<string> keys)
        {
            var wrapped = new SaveValue<List<string>> { value = keys ?? new List<string>() };
            string key = GetSlotDataKeysRegistryKey(slot);
            _datas[key] = wrapped;
            PlayerPrefs.SetString(key, JsonUtility.ToJson(wrapped));
        }

        private void WriteSaveSlotRegistry(List<int> slots)
        {
            var wrapped = new SaveValue<List<int>> { value = slots ?? new List<int>() };
            string key = GetSaveSlotRegistryKey();
            _datas[key] = wrapped;
            PlayerPrefs.SetString(key, JsonUtility.ToJson(wrapped));
        }

        /// <summary>删除一个槽里已登记的键，以及当前版本下在 <see cref="keys"/> 里配置过的键。</summary>
        private void DeleteSlotData(int slot)
        {
            List<string> fullKeys = TypePool.root.GetList<string>();
            try
            {
                List<string> tracked = LoadSlotDataKeys(slot);
                for (int i = 0; i < tracked.Count; i++)
                    fullKeys.Add(tracked[i]);

                if (_keys != null && _keys.keys != null)
                {
                    for (int i = 0; i < _keys.keys.Count; i++)
                    {
                        PlayerPrefsKeysComp.Key entry = _keys.keys[i];
                        if (entry == null || string.IsNullOrEmpty(entry.key))
                            continue;

                        string configured = BuildFullKey(slot, _version, entry.key);
                        if (!fullKeys.Contains(configured))
                            fullKeys.Add(configured);
                    }
                }

                for (int i = 0; i < fullKeys.Count; i++)
                {
                    _datas.Remove(fullKeys[i]);
                    PlayerPrefs.DeleteKey(fullKeys[i]);
                }

                RemoveCachedKeysWithPrefix(GetSlotDataPrefix(slot));
            }
            finally
            {
                TypePool.root.Return(fullKeys);
            }
        }

        private void RemoveCachedKeysWithPrefix(string prefix)
        {
            List<string> matched = TypePool.root.GetList<string>();
            try
            {
                foreach (var data in _datas)
                {
                    if (data.Key.StartsWith(prefix, StringComparison.Ordinal))
                        matched.Add(data.Key);
                }

                for (int i = 0; i < matched.Count; i++)
                {
                    _datas.Remove(matched[i]);
                    PlayerPrefs.DeleteKey(matched[i]);
                }
            }
            finally
            {
                TypePool.root.Return(matched);
            }
        }

        private void DeleteSlotDataKeyIndex(int slot)
        {
            string key = GetSlotDataKeysRegistryKey(slot);
            _datas.Remove(key);
            PlayerPrefs.DeleteKey(key);
        }
    }
}
