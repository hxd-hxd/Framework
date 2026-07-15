using System.Collections;
using System.Collections.Generic;

namespace Framework.Localization
{
    /// <summary>本地化接口</summary>
    public interface ILocalization
    {
        ///// <summary>获取或设置语言提供者</summary>
        //ILanguageProvider LanguageProvider { get; set; }

        /// <summary>设置语言</summary>
        void SetLanguage(string language);

        /// <summary>设置为语言提供者提供的语言</summary>
        void SetLanguage<T>(T languageProvider) where T : ILanguageProvider;
    }
}