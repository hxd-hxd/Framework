using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Localization;
using UnityEngine;

namespace Framework.LocalizationSimple
{
    /// <summary>本地化引用</summary>
    public class LocalizationBaseReference : LocalizationBase
    {
        [Header("本地化列表")]
        [SerializeField]
        private List<LocalizationBase> _localizations;

        public List<LocalizationBase> localizations { get => _localizations; }

        public override void SetLanguage(string language)
        {
            base.SetLanguage(language);

            foreach (var l in _localizations)
            {
                if (l != null)
                {
                    l._defaultLanguage = _defaultLanguage;
                    l.SetLanguage(language);
                }
            }
        }

        public override void SetLanguage(ILanguageProvider languageProvider)
        {
            base.SetLanguage(languageProvider);

            foreach (var l in _localizations)
            {
                l._defaultProvider = _defaultProvider;
                if (l != null) l.SetLanguage(languageProvider);
            }
        }

    }
}