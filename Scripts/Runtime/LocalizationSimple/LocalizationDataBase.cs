using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Localization;

namespace Framework.LocalizationSimple
{
    /// <summary>本地化数据基类</summary>
    [Serializable]
    public abstract class LocalizationDataBase : ILocalizationData
    {
        public string _id;

        public string _language;

        public LanguageProviderComponentBase _langProvider;

        public virtual string id { get => _id; set => _id = value; }

        public virtual string language { get => _language; set => _language = value; }

        public virtual ILanguageProvider langProvider { get => _langProvider; set => _langProvider = value as LanguageProviderComponentBase; }

        public virtual T GetData<T>()
        {
            throw new NotImplementedException();
        }

        public virtual void SetData<T>(T data)
        {
            throw new NotImplementedException();
        }
    }
}