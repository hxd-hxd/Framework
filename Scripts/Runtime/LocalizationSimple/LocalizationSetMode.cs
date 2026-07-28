namespace Framework.LocalizationSimple
{
    /// <summary>本地化设置方式</summary>
    public enum LocalizationSetMode
    {
        /// <summary>直接使用类型设置</summary>
        Type = 0,

        /// <summary>使用提供者间接提供设置</summary>
        Provider,
    }
}