using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Localization;
using UnityEngine;
using UnityEngine.Events;

namespace Framework.LocalizationSimple
{
    using LocalizationData = LocalizationDataSprite;

    /// <summary>精灵本地化项，数据导向的</summary>
    [Serializable]
    public class LocalizationItemSprite : LocalizationItemBase<LocalizationData>
    {
        [SerializeField]
        [LocalizationDataCfg(LocalizationDataCfgMode.OnlyLang)]
        private List<LocalizationData> _datas = new List<LocalizationData>();

        [SerializeField]
        private UnityEvent<Sprite> _onExecute;

        public override List<LocalizationData> datas { get => _datas; set => _datas = value; }

        public UnityEvent<Sprite> onExecute { get => _onExecute; set => _onExecute = value; }

        protected override void Execute(LocalizationData data)
        {
            if (data != null && data._sprite != null)
            {
                _onExecute?.Invoke(data._sprite);
            }
        }
    }
}
