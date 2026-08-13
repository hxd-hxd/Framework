using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Localization;
using UnityEngine;
using UnityEngine.Events;

namespace Framework.LocalizationSimple
{
    using LocalizationData = LocalizationDataString;

    /// <summary>精灵本地化项，数据导向的</summary>
    [Serializable]
    public class LocalizationItemString : LocalizationItemBase<LocalizationDataString>
    {
        [SerializeField]
        [LocalizationDataCfg(LocalizationDataCfgMode.OnlyLang)]
        private List<LocalizationDataString> _datas = new List<LocalizationDataString>();

        [SerializeField]
        private UnityEvent<string> _onExecute;

        public override List<LocalizationDataString> datas { get => _datas; set => _datas = value; }

        public UnityEvent<string> onExecute { get => _onExecute; set => _onExecute = value; }

        protected override void Execute(LocalizationDataString data)
        {
            if (data != null && data._text != null)
            {
                _onExecute?.Invoke(data._text);
            }
        }
    }
}
