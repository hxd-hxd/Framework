using Framework.UI;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    /// <summary>
    /// <see cref="UIGroup"/> 检视面板：保持默认绘制，并显示 <c>_items</c>。
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
    }
}
