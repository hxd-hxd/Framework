using System.Collections;
using System.Collections.Generic;

namespace Framework.Localization
{
    /// <summary>本地化数据提供者接口</summary>
    public interface ILocalizationDataProvider
    {
        /// <summary>获取对应 id 的所有数据</summary>
        List<Data> GetDatasById<Data>(string id) where Data : ILocalizationData;

        /// <summary>尝试获取对应 id 的所有数据</summary>
        bool TryGetDatasById<Data>(string id, ref List<Data> data) where Data : ILocalizationData;

        /// <summary>获取对应 语言 的所有数据</summary>
        List<Data> GetDatasByLang<Data>(string language) where Data : ILocalizationData;

        /// <summary>尝试获取对应 语言 的所有数据</summary>
        bool TryGetDatasByLang<Data>(string language, ref List<Data> data) where Data : ILocalizationData;

        /// <summary>获取对应 语言提供者 的所有数据</summary>
        List<Data> GetDatasByProvider<Data>(ILanguageProvider languageProvider) where Data : ILocalizationData;

        /// <summary>尝试获取对应 语言提供者 的所有数据</summary>
        bool TryGetDatasByProvider<Data>(ILanguageProvider languageProvider, ref List<Data> data) where Data : ILocalizationData;

        /// <summary>获取对应 id 和 语言 的数据</summary>
        Data GetData<Data>(string id, string language) where Data : ILocalizationData;

        /// <summary>尝试获取对应 id 和 语言 的数据</summary>
        bool TryGetData<Data>(string id, string language, out Data data) where Data : ILocalizationData;

        Data GetData<Data>(string id, ILanguageProvider languageProvider) where Data : ILocalizationData;

        bool TryGetData<Data>(string id, ILanguageProvider languageProvider, out Data data) where Data : ILocalizationData;

    }
}