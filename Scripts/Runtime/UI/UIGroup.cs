using System.Collections.Generic;
using UnityEngine;

namespace Framework.UI
{
    /// <summary>UI 分组</summary>
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public sealed class UIGroup : UIGroupBase
    {
        [SerializeField]
        private int _sortOrder;
        [SerializeField]
        private Canvas _canvas;
        [SerializeField]
        private UIGroupBase _group;

        public override int sortOrder
        {
            get
            {
                if (!isRootGroup)
                {
                    return transform.GetSiblingIndex();
                }
                return _sortOrder;
            }
            set
            {
                bool needUpdate = _sortOrder != value;
                _sortOrder = value;
                if (needUpdate) UpdateSelfSort();
            }
        }

        public override IUIGroup rootGroup
        {
            get
            {
                IUIGroup g = this;
                while (true)
                {
                    // 溯源
                    if (!ObjectUtility.IsNull(g.group))
                    {
                        g = g.group;
                        continue;
                    }
                    break;
                }
                return g;
            }
        }

        public override IUIGroup group
        {
            get => _group;
            set
            {
                if (Equals(_group, value)) return;

                // 缓存旧的
                var oldGroup = _group;

                // 设置所属分组
                _group = value as UIGroupBase;

                // 更新所属分组
                if (oldGroup) oldGroup.RemoveItem(this);
                if (_group) _group.AddItem(this);
            }
        }

        public string itemName => groupName;

        public Canvas canvas { get => _canvas; set => _canvas = value; }

        public bool useCanvasSortOrder => isRootGroup || _canvas.isRootCanvas || (!_canvas.isRootCanvas && _canvas.overrideSorting);

        protected override void Awake()
        {
            base.Awake();
            if (!_canvas) TryGetComponent(out _canvas);
            if (_canvas)
            {
                _canvas.overrideSorting = true;
            }

            _sortOrder = sortOrder;
            UpdateParentGroup();
        }

        private void Start()
        {
            if (Application.isPlaying)
                UpdateSelfSort();
        }

        private void OnTransformParentChanged()
        {
            //Debug.Log($"{name} 父级切换");
            if (!Application.isEditor) return;

            UpdateParentGroup();
        }

        public override void AddItem(IUIGroupItem item)
        {
            base.AddItem(item);

            // 更新排序
            if (item is UIGroup group)
            {
                group.UpdateSelfSort();
            }
        }

        public override IUIGroup CreateSubGroup(string name)
        {
            // 不能同名
            if (!ObjectUtility.IsNull(internalItems.Find(item => item is IUIGroup iGroup && iGroup.itemName == name)))
                new System.Exception("创建失败，已有同名子组");

            var go = new GameObject(name);
            go.transform.SetParent(transform);
#if UNITY_2022_1_OR_NEWER
            go.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
#else
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
#endif
            var group = go.AddComponent<UIGroup>();
            group.groupName = name;
            internalItems.Add(group);
            return group;
        }

        public override bool HasEnableItem()
        {
            RemoveDestroyedItems();
            // 检查子组有效项
            foreach (var item in internalItems)
            {
                if (!item.isEnable) continue;
                if (item is IUIGroup iGroup)
                {
                    if (iGroup.HasEnableItem()) return true;
                    continue;
                }
                return true;
            }
            return false;
        }

        /// <summary>设置排序并不更新</summary>
        public void SetSortOrderNotUpdate(int sortOrder)
        {
            _sortOrder = sortOrder;
        }

        /// <summary>更新自身排序</summary>
        public void UpdateSelfSort()
        {
            if (_canvas)
            {
                _canvas.sortingOrder = _sortOrder;
                _canvas.overrideSorting = isRootGroup;
            }

            // 需调整层级顺序，不调整根节点顺序，因为根节点只能通过控制 canvas.sortingOrder 实现排序
            if (!isRootGroup)
                UpdateSiblingIndex();
        }

        private void UpdateParentGroup()
        {
            // 缓存旧的
            UIGroupBase oldGroup = null;
            if (this.group is UIGroupBase g) oldGroup = g;

            // 设置所属分组
            transform.parent.TryGetComponent(out UIGroupBase group);
            this.group = group;
        }

        internal void UpdateSiblingIndex()
        {
            if (transform.parent)
            {
                int i = transform.GetSiblingIndex();
                int target = i;

                // 检查是否比前面的小
                int min = i;
                while ((min -= 1) >= 0)
                {
                    var t = transform.parent.GetChild(min);
                    if (t.TryGetComponent(out IUIGroup group))
                    {
                        if (_sortOrder < group.sortOrder)
                        {
                            target = t.GetSiblingIndex();
                            continue;
                        }
                    }
                    // 前一个节点不是分组就继续往前
                    else continue;

                    // 前面没有更大的
                    break;
                }
                if (target != i) transform.SetSiblingIndex(target);

                // 检查是否比后面的大
                i = transform.GetSiblingIndex();
                target = i;
                int count = transform.parent.childCount;
                int max = i;
                while ((max += 1) < count)
                {
                    var t = transform.parent.GetChild(max);
                    if (t.TryGetComponent(out IUIGroup group))
                    {
                        if (_sortOrder > group.sortOrder)
                        {
                            target = t.GetSiblingIndex();
                            continue;
                        }
                    }
                    // 后一个节点不是分组就继续往后
                    else continue;

                    // 后面没有更小的
                    break;
                }
                if (target != i) transform.SetSiblingIndex(target);
            }
        }
    }
}
