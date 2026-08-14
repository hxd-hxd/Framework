using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Localization;
using UnityEngine;

namespace Framework.LocalizationSimple
{
    /// <summary>本地化组件基类</summary>
    public abstract class LocalizationCompBase : MonoBehaviour, ILocalization
    {
        [Header("语言类型")]
        public string _currentLanguage;
        public string _defaultLanguage;

        [Header("语言类型提供者")]
        public LanguageProviderComponentBase _currentProvider;
        public LanguageProviderComponentBase _defaultProvider;

        public string currentLanguage => _currentLanguage;

        public ILanguageProvider currentProvider => _currentProvider;

        public string defaultLanguage { get => _defaultLanguage; set => _defaultLanguage = value; }

        public ILanguageProvider defaultProvider { get => _defaultProvider; set => _defaultProvider = value as LanguageProviderComponentBase; }

        public abstract void GetAllItem(ref List<ILocalizationItem> items);

        protected virtual void Start()
        {

        }

        /// <summary>获取可用的默认语言</summary>
        public virtual string GetUsableDefaultLang()
        {
            // 优先本地
            if (!string.IsNullOrEmpty(_defaultLanguage)) return _defaultLanguage;
            // 没有则用全局
            return LocalizationSetManager.Instance.defaultLanguage;
        }

        /// <summary>获取可用的默认语言提供者</summary>
        public virtual ILanguageProvider GetUsableDefaultLangProvider()
        {
            // 优先本地
            if (!ObjectUtility.IsNull(_defaultProvider)) return _defaultProvider;
            // 没有则用全局
            return LocalizationSetManager.Instance.defaultLangProvider;
        }

        public virtual void SetLanguage(string language)
        {
            _currentLanguage = language;
        }

        public virtual void SetLanguage(ILanguageProvider languageProvider)
        {
            _currentProvider = languageProvider as LanguageProviderComponentBase;
        }

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
            var def = GetUsableDefaultLang();
            if (items != null && items.Count > 0)
                foreach (var item in items)
                {
                    SetItemLanguage(item, cur, def);
                }
        }

        protected void SetLanguageInternal<T, Data>(List<T> items, ILanguageProvider cur) where T : LocalizationItemBase<Data> where Data : LocalizationDataBase
        {
            var def = GetUsableDefaultLangProvider();
            if (items != null && items.Count > 0)
                foreach (var item in items)
                {
                    SetItemLanguage(item, cur, def);
                }
        }


        protected void AddItemTo<Item>(List<Item> srcItems, List<ILocalizationItem> destItems) where Item : ILocalizationItem
        {
            if (srcItems == null || srcItems.Count <= 0) return;
            foreach (var srcItem in srcItems)
            {
                if (srcItem != null) destItems.Add(srcItem);
            }
        }
    }
}