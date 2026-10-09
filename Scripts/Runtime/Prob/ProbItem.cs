
using System.ComponentModel;
using UnityEngine;

namespace Framework.Prob
{
    using Container = IContainer<string, ProbBranch, ProbItem>;

    /// <summary>
    /// 概率类
    /// </summary>
    [System.Serializable]
    public class ProbItem : BaseProbItem<string, ProbBranch, ProbItem>
    {
        [SerializeField]
        private string _value;
        [SerializeField]
        private float _probValue = -1;
        [SerializeField]
        private bool _enable = true;

        private Container _ownerContainer;

        public override string value
        {
            get => _value;
            set
            {
                if (owner != null && !Equals(_value, value)) owner.isDirty = true;
                _value = value;
            }
        }
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

        public override Container owner { get => _ownerContainer; set => _ownerContainer = value; }

        public ProbItem()
        {
            _probValue = -1;
            _enable = true;
            _value = default;
        }

        public ProbItem(float probValue)
        {
            _enable = true;
            _value = default;
            this._probValue = probValue;
        }

        public ProbItem(float probValue, string t)
        {
            _enable = true;
            this._probValue = probValue;
            this._value = t;
        }
    }

    /// <summary>
    /// 概率类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public class ProbItem<T> : BaseProbItem<T, ProbBranch<T>, ProbItem<T>>
    {
        [SerializeField]
        private T _value;
        [SerializeField]
        private float _probValue = -1;
        [SerializeField]
        private bool _enable = true;

        private IContainer<T, ProbBranch<T>, ProbItem<T>> _owner;

        public override T value
        {
            get => _value;
            set
            {
                if (owner != null && !Equals(_value, value)) owner.isDirty = true;
                _value = value;
            }
        }
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

        public override IContainer<T, ProbBranch<T>, ProbItem<T>> owner { get => _owner; set => _owner = value; }

        public ProbItem()
        {
            _probValue = -1;
            _enable = true;
            _value = default;
        }

        public ProbItem(float probValue)
        {
            _enable = true;
            _value = default;
            this._probValue = probValue;
        }

        public ProbItem(float probValue, T t)
        {
            _enable = true;
            this._probValue = probValue;
            this._value = t;
        }

    }

}