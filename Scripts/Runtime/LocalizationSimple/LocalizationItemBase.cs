using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Localization;

namespace Framework.LocalizationSimple
{
    /// <summary>本地化项基类</summary>
    [Serializable]
    public abstract class LocalizationItemBase<Data> where Data : LocalizationDataBase
    {
        /// <summary>数据</summary>
        public abstract List<Data> datas { get; set; }

        /// <summary>执行本地化操作</summary>
        protected abstract void Execute(Data data);

        public virtual Data GetData(string language)
        {
            Data data = default;
            if (datas != null && datas.Count > 0)
            {
                data = datas.Find(d => d._language == language);
            }
            return data;
        }

        public virtual bool TryGetData(string language, out Data data)
        {
            bool r = false;
            data = default;
            if (datas != null && datas.Count > 0)
            {
                data = datas.Find(d => d._language == language);
                r = data != null;
            }
            return r;
        }

        public virtual Data GetData<T>(T languageProvider) where T : ILanguageProvider
        {
            Data data = default;
            //if (!ObjectUtility.IsNull(languageProvider as object))
            if (languageProvider == null)
            {
                if (datas != null && datas.Count > 0)
                {
                    data = datas.Find(d =>
                    {
                        //return !ObjectUtility.IsNull(d._langProvider) && d._langProvider.IsProviderLanguage(languageProvider);
                        return d._langProvider != null && d._langProvider.IsProviderLanguage(languageProvider);
                    });
                }
            }
            return data;
        }

        public virtual bool TryGetData<T>(T languageProvider, out Data data) where T : ILanguageProvider
        {
            bool r = false;
            data = default;
            if (languageProvider == null) return r;
            if (datas != null && datas.Count > 0)
            {
                data = datas.Find(d =>
                {
                    return d._langProvider != null && d._langProvider.IsProviderLanguage(languageProvider);
                });
                r = data != null;
            }
            return r;
        }

        public virtual void SetLanguage(string language)
        {
            var data = GetData(language);
            Execute(data);
        }

        public virtual void SetLanguage<T>(T languageProvider) where T : ILanguageProvider
        {
            var data = GetData(languageProvider);
            Execute(data);
        }

        /// <summary>直接使用数据设置语言</summary>
        public virtual void SetLanguage(Data data)
        {
            Execute(data);
        }

    }
}
