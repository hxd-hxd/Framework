// -------------------------
// 创建日期：2023/10/19 1:41:25
// -------------------------

using System;
using System.Collections.Generic;

namespace Framework.Event
{
    /// <summary>
    /// 事件管理器
    /// </summary>
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

        /// <summary>移除侦听</summary>
        void RemoveListener<TID>(TID id, Delegate listener);

        /// <summary>发送消息</summary>
        void Send<TID>(TID id, params object[] args);
    }
    
    /// <summary>
    /// 事件管理器
    /// </summary>
    public interface IEventManager<TID> : IEventManager
    {

        /// <summary>清除指定 id 的所有监听</summary>
        void Clear(TID id);

        /// <summary>添加侦听</summary>
        void AddListener(TID id, Delegate listener);

        /// <summary>移除侦听</summary>
        void RemoveListener(TID id, Delegate listener);

        /// <summary>发送消息</summary>
        void Send(TID id, params object[] args);
    }


}