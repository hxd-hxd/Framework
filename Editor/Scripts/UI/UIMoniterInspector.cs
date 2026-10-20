using System;
using System.Collections.Generic;
using Framework.UI;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    /// <summary>
    /// <see cref="UIMoniter"/> 检视面板：保持默认绘制，并显示已注册的 <see cref="IUI"/>、
    /// <see cref="IUIGroup"/>、<see cref="IUIGroupItem.topmostIgnore"/> 以及 <see cref="UIManager.GetTopUI"/>。
    /// </summary>
    [CustomEditor(typeof(UIMoniter))]
    public class UIMoniterInspector : UnityEditor.Editor
    {
        readonly List<KeyValuePair<Type, IUI>> _uiList = new List<KeyValuePair<Type, IUI>>();
        readonly List<KeyValuePair<string, IUIGroup>> _groupList = new List<KeyValuePair<string, IUIGroup>>();
        readonly List<IUIGroupItem> _itemBuffer = new List<IUIGroupItem>();

        bool _uiFoldout = true;
        bool _groupFoldout = true;

        public override bool RequiresConstantRepaint() => Application.isPlaying;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space();

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("进入播放模式后显示 UI 与 UI 分组。", MessageType.Info);
                return;
            }

            if (!TrySnapshot(out IUI topUI, out IUIGroup topGroup))
            {
                EditorGUILayout.HelpBox("UI 列表正在变化，稍后刷新。", MessageType.None);
                return;
            }

            DrawTop(topUI, topGroup);
            DrawUIs(topUI);
            DrawGroups(topGroup);
        }

        bool TrySnapshot(out IUI topUI, out IUIGroup topGroup)
        {
            topUI = null;
            topGroup = null;
            try
            {
                UIManager manager = UIManager.Instance;
                topUI = manager.GetTopUI();
                topGroup = manager.GetTopUIGroup();

                _uiList.Clear();
                foreach (var kv in manager.uis)
                    _uiList.Add(kv);

                _groupList.Clear();
                foreach (var kv in manager.groups)
                    _groupList.Add(kv);
            }
            catch (InvalidOperationException)
            {
                return false;
            }

            _uiList.Sort(CompareUI);
            _groupList.Sort(CompareGroup);
            return true;
        }

        void DrawTop(IUI topUI, IUIGroup topGroup)
        {
            EditorGUILayout.LabelField("顶部", EditorStyles.boldLabel);
            DrawObjectRow("GetTopUI", topUI, TopUILabel(topUI));
            DrawObjectRow("GetTopUIGroup", topGroup, TopGroupLabel(topGroup));
        }

        void DrawUIs(IUI topUI)
        {
            _uiFoldout = EditorGUILayout.Foldout(_uiFoldout, $"UI（{_uiList.Count}）", true);
            if (!_uiFoldout)
                return;

            if (_uiList.Count == 0)
            {
                EditorGUILayout.LabelField("（空）");
                return;
            }

            EditorGUI.indentLevel++;
            for (int i = 0; i < _uiList.Count; i++)
            {
                var kv = _uiList[i];
                DrawObjectRow(UILabel(kv.Key, kv.Value, topUI), kv.Value, null);
            }
            EditorGUI.indentLevel--;
        }

        void DrawGroups(IUIGroup topGroup)
        {
            _groupFoldout = EditorGUILayout.Foldout(_groupFoldout, $"UI 分组（{_groupList.Count}）", true);
            if (!_groupFoldout)
                return;

            if (_groupList.Count == 0)
            {
                EditorGUILayout.LabelField("（空）");
                return;
            }

            EditorGUI.indentLevel++;
            for (int i = 0; i < _groupList.Count; i++)
            {
                var kv = _groupList[i];
                IUIGroup group = kv.Value;
                DrawObjectRow(GroupLabel(kv.Key, group, topGroup), group, null);
                DrawGroupItems(group);
            }
            EditorGUI.indentLevel--;
        }

        void DrawGroupItems(IUIGroup group)
        {
            if (ObjectUtility.IsNull(group) || group.itemCount <= 0)
                return;

            IUIGroupItem topItem = null;
            _itemBuffer.Clear();
            try
            {
                topItem = group.GetTopItem();
                foreach (IUIGroupItem item in group)
                    _itemBuffer.Add(item);
            }
            catch (InvalidOperationException)
            {
                EditorGUILayout.HelpBox("分组项正在变化，稍后刷新。", MessageType.None);
                return;
            }

            EditorGUI.indentLevel++;
            for (int i = 0; i < _itemBuffer.Count; i++)
            {
                IUIGroupItem item = _itemBuffer[i];
                DrawObjectRow(ItemLabel(item, i, topItem), item, null);
            }
            EditorGUI.indentLevel--;
        }

        static void DrawObjectRow(string label, object value, string fallback)
        {
            using (new EditorGUI.DisabledScope(true))
            {
                if (value is Component comp && comp)
                    EditorGUILayout.ObjectField(label, comp, typeof(Component), true);
                else if (value == null || value is Component)
                    EditorGUILayout.ObjectField(label, null, typeof(Component), true);
                else
                    EditorGUILayout.LabelField(label, string.IsNullOrEmpty(fallback) ? value.GetType().Name : fallback);
            }
        }

        static string TopUILabel(IUI ui)
        {
            if (ObjectUtility.IsNull(ui))
                return null;
            return $"{ui.name}（{ui.GetType().Name}）";
        }

        static string TopGroupLabel(IUIGroup group)
        {
            if (ObjectUtility.IsNull(group))
                return null;
            return group.groupName;
        }

        static string UILabel(Type type, IUI ui, IUI topUI)
        {
            if (ObjectUtility.IsNull(ui))
                return $"{TypeName(type)}（空）";

            string label = $"{ui.name}（{TypeName(type)}，{EnableText(ui.isEnable)}，排序 {ui.sortOrder}";
            if (ui is IUIGroupItem item)
                label += ObjectUtility.IsNull(item.group) ? "，无分组" : $"，{item.group.groupName}";
            if (ui.topmostIgnore)
                label += "，置顶忽略";
            if (IsSame(ui, topUI))
                label += "，顶部";
            return label + "）";
        }

        static string GroupLabel(string key, IUIGroup group, IUIGroup topGroup)
        {
            if (ObjectUtility.IsNull(group))
                return $"{key}（空）";

            string label = $"{group.groupName}（{EnableText(group.isEnable)}，排序 {group.sortOrder}，项 {group.itemCount}";
            if (group.topmostIgnore)
                label += "，置顶忽略";
            if (IsSame(group, topGroup))
                label += "，顶部组";
            return label + "）";
        }

        static string ItemLabel(IUIGroupItem item, int index, IUIGroupItem topItem)
        {
            if (ObjectUtility.IsNull(item))
                return $"{index}（空）";

            string name = string.IsNullOrEmpty(item.itemName) ? index.ToString() : item.itemName;
            string label = $"{name}（{EnableText(item.isEnable)}，排序 {item.sortOrder}";
            if (item.topmostIgnore)
                label += "，置顶忽略";
            if (IsSame(item, topItem))
                label += "，顶部项";
            return label + "）";
        }

        static string EnableText(bool enable) => enable ? "启用" : "未启用";

        static string TypeName(Type type) => type != null ? type.Name : "null";

        static bool IsSame(object a, object b)
        {
            if (ObjectUtility.IsNull(a) || ObjectUtility.IsNull(b))
                return false;
            if (ReferenceEquals(a, b))
                return true;
            return a is UnityEngine.Object ua && b is UnityEngine.Object ub && ua == ub;
        }

        static int CompareUI(KeyValuePair<Type, IUI> a, KeyValuePair<Type, IUI> b)
        {
            int order = SortOrder(b.Value).CompareTo(SortOrder(a.Value));
            if (order != 0)
                return order;
            return string.Compare(TypeName(a.Key), TypeName(b.Key), StringComparison.Ordinal);
        }

        static int CompareGroup(KeyValuePair<string, IUIGroup> a, KeyValuePair<string, IUIGroup> b)
        {
            int order = SortOrder(b.Value).CompareTo(SortOrder(a.Value));
            if (order != 0)
                return order;
            return string.Compare(a.Key, b.Key, StringComparison.Ordinal);
        }

        static int SortOrder(IUI ui) => ObjectUtility.IsNull(ui) ? int.MinValue : ui.sortOrder;

        static int SortOrder(IUIGroup group) => ObjectUtility.IsNull(group) ? int.MinValue : group.sortOrder;
    }
}
