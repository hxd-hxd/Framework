using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Framework;

namespace Framework.Editor
{
    /// <summary>
    /// <see cref="SubclassSelectAttribute"/> 属性绘制：以下拉菜单列出父类型的所有子类，
    /// 选中后将对应类型全名写入字符串字段。
    /// </summary>
    [CustomPropertyDrawer(typeof(SubclassSelectAttribute))]
    public class SubclassSelectAttributeDrawer : PropertyDrawer
    {
        const string NoneLabel = "无";

        class TypeOptions
        {
            public string[] fullNames;
            public GUIContent[] displays;
            public string[] popupValues;
            public GUIContent[] popupDisplays;
        }

        static readonly Dictionary<Type, TypeOptions> s_options = new Dictionary<Type, TypeOptions>();

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
                    $"SubclassSelectAttribute 不支持的类型 \"{property.type}\"，仅支持 string",
                    MessageType.Error);
                EditorGUI.EndProperty();
                return;
            }

            var attr = (SubclassSelectAttribute)attribute;
            if (attr.parentType == null)
            {
                EditorGUI.HelpBox(position, "SubclassSelectAttribute 的 parentType 为空", MessageType.Error);
                EditorGUI.EndProperty();
                return;
            }

            var options = GetOptions(attr.parentType);
            BuildPopup(options, property.stringValue, out var displays, out var values, out int index);

            EditorGUI.BeginChangeCheck();
            int newIndex = EditorGUI.Popup(position, label, index, displays);
            if (EditorGUI.EndChangeCheck() && newIndex >= 0 && newIndex < values.Length)
                property.stringValue = values[newIndex];

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (PropertyVariableAttributeNestingDrawer.ShouldNest(property, fieldInfo))
                return EditorGUI.GetPropertyHeight(property, label, true);

            if (property.propertyType != SerializedPropertyType.String
                || ((SubclassSelectAttribute)attribute).parentType == null)
                return EditorGUIUtility.singleLineHeight * 2f + 2f;

            return EditorGUIUtility.singleLineHeight;
        }

        static TypeOptions GetOptions(Type parentType)
        {
            if (s_options.TryGetValue(parentType, out var cached))
                return cached;

            var derived = TypeCache.GetTypesDerivedFrom(parentType);
            var types = new List<Type>(derived.Count);
            for (int i = 0; i < derived.Count; i++)
            {
                var type = derived[i];
                if (type == null || type.IsAbstract || type.IsInterface || type.IsGenericTypeDefinition)
                    continue;
                if (string.IsNullOrEmpty(type.FullName))
                    continue;
                types.Add(type);
            }

            types.Sort(CompareTypes);

            var nameCounts = new Dictionary<string, int>(types.Count);
            for (int i = 0; i < types.Count; i++)
            {
                string name = types[i].Name;
                nameCounts.TryGetValue(name, out int count);
                nameCounts[name] = count + 1;
            }

            var fullNames = new string[types.Count];
            var displays = new GUIContent[types.Count];
            for (int i = 0; i < types.Count; i++)
            {
                var type = types[i];
                string fullName = type.FullName;
                fullNames[i] = fullName;
                string label = nameCounts[type.Name] > 1 ? fullName : type.Name;
                displays[i] = new GUIContent(label, fullName);
            }

            var popupValues = new string[fullNames.Length + 1];
            var popupDisplays = new GUIContent[displays.Length + 1];
            popupValues[0] = string.Empty;
            popupDisplays[0] = new GUIContent(NoneLabel);
            Array.Copy(fullNames, 0, popupValues, 1, fullNames.Length);
            Array.Copy(displays, 0, popupDisplays, 1, displays.Length);

            var options = new TypeOptions
            {
                fullNames = fullNames,
                displays = displays,
                popupValues = popupValues,
                popupDisplays = popupDisplays,
            };
            s_options[parentType] = options;
            return options;
        }

        static int CompareTypes(Type a, Type b)
        {
            int name = string.CompareOrdinal(a.Name, b.Name);
            return name != 0 ? name : string.CompareOrdinal(a.FullName, b.FullName);
        }

        static void BuildPopup(
            TypeOptions options,
            string current,
            out GUIContent[] displays,
            out string[] values,
            out int index)
        {
            if (string.IsNullOrEmpty(current))
            {
                displays = options.popupDisplays;
                values = options.popupValues;
                index = 0;
                return;
            }

            for (int i = 0; i < options.fullNames.Length; i++)
            {
                if (options.fullNames[i] == current)
                {
                    displays = options.popupDisplays;
                    values = options.popupValues;
                    index = i + 1;
                    return;
                }
            }

            displays = new GUIContent[options.popupDisplays.Length + 1];
            values = new string[options.popupValues.Length + 1];
            displays[0] = options.popupDisplays[0];
            values[0] = options.popupValues[0];
            displays[1] = new GUIContent(current + " (缺失)", current);
            values[1] = current;
            Array.Copy(options.popupDisplays, 1, displays, 2, options.popupDisplays.Length - 1);
            Array.Copy(options.popupValues, 1, values, 2, options.popupValues.Length - 1);
            index = 1;
        }
    }
}
