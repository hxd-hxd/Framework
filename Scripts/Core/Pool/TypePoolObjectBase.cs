// -------------------------
// 创建日期：2023/10/19 1:41:25
// -------------------------

namespace Framework
{
    /// <summary>
    /// 类型池 <see cref="TypePool"/> 对象基类
    /// </summary>
    public abstract class TypePoolObjectBase : ITypePoolObject
    {
        public abstract void Clear();

        /// <summary>
        /// 返回到对象池
        /// </summary>
        public virtual void Return()
        {
            TypePool.root.Return(this);
        }
    }

}