using System;
using System.Collections.Generic;
using Framework.Save;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    /// <summary>
    /// <see cref="PlayerPrefsSaveDataProviderComp"/> 检视面板：
    /// 保持默认绘制，只读显示 <see cref="PlayerPrefsSaveDataProviderComp.nameSpace"/>，
    /// 并列出 PlayerPrefs 里已保存的存档槽和对应键。
    /// </summary>
    [CustomEditor(typeof(PlayerPrefsSaveDataProviderComp))]
    [CanEditMultipleObjects]
    public class PlayerPrefsSaveDataProviderCompInspector : UnityEditor.Editor
    {
        static readonly GUIContent NameSpaceLabel = new GUIContent(
            "nameSpace",
            "命名空间，会附加到键的前面。由根名称、应用名、存档槽和版本号拼接而成，示例：Save/应用名/0/1.0 或 Save/应用名/0/。");

        readonly List<int> _slots = new List<int>();
        readonly List<string> _keys = new List<string>();
        readonly HashSet<string> _drawnRegistries = new HashSet<string>();
        readonly Dictionary<string, bool> _slotFoldouts = new Dictionary<string, bool>();

        bool _savedFoldout = true;

        public override bool RequiresConstantRepaint() => Application.isPlaying;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            DrawNameSpace();
            DrawSavedSaves();
            DrawDeleteAllButton();
        }

        void DrawNameSpace()
        {
            EditorGUILayout.Space();

            string nameSpace = null;
            bool mixed = false;
            for (int i = 0; i < targets.Length; i++)
            {
                string value = ((PlayerPrefsSaveDataProviderComp)targets[i]).nameSpace;
                if (i == 0)
                    nameSpace = value;
                else if (nameSpace != value)
                {
                    mixed = true;
                    break;
                }
            }

            EditorGUI.showMixedValue = mixed;
            using (new EditorGUI.DisabledScope(true))
                EditorGUILayout.TextField(NameSpaceLabel, nameSpace ?? string.Empty);
            EditorGUI.showMixedValue = false;
        }

        /// <summary>
        /// 槽列表键是 <c>根名称/应用名/存档槽列表键</c>，每个槽的数据键列表是 <c>根名称/应用名/槽数据键列表键/槽号</c>。
        /// 与运行时登记存档槽、槽内键的规则一致，直接读 PlayerPrefs，不经过内存缓存。
        /// </summary>
        void DrawSavedSaves()
        {
            EditorGUILayout.Space();
            _savedFoldout = EditorGUILayout.Foldout(_savedFoldout, "已保存的存档", true);
            if (!_savedFoldout)
                return;

            _drawnRegistries.Clear();
            bool any = false;
            for (int i = 0; i < targets.Length; i++)
            {
                var provider = targets[i] as PlayerPrefsSaveDataProviderComp;
                if (!provider)
                    continue;
                if (!TryReadNaming(provider, out string rootPrefix, out string slotKeyName, out string dataKeysName))
                    continue;

                string registryKey = rootPrefix + "/" + slotKeyName;
                if (!_drawnRegistries.Add(registryKey))
                    continue;

                if (!DrawRegistry(provider, rootPrefix, registryKey, dataKeysName))
                    continue;

                any = true;
            }

            if (!any)
                EditorGUILayout.HelpBox("没有已保存的存档。", MessageType.Info);
        }

        /// <summary>清空 PlayerPrefs。会删掉本项目的全部键，不只是当前组件登记过的存档。</summary>
        void DrawDeleteAllButton()
        {
            EditorGUILayout.Space();
            if (!GUILayout.Button("删除所有数据"))
                return;

            if (!EditorUtility.DisplayDialog(
                "删除所有数据",
                "将清空 PlayerPrefs 中的全部数据，包括其他系统写入的键。此操作不能撤销。",
                "删除",
                "取消"))
                return;

            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i] is PlayerPrefsSaveDataProviderComp provider)
                    provider.RemoveAll();
            }
        }

        bool DrawRegistry(PlayerPrefsSaveDataProviderComp provider, string rootPrefix, string registryKey, string dataKeysName)
        {
            CollectSlots(provider, rootPrefix, registryKey, dataKeysName);
            if (_slots.Count == 0)
                return false;

            if (targets.Length > 1)
                EditorGUILayout.LabelField(string.IsNullOrEmpty(rootPrefix) ? provider.name : rootPrefix, EditorStyles.boldLabel);

            EditorGUI.indentLevel++;
            for (int i = 0; i < _slots.Count; i++)
                DrawSlot(provider, rootPrefix, dataKeysName, _slots[i]);
            EditorGUI.indentLevel--;
            return true;
        }

        void DrawSlot(PlayerPrefsSaveDataProviderComp provider, string rootPrefix, string dataKeysName, int slot)
        {
            CollectKeys(provider, rootPrefix, dataKeysName, slot);

            string foldoutId = rootPrefix + "/" + dataKeysName + "/" + slot;
            if (!_slotFoldouts.TryGetValue(foldoutId, out bool open))
                open = true;

            string title = slot == provider.saveSlot
                ? $"存档槽 {slot}（当前，{_keys.Count}）"
                : $"存档槽 {slot}（{_keys.Count}）";
            open = EditorGUILayout.Foldout(open, title, true);
            _slotFoldouts[foldoutId] = open;
            if (!open)
                return;

            if (_keys.Count == 0)
            {
                EditorGUILayout.LabelField("（无键）");
                return;
            }

            EditorGUI.indentLevel++;
            for (int i = 0; i < _keys.Count; i++)
                DrawKey(provider, slot, _keys[i]);
            EditorGUI.indentLevel--;
        }

        void DrawKey(PlayerPrefsSaveDataProviderComp provider, int slot, string fullKey)
        {
            Type valueType = FindConfiguredType(provider, slot, fullKey);
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.TextField("键", fullKey);
                EditorGUILayout.TextField("值", DescribeValue(fullKey, valueType));
            }
        }

        void CollectSlots(PlayerPrefsSaveDataProviderComp provider, string rootPrefix, string registryKey, string dataKeysName)
        {
            _slots.Clear();
            ReadIntList(registryKey, _slots);

            int current = provider.saveSlot;
            if (!_slots.Contains(current))
            {
                CollectKeys(provider, rootPrefix, dataKeysName, current);
                if (_keys.Count > 0)
                    _slots.Add(current);
            }

            _slots.Sort();
            for (int i = _slots.Count - 1; i > 0; i--)
            {
                if (_slots[i] == _slots[i - 1])
                    _slots.RemoveAt(i);
            }
        }

        /// <summary>槽位索引里的完整键，再加上当前槽在键表里已经写入 PlayerPrefs 的键。</summary>
        void CollectKeys(PlayerPrefsSaveDataProviderComp provider, string rootPrefix, string dataKeysName, int slot)
        {
            _keys.Clear();
            ReadStringList(rootPrefix + "/" + dataKeysName + "/" + slot, _keys);

            if (slot == provider.saveSlot)
                AppendConfiguredKeys(provider, _keys);

            _keys.Sort(StringComparer.Ordinal);
        }

        static void AppendConfiguredKeys(PlayerPrefsSaveDataProviderComp provider, List<string> keys)
        {
            PlayerPrefsKeysComp keyComp = provider.keys;
            if (keyComp == null || keyComp.keys == null)
                return;

            for (int i = 0; i < keyComp.keys.Count; i++)
            {
                PlayerPrefsKeysComp.Key entry = keyComp.keys[i];
                if (entry == null || string.IsNullOrEmpty(entry.key))
                    continue;

                string fullKey = provider.GetKey(entry.key);
                if (!PlayerPrefs.HasKey(fullKey) || keys.Contains(fullKey))
                    continue;

                keys.Add(fullKey);
            }
        }

        static Type FindConfiguredType(PlayerPrefsSaveDataProviderComp provider, int slot, string fullKey)
        {
            if (slot != provider.saveSlot)
                return null;

            PlayerPrefsKeysComp keyComp = provider.keys;
            if (keyComp == null || keyComp.keys == null)
                return null;

            for (int i = 0; i < keyComp.keys.Count; i++)
            {
                PlayerPrefsKeysComp.Key entry = keyComp.keys[i];
                if (entry == null || string.IsNullOrEmpty(entry.key))
                    continue;
                if (provider.GetKey(entry.key) == fullKey)
                    return entry.valueType;
            }

            return null;
        }

        static bool TryReadNaming(
            PlayerPrefsSaveDataProviderComp provider,
            out string rootPrefix,
            out string slotKeyName,
            out string dataKeysName)
        {
            rootPrefix = provider.rootPrefix ?? string.Empty;
            var serialized = new SerializedObject(provider);
            SerializedProperty slotKey = serialized.FindProperty("_saveSlotKey");
            SerializedProperty dataKeys = serialized.FindProperty("_saveSlotDataKeysKey");
            slotKeyName = slotKey != null ? slotKey.stringValue : string.Empty;
            dataKeysName = dataKeys != null ? dataKeys.stringValue : string.Empty;
            return !string.IsNullOrEmpty(slotKeyName) && !string.IsNullOrEmpty(dataKeysName);
        }

        static void ReadIntList(string prefsKey, List<int> result)
        {
            result.Clear();
            if (!TryReadJson(prefsKey, out SaveValue<List<int>> wrapped) || wrapped.value == null)
                return;

            for (int i = 0; i < wrapped.value.Count; i++)
                result.Add(wrapped.value[i]);
        }

        static void ReadStringList(string prefsKey, List<string> result)
        {
            result.Clear();
            if (!TryReadJson(prefsKey, out SaveValue<List<string>> wrapped) || wrapped.value == null)
                return;

            for (int i = 0; i < wrapped.value.Count; i++)
            {
                if (!string.IsNullOrEmpty(wrapped.value[i]))
                    result.Add(wrapped.value[i]);
            }
        }

        static bool TryReadJson<T>(string prefsKey, out T value) where T : class
        {
            value = null;
            if (!PlayerPrefs.HasKey(prefsKey))
                return false;

            string json = PlayerPrefs.GetString(prefsKey);
            if (string.IsNullOrEmpty(json))
                return false;

            try
            {
                value = JsonUtility.FromJson<T>(json);
            }
            catch (Exception)
            {
                return false;
            }

            return value != null;
        }

        /// <summary>按键表里的类型读取。没有类型时，非空字符串按原文显示，否则区分整数和浮点。</summary>
        static string DescribeValue(string fullKey, Type type)
        {
            if (!PlayerPrefs.HasKey(fullKey))
                return "（未写入）";

            if (type == typeof(int))
                return PlayerPrefs.GetInt(fullKey).ToString();
            if (type == typeof(float))
                return PlayerPrefs.GetFloat(fullKey).ToString();
            if (type == typeof(string))
                return PlayerPrefs.GetString(fullKey);

            string text = PlayerPrefs.GetString(fullKey);
            if (type != null || !string.IsNullOrEmpty(text))
                return text;

            float number = PlayerPrefs.GetFloat(fullKey);
            int integer = PlayerPrefs.GetInt(fullKey);
            if (!Mathf.Approximately(number, integer))
                return number.ToString();
            return integer.ToString();
        }
    }
}
