using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Framework
{
    /// <summary>
    /// UGUI 基类
    /// </summary>
    public abstract partial class UIBase : MonoBehaviour, IUI
    {
        string IUI.name
        {
            get => name;
            set => name = value;
        }
        public virtual bool isEnable
        {
            get => gameObject.activeSelf;
            set => _Enable(value);
        }

        protected virtual void Awake()
        {
            UIManager.Register(this);
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
            UIManager.Unregister(this);
        }

        /// <summary>
        /// 在刷新的时候触发
        /// </summary>
        protected virtual void OnRefresh()
        {

        }

        public virtual void Init()
        {

        }

        /// <summary>
        /// 反向启用。即当前为启用则禁用，当前为禁用则启用
        /// </summary>
        public void Enable() => _Enable(gameObject.activeSelf);
        public virtual void Enable(bool isEnable)
        {
            _Enable(isEnable);
        }
        protected void _Enable(bool isEnable)
        {
            if (gameObject.activeSelf == isEnable) return;

            gameObject.SetActive(isEnable);
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public void Refresh()
        {
            OnRefresh();
        }

        /// <summary>
        /// 销毁界面
        /// </summary>
        public virtual void Destroy()
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