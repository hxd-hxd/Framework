using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Localization;
using UnityEngine;

namespace Framework.LocalizationSimple
{
    /// <summary>默认本地化</summary>
    public class DefaultLocalization : LocalizationCompBase
    {
        [Header("文本")]
        public List<LocalizationItemText> _itemsText;

        [Header("图片")]
        public List<LocalizationItemImage> _itemsImage;

        [Header("游戏对象")]
        public List<LocalizationItemGameObject> _itemsGameObject;

        public override void GetAllItem(ref List<ILocalizationItem> items)
        {
            items ??= TypePool.root.GetList<ILocalizationItem>();

            AddItemTo(_itemsText, items);
            AddItemTo(_itemsImage, items);
            AddItemTo(_itemsGameObject, items);
        }

        public override void SetLanguage(string language)
        {
            base.SetLanguage(language);

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

        public override void SetLanguage(ILanguageProvider languageProvider)
        {
            base.SetLanguage(languageProvider);

            var cur = _currentProvider;
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