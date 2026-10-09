
namespace Framework.Prob
{
    /// <summary>概率类接口</summary>
    public interface IProb
    {
        /// <summary>启用</summary>
        bool enable { get; set; }

        /// <summary>概率值</summary>
        float probValue { get; set; }

        /// <summary>
        /// 真实概率（返回百分比值，例：概率是 20%，即返回 20）
        /// <para>ps：计算公式：真实概率 = 上级真实概率 * 本级百分比转换概率</para>
        /// </summary>
        /// <returns>百分比值</returns>
        float RealProb();

        /// <summary>获取完整节点路径</summary>
        string GetPath();

        /// <summary>是否有效</summary>
        bool IsValid();
    }
}