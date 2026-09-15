// -------------------------
// 创建日期：2026/9/15 17:10:00
// -------------------------

using System;
using Framework;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    /// <summary>
    /// <see cref="TypeNameSelectAttribute"/> 属性绘制：字符串字段旁提供类型选择按钮。
    /// </summary>
    [CustomPropertyDrawer(typeof(TypeNameSelectAttribute))]
    public class TypeNameSelectAttributeDrawer : PropertyDrawer
    {
        const float ButtonWidth = 48f;
        const float Spacing = 2f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (PropertyVariableAttributeNestingDrawer.ShouldNest(property, fieldInfo))
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            label = EditorGUI.BeginProperty(position, label, property);

            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.HelpBox(position,
                    $"TypeNameSelectAttribute 不支持的类型 \"{property.type}\"，仅支持 string",
                    MessageType.Error);
                EditorGUI.EndProperty();
                return;
            }

            var attr = (TypeNameSelectAttribute)attribute;
            Rect fieldRect = new Rect(position.x, position.y, position.width - ButtonWidth - Spacing, EditorGUIUtility.singleLineHeight);
            Rect buttonRect = new Rect(position.xMax - ButtonWidth, position.y, ButtonWidth, EditorGUIUtility.singleLineHeight);

            EditorGUI.PropertyField(fieldRect, property, label);
            if (GUI.Button(buttonRect, "选择"))
                TypeSelectWindow.Open(property, attr.arrayOnly);

            if (TryGetValidation(property.stringValue, attr.arrayOnly, out string message, out MessageType messageType))
            {
                Rect helpRect = new Rect(
                    position.x,
                    fieldRect.yMax + Spacing,
                    position.width,
                    position.yMax - fieldRect.yMax - Spacing);
                EditorGUI.HelpBox(helpRect, message, messageType);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (PropertyVariableAttributeNestingDrawer.ShouldNest(property, fieldInfo))
                return EditorGUI.GetPropertyHeight(property, label, true);

            float height = EditorGUIUtility.singleLineHeight;
            if (property.propertyType != SerializedPropertyType.String)
                return height * 2f + Spacing;

            var attr = (TypeNameSelectAttribute)attribute;
            if (TryGetValidation(property.stringValue, attr.arrayOnly, out _, out _))
                height += EditorGUIUtility.singleLineHeight * 2f + Spacing;

            return height;
        }

        static bool TryGetValidation(string typeName, bool arrayOnly, out string message, out MessageType messageType)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                message = "类型名为空";
                messageType = MessageType.Warning;
                return true;
            }

            Type type = TypeSelectCache.Find(typeName);
            if (type == null)
            {
                message = "无法解析类型：" + typeName;
                messageType = MessageType.Warning;
                return true;
            }

            if (arrayOnly && !type.IsArray)
            {
                message = "需要数组类型全名，当前为：" + type.FullName;
                messageType = MessageType.Error;
                return true;
            }

            if (!arrayOnly && type.IsArray)
            {
                message = "不能使用数组类型";
                messageType = MessageType.Error;
                return true;
            }

            message = null;
            messageType = MessageType.None;
            return false;
        }
    }
}
