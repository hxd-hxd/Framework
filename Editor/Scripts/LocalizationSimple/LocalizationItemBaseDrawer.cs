using System.Collections.Generic;
using Framework.LocalizationSimple;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    /// <summary>
    /// <see cref="LocalizationItemBase"/> 及其子类的属性绘制。
    /// 根据 <see cref="LocalizationDataGetMode"/> 切换 Provider（id / 数据提供者）与 Data（内联数据）相关字段。
    /// </summary>
    [CustomPropertyDrawer(typeof(LocalizationItemBase), true)]
    public class LocalizationItemBaseDrawer : PropertyDrawer
    {
        const float Spacing = 2f;
        const string DataModeField = "_dataMode";
        const string DataIdField = "_dataId";
        const string DataProviderField = "_dataProvider";
        const string DatasField = "_datas";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var mode = GetMode(property);
            label = EditorGUI.BeginProperty(position, label, property);

            float line = EditorGUIUtility.singleLineHeight;
            var foldRect = new Rect(position.x, position.y, position.width, line);
            property.isExpanded = EditorGUI.Foldout(foldRect, property.isExpanded, label, true);

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                float y = foldRect.yMax + Spacing;
                DrawChildren(position, property, mode, ref y);
                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;
            if (!property.isExpanded)
                return height;

            height += Spacing;
            var mode = GetMode(property);
            foreach (var child in EnumerateDrawnChildren(property, mode))
                height += EditorGUI.GetPropertyHeight(child, true) + Spacing;

            return height;
        }

        static LocalizationDataGetMode GetMode(SerializedProperty property)
        {
            var modeProp = property.FindPropertyRelative(DataModeField);
            return modeProp != null
                ? (LocalizationDataGetMode)modeProp.enumValueIndex
                : LocalizationDataGetMode.Data;
        }

        static bool ShouldDrawField(string fieldName, LocalizationDataGetMode mode)
        {
            if (fieldName == DataIdField || fieldName == DataProviderField)
                return mode == LocalizationDataGetMode.Provider;
            if (fieldName == DatasField)
                return mode == LocalizationDataGetMode.Data;
            return true;
        }

        static IEnumerable<SerializedProperty> EnumerateDrawnChildren(SerializedProperty property, LocalizationDataGetMode mode)
        {
            var end = property.GetEndProperty();
            var iterator = property.Copy();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren) && !SerializedProperty.EqualContents(iterator, end))
            {
                enterChildren = false;
                if (!ShouldDrawField(iterator.name, mode))
                    continue;
                yield return iterator.Copy();
            }
        }

        static void DrawChildren(Rect position, SerializedProperty property, LocalizationDataGetMode mode, ref float y)
        {
            foreach (var child in EnumerateDrawnChildren(property, mode))
            {
                float h = EditorGUI.GetPropertyHeight(child, true);
                DrawChild(new Rect(position.x, y, position.width, h), child);
                y += h + Spacing;
            }
        }

        static void DrawChild(Rect position, SerializedProperty property)
        {
            var indented = EditorGUI.IndentedRect(position);
            indented.height = position.height;

            int oldIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            EditorGUI.PropertyField(indented, property, true);
            EditorGUI.indentLevel = oldIndent;
        }
    }
}
