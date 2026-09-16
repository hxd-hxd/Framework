// -------------------------
// 创建日期：2026/9/15 17:10:00
// -------------------------

using System;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;

namespace Framework.Editor
{
    /// <summary>按命名空间分级的类型树。</summary>
    public class TypeSelectTreeView : TreeView
    {
        public class Item : TreeViewItem
        {
            public Type type;
            public string fullName;
            public string searchText;
        }

        readonly Action<Type> _onPicked;
        readonly Action<Type> _onConfirmed;
        readonly Dictionary<Type, Item> _typeItems = new Dictionary<Type, Item>();

        bool _showSystemTypes;
        bool _excludeReferenceTypes;
        bool _excludeValueTypes;
        bool _excludeAbstractClasses;
        Type _constraintParam;
        bool _allowGenericDefinitions = true;
        int _idSeed = 1;

        public TypeSelectTreeView(TreeViewState state, bool showSystemTypes, Action<Type> onPicked, Action<Type> onConfirmed)
            : base(state)
        {
            _showSystemTypes = showSystemTypes;
            _onPicked = onPicked;
            _onConfirmed = onConfirmed;
            showBorder = true;
            showAlternatingRowBackgrounds = true;
        }

        public bool showSystemTypes
        {
            get => _showSystemTypes;
            set => SetFilter(ref _showSystemTypes, value);
        }

        public bool excludeReferenceTypes
        {
            get => _excludeReferenceTypes;
            set => SetFilter(ref _excludeReferenceTypes, value);
        }

        public bool excludeValueTypes
        {
            get => _excludeValueTypes;
            set => SetFilter(ref _excludeValueTypes, value);
        }

        public bool excludeAbstractClasses
        {
            get => _excludeAbstractClasses;
            set => SetFilter(ref _excludeAbstractClasses, value);
        }

        public void SetFilters(bool showSystemTypes, bool excludeReferenceTypes, bool excludeValueTypes, bool excludeAbstractClasses)
        {
            ApplyViewSettings(
                showSystemTypes,
                excludeReferenceTypes,
                excludeValueTypes,
                excludeAbstractClasses,
                _constraintParam,
                _allowGenericDefinitions);
        }

        public void ApplyViewSettings(
            bool showSystemTypes,
            bool excludeReferenceTypes,
            bool excludeValueTypes,
            bool excludeAbstractClasses,
            Type constraintParam,
            bool allowGenericDefinitions)
        {
            bool changed = _showSystemTypes != showSystemTypes
                || _excludeReferenceTypes != excludeReferenceTypes
                || _excludeValueTypes != excludeValueTypes
                || _excludeAbstractClasses != excludeAbstractClasses
                || _constraintParam != constraintParam
                || _allowGenericDefinitions != allowGenericDefinitions;
            if (!changed)
                return;

            _showSystemTypes = showSystemTypes;
            _excludeReferenceTypes = excludeReferenceTypes;
            _excludeValueTypes = excludeValueTypes;
            _excludeAbstractClasses = excludeAbstractClasses;
            _constraintParam = constraintParam;
            _allowGenericDefinitions = allowGenericDefinitions;
            Reload();
        }

        public Type selectedType
        {
            get
            {
                IList<int> selection = GetSelection();
                if (selection == null || selection.Count == 0)
                    return null;
                var item = FindItem(selection[0], rootItem) as Item;
                return item != null ? item.type : null;
            }
        }

        public void SelectType(Type type)
        {
            if (type == null)
                return;
            if (!_typeItems.TryGetValue(type, out Item item))
                return;

            SetSelection(new[] { item.id }, TreeViewSelectionOptions.RevealAndFrame);
            FrameItem(item.id);
        }

        protected override bool CanMultiSelect(TreeViewItem item) => false;

        protected override TreeViewItem BuildRoot()
        {
            _idSeed = 1;
            _typeItems.Clear();

            var root = new Item { id = 0, depth = -1, displayName = "Root" };
            var folders = new Dictionary<string, Item>(StringComparer.Ordinal);

            IReadOnlyList<TypeSelectCache.Entry> entries = TypeSelectCache.GetEntries();
            for (int i = 0; i < entries.Count; i++)
            {
                TypeSelectCache.Entry entry = entries[i];
                if (!ShouldShow(entry))
                    continue;

                AddTypeItem(root, folders, entry);
            }

            if (!root.hasChildren)
            {
                root.AddChild(new Item
                {
                    id = _idSeed++,
                    displayName = "(无类型)",
                });
            }

            SortRecursive(root);
            SetupDepthsFromParentsAndChildren(root);
            return root;
        }

        public void ExpandTopLevelIfFew(int maxCount)
        {
            if (rootItem == null || !rootItem.hasChildren)
                return;
            if (rootItem.children.Count > maxCount)
                return;
            for (int i = 0; i < rootItem.children.Count; i++)
                SetExpanded(rootItem.children[i].id, true);
        }

        protected override bool DoesItemMatchSearch(TreeViewItem item, string search)
        {
            if (string.IsNullOrEmpty(search))
                return true;

            var typed = item as Item;
            if (typed == null || typed.type == null)
                return false;

            return typed.searchText != null
                && typed.searchText.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            Type type = null;
            if (selectedIds != null && selectedIds.Count > 0)
            {
                var item = FindItem(selectedIds[0], rootItem) as Item;
                type = item != null ? item.type : null;
            }

            _onPicked?.Invoke(type);
        }

        protected override void DoubleClickedItem(int id)
        {
            var item = FindItem(id, rootItem) as Item;
            if (item != null && item.type != null)
                _onConfirmed?.Invoke(item.type);
        }

        bool ShouldShow(TypeSelectCache.Entry entry)
        {
            Type type = entry.type;
            if (type == null)
                return false;

            if (!_showSystemTypes && entry.isSystem && !TypeSelectCache.IsAlwaysVisible(type))
                return false;
            if (_excludeReferenceTypes && !type.IsValueType)
                return false;
            if (_excludeValueTypes && type.IsValueType)
                return false;
            if (_excludeAbstractClasses && IsAbstractClass(type))
                return false;
            if (!_allowGenericDefinitions && type.IsGenericTypeDefinition)
                return false;
            if (_constraintParam != null && !TypeSelectCache.MatchesGenericParameter(type, _constraintParam))
                return false;
            return true;
        }

        static bool IsAbstractClass(Type type)
        {
            return type.IsClass && type.IsAbstract;
        }

        void SetFilter(ref bool field, bool value)
        {
            if (field == value)
                return;
            field = value;
            Reload();
        }

        void AddTypeItem(Item root, Dictionary<string, Item> folders, TypeSelectCache.Entry entry)
        {
            Item parent = GetNamespaceFolder(root, folders, entry.type);
            parent = EnsureDeclaringChain(parent, entry.type);
            CreateTypeItem(parent, entry.type, entry);
        }

        Item GetNamespaceFolder(Item root, Dictionary<string, Item> folders, Type type)
        {
            string ns = type.Namespace;
            if (string.IsNullOrEmpty(ns))
                return GetOrCreateNode(folders, root, "(global)", "(global)");

            Item parent = root;
            string path = string.Empty;
            string[] parts = ns.Split('.');
            for (int i = 0; i < parts.Length; i++)
            {
                path = path.Length == 0 ? parts[i] : path + "." + parts[i];
                parent = GetOrCreateNode(folders, parent, path, parts[i]);
            }

            return parent;
        }

        Item EnsureDeclaringChain(Item namespaceFolder, Type type)
        {
            Type declaring = type.DeclaringType;
            if (declaring == null)
                return namespaceFolder;

            var stack = new Stack<Type>();
            while (declaring != null)
            {
                stack.Push(declaring);
                declaring = declaring.DeclaringType;
            }

            Item parent = namespaceFolder;
            while (stack.Count > 0)
            {
                Type outer = stack.Pop();
                parent = CreateTypeItem(parent, outer, default);
            }

            return parent;
        }

        Item CreateTypeItem(Item parent, Type type, TypeSelectCache.Entry entry)
        {
            if (_typeItems.TryGetValue(type, out Item exist))
            {
                if (exist.type == null)
                    exist.type = type;
                if (string.IsNullOrEmpty(exist.fullName))
                    exist.fullName = entry.fullName ?? type.FullName;
                if (!string.IsNullOrEmpty(entry.name))
                    exist.searchText = BuildSearchText(entry);
                return exist;
            }

            string fullName = entry.fullName ?? type.FullName;
            var item = new Item
            {
                id = _idSeed++,
                displayName = TypeSelectCache.GetDisplayName(type),
                type = type,
                fullName = fullName,
                searchText = string.IsNullOrEmpty(entry.name)
                    ? type.Name + " " + fullName
                    : BuildSearchText(entry),
            };
            parent.AddChild(item);
            _typeItems[type] = item;
            return item;
        }

        Item GetOrCreateNode(Dictionary<string, Item> folders, Item parent, string path, string displayName)
        {
            if (folders.TryGetValue(path, out Item exist))
                return exist;

            var node = new Item
            {
                id = _idSeed++,
                displayName = displayName,
                fullName = path,
                searchText = path,
            };
            parent.AddChild(node);
            folders[path] = node;
            return node;
        }

        static string BuildSearchText(TypeSelectCache.Entry entry)
        {
            if (string.IsNullOrEmpty(entry.ns))
                return entry.name + " " + entry.fullName;
            return entry.name + " " + entry.ns + " " + entry.fullName;
        }

        static void SortRecursive(TreeViewItem item)
        {
            if (item.children == null || item.children.Count == 0)
                return;

            item.children.Sort(CompareItems);
            for (int i = 0; i < item.children.Count; i++)
                SortRecursive(item.children[i]);
        }

        static int CompareItems(TreeViewItem a, TreeViewItem b)
        {
            var ia = a as Item;
            var ib = b as Item;
            bool aFolder = ia == null || ia.type == null;
            bool bFolder = ib == null || ib.type == null;
            if (aFolder != bFolder)
                return aFolder ? -1 : 1;
            return string.CompareOrdinal(a.displayName, b.displayName);
        }
    }
}
