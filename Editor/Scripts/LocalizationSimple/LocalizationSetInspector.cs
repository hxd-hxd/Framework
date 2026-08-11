using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Framework;
using Framework.Localization;
using Framework.LocalizationSimple;
using LangType = Framework.Localization.Language;

namespace Framework.Editor
{
    [CustomEditor(typeof(LocalizationSet))]
    class LocalizationSetInspector : LocalizationSetInspectorBase<LangType>
    {
        LocalizationSet my => (LocalizationSet)target;

        protected override Component SetTarget => my;
        protected override LocalizationSetMode SetMode => my._setMode;
        protected override List<ILocalization> ConfiguredLocalizations => my._localizations;
        protected override List<LanguageProviderComponentBase> LangProviders => my._langProviders;

        protected override string GetLangLabel(LangType lang) => my.LangTypeToString(lang);

        protected override LangType GetCurrentLanguage() => LocalizationCurLanguage.Ins.curLanguage;

        protected override bool IsLanguageVisible(LangType lang) => lang != LangType.Unspecified;

        protected override void DrawCurrentLanguageButton()
        {
            var curLang = GetCurrentLanguage();
            if (GUILayout.Button($"设置当前语言（{GetLangLabel(curLang)}）"))
                ApplyLanguage(curLang);
        }
    }
}
