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
    /// 事件组
    /// </summary>
    public class EventGroup
    {
        static Dictionary<string, LinkedList<Delegate>> _entrepot = new Dictionary<string, LinkedList<Delegate>>(20);

        /// <summary>添加侦听，以 <see cref="Type.GetHashCode"/> 为 id</summary>
        public void AddListener<T>(Action listener)
        {
            AddListener<T>(listener);
        }
        /// <summary>添加侦听，以 <see cref="Type.GetHashCode"/> 为 id</summary>
        public void AddListener(Type tId, Action listener)
        {
            var id = tId.GetHashCode().ToString();
            AddListener(id, listener);
        }
        /// <summary>添加侦听，以 <see cref="Type.GetHashCode"/> 为 id</summary>
        public void AddListener<T>(Action<T> listener)
        {
            var id = typeof(T).GetHashCode().ToString();
            AddListener(id, listener);
        }
        /// <summary>添加侦听，以 <see cref="Type.GetHashCode"/> 为 id</summary>
        public void AddListener<T>(Type tId, Action<T> listener)
        {
            var id = tId.GetHashCode().ToString();
            AddListener(id, listener);
        }
        /// <summary>添加侦听</summary>
        public void AddListener(string id, Action listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public void AddListener<T1>(string id, Action<T1> listener)
        {
            AddListener(id, listener as Delegate);
        }

        #region 添加侦听，多参数
        /// <summary>添加侦听</summary>
        public void AddListener<T1, T2>(string id, Action<T1, T2> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public void AddListener<T1, T2, T3>(string id, Action<T1, T2, T3> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public void AddListener<T1, T2, T3, T4>(string id,
            Action<T1, T2, T3, T4> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public void AddListener<T1, T2, T3, T4, T5>(string id,
            Action<T1, T2, T3, T4, T5> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public void AddListener<T1, T2, T3, T4, T5, T6>(string id,
            Action<T1, T2, T3, T4, T5, T6> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public void AddListener<T1, T2, T3, T4, T5, T6, T7>(string id,
            Action<T1, T2, T3, T4, T5, T6, T7> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public void AddListener<T1, T2, T3, T4, T5, T6, T7, T8>(string id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(string id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(string id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(string id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(string id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(string id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public void AddListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(string id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> listener)
        {
            AddListener(id, listener as Delegate);
        }

        #endregion

        /// <summary>添加侦听</summary>
        public void AddListener(string id, Delegate listener)
        {
            if (listener == null) return;

            if (!_entrepot.ContainsKey(id))
                _entrepot[id] = new LinkedList<Delegate>();
            if (!_entrepot[id].Contains(listener))
                _entrepot[id].AddLast(listener);

            EventCenter.AddListener(id, listener);
        }


        /// <summary>移除侦听，以 <see cref="Type.GetHashCode"/> 为 id</summary>
        public void RemoveListener<T>(Action listener)
        {
            var id = typeof(T).GetHashCode().ToString();
            RemoveListener(id, listener);
        }
        /// <summary>移除侦听，以 <see cref="Type.GetHashCode"/> 为 id</summary>
        public void RemoveListener<T>(Type tId, Action listener)
        {
            var id = tId.GetHashCode().ToString();
            RemoveListener(id, listener);
        }
        /// <summary>移除侦听，以 <see cref="Type.GetHashCode"/> 为 id</summary>
        public void RemoveListener<T>(Action<T> listener)
        {
            var id = typeof(T).GetHashCode().ToString();
            RemoveListener(id, listener);
        }
        /// <summary>移除侦听，以 <see cref="Type.GetHashCode"/> 为 id</summary>
        public void RemoveListener<T>(Type tId, Action<T> listener)
        {
            var id = tId.GetHashCode().ToString();
            RemoveListener(id, listener);
        }
        /// <summary>移除侦听</summary>
        public void RemoveListener(string id, Action listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public void RemoveListener<T1>(string id, Action<T1> listener)
        {
            RemoveListener(id, listener as Delegate);
        }

        #region 移除侦听，多参数
        /// <summary>移除侦听</summary>
        public void RemoveListener<T1, T2>(string id, Action<T1, T2> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public void RemoveListener<T1, T2, T3>(string id, Action<T1, T2, T3> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public void RemoveListener<T1, T2, T3, T4>(string id,
            Action<T1, T2, T3, T4> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public void RemoveListener<T1, T2, T3, T4, T5>(string id,
            Action<T1, T2, T3, T4, T5> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public void RemoveListener<T1, T2, T3, T4, T5, T6>(string id,
            Action<T1, T2, T3, T4, T5, T6> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public void RemoveListener<T1, T2, T3, T4, T5, T6, T7>(string id,
            Action<T1, T2, T3, T4, T5, T6, T7> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public void RemoveListener<T1, T2, T3, T4, T5, T6, T7, T8>(string id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public void RemoveListener<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public void RemoveListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public void RemoveListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(string id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public void RemoveListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(string id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public void RemoveListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(string id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public void RemoveListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(string id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public void RemoveListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(string id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public void RemoveListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(string id,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> listener)
        {
            RemoveListener(id, listener as Delegate);
        }

        #endregion

        /// <summary>移除侦听</summary>
        public void RemoveListener(string id, Delegate listener)
        {
            if (listener == null) return;

            if (_entrepot.ContainsKey(id))
                //if (_entrepot[id].Contains(listener))
                _entrepot[id].Remove(listener);

            EventCenter.RemoveListener(id, listener);
        }

        /// <summary>清除所有监听</summary>
        public void Clear()
        {
            foreach (var msgs in _entrepot)
            {
                foreach (var msg in msgs.Value)
                {
                    EventCenter.RemoveListener(msgs.Key, msg);
                }
                msgs.Value.Clear();
            }
            _entrepot.Clear();
        }
    }
}