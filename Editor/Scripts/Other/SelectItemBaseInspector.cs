using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    /// <summary>
    /// <see cref="SelectItemBase"/> 检视面板：保持默认绘制，
    /// 并额外提供类属性 <see cref="SelectItemBase.isSelect"/>、<see cref="SelectItemBase.canSelect"/> 的编辑。
    /// </summary>
    [CustomEditor(typeof(SelectItemBase), true)]
    public class SelectItemBaseInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // 1. 默认序列化字段绘制
            DrawDefaultInspector();

            // 2. 额外类属性
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("属性", EditorStyles.boldLabel);

            var item = (SelectItemBase)target;
            if (item == null)
            {
                EditorGUILayout.HelpBox("无法获取 SelectItemBase 实例", MessageType.Warning);
                return;
            }

            EditorGUI.BeginChangeCheck();
            bool isSelect = EditorGUILayout.Toggle("isSelect", item.isSelect);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(item, "Set SelectItemBase.isSelect");
                item.isSelect = isSelect;
                EditorUtility.SetDirty(item);
            }

            EditorGUI.BeginChangeCheck();
            bool canSelect = EditorGUILayout.Toggle("canSelect", item.canSelect);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(item, "Set SelectItemBase.canSelect");
                item.canSelect = canSelect;
                EditorUtility.SetDirty(item);
            }
        }
    }
}
