using System.Collections;
using System.Collections.Generic;

namespace Framework.Prob
{
    /// <summary>概率类实用程序</summary>
    public static class ProbUtility
    {
        /// <summary>
        /// 根据概率随机一个
        /// <para><paramref name="ignoreEnable"/>：忽视启用的影响</para>
        /// </summary>
        public static T RandomProb<T>(this List<T> probs, IRandomProvider randomProvider, bool ignoreEnable) where T : IProb
        {
            if (probs == null) return default;

            List<T> ps = probs.GetValidProb(ignoreEnable);

            var r = InternalRandomProb(ps, randomProvider);

            TypePool.root.Return(ps);
            return r;
        }

        /// <summary>获取列表中可用的部分</summary>
        public static List<T> GetValidProb<T>(this List<T> probs, bool ignoreEnable) where T : IProb
        {
            List<T> ps = TypePool.root.GetList<T>();
            foreach (var p in probs)
            {
                if (p.IsValid(ignoreEnable))
                    ps.Add(p);
            }
            return ps;
        }

        /// <summary>
        /// 获取此列表的总概率值
        /// <para><paramref name="ignoreEnable"/>：忽视启用的影响</para>
        /// <para><paramref name="ignoreMinus"/>：忽视负值的影响（如非特殊需要，请保持默认）</para>
        /// </summary>
        public static float GetSumProbValue<T>(this List<T> probs, bool ignoreEnable = false, bool ignoreMinus = false) where T : IProb
        {
            float pv = 0;
            for (int i = 0; i < probs.Count; i++)
            {
                if (!ignoreEnable)
                {
                    if (!probs[i].enable) continue;
                }
                if (!ignoreMinus && probs[i].probValue <= 0) continue;

                pv += probs[i].probValue;
            }

            return pv;
        }

        /// <summary>完整排序</summary>
        public static void SortProbs<T>(this List<T> probs) where T : IProb
        {
            probs.Sort((v1, v2) => v1.probValue.CompareTo(v2.probValue));
        }

        /// <summary>
        /// 转换成百分比的形式
        /// <para><paramref name="cardinality"/>：目标基数</para>
        /// <para>ps：不应为负数</para>
        /// </summary>
        public static float FormatProb100(this float self, float cardinality)
        {
            return self / cardinality * 100;
        }

        /// <summary>
        /// 转换成归一化的形式（计算比率）
        /// <para><paramref name="cardinality"/>：目标基数</para>
        /// <para>ps：不应为负数</para>
        /// </summary>
        public static float FormatProb1(this float self, float cardinality)
        {
            return self / cardinality;
        }

        /// <summary>根据概率随机一个</summary>
        internal static T InternalRandomProb<T>(this List<T> ps, IRandomProvider randomProvider) where T : IProb
        {
            if (ps.Count <= 0) return default;
            if (ps.Count == 1) return ps[0];

            float randomPV = randomProvider.RandomProbValue();// 随机一个概率值

            // 用于随机的核心算法

            // 计算概率
            float pvSumUp = 0;// 总概率
            for (int i = 0; i < ps.Count; i++)
            {
                pvSumUp += ps[i].probValue;
            }

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

        /*
        TODO：添加批量随机方法，用已优化大批量随机的性能。
        批量随机比较麻烦，因为是树形结构，每一个分支都有自己的概率列表，在批量随机过程中，需缓存概率列表的计算结果，只在有动态变动时重新计算（例如有的道具有获取次数限制，达标后禁用或移除），其他时候复用，已达到优化目的。
        */

    }
}