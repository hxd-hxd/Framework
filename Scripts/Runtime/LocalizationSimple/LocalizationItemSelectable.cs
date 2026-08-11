using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.LocalizationSimple
{
    using LocalizationData = LocalizationDataSelectableSprite;

    /// <summary>选择本地化项，逻辑功能导向的</summary>
    [Serializable]
    public class LocalizationItemSelectable : LocalizationItemBase<LocalizationData>
    {
        // 为了兼容 Unity 的序列化，暂时不删
        [Obsolete("使用 _itemS 替换")]
        public Button _item;

        public Selectable _itemS;

        [SerializeField]
        private List<LocalizationData> _datas = new List<LocalizationData>();

        public override List<LocalizationData> datas { get => _datas; set => _datas = value; }

        protected override void Execute(LocalizationData data)
        {
            if (data != null && data._spriteSwapData != null)
            {
                var spriteSwapData = data._spriteSwapData;

                if (_item)
                {
                    var spriteState = _item.spriteState;

                    if (spriteSwapData._pressedSprite._enable)
                        spriteState.pressedSprite = spriteSwapData._pressedSprite._sprite;

                    if (spriteSwapData._highlightedSprite._enable)
                        spriteState.highlightedSprite = spriteSwapData._highlightedSprite._sprite;

                    if (spriteSwapData._selectedSprite._enable)
                        spriteState.selectedSprite = spriteSwapData._selectedSprite._sprite;

                    if (spriteSwapData._disabledSprite._enable)
                        spriteState.disabledSprite = spriteSwapData._disabledSprite._sprite;

                    _item.spriteState = spriteState;
                }

                if (_itemS)
                {
                    // 选择
                    var spriteStateS = _itemS.spriteState;

                    if (spriteSwapData._pressedSprite._enable)
                        spriteStateS.pressedSprite = spriteSwapData._pressedSprite._sprite;

                    if (spriteSwapData._highlightedSprite._enable)
                        spriteStateS.highlightedSprite = spriteSwapData._highlightedSprite._sprite;

                    if (spriteSwapData._selectedSprite._enable)
                        spriteStateS.selectedSprite = spriteSwapData._selectedSprite._sprite;

                    if (spriteSwapData._disabledSprite._enable)
                        spriteStateS.disabledSprite = spriteSwapData._disabledSprite._sprite;

                    _itemS.spriteState = spriteStateS;
                }
            }
        }

        public override void SetLanguage(string language)
        {
            if (_item || _itemS)
            {
                base.SetLanguage(language);
            }
        }

        public override void SetLanguage(ILanguageProvider languageProvider)
        {
            if (_item || _itemS)
            {
                base.SetLanguage(languageProvider);
            }
        }
    }
}
