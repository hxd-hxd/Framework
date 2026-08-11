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
            if (language != null && language.GetType() == typeof(LangType))
            {
                _lang = (LangType)(object)language;
            }
        }

        public T GetLanguage<T>()
        {
            //if (typeof(T) == typeof(LangType))
            //{
            //    return (T)(object)_lang;
            //}
            //else
            //{
            //    throw new System.InvalidCastException($"无法将 {typeof(LangType)} 转换为 {typeof(T)}");
            //}
            var obj = (object)_lang;
            if (obj is T t)
            {
                return t;
            }
            else
            {
                throw new System.InvalidCastException($"无法将 {typeof(LangType)} 转换为 {typeof(T)}");
            }
            //return default;
        }

        public bool TryGetLanguage<T>(out T language)
        {
            language = default;

            bool r = false;
            //if (typeof(T) == typeof(LangType))
            //{
            //    language = (T)(object)_lang;
            //    r = true;
            //}
            var obj = (object)_lang;
            if (obj is T t)
            {
                language = t;
                r = true;
            }

            return r;
        }

        public bool IsLanguage<T>(T language)
        {
            bool r = false;
            if (typeof(T) == typeof(LangType))
            {
                var l = (LangType)(object)language;
                r = l == _lang;
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