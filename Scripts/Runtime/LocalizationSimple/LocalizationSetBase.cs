using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Localization;
using UnityEngine;
using UnityEngine.Events;

namespace Framework.LocalizationSimple
{
    /// <summary>设置本地化语言基类</summary>
    public abstract class LocalizationSetBase : MonoBehaviour, ILocalizationSet
    {
        public LocalizationSetMode _setMode;
        //[SerializeField]
        //private bool _isSendSetEventInform;
        public List<ILocalization> _localizations = new List<ILocalization>();
        public List<LanguageProviderComponentBase> _langProviders;

        //private UnityEvent<LocalizationCurLangSetInfo> _onSetInfromEvent;

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

        ///// <summary>是否发送设置通知</summary>
        //public bool isSendSetEventInform { get => _isSendSetEventInform; set => _isSendSetEventInform = value; }

        protected virtual void Awake()
        {
            GetComponentsInChildren(_localizations);
        }

        protected virtual void OnEnable()
        {
            Set();

            //LocalizationCurLanguage.Instance.onSetEvent += OnSetEvent;
        }

        protected virtual void OnDisable()
        {
            //LocalizationCurLanguage.Instance.onSetEvent -= OnSetEvent;
        }

        /// <summary>获取所有语言</summary>
        public abstract void GetAllLangType(ref List<object> list);

        /// <summary>将语言类型转换成字符串形式</summary>
        public abstract string LangTypeToString(object lang);

        /// <summary>获取当前语言</summary>
        public abstract object GetCurrentLang();

        /// <summary>设置</summary>
        public abstract void Set();

        /// <summary>按指定语言设置</summary>
        public virtual void Set(object lang)
        {
            Set(lang, _setMode);
        }

        /// <summary>按指定语言和设置方式设置</summary>
        public virtual void Set(object lang, LocalizationSetMode setMode)
        {
            switch (setMode)
            {
                case LocalizationSetMode.Type:
                    string type = LangTypeToString(lang);
                    foreach (var localization in localizations)
                    {
                        localization.SetLanguage(type);
                    }
                    //if (_isSendSetEventInform)
                    //{
                    //    var setInfo = new LocalizationCurLangSetInfo
                    //    {
                    //        setter = this,
                    //        setMode = _setMode,
                    //        oldLang = LangTypeToString(GetCurrentLang()),
                    //        newLang = type,
                    //    };
                    //    LocalizationCurLanguage.Instance.SendSetInform(setInfo);
                    //}
                    break;
                case LocalizationSetMode.Provider:
                    var langProviders = _langProviders;
                    if (Application.isPlaying)
                    {
                        if (langProviders == null || langProviders.Count == 0) 
                            langProviders = LocalizationSetManagerComp.Instance.defultLangProviders;
                    }
                    var provider = langProviders.Find(d =>
                    {
                        return d != null && d.IsLanguage(lang);
                    });
                    foreach (var localization in localizations)
                    {
                        localization.SetLanguage(provider);
                    }
                    //if (_isSendSetEventInform)
                    //{
                    //    var setInfo = new LocalizationCurLangSetInfo
                    //    {
                    //        setter = this,
                    //        setMode = _setMode,
                    //        oldLang = GetCurrentLang(),
                    //        newLang = lang,
                    //    };
                    //    LocalizationCurLanguage.Instance.SendSetInform(setInfo);
                    //}
                    break;
                default:
                    break;
            }
        }

        ///// <summary>处理语言设置事件</summary>
        //protected virtual void OnSetEvent(LocalizationCurLangSetInfo info)
        //{
        //    // 这里不需要设置语言，也不需要做其他的
        //}
    }
}