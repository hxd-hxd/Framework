using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Localization;
using Framework.LocalizationSimple;

namespace Framework.Test
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(TestLocalizationLanguageProvider))]
    public class TestLocalizationLanguageProviderInspector : Editor
    {
        TestLocalizationLanguageProvider my => target as TestLocalizationLanguageProvider;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("语言提供者组件信息", EditorStyles.boldLabel);

            if (my._langProviderComp)
                my._langProviderComp._lang = (Language)EditorGUILayout.EnumPopup("语言提供者组件语言", my._langProviderComp._lang);

            EditorGUILayout.LabelField("获取不同类型语言提供者提供的语言");
            EditorGUI.indentLevel++;
            EditorGUILayout.TextField("通过类型 object 获取", my._langProviderComp.GetLanguage<object>().ToString());
            EditorGUILayout.TextField("通过类型 Enum 获取", my._langProviderComp.GetLanguage<Enum>().ToString());
            EditorGUILayout.TextField("通过类型 Language 获取", my._langProviderComp.GetLanguage<Language>().ToString());
            my._langProviderComp.TryGetLanguage(out string langStr);
            EditorGUILayout.TextField("通过类型 string 获取", langStr);
            EditorGUI.indentLevel--;

            EditorGUILayout.LabelField("判断语言提供者是否提供指定语言");
            EditorGUI.indentLevel++;
            EditorGUILayout.Toggle($"{my._lang}（{my._lang.GetType()}）", my._langProviderComp.IsLanguage(my._lang));
            EditorGUILayout.Toggle($"{my._lang}（object）", my._langProviderComp.IsLanguage((object)my._lang));
            EditorGUILayout.Toggle($"{my._lang}（Enum）", my._langProviderComp.IsLanguage((Enum)my._lang));
            EditorGUILayout.Toggle($"{my._lang}（string）", my._langProviderComp.IsLanguage(my._lang.ToString()));
            EditorGUI.indentLevel--;

            EditorGUILayout.Toggle($"提供者组件是否与提供者提供相同语言", my._langProviderComp.IsProviderLanguage(my._langProvider));
        }
    }
#endif

    public class TestLocalizationLanguageProvider : MonoBehaviour
    {
        public Language _lang;

        public DefaultLangProviderComp _langProviderComp;

        public DefaultLangProvider _langProvider;

        void Start()
        {

        }

    }
}
