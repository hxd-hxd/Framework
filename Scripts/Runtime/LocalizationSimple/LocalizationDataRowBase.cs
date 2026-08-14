using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Localization;

namespace Framework.LocalizationSimple
{
    /// <summary>本地化项基类</summary>
    [Serializable]
    public abstract class LocalizationDataRowBase : ILocalizationDataProvider
    {
        /// <summary>数据类型</summary>
        public abstract Type dataType { get; }

        /// <summary>唯一标识符</summary>
        public abstract string id { get; set; }

        public abstract List<Data> GetDatasById<Data>(string id) where Data : ILocalizationData;

        public abstract bool TryGetDatasById<Data>(string id, ref List<Data> data) where Data : ILocalizationData;

        public abstract List<Data> GetDatasByLang<Data>(string language) where Data : ILocalizationData;

        public abstract bool TryGetDatasByLang<Data>(string language, ref List<Data> data) where Data : ILocalizationData;

        public abstract List<Data> GetDatasByLang<Data>(ILanguageProvider languageProvider) where Data : ILocalizationData;

        public abstract bool TryGetDatasByLang<Data>(ILanguageProvider languageProvider, ref List<Data> data) where Data : ILocalizationData;

        public abstract Data GetData<Data>(string id, string language) where Data : ILocalizationData;

        public abstract bool TryGetData<Data>(string id, string language, out Data data) where Data : ILocalizationData;

        public abstract Data GetData<Data>(string id, ILanguageProvider languageProvider) where Data : ILocalizationData;

        public abstract bool TryGetData<Data>(string id, ILanguageProvider languageProvider, out Data data) where Data : ILocalizationData;

        /// <summary>设置对应语言的数据</summary>
        public abstract void SetData(string language, ILocalizationData data);

        /// <summary>设置对应语言提供者的数据</summary>
        public abstract void SetData(ILanguageProvider languageProvider, ILocalizationData data);

        /// <summary>移除对应语言的数据</summary>
        public abstract void RemoveData(string language);

        /// <summary>移除对应语言提供者的数据</summary>
        public abstract void RemoveData(ILanguageProvider languageProvider);
    }
}
