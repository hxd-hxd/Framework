using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Prob
{
    /// <summary>分支</summary>
    [System.Serializable]
    public class ProbBranch : BaseProbBranch<string, ProbBranch, ProbItem>
    {
        [SerializeField]
        private string _containerName = "Node";

        [SerializeField]
        protected float _probValue = -1;
        [SerializeField]
        protected bool _enable = true;
        [SerializeField]
        protected bool _allProb;

        [SerializeField]
        protected IContainer<string, ProbBranch, ProbItem> _owner;

        [SerializeField]
        private List<ProbItem> _items = new List<ProbItem>();
        [SerializeField]
        private List<ProbBranch> _branchs = new List<ProbBranch>();

        public override List<ProbItem> items { get => _items; set => _items = value; }
        public override List<ProbBranch> branchs { get => _branchs; set => _branchs = value; }
        public override string containerName { get => _containerName; set => _containerName = value; }

        public override IRandomProvider randomProvider { get => RandomProvider.instance; set { } }

        public override float probValue { get => _probValue; set => _probValue = value; }
        public override bool enable { get => _enable; set => _enable = value; }
        public override bool allProb { get => _allProb; set => _allProb = value; }
        public override IContainer<string, ProbBranch, ProbItem> owner { get => _owner; set => _owner = value; }

        public ProbBranch()
        {
            Init();
        }

        public ProbBranch(float probValue)
        {
            Init();
            this.probValue = probValue;
        }

        public ProbBranch(string containerName, float probValue)
        {
            Init();

            this.probValue = probValue;
            this.containerName = containerName;
        }
    }

    /// <summary>分支</summary>
    [System.Serializable]
    public class ProbBranch<T> : BaseProbBranch<T, ProbBranch<T>, ProbItem<T>>
    {
        [SerializeField]
        protected float _probValue = -1;
        [SerializeField]
        protected bool _enable = true;
        [SerializeField]
        protected bool _allProb;

        [SerializeField]
        protected IContainer<T, ProbBranch<T>, ProbItem<T>> _owner;

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

        public override float probValue { get => _probValue; set => _probValue = value; }
        public override bool enable { get => _enable; set => _enable = value; }
        public override bool allProb { get => _allProb; set => _allProb = value; }
        public override IContainer<T, ProbBranch<T>, ProbItem<T>> owner { get => _owner; set => _owner = value; }

        public ProbBranch()
        {
            Init();
        }

        public ProbBranch(float probValue)
        {
            Init();
            this.probValue = probValue;
        }

        public ProbBranch(string containerName, float probValue)
        {
            Init();
            this.probValue = probValue;
            this.containerName = containerName;
        }
    }

}