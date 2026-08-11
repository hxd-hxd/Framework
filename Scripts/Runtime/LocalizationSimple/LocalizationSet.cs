using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Localization;
using LangType = Framework.Localization.Language;

namespace Framework.LocalizationSimple
{
    /// <summary>设置本地化语言</summary>
    public class LocalizationSet : MonoBehaviour, ILocalizationSet
    {
        public LocalizationSetMode _setMode;
        public List<ILocalization> _localizations = new List<ILocalization>();
        public List<LanguageProviderComponentBase> _langProviders;

        protected virtual List<ILocalization> localizations
        {
            get
            {
                if (_localizations == null)
                {
                    _localizations = new List<ILocalization>();
                }
                if (_localizations.Count <= 0)
                {
                    GetComponentsInChildren(_localizations);
                }
                return _localizations;
            }
        }

        protected virtual void Awake()
        {
            GetComponentsInChildren(_localizations);
        }

        protected virtual void OnEnable()
        {
            Set();
        }

        /// <summary>设置</summary>
        public virtual void Set()
        {
            Set(LocalizationCurLanguage.Instance.curLanguage);
        }

        /// <summary>设置</summary>
        public virtual void Set(LangType lang)
        {
            switch (_setMode)
            {
                case LocalizationSetMode.Type:
                    string type = LangTypeToString(lang);
                    foreach (var localization in localizations)
                    {
                        localization.SetLanguage(type);
                    }
                    break;
                case LocalizationSetMode.Provider:
                    var provider = _langProviders.Find(d =>
                    {
                        return d != null && d.IsLanguage(lang);
                    });
                    foreach (var localization in localizations)
                    {
                        localization.SetLanguage(provider);
                        //provider.SetLanguage(localization);
                    }
                    break;
                default:
                    break;
            }
        }

        public virtual string LangTypeToString(LangType lang)
        {
            return lang switch
            {
                LangType.ChineseSimplified => "汉语",
                LangType.ChineseTraditional => "汉语-繁体",
                LangType.English => "英文",
                _ => lang.ToString()
            };
        }
    }
}