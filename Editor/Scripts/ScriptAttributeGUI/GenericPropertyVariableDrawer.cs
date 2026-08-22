using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEditor;
using UnityEngine;
using Framework.Runtime;

namespace Framework.Editor
{
    [CustomPropertyDrawer(typeof(PropertyVariable<>))]
    public class GenericPropertyVariableDrawer : LineCountPropertyDrawer
    {
        private const float FieldSpacing = 2f;

        public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(pos, property, label);
            label = EditorGUI.BeginProperty(pos, label, property);

            var value = property.FindPropertyRelative("_value");
            var onChangeCallback = property.FindPropertyRelative("_onChangeCallback");

            float y = pos.y;
            float valueHeight = EditorGUI.GetPropertyHeight(value, label, true);
            PropertyField(new Rect(pos.x, y, pos.width, valueHeight), value, label);
            y += valueHeight;

            if (onChangeCallback != null && ShouldDrawOnChangeCallback())
            {
                y += FieldSpacing;
                float callbackHeight = EditorGUI.GetPropertyHeight(onChangeCallback, true);
                EditorGUI.indentLevel++;
                PropertyField(new Rect(pos.x, y, pos.width, callbackHeight), onChangeCallback, true);
                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var value = property.FindPropertyRelative("_value");
            var onChangeCallback = property.FindPropertyRelative("_onChangeCallback");

            propertyHeight = EditorGUI.GetPropertyHeight(value, label, true);
            if (onChangeCallback != null && ShouldDrawOnChangeCallback())
            {
                propertyHeight += FieldSpacing;
                propertyHeight += EditorGUI.GetPropertyHeight(onChangeCallback, true);
            }

            return totalHeight;
        }

        /// <summary>是否绘制 <c>onChangeCallback</c></summary>
        protected virtual bool ShouldDrawOnChangeCallback()
        {
            return fieldInfo == null
                || fieldInfo.GetCustomAttribute<PropertyVariableHideEventAttribute>(true) == null;
        }

        protected virtual void PropertyField(Rect pos, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(pos, property, label, true);
        }

        protected virtual void PropertyField(Rect pos, SerializedProperty property, bool includeChildren)
        {
            EditorGUI.PropertyField(pos, property, includeChildren);
        }
    }
}