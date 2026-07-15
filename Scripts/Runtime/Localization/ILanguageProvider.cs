using System.Collections;
using System.Collections.Generic;

namespace Framework.Localization
{
    /// <summary>本地化语言提供者接口</summary>
    public interface ILanguageProvider
    {
        /// <summary>设置语言</summary>
        void SetLanguage<T>(T language);

        /// <summary>获取语言</summary>
        T GetLanguage<T>();

        /// <summary>尝试获取语言</summary>
        bool TryGetLanguage<T>(out T language);

        /// <summary>是否为指定语言</summary>
        bool IsLanguage<T>(T language);

        /// <summary>是否提供相同语言</summary>
        bool IsProviderLanguage<T>(T languageProvider) where T: ILanguageProvider;
    }
}