using System.Collections;
using System.Collections.Generic;

namespace Framework.Localization
{
    /// <summary>本地化接口</summary>
    public interface ILocalization
    {
        /// <summary>获取当前语言language</summary>
        string currentLanguage { get; }

        /// <summary>获取当前语言提供者</summary>
        ILanguageProvider currentProvider { get; }

        /// <summary>默认语言类型</summary>
        string defaultLanguage { get; set; }

        /// <summary>默认语言提供者</summary>
        ILanguageProvider defaultProvider { get; set; }

        /// <summary>设置语言，当没有目标语言时会尝试使用默认语言 <see cref="defaultLanguage"/>
        /// <para></para><paramref name="language"/>：目标语言
        /// </summary>
        void SetLanguage(string language);

        /// <summary>设置为语言提供者提供的语言，当没有目标语言时会尝试使用默认语言提供者 <see cref="defaultProvider"/>
        /// <para></para><paramref name="languageProvider"/>：目标语言
        /// </summary>
        void SetLanguage<T>(T languageProvider) where T : ILanguageProvider;
    }
}