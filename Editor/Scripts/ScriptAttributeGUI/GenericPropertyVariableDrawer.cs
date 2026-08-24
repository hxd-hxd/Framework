using System;
using System.Reflection;

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Framework.Runtime;

namespace Framework.Editor
{
    [CustomPropertyDrawer(typeof(PropertyVariable<>))]
    public class GenericPropertyVariableDrawer : LineCountPropertyDrawer
    {
        private const float FieldSpacing = 2f;
        /// <summary>列表中折页元素相对拖拽条再缩进一级（Unity 默认 15px），再往前 2px。</summary>
        private const float ListFoldoutElementIndent = 13f;

        private PropertyDrawer _cachedValueDrawer;
        private bool _valueDrawerResolved;
        private bool _valueDrawerUsable;
        private bool? _hideEvent;
        private ReorderableList _cachedList;
        private string _cachedListPath;

        public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(pos, property, label);
            label = EditorGUI.BeginProperty(pos, label, property);

            var value = property.FindPropertyRelative("_value");
            var onChangeCallback = property.FindPropertyRelative("_onChangeCallback");

            float y = pos.y;
            if (ShouldSplitFoldoutHeader(value, label))
            {
                float headerHeight = singleLineHeight;
                DrawSplitFoldoutHeader(new Rect(pos.x, y, pos.width, headerHeight), value, label);
                y += headerHeight;

                if (value.isExpanded)
                {
                    float contentHeight = GetSplitValueContentHeight(value, pos.width);
                    DrawSplitValueContent(new Rect(pos.x, y, pos.width, contentHeight), value);
                    y += contentHeight;
                }
            }
            else
            {
                float valueHeight = GetValueHeight(value, label, pos.width);
                DrawValue(new Rect(pos.x, y, pos.width, valueHeight), value, label);
                y += valueHeight;
            }

            if (onChangeCallback != null && ShouldDrawOnChangeCallback())
            {
                y += FieldSpacing;
                bool includeChildren = onChangeCallback.isExpanded;
                float callbackHeight = EditorGUI.GetPropertyHeight(onChangeCallback, includeChildren);
                EditorGUI.indentLevel++;
                PropertyField(new Rect(pos.x, y, pos.width, callbackHeight), onChangeCallback, includeChildren);
                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            lineCount = 0;
            // 传 0：优先复用转发 Drawer 上一帧 OnGUI 缓存的真实宽度，避免估算宽覆盖导致换行不一致
            propertyHeight = MeasureStackHeight(property, label, 0f);
            return totalHeight;
        }

        /// <summary>
        /// 垂直栈：可选 Foldout 标题行 + value 内容 + 可选 callback。
        /// 拆标题时总高按「标题一行 + none 口径内容」分配，与绘制一致，避免 hWith 多算一行。
        /// </summary>
        protected float MeasureStackHeight(SerializedProperty property, GUIContent label, float width)
        {
            var value = property.FindPropertyRelative("_value");
            var onChangeCallback = property.FindPropertyRelative("_onChangeCallback");

            float height;
            if (ShouldSplitFoldoutHeader(value, label))
            {
                height = singleLineHeight;
                if (value.isExpanded)
                    height += GetSplitValueContentHeight(value, width);
            }
            else
            {
                height = GetValueHeight(value, label, width);
            }

            if (onChangeCallback != null && ShouldDrawOnChangeCallback())
            {
                height += FieldSpacing;
                height += EditorGUI.GetPropertyHeight(onChangeCallback, onChangeCallback.isExpanded);
            }

            return height;
        }

        /// <summary>
        /// 拆出 Foldout 标题后的内容高度：列表优先 ReorderableList；
        /// 否则转发 Drawer 以 none 测高；再否则只计子属性。
        /// </summary>
        protected float GetSplitValueContentHeight(SerializedProperty value, float width)
        {
            if (IsSerializedList(value))
                return GetReorderableList(value).GetHeight();
            if (TryGetUsableValueDrawer(value, out _))
                return GetValueHeight(value, GUIContent.none, width);
            return GetChildrenOnlyHeight(value);
        }

        protected void DrawSplitValueContent(Rect rect, SerializedProperty value)
        {
            if (IsSerializedList(value))
            {
                GetReorderableList(value).DoList(rect);
                return;
            }

            if (TryGetUsableValueDrawer(value, out var valueDrawer))
            {
                if (!PropertyAttributeForwardingUtility.TryDrawForwarded(rect, value, GUIContent.none, valueDrawer))
                    valueDrawer.OnGUI(rect, value, GUIContent.none);
                return;
            }

            DrawChildrenOnly(rect, value);
        }

        protected float GetValueHeight(SerializedProperty value, GUIContent valueLabel, float width)
        {
            if (TryGetUsableValueDrawer(value, out var valueDrawer))
            {
                if (valueDrawer is IWidthAwarePropertyDrawer widthAware)
                    return widthAware.GetPropertyHeight(value, valueLabel, width);
                return valueDrawer.GetPropertyHeight(value, valueLabel);
            }

            return EditorGUI.GetPropertyHeight(value, valueLabel, true);
        }

        protected void DrawValue(Rect rect, SerializedProperty value, GUIContent valueLabel)
        {
            if (TryGetUsableValueDrawer(value, out var valueDrawer))
            {
                if (!PropertyAttributeForwardingUtility.TryDrawForwarded(rect, value, valueLabel, valueDrawer))
                    valueDrawer.OnGUI(rect, value, valueLabel);
                return;
            }

            PropertyField(rect, value, valueLabel);
        }

        bool TryGetUsableValueDrawer(SerializedProperty value, out PropertyDrawer drawer)
        {
            if (!_valueDrawerResolved)
            {
                drawer = null;
                if (fieldInfo == null)
                    return false;

                _valueDrawerResolved = true;
                if (PropertyAttributeForwardingUtility.TryGetForwardedDrawer(fieldInfo, value, out _cachedValueDrawer))
                {
                    _valueDrawerUsable = PropertyAttributeForwardingUtility.CanSafelyUseDrawer(
                        _cachedValueDrawer,
                        value);
                }
            }

            drawer = _cachedValueDrawer;
            return _valueDrawerUsable;
        }

        /// <summary>仅绘制子属性，不对父级 PropertyField(none)，避免第二层无标题折页。</summary>
        protected void DrawChildrenOnly(Rect rect, SerializedProperty value)
        {
            if (value == null || !value.hasVisibleChildren)
                return;

            var child = value.Copy();
            var end = value.GetEndProperty();
            if (!child.NextVisible(true))
                return;

            float y = rect.y;
            float spacing = EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.indentLevel++;
            while (!SerializedProperty.EqualContents(child, end))
            {
                float h = EditorGUI.GetPropertyHeight(child, null, true);
                EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, h), child, true);
                y += h + spacing;
                if (!child.NextVisible(false))
                    break;
            }
            EditorGUI.indentLevel--;
        }

        /// <summary>展开后仅子属性的高度（不含折页标题行）。</summary>
        protected float GetChildrenOnlyHeight(SerializedProperty value)
        {
            if (value == null || !value.hasVisibleChildren)
                return 0f;

            var child = value.Copy();
            var end = value.GetEndProperty();
            if (!child.NextVisible(true))
                return 0f;

            float height = 0f;
            float spacing = EditorGUIUtility.standardVerticalSpacing;
            while (!SerializedProperty.EqualContents(child, end))
            {
                height += EditorGUI.GetPropertyHeight(child, null, true) + spacing;
                if (!child.NextVisible(false))
                    break;
            }

            if (height > 0f)
                height -= spacing;
            return height;
        }

        /// <summary>是否绘制 <c>onChangeCallback</c></summary>
        protected virtual bool ShouldDrawOnChangeCallback()
        {
            if (!_hideEvent.HasValue)
            {
                _hideEvent = fieldInfo != null
                    && fieldInfo.GetCustomAttribute<PropertyVariableHideEventAttribute>(true) != null;
            }

            return !_hideEvent.Value;
        }

        /// <summary>
        /// 折页类 <c>_value</c> 同时传外层 label 时，Unity 会多算约一行；
        /// 拆成一次 Foldout（箭头+标题同行）+ 仅内容。无可见子属性则不拆，避免误伤单行类型。
        /// </summary>
        protected virtual bool ShouldSplitFoldoutHeader(SerializedProperty value, GUIContent outerLabel)
        {
            if (value == null || outerLabel == null || string.IsNullOrEmpty(outerLabel.text))
                return false;
            if (!value.hasVisibleChildren)
                return false;
            return IsFoldoutLikeProperty(value);
        }

        /// <summary>
        /// 真正走折页绘制的类型。List/T[] 的 propertyType 也是 Generic，会走拆标题：
        /// 外层 Foldout 画字段名，列表 Size 放标题行右侧（无 Size 字），内容为可拖拽元素。
        /// Vector2/3 等虽有子字段，但是多字段单行，不能当折页拆。
        /// </summary>
        protected static bool IsFoldoutLikeProperty(SerializedProperty value)
        {
            var t = value.propertyType;
            return t == SerializedPropertyType.Generic
                || t == SerializedPropertyType.ManagedReference
                || t == SerializedPropertyType.Vector4;
        }

        /// <summary>非 string 的数组 / <c>List&lt;T&gt;</c>。</summary>
        protected static bool IsSerializedList(SerializedProperty value)
        {
            return value != null
                && value.isArray
                && value.propertyType != SerializedPropertyType.String;
        }

        /// <summary>与 Unity 默认数组头一致：Foldout + 标题，右侧无标签数字改长度。</summary>
        void DrawSplitFoldoutHeader(Rect rect, SerializedProperty value, GUIContent label)
        {
            if (IsSerializedList(value))
            {
                const float sizeWidth = 48f;
                const float gap = 2f;
                const float rightInset = 2f;
                var foldoutRect = new Rect(rect.x, rect.y, Mathf.Max(0f, rect.width - sizeWidth - gap - rightInset), rect.height);
                value.isExpanded = EditorGUI.Foldout(foldoutRect, value.isExpanded, label, true);
                DrawArraySizeField(new Rect(rect.xMax - sizeWidth - rightInset, rect.y, sizeWidth, rect.height), value);
                return;
            }

            value.isExpanded = EditorGUI.Foldout(rect, value.isExpanded, label, true);
        }

        static void DrawArraySizeField(Rect rect, SerializedProperty value)
        {
            var sizeProp = value != null ? value.FindPropertyRelative("Array.size") : null;
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            if (sizeProp != null)
            {
                EditorGUI.BeginProperty(rect, GUIContent.none, sizeProp);
                EditorGUI.BeginChangeCheck();
                int newSize = EditorGUI.DelayedIntField(rect, GUIContent.none, sizeProp.intValue);
                if (EditorGUI.EndChangeCheck())
                    sizeProp.intValue = Mathf.Max(0, newSize);
                EditorGUI.EndProperty();
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                int newSize = EditorGUI.DelayedIntField(rect, GUIContent.none, value.arraySize);
                if (EditorGUI.EndChangeCheck())
                    value.arraySize = Mathf.Max(0, newSize);
            }

            EditorGUI.indentLevel = indent;
        }

        /// <summary>无标题 ReorderableList：字段名和 Size 在外层 Foldout 行。</summary>
        ReorderableList GetReorderableList(SerializedProperty value)
        {
            if (_cachedList != null
                && _cachedList.serializedProperty != null
                && _cachedList.serializedProperty.serializedObject == value.serializedObject
                && _cachedListPath == value.propertyPath)
            {
                _cachedList.serializedProperty = value;
                return _cachedList;
            }

            var list = new ReorderableList(value.serializedObject, value, true, false, true, true)
            {
                headerHeight = 0f,
            };
            list.drawElementCallback = DrawReorderableElement;
            list.elementHeightCallback = GetReorderableElementHeight;
            _cachedList = list;
            _cachedListPath = value.propertyPath;
            return list;
        }

        void DrawReorderableElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var listProp = _cachedList != null ? _cachedList.serializedProperty : null;
            if (listProp == null || index < 0 || index >= listProp.arraySize)
                return;

            var element = listProp.GetArrayElementAtIndex(index);
            rect.y += 1f;
            rect.height = EditorGUI.GetPropertyHeight(element, true);
            if (IsFoldoutLikeProperty(element) && rect.width > ListFoldoutElementIndent)
            {
                rect.x += ListFoldoutElementIndent;
                rect.width -= ListFoldoutElementIndent;
            }

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            EditorGUI.PropertyField(rect, element, true);
            EditorGUI.indentLevel = indent;
        }

        float GetReorderableElementHeight(int index)
        {
            var listProp = _cachedList != null ? _cachedList.serializedProperty : null;
            if (listProp == null || index < 0 || index >= listProp.arraySize)
                return singleLineHeight;

            var element = listProp.GetArrayElementAtIndex(index);
            return EditorGUI.GetPropertyHeight(element, true)
                + EditorGUIUtility.standardVerticalSpacing;
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
