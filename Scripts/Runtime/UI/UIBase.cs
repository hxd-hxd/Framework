using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Serialization;
using Framework.UI;

namespace Framework
{
    /// <summary>UI 基类</summary>
    public abstract partial class UIBase : MonoBehaviour, IUI
    {
        [SerializeField]
        private UIGroupBase _group;

        [SerializeField]
        [HideInInspector]
        private TopmostIgnoreComp _topmostIgnoreComp;

        [Tooltip("自动置顶")]
        [SerializeField]
        [FormerlySerializedAs("_autoPanelTopmost")]
        private bool _topmostInEnabled = true;// 自动置顶

        /// <summary>控制此 UI 自身的启用，如要置顶等请调用 <see cref="Enable(bool)"/></summary>
        public virtual bool isEnable
        {
            get => gameObject.activeSelf;
            set => gameObject.SetActive(value);
        }

        public virtual IUIGroup group
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

        public virtual int sortOrder
        {
            get => transform.GetSiblingIndex();
            set
            {
                // 只在有父节点时才调整排序，不然没有意义
                if (transform.parent)
                {
                    transform.SetSiblingIndex(value);
                }
            }
        }

        public virtual bool topmostIgnore => _topmostIgnoreComp != null;

        public virtual bool topmostInEnabled { get => _topmostInEnabled; set => _topmostInEnabled = value; }

        public IUIGroup rootGroup
        {
            get
            {
                var g = group;
                while (!ObjectUtility.IsNull(g))
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

        string IUI.name
        {
            get => name;
            set => name = value;
        }
        string IUIGroupItem.itemName { get => name; set => name = value; }

        protected virtual void Awake()
        {
            if (_topmostIgnoreComp == null) TryGetComponent(out _topmostIgnoreComp);

        }

        protected virtual void Start()
        {

        }

        protected virtual void OnEnable()
        {
            
        }

        protected virtual void OnDisable()
        {

        }

        protected virtual void OnDestroy()
        {

        }

        private void OnTransformParentChanged()
        {
            //Debug.Log($"{name} 父级切换");
            if (!Application.isEditor) return;

            UpdateParentGroup();
        }

        public virtual void Init()
        {

        }

        /// <summary>启用或禁用，如重写，需和 <see cref="isEnable"/> 相 配合</summary>
        public virtual void Enable(bool isEnable)
        {
            UIManager.Instance.EnableUI(this, isEnable);
        }

        /// <summary>刷新</summary>
        public void Refresh()
        {
            OnRefresh();
        }

        /// <summary>销毁界面</summary>
        public virtual void Destroy()
        {

        }

        public virtual void OnEnabled(bool isEnable)
        {
            this.isEnable = isEnable;
            OnRefresh();
        }

        public virtual void OnRefresh()
        {

        }

        public virtual void OnTopmost()
        {
            //transform.SetAsLastSibling();

            Debug.Log($"{name} 置顶");
        }

        public virtual void OnCovered()
        {
            Debug.Log($"{name} 被覆盖");
        }

        public virtual void OnNotCovered()
        {
            // 这里的未覆盖其实就是自身又到顶部了，所以直接通知置顶就行了
            OnTopmost();
        }

        public static void AddEvent(Button btn, UnityAction e)
        {
            if (btn)
            {
                btn.onClick.AddListener(e);
            }
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
    }
}