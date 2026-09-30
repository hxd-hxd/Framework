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
        private float _probValue = -1;
        [SerializeField]
        private bool _enable = true;
        [SerializeField]
        private bool _allProb;
        [SerializeField]
        private bool _isDirty = true;

        [SerializeField]
        private IContainer<string, ProbBranch, ProbItem> _owner;

        [SerializeField]
        private List<ProbItem> _items = new List<ProbItem>();
        [SerializeField]
        private List<ProbBranch> _branchs = new List<ProbBranch>();

        public override List<ProbItem> items
        {
            get => _items;
            set
            {
                if (!Equals(items, value)) isDirty = true;
                items = value;
            }
        }
        public override List<ProbBranch> branchs
        {
            get => _branchs;
            set
            {
                if (!Equals(branchs, value)) isDirty = true;
                branchs = value;
            }
        }
        public override string containerName { get => _containerName; set => _containerName = value; }

        public override IRandomProvider randomProvider { get => RandomProvider.instance; set { } }

        public override float probValue
        {
            get => _probValue;
            set
            {
                if (owner != null && !Equals(_probValue, value)) owner.isDirty = true;
                _probValue = value;
            }
        }
        public override bool enable
        {
            get => _enable;
            set
            {
                if (owner != null && !Equals(_enable, value)) owner.isDirty = true;
                _enable = value;
            }
        }
        public override bool allProb
        {
            get => _allProb;
            set
            {
                if (owner != null && !Equals(_allProb, value)) owner.isDirty = true;
                _allProb = value;
            }
        }

        public override IContainer<string, ProbBranch, ProbItem> owner { get => _owner; set => _owner = value; }

        public override bool isDirty { get => _isDirty; set => _isDirty = value; }

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
        private float _probValue = -1;
        [SerializeField]
        private bool _enable = true;
        [SerializeField]
        private bool _allProb;
        [SerializeField]
        private bool _isDirty = true;

        [SerializeField]
        private IContainer<T, ProbBranch<T>, ProbItem<T>> _owner;

        [SerializeField]
        private string _containerName = "Node";

        [Header("ReadOnly")]
        [SerializeField]
        private List<ProbItem<T>> _items = new List<ProbItem<T>>();
        [SerializeField]
        private List<ProbBranch<T>> _branchs = new List<ProbBranch<T>>();

        public override List<ProbItem<T>> items
        {
            get => _items;
            set
            {
                if (!Equals(items, value)) isDirty = true;
                items = value;
            }
        }
        public override List<ProbBranch<T>> branchs
        {
            get => _branchs;
            set
            {
                if (!Equals(branchs, value)) isDirty = true;
                branchs = value;
            }
        }
        public override string containerName { get => _containerName; set => _containerName = value; }

        public override IRandomProvider randomProvider { get => RandomProvider.instance; set { } }

        public override float probValue
        {
            get => _probValue;
            set
            {
                if (owner != null && !Equals(_probValue, value)) owner.isDirty = true;
                _probValue = value;
            }
        }
        public override bool enable
        {
            get => _enable;
            set
            {
                if (owner != null && !Equals(_enable, value)) owner.isDirty = true;
                _enable = value;
            }
        }
        public override bool allProb
        {
            get => _allProb;
            set
            {
                if (owner != null && !Equals(_allProb, value)) owner.isDirty = true;
                _allProb = value;
            }
        }

        public override IContainer<T, ProbBranch<T>, ProbItem<T>> owner { get => _owner; set => _owner = value; }

        public override bool isDirty { get => _isDirty; set => _isDirty = value; }

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