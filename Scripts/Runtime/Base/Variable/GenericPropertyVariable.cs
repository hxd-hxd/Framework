using System;
using UnityEngine;

namespace Framework.Runtime
{
    /// <summary>
    /// 属性变量。
    /// </summary>
    [Serializable]
    public class PropertyVariable<T> : Core.ProperttyVariable<T>
    {
        [SerializeField]
        private T _value;
        private Action<T, T> m_ChangeCallback;

        /// <summary>
        /// 初始化变量的新实例。
        /// </summary>
        public PropertyVariable()
        {
            _value = default(T);
        }

        /// <summary>
        /// 初始化变量的新实例。
        /// <para><paramref name="changeCallback"/>：值改变时的回调</para>
        /// </summary>
        public PropertyVariable(Action<T, T> changeCallback)
        {
            _value = default(T);

            m_ChangeCallback = changeCallback;
        }

        /// <summary>
        /// 获取或设置变量值。
        /// </summary>
        public override T Value
        {
            get
            {
                return _value;
            }
            set
            {
                InteralSetValue(value);
            }
        }

        new public event Action<T, T> changeCallback
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
        /// 获取变量值。
        /// </summary>
        /// <returns>变量值。</returns>
        public override object GetValue()
        {
            return _value;
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
        public override void SetValue(T value)
        {
            InteralSetValue(value);
        }

        /// <summary>
        /// 设置变量值，但不触发 <see cref="changeCallback"/> 回调事件。
        /// </summary>
        /// <param name="value">变量值。</param>
        public override void SetValueNotCallback(T value)
        {
            _value = value;
        }

        /// <summary>
        /// 清理变量值。
        /// </summary>
        public override void Clear()
        {
            _value = default(T);
            m_ChangeCallback = null;
        }

        /// <summary>
        /// 获取变量字符串。
        /// </summary>
        /// <returns>变量字符串。</returns>
        public override string ToString()
        {
            return (_value != null) ? _value.ToString() : null;
        }

        private void InteralSetValue(T value)
        {
            var oldValue = _value;
            _value = value;

            m_ChangeCallback?.Invoke(oldValue, value);
        }

    }
}
