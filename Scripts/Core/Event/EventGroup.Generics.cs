// -------------------------
// 创建日期：2023/10/19 1:41:25
// -------------------------

using System;
using System.Collections;
using System.Collections.Generic;

namespace Framework.Event
{
    /// <summary>
    /// 事件组
    /// </summary>
    public partial class EventGroup<TID> : IEventGroup<TID>, ITypePoolObject
    {
        Dictionary<TID, LinkedList<Delegate>> _entrepot = new Dictionary<TID, LinkedList<Delegate>>(20);

        #region 添加侦听
        /// <summary>添加侦听</summary>
        public void AddListener(TID id, Action listener)
        {
            AddListener(id, listener as Delegate);
        }

        #region 添加侦听，多参数
        /// <summary>添加侦听</summary>
        public void AddListener<T1>(TID id, Action<T1> listener)
        {
            AddListener(id, listener as Delegate);
        }
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
        public void AddListener(TID id, Delegate listener)
        {
            if (listener == null) return;

            if (!_entrepot.ContainsKey(id) || _entrepot[id] == null)
                _entrepot[id] = new LinkedList<Delegate>();
            if (!_entrepot[id].Contains(listener))
                _entrepot[id].AddLast(listener);

            EventCenter<TID>.AddListener(id, listener);
        }
        #endregion


        #region 移除侦听
        /// <summary>移除侦听</summary>
        public void RemoveListener(TID id, Action listener)
        {
            RemoveListener(id, listener as Delegate);
        }

        #region 移除侦听，多参数
        /// <summary>移除侦听</summary>
        public void RemoveListener<T1>(TID id, Action<T1> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
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
        public void RemoveListener(TID id, Delegate listener)
        {
            if (listener == null) return;

            if (_entrepot.ContainsKey(id))
                _entrepot[id].Remove(listener);

            EventCenter<TID>.RemoveListener(id, listener);
        }
        #endregion

        /// <summary>清除所有监听</summary>
        public void Clear()
        {
            foreach (var msgs in _entrepot)
            {
                TID id = msgs.Key;
                Clear(id);
                msgs.Value.Clear();
            }
            //_entrepot.Clear();
        }
        /// <summary>清除指定 id 所有监听</summary>
        public void Clear(TID id)
        {
            if (_entrepot.TryGetValue(id, out var es))
            {
                foreach (var msg in es)
                {
                    EventCenter<TID>.RemoveListener(id, msg);
                }
                es.Clear();
            }
        }
        public void ClearAll()
        {
            Clear();

            _entrepot.Clear();
        }

        void IEventGroup.Clear<TID1>(TID1 id)
        {
            if (id is TID tid)
                Clear(tid);
        }

        void IEventGroup.Clear<TID1>()
        {
            if (typeof(TID) == typeof(TID1))
                Clear();
        }

        void IEventGroup.AddListener<TID1>(TID1 id, Delegate listener)
        {
            if (id is TID tid)
                AddListener(tid, listener);
        }

        void IEventGroup.RemoveListener<TID1>(TID1 id, Delegate listener)
        {
            if (id is TID tid)
                RemoveListener(tid, listener);
        }

    }
}