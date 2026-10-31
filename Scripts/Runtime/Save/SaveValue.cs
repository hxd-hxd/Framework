using System;

namespace Framework.Save
{
    /// <summary>存档值包装。
    /// <para><see cref="UnityEngine.JsonUtility"/> 可以序列化该泛型。</para>
    /// <para><typeparamref name="T"/> 为 <see cref="UnityEngine.Object"/> 时只记下 instanceID，不能跨启动还原。</para>
    /// </summary>
    [Serializable]
    public class SaveValue<T>
    {
        public T value;
    }
}
