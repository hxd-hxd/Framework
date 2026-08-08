using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Localization;
using UnityEngine;

namespace Framework.LocalizationSimple
{
    /// <summary>按钮本地化</summary>
    public class ButtonLocalization : LocalizationBase
    {
        [Header("按钮")]
        public List<LocalizationItemButton> _itemsButton;

        public override void SetLanguage(string language)
        {
            base.SetLanguage(language);

            SetLanguageInternal<LocalizationItemButton, LocalizationDataButtonSprite>(_itemsButton, language);
        }

        public override void SetLanguage(ILanguageProvider languageProvider)
        {
            base.SetLanguage(languageProvider);

            SetLanguageInternal<LocalizationItemButton, LocalizationDataButtonSprite>(_itemsButton, languageProvider);
        }
    }
}