// -------------------------
// 创建日期：2024/11/12 14:40:46
// -------------------------

using System;
using System.Numerics;

namespace Framework
{
    /// <summary>
    /// 字节大小相关的实用函数集
    /// </summary>
    public static class BytesUtility
    {
        // 字节单位
        public const float B = 1;
        public const float KB = 1024;
        public const float MB = 1048576;
        public const float GB = 1073741824;
        public const float TB = 1099511627776;
        public const float PB = 1125899906842624;

        /// <summary>
        /// 选择合适的转换单位，并以字符串形式表示
        /// </summary>
        /// <param name="byteSize"></param>
        /// <param name="decimals">要保留的小数位</param>
        /// <returns></returns>
        public static string ToString(long byteSize, int decimals = 1)
        {
            if (decimals < 0) decimals = 0;
            string f = $"f{decimals}";
            
            string r = "0";
            if (byteSize > 0)
            {
                if (byteSize >= PB)
                {
                    r = $"{(byteSize / PB).ToString(f)} pb";
                }
                else
                if (byteSize >= TB)
                {
                    r = $"{(byteSize / TB).ToString(f)} tb";
                }
                else
                if (byteSize >= GB)
                {
                    r = $"{(byteSize / GB).ToString(f)} gb";
                }
                else
                if (byteSize >= MB)
                {
                    r = $"{(byteSize / MB).ToString(f)} mb";
                }
                else
                if (byteSize >= KB)
                {
                    r = $"{(byteSize / KB).ToString(f)} kb";
                }
                else
                {
                    r = $"{byteSize} byte";
                }
            }

            return r;
        }

        /// <summary>
        /// 将字节转换成合适大小的单位（1024 b = 1 kb）
        /// </summary>
        /// <param name="byteSize"></param>
        /// <param name="value"></param>
        /// <param name="unit"></param>
        public static void ToUnit(long byteSize, out float value, out ByteUnitType unit)
        {
            value = 0;
            unit = ByteUnitType.None;
            if (byteSize > 0)
            {
                if (byteSize >= PB)
                {
                    unit = ByteUnitType.PB;
                    value = byteSize / PB;
                }
                else
                if (byteSize >= TB)
                {
                    unit = ByteUnitType.TB;
                    value = byteSize / TB;
                }
                else
                if (byteSize >= GB)
                {
                    unit = ByteUnitType.GB;
                    value = byteSize / GB;
                }
                else
                if (byteSize >= MB)
                {
                    unit = ByteUnitType.MB;
                    value = byteSize / MB;
                }
                else
                if (byteSize >= KB)
                {
                    unit = ByteUnitType.KB;
                    value = byteSize / KB;
                }
                else
                {
                    unit = ByteUnitType.B;
                    value = byteSize / B;
                }
            }
        }
    }

    /// <summary>表示数据大小单位</summary>
    public enum ByteUnitType : long
    {
        None = 0,
        /// <summary>1 byte</summary>
        B = 1,
        /// <summary>1 kb</summary>
        KB = 1024,
        /// <summary>1 mb</summary>
        MB = 1048576,
        /// <summary>1 gb</summary>
        GB = 1073741824,
        /// <summary>1 tb</summary>
        TB = 1099511627776,
        /// <summary>1 pb</summary>
        PB = 1125899906842624
    }
}