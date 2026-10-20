using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.UI
{
    /// <summary>UI 分组基类</summary>
    public abstract class UIGroupBase : MonoBehaviour, IUIGroup
    {
        [SerializeField]
        private string _groupName = "默认";

        [SerializeField]
        [HideInInspector]
        private TopmostIgnoreComp _topmostIgnoreComp;

        private List<IUIGroupItem> _items = new List<IUIGroupItem>(4);

        public abstract int sortOrder { get; set; }

        public abstract IUIGroup rootGroup { get; }

        public abstract IUIGroup group { get; set; }

        public abstract IUIGroup CreateSubGroup(string name);

        public virtual bool isRootGroup => ObjectUtility.IsNull(group);

        public virtual string groupName { get => _groupName; set => _groupName = value; }

        public virtual bool isEnable { get => gameObject.activeSelf; set => gameObject.SetActive(value); }

        public virtual int itemCount => _items.Count;

        public virtual bool topmostIgnore => _topmostIgnoreComp != null;

        public int globalSortOrder
        {
            get
            {
                // 这里用分组判空主要是节省性能
                if (!ObjectUtility.IsNull(group))
                    return rootGroup.sortOrder;
                return sortOrder;
            }
        }

        public IReadOnlyList<IUIGroupItem> items => _items;

        internal List<IUIGroupItem> internalItems => _items;

        string IUIGroupItem.itemName { get => _groupName; set => _groupName = value; }

        protected virtual void Awake()
        {
            if (_topmostIgnoreComp == null) TryGetComponent(out _topmostIgnoreComp);

            // 自动收集子节点项
            InternalUpdateGetChildItem();
        }

        public virtual void AddItem(IUIGroupItem item)
        {
            if (_items.Contains(item)) return;

            if (!Equals(item.group, this))
            {
                item.group = this;
                return;
            }
            else
            {
                if (item is Component comp)
                {
                    if (comp.transform.parent != transform)
                        comp.transform.SetParent(transform);
                }
            }
            _items.Add(item);
        }

        public virtual void RemoveItem(IUIGroupItem item)
        {
            int i = _items.IndexOf(item);
            if (i >= 0)
            {
                if (Equals(item.group, this))
                {
                    if (item is Component comp)
                    {
                        if (comp.transform.parent == transform)
                            comp.transform.SetParent(transform.parent);
                    }
                    item.group = null;
                }
                else
                {
                    _items.RemoveAt(i);
                }
            }
        }

        public virtual bool ContainsItem(IUIGroupItem item)
        {
            return _items.Contains(item);
        }

        public virtual IUIGroupItem GetItem(string name)
        {
            var item = _items.Find(i => i.itemName == name);
            return item;
        }

        /// <summary>
        /// 移除已销毁的项。编辑器撤销创建子分组后，对象会被销毁，但这份运行时列表不会跟着撤销。
        /// </summary>
        public void RemoveDestroyedItems()
        {
            for (int i = _items.Count - 1; i >= 0; i--)
            {
                if (ObjectUtility.IsNull(_items[i]))
                    _items.RemoveAt(i);
            }
        }

        public virtual IUIGroupItem GetTopItem()
        {
            RemoveDestroyedItems();
            // 同级中如果顶部项是 UI，就不需要往下找了
            // 获取顶部项时要添加子组检测
            IUIGroupItem topItem = null;
            for (int i = internalItems.Count - 1; i >= 0; i--)
            {
                var item = internalItems[i];
                if (ObjectUtility.IsNull(item)
                    || !item.isEnable
                    || item.topmostIgnore)
                    continue;

                // 如果是子组
                if (item is IUIGroup iGroup)
                {
                    if (!iGroup.HasEnableItem()) continue;
                    if (topItem == null || iGroup.sortOrder > topItem.sortOrder)
                        topItem = item;
                    continue;
                }

                if (topItem == null || item.sortOrder > topItem.sortOrder)
                    topItem = item;
            }
            // 继续往下找
            if (topItem is IUIGroup tGroup)
            {
                topItem = tGroup.GetTopItem();
            }
            return topItem;
        }

        public virtual IUIGroupItem GetMaxSortOrderItem()
        {
            //return _items.Count > 0 ? _items[itemCount - 1] : null;
            IUIGroupItem maxItem = null;
            for (int i = 0; i < _items.Count; i++)
            {
                var item = _items[i];

                if (ObjectUtility.IsNull(maxItem))
                {
                    maxItem = item;
                    continue;
                }

                if (item.sortOrder > maxItem.sortOrder)
                {
                    maxItem = item;
                }
            }
            return maxItem;
        }

        public virtual bool HasEnableItem()
        {
            RemoveDestroyedItems();
            for (int i = _items.Count - 1; i >= 0; i--)
            {
                // 组中按顺序排，所以从后往前找
                var item = _items[i];
                if (item.isEnable) return true;
            }
            return false;
        }

        public virtual void OnCovered()
        {
            // 转发覆盖通知
            IUIGroupItem topItem = GetTopItem();
            if (!ObjectUtility.IsNull(topItem)) topItem.OnCovered();
        }

        public virtual void OnNotCovered()
        {
            // 转发非覆盖通知
            IUIGroupItem topItem = GetTopItem();
            if (!ObjectUtility.IsNull(topItem)) topItem.OnNotCovered();
        }

        public IEnumerator<IUIGroupItem> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        /// <summary>根性获取子节点中的项</summary>
        internal void InternalUpdateGetChildItem()
        {
            //_items.Clear();
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (child.TryGetComponent(out IUIGroupItem item))
                {
                    if (!Equals(item.group, this))
                        item.group = this;
                    else
                        _items.Add(item);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
