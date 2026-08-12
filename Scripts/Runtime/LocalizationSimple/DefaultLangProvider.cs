using System;
using Framework.Localization;
using LangType = Framework.Localization.Language;

namespace Framework.LocalizationSimple
{
    /// <summary>本地化语言提供者组件</summary>
    [Serializable]
    public class DefaultLangProvider : ILanguageProvider
    {
        public LangType _lang;

        public void SetLanguage<T>(T language)
        {
            if (language is LangType t)
            {
                _lang = t;
            }
            else
            {
                throw new System.InvalidCastException($"无法将 {(language == null ? typeof(T) : language.GetType())} 转换为 {typeof(LangType)}");
            }
        }

        public T GetLanguage<T>()
        {
            if (_lang is T t)
            {
                return t;
            }
            else
            {
                throw new System.InvalidCastException($"无法将 {typeof(LangType)} 转换为 {typeof(T)}");
            }
        }

        public bool TryGetLanguage<T>(out T language)
        {
            language = default;

            bool r = false;
            if (_lang is T t)
            {
                language = t;
                r = true;
            }

            return r;
        }

        public bool IsLanguage<T>(T language)
        {
            bool r = false;
            if (language is LangType t)
            {
                r = t == _lang;
            }

            return r;
        }

        public bool IsProviderLanguage(ILanguageProvider languageProvider)
        {
            if (languageProvider == null) return false;

            if (languageProvider.TryGetLanguage(out LangType language))
            {
                return language == _lang;
            }

            return false;
        }
    }
}