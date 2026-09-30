using System.Collections;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;

using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{

    [CustomPropertyDrawer(typeof(MinMax<>))]
    public class MinMaxDrawer : LineCountPropertyDrawer, IWidthAwarePropertyDrawer
    {
        /// <summary>
        /// 是否 <see cref="MinMax{T}"/>
        /// </summary>
        protected bool isMinMaxT = true;
        /// <summary>
        /// <see cref="MinMax{T}"/> 泛型 T 的类型
        /// </summary>
        protected SerializedPropertyType minMaxTType;

        /// <summary>最近一次测高/绘制使用的宽度（同帧缓存实例可跨 GetPropertyHeight→OnGUI 复用）。</summary>
        private float _layoutWidth;

        /// <summary>字段高 + 提示高（纯像素，不经 lineCount）。</summary>
        protected struct AttributeLayout
        {
            public float fieldHeight;
            public float hintHeight;
            public float Total => fieldHeight + hintHeight;
        }

        public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(pos, property, label);
            if (pos.width > 1f)
                _layoutWidth = pos.width;

            label = EditorGUI.BeginProperty(originalPos, label, property);
            var layout = MeasureLayout(property, label, ResolveLayoutWidth());

            // 非 MinMax<T> 泛型类
            if (property.type != "MinMax`1")
            {
                isMinMaxT = false;
                // 外层（如 PropertyVariable）已画过带标题 Foldout 时会传 none；
                // 再 PropertyField(父级, none) 会多出第二层无标题折页，故只画子字段。
                if (ShouldDrawChildrenOnly(property, label))
                    DrawChildrenOnly(GetAttributeFieldRect(layout), property);
                else
                    OnAttribute(GetAttributeFieldRect(layout), property, label);
                DrawAttributeHint(layout, property);

                EditorGUI.EndProperty();
                return;
            }

            var min = property.FindPropertyRelative("min");
            var max = property.FindPropertyRelative("max");

            isMinMaxT = true;
            minMaxTType = min.propertyType;
            if (IsStackedLine(min.propertyType))
            {
                DrawStackedMinMax(pos, min, max, label);
                DrawAttributeHint(layout, min);
            }
            else if (IsUniline(min.propertyType))
            {
                pos.height = layout.fieldHeight;
                pos = DrawMinMaxPrefixLabel(pos, label);
                PropertyField1(pos, min, label);
                PropertyField1(pos, max, label, 1);
                DrawAttributeHint(layout, min);
            }
            else
            {
                var valueLabel = new GUIContent(property.displayName);
                OnAttribute(GetAttributeFieldRect(layout), property, valueLabel);
                DrawAttributeHint(layout, min);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // 传 0：不把估算宽写入 _layoutWidth，优先保留 OnGUI 缓存的真实宽度
            return GetPropertyHeight(property, label, 0f);
        }

        public float GetPropertyHeight(SerializedProperty property, GUIContent label, float width)
        {
            // 仅在传入真实绘制宽时更新缓存；估算宽（≤0 或外层 Estimate）不覆盖上一帧 OnGUI 宽度
            if (width > 1f)
                _layoutWidth = width;

            lineCount = 0;
            var layout = MeasureLayout(property, label, ResolveLayoutWidth());
            propertyHeight = layout.Total;
            return totalHeight;
        }

        /// <summary>
        /// 与 OnGUI 共用的布局测量：字段高度 + 提示高度。
        /// </summary>
        protected virtual AttributeLayout MeasureLayout(SerializedProperty property, GUIContent label, float width)
        {
            SerializedProperty hintProp = property;
            float fieldHeight;

            if (property.type == "MinMax`1")
            {
                isMinMaxT = true;
                var min = property.FindPropertyRelative("min");
                if (min != null)
                {
                    minMaxTType = min.propertyType;
                    hintProp = min;
                }

                if (min != null && IsStackedLine(min.propertyType))
                    fieldHeight = GetStackedLineFieldHeight();
                else if (min != null && IsUniline(min.propertyType))
                    fieldHeight = singleLineHeight;
                else
                    fieldHeight = AmendFieldHeight(EditorGUI.GetPropertyHeight(property, label, true));
            }
            else
            {
                isMinMaxT = false;
                if (ShouldDrawChildrenOnly(property, label))
                    fieldHeight = GetChildrenOnlyHeight(property);
                else
                    fieldHeight = AmendFieldHeight(EditorGUI.GetPropertyHeight(property, label, true));
            }

            float hintHeight = 0f;
            if (OnAttributeHint(hintProp, out string msg, out var msgType))
                hintHeight = CalcAttributeHintHeight(msg, width, msgType);

            return new AttributeLayout
            {
                fieldHeight = fieldHeight,
                hintHeight = hintHeight,
            };
        }

        /// <summary>
        /// label 已被外层消费且属性有子字段时，只画子内容，避免第二层无标题折页。
        /// </summary>
        protected static bool ShouldDrawChildrenOnly(SerializedProperty property, GUIContent label)
        {
            if (property == null || !property.hasVisibleChildren)
                return false;
            return label == null || string.IsNullOrEmpty(label.text);
        }

        protected static float GetChildrenOnlyHeight(SerializedProperty property)
        {
            if (property == null || !property.hasVisibleChildren)
                return 0f;

            var child = property.Copy();
            var end = property.GetEndProperty();
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

        protected static void DrawChildrenOnly(Rect rect, SerializedProperty property)
        {
            if (property == null || !property.hasVisibleChildren)
                return;

            var child = property.Copy();
            var end = property.GetEndProperty();
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

        protected float ResolveLayoutWidth()
        {
            if (_layoutWidth > 1f)
                return _layoutWidth;
            if (originalPos.width > 1f)
                return originalPos.width;
            return PropertyAttributeForwardingUtility.EstimateLayoutWidth();
        }

        /// <summary>
        /// 使用 Unity HelpBox 样式按宽度自动换行计算提示高度。
        /// </summary>
        protected virtual float CalcAttributeHintHeight(string msg, float width, MessageType type = MessageType.None)
        {
            // HelpBox 有图标时文本区变窄，必须按缩小后的宽度算换行，否则多行会低估高度
            const float helpBoxIconPad = 40f;
            float textWidth = type != MessageType.None
                ? Mathf.Max(width - helpBoxIconPad, 40f)
                : width;

            float height = EditorStyles.helpBox.CalcHeight(new GUIContent(msg), textWidth);
            if (type != MessageType.None)
                height = Mathf.Max(height, 40f);
            return height;
        }

        /// <summary>
        /// 属性字段区域：按布局中的字段高度绘制，提示区另计。
        /// </summary>
        protected Rect GetAttributeFieldRect(AttributeLayout layout)
        {
            return new Rect(originalPos.x, originalPos.y, originalPos.width, layout.fieldHeight);
        }

        /// <summary>修正属性高度（返回修正后的像素高）</summary>
        protected virtual float AmendFieldHeight(float fieldHeight)
        {
            // 处理 Vector4 内联布局的高度显示问题
            if (IsUnilineSerializedPropertyType(SerializedPropertyType.Vector4)
                && minMaxTType == SerializedPropertyType.Vector4)
            {
                // 除非 Unity 在后续版本中做了修改，否则这里的数值是固定的
                int h = (int)fieldHeight;
                if (h == 58) return 18;
                if (h == 138 || h == 218) return 100;
            }

            return fieldHeight;
        }

        /// <summary>
        /// 处理特性，最终进行
        /// </summary>
        protected virtual void OnAttribute(Rect pos, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(pos, property, label, true);
        }

        /// <summary>在字段下方绘制提示（高度已计入 MeasureLayout）。</summary>
        protected virtual void DrawAttributeHint(AttributeLayout layout, SerializedProperty property)
        {
            if (layout.hintHeight <= 0f)
                return;
            if (!OnAttributeHint(property, out string msg, out var msgType))
                return;

            var br = new Rect(
                originalPos.x,
                originalPos.y + layout.fieldHeight,
                ResolveLayoutWidth(),
                layout.hintHeight);

            var old_contentColor = GUI.contentColor;
            var old_backgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.yellow;
            GUI.contentColor = new Color(1, 1, 60 / 255f, 1);
            EditorGUI.HelpBox(br, msg, msgType);
            GUI.backgroundColor = old_backgroundColor;
            GUI.contentColor = old_contentColor;
        }

        /// <summary>应用特性时的提示信息</summary>
        protected virtual bool OnAttributeHint(SerializedProperty property, out string msg, out MessageType type)
        {
            msg = null;
            type = MessageType.None;
            return false;
        }

        /// <summary>默认绘制</summary>
        protected virtual void OnDefault(Rect pos, SerializedProperty property, GUIContent label)
        {
            OnAttribute(pos, property, label);
        }

        /// <summary>
        /// 标题跟在第一行 min 前面；PrefixLabel 只用单行高度，避免标题在两行间垂直居中。
        /// </summary>
        protected Rect DrawMinMaxPrefixLabel(Rect pos, GUIContent label)
        {
            EditorGUIUtilityExtend.SetLabelWidth(EditorGUIUtility.labelWidth * 0.75f,
                () => pos = EditorGUI.PrefixLabel(pos, GUIUtility.GetControlID(FocusType.Keyboard), label));
            return pos;
        }

        /// <summary>
        /// Vector3 等：min 跟在标题后，max 另起一行并与 min 对齐。
        /// </summary>
        protected virtual void DrawStackedMinMax(Rect pos, SerializedProperty min, SerializedProperty max, GUIContent label)
        {
            pos.height = singleLineHeight;
            pos = DrawMinMaxPrefixLabel(pos, label);

            float spacing = EditorGUIUtility.standardVerticalSpacing;
            var minRect = new Rect(pos.x, pos.y, pos.width, singleLineHeight);
            var maxRect = new Rect(pos.x, pos.y + singleLineHeight + spacing, pos.width, singleLineHeight);

            OnUniline(minRect, min, new GUIContent(min.displayName), 0, 1f);
            OnUniline(maxRect, max, new GUIContent(max.displayName), 0, 1f);
        }

        protected float GetStackedLineFieldHeight()
        {
            return singleLineHeight * 2f + EditorGUIUtility.standardVerticalSpacing;
        }

        /// <summary>
        /// 执行单行绘制
        /// </summary>
        /// <param name="widthRatio">值区域占剩余宽度的比例；并排 min/max 为约一半，上下堆叠为 1。</param>
        protected virtual void OnUniline(Rect pos, SerializedProperty property, GUIContent label, int level = 0, float widthRatio = 0.495f)
        {
            int l = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            float unit = 4;
            float nameWidth = 30;// 名字宽度
            float vWidth = pos.width * widthRatio;// 值宽度
            float vOffsetX = (vWidth + unit) * level;// 值 x 偏移

            // 使用适应性的宽度
            var vRect = new Rect(pos.x + vOffsetX, pos.y, vWidth, pos.height);

            EditorGUIUtilityExtend.SetLabelWidth(nameWidth,
                    () => OnAttribute(vRect, property, label));

            EditorGUI.indentLevel = l;
        }

        [Obsolete]
        protected void PropertyField(Rect pos, SerializedProperty property, GUIContent label, int level = 0)
        {
            var displayName = new GUIContent(property.displayName);
            if (IsUniline(property.propertyType))
            {
                int l = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;

                float unit = 4;
                float nameWidth = 30;// 名字宽度
                float vWidth = pos.width * 0.375f;// 值宽度
                float nameOffsetX = (nameWidth + vWidth + unit * 3) * level;// 名字 x 偏移
                float vOffsetX = (nameOffsetX) * level + nameWidth;// 值 x 偏移

                // 使用适应性的宽度
                var nameRect = new Rect(pos.x + nameOffsetX, pos.y, nameWidth, pos.height);
                var vRect = new Rect(pos.x + vOffsetX, pos.y, vWidth, pos.height);
                EditorGUI.HandlePrefixLabel(pos, nameRect, displayName);

                OnAttribute(vRect, property, GUIContent.none);

                EditorGUI.indentLevel = l;
            }
            else
            {
                OnDefault(pos, property, displayName);
            }
        }
        protected void PropertyField1(Rect pos, SerializedProperty property, GUIContent label, int level = 0)
        {
            var displayName = new GUIContent(property.displayName);
            if (IsUniline(property.propertyType))
            {
                OnUniline(pos, property, displayName, level);
            }
            else
            {
                // 如果不满足单行绘制条件，则使用默认绘制
                OnDefault(pos, property, displayName);
            }
        }

        /// <summary>是否在一行中显示</summary>
        protected bool IsUniline(SerializedPropertyType type)
        {
            return IsUnilineSerializedPropertyType(type);
        }
        /// <summary>
        /// Vector3 分量较多，min/max 并排会挤；改为两行：标题后跟 min，max 另起一行。
        /// </summary>
        protected bool IsStackedLine(SerializedPropertyType type)
        {
            return type == SerializedPropertyType.Vector3
                || type == SerializedPropertyType.Vector3Int;
        }
        /// <summary>指定的 <see cref="SerializedPropertyType"/> 是否在一行中显示</summary>
        protected bool IsUnilineSerializedPropertyType(SerializedPropertyType type)
        {
            return type == SerializedPropertyType.Float
                || type == SerializedPropertyType.Integer
                || type == SerializedPropertyType.Boolean
                || type == SerializedPropertyType.Enum
                || type == SerializedPropertyType.Color
                || type == SerializedPropertyType.Vector2 || type == SerializedPropertyType.Vector2Int
                ;
        }
    }
}
