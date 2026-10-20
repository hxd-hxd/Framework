// -------------------------
// 创建日期：2024/3/25 17:04:00
// -------------------------

using System;
using Framework.UI;

namespace Framework
{
    /// <summary>UI 接口</summary>
    public interface IUI : IUIGroupItem
    {
        /// <summary>名称</summary>
        string name { get; set; }

        /// <summary>在启用时置顶</summary>
        bool topmostInEnabled { get; set; }

        /// <summary>初始化</summary>
        void Init();

        /// <summary>启用</summary>
        void Enable(bool isEnable);

        /// <summary>在启用时调用</summary>
        void OnEnabled(bool isEnable);

        /// <summary>在刷新的时候触发</summary>
        void OnRefresh();

        /// <summary>在全局置顶时触发</summary>
        void OnTopmost();

    }
}