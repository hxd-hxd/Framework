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
        private IUIGroup _owner;

        public virtual bool isEnable
        {
            get => gameObject.activeSelf;
            set => gameObject.SetActive(value);
        }

        public string itemName => name;

        public virtual IUIGroup group { get => _owner; set => _owner = value; }

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

        public virtual void Init()
        {

        }

        public virtual void Enable(bool isEnable)
        {
            this.isEnable = isEnable;
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
    }
}