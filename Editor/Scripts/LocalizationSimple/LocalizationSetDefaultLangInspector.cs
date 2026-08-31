using System.Collections.Generic;
using Framework.Localization;
using Framework.LocalizationSimple;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.Editor
{
    /// <summary>
    /// <see cref="LocalizationSetDefaultLang"/> 检视面板：
    /// 显示 <see cref="LocalizationSetManager"/> 当前默认语言与语言提供者，并提供 SetToGlobal / SetToLocal 按钮。
    /// </summary>
    [CustomEditor(typeof(LocalizationSetDefaultLang))]
    public class LocalizationSetDefaultLangInspector : UnityEditor.Editor
    {
        const string SetToLocalUndoName = "设置语言到本地化";

        LocalizationSetDefaultLang my => (LocalizationSetDefaultLang)target;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("LocalizationSetManager", EditorStyles.boldLabel);

            var manager = LocalizationSetManager.Instance;
            DrawCurrentDefaultLanguage(manager.defaultLanguage);
            DrawCurrentDefaultLangProvider(manager.defaultLangProvider);

            EditorGUILayout.Space();
            if (GUILayout.Button("设置语言到全局"))
            {
                my.SetToGlobal();
            }

            DrawSetToLocalButton();
        }

        static void DrawCurrentDefaultLanguage(string language)
        {
            if (string.IsNullOrEmpty(language))
            {
                EditorGUILayout.HelpBox("当前无全局默认语言类型", MessageType.Info);
                return;
            }

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("当前全局默认语言类型", language);
            EditorGUI.EndDisabledGroup();
        }

        static void DrawCurrentDefaultLangProvider(ILanguageProvider provider)
        {
            if (provider == null)
            {
                EditorGUILayout.HelpBox("当前无全局默认语言提供者", MessageType.Info);
                return;
            }

            EditorGUI.BeginDisabledGroup(true);
            if (provider is Object unityObj)
            {
                EditorGUILayout.ObjectField("当前全局默认语言提供者", unityObj, typeof(Object), true);
            }
            else
            {
                EditorGUILayout.LabelField("当前全局默认语言提供者", provider.GetType().FullName);
            }
            EditorGUI.EndDisabledGroup();
        }

        void DrawSetToLocalButton()
        {
            serializedObject.Update();
            var set = serializedObject.FindProperty("_localizationSet").objectReferenceValue as LocalizationSetBase;

            EditorGUI.BeginDisabledGroup(set == null);
            if (GUILayout.Button("设置语言到本地化"))
                SetLanguageToAllLocalizations(set);
            EditorGUI.EndDisabledGroup();

            if (set == null)
                EditorGUILayout.HelpBox("未指定 LocalizationSet，无法设置到本地化", MessageType.Info);
        }

        /// <summary>
        /// 编辑器版 <see cref="LocalizationSetDefaultLang.SetToLocal"/>：
        /// 走同一套运行时 API（<see cref="ILocalization.defaultLanguage"/> / <see cref="ILocalization.defaultProvider"/>），
        /// 并在修改前记录所有本地化 Unity 对象以支持撤销重做。
        /// </summary>
        void SetLanguageToAllLocalizations(LocalizationSetBase set)
        {
            if (set == null) return;

            Undo.IncrementCurrentGroup();
            int undoGroup = Undo.GetCurrentGroup();

            bool setListEmpty = set._localizations == null || set._localizations.Count <= 0;
            if (setListEmpty)
                Undo.RecordObject(set, SetToLocalUndoName);

            var objects = new HashSet<Object>();
            foreach (var localization in set.localizations)
                CollectLocalizationObjects(localization, objects);

            foreach (var obj in objects)
                Undo.RecordObject(obj, SetToLocalUndoName);

            my.SetToLocal();

            if (setListEmpty)
            {
                EditorUtility.SetDirty(set);
                RecordPrefabModifications(set);
            }

            foreach (var obj in objects)
            {
                EditorUtility.SetDirty(obj);
                RecordPrefabModifications(obj);
            }

            Undo.SetCurrentGroupName(SetToLocalUndoName);
            Undo.CollapseUndoOperations(undoGroup);
        }

        /// <summary>
        /// 收集本地化 Unity 对象及其序列化字段中引用的嵌套 <see cref="ILocalization"/>，
        /// 不依赖具体组件类型，以便撤销能覆盖引用型本地化（如 LocalizationBaseReference）。
        /// </summary>
        static void CollectLocalizationObjects(ILocalization localization, HashSet<Object> objects)
        {
            if (!(localization is Object unityObj) || !objects.Add(unityObj))
                return;

            var so = new SerializedObject(unityObj);
            var iterator = so.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = true;
                if (iterator.propertyType != SerializedPropertyType.ObjectReference)
                    continue;
                if (iterator.objectReferenceValue is ILocalization nested)
                    CollectLocalizationObjects(nested, objects);
            }
        }

        static void RecordPrefabModifications(Object obj)
        {
            if (obj && PrefabUtility.IsPartOfPrefabInstance(obj))
                PrefabUtility.RecordPrefabInstancePropertyModifications(obj);
        }
    }
}
