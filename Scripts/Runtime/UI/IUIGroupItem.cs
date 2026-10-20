
namespace Framework.UI
{
    /// <summary>UI 组项</summary>
    public interface IUIGroupItem
    {
        /// <summary>项名</summary>
        string itemName { get; set; }

        /// <summary>是否启用</summary>
        bool isEnable { get; set; }

        /// <summary>本地排序</summary>
        int sortOrder { get; set; }

        /// <summary>全局排序</summary>
        int globalSortOrder { get; }

        /// <summary>所属分组</summary>
        IUIGroup group { get; set; }

        /// <summary>根分组</summary>
        IUIGroup rootGroup { get; }

        /// <summary>是否在检查置顶时忽略此项</summary>
        bool topmostIgnore { get; }

        ///// <summary>可置顶</summary>
        //bool canTopmost { get; }

        /// <summary>在被覆盖时触发，例如当前在最上面，其他 item 变成了最上面</summary>
        void OnCovered();

        /// <summary>在没有被覆盖时触发，例如当前在最上面的其他 item 变成了没有了，自己变成了最上面</summary>
        void OnNotCovered();

        // TOOD：考虑添加名称路径和排序路径接口
    }
}
