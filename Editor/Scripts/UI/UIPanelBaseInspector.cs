using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    /// <summary>
    /// <see cref="UIPanelBase"/> 检视面板：保持 <see cref="UIBaseInspector"/> 的绘制，并提供置顶按钮。
    /// 置顶走 <see cref="UIPanelExtend.PanelTopmost"/>，与运行时自动置顶同一条路径。
    /// </summary>
    [CustomEditor(typeof(UIPanelBase), true)]
    public class UIPanelBaseInspector : UIBaseInspector
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            UIPanelBase panel = (UIPanelBase)target;
            EditorGUILayout.Space();
            if (!panel.isEnable)
                EditorGUILayout.HelpBox("界面未启用，无法置顶。", MessageType.Info);

            using (new EditorGUI.DisabledScope(!panel.isEnable))
            {
                if (GUILayout.Button("置顶"))
                    Topmost(panel);
            }
        }

        static void Topmost(UIPanelBase panel)
        {
            Undo.RecordObject(panel, "置顶界面");
            if (panel.canvas)
                Undo.RecordObject(panel.canvas, "置顶界面");
            if (panel.transform.parent)
                RegisterChildrenOrderUndo(panel.transform.parent, "置顶界面");

            panel.PanelTopmost();
        }
    }
}
