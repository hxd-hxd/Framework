using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Localization;
using UnityEngine;

namespace Framework.LocalizationSimple
{
    /// <summary>本地化基类</summary>
    public abstract class LocalizationBase : MonoBehaviour, ILocalization
    {
        [Header("语言类型")]
        public string _currentLanguage = "0";
        public string _defaultLanguage = "0";

        [Header("语言类型提供者")]
        public LanguageProviderComponentBase _currentProvider;
        public LanguageProviderComponentBase _defaultProvider;

        string ILocalization.currentLanguage => _currentLanguage;
        //public abstract string currentLanguage { get; }

        ILanguageProvider ILocalization.currentProvider => _currentProvider;

        string ILocalization.defaultLanguage { get => _defaultLanguage; set => _defaultLanguage = value; }

        ILanguageProvider ILocalization.defaultProvider { get => _defaultProvider; set => _defaultProvider = value as LanguageProviderComponentBase; }

        protected virtual void Start()
        {
            //SetLanguage(_language);
        }

        public abstract void SetLanguage(string language);

        public abstract void SetLanguage<T>(T languageProvider) where T : ILanguageProvider;

        protected void SetItemLanguage<Data>(LocalizationItemBase<Data> item, string cur, string def) where Data : LocalizationDataBase
        {
            if (item.TryGetData(cur, out var data) || (cur != def && item.TryGetData(def, out data))) item.SetLanguage(data);
        }

        protected void SetItemLanguage<Data>(LocalizationItemBase<Data> item, ILanguageProvider cur, ILanguageProvider def) where Data : LocalizationDataBase
        {
            if (item.TryGetData(cur, out var data) || (cur != def && !cur.IsProviderLanguage(def) && item.TryGetData(def, out data))) item.SetLanguage(data);
        }

        protected void SetLanguageInternal<T, Data>(List<T> items, string cur) where T : LocalizationItemBase<Data> where Data : LocalizationDataBase
        {
            //_currentLanguage = cur;
            var def = _defaultLanguage;
            if (items != null && items.Count > 0)
                foreach (var item in items)
                {
                    //item.SetLanguage(language);
                    SetItemLanguage(item, cur, def);
                }
        }

        protected void SetLanguageInternal<T, Data>(List<T> items, ILanguageProvider cur) where T : LocalizationItemBase<Data> where Data : LocalizationDataBase
        {
            //var langProvider = cur as LanguageProviderComponentBase;
            //if (langProvider == null)
            //{
            //    return;
            //}
            //_currentProvider = langProvider;
            var def = _defaultProvider;
            if (items != null && items.Count > 0)
                foreach (var item in items)
                {
                    //item.SetLanguage(language);
                    SetItemLanguage(item, cur, def);
                }
        }
    }
}