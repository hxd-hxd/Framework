using UnityEditor;
using Framework;
using Framework.LocalizationSimple;
using LangType = Framework.Localization.Language;

namespace Framework.Editor
{
    [CustomEditor(typeof(LocalizationSet))]
    class LocalizationSetInspector : LocalizationSetInspectorBase
    {
        protected override object GetCurrentLanguage() => LocalizationCurLanguage.Ins.curLanguage;

        protected override bool IsLanguageVisible(object lang) =>
            !(lang is LangType typed && typed == LangType.Unspecified);
    }
}
