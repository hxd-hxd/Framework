// -------------------------
// 创建日期：2026/9/15 17:10:00
// -------------------------

using System;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Framework.Editor
{
    /// <summary>类型选择窗口：按命名空间树状列出类型，支持搜索与泛型拼装。</summary>
    public class TypeSelectWindow : EditorWindow
    {
        const int MaxGenericDepth = 3;

        UnityEngine.Object[] _targets;
        string _propertyPath;
        Action<Type> _onSelected;

        bool _arrayOnly;
        bool _allowWrapAsArray;
        bool _wrapAsArray;
        int _depth;
        Type _constraintParam;
        string _currentValue;

        bool _showSystemTypes;
        bool _excludeReferenceTypes;
        bool _excludeValueTypes;
        bool _excludeAbstractClasses;

        Type _treeType;
        Type[] _genericArgs;
        Type[] _stashedArgs;
        int[] _argLinks;
        string _error;

        TreeViewState _treeState;
        TypeSelectTreeView _treeView;
        SearchField _searchField;

        public static void Open(SerializedProperty property, bool arrayOnly)
        {
            if (property == null)
                return;

            string title = arrayOnly ? "选择数组类型" : "选择类型";
            TypeSelectWindow window = FindExactWindow();
            if (window == null)
                window = CreateInstance<TypeSelectWindow>();
            window.titleContent = new GUIContent(title);
            window.minSize = new Vector2(420, 560);
            window.InitProperty(
                property.serializedObject.targetObjects,
                property.propertyPath,
                property.stringValue,
                arrayOnly);
            window.ShowUtility();
            window.Focus();
        }

        public static void OpenForArgument(
            string title,
            Type genericParameter,
            int depth,
            Action<Type> onSelected)
        {
            if (onSelected == null)
                return;

            TypeSelectWindow window = CreateInstance<TypeSelectArgumentWindow>();
            window.titleContent = new GUIContent(string.IsNullOrEmpty(title) ? "选择类型参数" : title);
            window.minSize = new Vector2(420, 560);
            window.InitArgument(genericParameter, depth, onSelected);
            window.ShowUtility();
            window.Focus();
        }

        void InitProperty(UnityEngine.Object[] targets, string propertyPath, string currentValue, bool arrayOnly)
        {
            _targets = targets;
            _propertyPath = propertyPath;
            _currentValue = currentValue;
            _arrayOnly = arrayOnly;
            _onSelected = null;
            _allowWrapAsArray = false;
            _wrapAsArray = false;
            _depth = 0;
            _constraintParam = null;
            InitCore(ResolveElementType(currentValue));
        }

        void InitArgument(Type genericParameter, int depth, Action<Type> onSelected)
        {
            _targets = null;
            _propertyPath = null;
            _currentValue = null;
            _arrayOnly = false;
            _onSelected = onSelected;
            _allowWrapAsArray = true;
            _wrapAsArray = false;
            _depth = depth;
            _constraintParam = genericParameter;
            InitCore(null);
        }

        void InitCore(Type current)
        {
            _searchField = new SearchField();
            _treeState = new TreeViewState();
            _error = null;
            _stashedArgs = null;

            SetupCurrent(current);
            AutoEnableSystemTypes(current);

            _treeView = new TypeSelectTreeView(_treeState, _showSystemTypes, OnPicked, OnConfirmed);
            ApplyTreeSettings();
            _treeView.Reload();

            Type selectInTree = _treeType;
            if (selectInTree != null)
                _treeView.SelectType(selectInTree);
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
            if (!IsHostValid())
            {
                EditorGUILayout.HelpBox("原对象已失效，请重新打开选择窗口。", MessageType.Warning);
                if (GUILayout.Button("关闭"))
                    Close();
                return;
            }

            HandleKeys();
            DrawToolbar();
            DrawTree();
            DrawComposePanel();
            DrawFooter();
        }

        void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("搜索", GUILayout.Width(32));
            if (_treeView != null)
                _treeView.searchString = _searchField.OnToolbarGUI(_treeView.searchString);
            EditorGUILayout.EndHorizontal();

            DrawFilters();

            if (_arrayOnly)
                EditorGUILayout.HelpBox("选择元素类型，确认后写入对应的一维数组类型全名。", MessageType.Info);
            if (_allowWrapAsArray)
                _wrapAsArray = EditorGUILayout.ToggleLeft("作为数组 T[]", _wrapAsArray);
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
                ApplyTreeSettings();
                if (_treeType != null)
                    _treeView.SelectType(_treeType);
            }

            if (_excludeReferenceTypes && _excludeValueTypes)
                EditorGUILayout.HelpBox("引用类型和值类型均已排除，当前没有可选类型。", MessageType.Info);
        }

        void DrawTree()
        {
            Rect rect = GUILayoutUtility.GetRect(0, 0, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            _treeView?.OnGUI(rect);
        }

        void DrawComposePanel()
        {
            if (_treeType == null || !_treeType.IsGenericTypeDefinition || _genericArgs == null)
                return;

            Type[] parameters = _treeType.GetGenericArguments();
            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField("泛型参数  " + TypeSelectCache.GetDisplayName(_treeType), EditorStyles.boldLabel);

            for (int i = 0; i < parameters.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(parameters[i].Name, GUILayout.Width(72));

                bool linked = _argLinks != null && i < _argLinks.Length && _argLinks[i] >= 0;
                if (linked)
                    _genericArgs[i] = _genericArgs[_argLinks[i]];

                string argLabel = _genericArgs[i] != null
                    ? TypeSelectCache.GetDisplayName(_genericArgs[i])
                    : "未选择";
                EditorGUILayout.LabelField(argLabel, EditorStyles.helpBox);

                using (new EditorGUI.DisabledScope(linked))
                {
                    if (GUILayout.Button("选择", GUILayout.Width(48)))
                        OpenArgumentPicker(i, parameters[i]);
                }

                if (parameters.Length >= 2 && i > 0)
                    DrawArgLink(i, parameters);

                EditorGUILayout.EndHorizontal();
            }

            if (!string.IsNullOrEmpty(_error))
                EditorGUILayout.HelpBox(_error, MessageType.Error);
        }

        void DrawArgLink(int index, Type[] parameters)
        {
            int optionCount = index + 1;
            var options = new string[optionCount];
            options[0] = "独立选择";
            for (int s = 0; s < index; s++)
                options[s + 1] = "与 " + parameters[s].Name + " 相同";

            int current = _argLinks[index] < 0 ? 0 : _argLinks[index] + 1;
            int next = EditorGUILayout.Popup(current, options, GUILayout.Width(128));
            int link = next == 0 ? -1 : next - 1;
            if (link == _argLinks[index])
                return;

            _argLinks[index] = link;
            if (link >= 0)
                _genericArgs[index] = _genericArgs[link];
            StashCurrentArgs();
            _error = null;
        }

        GUIStyle _footerLabelStyle;

        GUIStyle FooterLabelStyle
        {
            get
            {
                if (_footerLabelStyle == null)
                {
                    _footerLabelStyle = new GUIStyle(EditorStyles.miniLabel)
                    {
                        wordWrap = true,
                        alignment = TextAnchor.UpperLeft,
                    };
                }

                return _footerLabelStyle;
            }
        }

        void DrawFooter()
        {
            const float ButtonWidth = 72f;
            const float ButtonSpacing = 4f;
            const int MaxLines = 3;

            string label = GetSelectionLabel();
            float width = Mathf.Max(40f, position.width - 16f);
            var content = new GUIContent(InsertWrapHints(label), label);
            float height = FooterLabelStyle.CalcHeight(content, width);
            float maxHeight = EditorGUIUtility.singleLineHeight * MaxLines;
            EditorGUILayout.LabelField(content, FooterLabelStyle, GUILayout.Height(Mathf.Min(height, maxHeight)));

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            bool canConfirm = TryBuildResult(out _, out _);
            using (new EditorGUI.DisabledScope(!canConfirm))
            {
                if (GUILayout.Button("确定", GUILayout.Width(ButtonWidth)))
                    Confirm();
            }

            GUILayout.Space(ButtonSpacing);
            if (GUILayout.Button("取消", GUILayout.Width(ButtonWidth)))
                Close();

            EditorGUILayout.EndHorizontal();
        }

        static string InsertWrapHints(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            return text
                .Replace(",", ",\u200B")
                .Replace(".", ".\u200B")
                .Replace("[", "[\u200B")
                .Replace("`", "`\u200B")
                .Replace("+", "+\u200B");
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
                && _treeView != null
                && _treeView.HasFocus()
                && TryBuildResult(out _, out _))
            {
                Confirm();
                e.Use();
            }
        }

        void OnPicked(Type type)
        {
            _error = null;
            if (type != null && type.IsGenericTypeDefinition)
            {
                if (_treeType != type)
                    BeginCompose(type);
                _treeType = type;
                Repaint();
                return;
            }

            StashCurrentArgs();
            _treeType = type;
            _genericArgs = null;
            _argLinks = null;
            Repaint();
        }

        void OnConfirmed(Type type)
        {
            if (type != null && type.IsGenericTypeDefinition)
                return;
            OnPicked(type);
            Confirm();
        }

        void OpenArgumentPicker(int index, Type genericParameter)
        {
            var host = this;
            string title = "选择 " + TypeSelectCache.GetDisplayName(_treeType) + " 的 " + genericParameter.Name;
            OpenForArgument(title, genericParameter, _depth + 1, selected =>
            {
                if (host == null)
                    return;
                if (host._genericArgs == null || index < 0 || index >= host._genericArgs.Length)
                    return;
                host._genericArgs[index] = selected;
                if (host._argLinks != null && index < host._argLinks.Length)
                    host._argLinks[index] = -1;
                host.SyncLinkedArgs();
                host.StashCurrentArgs();
                host._error = null;
                host.Repaint();
            });
        }

        void Confirm()
        {
            if (!TryBuildResult(out Type result, out string error))
            {
                _error = error;
                Repaint();
                return;
            }

            if (_onSelected != null)
                _onSelected(result);
            else
                Apply(result.FullName);

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

        bool TryBuildResult(out Type result, out string error)
        {
            result = null;
            error = null;

            if (_treeType == null)
            {
                error = "未选择类型";
                return false;
            }

            if (_treeType.IsGenericTypeDefinition)
            {
                SyncLinkedArgs();
                if (!TypeSelectCache.TryMakeClosedType(_treeType, _genericArgs, out result, out error))
                    return false;
            }
            else
            {
                result = _treeType;
            }

            if (_arrayOnly || _wrapAsArray)
            {
                try
                {
                    result = result.MakeArrayType();
                }
                catch (ArgumentException e)
                {
                    error = e.Message;
                    result = null;
                    return false;
                }
            }

            if (_constraintParam != null && !TypeSelectCache.MatchesGenericParameter(result, _constraintParam))
            {
                error = TypeSelectCache.GetDisplayName(result) + " 不满足约束";
                result = null;
                return false;
            }

            return result != null && !string.IsNullOrEmpty(result.FullName);
        }

        string GetSelectionLabel()
        {
            if (!TryBuildResult(out Type result, out string error))
                return string.IsNullOrEmpty(error) ? "未选择类型" : error;
            return "将保存为 " + result.FullName;
        }

        void SetupCurrent(Type current)
        {
            _genericArgs = null;
            _argLinks = null;
            _treeType = null;
            if (current == null)
                return;

            if (current.IsGenericType && !current.IsGenericTypeDefinition)
            {
                _treeType = current.GetGenericTypeDefinition();
                Type[] args = current.GetGenericArguments();
                _genericArgs = (Type[])args.Clone();
                _argLinks = CreateIndependentLinks(_genericArgs.Length);
                StashCurrentArgs();
                return;
            }

            if (current.IsGenericTypeDefinition)
            {
                BeginCompose(current);
                _treeType = current;
                return;
            }

            _treeType = current;
        }

        void BeginCompose(Type definition)
        {
            StashCurrentArgs();
            int count = definition.GetGenericArguments().Length;
            _genericArgs = new Type[count];
            _argLinks = CreateIndependentLinks(count);
            ApplyStashedArgs(definition);
        }

        void StashCurrentArgs()
        {
            if (_genericArgs == null || _genericArgs.Length == 0)
                return;

            int length = _stashedArgs != null
                ? Math.Max(_stashedArgs.Length, _genericArgs.Length)
                : _genericArgs.Length;
            var next = new Type[length];
            if (_stashedArgs != null)
                Array.Copy(_stashedArgs, next, _stashedArgs.Length);

            for (int i = 0; i < _genericArgs.Length; i++)
            {
                if (_genericArgs[i] != null)
                    next[i] = _genericArgs[i];
            }

            _stashedArgs = next;
        }

        void ApplyStashedArgs(Type definition)
        {
            if (_stashedArgs == null || _genericArgs == null)
                return;

            Type[] parameters = definition.GetGenericArguments();
            int count = Math.Min(_stashedArgs.Length, _genericArgs.Length);
            for (int i = 0; i < count; i++)
            {
                Type arg = _stashedArgs[i];
                if (arg == null)
                    continue;
                if (TypeSelectCache.MatchesGenericParameter(arg, parameters[i]))
                    _genericArgs[i] = arg;
            }
        }

        void SyncLinkedArgs()
        {
            if (_genericArgs == null || _argLinks == null)
                return;

            for (int i = 0; i < _genericArgs.Length && i < _argLinks.Length; i++)
            {
                int link = _argLinks[i];
                if (link >= 0 && link < i)
                    _genericArgs[i] = _genericArgs[link];
            }
        }

        void ApplyTreeSettings()
        {
            _treeView.ApplyViewSettings(
                _showSystemTypes,
                _excludeReferenceTypes,
                _excludeValueTypes,
                _excludeAbstractClasses,
                _constraintParam,
                _depth < MaxGenericDepth);
        }

        void AutoEnableSystemTypes(Type current)
        {
            Type probe = current;
            if (probe != null && probe.IsGenericType && !probe.IsGenericTypeDefinition)
                probe = probe.GetGenericTypeDefinition();

            if (probe != null
                && TypeSelectCache.IsSystemType(probe)
                && !TypeSelectCache.IsAlwaysVisible(probe))
                _showSystemTypes = true;

            if (_constraintParam == null)
                return;

            Type[] constraints = _constraintParam.GetGenericParameterConstraints();
            for (int i = 0; i < constraints.Length; i++)
            {
                if (TypeSelectCache.IsSystemType(constraints[i])
                    && !TypeSelectCache.IsAlwaysVisible(constraints[i]))
                {
                    _showSystemTypes = true;
                    break;
                }
            }
        }

        bool IsHostValid()
        {
            if (_onSelected != null)
                return true;
            return _targets != null && _targets.Length > 0 && _targets[0] != null;
        }

        static int[] CreateIndependentLinks(int count)
        {
            var links = new int[count];
            for (int i = 0; i < count; i++)
                links[i] = -1;
            return links;
        }

        static TypeSelectWindow FindExactWindow()
        {
            TypeSelectWindow[] windows = Resources.FindObjectsOfTypeAll<TypeSelectWindow>();
            for (int i = 0; i < windows.Length; i++)
            {
                TypeSelectWindow window = windows[i];
                if (window != null && window.GetType() == typeof(TypeSelectWindow))
                    return window;
            }

            return null;
        }

        static Type ResolveElementType(string typeName)
        {
            Type type = TypeSelectCache.Find(typeName);
            if (type != null && type.IsArray)
                return type.GetElementType();
            return type;
        }
    }

    public class TypeSelectArgumentWindow : TypeSelectWindow
    {
    }
}
