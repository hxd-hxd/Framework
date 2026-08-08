using Framework.Localization;
using Framework.LocalizationSimple;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    /// <summary>
    /// <see cref="LocalizationSetRegister"/> 检视面板：保持默认绘制，
    /// 并额外显示 <see cref="LocalizationSetManager"/> 中已注册的数量与列表。
    /// </summary>
    [CustomEditor(typeof(LocalizationSetRegister))]
    public class LocalizationSetRegisterInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("LocalizationSetManager", EditorStyles.boldLabel);

            var manager = LocalizationSetManager.Instance;
            EditorGUILayout.LabelField("Count", manager.count.ToString());

            var sets = manager.sets;
            if (sets == null || sets.Count == 0)
            {
                EditorGUILayout.HelpBox("当前无已注册的 LocalizationSet", MessageType.Info);
                return;
            }

            EditorGUI.BeginDisabledGroup(true);
            for (int i = 0; i < sets.Count; i++)
            {
                var set = sets[i];
                string typeName = set != null ? set.GetType().FullName : "null";
                string label = $"[{i}] {typeName}";
                if (set is Object unityObj)
                {
                    EditorGUILayout.ObjectField(label, unityObj, typeof(Object), true);
                }
                else
                {
                    EditorGUILayout.LabelField(label);
                }
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}
