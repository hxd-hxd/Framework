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

        private List<IUIGroupItem> _items = new List<IUIGroupItem>(4);

        public abstract int sortOrder { get; set; }

        public virtual string groupName { get => _groupName; set => _groupName = value; }

        public virtual int itemCount => _items.Count;

        public bool isEnable { get => gameObject.activeSelf; set => gameObject.SetActive(value); }

        public IReadOnlyList<IUIGroupItem> items => _items;

        internal List<IUIGroupItem> internalItems => _items;

        protected virtual void Awake()
        {
            // 自动收集子节点项
            InternalUpdateGetChildItem();
        }

        public virtual void AddItem(IUIGroupItem item)
        {
            if (_items.Contains(item)) return;

            item.group = this;
            if (item is Component comp)
            {
                if (transform.parent != comp.transform.parent)
                    comp.transform.SetParent(transform);
            }

            _items.Add(item);
        }

        public virtual void RemoveItem(IUIGroupItem item)
        {
            if (_items.Remove(item))
            {
                item.group = null;
                if (item is Component comp)
                {
                    if (transform.parent == comp.transform.parent)
                        comp.transform.SetParent(transform.parent);
                }
            }
        }

        public virtual IUIGroupItem GetItem(string name)
        {
            var item = _items.Find(i => i.itemName == name);
            return item;
        }

        public virtual IUIGroupItem GetTopItem()
        {
            for (int i = _items.Count - 1; i >= 0; i--)
            {
                // 组中按顺序排，所以从后往前找到的第一个就是顶部的
                var item = _items[i];
                if (item.isEnable) return item;
            }
            return null;
        }

        public virtual bool HasEnableItem()
        {
            for (int i = _items.Count - 1; i >= 0; i--)
            {
                // 组中按顺序排，所以从后往前找
                var item = _items[i];
                if (item.isEnable) return true;
            }
            return false;
        }

        public IEnumerator<IUIGroupItem> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        /// <summary>根性获取子节点中的项</summary>
        internal void InternalUpdateGetChildItem()
        {
            _items.Clear();
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (child.TryGetComponent(out IUIGroupItem item))
                {
                    item.group = this;
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
