using UnityEngine;

namespace Framework.Prob
{
    /// <summary>随机提供者</summary>
    public class RandomProvider : IRandomProvider
    {
        private static RandomProvider _instance = new RandomProvider();

        /// <summary>默认实例</summary>
        public static RandomProvider instance => _instance;

        [SerializeField]
        private int _seed;

        /// <summary>随机数种子，在第一次设置之前不会使用，设置相同的种子会初始化状态</summary>
        public int seed
        {
            get => _seed;
            set
            {
                _seed = seed;
                Random.InitState(seed);
            }
        }

        public float RandomProbValue()
        {
            //System.Random
            return Random.Range(0, 100 + float.Epsilon);
        }
    }
}
