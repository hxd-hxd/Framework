using System.Collections;
using System.Collections.Generic;

namespace Framework.UI
{
    /// <summary>UI 分组</summary>
    public interface IUIGroup : IEnumerable<IUIGroupItem>
    {
        /// <summary>分组名</summary>
        string groupName { get; set; }

        /// <summary>是否启用</summary>
        bool isEnable { get; set; }

        /// <summary>排序</summary>
        int sortOrder { get; set; }

        /// <summary>包含项的数量</summary>
        int itemCount { get; }

        /// <summary>添加</summary>
        void AddItem(IUIGroupItem item);

        /// <summary>移除</summary>
        void RemoveItem(IUIGroupItem item);

        /// <summary>按名获取项</summary>
        IUIGroupItem GetItem(string name);

        /// <summary>获取顶部项</summary>
        IUIGroupItem GetTopItem();

        /// <summary>存在启用的项</summary>
        bool HasEnableItem();
    }
}
