using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Prob
{
    /// <summary>
    /// 概率生成器
    /// <para>可以这么理解原理：<see cref="ProbDevice"/> 生成器类似磁盘，<see cref="ProbBranch"/> 分支类似文件夹，<see cref="ProbItem"/> 类似各种文件</para>
    /// <para>就是随机在文件夹里找一个文件，根据你设定的 probValue 大小，其被随机到的概率也不同</para>
    /// <para><see cref="IProb.probValue"/> 概率值可以是任何大于零的数，但是推荐自己提前计算好值，然后填入，以免出现概率问题而难以追踪</para>
    /// </summary>
    [System.Serializable]
    public class ProbDevice : BaseProbDevice<string, ProbBranch, ProbItem>
    {
        static ProbDevice<bool> pdd = new ProbDevice<bool>();

        [SerializeField]
        private string _containerName = "Node";

        [SerializeField]
        private List<ProbItem> _items = new List<ProbItem>();
        [SerializeField]
        private List<ProbBranch> _branchs = new List<ProbBranch>();

        public override List<ProbItem> items { get => _items; set => _items = value; }
        public override List<ProbBranch> branchs { get => _branchs; set => _branchs = value; }
        public override string containerName { get => _containerName; set => _containerName = value; }

        public override IRandomProvider randomProvider { get => RandomProvider.instance; set { } }

        static ProbDevice()
        {
            pdd.AddItem(1, true);
            pdd.AddItem(1, false);
        }

        /// <summary>
        /// 随机 <see cref="bool"/>，概率值可以填任意数，他们会按比例进行计算
        /// <para><paramref name="truePV"/>：true 的概率值</para>
        /// <para><paramref name="falsePV"/>：false 的概率值</para>
        /// </summary>
        public static bool RandomBool(float truePV, float falsePV)
        {
            pdd.FindItem(true).probValue = truePV;
            pdd.FindItem(false).probValue = falsePV;
            return pdd.GetRandomValue();
        }
    }

    /// <summary>
    /// 概率生成器
    /// <para>可以这么理解原理：<see cref="ProbDevice"/> 生成器类似磁盘，<see cref="ProbBranch"/> 分支类似文件夹，<see cref="ProbItem"/> 类似各种文件</para>
    /// <para>就是随机在文件夹里找一个文件，根据你设定的 probValue 大小，其被随机到的概率也不同</para>
    /// <para><see cref="IProb.probValue"/> 概率值可以是任何大于零的数，但是推荐自己提前计算好值，然后填入，以免出现概率问题而难以追踪</para>
    /// </summary>
    [System.Serializable]
    public class ProbDevice<T> : BaseProbDevice<T, ProbBranch<T>, ProbItem<T>>
    {
        [SerializeField]
        private string _containerName = "Node";

        [Header("ReadOnly")]
        [SerializeField]
        private List<ProbItem<T>> _items = new List<ProbItem<T>>();
        [SerializeField]
        private List<ProbBranch<T>> _branchs = new List<ProbBranch<T>>();

        public override List<ProbItem<T>> items { get => _items; set => _items = value; }
        public override List<ProbBranch<T>> branchs { get => _branchs; set => _branchs = value; }
        public override string containerName { get => _containerName; set => _containerName = value; }

        public override IRandomProvider randomProvider { get => RandomProvider.instance; set { } }
    }
}
