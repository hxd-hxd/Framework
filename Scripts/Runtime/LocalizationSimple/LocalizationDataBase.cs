using System;
using System.Collections;
using System.Collections.Generic;

namespace Framework.LocalizationSimple
{
    /// <summary>本地化数据基类</summary>
    [Serializable]
    public abstract class LocalizationDataBase
    {
        /// <summary>语言</summary>
        public string _language;

        /// <summary>语言类型提供者</summary>
        public LanguageProviderComponentBase _langProvider;

    }
}