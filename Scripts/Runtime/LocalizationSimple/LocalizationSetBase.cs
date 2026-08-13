using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Localization;

namespace Framework.LocalizationSimple
{
    /// <summary>设置本地化语言基类</summary>
    public abstract class LocalizationSetBase : MonoBehaviour, ILocalizationSet
    {
        public LocalizationSetMode _setMode;
        public List<ILocalization> _localizations = new List<ILocalization>();
        public List<LanguageProviderComponentBase> _langProviders;

        public virtual List<ILocalization> localizations
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

        /// <summary>获取所有语言</summary>
        public abstract void GetAllLangType(ref List<object> list);

        /// <summary>将语言类型转换成字符串形式</summary>
        public abstract string LangTypeToString(object lang);

        /// <summary>设置</summary>
        public abstract void Set();

        /// <summary>按指定语言设置</summary>
        public virtual void Set(object lang)
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
                    }
                    break;
                default:
                    break;
            }
        }

    }
}