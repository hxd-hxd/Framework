using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEditor;
using UnityEditorInternal;
using Framework.Localization;
using Framework.LocalizationSimple;
using Object = UnityEngine.Object;

namespace Framework.Editor
{
    /// <summary>
    /// <see cref="LocalizationSet"/> 检视面板基类。
    /// 语言枚举与当前语言来源由子类注入，Apply / Refresh 逻辑共用。
    /// </summary>
    public abstract class LocalizationSetInspectorBase<TLang> : UnityEditor.Editor
        where TLang : struct, Enum
    {
        protected abstract Component SetTarget { get; }
        protected abstract LocalizationSetMode SetMode { get; }
        protected abstract List<ILocalization> ConfiguredLocalizations { get; }
        protected abstract List<LanguageProviderComponentBase> LangProviders { get; }
        protected abstract string GetLangLabel(TLang lang);
        protected abstract TLang GetCurrentLanguage();
        protected abstract void DrawCurrentLanguageButton();
        protected virtual bool IsLanguageVisible(TLang lang) => true;

        protected virtual void OnEnable()
        {
            Undo.undoRedoPerformed += OnUndoRedo;
        }

        protected virtual void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoRedo;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("语言设置", EditorStyles.boldLabel);

            DrawCurrentLanguageButton();
            DrawLanguageButtons();
        }

        protected void DrawLanguageButtons()
        {
            var langs = GetLanguagesToShow();
            if (langs.Count == 0)
            {
                if (SetMode == LocalizationSetMode.Provider)
                    EditorGUILayout.HelpBox("语言提供者列表中没有可匹配的语言", MessageType.Info);
                return;
            }

            const int buttonsPerRow = 2;
            // 预留 Inspector 左右边距与滚动条，保证随面板宽度自适应且各按钮等宽
            float buttonWidth = (EditorGUIUtility.currentViewWidth - 40f) / buttonsPerRow;
            var buttonWidthOption = GUILayout.Width(buttonWidth);

            for (int index = 0; index < langs.Count; index++)
            {
                if (index % buttonsPerRow == 0)
                    EditorGUILayout.BeginHorizontal();

                var lang = langs[index];
                if (GUILayout.Button($"设置 {GetLangLabel(lang)}", buttonWidthOption))
                    ApplyLanguage(lang);

                if (index % buttonsPerRow == buttonsPerRow - 1 || index == langs.Count - 1)
                    EditorGUILayout.EndHorizontal();
            }
        }

        protected List<TLang> GetLanguagesToShow()
        {
            var result = new List<TLang>();
            foreach (TLang lang in Enum.GetValues(typeof(TLang)))
            {
                if (!IsLanguageVisible(lang))
                    continue;

                if (SetMode == LocalizationSetMode.Provider)
                {
                    var providers = LangProviders;
                    if (providers == null ||
                        !providers.Exists(d => d != null && d.IsLanguage(lang)))
                        continue;
                }

                result.Add(lang);
            }
            return result;
        }

        void OnUndoRedo()
        {
            RefreshViews();
        }

        protected void ApplyLanguage(TLang? lang)
        {
            var targetLang = lang ?? GetCurrentLanguage();
            string undoName = lang.HasValue ? $"设置 {GetLangLabel(lang.Value)}" : "设置当前语言";

            Undo.IncrementCurrentGroup();
            int undoGroup = Undo.GetCurrentGroup();

            switch (SetMode)
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

        void ApplyTypeLanguage(TLang lang, string undoName)
        {
            string type = GetLangLabel(lang);

            foreach (var localization in CollectLocalizations())
            {
                ApplyLocalization(localization, undoName, type, null);
            }
        }

        void ApplyProviderLanguage(TLang lang, string undoName)
        {
            var provider = LangProviders?.Find(d => d != null && d.IsLanguage(lang));
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
                case SelectableLocalization bl:
                    ApplySelectableLocalization(bl, undoName, type, provider);
                    break;
                case LocalizationCompString cs:
                    ApplyLocalizationCompString(cs, undoName, type, provider);
                    break;
                case LocalizationCompSprite csp:
                    ApplyLocalizationCompSprite(csp, undoName, type, provider);
                    break;
                case LocalizationCompGameObject cgo:
                    ApplyLocalizationCompGameObject(cgo, undoName, type, provider);
                    break;
                case LocalizationCompSelectable csel:
                    ApplyLocalizationCompSelectable(csel, undoName, type, provider);
                    break;
                case LocalizationCompBase lb:
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
            LocalizationCompBase localization,
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
            LocalizationCompBase localization,
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

        static void EndApplyLocalizationBase(LocalizationCompBase localization, SerializedObject so)
        {
            so.ApplyModifiedPropertiesWithoutUndo();
            RecordPrefabModifications(localization);
        }

        static void ApplyLocalizationBaseCurrent(
            LocalizationCompBase localization,
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

        void ApplySelectableLocalization(
            SelectableLocalization bl,
            string undoName,
            string type,
            LanguageProviderComponentBase provider)
        {
            var blSo = BeginApplyLocalizationBase(bl, undoName, type, provider);

            ApplyItemsSelectable(bl._itemsButton, type, provider, bl._defaultLanguage, bl._defaultProvider, undoName);

            EndApplyLocalizationBase(bl, blSo);
        }

        void ApplyLocalizationCompString(
            LocalizationCompString comp,
            string undoName,
            string type,
            LanguageProviderComponentBase provider)
        {
            var so = BeginApplyLocalizationBase(comp, undoName, type, provider);
            EndApplyLocalizationBase(comp, so);

            var item = comp._item;
            if (item == null) return;

            var data = FindItemDataWithFallback(item, type, provider, comp._defaultLanguage, comp._defaultProvider);
            if (data != null && data._text != null)
                ApplyItemString(item, data, undoName);
        }

        void ApplyLocalizationCompSprite(
            LocalizationCompSprite comp,
            string undoName,
            string type,
            LanguageProviderComponentBase provider)
        {
            var so = BeginApplyLocalizationBase(comp, undoName, type, provider);
            EndApplyLocalizationBase(comp, so);

            var item = comp._item;
            if (item == null) return;

            var data = FindItemDataWithFallback(item, type, provider, comp._defaultLanguage, comp._defaultProvider);
            if (data != null && data._sprite != null)
                ApplyItemSprite(item, data, undoName);
        }

        void ApplyLocalizationCompGameObject(
            LocalizationCompGameObject comp,
            string undoName,
            string type,
            LanguageProviderComponentBase provider)
        {
            var so = BeginApplyLocalizationBase(comp, undoName, type, provider);

            var item = comp._item;
            if (item != null)
            {
                var data = FindItemDataWithFallback(item, type, provider, comp._defaultLanguage, comp._defaultProvider);
                if (data != null && data._gameObject != null)
                    ApplyGameObjectItem(item, so.FindProperty("_item"), data, undoName);
            }

            EndApplyLocalizationBase(comp, so);
        }

        void ApplyLocalizationCompSelectable(
            LocalizationCompSelectable comp,
            string undoName,
            string type,
            LanguageProviderComponentBase provider)
        {
            var so = BeginApplyLocalizationBase(comp, undoName, type, provider);
            EndApplyLocalizationBase(comp, so);

            ApplySelectableItem(comp._item, type, provider, comp._defaultLanguage, comp._defaultProvider, undoName);
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
                if (item == null) continue;
                var data = FindItemDataWithFallback(item, type, provider, defaultType, defaultProvider);
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
                if (item == null) continue;
                var data = FindItemDataWithFallback(item, type, provider, defaultType, defaultProvider);
                if (data != null && data._sprite != null)
                {
                    ApplyImage(item._item, data._sprite, undoName);
                }
            }
        }

        static void ApplyItemsSelectable(
            List<LocalizationItemSelectable> items,
            string type,
            LanguageProviderComponentBase provider,
            string defaultType,
            LanguageProviderComponentBase defaultProvider,
            string undoName)
        {
            if (items == null) return;

            foreach (var item in items)
            {
                ApplySelectableItem(item, type, provider, defaultType, defaultProvider, undoName);
            }
        }

        static void ApplySelectableItem(
            LocalizationItemSelectable item,
            string type,
            LanguageProviderComponentBase provider,
            string defaultType,
            LanguageProviderComponentBase defaultProvider,
            string undoName)
        {
            if (item == null) return;

            var data = FindItemDataWithFallback(item, type, provider, defaultType, defaultProvider);
            if (data?._spriteSwapData == null) return;

#pragma warning disable CS0618
            var legacyButton = item._item;
#pragma warning restore CS0618
            if (!legacyButton && !item._itemS) return;

            if (legacyButton)
                ApplySelectable(legacyButton, data._spriteSwapData, undoName);
            if (item._itemS)
                ApplySelectable(item._itemS, data._spriteSwapData, undoName);
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
                if (item == null) continue;

                var data = FindItemDataWithFallback(item, type, provider, defaultType, defaultProvider);
                if (data == null || data._gameObject == null) continue;

                ApplyGameObjectItem(item, itemsProp.GetArrayElementAtIndex(i), data, undoName);
            }
        }

        static void ApplyGameObjectItem(
            LocalizationItemGameObject item,
            SerializedProperty itemProp,
            LocalizationDataGameObject data,
            string undoName)
        {
            if (item == null || data?._gameObject == null || itemProp == null) return;

            var targetGO = data._gameObject;
            var curGOProp = itemProp.FindPropertyRelative("_curGO");

            if (item.TryGetAllDatas(out var allDatas) && allDatas != null)
            {
                foreach (var d in allDatas)
                {
                    var otherGO = d?._gameObject;
                    if (!otherGO || otherGO == targetGO || !otherGO.activeSelf) continue;

                    Undo.RecordObject(otherGO, undoName);
                    otherGO.SetActive(false);
                    RecordPrefabModifications(otherGO);
                }
            }

            Undo.RecordObject(targetGO, undoName);
            targetGO.SetActive(true);
            if (curGOProp != null)
                curGOProp.objectReferenceValue = targetGO;
            RecordPrefabModifications(targetGO);
        }

        static void ApplyItemString(LocalizationItemString item, LocalizationDataString data, string undoName)
        {
            if (item == null || data == null) return;
            RecordUnityEventTargets(item.onExecute, undoName);
            item.SetLanguage(data);
            DirtyUnityEventTargets(item.onExecute);
        }

        static void ApplyItemSprite(LocalizationItemSprite item, LocalizationDataSprite data, string undoName)
        {
            if (item == null || data == null) return;
            RecordUnityEventTargets(item.onExecute, undoName);
            item.SetLanguage(data);
            DirtyUnityEventTargets(item.onExecute);
        }

        static void RecordUnityEventTargets(UnityEventBase evt, string undoName)
        {
            if (evt == null) return;
            for (int i = 0; i < evt.GetPersistentEventCount(); i++)
            {
                var target = evt.GetPersistentTarget(i) as Object;
                if (!target) continue;
                Undo.RecordObject(target, undoName);
            }
        }

        static void DirtyUnityEventTargets(UnityEventBase evt)
        {
            if (evt == null) return;
            for (int i = 0; i < evt.GetPersistentEventCount(); i++)
            {
                switch (evt.GetPersistentTarget(i))
                {
                    case Component c when c:
                        EditorUtility.SetDirty(c);
                        RecordPrefabModifications(c);
                        break;
                    case GameObject go when go:
                        EditorUtility.SetDirty(go);
                        RecordPrefabModifications(go);
                        break;
                }
            }
        }

        /// <summary>
        /// 按 Item 的 Data / Provider 取数方式查找数据，并回退到默认语言；逻辑对齐 <see cref="LocalizationCompBase"/>。
        /// </summary>
        static T FindItemDataWithFallback<T>(
            LocalizationItemBase<T> item,
            string type,
            LanguageProviderComponentBase provider,
            string defaultType,
            LanguageProviderComponentBase defaultProvider) where T : LocalizationDataBase
        {
            if (item == null) return null;

            if (provider != null)
            {
                if (item.TryGetData(provider, out T data) && data != null) return data;
                if (defaultProvider == null || provider.IsProviderLanguage(defaultProvider)) return null;
                return item.TryGetData(defaultProvider, out data) ? data : null;
            }

            if (item.TryGetData(type, out T dataByType) && dataByType != null) return dataByType;
            if (string.IsNullOrEmpty(defaultType) || type == defaultType) return null;
            return item.TryGetData(defaultType, out dataByType) ? dataByType : null;
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

        static void ApplySelectable(
            Selectable selectable,
            LocalizationDataSelectableSprite.SpriteSwapData spriteSwapData,
            string undoName)
        {
            if (!selectable || spriteSwapData == null) return;

            Undo.RecordObject(selectable, undoName);
            var so = new SerializedObject(selectable);
            so.Update();

            var spriteStateProp = so.FindProperty("m_SpriteState");
            ApplySelectableSprite(spriteStateProp.FindPropertyRelative("m_HighlightedSprite"), spriteSwapData._highlightedSprite);
            ApplySelectableSprite(spriteStateProp.FindPropertyRelative("m_PressedSprite"), spriteSwapData._pressedSprite);
            ApplySelectableSprite(spriteStateProp.FindPropertyRelative("m_SelectedSprite"), spriteSwapData._selectedSprite);
            ApplySelectableSprite(spriteStateProp.FindPropertyRelative("m_DisabledSprite"), spriteSwapData._disabledSprite);

            so.ApplyModifiedPropertiesWithoutUndo();
            RecordPrefabModifications(selectable);
        }

        static void ApplySelectableSprite(
            SerializedProperty prop,
            LocalizationDataSelectableSprite.SpriteData spriteData)
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

        protected IEnumerable<ILocalization> CollectLocalizations()
        {
            var configured = ConfiguredLocalizations;
            if (configured != null && configured.Count > 0)
            {
                foreach (var localization in configured)
                {
                    yield return localization;
                }
                yield break;
            }

            foreach (var dl in SetTarget.GetComponentsInChildren<ILocalization>(true))
            {
                yield return dl;
            }
        }

        void RefreshViews()
        {
            EditorUtility.SetDirty(SetTarget);

            foreach (var localization in CollectLocalizations())
            {
                if (localization is LocalizationCompBase lb)
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

        static void RefreshLocalizationBase(LocalizationCompBase localization)
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
                case SelectableLocalization bl:
                    RefreshSelectableLocalization(bl);
                    break;
                case LocalizationCompString cs:
                    RefreshUnityEventTargets(cs._item?.onExecute);
                    EditorUtility.SetDirty(cs);
                    break;
                case LocalizationCompSprite csp:
                    RefreshUnityEventTargets(csp._item?.onExecute);
                    EditorUtility.SetDirty(csp);
                    break;
                case LocalizationCompGameObject cgo:
                    RefreshGameObjectItem(cgo._item);
                    EditorUtility.SetDirty(cgo);
                    break;
                case LocalizationCompSelectable csel:
                    RefreshSelectableItem(csel._item);
                    EditorUtility.SetDirty(csel);
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

        static void RefreshSelectableLocalization(SelectableLocalization bl)
        {
            if (bl._itemsButton != null)
            {
                foreach (var item in bl._itemsButton)
                {
                    RefreshSelectableItem(item);
                }
            }
            EditorUtility.SetDirty(bl);
        }

        static void RefreshSelectableItem(LocalizationItemSelectable item)
        {
            if (item == null) return;

#pragma warning disable CS0618
            if (item._item)
            {
                EditorUtility.SetDirty(item._item);
                RefreshGraphic(item._item.targetGraphic);
            }
#pragma warning restore CS0618

            if (item._itemS)
            {
                EditorUtility.SetDirty(item._itemS);
                RefreshGraphic(item._itemS.targetGraphic);
            }
        }

        static void RefreshGameObjectItem(LocalizationItemGameObject item)
        {
            if (item == null || !item.TryGetAllDatas(out var allDatas) || allDatas == null) return;

            foreach (var data in allDatas)
            {
                if (data?._gameObject)
                {
                    EditorUtility.SetDirty(data._gameObject);
                }
            }
        }

        static void RefreshUnityEventTargets(UnityEventBase evt)
        {
            if (evt == null) return;
            for (int i = 0; i < evt.GetPersistentEventCount(); i++)
            {
                switch (evt.GetPersistentTarget(i))
                {
                    case Graphic g when g:
                        RefreshGraphic(g);
                        break;
                    case Component c when c:
                        EditorUtility.SetDirty(c);
                        break;
                    case GameObject go when go:
                        EditorUtility.SetDirty(go);
                        break;
                }
            }
        }
    }
}