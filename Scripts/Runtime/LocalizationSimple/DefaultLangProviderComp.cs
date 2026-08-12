using System.Collections;
using System.Collections.Generic;
using Framework.Localization;
using Framework.LocalizationSimple;
using LangType = Framework.Localization.Language;

namespace Framework.LocalizationSimple
{
    /// <summary>本地化语言提供者组件</summary>
    public class DefaultLangProviderComp : LanguageProviderComponentBase
    {
        public LangType _lang;

        private DefaultLangProvider _provider = new DefaultLangProvider();

        public override void SetLanguage<T>(T language)
        {
            _provider.SetLanguage(language);
            _lang = _provider._lang;
        }

        public override T GetLanguage<T>()
        {
            _provider._lang = _lang;
            return _provider.GetLanguage<T>();
        }

        public override bool TryGetLanguage<T>(out T language)
        {
            _provider._lang = _lang;
            return _provider.TryGetLanguage(out language);
        }

        public override bool IsLanguage<T>(T language)
        {
            _provider._lang = _lang;
            return _provider.IsLanguage(language);
        }

        public override bool IsProviderLanguage(ILanguageProvider languageProvider)
        {
            _provider._lang = _lang;
            return _provider.IsProviderLanguage(languageProvider);
        }
    }
}