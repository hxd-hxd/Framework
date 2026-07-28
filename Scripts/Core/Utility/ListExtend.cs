using System;
using System.Collections;
using System.Collections.Generic;

namespace Framework.Core
{
    /// <summary>
    /// <see cref="IList"/>、<see cref="List{T}"/> 扩展
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

        /// <summary>调整列表大小到指定大小，多余的则移除，不够则创建</summary>
        public static void AdjustNew<T>(this List<T> values, int count) where T : new()
        {
            if (count < 0) return;

            while (values.Count != count)
            {
                if (values.Count > count)
                {
                    values.RemoveAt(values.Count - 1);
                }
                else if (values.Count < count)
                {
                    values.Add(new());
                }
            }
        }

        /// <summary>调整列表大小到指定大小，多余的则移除，不够则使用默认值</summary>
        public static void AdjustDefault<T>(this List<T> values, int count) where T : new()
        {
            if (count < 0) return;

            while (values.Count != count)
            {
                if (values.Count > count)
                {
                    values.RemoveAt(values.Count - 1);
                }
                else if (values.Count < count)
                {
                    values.Add(default);
                }
            }
        }

        /// <summary>调整列表大小到指定大小，多余的则移除，不够则创建</summary>
        public static void Adjust<T>(this List<T> values, int count, Action<T, int> callback) where T : new()
        {
            if (count < 0) return;

            while (values.Count != count)
            {
                if (values.Count > count)
                {
                    var obj = values[values.Count - 1];
                    values.RemoveAt(values.Count - 1);
                    callback?.Invoke(obj, -1);
                }
                else if (values.Count < count)
                {
                    T obj = new();
                    values.Add(obj);
                    callback?.Invoke(obj, 1);
                }
            }
        }

        /// <summary>调整列表大小到指定大小，多余的则移除，不够则创建</summary>
        public static void Adjust<T>(this List<T> values, int count, Func<T> newCallback)
        {
            if (count < 0) return;

            while (values.Count != count)
            {
                if (values.Count > count)
                {
                    values.RemoveAt(values.Count - 1);
                }
                else if (values.Count < count)
                {
                    T obj = newCallback.Invoke() ?? default;
                    values.Add(obj);
                    
                }
            }
        }
    }
}
