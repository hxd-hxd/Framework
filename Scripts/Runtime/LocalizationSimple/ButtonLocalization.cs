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
            _currentLanguage = language;

            //var cur = language;
            //var def = _defaultLanguage;

            //if (_itemsButton != null && _itemsButton.Count > 0)
            //    foreach (var item in _itemsButton)
            //    {
            //        SetItemLanguage(item, cur, def);
            //    }

            SetLanguageInternal<LocalizationItemButton, LocalizationDataButtonSprite>(_itemsButton, language);
        }

        public override void SetLanguage<T>(T languageProvider)
        {
            var langProvider = languageProvider as LanguageProviderComponentBase;
            if(langProvider == null)
            {
                return;
            }
            _currentProvider = langProvider;

            //var cur = langProvider;
            //var def = _defaultProvider;

            //if (_itemsButton != null && _itemsButton.Count > 0)
            //    foreach (var item in _itemsButton)
            //    {
            //        SetItemLanguage(item, cur, def);
            //    }

            SetLanguageInternal<LocalizationItemButton, LocalizationDataButtonSprite>(_itemsButton, languageProvider);
        }
    }
}