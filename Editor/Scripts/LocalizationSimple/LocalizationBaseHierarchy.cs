// -------------------------
// 创建日期：2026/8/8
// -------------------------

using Framework.LocalizationSimple;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    /// <summary>
    /// Hierarchy 中为挂载了 <see cref="LocalizationCompBase"/> 子类的对象显示多语言图标。
    /// </summary>
    [InitializeOnLoad]
    static class LocalizationBaseHierarchy
    {
        const string IconName = "多语言图标1_纯图标.png";
        const float IconSize = 16f;

        static Texture2D _icon;

        static LocalizationBaseHierarchy()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;
        }

        static Texture2D Icon
        {
            get
            {
                if (_icon == null)
                    _icon = EditorResources.Load<Texture2D>(IconName);
                return _icon;
            }
        }

        static void OnHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            var icon = Icon;
            if (icon == null)
                return;

            var go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (go == null)
                return;

            if (go.GetComponent<LocalizationCompBase>() == null)
                return;

            var iconRect = new Rect(
                selectionRect.xMax - IconSize,
                selectionRect.y + (selectionRect.height - IconSize) * 0.5f,
                IconSize,
                IconSize);

            GUI.DrawTexture(iconRect, icon);
        }
    }
}
