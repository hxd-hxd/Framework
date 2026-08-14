using System.Collections;
using System.Collections.Generic;
using Framework.Localization;
using UnityEngine;

namespace Framework.LocalizationSimple
{
    /// <summary>本地化设置默认语言</summary>
    [ExecuteAlways]
    public class LocalizationSetDefaultLang : MonoBehaviour
    {
        [SerializeField]
        private string _language;

        [SerializeField]
        private LanguageProviderComponentBase _langProvider;

        [SerializeField]
        private LocalizationSetBase _localizationSet;

        [SerializeField]
        private bool _awakeAutoSetToGlobal = true;

        [SerializeField]
        private bool _awakeAutoSetToLocal = false;

        private void Awake()
        {
            if (!_localizationSet)
            {
                TryGetComponent(out _localizationSet);
            }

            if (Application.isPlaying)
            {
                if (_awakeAutoSetToGlobal)
                    SetToGlobal();
                if (_awakeAutoSetToLocal)
                    SetToLocal();
            }
        }

        void Start()
        {

        }

        /// <summary>设置语言到全局</summary>
        public void SetToGlobal()
        {
            LocalizationSetManager.Instance.defaultLanguage = _language;
            LocalizationSetManager.Instance.defaultLangProvider = _langProvider;
        }

        /// <summary>设置语言类型到全局</summary>
        public void SetToGlobal(string lang)
        {
            _language = lang;
            LocalizationSetManager.Instance.defaultLanguage = _language;
        }

        /// <summary>设置语言提供者到全局</summary>
        public void SetToGlobal(LanguageProviderComponentBase langProvider)
        {
            _langProvider = langProvider;
            LocalizationSetManager.Instance.defaultLangProvider = _langProvider;
        }

        /// <summary>设置语言到本地化</summary>
        public void SetToLocal()
        {
            if (_localizationSet == null) return;

            foreach (var localization in _localizationSet.localizations)
            {
                if (ObjectUtility.IsNull(localization)) continue;
                localization.defaultLanguage = _language;
                localization.defaultProvider = _langProvider;
            }
        }

        /// <summary>设置语言类型到本地化</summary>
        public void SetToLocal(string lang)
        {
            _language = lang;

            if (_localizationSet == null) return;

            foreach (var localization in _localizationSet.localizations)
            {
                if (ObjectUtility.IsNull(localization)) continue;
                localization.defaultLanguage = _language;
            }
        }

        /// <summary>设置语言提供者到本地化</summary>
        public void SetToLocal(LanguageProviderComponentBase langProvider)
        {
            _langProvider = langProvider;

            if (_localizationSet == null) return;

            foreach (var localization in _localizationSet.localizations)
            {
                if (ObjectUtility.IsNull(localization)) continue;
                localization.defaultProvider = _langProvider;
            }
        }
    }
}
