using System.Collections.Generic;
using Framework.Localization;
using Framework.LocalizationSimple;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.Editor
{
    /// <summary>
    /// <see cref="LocalizationSetDataProvider"/> 检视面板：
    /// 显示 <see cref="LocalizationSetManager"/> 当前全局数据提供者，并提供 SetToGlobal / SetToItem 按钮。
    /// </summary>
    [CustomEditor(typeof(LocalizationSetDataProvider))]
    public class LocalizationSetDataProviderInspector : UnityEditor.Editor
    {
        const string SetToItemUndoName = "设置数据提供者到所有项";

        LocalizationSetDataProvider my => (LocalizationSetDataProvider)target;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("LocalizationSetManager", EditorStyles.boldLabel);

            DrawCurrentGlobalDataProvider(LocalizationSetManager.Instance.globalDataProvider);

            EditorGUILayout.Space();
            if (GUILayout.Button("设置数据提供者到全局"))
            {
                my.SetToGlobal();
            }

            DrawSetToItemButton();
        }

        static void DrawCurrentGlobalDataProvider(ILocalizationDataProvider provider)
        {
            if (provider == null)
            {
                EditorGUILayout.HelpBox("当前无全局数据提供者", MessageType.Info);
                return;
            }

            EditorGUI.BeginDisabledGroup(true);
            if (provider is Object unityObj)
            {
                EditorGUILayout.ObjectField("当前全局数据提供者", unityObj, typeof(Object), true);
            }
            else
            {
                EditorGUILayout.LabelField("当前全局数据提供者", provider.GetType().FullName);
            }
            EditorGUI.EndDisabledGroup();
        }

        void DrawSetToItemButton()
        {
            serializedObject.Update();
            var set = serializedObject.FindProperty("_localizationSet").objectReferenceValue as LocalizationSetBase;

            EditorGUI.BeginDisabledGroup(set == null);
            if (GUILayout.Button("设置数据提供者到所有项"))
                SetDataProviderToAllItems(set);
            EditorGUI.EndDisabledGroup();

            if (set == null)
                EditorGUILayout.HelpBox("未指定 LocalizationSet，无法设置到项", MessageType.Info);
        }

        /// <summary>
        /// 编辑器版 <see cref="LocalizationSetDataProvider.SetToItem"/>：
        /// 走同一套运行时 API（<see cref="ILocalization.GetAllItem"/> / <see cref="ILocalizationItem.dataProvider"/>），
        /// 并在修改前记录所有持有项的 Unity 对象以支持撤销重做。
        /// </summary>
        void SetDataProviderToAllItems(LocalizationSetBase set)
        {
            if (set == null) return;

            Undo.IncrementCurrentGroup();
            int undoGroup = Undo.GetCurrentGroup();

            bool setListEmpty = set._localizations == null || set._localizations.Count <= 0;
            if (setListEmpty)
                Undo.RecordObject(set, SetToItemUndoName);

            var objects = new HashSet<Object>();
            foreach (var localization in set.localizations)
                CollectLocalizationObjects(localization, objects);

            foreach (var obj in objects)
                Undo.RecordObject(obj, SetToItemUndoName);

            my.SetToItem();

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

            Undo.SetCurrentGroupName(SetToItemUndoName);
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
