using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.LocalizationSimple
{
    using LocalizationData = LocalizationDataButtonSprite;

    /// <summary>本地化项</summary>
    [Serializable]
    public class LocalizationItemButton : LocalizationItemBase<LocalizationData>
    {
        public Button _item;
        [SerializeField]
        private List<LocalizationData> _datas = new();

        public override List<LocalizationData> datas { get => _datas; set => _datas = value; }

        protected override void Execute(LocalizationData data)
        {
            if (data != null && data._spriteSwapData != null)
            {
                var spriteSwapData = data._spriteSwapData;
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
        }

        public override void SetLanguage(string language)
        {
            if (_item)
            {
                base.SetLanguage(language);
            }
        }

        public override void SetLanguage<T>(T languageProvider)
        {
            if (_item)
            {
                base.SetLanguage(languageProvider);
            }
        }
    }
}
