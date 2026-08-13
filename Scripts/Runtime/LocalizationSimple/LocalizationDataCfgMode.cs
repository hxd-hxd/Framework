namespace Framework.LocalizationSimple
{
    /// <summary>本地化数据配置方式</summary>
    public enum LocalizationDataCfgMode : byte
    {
        /// <summary>不可以配置 id 和 语言</summary>
        None = 0,

        /// <summary>只配置 id</summary>
        OnlyId,

        /// <summary>只配置 语言</summary>
        OnlyLang,

        /// <summary>可配置所有</summary>
        All,
    }
}