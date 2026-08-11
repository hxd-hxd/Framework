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
        public LocalizationDataGetMode _dataMode;

        public string _id;

        public LocalizationDataProviderCompBase _dataProvider;

        public virtual string id { get => _id; set => _id = value; }

        /// <summary>数据获取方式</summary>
        public virtual LocalizationDataGetMode dataMode { get => _dataMode; set => _dataMode = value; }

        /// <summary>数据</summary>
        public abstract List<Data> datas { get; set; }

        /// <summary>执行本地化操作</summary>
        protected abstract void Execute(Data data);

        public virtual Data GetData(string language)
        {
            Data data = default;
            TryGetData(language, out data);
            return data;
        }

        public virtual bool TryGetData(string language, out Data data)
        {
            bool r = false;
            data = default;
            if (_dataMode == LocalizationDataGetMode.Provider)
            {
                if (!ObjectUtility.IsNull(_dataProvider))
                    r = _dataProvider.TryGetData(_id, language, out data);
            }
            else if (datas != null && datas.Count > 0)
            {
                data = datas.Find(d => d._language == language);
                r = data != null;
            }
            return r;
        }

        public virtual Data GetData(ILanguageProvider languageProvider)
        {
            Data data = default;
            TryGetData(languageProvider, out data);
            return data;
        }

        public virtual bool TryGetData(ILanguageProvider languageProvider, out Data data)
        {
            bool r = false;
            data = default;
            if (ObjectUtility.IsNull(languageProvider))
                return false;

            if (_dataMode == LocalizationDataGetMode.Provider)
            {
                if (!ObjectUtility.IsNull(_dataProvider))
                    r = _dataProvider.TryGetData(_id, languageProvider, out data);
            }
            else if (datas != null && datas.Count > 0)
            {
                data = datas.Find(d =>
                    !ObjectUtility.IsNull(d._langProvider) && d._langProvider.IsProviderLanguage(languageProvider));
                r = data != null;
            }
            return r;
        }

        /// <summary>按当前数据获取方式取出该 id 下的全部数据</summary>
        public virtual bool TryGetAllDatas(out List<Data> result)
        {
            result = null;
            if (_dataMode == LocalizationDataGetMode.Provider)
            {
                if (ObjectUtility.IsNull(_dataProvider)) return false;
                return _dataProvider.TryGetDatasById(_id, ref result);
            }

            result = datas;
            return result != null && result.Count > 0;
        }

        public virtual void SetLanguage(string language)
        {
            var data = GetData(language);
            Execute(data);
        }

        public virtual void SetLanguage(ILanguageProvider languageProvider)
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
