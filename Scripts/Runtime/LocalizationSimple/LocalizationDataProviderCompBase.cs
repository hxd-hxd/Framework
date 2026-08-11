using System.Collections;
using UnityEngine;
using Framework.Localization;
using System.Collections.Generic;

namespace Framework.LocalizationSimple
{
    /// <summary>本地化 数据提供者组件基类</summary>
    public abstract class LocalizationDataProviderCompBase : MonoBehaviour, ILocalizationDataProvider
    {
        public abstract Data GetData<Data>(string id, string language) where Data : ILocalizationData;
        public abstract Data GetData<Data>(string id, ILanguageProvider languageProvider) where Data : ILocalizationData;
        public abstract List<Data> GetDatasById<Data>(string id) where Data : ILocalizationData;
        public abstract List<Data> GetDatasByLang<Data>(string language) where Data : ILocalizationData;
        public abstract List<Data> GetDatasByLang<Data>(ILanguageProvider languageProvider) where Data : ILocalizationData;
        public abstract bool TryGetData<Data>(string id, string language, out Data data) where Data : ILocalizationData;
        public abstract bool TryGetData<Data>(string id, ILanguageProvider languageProvider, out Data data) where Data : ILocalizationData;
        public abstract bool TryGetDatasById<Data>(string id, ref List<Data> data) where Data : ILocalizationData;
        public abstract bool TryGetDatasByLang<Data>(string language, ref List<Data> data) where Data : ILocalizationData;
        public abstract bool TryGetDatasByLang<Data>(ILanguageProvider languageProvider, ref List<Data> data) where Data : ILocalizationData;
    }
}