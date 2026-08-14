using System.Collections;
using System.Collections.Generic;
using Framework.Localization;

namespace Framework.LocalizationSimple
{
    /// <summary>本地化当前语言设置信息</summary>
    public struct LocalizationCurLangSetInfo
    {
        /// <summary>设置方式</summary>
        public LocalizationSetMode setMode;

        /// <summary>旧的语言</summary>
        public object oldLang;

        /// <summary>新的语言</summary>
        public object newLang;

        /// <summary>语言设置者，谁设置的语言</summary>
        public object setter;
    }
}
