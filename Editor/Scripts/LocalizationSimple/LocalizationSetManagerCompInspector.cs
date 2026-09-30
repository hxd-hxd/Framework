using System.Collections.Generic;
using Framework.Localization;
using Framework.LocalizationSimple;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.Editor
{
    /// <summary>
    /// <see cref="LocalizationSetManagerComp"/> 检视面板：
    /// 显示 <see cref="LocalizationSetManager"/> 已注册列表，并提供 Set 按钮设置本地化。
    /// </summary>
    [CustomEditor(typeof(LocalizationSetManagerComp))]
    public class LocalizationSetManagerCompInspector : UnityEditor.Editor
    {
        const string SetUndoName = "设置本地化";

        LocalizationSetManagerComp my => (LocalizationSetManagerComp)target;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("LocalizationSetManager", EditorStyles.boldLabel);

            DrawRegisteredSets(LocalizationSetManager.Instance);

            EditorGUILayout.Space();
            DrawSetButton();
        }

        static void DrawRegisteredSets(LocalizationSetManager manager)
        {
            EditorGUILayout.LabelField("Count", manager.count.ToString());

            var sets = manager.sets;
            if (sets == null || sets.Count == 0)
            {
                EditorGUILayout.HelpBox("当前无已注册的 LocalizationSet", MessageType.Info);
                return;
            }

            EditorGUI.BeginDisabledGroup(true);
            for (int i = 0; i < sets.Count; i++)
            {
                var set = sets[i];
                string typeName = set != null ? set.GetType().FullName : "null";
                string label = $"[{i}] {typeName}";
                if (set is Object unityObj)
                    EditorGUILayout.ObjectField(label, unityObj, typeof(Object), true);
                else
                    EditorGUILayout.LabelField(label);
            }
            EditorGUI.EndDisabledGroup();
        }

        void DrawSetButton()
        {
            var sets = CollectSetsToApply();
            bool canSet = sets.Count > 0;

            EditorGUI.BeginDisabledGroup(!canSet);
            if (GUILayout.Button("设置本地化"))
                SetLocalization(sets);
            EditorGUI.EndDisabledGroup();

            if (!canSet)
                EditorGUILayout.HelpBox("当前无可设置的 LocalizationSet，无法设置本地化", MessageType.Info);
        }

        /// <summary>
        /// 优先使用管理器已注册列表（跳过全局设置器）；
        /// 编辑器下若尚未注册，则回退到组件上的默认 LocalizationSet；
        /// 默认设置器若为全局设置器，再回退到场景中的非全局设置器。
        /// </summary>
        List<ILocalizationSet> CollectSetsToApply()
        {
            var result = new List<ILocalizationSet>();
            var registered = LocalizationSetManager.Instance.sets;
            if (registered != null)
            {
                foreach (var set in registered)
                {
                    if (set == null || result.Contains(set)) continue;
                    if (set is LocalizationSetBase registeredBase && registeredBase._isGolbalSetter)
                        continue;
                    result.Add(set);
                }
            }

            serializedObject.Update();
            var defaultSet = serializedObject.FindProperty("_defultLocalizationSet").objectReferenceValue as LocalizationSetBase;
            if (defaultSet != null && !defaultSet._isGolbalSetter && !result.Contains(defaultSet))
                result.Add(defaultSet);

            if (result.Count == 0)
            {
                foreach (var setBase in LocalizationSetInspectorBase.CollectManagedSets(defaultSet))
                    result.Add(setBase);
            }

            return result;
        }

        /// <summary>
        /// 编辑器版 <see cref="LocalizationSetManagerComp.Set"/>：
        /// 走同一套运行时 API（<see cref="ILocalizationSet.Set"/>），
        /// 并在修改前记录所有本地化 Unity 对象以支持撤销重做。
        /// </summary>
        void SetLocalization(List<ILocalizationSet> sets)
        {
            if (sets == null || sets.Count == 0) return;

            Undo.IncrementCurrentGroup();
            int undoGroup = Undo.GetCurrentGroup();

            var objects = new HashSet<Object>();
            var emptyListSets = new List<LocalizationSetBase>();
            foreach (var set in sets)
            {
                if (!(set is LocalizationSetBase setBase)) continue;

                bool setListEmpty = setBase._localizations == null || setBase._localizations.Count <= 0;
                if (setListEmpty)
                {
                    Undo.RecordObject(setBase, SetUndoName);
                    emptyListSets.Add(setBase);
                }

                foreach (var localization in setBase.localizations)
                    CollectLocalizationObjects(localization, objects);
            }

            foreach (var obj in objects)
                Undo.RecordObject(obj, SetUndoName);

            var fallbackProviders = my.defultLangProviders;
            foreach (var set in sets)
                ApplySet(set, fallbackProviders);

            foreach (var setBase in emptyListSets)
            {
                EditorUtility.SetDirty(setBase);
                RecordPrefabModifications(setBase);
            }

            foreach (var obj in objects)
            {
                EditorUtility.SetDirty(obj);
                RecordPrefabModifications(obj);
            }

            Undo.SetCurrentGroupName(SetUndoName);
            Undo.CollapseUndoOperations(undoGroup);

            Canvas.ForceUpdateCanvases();
            SceneView.RepaintAll();
            InternalEditorUtility.RepaintAllViews();
            EditorApplication.QueuePlayerLoopUpdate();
        }

        /// <summary>
        /// 调用运行时 <see cref="ILocalizationSet.Set"/>。
        /// Provider 模式若未配置语言提供者，则临时使用组件上的默认提供者，避免编辑器下空列表空引用。
        /// </summary>
        static void ApplySet(ILocalizationSet set, List<LanguageProviderComponentBase> fallbackProviders)
        {
            if (set == null) return;

            if (!(set is LocalizationSetBase setBase))
            {
                set.Set();
                return;
            }

            if (setBase._isGolbalSetter)
                return;

            if (setBase._setMode == LocalizationSetMode.Provider)
            {
                var providers = setBase._langProviders;
                if (providers == null || providers.Count == 0)
                    providers = fallbackProviders;
                if (providers == null || providers.Count == 0)
                    return;

                if (!ReferenceEquals(providers, setBase._langProviders))
                {
                    var old = setBase._langProviders;
                    setBase._langProviders = providers;
                    try
                    {
                        setBase.Set();
                    }
                    finally
                    {
                        setBase._langProviders = old;
                    }
                    return;
                }
            }

            setBase.Set();
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
