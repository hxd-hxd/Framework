using System;
using System.Collections;
using System.Collections.Generic;

namespace Framework.Prob
{
    /// <summary>概率类实用程序</summary>
    public static class ProbUtility
    {
        /// <summary>
        /// 根据概率随机一个
        /// <para><paramref name=""/>：忽视启用的影响</para>
        /// </summary>
        public static T RandomProb<T>(this List<T> probs, IRandomProvider randomProvider) where T : IProb
        {
            if (probs == null) return default;

            List<T> ps = probs.GetValidProb();

            var r = InternalRandomProb(ps, randomProvider);

            TypePool.root.Return(ps);
            return r;
        }

        /// <summary>获取列表中可用的部分</summary>
        public static List<T> GetValidProb<T>(this List<T> probs) where T : IProb
        {
            List<T> ps = TypePool.root.GetList<T>();
            foreach (var p in probs)
            {
                if (p.IsValid())
                    ps.Add(p);
            }
            return ps;
        }

        /// <summary>获取此列表的总概率值</summary>
        public static float GetSumProbValue<T>(this List<T> probs) where T : IProb
        {
            float pv = 0;
            for (int i = 0; i < probs.Count; i++)
            {
                if (probs[i].probValue <= 0) continue;

                pv += probs[i].probValue;
            }

            return pv;
        }

        /// <summary>获取此列表的概率范围列表</summary>
        public static void GetProbRanges<T>(this List<T> probs, List<float> probRanges) where T : IProb
        {
            // 总概率
            float pvSumUp = probs.GetSumProbValue();

            // 计算范围
            probRanges.Clear();
            if (probRanges.Capacity < probs.Count) probRanges.Capacity = probs.Count;

            float pv = 0;
            for (int i = 0; i < probs.Count; i++)
            {
                // 计算概率范围
                pv += probs[i].probValue;
                float formatPV = pv / pvSumUp * 100;// 格式化成百分比
                probRanges.Add(formatPV);
            }
        }

        /// <summary>
        /// 转换成百分比的形式
        /// <para><paramref name="cardinality"/>：目标基数</para>
        /// <para>注意：不应为负数</para>
        /// </summary>
        public static float FormatProb100(this float self, float cardinality) => self / cardinality * 100;

        /// <summary>
        /// 转换成归一化的形式（计算比率）
        /// <para><paramref name="cardinality"/>：目标基数</para>
        /// <para>注意：不应为负数</para>
        /// </summary>
        public static float FormatProb1(this float self, float cardinality) => self / cardinality;

        /// <summary>根据概率随机一个</summary>
        internal static T InternalRandomProb<T>(this List<T> ps, IRandomProvider randomProvider) where T : IProb
        {
            if (ps.Count <= 0) return default;
            if (ps.Count == 1) return ps[0];

            // 用于随机的核心算法

            // 总概率
            float pvSumUp = 0;
            for (int i = 0; i < ps.Count; i++)
            {
                pvSumUp += ps[i].probValue;
            }

            // 计算概率
            float randomPV = randomProvider.RandomProbValue();// 随机一个概率值

            // 从低到高对比范围
            float pv = 0;
            for (int i = 0; i < ps.Count - 1; i++)
            {
                // 计算概率范围
                pv += ps[i].probValue;
                float formatPV = pv / pvSumUp * 100;// 格式化成百分比

                if (randomPV <= formatPV) return ps[i];
            }

            return ps[ps.Count - 1];
        }

        /// <summary>根据概率随机一个
        /// <para><paramref name="probRanges"/>：概率范围列表，长度应为 <paramref name="ps"/> 的长度，应当是根据基数 100 计算过的值</para>
        /// </summary>
        internal static T InternalRandomProb<T>(this List<T> ps, IRandomProvider randomProvider, List<float> probRanges) where T : IProb
        {
            if (ps.Count <= 0) return default;
            if (ps.Count == 1) return ps[0];
            if (probRanges == null || probRanges.Count != ps.Count)
                throw new System.Exception($"概率范围列表长度（{probRanges?.Count ?? 0}）应为概率列表长度（{ps.Count}）");

            // 用于随机的核心算法

            // 计算概率
            float randomPV = randomProvider.RandomProbValue();// 随机一个概率值

            // 使用二分查找优化查找效率
            // 经实际测试，直接使用二分查找反而增加了 1/3 的消耗

            if (probRanges.Count < 50 || randomPV < 50)
            {
                // 直接查找
                // 从低到高对比范围
                for (int i = 0; i < probRanges.Count - 1; i++)
                {
                    // 计算概率范围
                    float formatPV = probRanges[i];
                    if (randomPV <= formatPV) return ps[i];
                }
            }
            else
            {
                // 二分查找
                // probRanges 天然有序，可直接使用二分查找
                int index = probRanges.BinarySearch(randomPV);
                if (index >= 0) return ps[index];
            }

            return ps[ps.Count - 1];
        }

        /*
        批量随机方法，用于优化大批量随机的性能。
        批量随机比较麻烦，因为是树形结构，每一个分支都有自己的概率列表，在批量随机过程中，需缓存概率列表的计算结果，只在有动态变动时重新计算（例如有的道具有获取次数限制，达标后禁用或移除），其他时候复用，以达到优化目的。
        */

    }
}