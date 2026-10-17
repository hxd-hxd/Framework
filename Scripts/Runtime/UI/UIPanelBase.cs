// -------------------------
// 创建日期：2022/10/27 10:13:13
// -------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>UGUI 界面基类</summary>
    [ExecuteInEditMode]
    public abstract partial class UIPanelBase : UIBase
    {
        [Tooltip("所属画布")]
        [SerializeField] protected Canvas _canvas;

        [Tooltip("自动置顶")]
        [SerializeField] protected bool _autoPanelTopmost = true;// 自动置顶
        [Tooltip("常驻界面")]
        [SerializeField] protected bool _permanent = false;
        [Tooltip("此界面是否启用")]
        [SerializeField] protected bool _isEnable = true;
        [Tooltip("是否阻挡其他射线")]
        [Obsolete]
        [SerializeField] protected bool _isResistRay = true;

        public Action OnEnableEvent;
        public Action OnDisableEvent;
        public Action OnDestroyEvent;

        /// <summary>自动置顶</summary>
        public virtual bool AutoPanelTopmost { get => _autoPanelTopmost; set => _autoPanelTopmost = value; }

        /// <summary>界面所属画布</summary>
        public Canvas canvas { get => _canvas; set => _canvas = value; }

        /// <summary>此界面是否启用
        /// <para>注意：此设置只是一个额外的标识，不会真的影响界面的显示，显示界面请使用 <see cref="Enable(bool)"/></para>
        /// </summary>
        public override bool isEnable
        {
            get
            {
                if (gameObject == null) return false;
                return gameObject.activeInHierarchy && _isEnable;
            }
            set
            {
                _isEnable = value;
            }
        }

        /// <summary>常驻界面</summary>
        public bool Permanent { get => _permanent; set => _permanent = value; }

        /// <summary>是否阻挡其他射线
        /// <para>注意：获取时同时启用 <see cref="isEnable"/> 才有效</para>
        /// </summary>
        [Obsolete]
        public virtual bool IsResistRay
        {
            get => _isResistRay && isEnable;
            set => _isResistRay = value;
        }

        /* Unity UGUI有两种调整排序的方式：
        一种是调整画布属性，全局排序；
        一种是调整节点顺序，本地排序。
        接口已提供一个属性的获取，由实现类自己决定实现哪一个，也可以两个都实现，但管理器对排序的判断也只有接口属性。

        为了使排序相关的逻辑不那么复杂，这里强制非根画布的 ui 使用本地排序，有根画布的 ui 使用全局排序
        */
        public override int sortOrder
        {
            get
            {
                int so = 0;
                if (_canvas)
                {
                    if (_canvas.isRootCanvas)
                    {
                        so = _canvas.sortingOrder;
                    }
                    else
                    {
                        //if (_canvas.overrideSorting)
                        //    so = _canvas.sortingOrder;
                        //else
                        so = base.sortOrder;
                    }
                }
                return so;
            }
            set
            {
                if (_canvas)
                {
                    if (_canvas.isRootCanvas)
                    {
                        _canvas.sortingOrder = value;
                    }
                    else
                    {
                        //if (_canvas.overrideSorting)
                        //    _canvas.sortingOrder = value;
                        //else
                        base.sortOrder = value;
                    }
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();

            if (!_canvas) _canvas = GetComponentInParent<Canvas>(true);

            UIPanelManager.Register(this);
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            UIPanelManager.GetCanvas(this);

            if (_autoPanelTopmost)
                this.PanelTopmost();

            OnEnableEvent?.Invoke();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UIPanelManager.RemovePanel(this);

            OnDestroyEvent?.Invoke();
        }

        protected override void OnDisable()
        {
            OnDisableEvent?.Invoke();
        }

        /// <summary>启用界面</summary>
        public override void Enable(bool enable)
        {
            if (_autoPanelTopmost)
                this.PanelTopmost();

            if (gameObject.activeSelf == enable) return;
            gameObject.SetActive(enable);
        }

        /// <summary>销毁界面</summary>
        public override void Destroy()
        {
            base.Destroy();
            this.DestroyPanel();
        }
    }
}