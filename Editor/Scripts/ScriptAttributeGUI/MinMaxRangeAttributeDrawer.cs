using UnityEditor;
using UnityEngine;
using Framework;

namespace Framework.Editor
{
    [CustomPropertyDrawer(typeof(MinMaxRangeAttribute))]
    internal class MinMaxRangeAttributeDrawer : MinMaxDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (PropertyVariableAttributeNestingDrawer.ShouldNest(property, fieldInfo))
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            base.OnGUI(position, property, label);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (PropertyVariableAttributeNestingDrawer.ShouldNest(property, fieldInfo))
                return EditorGUI.GetPropertyHeight(property, label, true);

            return base.GetPropertyHeight(property, label);
        }

        protected override void OnAttribute(Rect pos, SerializedProperty property, GUIContent label)
        {
            var range = attribute as MinMaxRangeAttribute;
            if (property.propertyType == SerializedPropertyType.Float)
            {
                EditorGUI.Slider(pos, property, range.min, range.max, label);
            }
            else if (property.propertyType == SerializedPropertyType.Integer)
            {
                EditorGUI.IntSlider(pos, property, (int)range.min, (int)range.max, label);
            }
            else
            {
                base.OnAttribute(pos, property, label);
            }
        }

        protected override bool OnAttributeHint(SerializedProperty property, out string msg, out MessageType type)
        {
            bool supported = property.propertyType == SerializedPropertyType.Float
                || property.propertyType == SerializedPropertyType.Integer;
            if (supported)
            {
                msg = null;
                type = MessageType.None;
                return false;
            }

            msg = !isMinMaxT
                ? $"MinMaxRangeAttribute 不支持的类型 \"{property.type}\"，仅支持设定 \"float 、int\" 等数值类型"
                : $"MinMaxRangeAttribute 不支持的类型 \"MinMax<{property.type}>\"，仅支持设定 MinMax<T> T 为 \"float 、int\" 等数值类型";
            type = MessageType.Error;
            return true;
        }
    }
}
