using System.Collections;
using System.Collections.Generic;
using Framework.Localization;

namespace Framework.LocalizationSimple
{
    /// <summary>默认本地化数据提供者组件，可直接编辑数据</summary>
    public class DefaultLocalizationDataProviderComp : LocalizationDataProviderCompBase
    {
        public DefaultLocalizationDataProvider _provider;

        public override Data GetData<Data>(string id, string language)
        {
            return _provider != null ? _provider.GetData<Data>(id, language) : default;
        }

        public override Data GetData<Data>(string id, ILanguageProvider languageProvider)
        {
            return _provider != null ? _provider.GetData<Data>(id, languageProvider) : default;
        }

        public override List<Data> GetDatasById<Data>(string id)
        {
            return _provider != null ? _provider.GetDatasById<Data>(id) : default;
        }

        public override List<Data> GetDatasByLang<Data>(string language)
        {
            return _provider != null ? _provider.GetDatasByLang<Data>(language) : default;
        }

        public override List<Data> GetDatasByLang<Data>(ILanguageProvider languageProvider)
        {
            return _provider != null ? _provider.GetDatasByLang<Data>(languageProvider) : default;
        }

        public override bool TryGetData<Data>(string id, string language, out Data data)
        {
            data = default;
            return _provider != null && _provider.TryGetData(id, language, out data);
        }

        public override bool TryGetData<Data>(string id, ILanguageProvider languageProvider, out Data data)
        {
            data = default;
            return _provider != null && _provider.TryGetData(id, languageProvider, out data);
        }

        public override bool TryGetDatasById<Data>(string id, ref List<Data> data)
        {
            return _provider != null && _provider.TryGetDatasById(id, ref data);
        }

        public override bool TryGetDatasByLang<Data>(string language, ref List<Data> data)
        {
            return _provider != null && _provider.TryGetDatasByLang(language, ref data);
        }

        public override bool TryGetDatasByLang<Data>(ILanguageProvider languageProvider, ref List<Data> data)
        {
            return _provider != null && _provider.TryGetDatasByLang(languageProvider, ref data);
        }
    }
}
