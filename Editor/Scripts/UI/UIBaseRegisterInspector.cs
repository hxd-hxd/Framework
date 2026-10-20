using Framework.UI;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    /// <summary>
    /// <see cref="UIBaseRegister"/> 检视面板：保持默认绘制。
    /// 编辑模式下，<c>_ui</c> 为空时对齐到同物体上的 <see cref="UIBase"/>。
    /// 并显示 <see cref="UIBaseUnregister"/> 与 <see cref="UIManager"/> 中的注册状态。
    /// </summary>
    [CustomEditor(typeof(UIBaseRegister))]
    public class UIBaseRegisterInspector : UnityEditor.Editor
    {
        UIBaseRegister my => (UIBaseRegister)target;

        public override bool RequiresConstantRepaint() => Application.isPlaying;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (!Application.isPlaying)
                SyncUI(my, serializedObject);

            DrawState(my);
        }

        /// <summary>
        /// 与 <see cref="UIBaseRegister"/> 的 <c>Awake</c> 相同：未指定时使用同物体上的 <see cref="UIBase"/>。
        /// </summary>
        static void SyncUI(UIBaseRegister register, SerializedObject so)
        {
            SerializedProperty prop = so.FindProperty("_ui");
            if (prop == null || prop.objectReferenceValue)
                return;

            register.TryGetComponent(out UIBase ui);
            if (!ui)
                return;

            prop.objectReferenceValue = ui;
            so.ApplyModifiedProperties();
        }

        static void DrawState(UIBaseRegister register)
        {
            EditorGUILayout.Space();
            UIBase ui = register.ui;
            if (!ui)
            {
                EditorGUILayout.HelpBox("未指定 UIBase，同物体上也没有 UIBase。", MessageType.Warning);
                return;
            }

            DrawUnregister(ui);
            DrawRegistration(ui);
        }

        static void DrawUnregister(UIBase ui)
        {
            ui.TryGetComponent(out UIBaseUnregister unregister);
            using (new EditorGUI.DisabledScope(true))
                EditorGUILayout.ObjectField("UIBaseUnregister", unregister, typeof(UIBaseUnregister), true);

            if (unregister)
                return;

            EditorGUILayout.HelpBox("UI 上还没有 UIBaseUnregister。进入播放后 Awake 会补上。", MessageType.Info);
            if (GUILayout.Button("添加 UIBaseUnregister"))
                Undo.AddComponent<UIBaseUnregister>(ui.gameObject);
        }

        static void DrawRegistration(UIBase ui)
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("进入播放模式后显示 UIManager 注册状态。", MessageType.Info);
                return;
            }

            UIManager manager = UIManager.Instance;
            bool same = manager.IsRegister(ui);
            IUI registered = null;
            if (manager.uis.TryGetValue(ui.GetType(), out IUI current) && !ObjectUtility.IsNull(current))
                registered = current;

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.Toggle("已注册", same);
                Component registeredComp = registered as Component;
                EditorGUILayout.ObjectField("UIManager 中的实例", registeredComp, typeof(Component), true);
            }

            if (same)
                return;

            if (registered != null)
                EditorGUILayout.HelpBox("UIManager 中该类型已注册了另一个实例。", MessageType.Warning);
            else
                EditorGUILayout.HelpBox("此 UI 尚未注册到 UIManager。", MessageType.Warning);
        }
    }
}
