using Framework.Save;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    /// <summary>
    /// <see cref="PlayerPrefsSaveDataProviderComp"/> 检视面板：
    /// 保持默认绘制，并只读显示 <see cref="PlayerPrefsSaveDataProviderComp.nameSpace"/>。
    /// </summary>
    [CustomEditor(typeof(PlayerPrefsSaveDataProviderComp))]
    [CanEditMultipleObjects]
    public class PlayerPrefsSaveDataProviderCompInspector : UnityEditor.Editor
    {
        static readonly GUIContent NameSpaceLabel = new GUIContent(
            "nameSpace",
            "命名空间，会附加到键的前面。由根名称和版本号拼接而成，示例：Save/1.0 或 Save/。");

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

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
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField(NameSpaceLabel, nameSpace ?? string.Empty);
            EditorGUI.EndDisabledGroup();
            EditorGUI.showMixedValue = false;
        }
    }
}
