// -------------------------
// 创建日期：2026/9/15 17:10:00
// -------------------------

using System;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Framework.Editor
{
    /// <summary>类型选择窗口：按命名空间树状列出类型，支持搜索。</summary>
    public class TypeSelectWindow : EditorWindow
    {
        UnityEngine.Object[] _targets;
        string _propertyPath;
        bool _arrayOnly;
        string _currentValue;
        bool _showSystemTypes;
        bool _excludeReferenceTypes;
        bool _excludeValueTypes;
        bool _excludeAbstractClasses;

        Type _selectedType;
        TreeViewState _treeState;
        TypeSelectTreeView _treeView;
        SearchField _searchField;

        public static void Open(SerializedProperty property, bool arrayOnly)
        {
            if (property == null)
                return;

            string title = arrayOnly ? "选择数组类型" : "选择类型";
            var window = GetWindow<TypeSelectWindow>(true, title, true);
            window.minSize = new Vector2(420, 520);
            window.Init(
                property.serializedObject.targetObjects,
                property.propertyPath,
                property.stringValue,
                arrayOnly);
            window.ShowUtility();
            window.Focus();
        }

        void Init(UnityEngine.Object[] targets, string propertyPath, string currentValue, bool arrayOnly)
        {
            _targets = targets;
            _propertyPath = propertyPath;
            _currentValue = currentValue;
            _arrayOnly = arrayOnly;
            _searchField = new SearchField();
            _treeState = new TreeViewState();

            Type current = ResolveElementType(currentValue);
            _showSystemTypes = current != null
                && TypeSelectCache.IsSystemType(current)
                && !TypeSelectCache.IsAlwaysVisible(current);

            _treeView = new TypeSelectTreeView(_treeState, _showSystemTypes, OnPicked, OnConfirmed);
            _treeView.SetFilters(
                _showSystemTypes,
                _excludeReferenceTypes,
                _excludeValueTypes,
                _excludeAbstractClasses);
            _treeView.Reload();
            _selectedType = current;

            if (current != null)
                _treeView.SelectType(current);
            else
                _treeView.ExpandTopLevelIfFew(3);

            _searchField.downOrUpArrowKeyPressed -= _treeView.SetFocusAndEnsureSelectedItem;
            _searchField.downOrUpArrowKeyPressed += _treeView.SetFocusAndEnsureSelectedItem;
            Repaint();
        }

        void OnDisable()
        {
            if (_searchField != null && _treeView != null)
                _searchField.downOrUpArrowKeyPressed -= _treeView.SetFocusAndEnsureSelectedItem;
        }

        void OnGUI()
        {
            if (_targets == null || _targets.Length == 0 || _targets[0] == null)
            {
                EditorGUILayout.HelpBox("原对象已失效，请重新打开选择窗口。", MessageType.Warning);
                if (GUILayout.Button("关闭"))
                    Close();
                return;
            }

            HandleKeys();
            DrawToolbar();
            DrawTree();
            DrawFooter();
        }

        void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("搜索", GUILayout.Width(32));
            if (_treeView != null)
            {
                _treeView.searchString = _searchField.OnToolbarGUI(_treeView.searchString);
            }

            EditorGUILayout.EndHorizontal();

            DrawFilters();

            if (_arrayOnly)
                EditorGUILayout.HelpBox("选择元素类型，确认后写入对应的一维数组类型全名。", MessageType.Info);
        }

        void DrawFilters()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginHorizontal();
            _showSystemTypes = EditorGUILayout.ToggleLeft("显示系统类型", _showSystemTypes, GUILayout.MinWidth(120));
            _excludeAbstractClasses = EditorGUILayout.ToggleLeft("排除抽象类", _excludeAbstractClasses, GUILayout.MinWidth(120));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            _excludeReferenceTypes = EditorGUILayout.ToggleLeft("排除引用类型", _excludeReferenceTypes, GUILayout.MinWidth(120));
            _excludeValueTypes = EditorGUILayout.ToggleLeft("排除值类型", _excludeValueTypes, GUILayout.MinWidth(120));
            EditorGUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck() && _treeView != null)
            {
                Type select = _selectedType ?? ResolveElementType(_currentValue);
                _treeView.SetFilters(
                    _showSystemTypes,
                    _excludeReferenceTypes,
                    _excludeValueTypes,
                    _excludeAbstractClasses);
                _treeView.SelectType(select);
                if (select != null)
                    _selectedType = select;
            }

            if (_excludeReferenceTypes && _excludeValueTypes)
                EditorGUILayout.HelpBox("引用类型和值类型均已排除，当前没有可选类型。", MessageType.Info);
        }

        void DrawTree()
        {
            Rect rect = GUILayoutUtility.GetRect(0, 0, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            _treeView?.OnGUI(rect);
        }

        void DrawFooter()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label(GetSelectionLabel(), EditorStyles.miniLabel);
            GUILayout.FlexibleSpace();

            using (new EditorGUI.DisabledScope(_selectedType == null))
            {
                if (GUILayout.Button("确定", GUILayout.Width(72)))
                    Confirm(_selectedType);
            }

            if (GUILayout.Button("取消", GUILayout.Width(72)))
                Close();

            EditorGUILayout.EndHorizontal();
        }

        void HandleKeys()
        {
            UnityEngine.Event e = UnityEngine.Event.current;
            if (e.type != EventType.KeyDown)
                return;

            if (e.keyCode == KeyCode.Escape)
            {
                Close();
                e.Use();
                return;
            }

            if ((e.keyCode == KeyCode.Return || e.keyCode == KeyCode.KeypadEnter)
                && _selectedType != null
                && _treeView != null
                && _treeView.HasFocus())
            {
                Confirm(_selectedType);
                e.Use();
            }
        }

        void OnPicked(Type type)
        {
            _selectedType = type;
            Repaint();
        }

        void OnConfirmed(Type type)
        {
            Confirm(type);
        }

        void Confirm(Type type)
        {
            if (type == null)
                return;

            string fullName;
            try
            {
                fullName = _arrayOnly ? type.MakeArrayType().FullName : type.FullName;
            }
            catch (ArgumentException)
            {
                Debug.LogWarning("无法从类型创建数组：" + type.FullName);
                return;
            }
            Apply(fullName);
            var window = this;
            EditorApplication.delayCall += () =>
            {
                if (window)
                    window.Close();
            };
        }

        void Apply(string fullName)
        {
            if (_targets == null || _targets.Length == 0 || string.IsNullOrEmpty(_propertyPath))
                return;

            var so = new SerializedObject(_targets);
            SerializedProperty property = so.FindProperty(_propertyPath);
            if (property == null)
                return;

            property.stringValue = fullName;
            so.ApplyModifiedProperties();
        }

        string GetSelectionLabel()
        {
            if (_selectedType == null)
                return "未选择类型";

            string fullName;
            try
            {
                fullName = _arrayOnly ? _selectedType.MakeArrayType().FullName : _selectedType.FullName;
            }
            catch (ArgumentException)
            {
                return _selectedType.FullName + "（无法作为数组元素）";
            }
            return "将保存为 " + fullName;
        }

        static Type ResolveElementType(string typeName)
        {
            Type type = TypeSelectCache.Find(typeName);
            if (type != null && type.IsArray)
                return type.GetElementType();
            return type;
        }
    }
}
