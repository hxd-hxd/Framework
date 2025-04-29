// -------------------------
// 创建日期：2023/10/19 1:41:25
// -------------------------

using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Framework.Event
{
    public static partial class EventCenter
    {
        #region 添加侦听
        /// <summary>添加侦听</summary>
        public static void AddListener(int id, Action listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public static void AddListener<T>(int id, Action<T> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public static void AddListener<T1, T2>(int id, Action<T1, T2> listener)
        {
            AddListener(id, listener as Delegate);
        }
        /// <summary>添加侦听</summary>
        public static void AddListener<T1, T2, T3>(int id, Action<T1, T2, T3> listener)
        {
            AddListener(id, listener as Delegate);
        }

        #endregion


        #region 移除侦听
        /// <summary>移除侦听</summary>
        public static void RemoveListener(int id, Action listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public static void RemoveListener<T>(int id, Action<T> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public static void RemoveListener<T1, T2>(int id, Action<T1, T2> listener)
        {
            RemoveListener(id, listener as Delegate);
        }
        /// <summary>移除侦听</summary>
        public static void RemoveListener<T1, T2, T3>(int id, Action<T1, T2, T3> listener)
        {
            RemoveListener(id, listener as Delegate);
        }

        #endregion

    }

}