using System;

namespace Framework.Core
{
    /// <summary>
    /// 属性变量。
    /// </summary>
    [Serializable]
    public class ProperttyVariable<T> : ProperttyVariable
    {
        private T m_Value;
        private Action<object, object> m_ChangeCallback;

        /// <summary>
        /// 初始化变量的新实例。
        /// </summary>
        public ProperttyVariable()
        {
            m_Value = default(T);
        }

        /// <summary>
        /// 初始化变量的新实例。
        /// <para><paramref name="changeCallback"/>：值改变时的回调</para>
        /// </summary>
        public ProperttyVariable(Action<object, object> changeCallback)
        {
            m_Value = default(T);

            m_ChangeCallback = changeCallback;
        }

        public override event Action<object, object> changeCallback
        {
            add
            {
                m_ChangeCallback += value;
            }
            remove
            {
                m_ChangeCallback -= value;
            }
        }

        /// <summary>
        /// 获取或设置变量值。
        /// </summary>
        public virtual T Value
        {
            get
            {
                return m_Value;
            }
            set
            {
                InteralSetValue(value);
            }
        }

        /// <summary>
        /// 获取变量类型。
        /// </summary>
        public override Type Type
        {
            get
            {
                return typeof(T);
            }
        }

        /// <summary>
        /// 获取变量值。
        /// </summary>
        /// <returns>变量值。</returns>
        public override object GetValue()
        {
            return m_Value;
        }

        /// <summary>
        /// 设置变量值。
        /// </summary>
        /// <param name="value">变量值。</param>
        public override void SetValue(object value)
        {
            InteralSetValue((T)value);
        }

        /// <summary>
        /// 设置变量值。
        /// </summary>
        /// <param name="value">变量值。</param>
        public virtual void SetValue(T value)
        {
            InteralSetValue(value);
        }

        /// <summary>
        /// 设置变量值，但不触发 <see cref="changeCallback"/> 回调事件。
        /// </summary>
        /// <param name="value">变量值。</param>
        public virtual void SetValueNotCallback(T value)
        {
            m_Value = value;
        }

        /// <summary>
        /// 清理变量值。
        /// </summary>
        public override void Clear()
        {
            m_Value = default(T);
            m_ChangeCallback = null;
        }

        /// <summary>
        /// 获取变量字符串。
        /// </summary>
        /// <returns>变量字符串。</returns>
        public override string ToString()
        {
            return (m_Value != null) ? m_Value.ToString() : null;
        }

        private void InteralSetValue(T value)
        {
            var oldValue = m_Value;
            m_Value = value;

            m_ChangeCallback?.Invoke(oldValue, value);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(ProperttyVariable<T> pv, T value)
        {
            bool result = Equals(pv.Value, value);
            return result;
        }

        public static bool operator !=(ProperttyVariable<T> pv, T value)
        {
            bool result = !(pv == value);
            return result;
        }

        public static implicit operator T(ProperttyVariable<T> pv)
        {
            var result = pv.Value;
            return result;
        }

        //public static ProperttyVariable<T> operator =(ProperttyVariable<T> pv, T value)
        //{
        //    pv.Value = value;
        //    return pv;
        //}

    }
}
