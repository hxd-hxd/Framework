using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.LocalizationSimple
{
    using LocalizationData = LocalizationDataString;

    /// <summary>文本本地化项，逻辑功能导向的</summary>
    [Serializable]
    public class LocalizationItemText : LocalizationItemBase<LocalizationDataString>
    {
        public Text _item;
        [SerializeField]
        [LocalizationDataCfg(LocalizationDataCfgMode.OnlyLang)]
        private List<LocalizationDataString> _datas = new List<LocalizationDataString>();

        public override List<LocalizationDataString> datas { get => _datas; set => _datas = value; }

        protected override void Execute(LocalizationDataString data)
        {
            if (_item == null) return;
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

        public override void SetLanguage(ILanguageProvider languageProvider)
        {
            if (_item)
            {
                base.SetLanguage(languageProvider);
            }
        }
    }
}