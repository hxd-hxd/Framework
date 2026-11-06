using UnityEngine;
using Framework.Localization;

namespace Framework.LocalizationSimple
{
    /// <summary>在初始时设置当前本地化语言</summary>
    public class LocalizationCurLangSetOnAwakeComp : MonoBehaviour
    {
        [SerializeField]
        private Language _curLanguageOnAwake = Language.ChineseSimplified;

        private void Awake()
        {
            LocalizationCurLanguage.Instance.curLanguage = _curLanguageOnAwake;
        }

        private void Start()
        {
            
        }
    }
}
