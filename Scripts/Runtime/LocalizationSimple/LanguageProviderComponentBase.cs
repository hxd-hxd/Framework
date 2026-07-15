using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Localization;

namespace Framework.LocalizationSimple
{
    /// <summary>本地化语言提供者组件基类</summary>
    public abstract class LanguageProviderComponentBase : MonoBehaviour, ILanguageProvider
    {
        public abstract void SetLanguage<T>(T language);

        public abstract T GetLanguage<T>();

        public abstract bool TryGetLanguage<T>(out T language);

        public abstract bool IsLanguage<T>(T language);

        public abstract bool IsProviderLanguage<T>(T languageProvider) where T : ILanguageProvider;
    }
}