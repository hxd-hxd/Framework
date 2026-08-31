using System.Collections;
using System.Collections.Generic;
using Framework.Localization;
using UnityEngine;

namespace Framework.LocalizationSimple
{
    /// <summary>本地化设置管理器组件</summary>
    public class LocalizationSetManagerComp : MonoSingleton<LocalizationSetManagerComp>
    {
        [SerializeField]
        private LocalizationSetBase _defultLocalizationSet;

        [SerializeField]
        private List<LanguageProviderComponentBase> _defultLangProviders;

        /// <summary>全局默认的本地化设置</summary>
        public LocalizationSetBase defultLocalizationSet { get => _defultLocalizationSet; set => _defultLocalizationSet = value; }

        /// <summary>语言提供者列表</summary>
        public List<LanguageProviderComponentBase> defultLangProviders { get => _defultLangProviders; set => _defultLangProviders = value; }


    }
}