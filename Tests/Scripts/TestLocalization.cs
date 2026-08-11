using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Localization;
using Framework.LocalizationSimple;

namespace Framework.Test
{
    public class TestLocalization : MonoBehaviour
    {
        public Language _curLanguage;

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
    }
}
