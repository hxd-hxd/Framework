using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Localization;
using Framework.LocalizationSimple;
using System;

namespace Framework.Test
{
    public class TestLocalization : MonoBehaviour
    {
        public Language _curLanguage;

        [Header("测试 LocalizationDataCfgAttribute")]
        [LocalizationDataCfg(LocalizationDataCfgMode.OnlyLang)]
        public LocalizationDataString _dataOnlyLang;
        [LocalizationDataCfg(LocalizationDataCfgMode.OnlyLang)]
        public List<LocalizationDataString> _datasOnlyLang;
        public TestLocalizationDataCfgAttribute<LocalizationDataString> _dataString;
        public TestLocalizationDataCfgAttribute<LocalizationDataSprite> _dataSprite;
        public TestLocalizationDataCfgAttribute<LocalizationData<Component>> _dataComponent;

        void Start()
        {

        }

        public void SetLanguage()
        {
            Debug.Log($"设置语言：{_curLanguage}");
            LocalizationCurLanguage.Instance.curLanguage = _curLanguage;
            LocalizationSetManager.Instance.Set();
        }

        public void SetLanguage_中文(bool isOn)
        {
            if (!isOn) return;
            _curLanguage = Language.ChineseSimplified;
            SetLanguage();
        }

        public void SetLanguage_中文_繁体(bool isOn)
        {
            if (!isOn) return;
            _curLanguage = Language.ChineseTraditional;
            SetLanguage();
        }

        public void SetLanguage_英文(bool isOn)
        {
            if (!isOn) return;
            _curLanguage = Language.English;
            SetLanguage();
        }

        [Serializable]
        public class TestLocalizationDataCfgAttribute<Data> where Data : LocalizationDataBase
        {

            public Data _data;

            // 测试直接应用到类型
            [LocalizationDataCfg(LocalizationDataCfgMode.All)]
            public Data _dataAll;

            [LocalizationDataCfg(LocalizationDataCfgMode.None)]
            public Data _dataNone;

            [LocalizationDataCfg(LocalizationDataCfgMode.OnlyId)]
            public Data _dataOnlyId;

            [LocalizationDataCfg(LocalizationDataCfgMode.OnlyLang)]
            public Data _dataOnlyLang;

            // 测试应用到容器类型
            //public List<string> _strs;
            
            public List<Data> _datas;
            
            [LocalizationDataCfg(LocalizationDataCfgMode.All)]
            public List<Data> _datasAll;

            [LocalizationDataCfg(LocalizationDataCfgMode.None)]
            public List<Data> _datasNone;

            [LocalizationDataCfg(LocalizationDataCfgMode.OnlyId)]
            public List<Data> _datasOnlyId;

            [LocalizationDataCfg(LocalizationDataCfgMode.OnlyLang)]
            public List<Data> _datasOnlyLang;

        }
    }
}
