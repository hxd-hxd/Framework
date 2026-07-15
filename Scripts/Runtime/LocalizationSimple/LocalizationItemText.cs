using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.LocalizationSimple
{
    using LocalizationData = LocalizationDataString;

    /// <summary>本地化项</summary>
    [Serializable]
    public class LocalizationItemText : LocalizationItemBase<LocalizationData>
    {
        public Text _item;
        [SerializeField]
        private List<LocalizationData> _datas = new();

        public override List<LocalizationData> datas { get => _datas; set => _datas = value; }

        protected override void Execute(LocalizationData data)
        {
            if (data != null && data._text != null)
            {
                _item.text = data._text;
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