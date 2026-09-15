using System;
using System.Collections;
using System.Collections.Generic;

namespace Framework.ObjectPool
{
    /// <summary>简化版的 <see cref="TypePool"/>，只用于内部，避免监视根池时污染被监视对象。</summary>
    internal class InternalTypePool
    {
        public static InternalTypePool root { get; } = new InternalTypePool();

        private readonly Dictionary<Type, List<object>> _pool = new Dictionary<Type, List<object>>(4);

        /// <summary>从对象池获取</summary>
        public T Get<T>()
        {
            TryGet(typeof(T), out var obj);
            return (T)obj;
        }

        /// <summary>获取 <see cref="List{T}"/></summary>
        public List<T> GetList<T>() => Get<List<T>>();

        /// <summary>获取 <see cref="Dictionary{TKey, TValue}"/></summary>
        public Dictionary<TKey, TValue> GetDic<TKey, TValue>() => Get<Dictionary<TKey, TValue>>();

        /// <summary>从对象池获取</summary>
        public bool TryGet<T>(out T obj)
        {
            bool r = TryGet(typeof(T), out var o);
            obj = (T)o;
            return r;
        }

        /// <summary>从对象池获取</summary>
        public bool TryGet(Type type, out object obj)
        {
            obj = null;
            if (type == null) return false;

            if (_pool.TryGetValue(type, out var tPool) && tPool.Count > 0)
            {
                obj = Fetch(tPool);
            }

            if (obj == null)
            {
                obj = Activator.CreateInstance(type, true);
            }

            InitializeObject(obj);
            return true;
        }

        /// <summary>返回对象池</summary>
        /// <remarks>对于 <see cref="ITypePoolObject"/> 对象会做清理工作</remarks>
        public void Return<T>(T obj) where T : class
        {
            if (obj == null) return;

            var target = obj.GetType();
            if (!_pool.TryGetValue(target, out var tPool))
            {
                tPool = new List<object>(4);
                _pool[target] = tPool;
            }

            if (!tPool.Contains(obj))
            {
                tPool.Add(obj);
                CleanupObject(obj);
            }
        }

        /// <summary>返回对象池</summary>
        /// <remarks>会清空</remarks>
        public void Return<T>(List<T> v)
        {
            if (v == null) return;
            v.Clear();
            Return<List<T>>(v);
        }

        /// <summary>返回对象池</summary>
        /// <remarks>会清空</remarks>
        public void Return<TKey, TValue>(Dictionary<TKey, TValue> v)
        {
            if (v == null) return;
            v.Clear();
            Return<Dictionary<TKey, TValue>>(v);
        }

        /// <summary>取出最后一个元素，取出的元素会被移除</summary>
        private static object Fetch(List<object> tPool)
        {
            int index = tPool.Count - 1;
            var obj = tPool[index];
            tPool.RemoveAt(index);
            return obj;
        }

        /// <summary>清理 <see cref="ITypePoolObject.Clear()"/></summary>
        private static void CleanupObject(object obj)
        {
            if (obj is ITypePoolObject tpo) tpo.Clear();
        }

        /// <summary>初始 <see cref="ITypePoolObjectInit.Init()"/></summary>
        private static void InitializeObject(object obj)
        {
            if (obj is ITypePoolObjectInit tpo) tpo.Init();
        }
    }
}
