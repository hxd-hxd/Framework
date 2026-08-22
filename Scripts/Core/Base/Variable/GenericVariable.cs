using System;

namespace Framework.Core
{
    /// <summary>
    /// 变量。
    /// </summary>
    /// <typeparam name="T">变量类型。</typeparam>
    public abstract class Variable<T> : Variable
    {
        private T m_Value;

        /// <summary>
        /// 初始化变量的新实例。
        /// </summary>
        public Variable()
        {
            m_Value = default(T);
        }

        public override Type Type
        {
            get
            {
                return typeof(T);
            }
        }

        /// <summary>
        /// 获取或设置变量值。
        /// </summary>
        public T Value
        {
            get
            {
                return m_Value;
            }
            set
            {
                m_Value = value;
            }
        }

        public override V GetValue<V>()
        {
            return (V)(object)m_Value;
        }

        public override bool TryGetValue<V>(out V result)
        {
            if (m_Value is V v)
            {
                result = v;
                return true;
            }
            result = default(V);
            return false;
        }

        public override void SetValue<V>(V value)
        {
            m_Value = (T)(object)value;
        }

        public override void Clear()
        {
            m_Value = default(T);
        }

        public override string ToString()
        {
            return (m_Value != null) ? m_Value.ToString() : "<Null>";
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Variable<T> pv, T value)
        {
            bool result = Equals(pv.Value, value);
            return result;
        }

        public static bool operator !=(Variable<T> pv, T value)
        {
            bool result = !(pv == value);
            return result;
        }

        public static implicit operator T(Variable<T> pv)
        {
            var result = pv.Value;
            return result;
        }

    }
}
