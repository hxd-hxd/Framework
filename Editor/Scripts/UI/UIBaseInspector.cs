using Framework.UI;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    /// <summary>
    /// <see cref="UIBase"/> 检视面板：保持默认绘制，显示 <see cref="IUIGroupItem.topmostIgnore"/>，
    /// 并提供 <see cref="UIBase.Enable"/> 按钮。
    /// 编辑模式下把隐藏的 <c>_topmostIgnoreComp</c> 对齐到同物体上的 <see cref="TopmostIgnoreComp"/>。
    /// </summary>
    [CustomEditor(typeof(UIBase), true)]
    public class UIBaseInspector : UnityEditor.Editor
    {
        bool _isEnable = true;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            UIBase ui = (UIBase)target;
            if (!ui)
                return;

            if (!Application.isPlaying)
                UIGroupInspector.SyncTopmostIgnore(ui, serializedObject);

            EditorGUILayout.Space();
            using (new EditorGUI.DisabledScope(true))
                EditorGUILayout.Toggle("topmostIgnore", ui.topmostIgnore);

            DrawEnable(ui);
        }

        /// <summary>绘制 <see cref="UIBase.Enable"/> 的参数和按钮。</summary>
        void DrawEnable(UIBase ui)
        {
            EditorGUILayout.Space();
            _isEnable = EditorGUILayout.Toggle("isEnable", _isEnable);
            if (GUILayout.Button("Enable"))
                Enable(ui, _isEnable);
        }

        static void Enable(UIBase ui, bool isEnable)
        {
            Undo.IncrementCurrentGroup();
            Undo.SetCurrentGroupName("Enable");
            int undoGroup = Undo.GetCurrentGroup();

            Undo.RecordObject(ui.gameObject, "Enable");
            Undo.RecordObject(ui, "Enable");
            if (ui.transform.parent)
                Undo.RegisterChildrenOrderUndo(ui.transform.parent, "Enable");

            if (ui is UIPanelBase panel && panel.canvas)
                Undo.RecordObject(panel.canvas, "Enable");

            ui.Enable(isEnable);

            EditorUtility.SetDirty(ui);
            EditorUtility.SetDirty(ui.gameObject);
            if (ui is UIPanelBase dirtyPanel && dirtyPanel.canvas)
                EditorUtility.SetDirty(dirtyPanel.canvas);

            Undo.CollapseUndoOperations(undoGroup);
        }
    }
}
