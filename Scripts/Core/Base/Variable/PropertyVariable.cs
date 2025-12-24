using System;

namespace Framework.Core
{
    /// <summary>
    /// 属性变量。
    /// </summary>
    public abstract class ProperttyVariable : Variable
    {
        /// <summary>
        /// 初始化变量的新实例。
        /// </summary>
        public ProperttyVariable()
        {
        }

        /// <summary>
        /// 属性改变事件。
        /// <para><typeparamref name="T"/> 参数 1：旧值</para>
        /// <para><typeparamref name="T"/> 参数 2：新值</para>
        /// </summary>
        public abstract event Action<object, object> changeCallback;

    }
}
