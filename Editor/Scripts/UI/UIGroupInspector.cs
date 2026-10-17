using Framework.UI;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    /// <summary>
    /// <see cref="UIGroup"/> 检视面板：保持默认绘制，并显示 <c>_items</c>、
    /// <see cref="UIGroup.sortOrder"/>、<see cref="UIGroup.HasEnableItem"/> 与 <see cref="UIGroup.GetTopItem"/>。
    /// 元素为 <see cref="Component"/> 时转换成组件引用显示。
    /// </summary>
    [CustomEditor(typeof(UIGroup), true)]
    public class UIGroupInspector : UnityEditor.Editor
    {
        public override bool RequiresConstantRepaint() => Application.isPlaying;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            DrawItems((UIGroup)target);
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
    }
}
