using System.Collections;
using System.Collections.Generic;
using Framework.Localization;

namespace Framework.LocalizationSimple
{
    /// <summary>本地化设置当前语言</summary>
    public class LocalizationCurLanguage : Singleton<LocalizationCurLanguage>
    {
        private Language _curLanguage = Language.ChineseSimplified;

        /// <summary>当前语言</summary>
        public Language curLanguage { get => _curLanguage; set => _curLanguage = value; }
    }
}
