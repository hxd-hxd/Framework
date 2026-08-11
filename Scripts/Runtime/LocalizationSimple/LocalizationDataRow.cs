using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Localization;

namespace Framework.LocalizationSimple
{
    /// <summary>本地化数据行，所有数据 id 都一样</summary>
    [Serializable]
    public class LocalizationDataRow<D> : LocalizationDataRowBase where D : ILocalizationData
    {
        public string _id;
        //public string _language;
        //public LanguageProviderComponentBase _langProvider;
        public List<D> _datas;
        private Dictionary<string, D> _langDic;// 按语言存的

        /// <summary>语言</summary>
        public override string id { get => _id; set => _id = value; }

        ///// <summary>语言</summary>
        //public string language { get => _language; set => _language = value; }

        ///// <summary>语言提供者</summary>
        //public LanguageProviderComponentBase langProvider { get => _langProvider; set => _langProvider = value; }

        /// <summary>数据</summary>
        public List<D> datas { get => _datas; set => _datas = value; }

        public override Type dataType => typeof(D);

        public void Init()
        {
            _langDic ??= new Dictionary<string, D>(_datas.Count);
            _langDic.Clear();
            foreach (var d in _datas)
            {
                _langDic[d.language] = d;
            }
        }

        /// <summary>获取所有数据，忽略 <paramref name="id"/></summary>
        public override List<Data> GetDatasById<Data>(string id)
        {
            List<Data> rs = null;
            TryGetDatasById(id, ref rs);
            return rs;
        }

        /// <summary>获取所有数据，忽略 <paramref name="id"/></summary>
        public override bool TryGetDatasById<Data>(string id, ref List<Data> rs)
        {
            if (_datas == null || _datas.Count <= 0) return false;

            rs ??= TypePool.root.GetList<Data>();
            foreach (var d in _datas)
            {
                if (d is Data data)
                {
                    data.id = this.id;
                    rs.Add(data);
                }
            }
            return true;
        }

        public override List<Data> GetDatasByLang<Data>(string language)
        {
            List<Data> rs = null;
            TryGetDatasByLang(language, ref rs);
            return rs;
        }

        public override bool TryGetDatasByLang<Data>(string language, ref List<Data> rs)
        {
            if (_datas == null || _datas.Count <= 0) return false;

            rs ??= TypePool.root.GetList<Data>();
            foreach (var d in _datas)
            {
                if (d is Data data && d.language == language)
                {
                    data.id = this.id;
                    rs.Add(data);
                }
            }
            return true;
        }

        public override List<Data> GetDatasByLang<Data>(ILanguageProvider languageProvider)
        {
            List<Data> rs = null;
            TryGetDatasByLang(languageProvider, ref rs);
            return rs;
        }

        public override bool TryGetDatasByLang<Data>(ILanguageProvider languageProvider, ref List<Data> rs)
        {
            if (_datas == null || _datas.Count <= 0) return false;

            rs ??= TypePool.root.GetList<Data>();
            foreach (var d in _datas)
            {
                if (d is Data data
                    && !ObjectUtility.IsNull(d.langProvider)
                    && d.langProvider.IsProviderLanguage(languageProvider))
                {
                    data.id = this.id;
                    rs.Add(data);
                }
            }
            return true;
        }

        /// <summary>获取对应语言的数据，忽略 <paramref name="id"/></summary>
        public override Data GetData<Data>(string id, string language)
        {
            Data data = default;
            TryGetData(id, language, out data);
            return data;
        }

        /// <summary>尝试获取对应语言的数据，忽略 <paramref name="id"/></summary>
        public override bool TryGetData<Data>(string id, string language, out Data data)
        {
            bool r = false;
            data = default;
            if (datas != null && datas.Count > 0)
            {
                data = (Data)(object)datas.Find(d => d.language == language);
                if (data != null) data.id = this.id;
                r = data != null;
            }
            return r;
        }

        /// <summary>获取对应语言提供者的数据，忽略 <paramref name="id"/></summary>
        public override Data GetData<Data>(string id, ILanguageProvider languageProvider)
        {
            Data data = default;
            TryGetData(id, languageProvider, out data);
            return data;
        }

        /// <summary>尝试获取对应语言提供者的数据，忽略 <paramref name="id"/></summary>
        public override bool TryGetData<Data>(string id, ILanguageProvider languageProvider, out Data data)
        {
            bool r = false;
            data = default;
            if (ObjectUtility.IsNull(languageProvider)) return r;
            if (datas != null && datas.Count > 0)
            {
                data = (Data)(object)datas.Find(d =>
                {
                    return !ObjectUtility.IsNull(d.langProvider) && d.langProvider.IsProviderLanguage(languageProvider);
                });
                if (data != null) data.id = this.id;
                r = data != null;
            }
            return r;
        }

        // 忽略 id 的方法
        /// <summary>获取对应语言的数据</summary>
        public Data GetData<Data>(string language) where Data : ILocalizationData
        {
            return GetData<Data>(null, language);
        }

        /// <summary>尝试获取对应语言的数据</summary>
        public bool TryGetData<Data>(string language, out Data data) where Data : ILocalizationData
        {
            return TryGetData(null, language, out data);
        }

        /// <summary>获取对应语言提供者的数据</summary>
        public Data GetData<Data>(ILanguageProvider languageProvider) where Data : ILocalizationData
        {
            return GetData<Data>(null, languageProvider);
        }

        /// <summary>尝试获取对应语言提供者的数据</summary>
        public bool TryGetData<Data>(ILanguageProvider languageProvider, ref Data data) where Data : ILocalizationData
        {
            return TryGetData(null, languageProvider, out data);
        }

    }
}
