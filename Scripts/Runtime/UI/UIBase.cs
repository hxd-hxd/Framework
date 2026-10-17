using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Framework.UI;

namespace Framework
{
    /// <summary>UI 基类</summary>
    public abstract partial class UIBase : MonoBehaviour, IUI, IUIGroupItem
    {
        [SerializeField]
        private UIGroupBase _group;

        public virtual bool isEnable
        {
            get => gameObject.activeSelf;
            set => gameObject.SetActive(value);
        }

        public string itemName => name;

        public IUIGroup rootGroup
        {
            get
            {
                var g = group;
                while (true)
                {
                    if (g is IUIGroupItem groupItem && groupItem.group != null)
                    {
                        g = groupItem.group;
                        continue;
                    }
                    break;
                }
                return g;
            }
        }

        public virtual IUIGroup group { get => _group; set => _group = value as UIGroupBase; }

        public virtual int sortOrder
        {
            get => transform.GetSiblingIndex();
            set
            {
                // 只在有父节点时才调整排序，不然没有意义
                if (transform.parent)
                {
                    int so = Mathf.Clamp(value, 0, transform.parent.childCount - 1);
                    transform.SetSiblingIndex(so);
                }
            }
        }

        string IUI.name
        {
            get => name;
            set => name = value;
        }

        protected virtual void Awake()
        {
            
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
        }

        public virtual void OnRefresh()
        {

        }

        public virtual void OnTopmost()
        {
            transform.SetAsLastSibling();
        }

        public virtual void OnCovered()
        {

        }

        public virtual void OnNotCovered()
        {

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

            // 更新所属分组
            if (oldGroup == group) return;
            if (oldGroup) oldGroup.InternalUpdateGetChildItem();
            if (group) group.InternalUpdateGetChildItem();
        }
    }
}