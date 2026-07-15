using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Localization;

namespace Framework.LocalizationSimple
{
    /// <summary>默认本地化</summary>
    public class DefaultLocalization : MonoBehaviour, ILocalization
    {
        [Header("语言类型")]
        public string _language = "0";

        [Header("语言类型提供者")]
        public LanguageProviderComponentBase _langProvider;

        [Header("文本")]
        public List<LocalizationItemText> _itemsText;

        [Header("图片")]
        public List<LocalizationItemImage> _itemsImage;

        [Header("游戏对象")]
        public List<LocalizationItemGameObject> _itemsGameObject;

        //ILanguageProvider ILocalization.LanguageProvider { get => null; set => _ = value; }

        void Start()
        {
            //SetLanguage(_language);
        }

        public void SetLanguage(string language)
        {
            _language = language;

            //SetLanguage<LocalizationItemText, LocalizationDataString>(_itemsText, language);
            if (_itemsText != null && _itemsText.Count > 0)
                foreach (var item in _itemsText)
                {
                    item.SetLanguage(language);
                }

            if (_itemsImage != null && _itemsImage.Count > 0)
                foreach (var item in _itemsImage)
                {
                    item.SetLanguage(language);
                }

            if (_itemsGameObject != null && _itemsGameObject.Count > 0)
                foreach (var item in _itemsGameObject)
                {
                    item.SetLanguage(language);
                }
        }

        // 太麻烦了
        //private void SetLanguage<T, Data>(List<T> items, string language) where T : LocalizationItemBase<Data> where Data :LocalizationDataBase
        //{
        //    if (items != null && items.Count > 0)
        //        foreach (var item in items)
        //        {
        //            item.SetLanguage(language);
        //        }
        //}

        public void SetLanguage<T>(T languageProvider) where T : ILanguageProvider
        {
            var langProvider = languageProvider as LanguageProviderComponentBase;
            if(langProvider == null)
            {
                return;
            }
            _langProvider = langProvider;

            if (_itemsText != null && _itemsText.Count > 0)
                foreach (var item in _itemsText)
                {
                    item.SetLanguage(_langProvider);
                }

            if (_itemsImage != null && _itemsImage.Count > 0)
                foreach (var item in _itemsImage)
                {
                    item.SetLanguage(_langProvider);
                }

            if (_itemsGameObject != null && _itemsGameObject.Count > 0)
                foreach (var item in _itemsGameObject)
                {
                    item.SetLanguage(languageProvider);
                }
        }
    }
}