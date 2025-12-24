using System.Collections;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;

using UnityEditor;
using UnityEngine;
using Framework.Runtime;

namespace Framework.Editor
{

    [CustomPropertyDrawer(typeof(PropertyVariable<>))]
    public class GenericPropertyVariableDrawer : LineCountPropertyDrawer
    {
        public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(pos, property, label);
            //pos.height = singleLineHeight;
            label = EditorGUI.BeginProperty(pos, label, property);

            var value = property.FindPropertyRelative("_value");
            //label = EditorGUI.BeginProperty(pos, label, value);

            if (IsUniline(value.propertyType))
            {
                PropertyField(pos, value, label);
            }
            else
            {
                PropertyField(pos, property, label);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // 调用父类更新
            base.GetPropertyHeight(property, label);

            return totalHeight;
        }

        protected virtual void PropertyField(Rect pos, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(pos, property, label, true);
        }

        /// <summary>是否在一行中显示</summary>
        protected bool IsUniline(SerializedPropertyType type)
        {
            //return false;
            return IsUnilineSerializedPropertyType(type)
                ;
        }

        /// <summary>指定的 <see cref="SerializedPropertyType"/> 是否在一行中显示</summary>
        protected bool IsUnilineSerializedPropertyType(SerializedPropertyType type)
        {
            //return false;
            return type == SerializedPropertyType.Float
                || type == SerializedPropertyType.Integer
                || type == SerializedPropertyType.Boolean
                || type == SerializedPropertyType.Enum
                || type == SerializedPropertyType.Color
                || type == SerializedPropertyType.Vector2 || type == SerializedPropertyType.Vector2Int
                || type == SerializedPropertyType.Vector3 || type == SerializedPropertyType.Vector3Int
                || type == SerializedPropertyType.String
                ;
        }
    }
}