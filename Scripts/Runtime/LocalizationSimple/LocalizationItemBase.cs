using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Localization;

namespace Framework.LocalizationSimple
{
    /// <summary>本地化项非泛型基类，供编辑器属性绘制识别派生类型。</summary>
    [Serializable]
    public abstract class LocalizationItemBase : ILocalizationItem
    {
        public abstract string dataId { get; set; }

        public abstract ILocalizationDataProvider dataProvider { get; set; }

        List<ILocalizationData> ILocalizationItem.datas { get; set; }

        public abstract void SetLanguage(string language);

        public abstract void SetLanguage(ILanguageProvider languageProvider);

        public abstract void SetLanguage(ILocalizationData data);
    }

    /// <summary>本地化项基类</summary>
    [Serializable]
    public abstract class LocalizationItemBase<Data> : LocalizationItemBase, ILocalizationItem where Data : LocalizationDataBase
    {
        public LocalizationDataGetMode _dataMode;

        [UnityEngine.Serialization.FormerlySerializedAs("_id")]
        public string _dataId;

        public LocalizationDataProviderCompBase _dataProvider;

        /// <summary>数据获取方式</summary>
        public virtual LocalizationDataGetMode dataMode { get => _dataMode; set => _dataMode = value; }

        public override string dataId { get => _dataId; set => _dataId = value; }

        /// <summary>数据</summary>
        public abstract List<Data> datas { get; set; }

        List<ILocalizationData> ILocalizationItem.datas
        {
            get
            {
                List<ILocalizationData> dataList = null;
                if (datas != null && datas.Count > 0)
                {
                    dataList = new List<ILocalizationData>();
                    foreach (var data in datas)
                    {
                        dataList.Add(data);
                    }
                }
                return dataList;
            }
            set
            {
                datas?.Clear();
                datas ??= new List<Data>();
                foreach (var data in value)
                {
                    if (data is Data d) datas.Add(d);
                }
            }
        }

        ///// <summary>数据提供者</summary>
        //public override LocalizationDataProviderCompBase dataProvider
        //{
        //    get => _dataProvider;
        //    set => _dataProvider = value;
        //}

        public override ILocalizationDataProvider dataProvider { get => _dataProvider; set => _dataProvider = value as LocalizationDataProviderCompBase; }

        /// <summary>执行本地化操作</summary>
        protected abstract void Execute(Data data);

        /// <summary>获取可用的数据提供者</summary>
        public virtual ILocalizationDataProvider GetUsableDataProvider()
        {
            // 优先本地
            if (!ObjectUtility.IsNull(_dataProvider)) return _dataProvider;
            // 没有则用全局
            return LocalizationSetManager.Instance.globalDataProvider;
        }

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
                var _dataProvider = GetUsableDataProvider();
                if (!ObjectUtility.IsNull(_dataProvider))
                    r = _dataProvider.TryGetData(_dataId, language, out data);
            }
            else if (datas != null && datas.Count > 0)
            {
                data = datas.Find(d => d.language == language);
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
                var _dataProvider = GetUsableDataProvider();
                if (!ObjectUtility.IsNull(_dataProvider))
                    r = _dataProvider.TryGetData(_dataId, languageProvider, out data);
            }
            else if (datas != null && datas.Count > 0)
            {
                data = datas.Find(d =>
                    !ObjectUtility.IsNull(d.langProvider) && d.langProvider.IsProviderLanguage(languageProvider));
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
                var _dataProvider = GetUsableDataProvider();
                if (ObjectUtility.IsNull(_dataProvider)) return false;
                return _dataProvider.TryGetDatasById(dataId, ref result);
            }

            result = datas;
            return result != null && result.Count > 0;
        }

        public override void SetLanguage(string language)
        {
            var data = GetData(language);
            Execute(data);
        }

        public override void SetLanguage(ILanguageProvider languageProvider)
        {
            var data = GetData(languageProvider);
            Execute(data);
        }

        public override void SetLanguage(ILocalizationData data)
        {
            if (data is Data d)
                Execute(d);
        }

        /// <summary>直接使用数据设置语言</summary>
        public virtual void SetLanguage(Data data)
        {
            Execute(data);
        }

    }
}
