using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Localization;

namespace Framework.LocalizationSimple
{
    /// <summary>本地化数据提供者，只提供对应语言的数据</summary>
    [Serializable]
    public class DefaultLocalizationDataProvider : ILocalizationDataProvider
    {
        public List<LocalizationDataRow<LocalizationDataString>> _stringDatas;
        public List<LocalizationDataRow<LocalizationDataSprite>> _spriteDatas;
        public List<LocalizationDataRow<LocalizationDataGameObject>> _goDatas;
        public List<LocalizationDataRow<LocalizationDataSelectableSprite>> _selectableSpriteDatas;
        private Dictionary<string, ILocalizationDataProvider> _rowDic;// 按 id 存的

        public void Init()
        {
            int capacity = (_stringDatas?.Count ?? 0)
                + (_spriteDatas?.Count ?? 0)
                + (_goDatas?.Count ?? 0)
                + (_selectableSpriteDatas?.Count ?? 0);
            _rowDic ??= new Dictionary<string, ILocalizationDataProvider>(Math.Max(capacity, 4));
            _rowDic.Clear();

            AddRows(_stringDatas);
            AddRows(_spriteDatas);
            AddRows(_goDatas);
            AddRows(_selectableSpriteDatas);

            void AddRows<Data>(List<LocalizationDataRow<Data>> rows) where Data : ILocalizationData
            {
                if (rows == null) return;
                foreach (var d in rows)
                {
                    if (d == null || string.IsNullOrEmpty(d.id)) continue;
                    _rowDic[d.id] = d;
                }
            }
        }

        public List<Data> GetDatasById<Data>(string id) where Data : ILocalizationData
        {
            List<Data> rs = null;
            TryGetDatasById(id, ref rs);
            return rs;
        }

        public bool TryGetDatasById<Data>(string id, ref List<Data> rs) where Data : ILocalizationData
        {
            if (_rowDic == null || _rowDic.Count <= 0)
            {
                Init();
            }
            if (_rowDic.TryGetValue(id, out var row))
            {
                return row.TryGetDatasById(id, ref rs);
            }
            return false;
        }

        public List<Data> GetDatasByLang<Data>(string language) where Data : ILocalizationData
        {
            List<Data> rs = null;
            TryGetDatasByLang(language, ref rs);
            return rs;
        }

        public bool TryGetDatasByLang<Data>(string language, ref List<Data> rs) where Data : ILocalizationData
        {
            if (_rowDic == null || _rowDic.Count <= 0)
            {
                Init();
            }
            bool r = false;
            foreach (var row in _rowDic)
            {
                r = r || row.Value.TryGetDatasByLang(language, ref rs);
            }
            return r;
        }

        public List<Data> GetDatasByLang<Data>(ILanguageProvider languageProvider) where Data : ILocalizationData
        {
            if (ObjectUtility.IsNull(languageProvider)) return default;
            List<Data> rs = null;
            TryGetDatasByLang(languageProvider, ref rs);
            return rs;
        }

        public bool TryGetDatasByLang<Data>(ILanguageProvider languageProvider, ref List<Data> rs) where Data : ILocalizationData
        {
            bool r = false;
            if (ObjectUtility.IsNull(languageProvider)) return r;
            if (_rowDic == null || _rowDic.Count <= 0)
            {
                Init();
            }
            foreach (var row in _rowDic)
            {
                r = r || row.Value.TryGetDatasByLang(languageProvider, ref rs);
            }
            return r;
        }

        public Data GetData<Data>(string id, string language) where Data : ILocalizationData
        {
            Data data = default;
            TryGetData(id, language, out data);
            return data;
        }

        public bool TryGetData<Data>(string id, string language, out Data data) where Data : ILocalizationData
        {
            bool r = false;
            data = default;
            if (_rowDic == null || _rowDic.Count <= 0)
            {
                Init();
            }
            if (_rowDic.TryGetValue(id, out var datas))
            {
                r = datas.TryGetData(id, language, out data);
            }
            return r;
        }

        public Data GetData<Data>(string id, ILanguageProvider languageProvider) where Data : ILocalizationData
        {
            Data data = default;
            TryGetData(id, languageProvider, out data);
            return data;
        }

        public bool TryGetData<Data>(string id, ILanguageProvider languageProvider, out Data data) where Data : ILocalizationData
        {
            bool r = false;
            data = default;
            if (ObjectUtility.IsNull(languageProvider)) return r;
            if (_rowDic == null || _rowDic.Count <= 0)
            {
                Init();
            }
            if (_rowDic.TryGetValue(id, out var datas))
            {
                r = datas.TryGetData(id, languageProvider, out data);
            }
            return r;
        }
    }
}
