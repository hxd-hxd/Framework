
namespace Framework.Core
{
    /// <summary>
    /// 数学相关实用程序
    /// </summary>
    public class MathUtility
    {
        /// <summary>
        /// 判断 <paramref name="a"/> 与 <paramref name="b"/> 是否近似
        /// <para><paramref name="precision"/>：精度</para>
        /// <code>例如：a = 1，b = 2，precision = 0.1，则当 a 大于等于 1.95 并且 a 小于等于 2.05 时，返回 true</code>
        /// </summary>
        public static bool Approximately(float a, float b, float precision)
        {
            float precisionHalf = precision / 2;
            bool result = a >= b - precisionHalf && a <= b + precisionHalf;
            return result;
        }
    }
}
