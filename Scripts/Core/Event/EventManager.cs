// -------------------------
// 创建日期：2023/10/19 1:41:25
// -------------------------

using System;
using System.Collections;
using System.Collections.Generic;

namespace Framework.Event
{
    /// <summary>
    /// 事件管理器
    /// </summary>
    public partial class EventManager<TID> : IEventManager<TID>, ITypePoolObject
    {
        private Dictionary<TID, LinkedList<Delegate>> _entrepot;

        public EventManager()
        {
            _entrepot = new Dictionary<TID, LinkedList<Delegate>>(20);
        }

        #region 添加侦听
        /// <summary>添加侦听</summary>
        public void AddListener(TID id, Action listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public void AddListener<T1>(TID id, Action<T1> listener)
        {
            AddListener(id, listener as Delegate);
        }

        #region 添加侦听，多参数
        /// <summary>添加侦听</summary>
        public void AddListener<T1, T2>(TID id, Action<T1, T2> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public void AddListener<T1, T2, T3>(TID id, Action<T1, T2, T3> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public void AddListener<T1, T2, T3, T4>(TID id,
            Action<T1, T2, T3, T4> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public void AddListener<T1, T2, T3, T4, T5>(TID id,
            Action<T1, T2, T3, T4, T5> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public void AddListener<T1, T2, T3, T4, T5, T6>(TID id,
            Action<T1, T2, T3, T4, T5, T6> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public void AddListener<T1, T2, T3, T4, T5, T6, T7>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public void AddListener<T1, T2, T3, T4, T5, T6, T7, T8>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> listener)
        {
            AddListener(id, listener as Delegate);
        }

        #endregion

        /// <summary>添加侦听</summary>
        public virtual void AddListener(TID id, Delegate listener)
        {
            if (listener == null) return;

            if (!_entrepot.ContainsKey(id) || _entrepot[id] == null)
                _entrepot[id] = new LinkedList<Delegate>();
            if (!_entrepot[id].Contains(listener))
                _entrepot[id].AddLast(listener);
        }
        #endregion


        #region 移除侦听
        /// <summary>移除侦听</summary>
        public void RemoveListener(TID id, Action listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public void RemoveListener<T1>(TID id, Action<T1> listener)
        {
            RemoveListener(id, listener as Delegate);
        }

        #region 移除侦听，多参数
        /// <summary>移除侦听</summary>
        public void RemoveListener<T1, T2>(TID id, Action<T1, T2> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public void RemoveListener<T1, T2, T3>(TID id, Action<T1, T2, T3> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public void RemoveListener<T1, T2, T3, T4>(TID id,
            Action<T1, T2, T3, T4> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public void RemoveListener<T1, T2, T3, T4, T5>(TID id,
            Action<T1, T2, T3, T4, T5> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public void RemoveListener<T1, T2, T3, T4, T5, T6>(TID id,
            Action<T1, T2, T3, T4, T5, T6> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public void RemoveListener<T1, T2, T3, T4, T5, T6, T7>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public void RemoveListener<T1, T2, T3, T4, T5, T6, T7, T8>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public void RemoveListener<T1, T2, T3, T4, T5, T6, T7, T8, T9>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public void RemoveListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public void RemoveListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public void RemoveListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public void RemoveListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public void RemoveListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public void RemoveListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public void RemoveListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> listener)
        {
            RemoveListener(id, listener as Delegate);
        }

        #endregion

        /// <summary>移除侦听</summary>
        public virtual void RemoveListener(TID id, Delegate listener)
        {
            if (listener == null) return;

            if (_entrepot.ContainsKey(id))
                //if (_entrepot[id].Contains(listener))
                _entrepot[id].Remove(listener);
        }
        #endregion


        #region 发送消息
        /// <summary>发送消息</summary>
        public void Send(TID id)
        {
            if (!_entrepot.ContainsKey(id)) return;

            var msgs = _entrepot[id];
            if (msgs.Count > 0)
            {
                var node = msgs.First;
                while (node != null)
                {
                    if (node.Value is Action ea)
                        ea.Invoke();

                    node = node.Next;
                }
            }
        }
        /// <summary>发送消息</summary>
        public void Send<T1>(TID id, T1 msg1)
        {
            if (!_entrepot.ContainsKey(id)) return;

            var msgs = _entrepot[id];
            if (msgs.Count > 0)
            {
                var node = msgs.First;
                while (node != null)
                {
                    if (node.Value is Action<T1> ea)
                        ea.Invoke(msg1);

                    node = node.Next;
                }
            }
        }

        #region 发送消息，多参数
        /// <summary>发送消息</summary>
        public void Send<T1, T2>(TID id, T1 msg1, T2 msg2)
        {
            if (!_entrepot.ContainsKey(id)) return;

            var msgs = _entrepot[id];
            if (msgs.Count > 0)
            {
                var node = msgs.First;
                while (node != null)
                {
                    if (node.Value is Action<T1, T2> ea)
                        ea.Invoke(msg1, msg2);

                    node = node.Next;
                }
            }
        }
        /// <summary>发送消息</summary>
        public void Send<T1, T2, T3>(TID id, T1 msg1, T2 msg2, T3 msg3)
        {
            if (!_entrepot.ContainsKey(id)) return;

            var msgs = _entrepot[id];
            if (msgs.Count > 0)
            {
                var node = msgs.First;
                while (node != null)
                {
                    if (node.Value is Action<T1, T2, T3> ea)
                        ea.Invoke(msg1, msg2, msg3);

                    node = node.Next;
                }
            }
        }
        /// <summary>发送消息</summary>
        public void Send<T1, T2, T3, T4>(TID id
            , T1 msg1, T2 msg2, T3 msg3, T4 msg4)
        {
            if (!_entrepot.ContainsKey(id)) return;

            var msgs = _entrepot[id];
            if (msgs.Count > 0)
            {
                var node = msgs.First;
                while (node != null)
                {
                    if (node.Value is Action<T1, T2, T3, T4> ea)
                        ea.Invoke(msg1, msg2, msg3, msg4);

                    node = node.Next;
                }
            }
        }
        /// <summary>发送消息</summary>
        public void Send<T1, T2, T3, T4, T5>(TID id
            , T1 msg1, T2 msg2, T3 msg3, T4 msg4, T5 msg5)
        {
            if (!_entrepot.ContainsKey(id)) return;

            var msgs = _entrepot[id];
            if (msgs.Count > 0)
            {
                var node = msgs.First;
                while (node != null)
                {
                    if (node.Value is Action<T1, T2, T3, T4, T5> ea)
                        ea.Invoke(msg1, msg2, msg3, msg4, msg5);

                    node = node.Next;
                }
            }
        }
        /// <summary>发送消息</summary>
        public void Send<T1, T2, T3, T4, T5, T6>(TID id
            , T1 msg1, T2 msg2, T3 msg3, T4 msg4, T5 msg5, T6 msg6)
        {
            if (!_entrepot.ContainsKey(id)) return;

            var msgs = _entrepot[id];
            if (msgs.Count > 0)
            {
                var node = msgs.First;
                while (node != null)
                {
                    if (node.Value is Action<T1, T2, T3, T4, T5, T6> ea)
                        ea.Invoke(msg1, msg2, msg3, msg4, msg5, msg6);

                    node = node.Next;
                }
            }
        }
        /// <summary>发送消息</summary>
        public void Send<T1, T2, T3, T4, T5, T6, T7>(TID id
            , T1 msg1, T2 msg2, T3 msg3, T4 msg4, T5 msg5, T6 msg6, T7 msg7)
        {
            if (!_entrepot.ContainsKey(id)) return;

            var msgs = _entrepot[id];
            if (msgs.Count > 0)
            {
                var node = msgs.First;
                while (node != null)
                {
                    if (node.Value is Action<T1, T2, T3, T4, T5, T6, T7> ea)
                        ea.Invoke(msg1, msg2, msg3, msg4, msg5, msg6, msg7);

                    node = node.Next;
                }
            }
        }
        /// <summary>发送消息</summary>
        public void Send<T1, T2, T3, T4, T5, T6, T7, T8>(TID id
            , T1 msg1, T2 msg2, T3 msg3, T4 msg4, T5 msg5, T6 msg6, T7 msg7, T8 msg8)
        {
            if (!_entrepot.ContainsKey(id)) return;

            var msgs = _entrepot[id];
            if (msgs.Count > 0)
            {
                var node = msgs.First;
                while (node != null)
                {
                    if (node.Value is Action<T1, T2, T3, T4, T5, T6, T7, T8> ea)
                        ea.Invoke(msg1, msg2, msg3, msg4, msg5, msg6, msg7, msg8);

                    node = node.Next;
                }
            }
        }
        /// <summary>发送消息</summary>
        public void Send<T1, T2, T3, T4, T5, T6, T7, T8, T9>(TID id
            , T1 msg1, T2 msg2, T3 msg3, T4 msg4, T5 msg5, T6 msg6, T7 msg7, T8 msg8, T9 msg9)
        {
            if (!_entrepot.ContainsKey(id)) return;

            var msgs = _entrepot[id];
            if (msgs.Count > 0)
            {
                var node = msgs.First;
                while (node != null)
                {
                    if (node.Value is Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> ea)
                        ea.Invoke(msg1, msg2, msg3, msg4, msg5, msg6, msg7, msg8, msg9);

                    node = node.Next;
                }
            }
        }
        /// <summary>发送消息</summary>
        public void Send<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(TID id
            , T1 msg1, T2 msg2, T3 msg3, T4 msg4, T5 msg5, T6 msg6, T7 msg7, T8 msg8, T9 msg9, T10 msg10)
        {
            if (!_entrepot.ContainsKey(id)) return;

            var msgs = _entrepot[id];
            if (msgs.Count > 0)
            {
                var node = msgs.First;
                while (node != null)
                {
                    if (node.Value is Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> ea)
                        ea.Invoke(msg1, msg2, msg3, msg4, msg5, msg6, msg7, msg8, msg9, msg10);

                    node = node.Next;
                }
            }
        }
        /// <summary>发送消息</summary>
        public void Send<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(TID id
            , T1 msg1, T2 msg2, T3 msg3, T4 msg4, T5 msg5, T6 msg6, T7 msg7, T8 msg8, T9 msg9, T10 msg10, T11 msg11)
        {
            if (!_entrepot.ContainsKey(id)) return;

            var msgs = _entrepot[id];
            if (msgs.Count > 0)
            {
                var node = msgs.First;
                while (node != null)
                {
                    if (node.Value is Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> ea)
                        ea.Invoke(msg1, msg2, msg3, msg4, msg5, msg6, msg7, msg8, msg9, msg10, msg11);

                    node = node.Next;
                }
            }
        }
        /// <summary>发送消息</summary>
        public void Send<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(TID id
            , T1 msg1, T2 msg2, T3 msg3, T4 msg4, T5 msg5, T6 msg6, T7 msg7, T8 msg8, T9 msg9, T10 msg10, T11 msg11, T12 msg12)
        {
            if (!_entrepot.ContainsKey(id)) return;

            var msgs = _entrepot[id];
            if (msgs.Count > 0)
            {
                var node = msgs.First;
                while (node != null)
                {
                    if (node.Value is Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> ea)
                        ea.Invoke(msg1, msg2, msg3, msg4, msg5, msg6, msg7, msg8, msg9, msg10, msg11, msg12);

                    node = node.Next;
                }
            }
        }
        /// <summary>发送消息</summary>
        public void Send<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(TID id
            , T1 msg1, T2 msg2, T3 msg3, T4 msg4, T5 msg5, T6 msg6, T7 msg7, T8 msg8, T9 msg9, T10 msg10, T11 msg11, T12 msg12, T13 msg13)
        {
            if (!_entrepot.ContainsKey(id)) return;

            var msgs = _entrepot[id];
            if (msgs.Count > 0)
            {
                var node = msgs.First;
                while (node != null)
                {
                    if (node.Value is Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> ea)
                        ea.Invoke(msg1, msg2, msg3, msg4, msg5, msg6, msg7, msg8, msg9, msg10, msg11, msg12, msg13);

                    node = node.Next;
                }
            }
        }
        /// <summary>发送消息</summary>
        public void Send<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(TID id
            , T1 msg1, T2 msg2, T3 msg3, T4 msg4, T5 msg5, T6 msg6, T7 msg7, T8 msg8, T9 msg9, T10 msg10, T11 msg11, T12 msg12, T13 msg13, T14 msg14)
        {
            if (!_entrepot.ContainsKey(id)) return;

            var msgs = _entrepot[id];
            if (msgs.Count > 0)
            {
                var node = msgs.First;
                while (node != null)
                {
                    if (node.Value is Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> ea)
                        ea.Invoke(msg1, msg2, msg3, msg4, msg5, msg6, msg7, msg8, msg9, msg10, msg11, msg12, msg13, msg14);

                    node = node.Next;
                }
            }
        }
        /// <summary>发送消息</summary>
        public void Send<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(TID id
            , T1 msg1, T2 msg2, T3 msg3, T4 msg4, T5 msg5, T6 msg6, T7 msg7, T8 msg8, T9 msg9, T10 msg10, T11 msg11, T12 msg12, T13 msg13, T14 msg14, T15 msg15)
        {
            if (!_entrepot.ContainsKey(id)) return;

            var msgs = _entrepot[id];
            if (msgs.Count > 0)
            {
                var node = msgs.First;
                while (node != null)
                {
                    if (node.Value is Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> ea)
                        ea.Invoke(msg1, msg2, msg3, msg4, msg5, msg6, msg7, msg8, msg9, msg10, msg11, msg12, msg13, msg14, msg15);

                    node = node.Next;
                }
            }
        }
        /// <summary>发送消息</summary>
        public void Send<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(TID id
            , T1 msg1, T2 msg2, T3 msg3, T4 msg4, T5 msg5, T6 msg6, T7 msg7, T8 msg8, T9 msg9, T10 msg10, T11 msg11, T12 msg12, T13 msg13, T14 msg14, T15 msg15, T16 msg16)
        {
            if (!_entrepot.ContainsKey(id)) return;

            var msgs = _entrepot[id];
            if (msgs.Count > 0)
            {
                var node = msgs.First;
                while (node != null)
                {
                    if (node.Value is Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> ea)
                        ea.Invoke(msg1, msg2, msg3, msg4, msg5, msg6, msg7, msg8, msg9, msg10, msg11, msg12, msg13, msg14, msg15, msg16);

                    node = node.Next;
                }
            }
        }

        #endregion

        /// <summary>发送消息</summary>
        public virtual void Send(TID id, params object[] args)
        {
            SendInternal(id, args, false);
        }
        /// <summary>发送消息</summary>
        internal void SendInternal(TID id, object[] args)
        {
            SendInternal(id, args, true);
        }
        /// <summary>发送消息</summary>
        internal void SendInternal(TID id, object[] args, bool returnPool)
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
        #endregion


        #region 清除监听
        /// <summary>清除指定 id 的所有监听</summary>
        public virtual void Clear(TID id)
        {
            if (_entrepot.TryGetValue(id, out var listeners))
            {
                listeners?.Clear();
            }
        }
        /// <summary>清除所有监听</summary>
        public virtual void Clear()
        {
            foreach (var listeners in _entrepot)
            {
                listeners.Value?.Clear();
            }
        }
        /// <summary>清空消息库</summary>
        public virtual void ClearAll()
        {
            Clear();

            _entrepot?.Clear();
        }
        #endregion


        public bool IsListening(TID id)
        {
            return _entrepot.ContainsKey(id) && _entrepot[id].Count > 0;
        }

        public bool IsListening(TID id, Delegate listener)
        {
            if (_entrepot.ContainsKey(id))
                return _entrepot[id].Contains(listener);
            return false;
        }


        #region 显式实现
        void IEventManager.Clear<TID1>(TID1 id)
        {
            if (id is TID tid)
                Clear(tid);
            else throw new TypeAccessException($"类型必须是“{typeof(TID)}”，而不是“{typeof(TID1)}”");
        }

        void IEventManager.Clear<TID1>()
        {
            if (typeof(TID) == typeof(TID1))
                Clear();
            else throw new TypeAccessException($"类型必须是“{typeof(TID)}”，而不是“{typeof(TID1)}”");
        }

        void IEventManager.AddListener<TID1>(TID1 id, Delegate listener)
        {
            if (id is TID tid)
                AddListener(tid, listener);
            else throw new TypeAccessException($"类型必须是“{typeof(TID)}”，而不是“{typeof(TID1)}”");
        }

        void IEventManager.RemoveListener<TID1>(TID1 id, Delegate listener)
        {
            if (id is TID tid)
                RemoveListener(tid, listener);
            else throw new TypeAccessException($"类型必须是“{typeof(TID)}”，而不是“{typeof(TID1)}”");
        }

        void IEventManager.Send<TID1>(TID1 id, params object[] args)
        {
            if (id is TID tid)
                Send(tid, args);
            else throw new TypeAccessException($"类型必须是“{typeof(TID)}”，而不是“{typeof(TID1)}”");
        }

        void IEventManager.Send<TID1>(TID1 id)
        {
            if (id is TID tid)
                Send(tid);
            else throw new TypeAccessException($"类型必须是“{typeof(TID)}”，而不是“{typeof(TID1)}”");
        }

        void IEventManager.Send<TID1, T>(TID1 id, T msg)
        {
            if (id is TID tid)
                Send(tid, msg);
            else throw new TypeAccessException($"类型必须是“{typeof(TID)}”，而不是“{typeof(TID1)}”");
        }

        bool IEventManager.IsListening<TID1>(TID1 id)
        {
            if (id is TID tid)
                return IsListening(tid);
            else throw new TypeAccessException($"类型必须是“{typeof(TID)}”，而不是“{typeof(TID1)}”");
        }

        bool IEventManager.IsListening<TID1>(TID1 id, Delegate listener)
        {
            if (id is TID tid)
                return IsListening(tid, listener);
            else throw new TypeAccessException($"类型必须是“{typeof(TID)}”，而不是“{typeof(TID1)}”");
        }

        //void IEventManager.Clear(object id)
        //{
        //    if (id is TID tid)
        //    {
        //        Clear(tid);
        //    }
        //}

        //void IEventManager.AddListener(object id, Delegate listener)
        //{
        //    if (id is TID tid)
        //    {
        //        AddListener(tid, listener);
        //    }
        //}

        //void IEventManager.RemoveListener(object id, Delegate listener)
        //{
        //    if (id is TID tid)
        //    {
        //        RemoveListener(tid, listener);
        //    }
        //}

        //void IEventManager.Send(object id, params object[] args)
        //{
        //    if (id is TID tid)
        //    {
        //        Send(tid, args);
        //    }
        //} 
        #endregion
    }

}