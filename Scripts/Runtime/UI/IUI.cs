// -------------------------
// 创建日期：2024/3/25 17:04:00
// -------------------------

using System;

namespace Framework
{
    /// <summary>UI 接口</summary>
    public interface IUI
    {
        /// <summary>名称</summary>
        string name { get; set; }

        /// <summary>是否启用</summary>
        bool isEnable { get; set; }

        /// <summary>排序</summary>
        int sortOrder { get; set; }

        /// <summary>初始化</summary>
        void Init();

        /// <summary>启用</summary>
        void Enable(bool isEnable);

        /// <summary>在刷新的时候触发</summary>
        void OnRefresh();

        /// <summary>在置顶时触发</summary>
        void OnTopmost();

        /// <summary>在被覆盖时触发，例如当前在最上面，其他 ui 变成了最上面</summary>
        void OnCovered();

        /// <summary>在没有被覆盖时触发，例如当前在最上面的其他 ui 变成了没有了，自己变成了最上面</summary>
        void OnNotCovered();

    }
}