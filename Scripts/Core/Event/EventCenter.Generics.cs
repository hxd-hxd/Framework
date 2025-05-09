﻿// -------------------------
// 创建日期：2023/10/19 1:41:25
// -------------------------

using System;
using System.Collections.Generic;

namespace Framework.Event
{
    /// <summary>
    /// 事件处理调度中心
    /// <para>事件消息支持所有类型的参数，并且参数支持面向对象特性，
    /// 推荐的用法是消息继承 <see cref="IEventMessage"/></para>
    /// <code>
    /// public class Msg1 : IEventMessage { }
    /// public class Msg2 : IEventMessage { }
    /// </code>
    /// <para>注意：当以类型作为 id 时，不要用系统类型作为 id，应该自定义专用的消息类型作为 id，为了通用性，将不对作为 id 的类型进行限制</para>
    /// </summary>
    public static partial class EventCenter<TID>
    {
        internal static EventManager<TID> _eventManager = new EventManager<TID>();

        static EventCenter()
        {
            EventCenter.Add(_eventManager);
        }


        #region 添加侦听
        /// <summary>添加侦听</summary>
        public static void AddListener(TID id, Action listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public static void AddListener<T1>(TID id, Action<T1> listener)
        {
            AddListener(id, listener as Delegate);
        }

        #region 添加侦听，多参数
        /// <summary>添加侦听</summary>
        public static void AddListener<T1, T2>(TID id, Action<T1, T2> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public static void AddListener<T1, T2, T3>(TID id, Action<T1, T2, T3> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public static void AddListener<T1, T2, T3, T4>(TID id,
            Action<T1, T2, T3, T4> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public static void AddListener<T1, T2, T3, T4, T5>(TID id,
            Action<T1, T2, T3, T4, T5> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public static void AddListener<T1, T2, T3, T4, T5, T6>(TID id,
            Action<T1, T2, T3, T4, T5, T6> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public static void AddListener<T1, T2, T3, T4, T5, T6, T7>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public static void AddListener<T1, T2, T3, T4, T5, T6, T7, T8>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public static void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public static void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public static void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public static void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public static void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public static void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public static void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public static void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> listener)
        {
            AddListener(id, listener as Delegate);
        }

        #endregion

        /// <summary>添加侦听</summary>
        public static void AddListener(TID id, Delegate listener)
        {
            if (listener == null) return;

            _eventManager.AddListener(id, listener);
        }
        #endregion


        #region 移除侦听
        /// <summary>移除侦听</summary>
        public static void RemoveListener(TID id, Action listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public static void RemoveListener<T1>(TID id, Action<T1> listener)
        {
            RemoveListener(id, listener as Delegate);
        }

        #region 移除侦听，多参数
        /// <summary>移除侦听</summary>
        public static void RemoveListener<T1, T2>(TID id, Action<T1, T2> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public static void RemoveListener<T1, T2, T3>(TID id, Action<T1, T2, T3> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public static void RemoveListener<T1, T2, T3, T4>(TID id,
            Action<T1, T2, T3, T4> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public static void RemoveListener<T1, T2, T3, T4, T5>(TID id,
            Action<T1, T2, T3, T4, T5> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public static void RemoveListener<T1, T2, T3, T4, T5, T6>(TID id,
            Action<T1, T2, T3, T4, T5, T6> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public static void RemoveListener<T1, T2, T3, T4, T5, T6, T7>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public static void RemoveListener<T1, T2, T3, T4, T5, T6, T7, T8>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public static void RemoveListener<T1, T2, T3, T4, T5, T6, T7, T8, T9>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public static void RemoveListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public static void RemoveListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public static void RemoveListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public static void RemoveListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public static void RemoveListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public static void RemoveListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public static void RemoveListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> listener)
        {
            RemoveListener(id, listener as Delegate);
        }

        #endregion

        /// <summary>移除侦听</summary>
        public static void RemoveListener(TID id, Delegate listener)
        {
            if (listener == null) return;

            _eventManager.RemoveListener(id, listener);
        } 
        #endregion


        #region 清除监听
        /// <summary>清除指定 id 的所有监听</summary>
        public static void Clear(TID id)
        {
            _eventManager.Clear(id);
        }
        /// <summary>清除所有监听</summary>
        public static void Clear()
        {
            _eventManager.Clear();
        }
        /// <summary>清空消息库</summary>
        public static void ClearAll()
        {
            _eventManager.ClearAll();
        }
        #endregion


        #region 发送消息
        /// <summary>发送消息</summary>
        public static void Send(TID id)
        {
            SendInternal(id, null);
        }
        /// <summary>发送消息</summary>
        public static void Send<T1>(TID id, T1 msg1)
        {
            var args = TypePool.root.GetArrayE<object>(msg1);
            SendInternal(id, args);
        }

        #region 发送消息，多参数
        /// <summary>发送消息</summary>
        public static void Send<T1, T2>(TID id, T1 msg1, T2 msg2)
        {
            var args = TypePool.root.GetArrayE<object>(msg1, msg2);
            SendInternal(id, args);
        }
        /// <summary>发送消息</summary>
        public static void Send<T1, T2, T3>(TID id, T1 msg1, T2 msg2, T3 msg3)
        {
            var args = TypePool.root.GetArrayE<object>(msg1, msg2, msg3);
            SendInternal(id, args);
        }
        /// <summary>发送消息</summary>
        public static void Send<T1, T2, T3, T4>(TID id
            , T1 msg1, T2 msg2, T3 msg3, T4 msg4)
        {
            var args = TypePool.root.GetArrayE<object>(msg1, msg2, msg3);
            SendInternal(id, args);
        }
        /// <summary>发送消息</summary>
        public static void Send<T1, T2, T3, T4, T5>(TID id
            , T1 msg1, T2 msg2, T3 msg3, T4 msg4, T5 msg5)
        {
            var args = TypePool.root.GetArrayE<object>(msg1, msg2, msg3, msg4, msg5);
            SendInternal(id, args);
        }
        /// <summary>发送消息</summary>
        public static void Send<T1, T2, T3, T4, T5, T6>(TID id
            , T1 msg1, T2 msg2, T3 msg3, T4 msg4, T5 msg5, T6 msg6)
        {
            var args = TypePool.root.GetArrayE<object>(msg1, msg2, msg3, msg4, msg5, msg6);
            SendInternal(id, args);
        }
        /// <summary>发送消息</summary>
        public static void Send<T1, T2, T3, T4, T5, T6, T7>(TID id
            , T1 msg1, T2 msg2, T3 msg3, T4 msg4, T5 msg5, T6 msg6, T7 msg7)
        {
            var args = TypePool.root.GetArrayE<object>(msg1, msg2, msg3, msg4, msg5, msg6, msg7);
            SendInternal(id, args);
        }
        /// <summary>发送消息</summary>
        public static void Send<T1, T2, T3, T4, T5, T6, T7, T8>(TID id
            , T1 msg1, T2 msg2, T3 msg3, T4 msg4, T5 msg5, T6 msg6, T7 msg7, T8 msg8)
        {
            var args = TypePool.root.GetArrayE<object>(msg1, msg2, msg3, msg4, msg5, msg6, msg7, msg8);
            SendInternal(id, args);
        }
        /// <summary>发送消息</summary>
        public static void Send<T1, T2, T3, T4, T5, T6, T7, T8, T9>(TID id
            , T1 msg1, T2 msg2, T3 msg3, T4 msg4, T5 msg5, T6 msg6, T7 msg7, T8 msg8, T9 msg9)
        {
            var args = TypePool.root.GetArrayE<object>(msg1, msg2, msg3, msg4, msg5, msg6, msg7, msg8, msg9);
            SendInternal(id, args);
        }
        /// <summary>发送消息</summary>
        public static void Send<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(TID id
            , T1 msg1, T2 msg2, T3 msg3, T4 msg4, T5 msg5, T6 msg6, T7 msg7, T8 msg8, T9 msg9, T10 msg10)
        {
            var args = TypePool.root.GetArrayE<object>(msg1, msg2, msg3, msg4, msg5, msg6, msg7, msg8, msg9, msg10);
            SendInternal(id, args);
        }
        /// <summary>发送消息</summary>
        public static void Send<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(TID id
            , T1 msg1, T2 msg2, T3 msg3, T4 msg4, T5 msg5, T6 msg6, T7 msg7, T8 msg8, T9 msg9, T10 msg10, T11 msg11)
        {
            var args = TypePool.root.GetArrayE<object>(msg1, msg2, msg3, msg4, msg5, msg6, msg7, msg8, msg9, msg10, msg11);
            SendInternal(id, args);
        }
        /// <summary>发送消息</summary>
        public static void Send<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(TID id
            , T1 msg1, T2 msg2, T3 msg3, T4 msg4, T5 msg5, T6 msg6, T7 msg7, T8 msg8, T9 msg9, T10 msg10, T11 msg11, T12 msg12)
        {
            var args = TypePool.root.GetArrayE<object>(msg1, msg2, msg3, msg4, msg5, msg6, msg7, msg8, msg9, msg10, msg11);
            SendInternal(id, args);
        }
        /// <summary>发送消息</summary>
        public static void Send<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(TID id
            , T1 msg1, T2 msg2, T3 msg3, T4 msg4, T5 msg5, T6 msg6, T7 msg7, T8 msg8, T9 msg9, T10 msg10, T11 msg11, T12 msg12, T13 msg13)
        {
            var args = TypePool.root.GetArrayE<object>(msg1, msg2, msg3, msg4, msg5, msg6, msg7, msg8, msg9, msg10, msg11, msg12, msg13);
            SendInternal(id, args);
        }
        /// <summary>发送消息</summary>
        public static void Send<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(TID id
            , T1 msg1, T2 msg2, T3 msg3, T4 msg4, T5 msg5, T6 msg6, T7 msg7, T8 msg8, T9 msg9, T10 msg10, T11 msg11, T12 msg12, T13 msg13, T14 msg14)
        {
            var args = TypePool.root.GetArrayE<object>(msg1, msg2, msg3, msg4, msg5, msg6, msg7, msg8, msg9, msg10, msg11, msg12, msg13, msg14);
            SendInternal(id, args);
        }
        /// <summary>发送消息</summary>
        public static void Send<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(TID id
            , T1 msg1, T2 msg2, T3 msg3, T4 msg4, T5 msg5, T6 msg6, T7 msg7, T8 msg8, T9 msg9, T10 msg10, T11 msg11, T12 msg12, T13 msg13, T14 msg14, T15 msg15)
        {
            var args = TypePool.root.GetArrayE<object>(msg1, msg2, msg3, msg4, msg5, msg6, msg7, msg8, msg9, msg10, msg11, msg12, msg13, msg14, msg15);
            SendInternal(id, args);
        }
        /// <summary>发送消息</summary>
        public static void Send<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(TID id
            , T1 msg1, T2 msg2, T3 msg3, T4 msg4, T5 msg5, T6 msg6, T7 msg7, T8 msg8, T9 msg9, T10 msg10, T11 msg11, T12 msg12, T13 msg13, T14 msg14, T15 msg15, T16 msg16)
        {
            var args = TypePool.root.GetArrayE<object>(msg1, msg2, msg3, msg4, msg5, msg6, msg7, msg8, msg9, msg10, msg11, msg12, msg13, msg14, msg15, msg16);
            SendInternal(id, args);
        }

        #endregion

        /// <summary>发送消息</summary>
        public static void Send(TID id, params object[] args)
        {
            SendInternal(id, args, false);
        }
        /// <summary>发送消息</summary>
        internal static void SendInternal(TID id, object[] args)
        {
            SendInternal(id, args, true);
        }
        /// <summary>发送消息</summary>
        internal static void SendInternal(TID id, object[] args, bool returnPool)
        {
            _eventManager.SendInternal(id, args, returnPool);
        } 
        #endregion
    }

}