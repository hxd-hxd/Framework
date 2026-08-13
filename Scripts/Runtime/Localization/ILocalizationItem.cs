using System.Collections.Generic;

namespace Framework.Localization
{
    /// <summary>本地化数据接口</summary>
    public interface ILocalizationItem
    {
        /// <summary>数据唯一标识符</summary>
        string dataId { get; set; }

        /// <summary>数据提供者</summary>
        public ILocalizationDataProvider dataProvider { get; set; }

        /// <summary>数据</summary>
        List<ILocalizationData> datas { get; set; }

        /// <summary>根据语言类型设置语言</summary>
        void SetLanguage(string language);

        /// <summary>根据语言提供者设置语言</summary>
        void SetLanguage(ILanguageProvider languageProvider);

        /// <summary>直接使用数据设置语言</summary>
        void SetLanguage(ILocalizationData data);
    }
}