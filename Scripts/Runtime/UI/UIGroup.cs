using System.Collections.Generic;
using UnityEngine;

namespace Framework.UI
{
    /// <summary>UI 分组</summary>
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class UIGroup : UIGroupBase, IUIGroupItem
    {
        [SerializeField]
        private int _sortOrder;
        [SerializeField]
        private Canvas _canvas;
        [SerializeField]
        private UIGroupBase _group;

        public override int sortOrder
        {
            get => _sortOrder;
            set
            {
                bool needUpdate = _sortOrder != value;
                _sortOrder = value;
                if (needUpdate) UpdateSelfSort();
            }
        }

        public string itemName => groupName;

        public IUIGroup group { get => _group; set => _group = value as UIGroupBase; }

        public Canvas canvas { get => _canvas; set => _canvas = value; }

        public bool isRootGroup => group == null;

        protected override void Awake()
        {
            base.Awake();
            if (!_canvas) TryGetComponent(out _canvas);
            if (_canvas)
            {
                _canvas.overrideSorting = true;
            }

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
            if (Application.isPlaying) return;

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

        public override IUIGroupItem GetTopItem()
        {
            //return base.GetTopItem();

            // TODO：获取顶部项时要添加子组检测
            IUIGroupItem topItem = null;
            for (int i = internalItems.Count - 1; i >= 0; i--)
            {
                var item = internalItems[i];
                if (!item.isEnable) continue;

                if (topItem == null)
                {
                    topItem = item;
                    continue;
                }

                // 如果是子组
                if (item is IUIGroup iGroup)
                {
                    if (iGroup.HasEnableItem())
                        if (iGroup.sortOrder > topItem.sortOrder)
                        {
                            topItem = item;
                        }
                    continue;
                }

                if (item.sortOrder > topItem.sortOrder)
                {
                    topItem = item;
                }
            }
            return topItem;
        }

        public override bool HasEnableItem()
        {
            // 检查子组有效项
            foreach (var item in internalItems)
            {
                if (!item.isEnable) continue;
                if (item is IUIGroup iGroup)
                {
                    if (iGroup.HasEnableItem()) return true;
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

        public void OnCovered()
        {
            // 当是子组时，转发覆盖通知
            IUIGroupItem topItem = GetTopItem();

            if (topItem != null)
            {
                topItem.OnCovered();
            }
        }

        public void OnNotCovered()
        {
            // TODO：转发非覆盖通知
            throw new System.NotImplementedException();
        }

        private void UpdateParentGroup()
        {
            transform.parent.TryGetComponent(out UIGroup group);
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
                        if (sortOrder < group.sortOrder)
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
                        if (sortOrder > group.sortOrder)
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
