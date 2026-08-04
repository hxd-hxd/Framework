using System;

namespace Framework
{
    /// <summary>选择项接口</summary>
    public interface ISelectItem
    {
        /// <summary>是否选择</summary>
        bool isSelect { get; set; }

        /// <summary>可以选择</summary>
        bool canSelect { get; set; }

        /// <summary>选择变化事件</summary>
        Action<bool> onSelectChanged { get; set; }

        /// <summary>设置选中状态，不触发事件
        /// <para></para><paramref name="value"/>: 是否选中
        /// </summary>
        void SetIsSelectWithoutNotify(bool value);
    }
}
