using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Localization;
using UnityEngine;

namespace Framework.LocalizationSimple
{
    /// <summary>选择本地化</summary>
    public class SelectableLocalization : LocalizationCompBase
    {
        [Header("选择")]
        [UnityEngine.Serialization.FormerlySerializedAs("_itemsButton")]
        public List<LocalizationItemSelectable> _itemsSelectable;

        public override void SetLanguage(string language)
        {
            base.SetLanguage(language);

            SetLanguageInternal<LocalizationItemSelectable, LocalizationDataSelectableSprite>(_itemsSelectable, language);
        }

        public override void SetLanguage(ILanguageProvider languageProvider)
        {
            base.SetLanguage(languageProvider);

            SetLanguageInternal<LocalizationItemSelectable, LocalizationDataSelectableSprite>(_itemsSelectable, languageProvider);
        }
    }
}