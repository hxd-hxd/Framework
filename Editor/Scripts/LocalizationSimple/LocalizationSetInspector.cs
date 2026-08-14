using UnityEditor;
using Framework.LocalizationSimple;
using LangType = Framework.Localization.Language;

namespace Framework.Editor
{
    [CustomEditor(typeof(LocalizationSet))]
    class LocalizationSetInspector : LocalizationSetInspectorBase
    {
        protected override bool IsLanguageVisible(object lang) =>
            !(lang is LangType typed && typed == LangType.Unspecified);
    }
}
