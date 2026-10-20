using Framework.UI;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    /// <summary>
    /// <see cref="UIGroup"/> 检视面板：保持默认绘制，提供 <see cref="UIGroup.CreateSubGroup"/> 按钮，
    /// 并显示 <c>_items</c>、<see cref="UIGroup.sortOrder"/>、<see cref="IUIGroupItem.topmostIgnore"/>、
    /// <see cref="UIGroup.HasEnableItem"/> 与 <see cref="UIGroup.GetTopItem"/>。
    /// 元素为 <see cref="Component"/> 时转换成组件引用显示。
    /// </summary>
    [CustomEditor(typeof(UIGroup), true)]
    public class UIGroupInspector : UnityEditor.Editor
    {
        string _subGroupName = "子分组";

        public override bool RequiresConstantRepaint() => Application.isPlaying;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            UIGroup group = (UIGroup)target;
            if (!group)
                return;

            // 撤销创建后，子分组对象已销毁，但父级列表里还留着引用
            group.RemoveDestroyedItems();
            if (!Application.isPlaying)
                SyncTopmostIgnoreTree(group, serializedObject);
            DrawCreateSubGroup(group);
            DrawItems(group);
        }

        /// <summary>绘制子分组名称和 <see cref="UIGroup.CreateSubGroup"/> 按钮。</summary>
        void DrawCreateSubGroup(UIGroup group)
        {
            EditorGUILayout.Space();
            _subGroupName = EditorGUILayout.TextField("子分组名称", _subGroupName);

            bool nameEmpty = string.IsNullOrEmpty(_subGroupName);
            bool duplicated = !nameEmpty && HasSameNameSubGroup(group, _subGroupName);
            if (duplicated)
                EditorGUILayout.HelpBox("创建失败，已有同名子组。", MessageType.Warning);

            using (new EditorGUI.DisabledScope(nameEmpty || duplicated))
            {
                if (GUILayout.Button("CreateSubGroup"))
                    CreateSubGroup(group, _subGroupName);
            }
        }

        static void CreateSubGroup(UIGroup group, string name)
        {
            Undo.IncrementCurrentGroup();
            Undo.SetCurrentGroupName("创建子分组");
            int undoGroup = Undo.GetCurrentGroup();

            IUIGroup created = group.CreateSubGroup(name);
            if (created is Component comp && comp)
            {
                Undo.RegisterCreatedObjectUndo(comp.gameObject, "创建子分组");
                EditorGUIUtility.PingObject(comp.gameObject);
            }

            Undo.CollapseUndoOperations(undoGroup);
        }

        static bool HasSameNameSubGroup(UIGroup group, string name)
        {
            var items = group.items;
            if (items == null)
                return false;

            for (int i = 0; i < items.Count; i++)
            {
                if (ObjectUtility.IsNull(items[i]))
                    continue;
                if (items[i] is IUIGroup sub && sub.itemName == name)
                    return true;
            }
            return false;
        }

        static void DrawItems(UIGroup group)
        {
            EditorGUILayout.Space();
            IUIGroupItem topItem = group.GetTopItem();
            if (!group.isRootGroup)
                EditorGUILayout.HelpBox("子分组请调整层级顺序", MessageType.Info);

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.IntField("sortOrder", group.sortOrder);
                EditorGUILayout.LabelField("sortOrder 来源", group.isRootGroup ? "根组 _sortOrder" : "子组 SiblingIndex");
                EditorGUILayout.Toggle("topmostIgnore", group.topmostIgnore);
                EditorGUILayout.Toggle("HasEnableItem", group.HasEnableItem());
                DrawTopItem(topItem);
            }

            var items = group.items;
            int count = items != null ? items.Count : 0;
            EditorGUILayout.LabelField($"Items（{count}）", EditorStyles.boldLabel);

            if (count == 0)
            {
                EditorGUILayout.LabelField("（空）");
                return;
            }

            EditorGUI.indentLevel++;
            for (int i = 0; i < count; i++)
            {
                IUIGroupItem item = items[i];
                string label = item != null && !string.IsNullOrEmpty(item.itemName)
                    ? item.itemName
                    : i.ToString();
                if (item != null)
                    label = $"{label}（{ItemStateLabel(item, group, topItem)}）";

                if (item is Component comp)
                {
                    using (new EditorGUI.DisabledScope(true))
                    {
                        EditorGUILayout.ObjectField(label, comp, typeof(Component), true);
                    }
                }
                else if (item == null)
                {
                    EditorGUILayout.LabelField(label, "null");
                }
                else
                {
                    EditorGUILayout.LabelField(label, item.GetType().Name);
                }
            }
            EditorGUI.indentLevel--;
        }

        static void DrawTopItem(IUIGroupItem topItem)
        {
            if (topItem is Component comp)
                EditorGUILayout.ObjectField("GetTopItem", comp, typeof(Component), true);
            else if (topItem == null)
                EditorGUILayout.ObjectField("GetTopItem", null, typeof(Component), true);
            else
                EditorGUILayout.LabelField("GetTopItem", $"{topItem.itemName}（{topItem.GetType().Name}）");
        }

        static string ItemStateLabel(IUIGroupItem item, IUIGroup owner, IUIGroupItem topItem)
        {
            string state = $"{EnableLabel(item)}，排序 {item.sortOrder}";
            if (item.topmostIgnore)
                state += "，置顶忽略";
            if (ReferenceEquals(item, topItem))
                return state + "，顶部项";
            if (topItem != null && IsAncestorOfTopItem(item, owner, topItem))
                return state + "，顶部项所在子组";
            return state;
        }

        /// <summary>与 <see cref="UIGroup.HasEnableItem"/> 相同的判定：未启用的跳过，子组继续向下查。</summary>
        static string EnableLabel(IUIGroupItem item)
        {
            if (!item.isEnable)
                return "未启用";

            if (item is IUIGroup iGroup)
                return iGroup.HasEnableItem() ? "子组有启用项" : "子组无启用项";

            return "启用";
        }

        static bool IsAncestorOfTopItem(IUIGroupItem item, IUIGroup owner, IUIGroupItem topItem)
        {
            IUIGroup group = topItem.group;
            while (group != null && !ReferenceEquals(group, owner))
            {
                if (ReferenceEquals(group, item))
                    return true;
                group = group is IUIGroupItem groupItem ? groupItem.group : null;
            }
            return false;
        }

        /// <summary>
        /// 编辑模式下把 <c>_topmostIgnoreComp</c> 对齐到同物体上的 <see cref="TopmostIgnoreComp"/>。
        /// 这样 <see cref="IUIGroupItem.topmostIgnore"/> 与 <see cref="UIGroup.GetTopItem"/> 在进播放前就和组件是否存在一致。
        /// </summary>
        static void SyncTopmostIgnoreTree(UIGroup group, SerializedObject groupSerialized)
        {
            SyncTopmostIgnore(group, groupSerialized);
            SyncChildItems(group);
        }

        static void SyncChildItems(UIGroupBase group)
        {
            var items = group.items;
            if (items == null)
                return;

            for (int i = 0; i < items.Count; i++)
            {
                Component child = items[i] as Component;
                if (!child || child == group)
                    continue;

                SerializedObject so = new SerializedObject(child);
                try
                {
                    SyncTopmostIgnore(child, so);
                }
                finally
                {
                    so.Dispose();
                }

                UIGroupBase childGroup = child as UIGroupBase;
                if (childGroup != null)
                    SyncChildItems(childGroup);
            }
        }

        internal static void SyncTopmostIgnore(Component comp, SerializedObject so)
        {
            SerializedProperty prop = so.FindProperty("_topmostIgnoreComp");
            if (prop == null)
                return;

            comp.TryGetComponent(out TopmostIgnoreComp ignore);
            if (prop.objectReferenceValue == ignore)
                return;

            prop.objectReferenceValue = ignore;
            so.ApplyModifiedProperties();
        }
    }
}
