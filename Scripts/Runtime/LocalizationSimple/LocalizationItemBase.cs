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

        public virtual void SetLanguage(string language)
        {
            if (datas != null && datas.Count > 0)
            {
                var data = datas.Find(d => d._language == language);
                //if (data != null)
                {
                    Execute(data);
                }
            }
        }

        public virtual void SetLanguage<T>(T languageProvider) where T : ILanguageProvider
        {
            if (languageProvider != null)
            {
                if (datas != null && datas.Count > 0)
                {
                    var data = datas.Find(d =>
                    {
                        return d._langProvider != null && d._langProvider.IsProviderLanguage(languageProvider);
                    });
                    //if (data != null)
                    {
                        Execute(data);
                    }
                }
            }
        }

    }
}
