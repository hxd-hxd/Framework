using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditorInternal;
using Framework.Localization;
using Framework.LocalizationSimple;
using LangType = Framework.Localization.Language;

namespace Framework.Editor
{
    [CustomEditor(typeof(LocalizationSet))]
    class LocalizationSetInspector : UnityEditor.Editor
    {
        LocalizationSet my => (LocalizationSet)target;

        void OnEnable()
        {
            Undo.undoRedoPerformed += OnUndoRedo;
        }

        void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoRedo;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("语言设置", EditorStyles.boldLabel);

            //if (GUILayout.Button("设置当前语言"))
            //{
            //    ApplyLanguage(null);
            //}

            const int buttonsPerRow = 2;
            // 预留 Inspector 左右边距与滚动条，保证随面板宽度自适应且各按钮等宽
            float buttonWidth = (EditorGUIUtility.currentViewWidth - 40f) / buttonsPerRow;
            var buttonWidthOption = GUILayout.Width(buttonWidth);

            int index = 0;
            EditorGUILayout.BeginHorizontal();
            foreach (LangType lang in System.Enum.GetValues(typeof(LangType)))
            {
                if (index > 0 && index % buttonsPerRow == 0)
                {
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                }

                if (GUILayout.Button($"设置 {GetLangLabel(lang)}", buttonWidthOption))
                {
                    ApplyLanguage(lang);
                }

                index++;
            }
            EditorGUILayout.EndHorizontal();
        }

        void OnUndoRedo()
        {
            RefreshViews();
        }

        string GetLangLabel(LangType lang)
        {
            return my.LangTypeToString(lang);
        }

        void ApplyLanguage(LangType? lang)
        {
            //var targetLang = lang ?? .currentLang;
            var targetLang = lang.Value;
            string undoName = lang.HasValue ? $"设置 {GetLangLabel(lang.Value)}" : "设置当前语言";

            Undo.IncrementCurrentGroup();
            int undoGroup = Undo.GetCurrentGroup();

            switch (my._setMode)
            {
                case LocalizationSetMode.Type:
                    ApplyTypeLanguage(targetLang, undoName);
                    break;
                case LocalizationSetMode.Provider:
                    ApplyProviderLanguage(targetLang, undoName);
                    break;
            }

            Undo.SetCurrentGroupName(undoName);
            Undo.CollapseUndoOperations(undoGroup);
            RefreshViews();
        }

        void ApplyTypeLanguage(LangType lang, string undoName)
        {
            string type = GetLangLabel(lang);

            foreach (var localization in CollectLocalizations())
            {
                ApplyLocalization(localization, undoName, type, null);
            }
        }

        void ApplyProviderLanguage(LangType lang, string undoName)
        {
            var provider = my._langProviders?.Find(d => d != null && d.IsLanguage(lang));
            if (provider == null) return;

            foreach (var localization in CollectLocalizations())
            {
                ApplyLocalization(localization, undoName, null, provider);
            }
        }

        void ApplyLocalization(
            ILocalization localization,
            string undoName,
            string type,
            LanguageProviderComponentBase provider)
        {
            switch (localization)
            {
                case LocalizationBaseReference reference:
                    ApplyLocalizationBaseReference(reference, undoName, type, provider);
                    break;
                case DefaultLocalization dl:
                    ApplyDefaultLocalization(dl, undoName, type, provider);
                    break;
                case ButtonLocalization bl:
                    ApplyButtonLocalization(bl, undoName, type, provider);
                    break;
                case LocalizationBase lb:
                    ApplyLocalizationBaseCurrent(lb, undoName, type, provider);
                    break;
            }
        }

        void ApplyLocalizationBaseReference(
            LocalizationBaseReference reference,
            string undoName,
            string type,
            LanguageProviderComponentBase provider)
        {
            if (reference.localizations == null) return;

            ApplyLocalizationBaseCurrent(reference, undoName, type, provider);

            foreach (var localization in reference.localizations)
            {
                if (localization == null) continue;
                SyncDefaultLanguageFromReference(reference, localization, undoName);
                ApplyLocalization(localization, undoName, type, provider);
            }
        }

        static void SyncDefaultLanguageFromReference(
            LocalizationBaseReference reference,
            LocalizationBase localization,
            string undoName)
        {
            Undo.RecordObject(localization, undoName);
            var childSo = new SerializedObject(localization);
            childSo.Update();
            childSo.FindProperty("_defaultLanguage").stringValue = reference._defaultLanguage;
            childSo.FindProperty("_defaultProvider").objectReferenceValue = reference._defaultProvider;
            childSo.ApplyModifiedPropertiesWithoutUndo();
            RecordPrefabModifications(localization);
        }

        static SerializedObject BeginApplyLocalizationBase(
            LocalizationBase localization,
            string undoName,
            string type,
            LanguageProviderComponentBase provider)
        {
            Undo.RecordObject(localization, undoName);
            var so = new SerializedObject(localization);
            so.Update();

            if (provider != null)
            {
                so.FindProperty("_currentProvider").objectReferenceValue = provider;
            }
            else
            {
                so.FindProperty("_currentLanguage").stringValue = type;
            }

            return so;
        }

        static void EndApplyLocalizationBase(LocalizationBase localization, SerializedObject so)
        {
            so.ApplyModifiedPropertiesWithoutUndo();
            RecordPrefabModifications(localization);
        }

        static void ApplyLocalizationBaseCurrent(
            LocalizationBase localization,
            string undoName,
            string type,
            LanguageProviderComponentBase provider)
        {
            var so = BeginApplyLocalizationBase(localization, undoName, type, provider);
            EndApplyLocalizationBase(localization, so);
        }

        void ApplyDefaultLocalization(
            DefaultLocalization dl,
            string undoName,
            string type,
            LanguageProviderComponentBase provider)
        {
            var dlSo = BeginApplyLocalizationBase(dl, undoName, type, provider);

            ApplyItemsText(dl._itemsText, type, provider, dl._defaultLanguage, dl._defaultProvider, undoName);
            ApplyItemsImage(dl._itemsImage, type, provider, dl._defaultLanguage, dl._defaultProvider, undoName);
            ApplyItemsGameObject(dl, dlSo, type, provider, dl._defaultLanguage, dl._defaultProvider, undoName);

            EndApplyLocalizationBase(dl, dlSo);
        }

        void ApplyButtonLocalization(
            ButtonLocalization bl,
            string undoName,
            string type,
            LanguageProviderComponentBase provider)
        {
            var blSo = BeginApplyLocalizationBase(bl, undoName, type, provider);

            ApplyItemsButton(bl._itemsButton, type, provider, bl._defaultLanguage, bl._defaultProvider, undoName);

            EndApplyLocalizationBase(bl, blSo);
        }

        static void ApplyItemsText(
            List<LocalizationItemText> items,
            string type,
            LanguageProviderComponentBase provider,
            string defaultType,
            LanguageProviderComponentBase defaultProvider,
            string undoName)
        {
            if (items == null) return;

            foreach (var item in items)
            {
                if (item?.datas == null) continue;
                var data = FindDataWithFallback(item.datas, type, provider, defaultType, defaultProvider);
                if (data != null && data._text != null)
                {
                    ApplyText(item._item, data._text, undoName);
                }
            }
        }

        static void ApplyItemsImage(
            List<LocalizationItemImage> items,
            string type,
            LanguageProviderComponentBase provider,
            string defaultType,
            LanguageProviderComponentBase defaultProvider,
            string undoName)
        {
            if (items == null) return;

            foreach (var item in items)
            {
                if (item?.datas == null) continue;
                var data = FindDataWithFallback(item.datas, type, provider, defaultType, defaultProvider);
                if (data != null && data._sprite != null)
                {
                    ApplyImage(item._item, data._sprite, undoName);
                }
            }
        }

        static void ApplyItemsButton(
            List<LocalizationItemButton> items,
            string type,
            LanguageProviderComponentBase provider,
            string defaultType,
            LanguageProviderComponentBase defaultProvider,
            string undoName)
        {
            if (items == null) return;

            foreach (var item in items)
            {
                if (item?.datas == null || !item._item) continue;
                var data = FindDataWithFallback(item.datas, type, provider, defaultType, defaultProvider);
                if (data?._spriteSwapData != null)
                {
                    ApplyButton(item._item, data._spriteSwapData, undoName);
                }
            }
        }

        static void ApplyItemsGameObject(
            DefaultLocalization dl,
            SerializedObject dlSo,
            string type,
            LanguageProviderComponentBase provider,
            string defaultType,
            LanguageProviderComponentBase defaultProvider,
            string undoName)
        {
            var items = dl._itemsGameObject;
            if (items == null || items.Count == 0) return;

            var itemsProp = dlSo.FindProperty("_itemsGameObject");
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                if (item?.datas == null) continue;

                var data = FindDataWithFallback(item.datas, type, provider, defaultType, defaultProvider);
                if (data == null || data._gameObject == null) continue;

                var targetGO = data._gameObject;
                var itemProp = itemsProp.GetArrayElementAtIndex(i);
                var curGOProp = itemProp.FindPropertyRelative("_curGO");

                foreach (var d in item.datas)
                {
                    var otherGO = d?._gameObject;
                    if (!otherGO || otherGO == targetGO || !otherGO.activeSelf) continue;

                    Undo.RecordObject(otherGO, undoName);
                    otherGO.SetActive(false);
                    RecordPrefabModifications(otherGO);
                }

                Undo.RecordObject(targetGO, undoName);
                targetGO.SetActive(true);
                curGOProp.objectReferenceValue = targetGO;
                RecordPrefabModifications(targetGO);
            }
        }

        static T FindDataWithFallback<T>(
            List<T> datas,
            string type,
            LanguageProviderComponentBase provider,
            string defaultType,
            LanguageProviderComponentBase defaultProvider) where T : LocalizationDataBase
        {
            var data = FindData(datas, type, provider);
            if (data != null) return data;

            if (provider != null)
            {
                if (defaultProvider == null || provider.IsProviderLanguage(defaultProvider)) return null;
                return FindData(datas, null, defaultProvider);
            }

            if (string.IsNullOrEmpty(defaultType) || type == defaultType) return null;
            return FindData(datas, defaultType, null);
        }

        static T FindData<T>(
            List<T> datas,
            string type,
            LanguageProviderComponentBase provider) where T : LocalizationDataBase
        {
            if (provider != null)
            {
                return datas.Find(d =>
                    d._langProvider != null && d._langProvider.IsProviderLanguage(provider));
            }

            return datas.Find(d => d._language == type);
        }

        static void ApplyText(Text text, string value, string undoName)
        {
            if (!text) return;

            Undo.RecordObject(text, undoName);
            var so = new SerializedObject(text);
            so.Update();
            so.FindProperty("m_Text").stringValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
            RecordPrefabModifications(text);
        }

        static void ApplyImage(Image image, Sprite sprite, string undoName)
        {
            if (!image) return;

            Undo.RecordObject(image, undoName);
            var so = new SerializedObject(image);
            so.Update();
            so.FindProperty("m_Sprite").objectReferenceValue = sprite;
            so.ApplyModifiedPropertiesWithoutUndo();
            RecordPrefabModifications(image);
        }

        static void ApplyButton(
            Button button,
            LocalizationDataButtonSprite.SpriteSwapData spriteSwapData,
            string undoName)
        {
            if (!button || spriteSwapData == null) return;

            Undo.RecordObject(button, undoName);
            var so = new SerializedObject(button);
            so.Update();

            var spriteStateProp = so.FindProperty("m_SpriteState");
            ApplyButtonSprite(spriteStateProp.FindPropertyRelative("m_HighlightedSprite"), spriteSwapData._highlightedSprite);
            ApplyButtonSprite(spriteStateProp.FindPropertyRelative("m_PressedSprite"), spriteSwapData._pressedSprite);
            ApplyButtonSprite(spriteStateProp.FindPropertyRelative("m_SelectedSprite"), spriteSwapData._selectedSprite);
            ApplyButtonSprite(spriteStateProp.FindPropertyRelative("m_DisabledSprite"), spriteSwapData._disabledSprite);

            so.ApplyModifiedPropertiesWithoutUndo();
            RecordPrefabModifications(button);
        }

        static void ApplyButtonSprite(
            SerializedProperty prop,
            LocalizationDataButtonSprite.SpriteData spriteData)
        {
            if (prop == null || spriteData == null || !spriteData._enable) return;
            prop.objectReferenceValue = spriteData._sprite;
        }

        static void RecordPrefabModifications(Component component)
        {
            if (component && PrefabUtility.IsPartOfPrefabInstance(component))
            {
                PrefabUtility.RecordPrefabInstancePropertyModifications(component);
            }
        }

        static void RecordPrefabModifications(GameObject gameObject)
        {
            if (gameObject && PrefabUtility.IsPartOfPrefabInstance(gameObject))
            {
                PrefabUtility.RecordPrefabInstancePropertyModifications(gameObject);
            }
        }

        IEnumerable<ILocalization> CollectLocalizations()
        {
            if (my._localizations != null && my._localizations.Count > 0)
            {
                foreach (var localization in my._localizations)
                {
                    yield return localization;
                }
                yield break;
            }

            //foreach (var dl in my.GetComponents<ILocalization>())
            //{
            //    yield return dl;
            //}

            foreach (var dl in my.GetComponentsInChildren<ILocalization>(true))
            {
                yield return dl;
            }
        }

        void RefreshViews()
        {
            EditorUtility.SetDirty(my);

            foreach (var localization in CollectLocalizations())
            {
                if (localization is LocalizationBase lb)
                {
                    RefreshLocalizationBase(lb);
                }
                else if (localization is Component comp)
                {
                    EditorUtility.SetDirty(comp);
                }
            }

            Canvas.ForceUpdateCanvases();
            SceneView.RepaintAll();
            InternalEditorUtility.RepaintAllViews();
            EditorApplication.QueuePlayerLoopUpdate();
        }

        static void RefreshGraphic(Graphic graphic)
        {
            if (!graphic) return;
            graphic.SetAllDirty();
            EditorUtility.SetDirty(graphic);
        }

        static void RefreshLocalizationBaseReference(LocalizationBaseReference reference)
        {
            if (reference.localizations == null) return;

            foreach (var localization in reference.localizations)
            {
                RefreshLocalizationBase(localization);
            }

            EditorUtility.SetDirty(reference);
        }

        static void RefreshLocalizationBase(LocalizationBase localization)
        {
            if (!localization) return;

            switch (localization)
            {
                case LocalizationBaseReference reference:
                    RefreshLocalizationBaseReference(reference);
                    break;
                case DefaultLocalization dl:
                    RefreshDefaultLocalization(dl);
                    break;
                case ButtonLocalization bl:
                    RefreshButtonLocalization(bl);
                    break;
                default:
                    EditorUtility.SetDirty(localization);
                    break;
            }
        }

        static void RefreshDefaultLocalization(DefaultLocalization dl)
        {
            if (dl._itemsText != null)
            {
                foreach (var item in dl._itemsText)
                {
                    RefreshGraphic(item?._item);
                }
            }
            if (dl._itemsImage != null)
            {
                foreach (var item in dl._itemsImage)
                {
                    RefreshGraphic(item?._item);
                }
            }
            if (dl._itemsGameObject != null)
            {
                foreach (var item in dl._itemsGameObject)
                {
                    RefreshGameObjectItem(item);
                }
            }
            EditorUtility.SetDirty(dl);
        }

        static void RefreshButtonLocalization(ButtonLocalization bl)
        {
            if (bl._itemsButton != null)
            {
                foreach (var item in bl._itemsButton)
                {
                    RefreshButtonItem(item);
                }
            }
            EditorUtility.SetDirty(bl);
        }

        static void RefreshButtonItem(LocalizationItemButton item)
        {
            if (!item?._item) return;

            EditorUtility.SetDirty(item._item);
            RefreshGraphic(item._item.targetGraphic);
        }

        static void RefreshGameObjectItem(LocalizationItemGameObject item)
        {
            if (item?.datas == null) return;

            foreach (var data in item.datas)
            {
                if (data?._gameObject)
                {
                    EditorUtility.SetDirty(data._gameObject);
                }
            }
        }
    }
}