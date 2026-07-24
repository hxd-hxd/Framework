using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Localization;
using UnityEngine;

namespace Framework.LocalizationSimple
{
    /// <summary>默认本地化</summary>
    public class DefaultLocalization : LocalizationBase
    {
        [Header("文本")]
        public List<LocalizationItemText> _itemsText;

        [Header("图片")]
        public List<LocalizationItemImage> _itemsImage;

        [Header("游戏对象")]
        public List<LocalizationItemGameObject> _itemsGameObject;

        public override void SetLanguage(string language)
        {
            _currentLanguage = language;
            var cur = language;
            var def = _defaultLanguage;

            //SetLanguageInternal<LocalizationItemText, LocalizationDataString>(_itemsText, language);
            if (_itemsText != null && _itemsText.Count > 0)
                foreach (var item in _itemsText)
                {
                    SetItemLanguage(item, cur, def);
                }

            if (_itemsImage != null && _itemsImage.Count > 0)
                foreach (var item in _itemsImage)
                {
                    SetItemLanguage(item, cur, def);
                }

            if (_itemsGameObject != null && _itemsGameObject.Count > 0)
                foreach (var item in _itemsGameObject)
                {
                    SetItemLanguage(item, cur, def);
                }
        }

        public override void SetLanguage<T>(T languageProvider)
        {
            var langProvider = languageProvider as LanguageProviderComponentBase;
            if (langProvider == null)
            {
                return;
            }
            _currentProvider = langProvider;
            var cur = langProvider;
            var def = _defaultProvider;

            if (_itemsText != null && _itemsText.Count > 0)
                foreach (var item in _itemsText)
                {
                    SetItemLanguage(item, cur, def);
                }

            if (_itemsImage != null && _itemsImage.Count > 0)
                foreach (var item in _itemsImage)
                {
                    SetItemLanguage(item, cur, def);
                }

            if (_itemsGameObject != null && _itemsGameObject.Count > 0)
                foreach (var item in _itemsGameObject)
                {
                    SetItemLanguage(item, cur, def);
                }
        }

    }
}