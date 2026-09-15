// -------------------------
// 创建日期：2026/9/10 17:41:00
// -------------------------

using System;
using System.Collections.Generic;
using System.Text;
using Framework.ObjectPool;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    /// <summary>
    /// <see cref="ObjectPoolManager"/> 检视面板：保持默认绘制，
    /// 并显示 <see cref="TypePool"/>、<see cref="GameObjectPool"/> 中各池的对象信息。
    /// </summary>
    [CustomEditor(typeof(ObjectPoolManager))]
    public class ObjectPoolManagerInspector : UnityEditor.Editor
    {
        static readonly GUILayoutOption CountWidth = GUILayout.Width(64);

        readonly List<KeyValuePair<Type, int>> _typeCounts = new List<KeyValuePair<Type, int>>();
        readonly List<KeyValuePair<int, int>> _arrayLengthCounts = new List<KeyValuePair<int, int>>();
        readonly List<KeyValuePair<GameObject, List<GameObject>>> _goPools = new List<KeyValuePair<GameObject, List<GameObject>>>();
        readonly Dictionary<int, bool> _goInstanceFoldouts = new Dictionary<int, bool>();
        readonly StringBuilder _typeNameBuilder = new StringBuilder(64);

        string _filter = "";
        string _goFilter = "";
        bool _objectPoolFoldout = true;
        bool _arrayPoolFoldout = true;
        bool _goPoolFoldout = true;

        public override bool RequiresConstantRepaint() => Application.isPlaying;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.LabelField("TypePool", EditorStyles.boldLabel);
            DrawTypePool(TypePool.root);

            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField("GameObjectPool", EditorStyles.boldLabel);
            DrawGameObjectPool(GameObjectPool.root);
        }

        void DrawTypePool(TypePool pool)
        {
            if (pool == null)
            {
                EditorGUILayout.HelpBox("TypePool 为空", MessageType.Info);
                return;
            }

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.IntField("池类型数", pool.poolCount);
                EditorGUILayout.IntField("对象总数", pool.itemCount);
            }

            _filter = EditorGUILayout.TextField("筛选", _filter);

            DrawObjectPool(pool);
            DrawArrayPool(pool.arrayPool);
        }

        void DrawObjectPool(TypePool pool)
        {
            IReadOnlyDictionary<Type, List<object>> dict = pool.pool;
            int typeCount = dict != null ? dict.Count : 0;

            _objectPoolFoldout = EditorGUILayout.Foldout(_objectPoolFoldout, $"对象池（{typeCount}）", true);
            if (!_objectPoolFoldout)
                return;

            if (dict == null || typeCount == 0)
            {
                EditorGUILayout.LabelField("（空）");
                return;
            }

            _typeCounts.Clear();
            try
            {
                foreach (var kv in dict)
                {
                    if (!MatchFilter(kv.Key))
                        continue;
                    _typeCounts.Add(new KeyValuePair<Type, int>(kv.Key, kv.Value != null ? kv.Value.Count : 0));
                }
            }
            catch (InvalidOperationException)
            {
                EditorGUILayout.HelpBox("对象池正在变化，稍后刷新。", MessageType.None);
                Repaint();
                return;
            }

            _typeCounts.Sort(CompareTypeCount);
            DrawTypeCountHeader();
            DrawTypeCountRows();
        }

        void DrawArrayPool(ArrayPool arrayPool)
        {
            IReadOnlyDictionary<Type, Dictionary<int, List<Array>>> dict = arrayPool != null ? arrayPool.pool : null;
            int typeCount = dict != null ? dict.Count : 0;

            _arrayPoolFoldout = EditorGUILayout.Foldout(_arrayPoolFoldout, $"数组池（{typeCount}）", true);
            if (!_arrayPoolFoldout)
                return;

            if (arrayPool == null || dict == null || typeCount == 0)
            {
                EditorGUILayout.LabelField("（空）");
                return;
            }

            _typeCounts.Clear();
            try
            {
                foreach (var kv in dict)
                {
                    if (!MatchFilter(kv.Key))
                        continue;
                    _typeCounts.Add(new KeyValuePair<Type, int>(kv.Key, arrayPool.GetFreeCount(kv.Key)));
                }
            }
            catch (InvalidOperationException)
            {
                EditorGUILayout.HelpBox("数组池正在变化，稍后刷新。", MessageType.None);
                Repaint();
                return;
            }

            _typeCounts.Sort(CompareTypeCount);
            DrawTypeCountHeader();

            EditorGUI.indentLevel++;
            for (int i = 0; i < _typeCounts.Count; i++)
            {
                var item = _typeCounts[i];
                DrawTypeCountRow(item.Key, item.Value);
                DrawArrayLengthRows(dict, item.Key);
            }
            EditorGUI.indentLevel--;
        }

        void DrawGameObjectPool(GameObjectPool pool)
        {
            if (pool == null)
            {
                EditorGUILayout.HelpBox("GameObjectPool 为空", MessageType.Info);
                return;
            }

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.IntField("池数量", pool.poolCount);
                EditorGUILayout.IntField("对象总数", pool.itemSize);
                EditorGUILayout.ObjectField("默认模板", pool.template, typeof(GameObject), true);
                EditorGUILayout.ObjectField("回收父节点", pool.returnParent, typeof(Transform), true);
                EditorGUILayout.IntField("预创建协程", pool.preCreateInstanceCoroutineNum);
            }

            _goFilter = EditorGUILayout.TextField("筛选", _goFilter);

            IReadOnlyDictionary<GameObject, List<GameObject>> dict = pool.pool;
            int poolCount = dict != null ? dict.Count : 0;

            _goPoolFoldout = EditorGUILayout.Foldout(_goPoolFoldout, $"模板池（{poolCount}）", true);
            if (!_goPoolFoldout)
                return;

            if (dict == null || poolCount == 0)
            {
                EditorGUILayout.LabelField("（空）");
                return;
            }

            _goPools.Clear();
            try
            {
                foreach (var kv in dict)
                {
                    if (!MatchGoFilter(kv.Key))
                        continue;
                    _goPools.Add(kv);
                }
            }
            catch (InvalidOperationException)
            {
                EditorGUILayout.HelpBox("GameObject 池正在变化，稍后刷新。", MessageType.None);
                Repaint();
                return;
            }

            _goPools.Sort(CompareGoPool);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("模板", EditorStyles.miniBoldLabel);
            EditorGUILayout.LabelField("数量", EditorStyles.miniBoldLabel, CountWidth);
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel++;
            for (int i = 0; i < _goPools.Count; i++)
                DrawGoPoolRow(_goPools[i].Key, _goPools[i].Value);
            EditorGUI.indentLevel--;
        }

        void DrawGoPoolRow(GameObject template, List<GameObject> items)
        {
            int count = items != null ? items.Count : 0;
            int id = template != null ? template.GetInstanceID() : 0;
            if (!_goInstanceFoldouts.TryGetValue(id, out bool foldout))
                foldout = false;

            EditorGUILayout.BeginHorizontal();
            foldout = EditorGUILayout.Foldout(foldout, GUIContent.none, true);
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.ObjectField(template, typeof(GameObject), true);
                EditorGUILayout.LabelField(count.ToString(), CountWidth);
            }
            EditorGUILayout.EndHorizontal();

            _goInstanceFoldouts[id] = foldout;
            if (!foldout)
                return;

            EditorGUI.indentLevel++;
            if (items == null || items.Count == 0)
            {
                EditorGUILayout.LabelField("（空）");
            }
            else
            {
                using (new EditorGUI.DisabledScope(true))
                {
                    for (int i = 0; i < items.Count; i++)
                    {
                        var instance = items[i];
                        var label = new GUIContent($"[{i}]", instance ? instance.name : "(已销毁)");
                        EditorGUILayout.ObjectField(label, instance, typeof(GameObject), true);
                    }
                }
            }
            EditorGUI.indentLevel--;
        }

        bool MatchGoFilter(GameObject template)
        {
            if (string.IsNullOrEmpty(_goFilter))
                return true;
            if (template == null)
                return false;
            return template.name.IndexOf(_goFilter, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        static int CompareGoPool(KeyValuePair<GameObject, List<GameObject>> a, KeyValuePair<GameObject, List<GameObject>> b)
        {
            string nameA = a.Key != null ? a.Key.name : "";
            string nameB = b.Key != null ? b.Key.name : "";
            int nameCompare = string.Compare(nameA, nameB, StringComparison.Ordinal);
            if (nameCompare != 0)
                return nameCompare;
            int idA = a.Key != null ? a.Key.GetInstanceID() : 0;
            int idB = b.Key != null ? b.Key.GetInstanceID() : 0;
            return idA.CompareTo(idB);
        }

        void DrawArrayLengthRows(IReadOnlyDictionary<Type, Dictionary<int, List<Array>>> dict, Type type)
        {
            if (dict == null || !dict.TryGetValue(type, out var lengthPool) || lengthPool == null || lengthPool.Count <= 0)
                return;

            _arrayLengthCounts.Clear();
            foreach (var kv in lengthPool)
                _arrayLengthCounts.Add(new KeyValuePair<int, int>(kv.Key, kv.Value != null ? kv.Value.Count : 0));
            _arrayLengthCounts.Sort((a, b) => a.Key.CompareTo(b.Key));

            EditorGUI.indentLevel++;
            using (new EditorGUI.DisabledScope(true))
            {
                for (int i = 0; i < _arrayLengthCounts.Count; i++)
                {
                    var item = _arrayLengthCounts[i];
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"length {item.Key}");
                    EditorGUILayout.LabelField(item.Value.ToString(), CountWidth);
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUI.indentLevel--;
        }

        void DrawTypeCountHeader()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("类型", EditorStyles.miniBoldLabel);
            EditorGUILayout.LabelField("数量", EditorStyles.miniBoldLabel, CountWidth);
            EditorGUILayout.EndHorizontal();
        }

        void DrawTypeCountRows()
        {
            EditorGUI.indentLevel++;
            for (int i = 0; i < _typeCounts.Count; i++)
            {
                var item = _typeCounts[i];
                DrawTypeCountRow(item.Key, item.Value);
            }
            EditorGUI.indentLevel--;
        }

        void DrawTypeCountRow(Type type, int count)
        {
            string typeName = GetTypeDisplayName(type);
            var label = new GUIContent(typeName, type != null ? type.FullName : typeName);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label);
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.LabelField(count.ToString(), CountWidth);
            }
            EditorGUILayout.EndHorizontal();
        }

        bool MatchFilter(Type type)
        {
            if (string.IsNullOrEmpty(_filter))
                return true;
            if (type == null)
                return false;

            string displayName = GetTypeDisplayName(type);
            return displayName.IndexOf(_filter, StringComparison.OrdinalIgnoreCase) >= 0
                || (type.FullName != null && type.FullName.IndexOf(_filter, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        int CompareTypeCount(KeyValuePair<Type, int> a, KeyValuePair<Type, int> b)
        {
            return string.Compare(GetTypeDisplayName(a.Key), GetTypeDisplayName(b.Key), StringComparison.Ordinal);
        }

        string GetTypeDisplayName(Type type)
        {
            if (type == null)
                return "(null)";

            _typeNameBuilder.Clear();
            AppendTypeDisplayName(type, _typeNameBuilder);
            return _typeNameBuilder.ToString();
        }

        static void AppendTypeDisplayName(Type type, StringBuilder sb)
        {
            if (type.IsArray)
            {
                AppendTypeDisplayName(type.GetElementType(), sb);
                int rank = type.GetArrayRank();
                sb.Append('[');
                sb.Append(',', rank - 1);
                sb.Append(']');
                return;
            }

            if (type.IsGenericType)
            {
                if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    AppendTypeDisplayName(type.GetGenericArguments()[0], sb);
                    sb.Append('?');
                    return;
                }

                string name = type.Name;
                int tick = name.IndexOf('`');
                if (tick >= 0)
                    name = name.Substring(0, tick);
                sb.Append(name);
                sb.Append('<');
                var args = type.GetGenericArguments();
                for (int i = 0; i < args.Length; i++)
                {
                    if (i > 0)
                        sb.Append(", ");
                    AppendTypeDisplayName(args[i], sb);
                }
                sb.Append('>');
                return;
            }

            sb.Append(type.Name);
        }
    }
}
