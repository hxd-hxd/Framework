using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Localization;
using UnityEngine;

namespace Framework.LocalizationSimple
{
    /// <summary>本地化引用</summary>
    public class LocalizationBaseReference : LocalizationCompBase
    {
        [Header("本地化列表")]
        [SerializeField]
        private List<LocalizationCompBase> _localizations;

        public List<LocalizationCompBase> localizations { get => _localizations; }

        public override void GetAllItem(ref List<ILocalizationItem> items)
        {
            foreach (var localization in localizations)
            {
                if (localization != null)
                {
                    localization.GetAllItem(ref items);
                }
            }
        }

        public override void SetLanguage(string language)
        {
            base.SetLanguage(language);

            foreach (var l in _localizations)
            {
                if (ObjectUtility.IsNull(l)) continue;
                l._defaultLanguage = _defaultLanguage;
                l.SetLanguage(language);
            }
        }

        public override void SetLanguage(ILanguageProvider languageProvider)
        {
            base.SetLanguage(languageProvider);

            foreach (var l in _localizations)
            {
                if (ObjectUtility.IsNull(l)) continue;
                l._defaultProvider = _defaultProvider;
                l.SetLanguage(languageProvider);
            }
        }

    }
}