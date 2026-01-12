using System;
using System.Collections;
using System.Collections.Generic;

namespace Framework.Core
{
    /// <summary>
    /// <see cref="IList"/> 扩展
    /// </summary>
    public static class ListExtend
    {
        /// <summary>
        /// 将 <paramref name="sourceIndex"/> 处元素设置到指定位置 <paramref name="targetIndex"/>，并保持其他元素排序不变
        /// <code>例如：<paramref name="self"/> = { 1, 2, 3 }，<paramref name="sourceIndex"/> = 0，<paramref name="targetIndex"/> = 2，
        /// 结果：<paramref name="self"/> = { 2, 3, 1 }</code>
        /// </summary>
        /// <returns>0：未设置位置，-1：前移，即 <paramref name="sourceIndex"/> 小于 <paramref name="targetIndex"/>，1：后移，与前移相反</returns>
        public static int SetPos(this IList self, int sourceIndex, int targetIndex)
        {
            if (sourceIndex == targetIndex) return 0;
            if (sourceIndex < 0 || targetIndex < 0) return 0;
            int lastIndex = self.Count - 1;
            if (sourceIndex > lastIndex || targetIndex > lastIndex) return 0;
            
            /*
              有两种情况
                1、源小于目标，需要将源到目标的元素前移
                2、源大于目标，需要将源到目标的元素后移
             */

            int result = 0;
            var sourceItem = self[sourceIndex];
            if (sourceIndex > targetIndex)
            {
                for (int i = sourceIndex; i < targetIndex; i++)
                {
                    self[i] = self[i + 1];
                }

                result = -1;
            }
            else
            {
                for (int i = targetIndex - 1; i >= sourceIndex; i--)
                {
                    self[i] = self[i - 1];
                }

                result = 1;
            }

            self[targetIndex] = sourceItem;

            return result;
        }
    }
}
