using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.LocalizationSimple
{
    using LocalizationData = LocalizationDataSprite;

    /// <summary>本地化项</summary>
    [Serializable]
    public class LocalizationItemImage : LocalizationItemBase<LocalizationData>
    {
        public Image _item;
        [SerializeField]
        private List<LocalizationData> _datas = new();

        public override List<LocalizationData> datas { get => _datas; set => _datas = value; }

        protected override void Execute(LocalizationData data)
        {
            if (data != null && data._sprite != null)
            {
                _item.sprite = data._sprite;
            }
        }

        public override void SetLanguage(string language)
        {
            if (_item)
            {
                base.SetLanguage(language);
            }
        }

        public override void SetLanguage<T>(T languageProvider)
        {
            if (_item)
            {
                base.SetLanguage(languageProvider);
            }
        }
    }
}
