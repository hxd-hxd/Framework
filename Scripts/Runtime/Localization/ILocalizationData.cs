using System;
using System.Collections;
using System.Collections.Generic;

namespace Framework.Localization
{
    /// <summary>本地化数据接口</summary>
    public interface ILocalizationData
    {
        /// <summary>唯一标识</summary>
        string id { get; set; }

        /// <summary>语言</summary>
        string language { get; set; }

        /// <summary>语言提供者</summary>
        ILanguageProvider langProvider { get; set; }

        /// <summary>获取数据</summary>
        T GetData<T>();

        /// <summary>设置数据</summary>
        void SetData<T>(T data);

    }
}