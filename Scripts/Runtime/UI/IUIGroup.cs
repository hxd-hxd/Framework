using System.Collections;
using System.Collections.Generic;

namespace Framework.UI
{
    /// <summary>UI 分组</summary>
    public interface IUIGroup : IUIGroupItem, IEnumerable<IUIGroupItem>
    {
        /// <summary>分组名</summary>
        string groupName { get; set; }

        /// <summary>包含项的数量</summary>
        int itemCount { get; }

        /// <summary>是否根分组</summary>
        bool isRootGroup { get; }

        /// <summary>添加</summary>
        void AddItem(IUIGroupItem item);

        /// <summary>移除</summary>
        void RemoveItem(IUIGroupItem item);

        /// <summary>包含</summary>
        bool ContainsItem(IUIGroupItem item);

        /// <summary>创建子分组</summary>
        IUIGroup CreateSubGroup(string name);

        /// <summary>按名获取项</summary>
        IUIGroupItem GetItem(string name);

        /// <summary>获取顶部项</summary>
        IUIGroupItem GetTopItem();

        /// <summary>获取排序最高的项</summary>
        IUIGroupItem GetMaxSortOrderItem();

        /// <summary>存在启用的项</summary>
        bool HasEnableItem();

    }
}
