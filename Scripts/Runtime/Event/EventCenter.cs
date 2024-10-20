// -------------------------
// 创建日期：2023/10/19 1:41:25
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Framework.Event
{
    /// <summary>
    /// 事件处理中心
    /// <para>事件消息支持所有类型的参数，并且参数支持面向对象特性，
    /// 推荐的用法是消息继承 <see cref="IEventMessage"/></para>
    /// <code>
    /// public class Msg1 : IEventMessage { }
    /// public class Msg2 : IEventMessage { }
    /// </code>
    /// <para>注意：当以类型作为 id 时，不要用系统类型作为 id，应该自定义专用的消息类型作为 id，为了通用性，将不对作为 id 的类型进行限制</para>
    /// </summary>
    public static partial class EventCenter
    {
        static Dictionary<string, LinkedList<Delegate>> _entrepot = new Dictionary<string, LinkedList<Delegate>>(1000);

        ///// <summary>添加侦听</summary>
        //public static void AddListener<T>(EventHandler<T> listener) where T : IEventMessage
        //{
        //    var id = typeof(T).GetHashCode().ToString();
        //    AddListener(id, listener);
        //}
        ///// <summary>添加侦听</summary>
        //public static void AddListener<T>(string id, EventHandler<T> listener) where T : IEventMessage
        //{
        //    AddListener(id, listener as Delegate);
        //}

        /// <summary>添加侦听，以 <see cref="Type.GetHashCode"/> 为 id</summary>
        public static void AddListener<T>(Action listener)
        {
            var id = typeof(T).GetHashCode().ToString();
            AddListener(id, listener);
        }
        /// <summary>添加侦听，以 <see cref="Type.GetHashCode"/> 为 id</summary>
        public static void AddListener(Type tId, Action listener)
        {
            var id = tId.GetHashCode().ToString();
            AddListener(id, listener);
        }
        /// <summary>添加侦听，以 <see cref="Type.GetHashCode"/> 为 id</summary>
        public static void AddListener<T>(Action<T> listener)
        {
            var id = typeof(T).GetHashCode().ToString();
            AddListener(id, listener);
        }
        /// <summary>添加侦听，以 <see cref="Type.GetHashCode"/> 为 id</summary>
        public static void AddListener<T>(Type tId, Action<T> listener)
        {
            var id = tId.GetHashCode().ToString();
            AddListener(id, listener);
        }
        /// <summary>添加侦听</summary>
        public static void AddListener(string id, Action listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public static void AddListener<T1>(string id, Action<T1> listener)
        {
            AddListener(id, listener as Delegate);
        }

        #region 添加侦听，多参数
        /// <summary>添加侦听</summary>
        public static void AddListener<T1, T2>(string id, Action<T1, T2> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public static void AddListener<T1, T2, T3>(string id, Action<T1, T2, T3> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public static void AddListener<T1, T2, T3, T4>(string id,
            Action<T1, T2, T3, T4> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public static void AddListener<T1, T2, T3, T4, T5>(string id,
            Action<T1, T2, T3, T4, T5> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public static void AddListener<T1, T2, T3, T4, T5, T6>(string id,
            Action<T1, T2, T3, T4, T5, T6> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public static void AddListener<T1, T2, T3, T4, T5, T6, T7>(string id,
            Action<T1, T2, T3, T4, T5, T6, T7> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public static void AddListener<T1, T2, T3, T4, T5, T6, T7, T8>(string id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public static void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public static void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public static void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(string id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public static void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(string id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public static void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(string id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public static void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(string id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public static void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(string id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public static void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(string id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> listener)
        {
            AddListener(id, listener as Delegate);
        }

        ///// <summary>添加侦听</summary>
        //public static void AddListener<T>(string id, Action<T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T> listener)
        //{
        //    AddListener(id, listener as Delegate);
        //}
        ///// <summary>添加侦听</summary>
        //public static void AddListener<T>(string id, Action<T, T, T, T, T, T, T, T, T, T, T, T, T, T, T> listener)
        //{
        //    AddListener(id, listener as Delegate);
        //}
        ///// <summary>添加侦听</summary>
        //public static void AddListener<T>(string id, Action<T, T, T, T, T, T, T, T, T, T, T, T, T, T> listener)
        //{
        //    AddListener(id, listener as Delegate);
        //}
        ///// <summary>添加侦听</summary>
        //public static void AddListener<T>(string id, Action<T, T, T, T, T, T, T, T, T, T, T, T, T> listener)
        //{
        //    AddListener(id, listener as Delegate);
        //}
        ///// <summary>添加侦听</summary>
        //public static void AddListener<T>(string id, Action<T, T, T, T, T, T, T, T, T, T, T, T> listener)
        //{
        //    AddListener(id, listener as Delegate);
        //}
        ///// <summary>添加侦听</summary>
        //public static void AddListener<T>(string id, Action<T, T, T, T, T, T, T, T, T, T, T> listener)
        //{
        //    AddListener(id, listener as Delegate);
        //}
        ///// <summary>添加侦听</summary>
        //public static void AddListener<T>(string id, Action<T, T, T, T, T, T, T, T, T, T> listener)
        //{
        //    AddListener(id, listener as Delegate);
        //}
        ///// <summary>添加侦听</summary>
        //public static void AddListener<T>(string id, Action<T, T, T, T, T, T, T, T, T> listener)
        //{
        //    AddListener(id, listener as Delegate);
        //}
        ///// <summary>添加侦听</summary>
        //public static void AddListener<T>(string id, Action<T, T, T, T, T, T, T, T> listener)
        //{
        //    AddListener(id, listener as Delegate);
        //}
        ///// <summary>添加侦听</summary>
        //public static void AddListener<T>(string id, Action<T, T, T, T, T, T, T> listener)
        //{
        //    AddListener(id, listener as Delegate);
        //}
        ///// <summary>添加侦听</summary>
        //public static void AddListener<T>(string id, Action<T, T, T, T, T, T> listener)
        //{
        //    AddListener(id, listener as Delegate);
        //}
        ///// <summary>添加侦听</summary>
        //public static void AddListener<T>(string id, Action<T, T, T, T, T> listener)
        //{
        //    AddListener(id, listener as Delegate);
        //}
        ///// <summary>添加侦听</summary>
        //public static void AddListener<T>(string id, Action<T, T, T, T> listener)
        //{
        //    AddListener(id, listener as Delegate);
        //}
        ///// <summary>添加侦听</summary>
        //public static void AddListener<T>(string id, Action<T, T, T> listener)
        //{
        //    AddListener(id, listener as Delegate);
        //}
        ///// <summary>添加侦听</summary>
        //public static void AddListener<T>(string id, Action<T, T> listener)
        //{
        //    AddListener(id, listener as Delegate);
        //}
        #endregion

        /// <summary>添加侦听</summary>
        public static void AddListener(string id, Delegate listener)
        {
            if (listener == null) return;

            if (!_entrepot.ContainsKey(id))
                _entrepot[id] = new LinkedList<Delegate>();
            if (!_entrepot[id].Contains(listener))
                _entrepot[id].AddLast(listener);
        }


        ///// <summary>移除侦听</summary>
        //public static void RemoveListener<T>(EventHandler<T> listener) where T : IEventMessage
        //{
        //    var id = typeof(T).GetHashCode().ToString();
        //    RemoveListener(id, listener);
        //}
        ///// <summary>移除侦听</summary>
        //public static void RemoveListener<T>(string id, EventHandler<T> listener) where T : IEventMessage
        //{
        //    RemoveListener(id, listener as Delegate);
        //}

        /// <summary>移除侦听，以 <see cref="Type.GetHashCode"/> 为 id</summary>
        public static void RemoveListener<T>(Action listener)
        {
            var id = typeof(T).GetHashCode().ToString();
            RemoveListener(id, listener);
        }
        /// <summary>移除侦听，以 <see cref="Type.GetHashCode"/> 为 id</summary>
        public static void RemoveListener<T>(Type tId, Action listener)
        {
            var id = tId.GetHashCode().ToString();
            RemoveListener(id, listener);
        }
        /// <summary>移除侦听，以 <see cref="Type.GetHashCode"/> 为 id</summary>
        public static void RemoveListener<T>(Action<T> listener)
        {
            var id = typeof(T).GetHashCode().ToString();
            RemoveListener(id, listener);
        }
        /// <summary>移除侦听，以 <see cref="Type.GetHashCode"/> 为 id</summary>
        public static void RemoveListener<T>(Type tId, Action<T> listener)
        {
            var id = tId.GetHashCode().ToString();
            RemoveListener(id, listener);
        }
        /// <summary>移除侦听</summary>
        public static void RemoveListener(string id, Action listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public static void RemoveListener<T1>(string id, Action<T1> listener)
        {
            RemoveListener(id, listener as Delegate);
        }

        #region 移除侦听，多参数
        /// <summary>移除侦听</summary>
        public static void RemoveListener<T1, T2>(string id, Action<T1, T2> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public static void RemoveListener<T1, T2, T3>(string id, Action<T1, T2, T3> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public static void RemoveListener<T1, T2, T3, T4>(string id,
            Action<T1, T2, T3, T4> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public static void RemoveListener<T1, T2, T3, T4, T5>(string id,
            Action<T1, T2, T3, T4, T5> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public static void RemoveListener<T1, T2, T3, T4, T5, T6>(string id,
            Action<T1, T2, T3, T4, T5, T6> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public static void RemoveListener<T1, T2, T3, T4, T5, T6, T7>(string id,
            Action<T1, T2, T3, T4, T5, T6, T7> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public static void RemoveListener<T1, T2, T3, T4, T5, T6, T7, T8>(string id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public static void RemoveListener<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public static void RemoveListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public static void RemoveListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(string id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public static void RemoveListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(string id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public static void RemoveListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(string id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public static void RemoveListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(string id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public static void RemoveListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(string id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public static void RemoveListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(string id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> listener)
        {
            RemoveListener(id, listener as Delegate);
        }

        ///// <summary>移除侦听</summary>
        //public static void RemoveListener<T>(string id, Action<T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T> listener)
        //{
        //    RemoveListener(id, listener as Delegate);
        //}
        ///// <summary>移除侦听</summary>
        //public static void RemoveListener<T>(string id, Action<T, T, T, T, T, T, T, T, T, T, T, T, T, T, T> listener)
        //{
        //    RemoveListener(id, listener as Delegate);
        //}
        ///// <summary>移除侦听</summary>
        //public static void RemoveListener<T>(string id, Action<T, T, T, T, T, T, T, T, T, T, T, T, T, T> listener)
        //{
        //    RemoveListener(id, listener as Delegate);
        //}
        ///// <summary>移除侦听</summary>
        //public static void RemoveListener<T>(string id, Action<T, T, T, T, T, T, T, T, T, T, T, T, T> listener)
        //{
        //    RemoveListener(id, listener as Delegate);
        //}
        ///// <summary>移除侦听</summary>
        //public static void RemoveListener<T>(string id, Action<T, T, T, T, T, T, T, T, T, T, T, T> listener)
        //{
        //    RemoveListener(id, listener as Delegate);
        //}
        ///// <summary>移除侦听</summary>
        //public static void RemoveListener<T>(string id, Action<T, T, T, T, T, T, T, T, T, T, T> listener)
        //{
        //    RemoveListener(id, listener as Delegate);
        //}
        ///// <summary>移除侦听</summary>
        //public static void RemoveListener<T>(string id, Action<T, T, T, T, T, T, T, T, T, T> listener)
        //{
        //    RemoveListener(id, listener as Delegate);
        //}
        ///// <summary>移除侦听</summary>
        //public static void RemoveListener<T>(string id, Action<T, T, T, T, T, T, T, T, T> listener)
        //{
        //    RemoveListener(id, listener as Delegate);
        //}
        ///// <summary>移除侦听</summary>
        //public static void RemoveListener<T>(string id, Action<T, T, T, T, T, T, T, T> listener)
        //{
        //    RemoveListener(id, listener as Delegate);
        //}
        ///// <summary>移除侦听</summary>
        //public static void RemoveListener<T>(string id, Action<T, T, T, T, T, T, T> listener)
        //{
        //    RemoveListener(id, listener as Delegate);
        //}
        ///// <summary>移除侦听</summary>
        //public static void RemoveListener<T>(string id, Action<T, T, T, T, T, T> listener)
        //{
        //    RemoveListener(id, listener as Delegate);
        //}
        ///// <summary>移除侦听</summary>
        //public static void RemoveListener<T>(string id, Action<T, T, T, T, T> listener)
        //{
        //    RemoveListener(id, listener as Delegate);
        //}
        ///// <summary>移除侦听</summary>
        //public static void RemoveListener<T>(string id, Action<T, T, T, T> listener)
        //{
        //    RemoveListener(id, listener as Delegate);
        //}
        ///// <summary>移除侦听</summary>
        //public static void RemoveListener<T>(string id, Action<T, T, T> listener)
        //{
        //    RemoveListener(id, listener as Delegate);
        //}
        ///// <summary>移除侦听</summary>
        //public static void RemoveListener<T>(string id, Action<T, T> listener)
        //{
        //    RemoveListener(id, listener as Delegate);
        //}
        #endregion

        /// <summary>移除侦听</summary>
        public static void RemoveListener(string id, Delegate listener)
        {
            if (listener == null) return;

            if (_entrepot.ContainsKey(id))
                //if (_entrepot[id].Contains(listener))
                _entrepot[id].Remove(listener);
        }

        #region 清除监听
        /// <summary>清除指定 id 的所有监听</summary>
        public static void Clear<T>()
        {
            var id = typeof(T).GetHashCode().ToString();
            Clear(id);
        }
        /// <summary>清除指定 id 的所有监听</summary>
        public static void Clear(Type tId)
        {
            var id = tId.GetHashCode().ToString();
            Clear(id);
        }
        /// <summary>清除指定 id 的所有监听</summary>
        public static void Clear(string id)
        {
            if (_entrepot.TryGetValue(id, out var msgs))
            {
                msgs.Clear();
            }
        }
        /// <summary>清除所有监听</summary>
        public static void Clear()
        {
            foreach (var msgs in _entrepot)
            {
                msgs.Value?.Clear();
            }
        }
        /// <summary>清空消息库</summary>
        public static void ClearAll()
        {
            Clear();

            _entrepot?.Clear();
        }
        #endregion

        /// <summary>发送消息，以 <see cref="Type.GetHashCode"/> 为 id</summary>
        public static void Send<T>()
        {
            var id = typeof(T).GetHashCode().ToString();
            SendInternal(id, null);
        }
        /// <summary>发送消息，以 <see cref="Type.GetHashCode"/> 为 id</summary>
        public static void Send(Type tId)
        {
            var id = tId.GetHashCode().ToString();
            SendInternal(id, null);
        }
        /// <summary>发送消息，以 <see cref="Type.GetHashCode"/> 为 id</summary>
        public static void Send<T>(T msg)
        {
            var id = typeof(T).GetHashCode().ToString();
            Send(id, msg);
        }
        /// <summary>发送消息，以 <see cref="Type.GetHashCode"/> 为 id</summary>
        public static void Send<T>(Type tId, T msg)
        {
            var id = tId.GetHashCode().ToString();
            Send(id, msg);
        }
        /// <summary>发送消息</summary>
        public static void Send(string id)
        {
            SendInternal(id, null);
        }
        /// <summary>发送消息</summary>
        public static void Send<T1>(string id, T1 msg1)
        {
            var args = TypePool.root.GetArrayE<object>(msg1);
            SendInternal(id, args);
        }

        #region 发送消息，多参数
        /// <summary>发送消息</summary>
        public static void Send<T1, T2>(string id, T1 msg1, T2 msg2)
        {
            var args = TypePool.root.GetArrayE<object>(msg1, msg2);
            SendInternal(id, args);
        }
        /// <summary>发送消息</summary>
        public static void Send<T1, T2, T3>(string id, T1 msg1, T2 msg2, T3 msg3)
        {
            var args = TypePool.root.GetArrayE<object>(msg1, msg2, msg3);
            SendInternal(id, args);
        }
        /// <summary>发送消息</summary>
        public static void Send<T1, T2, T3, T4>(string id
            , T1 msg1, T2 msg2, T3 msg3, T4 msg4)
        {
            var args = TypePool.root.GetArrayE<object>(msg1, msg2, msg3);
            SendInternal(id, args);
        }
        /// <summary>发送消息</summary>
        public static void Send<T1, T2, T3, T4, T5>(string id
            , T1 msg1, T2 msg2, T3 msg3, T4 msg4, T5 msg5)
        {
            var args = TypePool.root.GetArrayE<object>(msg1, msg2, msg3, msg4, msg5);
            SendInternal(id, args);
        }
        /// <summary>发送消息</summary>
        public static void Send<T1, T2, T3, T4, T5, T6>(string id
            , T1 msg1, T2 msg2, T3 msg3, T4 msg4, T5 msg5, T6 msg6)
        {
            var args = TypePool.root.GetArrayE<object>(msg1, msg2, msg3, msg4, msg5, msg6);
            SendInternal(id, args);
        }
        /// <summary>发送消息</summary>
        public static void Send<T1, T2, T3, T4, T5, T6, T7>(string id
            , T1 msg1, T2 msg2, T3 msg3, T4 msg4, T5 msg5, T6 msg6, T7 msg7)
        {
            var args = TypePool.root.GetArrayE<object>(msg1, msg2, msg3, msg4, msg5, msg6, msg7);
            SendInternal(id, args);
        }
        /// <summary>发送消息</summary>
        public static void Send<T1, T2, T3, T4, T5, T6, T7, T8>(string id
            , T1 msg1, T2 msg2, T3 msg3, T4 msg4, T5 msg5, T6 msg6, T7 msg7, T8 msg8)
        {
            var args = TypePool.root.GetArrayE<object>(msg1, msg2, msg3, msg4, msg5, msg6, msg7, msg8);
            SendInternal(id, args);
        }
        /// <summary>发送消息</summary>
        public static void Send<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string id
            , T1 msg1, T2 msg2, T3 msg3, T4 msg4, T5 msg5, T6 msg6, T7 msg7, T8 msg8, T9 msg9)
        {
            var args = TypePool.root.GetArrayE<object>(msg1, msg2, msg3, msg4, msg5, msg6, msg7, msg8, msg9);
            SendInternal(id, args);
        }
        /// <summary>发送消息</summary>
        public static void Send<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string id
            , T1 msg1, T2 msg2, T3 msg3, T4 msg4, T5 msg5, T6 msg6, T7 msg7, T8 msg8, T9 msg9, T10 msg10)
        {
            var args = TypePool.root.GetArrayE<object>(msg1, msg2, msg3, msg4, msg5, msg6, msg7, msg8, msg9, msg10);
            SendInternal(id, args);
        }
        /// <summary>发送消息</summary>
        public static void Send<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(string id
            , T1 msg1, T2 msg2, T3 msg3, T4 msg4, T5 msg5, T6 msg6, T7 msg7, T8 msg8, T9 msg9, T10 msg10, T11 msg11)
        {
            var args = TypePool.root.GetArrayE<object>(msg1, msg2, msg3, msg4, msg5, msg6, msg7, msg8, msg9, msg10, msg11);
            SendInternal(id, args);
        }
        /// <summary>发送消息</summary>
        public static void Send<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(string id
            , T1 msg1, T2 msg2, T3 msg3, T4 msg4, T5 msg5, T6 msg6, T7 msg7, T8 msg8, T9 msg9, T10 msg10, T11 msg11, T12 msg12)
        {
            var args = TypePool.root.GetArrayE<object>(msg1, msg2, msg3, msg4, msg5, msg6, msg7, msg8, msg9, msg10, msg11);
            SendInternal(id, args);
        }
        /// <summary>发送消息</summary>
        public static void Send<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(string id
            , T1 msg1, T2 msg2, T3 msg3, T4 msg4, T5 msg5, T6 msg6, T7 msg7, T8 msg8, T9 msg9, T10 msg10, T11 msg11, T12 msg12, T13 msg13)
        {
            var args = TypePool.root.GetArrayE<object>(msg1, msg2, msg3, msg4, msg5, msg6, msg7, msg8, msg9, msg10, msg11, msg12, msg13);
            SendInternal(id, args);
        }
        /// <summary>发送消息</summary>
        public static void Send<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(string id
            , T1 msg1, T2 msg2, T3 msg3, T4 msg4, T5 msg5, T6 msg6, T7 msg7, T8 msg8, T9 msg9, T10 msg10, T11 msg11, T12 msg12, T13 msg13, T14 msg14)
        {
            var args = TypePool.root.GetArrayE<object>(msg1, msg2, msg3, msg4, msg5, msg6, msg7, msg8, msg9, msg10, msg11, msg12, msg13, msg14);
            SendInternal(id, args);
        }
        /// <summary>发送消息</summary>
        public static void Send<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(string id
            , T1 msg1, T2 msg2, T3 msg3, T4 msg4, T5 msg5, T6 msg6, T7 msg7, T8 msg8, T9 msg9, T10 msg10, T11 msg11, T12 msg12, T13 msg13, T14 msg14, T15 msg15)
        {
            var args = TypePool.root.GetArrayE<object>(msg1, msg2, msg3, msg4, msg5, msg6, msg7, msg8, msg9, msg10, msg11, msg12, msg13, msg14, msg15);
            SendInternal(id, args);
        }
        /// <summary>发送消息</summary>
        public static void Send<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(string id
            , T1 msg1, T2 msg2, T3 msg3, T4 msg4, T5 msg5, T6 msg6, T7 msg7, T8 msg8, T9 msg9, T10 msg10, T11 msg11, T12 msg12, T13 msg13, T14 msg14, T15 msg15, T16 msg16)
        {
            var args = TypePool.root.GetArrayE<object>(msg1, msg2, msg3, msg4, msg5, msg6, msg7, msg8, msg9, msg10, msg11, msg12, msg13, msg14, msg15, msg16);
            SendInternal(id, args);
        }

        ///// <summary>发送消息</summary>
        //public static void Send<T>(string id, T msg1, T msg2, T msg3, T msg4, T msg5, T msg6, T msg7, T msg8, T msg9, T msg10, T msg11, T msg12, T msg13, T msg14, T msg15, T msg16)
        //{
        //    var args = TypePool.root.GetArrayE<object>(msg1, msg2, msg3, msg4, msg5, msg6, msg7, msg8, msg9, msg10, msg11, msg12, msg13, msg14, msg15, msg16);
        //    Send(id, args);
        //}
        ///// <summary>发送消息</summary>
        //public static void Send<T>(string id, T msg1, T msg2, T msg3, T msg4, T msg5, T msg6, T msg7, T msg8, T msg9, T msg10, T msg11, T msg12, T msg13, T msg14, T msg15)
        //{
        //    var args = TypePool.root.GetArrayE<object>(msg1, msg2, msg3, msg4, msg5, msg6, msg7, msg8, msg9, msg10, msg11, msg12, msg13, msg14, msg15);
        //    Send(id, args);
        //}
        ///// <summary>发送消息</summary>
        //public static void Send<T>(string id, T msg1, T msg2, T msg3, T msg4, T msg5, T msg6, T msg7, T msg8, T msg9, T msg10, T msg11, T msg12, T msg13, T msg14)
        //{
        //    var args = TypePool.root.GetArrayE<object>(msg1, msg2, msg3, msg4, msg5, msg6, msg7, msg8, msg9, msg10, msg11, msg12, msg13, msg14);
        //    Send(id, args);
        //}
        ///// <summary>发送消息</summary>
        //public static void Send<T>(string id, T msg1, T msg2, T msg3, T msg4, T msg5, T msg6, T msg7, T msg8, T msg9, T msg10, T msg11, T msg12, T msg13)
        //{
        //    var args = TypePool.root.GetArrayE<object>(msg1, msg2, msg3, msg4, msg5, msg6, msg7, msg8, msg9, msg10, msg11, msg12, msg13);
        //    Send(id, args);
        //}
        ///// <summary>发送消息</summary>
        //public static void Send<T>(string id, T msg1, T msg2, T msg3, T msg4, T msg5, T msg6, T msg7, T msg8, T msg9, T msg10, T msg11, T msg12)
        //{
        //    var args = TypePool.root.GetArrayE<object>(msg1, msg2, msg3, msg4, msg5, msg6, msg7, msg8, msg9, msg10, msg11, msg12);
        //    Send(id, args);
        //}
        ///// <summary>发送消息</summary>
        //public static void Send<T>(string id, T msg1, T msg2, T msg3, T msg4, T msg5, T msg6, T msg7, T msg8, T msg9, T msg10, T msg11)
        //{
        //    var args = TypePool.root.GetArrayE<object>(msg1, msg2, msg3, msg4, msg5, msg6, msg7, msg8, msg9, msg10, msg11);
        //    Send(id, args);
        //}
        ///// <summary>发送消息</summary>
        //public static void Send<T>(string id, T msg1, T msg2, T msg3, T msg4, T msg5, T msg6, T msg7, T msg8, T msg9, T msg10)
        //{
        //    var args = TypePool.root.GetArrayE<object>(msg1, msg2, msg3, msg4, msg5, msg6, msg7, msg8, msg9, msg10);
        //    Send(id, args);
        //}
        ///// <summary>发送消息</summary>
        //public static void Send<T>(string id, T msg1, T msg2, T msg3, T msg4, T msg5, T msg6, T msg7, T msg8, T msg9)
        //{
        //    var args = TypePool.root.GetArrayE<object>(msg1, msg2, msg3, msg4, msg5, msg6, msg7, msg8, msg9);
        //    Send(id, args);
        //}
        ///// <summary>发送消息</summary>
        //public static void Send<T>(string id, T msg1, T msg2, T msg3, T msg4, T msg5, T msg6, T msg7, T msg8)
        //{
        //    var args = TypePool.root.GetArrayE<object>(msg1, msg2, msg3, msg4, msg5, msg6, msg7, msg8);
        //    Send(id, args);
        //}
        ///// <summary>发送消息</summary>
        //public static void Send<T>(string id, T msg1, T msg2, T msg3, T msg4, T msg5, T msg6, T msg7)
        //{
        //    var args = TypePool.root.GetArrayE<object>(msg1, msg2, msg3, msg4, msg5, msg6, msg7);
        //    Send(id, args);
        //}
        ///// <summary>发送消息</summary>
        //public static void Send<T>(string id, T msg1, T msg2, T msg3, T msg4, T msg5, T msg6)
        //{
        //    var args = TypePool.root.GetArrayE<object>(msg1, msg2, msg3, msg4, msg5, msg6);
        //    Send(id, args);
        //}
        ///// <summary>发送消息</summary>
        //public static void Send<T>(string id, T msg1, T msg2, T msg3, T msg4, T msg5)
        //{
        //    var args = TypePool.root.GetArrayE<object>(msg1, msg2, msg3, msg4, msg5);
        //    Send(id, args);
        //}
        ///// <summary>发送消息</summary>
        //public static void Send<T>(string id, T msg1, T msg2, T msg3, T msg4)
        //{
        //    var args = TypePool.root.GetArrayE<object>(msg1, msg2, msg3, msg4);
        //    Send(id, args);
        //}
        ///// <summary>发送消息</summary>
        //public static void Send<T>(string id, T msg1, T msg2, T msg3)
        //{
        //    var args = TypePool.root.GetArrayE<object>(msg1, msg2, msg3);
        //    Send(id, args);
        //}
        ///// <summary>发送消息</summary>
        //public static void Send<T>(string id, T msg1, T msg2)
        //{
        //    var args = TypePool.root.GetArrayE<object>(msg1, msg2);
        //    Send(id, args);
        //}
        #endregion

        /// <summary>发送消息</summary>
        public static void Send(string id, params object[] args)
        {
            SendInternal(id, args, false);
        }
        /// <summary>发送消息</summary>
        internal static void SendInternal(string id, object[] args)
        {
            SendInternal(id, args, true);
        }
        /// <summary>发送消息</summary>
        internal static void SendInternal(string id, object[] args, bool returnPool)
        {
            if (!_entrepot.ContainsKey(id)) return;

            var msgs = _entrepot[id];
            if (msgs.Count > 0)
            {
                var node = msgs.First;
                while (node != null)
                {
                    //if (e.Value is Action<T> ea)
                    //    ea.Invoke(msg);

                    node.Value.DynamicInvoke(args);

                    node = node.Next;
                }
            }

            if (returnPool)
                TypePool.root.Return(args);
        }
    }

    ///// <summary>
    ///// 用于事件消息处理
    ///// </summary>
    ///// <typeparam name="T"><see cref="IEventMessage"/></typeparam>
    //public delegate void EventHandler<T>(T msg) where T : IEventMessage;

}