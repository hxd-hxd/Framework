using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    using SelectItem = SelectItemBase;

    /// <summary>
    /// <see cref="Selecter"/> 属性绘制：保持默认序列化字段绘制，
    /// 并额外提供类属性的编辑/监视。
    /// </summary>
    [CustomPropertyDrawer(typeof(Selecter))]
    public class SelecterDrawer : PropertyDrawer
    {
        const float Spacing = 2f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);

            var line = EditorGUIUtility.singleLineHeight;
            var foldRect = new Rect(position.x, position.y, position.width, line);
            property.isExpanded = EditorGUI.Foldout(foldRect, property.isExpanded, label, true);

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                float y = foldRect.yMax + Spacing;

                // 1. 默认序列化字段绘制
                y = DrawDefaultChildren(position, property, y);

                // 2. 额外类属性
                y += Spacing;
                var headerRect = new Rect(position.x, y, position.width, line);
                EditorGUI.LabelField(headerRect, "属性", EditorStyles.boldLabel);
                y += line + Spacing;

                var selecter = GetTargetObject(property) as Selecter;
                if (selecter != null)
                {
                    y = DrawSelecterProperties(position, property, selecter, y);
                }
                else
                {
                    var tipRect = new Rect(position.x, y, position.width, line);
                    EditorGUI.LabelField(tipRect, "无法获取 Selecter 实例");
                }

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float line = EditorGUIUtility.singleLineHeight;
            float height = line;

            if (!property.isExpanded)
                return height;

            height += Spacing;
            height += GetDefaultChildrenHeight(property);
            height += Spacing;
            height += line + Spacing; // "属性" 标题
            height += GetSelecterPropertiesHeight();

            return height;
        }

        static float DrawDefaultChildren(Rect position, SerializedProperty property, float y)
        {
            var end = property.GetEndProperty();
            var iterator = property.Copy();
            bool enterChildren = true;

            while (iterator.NextVisible(enterChildren) && !SerializedProperty.EqualContents(iterator, end))
            {
                enterChildren = false;
                float h = EditorGUI.GetPropertyHeight(iterator, true);
                var rect = new Rect(position.x, y, position.width, h);
                EditorGUI.PropertyField(rect, iterator, true);
                y += h + Spacing;
            }

            return y;
        }

        static float GetDefaultChildrenHeight(SerializedProperty property)
        {
            float height = 0f;
            var end = property.GetEndProperty();
            var iterator = property.Copy();
            bool enterChildren = true;

            while (iterator.NextVisible(enterChildren) && !SerializedProperty.EqualContents(iterator, end))
            {
                enterChildren = false;
                height += EditorGUI.GetPropertyHeight(iterator, true) + Spacing;
            }

            return height;
        }

        static float GetSelecterPropertiesHeight()
        {
            // curItem / nextItem / prevItem / curItemIsLast / curItemIsFirst
            // allowMultipleSelections / allowCancelSelections / allowLoopSelections
            const int count = 8;
            float line = EditorGUIUtility.singleLineHeight;
            return count * (line + Spacing);
        }

        float DrawSelecterProperties(Rect position, SerializedProperty property, Selecter selecter, float y)
        {
            float line = EditorGUIUtility.singleLineHeight;
            var target = property.serializedObject.targetObject;

            // curItem（可写）
            {
                var rect = new Rect(position.x, y, position.width, line);
                EditorGUI.BeginChangeCheck();
                var newItem = (SelectItem)EditorGUI.ObjectField(rect, "curItem", selecter.curItem, typeof(SelectItem), true);
                if (EditorGUI.EndChangeCheck())
                {
                    Record(property, selecter, "Set Selecter.curItem");
                    selecter.curItem = newItem;
                    AfterChange(property, target);
                }
                y += line + Spacing;
            }

            // nextItem（只读）
            {
                var rect = new Rect(position.x, y, position.width, line);
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUI.ObjectField(rect, "nextItem", selecter.nextItem, typeof(SelectItem), true);
                }
                y += line + Spacing;
            }

            // prevItem（只读）
            {
                var rect = new Rect(position.x, y, position.width, line);
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUI.ObjectField(rect, "prevItem", selecter.prevItem, typeof(SelectItem), true);
                }
                y += line + Spacing;
            }

            // curItemIsLast（只读）
            {
                var rect = new Rect(position.x, y, position.width, line);
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUI.Toggle(rect, "curItemIsLast", selecter.curItemIsLast);
                }
                y += line + Spacing;
            }

            // curItemIsFirst（只读）
            {
                var rect = new Rect(position.x, y, position.width, line);
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUI.Toggle(rect, "curItemIsFirst", selecter.curItemIsFirst);
                }
                y += line + Spacing;
            }

            // allowMultipleSelections（走属性 setter）
            {
                var rect = new Rect(position.x, y, position.width, line);
                EditorGUI.BeginChangeCheck();
                bool value = EditorGUI.Toggle(rect, "allowMultipleSelections", selecter.allowMultipleSelections);
                if (EditorGUI.EndChangeCheck())
                {
                    Record(property, selecter, "Set Selecter.allowMultipleSelections");
                    selecter.allowMultipleSelections = value;
                    AfterChange(property, target);
                }
                y += line + Spacing;
            }

            // allowCancelSelections（走属性 setter）
            {
                var rect = new Rect(position.x, y, position.width, line);
                EditorGUI.BeginChangeCheck();
                bool value = EditorGUI.Toggle(rect, "allowCancelSelections", selecter.allowCancelSelections);
                if (EditorGUI.EndChangeCheck())
                {
                    Record(property, selecter, "Set Selecter.allowCancelSelections");
                    selecter.allowCancelSelections = value;
                    AfterChange(property, target);
                }
                y += line + Spacing;
            }

            // allowLoopSelections（走属性 setter）
            {
                var rect = new Rect(position.x, y, position.width, line);
                EditorGUI.BeginChangeCheck();
                bool value = EditorGUI.Toggle(rect, "allowLoopSelections", selecter.allowLoopSelections);
                if (EditorGUI.EndChangeCheck())
                {
                    Record(property, selecter, "Set Selecter.allowLoopSelections");
                    selecter.allowLoopSelections = value;
                    AfterChange(property, target);
                }
                y += line + Spacing;
            }

            return y;
        }

        static void Record(SerializedProperty property, Selecter selecter, string undoName)
        {
            Undo.RecordObject(property.serializedObject.targetObject, undoName);

            if (selecter.items == null)
                return;

            for (int i = 0; i < selecter.items.Count; i++)
            {
                var item = selecter.items[i];
                if (item)
                    Undo.RecordObject(item, undoName);
            }
        }

        static void AfterChange(SerializedProperty property, UnityEngine.Object target)
        {
            EditorUtility.SetDirty(target);
            property.serializedObject.Update();
        }

        /// <summary>从 SerializedProperty 路径解析出实际对象实例。</summary>
        static object GetTargetObject(SerializedProperty property)
        {
            if (property == null)
                return null;

            object obj = property.serializedObject.targetObject;
            if (obj == null)
                return null;

            string path = property.propertyPath.Replace(".Array.data[", "[");
            string[] elements = path.Split('.');

            for (int i = 0; i < elements.Length; i++)
            {
                string element = elements[i];
                if (element.Contains("["))
                {
                    string name = element.Substring(0, element.IndexOf("[", StringComparison.Ordinal));
                    int index = Convert.ToInt32(
                        element.Substring(element.IndexOf("[", StringComparison.Ordinal))
                            .Replace("[", string.Empty)
                            .Replace("]", string.Empty));
                    obj = GetValue(obj, name, index);
                }
                else
                {
                    obj = GetValue(obj, element);
                }

                if (obj == null)
                    return null;
            }

            return obj;
        }

        static object GetValue(object source, string name)
        {
            if (source == null)
                return null;

            var type = source.GetType();
            while (type != null)
            {
                var field = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (field != null)
                    return field.GetValue(source);

                var prop = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (prop != null)
                    return prop.GetValue(source, null);

                type = type.BaseType;
            }

            return null;
        }

        static object GetValue(object source, string name, int index)
        {
            var enumerable = GetValue(source, name) as IEnumerable;
            if (enumerable == null)
                return null;

            var enumerator = enumerable.GetEnumerator();
            for (int i = 0; i <= index; i++)
            {
                if (!enumerator.MoveNext())
                    return null;
            }

            return enumerator.Current;
        }
    }
}
