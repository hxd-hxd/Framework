using System.Collections.Generic;
using System.Reflection;
using Framework.LocalizationSimple;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    /// <summary>
    /// <see cref="LocalizationDataBase"/> 及其子类的属性绘制。
    /// 根据字段上的 <see cref="LocalizationDataCfgAttribute"/>（含 List/数组字段，Unity 会把 fieldInfo 指到集合字段）
    /// 决定 id / 语言相关字段是否显示。
    /// <para>
    /// 不要为 <see cref="LocalizationDataCfgAttribute"/> 再注册 PropertyDrawer：
    /// Unity 会把集合上的特性抽屉叠到元素上，与本抽屉叠加会导致标题重复。
    /// </para>
    /// </summary>
    [CustomPropertyDrawer(typeof(LocalizationDataBase), true)]
    public class LocalizationDataBaseDrawer : PropertyDrawer
    {
        const float Spacing = 2f;
        const string IdField = "_id";
        const string LanguageField = "_language";
        const string LangProviderField = "_langProvider";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var mode = ResolveMode(fieldInfo);
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
            var mode = ResolveMode(fieldInfo);
            foreach (var child in EnumerateDrawnChildren(property, mode))
                height += EditorGUI.GetPropertyHeight(child, true) + Spacing;

            return height;
        }

        static LocalizationDataCfgMode ResolveMode(FieldInfo fieldInfo)
        {
            var attr = fieldInfo?.GetCustomAttribute<LocalizationDataCfgAttribute>(true);
            return attr != null ? attr.mode : LocalizationDataCfgMode.All;
        }

        static bool ShouldDrawField(string fieldName, LocalizationDataCfgMode mode)
        {
            if (fieldName == IdField)
                return mode == LocalizationDataCfgMode.OnlyId || mode == LocalizationDataCfgMode.All;
            if (fieldName == LanguageField || fieldName == LangProviderField)
                return mode == LocalizationDataCfgMode.OnlyLang || mode == LocalizationDataCfgMode.All;
            return true;
        }

        static IEnumerable<SerializedProperty> EnumerateDrawnChildren(SerializedProperty property, LocalizationDataCfgMode mode)
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

        static void DrawChildren(Rect position, SerializedProperty property, LocalizationDataCfgMode mode, ref float y)
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
            // TextArea 内置抽屉在嵌套自定义 PropertyDrawer 中会把标题画两次
            // （HandlePrefixLabel 一次 + IndentedRect 错位一次）。多行字符串改为自绘。
            float line = EditorGUIUtility.singleLineHeight;
            bool isMultiLineString = property.propertyType == SerializedPropertyType.String
                && position.height > line + 1f;

            var indented = EditorGUI.IndentedRect(position);
            indented.height = position.height;

            int oldIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            if (isMultiLineString)
            {
                var labelRect = new Rect(indented.x, indented.y, indented.width, line);
                EditorGUI.LabelField(labelRect, property.displayName);

                var textRect = new Rect(indented.x, indented.y + line, indented.width, indented.height - line);
                EditorGUI.BeginChangeCheck();
                string newValue = EditorGUI.TextArea(textRect, property.stringValue ?? string.Empty);
                if (EditorGUI.EndChangeCheck())
                    property.stringValue = newValue;
            }
            else
            {
                EditorGUI.PropertyField(indented, property, true);
            }

            EditorGUI.indentLevel = oldIndent;
        }
    }
}
