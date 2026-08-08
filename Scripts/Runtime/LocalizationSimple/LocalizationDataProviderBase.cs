using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Localization;

namespace Framework.LocalizationSimple
{
    /// <summary>本地化项基类</summary>
    [Serializable]
    public abstract class LocalizationDataProviderBase : ILocalizationDataProvider
    {
        public abstract List<Data> GetDatasById<Data>(string id) where Data : ILocalizationData;

        public abstract bool TryGetDatasById<Data>(string id, ref List<Data> data) where Data : ILocalizationData;

        public abstract List<Data> GetDatasByLang<Data>(string language) where Data : ILocalizationData;

        public abstract bool TryGetDatasByLang<Data>(string language, ref List<Data> data) where Data : ILocalizationData;

        public abstract List<Data> GetDatasByProvider<Data>(ILanguageProvider languageProvider) where Data : ILocalizationData;

        public abstract bool TryGetDatasByProvider<Data>(ILanguageProvider languageProvider, ref List<Data> data) where Data : ILocalizationData;

        public abstract Data GetData<Data>(string id, string language) where Data : ILocalizationData;

        public abstract bool TryGetData<Data>(string id, string language, out Data data) where Data : ILocalizationData;

        public abstract Data GetData<Data>(string id, ILanguageProvider languageProvider) where Data : ILocalizationData;

        public abstract bool TryGetData<Data>(string id, ILanguageProvider languageProvider, out Data data) where Data : ILocalizationData;

    }
}
