using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 游戏框架组件抽象类。
    /// </summary>
    public abstract class FrameworkComponent : MonoBehaviour
    {

        /// <summary>
        /// 获取游戏框架模块优先级。
        /// </summary>
        /// <remarks>优先级较高的模块会优先轮询，并且关闭操作会后进行。</remarks>
        internal virtual int Priority
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// 游戏框架组件初始化。
        /// </summary>
        protected virtual void Awake()
        {
            ComponentManager.RegisterComponent(this);
        }

        /// <summary>
        /// 所有游戏框架模块轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        internal virtual void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {

        }
    }
}
