// -------------------------
// 创建日期：2023/10/19 1:41:25
// -------------------------

using System;
using System.Collections;
using System.Collections.Generic;
//using System.Linq;

namespace Framework.Event
{
    /// <summary>
    /// 事件管理器
    /// </summary>
    public partial class EventManager<TID> : IEventManager<TID>, ITypePoolObject
    {
        //private Dictionary<TID, List<Delegate>> _entrepot;
        private Dictionary<TID, List<EventListener>> _entrepot;

        public EventManager()
        {
            _entrepot = new Dictionary<TID, List<EventListener>>(20);
        }

        #region 添加侦听
        /// <summary>添加侦听</summary>
        public void AddListener(TID id, Action listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听
        /// <para><paramref name="priority"/>：监听执行优先级，数值越大越先执行，同优先级遵循先来后到原则</para>
        /// </summary>
        public void AddListener(TID id, Action listener, int priority)
        {
            AddListener(id, listener as Delegate, priority);
        }
        /// <summary>添加侦听</summary>
        public void AddListener<T1>(TID id, Action<T1> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听
        /// <para><paramref name="priority"/>：监听执行优先级，数值越大越先执行，同优先级遵循先来后到原则</para>
        /// </summary>
        public void AddListener<T1>(TID id, Action<T1> listener, int priority)
        {
            AddListener(id, listener as Delegate, priority);
        }

        #region 添加侦听，多参数
        /// <summary>添加侦听</summary>
        public void AddListener<T1, T2>(TID id, Action<T1, T2> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听
        /// <para><paramref name="priority"/>：监听执行优先级，数值越大越先执行，同优先级遵循先来后到原则</para>
        /// </summary>
        public void AddListener<T1, T2>(TID id, Action<T1, T2> listener, int priority)
        {
            AddListener(id, listener as Delegate, priority);
        }
        /// <summary>添加侦听</summary>
        public void AddListener<T1, T2, T3>(TID id, Action<T1, T2, T3> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听
        /// <para><paramref name="priority"/>：监听执行优先级，数值越大越先执行，同优先级遵循先来后到原则</para>
        /// </summary>
        public void AddListener<T1, T2, T3>(TID id, Action<T1, T2, T3> listener, int priority)
        {
            AddListener(id, listener as Delegate, priority);
        }
        /// <summary>添加侦听</summary>
        public void AddListener<T1, T2, T3, T4>(TID id,
            Action<T1, T2, T3, T4> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听
        /// <para><paramref name="priority"/>：监听执行优先级，数值越大越先执行，同优先级遵循先来后到原则</para>
        /// </summary>
        public void AddListener<T1, T2, T3, T4>(TID id,
            Action<T1, T2, T3, T4> listener, int priority)
        {
            AddListener(id, listener as Delegate, priority);
        }
        /// <summary>添加侦听</summary>
        public void AddListener<T1, T2, T3, T4, T5>(TID id,
            Action<T1, T2, T3, T4, T5> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听
        /// <para><paramref name="priority"/>：监听执行优先级，数值越大越先执行，同优先级遵循先来后到原则</para>
        /// </summary>
        public void AddListener<T1, T2, T3, T4, T5>(TID id,
            Action<T1, T2, T3, T4, T5> listener, int priority)
        {
            AddListener(id, listener as Delegate, priority);
        }
        /// <summary>添加侦听</summary>
        public void AddListener<T1, T2, T3, T4, T5, T6>(TID id,
            Action<T1, T2, T3, T4, T5, T6> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听
        /// <para><paramref name="priority"/>：监听执行优先级，数值越大越先执行，同优先级遵循先来后到原则</para>
        /// </summary>
        public void AddListener<T1, T2, T3, T4, T5, T6>(TID id,
            Action<T1, T2, T3, T4, T5, T6> listener, int priority)
        {
            AddListener(id, listener as Delegate, priority);
        }
        /// <summary>添加侦听</summary>
        public void AddListener<T1, T2, T3, T4, T5, T6, T7>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听
        /// <para><paramref name="priority"/>：监听执行优先级，数值越大越先执行，同优先级遵循先来后到原则</para>
        /// </summary>
        public void AddListener<T1, T2, T3, T4, T5, T6, T7>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7> listener, int priority)
        {
            AddListener(id, listener as Delegate, priority);
        }
        /// <summary>添加侦听</summary>
        public void AddListener<T1, T2, T3, T4, T5, T6, T7, T8>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听
        /// <para><paramref name="priority"/>：监听执行优先级，数值越大越先执行，同优先级遵循先来后到原则</para>
        /// </summary>
        public void AddListener<T1, T2, T3, T4, T5, T6, T7, T8>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8> listener, int priority)
        {
            AddListener(id, listener as Delegate, priority);
        }
        /// <summary>添加侦听</summary>
        public void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听
        /// <para><paramref name="priority"/>：监听执行优先级，数值越大越先执行，同优先级遵循先来后到原则</para>
        /// </summary>
        public void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> listener, int priority)
        {
            AddListener(id, listener as Delegate, priority);
        }
        /// <summary>添加侦听</summary>
        public void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听
        /// <para><paramref name="priority"/>：监听执行优先级，数值越大越先执行，同优先级遵循先来后到原则</para>
        /// </summary>
        public void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> listener, int priority)
        {
            AddListener(id, listener as Delegate, priority);
        }
        /// <summary>添加侦听</summary>
        public void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听
        /// <para><paramref name="priority"/>：监听执行优先级，数值越大越先执行，同优先级遵循先来后到原则</para>
        /// </summary>
        public void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> listener, int priority)
        {
            AddListener(id, listener as Delegate, priority);
        }
        /// <summary>添加侦听</summary>
        public void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听
        /// <para><paramref name="priority"/>：监听执行优先级，数值越大越先执行，同优先级遵循先来后到原则</para>
        /// </summary>
        public void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> listener, int priority)
        {
            AddListener(id, listener as Delegate, priority);
        }
        /// <summary>添加侦听</summary>
        public void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听
        /// <para><paramref name="priority"/>：监听执行优先级，数值越大越先执行，同优先级遵循先来后到原则</para>
        /// </summary>
        public void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> listener, int priority)
        {
            AddListener(id, listener as Delegate, priority);
        }
        /// <summary>添加侦听</summary>
        public void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听
        /// <para><paramref name="priority"/>：监听执行优先级，数值越大越先执行，同优先级遵循先来后到原则</para>
        /// </summary>
        public void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> listener, int priority)
        {
            AddListener(id, listener as Delegate, priority);
        }
        /// <summary>添加侦听</summary>
        public void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听
        /// <para><paramref name="priority"/>：监听执行优先级，数值越大越先执行，同优先级遵循先来后到原则</para>
        /// </summary>
        public void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> listener, int priority)
        {
            AddListener(id, listener as Delegate, priority);
        }
        /// <summary>添加侦听</summary>
        public void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听
        /// <para><paramref name="priority"/>：监听执行优先级，数值越大越先执行，同优先级遵循先来后到原则</para>
        /// </summary>
        public void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(TID id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> listener, int priority)
        {
            AddListener(id, listener as Delegate, priority);
        }

        #endregion

        public virtual void AddListener(TID id, Delegate listener)
        {
            if (listener == null) return;

            //if (!_entrepot.ContainsKey(id) || _entrepot[id] == null)
            //    _entrepot[id] = new List<EventListener>();
            //if (!_entrepot[id].Contains(listener))
            //    _entrepot[id].Add(listener);

            // 已注册，未指定优先级则不允许按默认优先级修改
            if (IsListening(id, listener)) return;

            var el = new EventListener();
            el.listener = listener;
            AddListener(id, el);
        }

        public virtual void AddListener(TID id, Delegate listener, int priority)
        {
            if (listener == null) return;

            var el = new EventListener();
            el.listener = listener;
            el.priority = priority;
            AddListener(id, el);
        }

        internal void AddListener(TID id, EventListener el)
        {
            if (!_entrepot.TryGetValue(id, out var events) || events == null)
                events = _entrepot[id] = new List<EventListener>();

            for (int i = 0; i < events.Count; i++)
            {
                var item = events[i];
                // 不允许重复侦听
                //if (item.listener == el.listener)
                if (item == el) 
                    return;
                // 允许修改优先级
                if (item.listener == el.listener && item.priority != el.priority)
                {
                    events.RemoveAt(i);
                    break;
                }
            }

            if (events.Count == 0)
            {
                events.Add(el);
                return;
            }

            // 优先级在列表中的顺序：前大后小
            // 优先看是否在最前端
            if (el.priority > events[0].priority)
            {
                // 放到最前面
                events.Insert(0, el);
                return;
            }
            for (int i = events.Count - 1; i >= 0; i--)
            {
                var item = events[i];
                if (el.priority <= item.priority)
                {
                    events.Insert(i + 1, el);
                    return;
                }
            }
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

            //if (_entrepot.ContainsKey(id))
            //    _entrepot[id].Remove(listener);

            if (_entrepot.TryGetValue(id, out var events))
            {
                for (int i = 0; i < events.Count; i++)
                {
                    var item = events[i];
                    if (item.listener == listener)
                    {
                        events.RemoveAt(i);
                        return;
                    }
                }
            }
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
                for (int i = 0; i < msgs.Count; i++)
                {
                    if (msgs[i].listener is Action ea)
                        ea.Invoke();
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
                for (int i = 0; i < msgs.Count; i++)
                {
                    if (msgs[i].listener is Action<T1> ea)
                        ea.Invoke(msg1);
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
                for (int i = 0; i < msgs.Count; i++)
                {
                    if (msgs[i].listener is Action<T1, T2> ea)
                        ea.Invoke(msg1, msg2);
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
                for (int i = 0; i < msgs.Count; i++)
                {
                    if (msgs[i].listener is Action<T1, T2, T3> ea)
                        ea.Invoke(msg1, msg2, msg3);
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
                for (int i = 0; i < msgs.Count; i++)
                {
                    if (msgs[i].listener is Action<T1, T2, T3, T4> ea)
                        ea.Invoke(msg1, msg2, msg3, msg4);
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
                for (int i = 0; i < msgs.Count; i++)
                {
                    if (msgs[i].listener is Action<T1, T2, T3, T4, T5> ea)
                        ea.Invoke(msg1, msg2, msg3, msg4, msg5);
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
                for (int i = 0; i < msgs.Count; i++)
                {
                    if (msgs[i].listener is Action<T1, T2, T3, T4, T5, T6> ea)
                        ea.Invoke(msg1, msg2, msg3, msg4, msg5, msg6);
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
                for (int i = 0; i < msgs.Count; i++)
                {
                    if (msgs[i].listener is Action<T1, T2, T3, T4, T5, T6, T7> ea)
                        ea.Invoke(msg1, msg2, msg3, msg4, msg5, msg6, msg7);
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
                for (int i = 0; i < msgs.Count; i++)
                {
                    if (msgs[i].listener is Action<T1, T2, T3, T4, T5, T6, T7, T8> ea)
                        ea.Invoke(msg1, msg2, msg3, msg4, msg5, msg6, msg7, msg8);
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
                for (int i = 0; i < msgs.Count; i++)
                {
                    if (msgs[i].listener is Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> ea)
                        ea.Invoke(msg1, msg2, msg3, msg4, msg5, msg6, msg7, msg8, msg9);
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
                for (int i = 0; i < msgs.Count; i++)
                {
                    if (msgs[i].listener is Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> ea)
                        ea.Invoke(msg1, msg2, msg3, msg4, msg5, msg6, msg7, msg8, msg9, msg10);
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
                for (int i = 0; i < msgs.Count; i++)
                {
                    if (msgs[i].listener is Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> ea)
                        ea.Invoke(msg1, msg2, msg3, msg4, msg5, msg6, msg7, msg8, msg9, msg10, msg11);
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
                for (int i = 0; i < msgs.Count; i++)
                {
                    if (msgs[i].listener is Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> ea)
                        ea.Invoke(msg1, msg2, msg3, msg4, msg5, msg6, msg7, msg8, msg9, msg10, msg11, msg12);
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
                for (int i = 0; i < msgs.Count; i++)
                {
                    if (msgs[i].listener is Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> ea)
                        ea.Invoke(msg1, msg2, msg3, msg4, msg5, msg6, msg7, msg8, msg9, msg10, msg11, msg12, msg13);
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
                for (int i = 0; i < msgs.Count; i++)
                {
                    if (msgs[i].listener is Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> ea)
                        ea.Invoke(msg1, msg2, msg3, msg4, msg5, msg6, msg7, msg8, msg9, msg10, msg11, msg12, msg13, msg14);
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
                for (int i = 0; i < msgs.Count; i++)
                {
                    if (msgs[i].listener is Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> ea)
                        ea.Invoke(msg1, msg2, msg3, msg4, msg5, msg6, msg7, msg8, msg9, msg10, msg11, msg12, msg13, msg14, msg15);
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
                for (int i = 0; i < msgs.Count; i++)
                {
                    if (msgs[i].listener is Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> ea)
                        ea.Invoke(msg1, msg2, msg3, msg4, msg5, msg6, msg7, msg8, msg9, msg10, msg11, msg12, msg13, msg14, msg15, msg16);
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
                for (int i = 0; i < msgs.Count; i++)
                {
                    msgs[i].listener?.DynamicInvoke(args);
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
            //if (_entrepot.ContainsKey(id))
            //    return _entrepot[id].Contains(listener); 

            if (_entrepot.TryGetValue(id, out var events))
            {
                for (int i = 0; i < events.Count; i++)
                {
                    var item = events[i];
                    if (item.listener == listener)
                        return true;
                }
            }
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

        void IEventManager.AddListener<TID1>(TID1 id, Delegate listener, int priority)
        {
            if (id is TID tid)
                AddListener(tid, listener, priority);
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