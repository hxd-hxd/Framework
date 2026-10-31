using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Framework.Editor
{
    /// <summary>
    /// <see cref="DictionarySerializable{TKey,TValue}"/> 的检视面板。
    /// 编辑序列化列表，重复键和空键标红，不进入字典。
    /// 字段上的 TextArea、Range、Min、Delayed、ColorUsage、GradientUsage 等特性会转到类型匹配的键或值。
    /// 条目总高度超过一屏时分页显示，避免把检视面板拉得过长。
    /// </summary>
    [CustomPropertyDrawer(typeof(DictionarySerializable<,>), true)]
    public class DictionarySerializableDrawer : PropertyDrawer
    {
        const float Gap = 4f;
        const float PairGap = 8f;
        const float MaxPageBody = 700f;
        const float PagerHeight = 22f;
        const float PageInset = 10f;
        const float DragHandleWidth = 16f;
        static readonly int DragHint = "DictionarySerializableDrag".GetHashCode();
        const long FnvOffset = 1469598103934665603L;
        const long FnvPrime = 1099511628211L;
        const string ConflictMessage = "空键或重复键不会进入字典，重复时保留靠前的一项。";
        const string MultiEditMessage = "多个对象的字典内容不同，无法一起编辑。";

        static readonly GUIContent ConflictContent = new GUIContent(ConflictMessage);
        static readonly GUIContent MultiEditContent = new GUIContent(MultiEditMessage);
        static readonly GUIContent KeyHeader = new GUIContent("键");
        static readonly GUIContent ValueHeader = new GUIContent("值");
        static readonly Color ConflictTint = new Color(1f, 0.65f, 0.65f);
        static Color PairSeparatorColor => EditorGUIUtility.isProSkin
            ? new Color(1f, 1f, 1f, 0.22f)
            : new Color(0f, 0f, 0f, 0.28f);
        static readonly Dictionary<Type, MethodInfo> s_equalsMethods = new Dictionary<Type, MethodInfo>();
        static readonly Dictionary<string, Type> s_resolvedTypes = new Dictionary<string, Type>();
        static readonly Dictionary<string, object> s_comparerInstances = new Dictionary<string, object>();
        static MethodInfo s_getHandler;
        static PropertyInfo s_hasPropertyDrawer;
        static bool s_handlerLookupFailed;
        static MethodInfo s_getFieldInfo;
        static bool s_fieldInfoLookupFailed;

        readonly Dictionary<string, Entry> _entries = new Dictionary<string, Entry>();
        float _layoutWidth;

        sealed class Entry
        {
            public ReorderableList list;
            public float layoutWidth;
            public object comparer;
            public Type keyType;
            public string comparerName;
            public bool shapeReady;
            public int keyPropertyType = int.MinValue;
            public int valuePropertyType = int.MinValue;
            public bool keyInline;
            public bool valueInline;
            public bool keyLeaf;
            public bool valueLeaf;
            public bool allLeaf;
            public bool keyCheckHidden;
            public bool valueCheckHidden;
            public FieldInfo fieldInfo;
            public bool keyForward;
            public bool valueForward;
            public int forwardBits = -1;
            public long keyFp = long.MinValue;
            public bool conflictsReady;
            public int conflictCount;
            public bool anyConflict;
            public long[] keyHashes = Array.Empty<long>();
            public bool[] absent = Array.Empty<bool>();
            public bool[] conflicts = Array.Empty<bool>();
            public long heightFp = long.MinValue;
            public bool heightsReady;
            public int heightCount;
            public bool measuredWide;
            public float constantHeight = -1f;
            public float[] heights = Array.Empty<float>();
            public int cursor = -1;
            public SerializedProperty cursorElement;
            public SerializedProperty cursorKey;
            public SerializedProperty cursorValue;
            public int page;
            public int pageCount = 1;
            public int[] pageStarts = Array.Empty<int>();
            public readonly List<int> pageStartBuffer = new List<int>();
            public int dragFrom = -1;
            public int dragTo = -1;
            public bool dragMoved;
            public Vector2 dragOrigin;
            public float dragGrab;
            public float animOrigin;
            public float[] animY = Array.Empty<float>();
            public double animStamp;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (position.width > 1f)
                _layoutWidth = position.width;

            label = EditorGUI.BeginProperty(position, label, property);
            try
            {
                var entries = property.FindPropertyRelative("_entries");
                float line = EditorGUIUtility.singleLineHeight;
                var foldRect = new Rect(position.x, position.y, position.width, line);

                if (entries == null)
                {
                    EditorGUI.LabelField(foldRect, label, new GUIContent("缺少序列化列表"));
                    return;
                }

                if (entries.hasMultipleDifferentValues)
                {
                    EditorGUI.LabelField(foldRect, label);
                    int indent = EditorGUI.indentLevel;
                    var area = EditorGUI.IndentedRect(foldRect);
                    EditorGUI.indentLevel = 0;
                    DrawMessage(area, area.width, MultiEditContent, MultiEditMessage, MessageType.Info);
                    EditorGUI.indentLevel = indent;
                    return;
                }

                property.isExpanded = EditorGUI.Foldout(foldRect, property.isExpanded, label, true);
                if (!property.isExpanded)
                    return;

                string id = CacheId(property);
                var entry = GetEntry(id);
                if (position.width > 1f)
                    entry.layoutWidth = position.width;

                var list = GetList(entry, entries, id);
                Refresh(entry, property, entries, ContentWidth(entry));
                int count = entries.arraySize;

                float y = foldRect.yMax + EditorGUIUtility.standardVerticalSpacing;
                int listIndent = EditorGUI.indentLevel;
                float listHeight = VisibleListHeight(entry, list, count);
                var listRect = EditorGUI.IndentedRect(new Rect(position.x, y, position.width, listHeight));
                EditorGUI.indentLevel = 0;
                entry.cursor = -1;
                entry.cursorKey = null;
                entry.cursorValue = null;
                if (count <= 0)
                    list.DoList(listRect);
                else
                    DrawPagedList(entry, list, id, listRect, count);

                if (entry.anyConflict)
                    DrawMessage(listRect, listRect.width, ConflictContent, ConflictMessage, MessageType.Warning);

                EditorGUI.indentLevel = listIndent;
            }
            finally
            {
                EditorGUI.EndProperty();
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;
            var entries = property.FindPropertyRelative("_entries");
            if (entries == null)
                return height;

            float width = _layoutWidth > 1f ? _layoutWidth : EditorGUIUtility.currentViewWidth;
            width = Mathf.Max(1f, width - EditorGUI.indentLevel * 15f);
            if (entries.hasMultipleDifferentValues)
                return height + EditorGUIUtility.standardVerticalSpacing + MessageHeight(MultiEditContent, width);

            if (!property.isExpanded)
                return height;

            string id = CacheId(property);
            var entry = GetEntry(id);
            var list = GetList(entry, entries, id);
            Refresh(entry, property, entries, ContentWidth(entry));
            int count = entries.arraySize;
            height += EditorGUIUtility.standardVerticalSpacing + VisibleListHeight(entry, list, count);
            if (entry.anyConflict)
                height += EditorGUIUtility.standardVerticalSpacing + MessageHeight(ConflictContent, ContentWidth(entry));
            return height;
        }

        Entry GetEntry(string id)
        {
            if (!_entries.TryGetValue(id, out var entry))
            {
                entry = new Entry();
                _entries[id] = entry;
            }

            return entry;
        }

        ReorderableList GetList(Entry entry, SerializedProperty entries, string id)
        {
            if (entry.list == null)
            {
                entry.list = new ReorderableList(entries.serializedObject, entries, true, true, true, true)
                {
                    drawHeaderCallback = rect =>
                    {
                        rect.xMin += DragHandleWidth;
                        DrawHeader(rect);
                    },
                    drawElementCallback = (rect, index, active, focused) =>
                    {
                        rect.xMin += DragHandleWidth;
                        DrawElement(id, rect, index);
                    },
                    elementHeightCallback = index => ElementHeight(id, index),
                    onAddCallback = AddPair,
                    onRemoveCallback = RemovePair
                };
            }

            entry.list.serializedProperty = entries;
            entry.list.draggable = true;
            return entry.list;
        }

        void Refresh(Entry entry, SerializedProperty property, SerializedProperty entries, float width)
        {
            EnsureComparer(entry, property);
            int count = entries.arraySize;
            if (count <= 0)
            {
                entry.conflictCount = 0;
                entry.anyConflict = false;
                entry.conflictsReady = true;
                entry.keyFp = 0;
                entry.heightCount = 0;
                entry.heightsReady = true;
                entry.heightFp = 0;
                entry.cursor = -1;
                BuildPages(entry, 0);
                return;
            }

            EnsureShape(entry, entries);
            long keyFp = Scan(entry, entries, count, !entry.allLeaf, out long valueFp);
            UpdateConflicts(entry, entries, count, keyFp);
            if (entry.allLeaf)
            {
                bool wide = EditorGUIUtility.wideMode;
                if (!entry.heightsReady || entry.heightCount != count || entry.measuredWide != wide)
                {
                    var element = entries.GetArrayElementAtIndex(0);
                    if (TryGetPair(element, out var key, out var value))
                    {
                        float row = Mathf.Max(
                            EditorGUI.GetPropertyHeight(key, GUIContent.none, true),
                            EditorGUI.GetPropertyHeight(value, GUIContent.none, true));
                        FillHeights(entry, count, row);
                        entry.measuredWide = wide;
                    }
                }

                BuildPages(entry, count);
                return;
            }

            long heightFp = Mix(Mix(keyFp, valueFp), FloatBits(width));
            heightFp = Mix(heightFp, EditorGUIUtility.wideMode ? 1L : 2L);
            heightFp = Mix(heightFp, FloatBits(EditorGUIUtility.singleLineHeight));
            heightFp = Mix(heightFp, FloatBits(EditorGUIUtility.standardVerticalSpacing));
            if (!entry.heightsReady || entry.heightFp != heightFp || entry.heightCount != count)
            {
                RebuildHeights(entry, entries, count);
                entry.heightFp = heightFp;
                entry.heightsReady = true;
                entry.heightCount = count;
            }

            BuildPages(entry, count);
        }

        static long Scan(Entry entry, SerializedProperty entries, int count, bool hashValues, out long valueFp)
        {
            EnsureLength(ref entry.keyHashes, count);
            EnsureLength(ref entry.absent, count);
            long keyFp = FnvOffset;
            valueFp = FnvOffset;
            var element = entries.GetArrayElementAtIndex(0);
            for (int i = 0; i < count; i++)
            {
                if (!TryGetPair(element, out var key, out var value))
                {
                    for (int j = i; j < count; j++)
                    {
                        entry.absent[j] = true;
                        entry.keyHashes[j] = 0;
                        keyFp = Mix(keyFp, 1L);
                    }

                    break;
                }

                bool absent = IsAbsentProperty(key);
                entry.absent[i] = absent;
                long hash = HashSubtree(key, false);
                entry.keyHashes[i] = hash;
                keyFp = Mix(keyFp, hash);
                if (absent)
                    keyFp = Mix(keyFp, 1L);
                if (hashValues)
                    valueFp = Mix(valueFp, HashSubtree(value, true));

                if (i + 1 >= count)
                    break;
                if (!element.Next(false))
                {
                    for (int j = i + 1; j < count; j++)
                    {
                        entry.absent[j] = true;
                        entry.keyHashes[j] = 0;
                        keyFp = Mix(keyFp, 1L);
                    }

                    break;
                }
            }

            return keyFp;
        }

        void UpdateConflicts(Entry entry, SerializedProperty entries, int count, long keyFp)
        {
            if (entry.conflictsReady && entry.keyFp == keyFp && entry.conflictCount == count)
                return;

            if (entry.comparer != null)
                RebuildComparerConflicts(entry, entries, count);
            else
                RebuildSerializedConflicts(entry, entries, count);

            entry.keyFp = keyFp;
            entry.conflictCount = count;
            entry.conflictsReady = true;
        }

        static void RebuildSerializedConflicts(Entry entry, SerializedProperty entries, int count)
        {
            EnsureLength(ref entry.conflicts, count);
            entry.anyConflict = false;
            for (int i = 0; i < count; i++)
            {
                bool conflict = entry.absent[i];
                if (!conflict)
                {
                    long hash = entry.keyHashes[i];
                    for (int j = 0; j < i; j++)
                    {
                        if (entry.absent[j] || entry.keyHashes[j] != hash)
                            continue;
                        if (KeysContentEqual(entries, j, i))
                        {
                            conflict = true;
                            break;
                        }
                    }
                }

                entry.conflicts[i] = conflict;
                if (conflict)
                    entry.anyConflict = true;
            }
        }

        void RebuildComparerConflicts(Entry entry, SerializedProperty entries, int count)
        {
            EnsureLength(ref entry.conflicts, count);
            var boxed = new object[count];
            var readable = new bool[count];
            var element = entries.GetArrayElementAtIndex(0);
            for (int i = 0; i < count; i++)
            {
                readable[i] = TryGetPair(element, out var key, out _) && TryReadKey(key, out boxed[i]);
                if (i + 1 < count && !element.Next(false))
                    break;
            }

            entry.anyConflict = false;
            for (int i = 0; i < count; i++)
            {
                bool conflict = false;
                if (readable[i])
                {
                    conflict = IsNullKey(boxed[i]);
                    if (!conflict)
                    {
                        for (int j = 0; j < i; j++)
                        {
                            if (!readable[j] || IsNullKey(boxed[j]))
                                continue;
                            if (KeysEqual(entry.comparer, entry.keyType, boxed[i], boxed[j]))
                            {
                                conflict = true;
                                break;
                            }
                        }
                    }
                }

                entry.conflicts[i] = conflict;
                if (conflict)
                    entry.anyConflict = true;
            }
        }

        static void FillHeights(Entry entry, int count, float height)
        {
            if (entry.heightsReady && entry.heightCount == count && Mathf.Approximately(entry.constantHeight, height))
                return;

            EnsureLength(ref entry.heights, count);
            for (int i = 0; i < count; i++)
                entry.heights[i] = height;
            entry.constantHeight = height;
            entry.heightCount = count;
            entry.heightsReady = true;
            entry.heightFp = long.MinValue;
        }

        static void RebuildHeights(Entry entry, SerializedProperty entries, int count)
        {
            EnsureLength(ref entry.heights, count);
            var element = entries.GetArrayElementAtIndex(0);
            float line = EditorGUIUtility.singleLineHeight;
            for (int i = 0; i < count; i++)
            {
                if (!TryGetPair(element, out var key, out var value))
                {
                    for (int j = i; j < count; j++)
                        entry.heights[j] = line;
                    break;
                }

                float keyHeight = entry.keyLeaf
                    ? line
                    : ContentHeight(key, entry.keyInline, entry.keyCheckHidden, entry.keyForward, entry.fieldInfo);
                float valueHeight = entry.valueLeaf
                    ? line
                    : ContentHeight(value, entry.valueInline, entry.valueCheckHidden, entry.valueForward, entry.fieldInfo);
                entry.heights[i] = Mathf.Max(keyHeight, valueHeight);
                if (i + 1 >= count)
                    break;
                if (!element.Next(false))
                {
                    for (int j = i + 1; j < count; j++)
                        entry.heights[j] = line;
                    break;
                }
            }

            entry.constantHeight = -1f;
        }

        void EnsureShape(Entry entry, SerializedProperty entries)
        {
            if (entries.arraySize <= 0)
                return;

            var element = entries.GetArrayElementAtIndex(0);
            if (!TryGetPair(element, out var key, out var value))
                return;
            int keyType = (int)key.propertyType;
            int valueType = (int)value.propertyType;
            entry.fieldInfo = fieldInfo;
            bool keyForward = UsesForward(fieldInfo, key);
            bool valueForward = UsesForward(fieldInfo, value);
            int forwardBits = (keyForward ? 1 : 0) | (valueForward ? 2 : 0);
            if (entry.shapeReady
                && entry.keyPropertyType == keyType
                && entry.valuePropertyType == valueType
                && entry.forwardBits == forwardBits)
                return;

            entry.keyPropertyType = keyType;
            entry.valuePropertyType = valueType;
            entry.forwardBits = forwardBits;
            entry.keyForward = keyForward;
            entry.valueForward = valueForward;
            entry.keyInline = !keyForward && ShouldInline(key);
            entry.valueInline = !valueForward && ShouldInline(value);
            entry.keyLeaf = !entry.keyInline && !keyForward && IsFixedHeight(key);
            entry.valueLeaf = !entry.valueInline && !valueForward && IsFixedHeight(value);
            entry.allLeaf = entry.keyLeaf && entry.valueLeaf;
            entry.keyCheckHidden = entry.keyInline && HasAnyHiddenChild(key);
            entry.valueCheckHidden = entry.valueInline && HasAnyHiddenChild(value);
            entry.shapeReady = true;
            entry.heightsReady = false;
            entry.conflictsReady = false;
        }

        void EnsureComparer(Entry entry, SerializedProperty property)
        {
            var nameProp = property.FindPropertyRelative("_comparerClassName");
            string name = nameProp != null ? nameProp.stringValue : null;
            if (string.IsNullOrWhiteSpace(name))
                name = string.Empty;
            if (entry.comparerName == name)
                return;

            entry.comparerName = name;
            entry.conflictsReady = false;
            if (name.Length == 0)
            {
                entry.comparer = null;
                entry.keyType = null;
                return;
            }

            entry.comparer = GetComparer(name);
            entry.keyType = entry.comparer != null ? KeyTypeOf(property) : null;
        }

        void DrawElement(string id, Rect rect, int index)
        {
            if (!_entries.TryGetValue(id, out var entry) || entry.list == null)
                return;

            var entries = entry.list.serializedProperty;
            if (entries == null || index < 0 || index >= entries.arraySize)
                return;

            if (!TryElementAt(entry, entries, index, out var keySource, out var valueSource))
                return;

            bool separated = ShowsPairSeparator(entry, index);
            if (separated)
                rect.height -= PairGap;
            float separatorTop = rect.yMax;

            if (rect.height > EditorGUIUtility.singleLineHeight)
            {
                rect.y += 1f;
                rect.height -= 2f;
            }

            float width = Mathf.Max(0f, (rect.width - Gap) * 0.5f);
            var keyRect = new Rect(rect.x, rect.y, width, rect.height);
            var valueRect = new Rect(rect.x + width + Gap, rect.y, width, rect.height);
            var keyProp = keySource.Copy();
            var valueProp = valueSource.Copy();
            bool conflict = index < entry.conflictCount && entry.conflicts != null && index < entry.conflicts.Length && entry.conflicts[index];

            float labelWidth = EditorGUIUtility.labelWidth;
            Color background = GUI.backgroundColor;
            EditorGUIUtility.labelWidth = Mathf.Max(40f, width * 0.45f);
            try
            {
                if (conflict)
                    GUI.backgroundColor = ConflictTint;
                DrawPropertyContent(keyRect, keyProp, entry.keyInline, entry.keyCheckHidden, entry.keyForward, entry.fieldInfo);
                GUI.backgroundColor = background;
                DrawPropertyContent(valueRect, valueProp, entry.valueInline, entry.valueCheckHidden, entry.valueForward, entry.fieldInfo);
            }
            finally
            {
                GUI.backgroundColor = background;
                EditorGUIUtility.labelWidth = labelWidth;
            }

            if (separated && UnityEngine.Event.current.type == UnityEngine.EventType.Repaint)
            {
                float y = separatorTop + (PairGap - 1f) * 0.5f;
                EditorGUI.DrawRect(new Rect(rect.x, y, rect.width, 1f), PairSeparatorColor);
            }
        }

        static bool ShowsPairSeparator(Entry entry, int index)
        {
            return entry != null && index >= 0 && index + 1 < entry.heightCount;
        }

        static bool TryElementAt(Entry entry, SerializedProperty entries, int index, out SerializedProperty key, out SerializedProperty value)
        {
            if (entry.cursor == index && entry.cursorKey != null && entry.cursorValue != null)
            {
                key = entry.cursorKey;
                value = entry.cursorValue;
                return true;
            }

            if (entry.cursor >= 0 && index == entry.cursor + 1 && entry.cursorElement != null)
            {
                var next = entry.cursorElement.Copy();
                if (next.Next(false) && TryGetPair(next, out key, out value))
                {
                    entry.cursorElement = next;
                    entry.cursorKey = key;
                    entry.cursorValue = value;
                    entry.cursor = index;
                    return true;
                }
            }

            if (index < 0 || index >= entries.arraySize)
            {
                key = null;
                value = null;
                return false;
            }

            entry.cursorElement = entries.GetArrayElementAtIndex(index);
            if (!TryGetPair(entry.cursorElement, out key, out value))
            {
                entry.cursor = -1;
                entry.cursorKey = null;
                entry.cursorValue = null;
                return false;
            }

            entry.cursorKey = key;
            entry.cursorValue = value;
            entry.cursor = index;
            return true;
        }

        static bool TryGetPair(SerializedProperty element, out SerializedProperty key, out SerializedProperty value)
        {
            key = element != null ? element.FindPropertyRelative("_key") : null;
            value = element != null ? element.FindPropertyRelative("_value") : null;
            return key != null && value != null;
        }

        float ElementHeight(string id, int index)
        {
            if (!_entries.TryGetValue(id, out var entry))
                return EditorGUIUtility.singleLineHeight;
            return RowHeight(entry, index);
        }

        static float RowHeight(Entry entry, int index)
        {
            float line = EditorGUIUtility.singleLineHeight;
            if (entry == null || index < 0 || index >= entry.heightCount)
                return line;
            if (entry.heights == null || index >= entry.heights.Length)
                return line;

            float height = entry.heights[index];
            if (height <= 1f)
                height = line;
            if (ShowsPairSeparator(entry, index))
                height += PairGap;
            return height;
        }

        void BuildPages(Entry entry, int count)
        {
            var starts = entry.pageStartBuffer;
            starts.Clear();
            if (count <= 0)
            {
                entry.pageCount = 1;
                entry.page = 0;
                if (entry.pageStarts.Length != 0)
                    entry.pageStarts = Array.Empty<int>();
                return;
            }

            int index = 0;
            while (index < count)
            {
                starts.Add(index);
                float used = 0f;
                int pageStart = index;
                while (index < count)
                {
                    float row = RowHeight(entry, index);
                    if (index > pageStart && used + row > MaxPageBody)
                        break;
                    used += row;
                    index++;
                }
            }

            if (entry.pageStarts.Length != starts.Count)
                entry.pageStarts = starts.ToArray();
            else
            {
                for (int i = 0; i < starts.Count; i++)
                    entry.pageStarts[i] = starts[i];
            }

            entry.pageCount = starts.Count;
            if (entry.page < 0)
                entry.page = 0;
            if (entry.page >= entry.pageCount)
                entry.page = entry.pageCount - 1;
        }

        float VisibleListHeight(Entry entry, ReorderableList list, int count)
        {
            if (count <= 0)
                return list.GetHeight();

            GetPageRange(entry, count, out int start, out int end);
            float body = 0f;
            for (int i = start; i < end; i++)
                body += RowHeight(entry, i);
            float pager = entry.pageCount > 1 ? PagerHeight : 0f;
            return HeaderHeight(list) + body + pager + FooterHeight(list);
        }

        static float HeaderHeight(ReorderableList list)
        {
            return list.headerHeight > 1f ? list.headerHeight : 18f;
        }

        static float FooterHeight(ReorderableList list)
        {
            return list.footerHeight > 1f ? list.footerHeight : 20f;
        }

        static void GetPageRange(Entry entry, int count, out int start, out int end)
        {
            if (entry.pageCount <= 1 || entry.pageStarts == null || entry.pageStarts.Length == 0)
            {
                start = 0;
                end = count;
                return;
            }

            int page = Mathf.Clamp(entry.page, 0, entry.pageCount - 1);
            start = entry.pageStarts[page];
            end = page + 1 < entry.pageStarts.Length ? entry.pageStarts[page + 1] : count;
            if (start < 0)
                start = 0;
            if (end < start)
                end = start;
            if (end > count)
                end = count;
        }

        void DrawPagedList(Entry entry, ReorderableList list, string id, Rect rect, int count)
        {
            GetPageRange(entry, count, out int start, out int end);
            if (list.index < start || list.index >= end)
                list.index = -1;

            float headerHeight = HeaderHeight(list);
            float footerHeight = FooterHeight(list);
            bool showPager = entry.pageCount > 1;
            float pagerHeight = showPager ? PagerHeight : 0f;
            var headerRect = new Rect(rect.x, rect.y, rect.width, headerHeight);
            var footerRect = new Rect(rect.x, rect.yMax - footerHeight, rect.width, footerHeight);
            var pagerRect = new Rect(rect.x, footerRect.y - pagerHeight, rect.width, pagerHeight);
            var bodyRect = new Rect(rect.x, headerRect.yMax, rect.width, Mathf.Max(0f, pagerRect.y - headerRect.yMax));

            var defaults = ReorderableList.defaultBehaviours;
            if (UnityEngine.Event.current.type == UnityEngine.EventType.Repaint)
            {
                defaults.DrawHeaderBackground(headerRect);
                if (defaults.boxBackground != null)
                    defaults.boxBackground.Draw(bodyRect, false, false, false, false);
            }

            var headerContent = headerRect;
            headerContent.xMin += PageInset + DragHandleWidth;
            headerContent.xMax -= PageInset;
            headerContent.y += 1f;
            DrawHeader(headerContent);

            float y = bodyRect.y;
            var mouse = UnityEngine.Event.current;
            float contentWidth = Mathf.Max(0f, bodyRect.width - PageInset * 2f - DragHandleWidth);
            int dragId = GUIUtility.GetControlID(DragHint, FocusType.Passive);
            bool dragging = GUIUtility.hotControl == dragId && entry.dragFrom >= start && entry.dragFrom < end;
            if (dragging && mouse.type == UnityEngine.EventType.MouseDrag)
            {
                if ((mouse.mousePosition - entry.dragOrigin).sqrMagnitude > 9f)
                    entry.dragMoved = true;
                mouse.Use();
            }

            if (dragging && entry.dragMoved)
                entry.dragTo = PickDropIndex(entry, mouse.mousePosition.y, bodyRect.y, start, end);

            if (GUIUtility.hotControl == dragId && mouse.type == UnityEngine.EventType.MouseUp)
            {
                if (entry.dragMoved && entry.dragFrom >= 0 && entry.dragTo >= 0 && entry.dragFrom != entry.dragTo)
                {
                    MoveAnim(entry, entry.dragFrom, entry.dragTo);
                    MovePair(list.serializedProperty, entry.dragFrom, entry.dragTo);
                    list.index = entry.dragTo;
                    entry.cursor = -1;
                    entry.conflictsReady = false;
                    entry.heightsReady = false;
                    entry.keyFp = long.MinValue;
                }

                entry.dragFrom = -1;
                entry.dragTo = -1;
                entry.dragMoved = false;
                GUIUtility.hotControl = 0;
                dragging = false;
                mouse.Use();
            }

            bool sliding = LayoutDragMotion(entry, mouse, bodyRect.y, start, end, dragging);
            if (sliding)
                RepaintInspector();

            Rect floating = default;
            bool hasFloating = false;
            for (int i = start; i < end; i++)
            {
                float rowHeight = RowHeight(entry, i);
                float drawY = i < entry.animY.Length ? entry.animY[i] : y;
                var handle = new Rect(bodyRect.x + PageInset, drawY, DragHandleWidth, rowHeight);
                var row = new Rect(handle.xMax, drawY, contentWidth, rowHeight);
                var background = new Rect(bodyRect.x, drawY, bodyRect.width, rowHeight);
                var hit = new Rect(bodyRect.x + PageInset, y, DragHandleWidth, rowHeight);
                if (mouse.type == UnityEngine.EventType.MouseDown && mouse.button == 0 && hit.Contains(mouse.mousePosition))
                {
                    list.index = i;
                    entry.dragFrom = i;
                    entry.dragTo = i;
                    entry.dragMoved = false;
                    entry.dragOrigin = mouse.mousePosition;
                    entry.dragGrab = mouse.mousePosition.y - y;
                    GUIUtility.hotControl = dragId;
                    mouse.Use();
                }
                else if (mouse.type == UnityEngine.EventType.ContextClick && hit.Contains(mouse.mousePosition))
                {
                    ShowMoveMenu(entry, list, i, count);
                    mouse.Use();
                }
                else if (mouse.type == UnityEngine.EventType.MouseDown && mouse.button == 0 && new Rect(row.x, y, row.width, rowHeight).Contains(mouse.mousePosition))
                {
                    list.index = i;
                }

                if (dragging && i == entry.dragFrom)
                {
                    floating = row;
                    hasFloating = true;
                    y += rowHeight;
                    continue;
                }

                defaults.DrawElementBackground(background, i, list.index == i, false, false);
                if (mouse.type == UnityEngine.EventType.Repaint && defaults.draggingHandle != null)
                {
                    var grip = new Rect(handle.x, drawY + 2f, DragHandleWidth, Mathf.Min(10f, rowHeight - 4f));
                    defaults.draggingHandle.Draw(grip, false, false, false, false);
                    DrawMenuHint(handle);
                }

                EditorGUIUtility.AddCursorRect(hit, MouseCursor.MoveArrow);
                DrawElement(id, row, i);
                y += rowHeight;
            }

            if (hasFloating)
            {
                float rowHeight = floating.height;
                var background = new Rect(bodyRect.x, floating.y, bodyRect.width, rowHeight);
                var handle = new Rect(bodyRect.x + PageInset, floating.y, DragHandleWidth, rowHeight);
                if (mouse.type == UnityEngine.EventType.Repaint)
                {
                    EditorGUI.DrawRect(new Rect(background.x + 2f, background.y + 3f, background.width, background.height), new Color(0f, 0f, 0f, 0.28f));
                    defaults.DrawElementBackground(background, entry.dragFrom, true, true, true);
                    if (defaults.draggingHandle != null)
                    {
                        var grip = new Rect(handle.x, floating.y + 2f, DragHandleWidth, Mathf.Min(10f, rowHeight - 4f));
                        defaults.draggingHandle.Draw(grip, false, false, false, false);
                        DrawMenuHint(handle);
                    }
                }

                DrawElement(id, floating, entry.dragFrom);
            }

            if (showPager)
            {
                var pagerContent = pagerRect;
                pagerContent.xMin += PageInset;
                pagerContent.xMax -= PageInset;
                DrawPager(pagerContent, entry, count, start, end);
            }
            defaults.DrawFooter(footerRect, list);
        }

        bool LayoutDragMotion(Entry entry, UnityEngine.Event mouse, float originY, int start, int end, bool dragging)
        {
            if (end <= start)
                return false;

            if (entry.animY == null || entry.animY.Length < end)
            {
                var next = new float[Mathf.NextPowerOfTwo(Mathf.Max(end, 8))];
                if (entry.animY != null)
                    Array.Copy(entry.animY, next, entry.animY.Length);
                entry.animY = next;
            }

            bool step = mouse.type == UnityEngine.EventType.Repaint || mouse.type == UnityEngine.EventType.MouseDrag;
            double now = EditorApplication.timeSinceStartup;
            float dt = entry.animStamp <= 0d ? 0.016f : Mathf.Clamp((float)(now - entry.animStamp), 0f, 0.05f);
            if (step)
                entry.animStamp = now;
            float blend = step ? 1f - Mathf.Exp(-18f * dt) : 0f;

            if (entry.animOrigin != 0f)
            {
                float delta = originY - entry.animOrigin;
                if (Mathf.Abs(delta) > 0.01f)
                {
                    for (int i = start; i < end && i < entry.animY.Length; i++)
                        entry.animY[i] += delta;
                }
            }

            entry.animOrigin = originY;
            float dragHeight = dragging ? RowHeight(entry, entry.dragFrom) : 0f;
            float layout = originY;
            bool moving = false;
            for (int i = start; i < end; i++)
            {
                float height = RowHeight(entry, i);
                float target = layout;
                if (dragging && entry.dragFrom != entry.dragTo)
                {
                    if (entry.dragFrom < entry.dragTo && i > entry.dragFrom && i <= entry.dragTo)
                        target -= dragHeight;
                    else if (entry.dragTo < entry.dragFrom && i >= entry.dragTo && i < entry.dragFrom)
                        target += dragHeight;
                }

                if (dragging && i == entry.dragFrom)
                {
                    float maxY = originY + Mathf.Max(0f, (end - start > 0 ? SumHeight(entry, start, end) : height) - height);
                    target = Mathf.Clamp(mouse.mousePosition.y - entry.dragGrab, originY, maxY);
                    entry.animY[i] = target;
                }
                else if (Mathf.Abs(entry.animY[i] - target) > 400f || (entry.animY[i] == 0f && target > 1f))
                {
                    entry.animY[i] = target;
                }
                else
                {
                    float next = Mathf.Lerp(entry.animY[i], target, blend);
                    if (Mathf.Abs(next - target) < 0.4f)
                        next = target;
                    else
                        moving = true;
                    entry.animY[i] = next;
                }

                layout += height;
            }

            return moving || dragging;
        }

        static float SumHeight(Entry entry, int start, int end)
        {
            float sum = 0f;
            for (int i = start; i < end; i++)
                sum += RowHeight(entry, i);
            return sum;
        }

        static void MoveAnim(Entry entry, int from, int to)
        {
            if (entry.animY == null || from == to || from < 0 || to < 0 || from >= entry.animY.Length || to >= entry.animY.Length)
                return;

            float value = entry.animY[from];
            if (from < to)
            {
                for (int i = from; i < to; i++)
                    entry.animY[i] = entry.animY[i + 1];
            }
            else
            {
                for (int i = from; i > to; i--)
                    entry.animY[i] = entry.animY[i - 1];
            }

            entry.animY[to] = value;
        }

        static void RepaintInspector()
        {
            if (EditorWindow.mouseOverWindow != null)
                EditorWindow.mouseOverWindow.Repaint();
            else if (EditorWindow.focusedWindow != null)
                EditorWindow.focusedWindow.Repaint();
        }

        static readonly GUIContent MenuHintContent = new GUIContent("⋮");
        static GUIStyle s_menuHint;

        static void DrawMenuHint(Rect handle)
        {
            if (UnityEngine.Event.current.type != UnityEngine.EventType.Repaint)
                return;

            if (s_menuHint == null)
            {
                s_menuHint = new GUIStyle(EditorStyles.miniLabel)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 12,
                    padding = new RectOffset(0, 0, 0, 0),
                    margin = new RectOffset(0, 0, 0, 0),
                };
            }

            var hint = handle;
            if (handle.height > 28f)
            {
                hint.y = handle.yMax - 16f;
                hint.height = 14f;
            }
            else
            {
                hint.y = handle.yMax - Mathf.Min(12f, handle.height);
                hint.height = Mathf.Min(12f, handle.height);
            }

            Color previous = GUI.color;
            GUI.color = EditorGUIUtility.isProSkin
                ? new Color(1f, 1f, 1f, 0.55f)
                : new Color(0f, 0f, 0f, 0.55f);
            s_menuHint.Draw(hint, MenuHintContent, false, false, false, false);
            GUI.color = previous;
        }

        static int PickDropIndex(Entry entry, float mouseY, float originY, int start, int end)
        {
            float y = originY;
            int target = Mathf.Max(start, end - 1);
            for (int i = start; i < end; i++)
            {
                float row = RowHeight(entry, i);
                if (mouseY < y + row * 0.5f)
                    return i;
                y += row;
                target = i;
            }

            return target;
        }

        static void MovePair(SerializedProperty entries, int from, int to)
        {
            if (entries == null || from == to)
                return;

            int count = entries.arraySize;
            if (from < 0 || to < 0 || from >= count || to >= count)
                return;

            entries.MoveArrayElement(from, to);
            entries.serializedObject.ApplyModifiedProperties();
        }

        void ShowMoveMenu(Entry entry, ReorderableList list, int index, int count)
        {
            var menu = new GenericMenu();
            if (index <= 0)
                menu.AddDisabledItem(new GUIContent("上移一项"));
            else
                menu.AddItem(new GUIContent("上移一项"), false, () => ShiftPair(entry, list, index, index - 1));

            if (index >= count - 1)
                menu.AddDisabledItem(new GUIContent("下移一项"));
            else
                menu.AddItem(new GUIContent("下移一项"), false, () => ShiftPair(entry, list, index, index + 1));

            menu.ShowAsContext();
        }

        void ShiftPair(Entry entry, ReorderableList list, int from, int to)
        {
            MoveAnim(entry, from, to);
            MovePair(list.serializedProperty, from, to);
            list.index = to;
            entry.cursor = -1;
            entry.conflictsReady = false;
            entry.heightsReady = false;
            entry.keyFp = long.MinValue;
            entry.page = PageContaining(entry, to);
        }

        static int PageContaining(Entry entry, int index)
        {
            if (entry.pageStarts == null || entry.pageStarts.Length == 0)
                return 0;

            int page = 0;
            for (int i = 0; i < entry.pageStarts.Length; i++)
            {
                if (entry.pageStarts[i] <= index)
                    page = i;
                else
                    break;
            }

            return page;
        }

        static void DrawPager(Rect rect, Entry entry, int count, int start, int end)
        {
            const float buttonWidth = 28f;
            const float fieldWidth = 48f;
            float y = rect.y + 2f;
            float height = Mathf.Max(0f, rect.height - 4f);
            int shownPage = entry.page + 1;
            string totalText = "/ " + entry.pageCount;
            string rangeText = string.Format("{0}-{1} / {2}", start + 1, Mathf.Max(start, end), count);
            float totalWidth = EditorStyles.miniLabel.CalcSize(new GUIContent(totalText)).x + 4f;
            float controlsWidth = buttonWidth + 4f + fieldWidth + 4f + totalWidth + 4f + buttonWidth;
            float controlsX = rect.xMax - controlsWidth;

            var rangeRect = new Rect(rect.x, rect.y, Mathf.Max(0f, controlsX - rect.x - 4f), rect.height);
            GUI.Label(rangeRect, rangeText, EditorStyles.miniLabel);

            var prev = new Rect(controlsX, y, buttonWidth, height);
            var field = new Rect(prev.xMax + 4f, y, fieldWidth, height);
            var total = new Rect(field.xMax + 4f, y, totalWidth, height);
            var next = new Rect(total.xMax + 4f, y, buttonWidth, height);

            EditorGUI.BeginDisabledGroup(entry.page <= 0);
            if (GUI.Button(prev, "<", EditorStyles.miniButton))
                entry.page--;
            EditorGUI.EndDisabledGroup();

            int inputPage = EditorGUI.DelayedIntField(field, shownPage);
            if (inputPage != shownPage)
                entry.page = Mathf.Clamp(inputPage, 1, entry.pageCount) - 1;

            GUI.Label(total, totalText, EditorStyles.miniLabel);

            EditorGUI.BeginDisabledGroup(entry.page >= entry.pageCount - 1);
            if (GUI.Button(next, ">", EditorStyles.miniButton))
                entry.page++;
            EditorGUI.EndDisabledGroup();
        }

        static void DrawPropertyContent(
            Rect rect,
            SerializedProperty property,
            bool inline,
            bool checkHidden,
            bool forward,
            FieldInfo sourceField)
        {
            if (forward && TryDrawForwarded(rect, property, sourceField))
                return;

            if (!inline)
            {
                EditorGUI.PropertyField(rect, property, GUIContent.none, true);
                return;
            }

            float y = rect.y;
            float spacing = EditorGUIUtility.standardVerticalSpacing;
            int index = 0;
            int depth = property.depth;
            var child = property.Copy();
            if (!child.Next(true) || child.depth != depth + 1)
                return;

            do
            {
                if (child.depth != depth + 1)
                    return;
                if (checkHidden && IsHidden(child))
                    continue;

                var drawn = child.Copy();
                if (index++ > 0)
                    y += spacing;
                float height = EditorGUI.GetPropertyHeight(drawn, true);
                EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, height), drawn, true);
                y += height;
            }
            while (child.Next(false));
        }

        static float ContentHeight(
            SerializedProperty property,
            bool inline,
            bool checkHidden,
            bool forward,
            FieldInfo sourceField)
        {
            if (forward && TryForwardedHeight(property, sourceField, out float forwarded))
                return forwarded;

            if (!inline)
                return EditorGUI.GetPropertyHeight(property, GUIContent.none, true);

            float height = 0f;
            float spacing = EditorGUIUtility.standardVerticalSpacing;
            int index = 0;
            int depth = property.depth;
            var child = property.Copy();
            if (child.Next(true) && child.depth == depth + 1)
            {
                do
                {
                    if (child.depth != depth + 1)
                        break;
                    if (checkHidden && IsHidden(child))
                        continue;

                    var drawn = child.Copy();
                    if (index++ > 0)
                        height += spacing;
                    height += EditorGUI.GetPropertyHeight(drawn, true);
                }
                while (child.Next(false));
            }

            return height > 0f ? height : EditorGUIUtility.singleLineHeight;
        }

        /// <summary>
        /// 自定义类默认是无标题折叠，收起时看起来只有类型名。没有专用绘制器时直接画出字段。
        /// </summary>
        static bool ShouldInline(SerializedProperty property)
        {
            if (property.propertyType != SerializedPropertyType.Generic)
                return false;
            if (IsSingleLineComposite(property.type) || HasPropertyDrawer(property))
                return false;
            return HasDirectChild(property);
        }

        static bool IsFixedHeight(SerializedProperty property)
        {
            return IsSingleLineType(property.propertyType) && !HasPropertyDrawer(property);
        }

        /// <summary>字段上的 TextArea、Range 等特性，类型匹配时画到键或值上。</summary>
        static bool UsesForward(FieldInfo sourceField, SerializedProperty property)
        {
            if (sourceField == null || property == null)
                return false;

            return PropertyAttributeForwardingUtility.TryGetForwardedDrawer(sourceField, property, out var drawer)
                && PropertyAttributeForwardingUtility.CanSafelyUseDrawer(drawer, property);
        }

        static bool TryDrawForwarded(Rect rect, SerializedProperty property, FieldInfo sourceField)
        {
            if (!PropertyAttributeForwardingUtility.TryGetForwardedDrawer(sourceField, property, out var drawer)
                || !PropertyAttributeForwardingUtility.CanSafelyUseDrawer(drawer, property))
                return false;

            if (!PropertyAttributeForwardingUtility.TryDrawForwarded(rect, property, GUIContent.none, drawer))
                drawer.OnGUI(rect, property, GUIContent.none);
            return true;
        }

        static bool TryForwardedHeight(SerializedProperty property, FieldInfo sourceField, out float height)
        {
            height = 0f;
            if (!PropertyAttributeForwardingUtility.TryGetForwardedDrawer(sourceField, property, out var drawer)
                || !PropertyAttributeForwardingUtility.CanSafelyUseDrawer(drawer, property))
                return false;

            if (drawer.attribute is TextAreaAttribute textArea)
                height = TextAreaHeight(property, textArea);
            else if (drawer.attribute is MultilineAttribute multiline)
                height = Mathf.Max(1, multiline.lines) * EditorGUIUtility.singleLineHeight;
            else
                height = PropertyAttributeForwardingUtility.GetPropertyHeight(property, GUIContent.none, sourceField);

            if (height < 1f)
                height = EditorGUIUtility.singleLineHeight;
            return true;
        }

        static float TextAreaHeight(SerializedProperty property, TextAreaAttribute textArea)
        {
            float line = EditorGUIUtility.singleLineHeight;
            int minLines = Mathf.Max(1, textArea.minLines);
            int maxLines = Mathf.Max(minLines, textArea.maxLines);
            float width = Mathf.Max(80f, EditorGUIUtility.currentViewWidth * 0.42f);
            float content = EditorStyles.textArea.CalcHeight(
                new GUIContent(property.stringValue ?? string.Empty),
                width);
            return Mathf.Clamp(content, minLines * line, maxLines * line);
        }

        static bool IsSingleLineType(SerializedPropertyType type)
        {
            switch (type)
            {
                case SerializedPropertyType.Integer:
                case SerializedPropertyType.Boolean:
                case SerializedPropertyType.Float:
                case SerializedPropertyType.String:
                case SerializedPropertyType.Color:
                case SerializedPropertyType.ObjectReference:
                case SerializedPropertyType.LayerMask:
                case SerializedPropertyType.Enum:
                case SerializedPropertyType.Vector2:
                case SerializedPropertyType.Vector3:
                case SerializedPropertyType.Vector4:
                case SerializedPropertyType.Vector2Int:
                case SerializedPropertyType.Vector3Int:
                case SerializedPropertyType.Rect:
                case SerializedPropertyType.RectInt:
                case SerializedPropertyType.Character:
                case SerializedPropertyType.Quaternion:
#if UNITY_2022_1_OR_NEWER
                case SerializedPropertyType.Hash128:
#endif
                case SerializedPropertyType.Gradient:
                case SerializedPropertyType.AnimationCurve:
                    return true;
                default:
                    return false;
            }
        }

        static bool HasDirectChild(SerializedProperty property)
        {
            int depth = property.depth;
            SerializedProperty child = property.Copy();
            return child.Next(true) && child.depth == depth + 1 && !IsHidden(child);
        }

        static bool HasAnyHiddenChild(SerializedProperty property)
        {
            int depth = property.depth;
            var child = property.Copy();
            if (!child.Next(true) || child.depth != depth + 1)
                return false;

            do
            {
                if (child.depth != depth + 1)
                    return false;
                if (IsHidden(child))
                    return true;
            }
            while (child.Next(false));

            return false;
        }

        static bool IsSingleLineComposite(string typeName)
        {
            switch (typeName)
            {
                case "Vector2":
                case "Vector3":
                case "Vector4":
                case "Vector2Int":
                case "Vector3Int":
                case "Quaternion":
                case "Rect":
                case "RectInt":
                case "Bounds":
                case "BoundsInt":
                case "Hash128":
                    return true;
                default:
                    return false;
            }
        }

        static bool HasPropertyDrawer(SerializedProperty property)
        {
            if (!s_handlerLookupFailed && s_getHandler == null)
            {
                var utility = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.ScriptAttributeUtility");
                s_getHandler = utility?.GetMethod(
                    "GetHandler",
                    BindingFlags.Static | BindingFlags.NonPublic,
                    null,
                    new[] { typeof(SerializedProperty) },
                    null);
                s_handlerLookupFailed = s_getHandler == null;
            }

            if (s_getHandler == null)
                return false;

            try
            {
                object handler = s_getHandler.Invoke(null, new object[] { property });
                if (handler == null)
                    return false;

                if (s_hasPropertyDrawer == null)
                {
                    s_hasPropertyDrawer = handler.GetType().GetProperty(
                        "hasPropertyDrawer",
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                }

                return s_hasPropertyDrawer != null && (bool)s_hasPropertyDrawer.GetValue(handler);
            }
            catch (Exception)
            {
                return false;
            }
        }

        static bool IsHidden(SerializedProperty property)
        {
            try
            {
                if (s_getFieldInfo == null && !s_fieldInfoLookupFailed)
                {
                    var utility = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.ScriptAttributeUtility");
                    s_getFieldInfo = utility?.GetMethod(
                        "GetFieldInfoFromProperty",
                        BindingFlags.Static | BindingFlags.NonPublic,
                        null,
                        new[] { typeof(SerializedProperty), typeof(Type).MakeByRefType() },
                        null);
                    s_fieldInfoLookupFailed = s_getFieldInfo == null;
                }

                if (s_getFieldInfo == null)
                    return false;

                object[] args = { property, null };
                var field = s_getFieldInfo.Invoke(null, args) as FieldInfo;
                return field != null && field.IsDefined(typeof(HideInInspector), true);
            }
            catch (Exception)
            {
                return false;
            }
        }

        static void DrawHeader(Rect rect)
        {
            float width = Mathf.Max(0f, (rect.width - Gap) * 0.5f);
            EditorGUI.LabelField(new Rect(rect.x, rect.y, width, rect.height), KeyHeader);
            EditorGUI.LabelField(new Rect(rect.x + width + Gap, rect.y, width, rect.height), ValueHeader);
        }

        void AddPair(ReorderableList list)
        {
            var entries = list.serializedProperty;
            if (entries == null)
                return;

            int count = entries.arraySize + 1;
            entries.arraySize = count;
            list.index = count - 1;
            FocusLastPage(list);
        }

        void FocusLastPage(ReorderableList list)
        {
            foreach (var pair in _entries)
            {
                if (pair.Value.list != list)
                    continue;
                pair.Value.page = int.MaxValue;
                return;
            }
        }

        static void RemovePair(ReorderableList list)
        {
            var entries = list.serializedProperty;
            if (entries == null)
                return;

            int index = list.index;
            if (index < 0 || index >= entries.arraySize)
                return;

            DeleteElement(entries, index);
            if (entries.arraySize == 0)
                list.index = -1;
            else if (index >= entries.arraySize)
                list.index = entries.arraySize - 1;
        }

        static void DeleteElement(SerializedProperty array, int index)
        {
            if (index < 0 || index >= array.arraySize)
                return;

            int size = array.arraySize;
            var element = array.GetArrayElementAtIndex(index);
            if (element.propertyType == SerializedPropertyType.ObjectReference && element.objectReferenceValue != null)
                element.objectReferenceValue = null;

            array.DeleteArrayElementAtIndex(index);
            if (array.arraySize == size && index < array.arraySize)
                array.DeleteArrayElementAtIndex(index);
        }

        static bool KeysContentEqual(SerializedProperty entries, int a, int b)
        {
            if (!TryGetPair(entries.GetArrayElementAtIndex(a), out var first, out _))
                return false;
            if (!TryGetPair(entries.GetArrayElementAtIndex(b), out var other, out _))
                return false;

            return ContentEquals(first, other);
        }

        static bool IsAbsentProperty(SerializedProperty key)
        {
            switch (key.propertyType)
            {
                case SerializedPropertyType.ObjectReference:
                    if (key.objectReferenceInstanceIDValue == 0)
                        return true;
                    return key.objectReferenceValue == null;
                case SerializedPropertyType.ManagedReference:
                    return IsAbsentManagedReference(key);
                default:
                    return false;
            }
        }

        static bool ContentEquals(SerializedProperty a, SerializedProperty b)
        {
            if (a == null || b == null || a.propertyType != b.propertyType)
                return false;
            if (TryLeafEquals(a, b, out bool equal))
                return equal;
            if (a.propertyType == SerializedPropertyType.ManagedReference &&
                a.managedReferenceFullTypename != b.managedReferenceFullTypename)
                return false;

            return ChildrenEqual(a, b);
        }

        static bool ChildrenEqual(SerializedProperty a, SerializedProperty b)
        {
            int depthA = a.depth;
            int depthB = b.depth;
            var ia = a.Copy();
            var ib = b.Copy();
            bool hasA = ia.Next(true);
            bool hasB = ib.Next(true);
            int guard = 0;
            while (hasA && hasB && guard++ < 100000)
            {
                if (ia.depth <= depthA || ib.depth <= depthB)
                    break;
                if (ia.depth - depthA != ib.depth - depthB || ia.propertyType != ib.propertyType || ia.name != ib.name)
                    return false;
                if (TryLeafEquals(ia, ib, out bool equal))
                {
                    if (!equal)
                        return false;
                    hasA = ia.Next(false);
                    hasB = ib.Next(false);
                    continue;
                }

                hasA = ia.Next(true);
                hasB = ib.Next(true);
            }

            bool endA = !hasA || ia.depth <= depthA;
            bool endB = !hasB || ib.depth <= depthB;
            return endA && endB;
        }

        static long HashSubtree(SerializedProperty root, bool includeExpanded)
        {
            if (TryHashLeaf(root, out long leaf))
            {
                if (includeExpanded && root.isExpanded)
                    return Mix(leaf, 1L);
                return leaf;
            }

            long hash = Mix(FnvOffset, (long)root.propertyType);
            if (root.propertyType == SerializedPropertyType.ManagedReference)
                hash = Mix(hash, ManagedReferenceHash(root));
            if (includeExpanded)
                hash = Mix(hash, root.isExpanded ? 1L : 2L);
            if (!root.hasChildren)
                return hash;

            int depth = root.depth;
            var it = root.Copy();
            if (!it.Next(true))
                return hash;

            int guard = 0;
            while (guard++ < 100000)
            {
                if (it.depth <= depth)
                    break;
                if (includeExpanded)
                    hash = Mix(hash, it.isExpanded ? 1L : 2L);
                if (TryHashLeaf(it, out long childLeaf))
                {
                    hash = Mix(hash, childLeaf);
                    if (!it.Next(false))
                        break;
                    continue;
                }

                hash = Mix(hash, (long)it.propertyType + 1L);
                if (!it.Next(true))
                    break;
            }

            return hash;
        }

        static bool TryHashLeaf(SerializedProperty property, out long hash)
        {
            hash = 0;
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    hash = property.longValue;
                    return true;
                case SerializedPropertyType.Boolean:
                    hash = property.boolValue ? 1L : 0L;
                    return true;
                case SerializedPropertyType.Float:
                    hash = FloatBits(property.floatValue);
                    return true;
                case SerializedPropertyType.String:
                    hash = HashString(property.stringValue);
                    return true;
                case SerializedPropertyType.Color:
                    hash = HashColor(property.colorValue);
                    return true;
                case SerializedPropertyType.ObjectReference:
                    hash = property.objectReferenceInstanceIDValue;
                    return true;
                case SerializedPropertyType.LayerMask:
                case SerializedPropertyType.Enum:
                case SerializedPropertyType.Character:
                    hash = property.intValue;
                    return true;
                case SerializedPropertyType.Vector2:
                    hash = HashVector2(property.vector2Value);
                    return true;
                case SerializedPropertyType.Vector3:
                    hash = HashVector3(property.vector3Value);
                    return true;
                case SerializedPropertyType.Vector4:
                    hash = HashVector4(property.vector4Value);
                    return true;
                case SerializedPropertyType.Vector2Int:
                    var vector2Int = property.vector2IntValue;
                    hash = Mix(Mix(FnvOffset, vector2Int.x), vector2Int.y);
                    return true;
                case SerializedPropertyType.Vector3Int:
                    var vector3Int = property.vector3IntValue;
                    hash = Mix(Mix(Mix(FnvOffset, vector3Int.x), vector3Int.y), vector3Int.z);
                    return true;
                case SerializedPropertyType.Rect:
                    hash = HashRect(property.rectValue);
                    return true;
                case SerializedPropertyType.RectInt:
                    var rectInt = property.rectIntValue;
                    hash = Mix(Mix(Mix(Mix(FnvOffset, rectInt.x), rectInt.y), rectInt.width), rectInt.height);
                    return true;
                case SerializedPropertyType.Quaternion:
                    hash = HashQuaternion(property.quaternionValue);
                    return true;
                case SerializedPropertyType.Bounds:
                    var bounds = property.boundsValue;
                    hash = Mix(HashVector3(bounds.center), HashVector3(bounds.extents));
                    return true;
                case SerializedPropertyType.BoundsInt:
                    var boundsInt = property.boundsIntValue;
                    hash = Mix(Mix(Mix(FnvOffset, boundsInt.x), boundsInt.y), boundsInt.z);
                    hash = Mix(Mix(Mix(hash, boundsInt.size.x), boundsInt.size.y), boundsInt.size.z);
                    return true;
#if UNITY_2022_1_OR_NEWER
                case SerializedPropertyType.Hash128:
                    hash = property.hash128Value.GetHashCode();
                    return true;
#endif
                case SerializedPropertyType.AnimationCurve:
                case SerializedPropertyType.Gradient:
                    hash = (long)property.propertyType;
                    return true;
                default:
                    return TryHash128Fallback(property, out hash);
            }
        }

        static bool TryLeafEquals(SerializedProperty a, SerializedProperty b, out bool equal)
        {
            equal = false;
            if (a.propertyType != b.propertyType)
                return false;

            switch (a.propertyType)
            {
                case SerializedPropertyType.Integer:
                    equal = a.longValue == b.longValue;
                    return true;
                case SerializedPropertyType.Boolean:
                    equal = a.boolValue == b.boolValue;
                    return true;
                case SerializedPropertyType.Float:
                    equal = a.floatValue == b.floatValue;
                    return true;
                case SerializedPropertyType.String:
                    equal = a.stringValue == b.stringValue;
                    return true;
                case SerializedPropertyType.Color:
                    equal = a.colorValue == b.colorValue;
                    return true;
                case SerializedPropertyType.ObjectReference:
                    equal = a.objectReferenceInstanceIDValue == b.objectReferenceInstanceIDValue;
                    return true;
                case SerializedPropertyType.LayerMask:
                case SerializedPropertyType.Enum:
                case SerializedPropertyType.Character:
                    equal = a.intValue == b.intValue;
                    return true;
                case SerializedPropertyType.Vector2:
                    equal = a.vector2Value == b.vector2Value;
                    return true;
                case SerializedPropertyType.Vector3:
                    equal = a.vector3Value == b.vector3Value;
                    return true;
                case SerializedPropertyType.Vector4:
                    equal = a.vector4Value == b.vector4Value;
                    return true;
                case SerializedPropertyType.Vector2Int:
                    equal = a.vector2IntValue == b.vector2IntValue;
                    return true;
                case SerializedPropertyType.Vector3Int:
                    equal = a.vector3IntValue == b.vector3IntValue;
                    return true;
                case SerializedPropertyType.Rect:
                    equal = a.rectValue == b.rectValue;
                    return true;
                case SerializedPropertyType.RectInt:
                    equal = a.rectIntValue.Equals(b.rectIntValue);
                    return true;
                case SerializedPropertyType.Quaternion:
                    equal = a.quaternionValue == b.quaternionValue;
                    return true;
                case SerializedPropertyType.Bounds:
                    equal = a.boundsValue == b.boundsValue;
                    return true;
                case SerializedPropertyType.BoundsInt:
                    equal = a.boundsIntValue.Equals(b.boundsIntValue);
                    return true;
#if UNITY_2022_1_OR_NEWER
                case SerializedPropertyType.Hash128:
                    equal = a.hash128Value == b.hash128Value;
                    return true;
#endif
                case SerializedPropertyType.AnimationCurve:
                case SerializedPropertyType.Gradient:
                    equal = false;
                    return true;
                default:
                    return TryHash128FallbackEquals(a, b, out equal);
            }
        }

        static long HashString(string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;
            long hash = FnvOffset;
            for (int i = 0; i < value.Length; i++)
                hash = Mix(hash, value[i]);
            return hash;
        }

        static long HashColor(Color color)
        {
            long hash = HashVector4(new Vector4(color.r, color.g, color.b, color.a));
            return hash;
        }

        static long HashRect(Rect rect)
        {
            return HashVector4(new Vector4(rect.x, rect.y, rect.width, rect.height));
        }

        static long HashQuaternion(Quaternion quaternion)
        {
            return HashVector4(new Vector4(quaternion.x, quaternion.y, quaternion.z, quaternion.w));
        }

        static long HashVector2(Vector2 value)
        {
            return Mix(Mix(FnvOffset, FloatBits(value.x)), FloatBits(value.y));
        }

        static long HashVector3(Vector3 value)
        {
            return Mix(Mix(Mix(FnvOffset, FloatBits(value.x)), FloatBits(value.y)), FloatBits(value.z));
        }

        static long HashVector4(Vector4 value)
        {
            return Mix(Mix(Mix(Mix(FnvOffset, FloatBits(value.x)), FloatBits(value.y)), FloatBits(value.z)), FloatBits(value.w));
        }

        [StructLayout(LayoutKind.Explicit)]
        struct FloatBitsUnion
        {
            [FieldOffset(0)] public float Float;
            [FieldOffset(0)] public int Int;
        }

        static long FloatBits(float value)
        {
            if (value == 0f)
                return 0;
            return new FloatBitsUnion { Float = value }.Int;
        }

        static long Mix(long hash, long value)
        {
            unchecked
            {
                return (hash ^ value) * FnvPrime;
            }
        }

        static bool TryReadKey(SerializedProperty key, out object value)
        {
            return TryGetBoxedValue(key, out value);
        }

        /// <summary>
        /// boxedValue 从 2022.1 起才有。更早版本按属性路径读目标对象。
        /// </summary>
        static bool TryGetBoxedValue(SerializedProperty property, out object value)
        {
            value = null;
            if (property == null)
                return false;

            try
            {
#if UNITY_2022_1_OR_NEWER
                value = property.boxedValue;
                return true;
#else
                var target = property.serializedObject.targetObject;
                if (target == null)
                    return false;
                value = ReadSerializedValue(target, property.propertyPath);
                return true;
#endif
            }
            catch (Exception)
            {
                value = null;
                return false;
            }
        }

        /// <summary>
        /// managedReferenceId 从 2021.2 起才有。空引用是 -2，未知是 -1。
        /// </summary>
        static bool IsAbsentManagedReference(SerializedProperty property)
        {
#if UNITY_2021_2_OR_NEWER
            return property.managedReferenceId < 0L;
#else
            return string.IsNullOrEmpty(property.managedReferenceFullTypename);
#endif
        }

        static long ManagedReferenceHash(SerializedProperty property)
        {
#if UNITY_2021_2_OR_NEWER
            return property.managedReferenceId;
#else
            return HashString(property.managedReferenceFullTypename);
#endif
        }

        /// <summary>
        /// 2022.1 之前 Hash128 不是 <see cref="SerializedPropertyType"/>，序列化类型名仍是 Hash128。
        /// </summary>
        static bool TryHash128Fallback(SerializedProperty property, out long hash)
        {
            hash = 0;
#if UNITY_2022_1_OR_NEWER
            return false;
#else
            if (property.type != "Hash128" || !TryGetBoxedValue(property, out object boxed) || !(boxed is Hash128 hash128))
                return false;
            hash = hash128.GetHashCode();
            return true;
#endif
        }

        static bool TryHash128FallbackEquals(SerializedProperty a, SerializedProperty b, out bool equal)
        {
            equal = false;
#if UNITY_2022_1_OR_NEWER
            return false;
#else
            if (a.type != "Hash128")
                return false;
            equal = TryGetBoxedValue(a, out object left)
                && TryGetBoxedValue(b, out object right)
                && left is Hash128 hashA
                && right is Hash128 hashB
                && hashA == hashB;
            return true;
#endif
        }

#if !UNITY_2022_1_OR_NEWER
        static object ReadSerializedValue(object root, string path)
        {
            object current = root;
            int start = 0;
            while (start < path.Length)
            {
                if (current == null)
                    return null;

                int dot = path.IndexOf('.', start);
                string part = dot >= 0 ? path.Substring(start, dot - start) : path.Substring(start);
                start = dot >= 0 ? dot + 1 : path.Length;
                if (part == "Array")
                    continue;
                if (part.StartsWith("data[", StringComparison.Ordinal) && part.EndsWith("]"))
                {
                    if (!int.TryParse(part.Substring(5, part.Length - 6), out int index))
                        return null;
                    if (!(current is IList list) || (uint)index >= (uint)list.Count)
                        return null;
                    current = list[index];
                    continue;
                }

                var field = FindField(current.GetType(), part);
                if (field == null)
                    return null;
                current = field.GetValue(current);
            }

            return current;
        }
#endif

        static bool IsNullKey(object key)
        {
            if (key == null)
                return true;

            return key is UnityEngine.Object unityObject && unityObject == null;
        }

        static bool KeysEqual(object comparer, Type keyType, object a, object b)
        {
            if (comparer != null && keyType != null)
            {
                var method = EqualsMethod(comparer.GetType(), keyType);
                if (method != null)
                {
                    try
                    {
                        return (bool)method.Invoke(comparer, new[] { a, b });
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            if (comparer is IEqualityComparer nonGeneric)
                return nonGeneric.Equals(a, b);

            return Equals(a, b);
        }

        static MethodInfo EqualsMethod(Type comparerType, Type keyType)
        {
            if (!s_equalsMethods.TryGetValue(comparerType, out var method))
            {
                var iface = typeof(IEqualityComparer<>).MakeGenericType(keyType);
                method = iface.GetMethod("Equals", new[] { keyType, keyType });
                s_equalsMethods[comparerType] = method;
            }

            return method;
        }

        static object GetComparer(string className)
        {
            if (s_comparerInstances.TryGetValue(className, out var comparer))
                return comparer;

            try
            {
                var comparerType = ResolveType(className);
                comparer = comparerType != null ? Activator.CreateInstance(comparerType) : null;
            }
            catch (Exception)
            {
                comparer = null;
            }

            s_comparerInstances[className] = comparer;
            return comparer;
        }

        static Type ResolveType(string typeName)
        {
            if (s_resolvedTypes.TryGetValue(typeName, out var cached))
                return cached;

            var type = Type.GetType(typeName);
            if (type == null)
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                for (int i = 0; i < assemblies.Length; i++)
                {
                    type = assemblies[i].GetType(typeName);
                    if (type != null)
                        break;
                }
            }

            s_resolvedTypes[typeName] = type;
            return type;
        }

        static Type KeyTypeOf(SerializedProperty property)
        {
            try
            {
                if (s_getFieldInfo == null && !s_fieldInfoLookupFailed)
                {
                    var utility = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.ScriptAttributeUtility");
                    s_getFieldInfo = utility?.GetMethod(
                        "GetFieldInfoFromProperty",
                        BindingFlags.Static | BindingFlags.NonPublic,
                        null,
                        new[] { typeof(SerializedProperty), typeof(Type).MakeByRefType() },
                        null);
                    s_fieldInfoLookupFailed = s_getFieldInfo == null;
                }

                if (s_getFieldInfo != null)
                {
                    object[] args = { property, null };
                    var field = s_getFieldInfo.Invoke(null, args) as FieldInfo;
                    if (field != null)
                    {
                        var keyType = DictionaryKeyType(field.FieldType);
                        if (keyType != null)
                            return keyType;
                    }
                }
            }
            catch (Exception)
            {
                // 内部接口失败时改走字段路径
            }

            return KeyTypeFromPath(property);
        }

        static Type KeyTypeFromPath(SerializedProperty property)
        {
            var target = property.serializedObject.targetObject;
            if (target == null)
                return null;

            Type current = target.GetType();
            string path = property.propertyPath;
            int start = 0;
            while (start < path.Length && current != null)
            {
                int dot = path.IndexOf('.', start);
                string part = dot >= 0 ? path.Substring(start, dot - start) : path.Substring(start);
                start = dot >= 0 ? dot + 1 : path.Length;
                if (part == "Array")
                    continue;
                if (part.StartsWith("data[", StringComparison.Ordinal))
                {
                    current = ElementType(current);
                    continue;
                }

                var field = FindField(current, part);
                if (field == null)
                    return null;
                current = field.FieldType;
            }

            return DictionaryKeyType(current);
        }

        static FieldInfo FindField(Type type, string name)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            while (type != null)
            {
                var field = type.GetField(name, flags);
                if (field != null)
                    return field;
                type = type.BaseType;
            }

            return null;
        }

        static Type ElementType(Type type)
        {
            if (type == null)
                return null;
            if (type.IsArray)
                return type.GetElementType();
            if (type.IsGenericType)
            {
                var args = type.GetGenericArguments();
                if (args.Length > 0)
                    return args[0];
            }

            return type;
        }

        static Type DictionaryKeyType(Type type)
        {
            type = UnwrapCollection(type);
            while (type != null)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(DictionarySerializable<,>))
                    return type.GetGenericArguments()[0];
                type = type.BaseType;
            }

            return null;
        }

        static Type UnwrapCollection(Type type)
        {
            while (type != null)
            {
                if (type.IsArray)
                {
                    type = type.GetElementType();
                    continue;
                }

                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    type = type.GetGenericArguments()[0];
                    continue;
                }

                break;
            }

            return type;
        }

        static void EnsureLength<T>(ref T[] array, int count)
        {
            if (array != null && array.Length >= count)
                return;

            int size = 4;
            while (size < count)
                size *= 2;
            array = new T[size];
        }

        float ContentWidth(Entry entry)
        {
            float width = entry.layoutWidth > 1f ? entry.layoutWidth : _layoutWidth;
            if (width <= 1f)
                width = EditorGUIUtility.currentViewWidth;
            return Mathf.Max(1f, width - EditorGUI.indentLevel * 15f);
        }

        static float MessageHeight(GUIContent content, float width)
        {
            if (width < 1f)
                width = EditorGUIUtility.currentViewWidth;
            return EditorStyles.helpBox.CalcHeight(content, width);
        }

        static void DrawMessage(Rect above, float width, GUIContent content, string message, MessageType type)
        {
            float height = MessageHeight(content, width);
            var rect = new Rect(above.x, above.yMax + EditorGUIUtility.standardVerticalSpacing, width, height);
            EditorGUI.HelpBox(rect, message, type);
        }

        static string CacheId(SerializedProperty property)
        {
            var target = property.serializedObject.targetObject;
            int instanceId = target != null ? target.GetInstanceID() : 0;
            return instanceId + ":" + property.propertyPath;
        }
    }
}
