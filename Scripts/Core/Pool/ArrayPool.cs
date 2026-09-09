// -------------------------
// 创建日期：2023/10/19 1:41:25
// -------------------------

using System;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{
    // 由于数组大小不变的特殊性，故需要单独处理
    /// <summary>数组池</summary>
    public sealed class ArrayPool
    {
        private Dictionary<Type, Dictionary<int, List<Array>>> _pool;
        private HashSet<Array> _queryCache = new HashSet<Array>();

        public ArrayPool() : this(1)
        {
        }

        public ArrayPool(int capacity)
        {
            _pool = new Dictionary<Type, Dictionary<int, List<Array>>>(capacity);
        }

        public IReadOnlyDictionary<Type, Dictionary<int, List<Array>>> pool => _pool;

        /// <summary>池子数量，每个类型对应一个池子</summary>
        public int poolCount => _pool.Count;

        /// <summary>池子里的对象数量</summary>
        public int itemSize
        {
            get
            {
                int sum = 0;
                foreach (var item in _pool)
                {
                    sum += GetFreeCount(item.Key);
                }
                return sum;
            }
        }

        /// <summary>创建实例</summary>
        private Array CreateInstance(Type elementType, int length) => Array.CreateInstance(elementType, length);

        /// <summary>获取 <typeparamref name="T"/>[]</summary>
        public T[] GetArray<T>(int length) => GetArray(typeof(T), length) as T[];

        /// <summary>获取 <see cref="Array"/></summary>
        public Array GetArray(Type elementType, int length)
        {
            if (elementType == null) throw new ArgumentNullException(nameof(elementType), "元素类型为空");
            if (length < 0) throw new ArgumentOutOfRangeException(nameof(length), "数组长度小于 0");

            TryGetArray(elementType, length, out var obj);
            return obj;
        }

        /// <summary>从对象池获取</summary>
        public bool TryGetArray<T>(int length, out T[] obj)
        {
            var r = TryGetArray(typeof(T), length, out var a);
            obj = a as T[];
            return r;
        }

        /// <summary>从对象池获取</summary>
        public bool TryGetArray(Type elementType, int length, out Array obj)
        {
            obj = null;
            if (elementType == null) return false;
            if (length < 0) return false;

            Type target = elementType.MakeArrayType();
            bool has = _pool.TryGetValue(target, out var tPool);

            if (has)
            {
                if (tPool.TryGetValue(length, out var aPool))
                {
                    obj = Fetch(aPool);
                }
            }

            if (obj == null)
            {
                obj = CreateInstance(elementType, length);
            }
            return true;
        }


        #region 转换成数组
        /// <summary>转换成可重复利用的 <typeparamref name="T"/>[] 数组
        /// </summary>
        public T[] ToArray<T>(ICollection<T> v)
        {
            if (v == null) return null;
            return ToArray<T>(v, (Func<T, T>)null);
        }

        /// <summary>转换成可重复利用的数组
        /// <para><typeparamref name="T"/>：要转换的目标数组元素类型</para>
        /// </summary>
        public T[] ToArray<T, S>(ICollection<S> v)
        {
            if (v == null) return null;
            return ToArray<T, S>(v, (Func<S, T>)null);
        }

        /// <summary>转换成可重复利用的 <typeparamref name="T"/>[] 数组，自行处理元素的转换结果
        /// </summary>
        public T[] ToArray<T>(ICollection<T> v, Func<T, T> handle)
        {
            if (v == null) return null;
            var objs = GetArray<T>(v.Count);
            int i = 0;
            foreach (var t in v)
            {
                if (handle != null)
                    objs[i] = handle(t);
                else
                    objs[i] = t;
                i++;
            }
            return objs;
        }

        /// <summary>转换成可重复利用的数组，自行处理元素的转换结果
        /// <para><typeparamref name="T"/>：要转换的目标数组元素类型</para>
        /// </summary>
        public T[] ToArray<T, S>(ICollection<S> v, Func<S, T> handle)
        {
            if (v == null) return null;
            var objs = GetArray<T>(v.Count);
            int i = 0;
            foreach (var t in v)
            {
                if (handle != null)
                    objs[i] = handle(t);
                else
                    objs[i] = (T)(object)t;
                i++;
            }
            return objs;
        }

        #endregion


        #region 返回对象池

        /// <summary>返回对象池</summary>
        /// <remarks>会将元素置为默认值</remarks>
        public void Return<T>(T[] v)
        {
            if (v == null) return;
            Return(v as Array);
        }

        /// <summary>返回对象池</summary>
        /// <remarks>会将元素置为默认值</remarks>
        public void Return(Array v)
        {
            if (v == null) return;

            int length = v.Length;
            var target = v.GetType();

            if (!_pool.TryGetValue(target, out var tPool))
            {
                tPool = CreatePool();
                _pool[target] = tPool;
            }

            if (!tPool.TryGetValue(length, out var aPool))
            {
                aPool = new List<Array>();
                tPool[length] = aPool;
            }

            if (!_queryCache.Contains(v))
            {
                _queryCache.Add(v);
                aPool.Add(v);

                CleanupObject(v);
            }
        }

        #endregion

        /// <summary>获取池中指定类型实例的可用数量
        /// <para><typeparamref name="T"/>：数组类型，而非元素类型，例如：typeof(int[])，而非 typeof(int)</para>
        /// </summary>
        public int GetFreeCount<T>()
        {
            return GetFreeCount(typeof(T));
        }

        /// <summary>获取池中指定类型实例的可用数量
        /// <para><paramref name="type"/>：数组类型，而非元素类型，例如：typeof(int[])，而非 typeof(int)</para>
        /// </summary>
        public int GetFreeCount(Type type)
        {
            int count = 0;
            var has = _pool.TryGetValue(type, out var tPool);
            if (has)
            {
                foreach (var item in tPool)
                {
                    count += item.Value.Count;
                }
            }
            return count;
        }

        /// <summary>清除对象池</summary>
        public void Clear()
        {
            foreach (var item in _pool)
            {
                item.Value?.Clear();
            }
            _pool.Clear();
            _queryCache.Clear();
        }


        /// <summary>取出最后一个元素，避免中间的元素挪动影响性能，取出的元素会被移除</summary>
        private T Fetch<T>(List<T> tPool)
        {
            if (tPool.Count <= 0) return default;
            return Fetch(tPool, tPool.Count - 1);
        }

        /// <summary>取出指定索引处元素，取出的元素会被移除</summary>
        private T Fetch<T>(List<T> tPool, int index)
        {
            if (tPool.Count <= 0) return default;
            var _obj = tPool[index];
            tPool.RemoveAt(index);
            if (_obj is Array a) _queryCache.Remove(a);
            return _obj;
        }

        private Dictionary<int, List<Array>> CreatePool()
        {
            return new Dictionary<int, List<Array>>(5);
        }

        private void CleanupObject(Array v)
        {
            Array.Clear(v, 0, v.Length);
        }

    }
}