using System;
using System.Collections;
using System.Collections.Generic;
using Framework;
using UnityEngine;

namespace Framework
{
    /// <summary>选择项基类</summary>
    public abstract class SelectItemBase : MonoBehaviour, ISelectItem
    {
        public abstract bool isSelect { get; set; }

        public abstract bool canSelect { get; set; }

        public abstract Action<bool> onSelectChanged { get; set; }
        
        /// <summary>设置选择时不通知外部事件</summary>
        public abstract void SetIsSelectWithoutNotify(bool value);

        /// <summary>设置选择时不通知内部事件</summary>
        protected internal abstract void SetIsSelectWithoutNotifyInternal(bool value);

        /// <summary>选择改变时的内部事件</summary>
        protected internal Action<SelectItemBase, bool> onSelectChangedInternal;
    }
}
