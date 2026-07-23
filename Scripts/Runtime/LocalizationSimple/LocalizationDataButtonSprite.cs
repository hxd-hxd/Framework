using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.LocalizationSimple
{
    #region 编辑器扩展

#if UNITY_EDITOR
    using UnityEditor;

    /// <summary><see cref="LocalizationDataButtonSprite.SpriteData"/> 属性绘制</summary>
    [CustomPropertyDrawer(typeof(LocalizationDataButtonSprite.SpriteData))]
    public class LocalizationDataButtonSpriteSpriteDataDrawer : PropertyDrawer
    {
        const float ToggleWidth = 18f;
        const float Spacing = 4f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);
            string labelText = string.IsNullOrEmpty(label.text) ? property.displayName : label.text;

            var enableProp = property.FindPropertyRelative("_enable");
            var spriteProp = property.FindPropertyRelative("_sprite");

            float indentPx = EditorGUI.indentLevel * 15f;
            float toggleSpace = ToggleWidth + Spacing;

            // 用前导空格把属性名右移，给开关留位置（PrefixLabel 是此前唯一能显示属性名的方式）
            float spaceW = EditorStyles.label.CalcSize(new GUIContent(" ")).x;
            int spaces = Mathf.Max(1, Mathf.CeilToInt(toggleSpace / Mathf.Max(spaceW, 1f)));
            var drawnLabel = new GUIContent(new string(' ', spaces) + labelText, label.image, label.tooltip);

            Rect contentRect = EditorGUI.PrefixLabel(position, drawnLabel);

            int oldIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var toggleRect = new Rect(position.x + indentPx, position.y, ToggleWidth, position.height);
            EditorGUI.PropertyField(toggleRect, enableProp, GUIContent.none);
            EditorGUI.PropertyField(contentRect, spriteProp, GUIContent.none);

            EditorGUI.indentLevel = oldIndent;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
#endif

    #endregion

    /// <summary>本地化数据</summary>
    [Serializable]
    public class LocalizationDataButtonSprite : LocalizationDataBase
    {
        public SpriteSwapData _spriteSwapData;

        [Serializable]
        public class SpriteSwapData
        {
            /// <summary>高亮</summary>
            public SpriteData _highlightedSprite;

            /// <summary>按压</summary>
            public SpriteData _pressedSprite;

            /// <summary>选中</summary>
            public SpriteData _selectedSprite;

            /// <summary>禁用</summary>
            public SpriteData _disabledSprite;
        }

        [Serializable]
        public class SpriteData
        {
            public bool _enable;

            /// <summary>精灵</summary>
            public Sprite _sprite;
        }
    }
}