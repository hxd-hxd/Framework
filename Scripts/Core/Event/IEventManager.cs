// -------------------------
// 创建日期：2023/10/19 1:41:25
// -------------------------

using System;

namespace Framework.Event
{
    /// <summary>事件管理器</summary>
    public interface IEventManager
    {
        /// <summary>清除所有监听</summary>
        void Clear();

        /// <summary>清空消息库</summary>
        void ClearAll();

        /// <summary>清除指定 id 所有监听</summary>
        void Clear<TID>(TID id);

        /// <summary>清除指定 id 类型所有监听</summary>
        void Clear<TID>();

        /// <summary>添加侦听</summary>
        void AddListener<TID>(TID id, Delegate listener);

        /// <summary>添加侦听
        /// <para><paramref name="priority"/>：监听执行优先级，数值越大越先执行，同优先级遵循先来后到原则</para>
        /// </summary>
        void AddListener<TID>(TID id, Delegate listener, int priority);

        /// <summary>移除侦听</summary>
        void RemoveListener<TID>(TID id, Delegate listener);

        /// <summary>发送消息</summary>
        void Send<TID>(TID id, params object[] args);

        /// <summary>发送消息</summary>
        void Send<TID>(TID id);

        /// <summary>发送消息</summary>
        void Send<TID, T>(TID id, T msg);

        /// <summary>检查指定 id 是否在监听</summary>
        bool IsListening<TID>(TID id);

        /// <summary>检查指定事件是否在监听</summary>
        bool IsListening<TID>(TID id, Delegate listener);
    }

    /// <summary>事件管理器</summary>
    public interface IEventManager<TID> : IEventManager
    {
        /// <summary>清除指定 id 的所有监听</summary>
        void Clear(TID id);

        /// <summary>添加侦听</summary>
        void AddListener(TID id, Delegate listener);

        /// <summary>添加侦听
        /// <para><paramref name="priority"/>：监听执行优先级，数值越大越先执行，同优先级遵循先来后到原则</para>
        /// </summary>
        void AddListener(TID id, Delegate listener, int priority);

        /// <summary>移除侦听</summary>
        void RemoveListener(TID id, Delegate listener);

        /// <summary>发送消息</summary>
        void Send(TID id, params object[] args);

        /// <summary>发送消息</summary>
        void Send(TID id);

        /// <summary>发送消息</summary>
        void Send<T>(TID id, T msg);

        /// <summary>检查指定 id 是否有监听</summary>
        bool IsListening(TID id);

        /// <summary>检查指定事件是否在监听</summary>
        bool IsListening(TID id, Delegate listener);
    }
}