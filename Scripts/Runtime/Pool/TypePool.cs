// -------------------------
// 创建日期：2023/10/19 1:41:25
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Framework
{
    /// <summary>
    /// 类型池
    /// </summary>
    [Serializable]
    public class TypePool
    {
        /// <summary>
        /// 公共池，不管理自己的对象池时使用
        /// </summary>
        public static TypePool root { get; } = new TypePool();

        protected Dictionary<Type, List<object>> _pool;

        protected Type[] _tempTypes1 = new Type[1], _tempTypes2 = new Type[2];

        public TypePool()
        {
            _pool = new Dictionary<Type, List<object>>();
        }

        public TypePool(int capacity)
        {
            _pool = new Dictionary<Type, List<object>>(capacity);
        }

        public Dictionary<Type, List<object>> pool => _pool;

        /// <summary>
        /// 池子数量，每个类型对应一个池子
        /// </summary>
        public virtual int poolCount => _pool.Count;
        /// <summary>
        /// 池子里的对象数量
        /// </summary>
        public virtual int itemSize
        {
            get
            {
                int sum = 0;
                foreach (var item in _pool)
                {
                    sum += item.Value.Count;
                }
                return _pool.Count;
            }
        }

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <returns></returns>
        protected virtual T CreateInstance<T>() => Activator.CreateInstance<T>();
        /// <summary>
        /// 创建实例
        /// </summary>
        /// <returns></returns>
        protected virtual object CreateInstance(Type type) => Activator.CreateInstance(type);
        /// <summary>
        /// 创建实例
        /// </summary>
        /// <returns></returns>
        protected virtual object CreateInstance(Type type, params object[] args) => Activator.CreateInstance(type, args);

        /// <summary>从对象池获取
        /// <para><paramref name="ctorArgs"/>：仅用于创建对象实例时，向构造函数传递的参数</para>
        /// <para>注意：要获取 <see cref="Array"/> 请使用 <paramref name="GetArray"/></para>
        /// </summary>
        public virtual T Get<T>(params object[] ctorArgs)
        {
            var target = typeof(T);
            T obj = (T)Get(target, ctorArgs);
            return obj;
        }
        /// <summary>从对象池获取
        /// <para><paramref name="ctorArgs"/>：仅用于创建对象实例时，向构造函数传递的参数</para>
        /// <para>注意：要获取 <see cref="Array"/> 请使用 <paramref name="GetArray"/></para>
        /// </summary>
        public virtual object Get(Type type, params object[] ctorArgs)
        {
            object obj = null;

            var target = type;
            var has = _pool.TryGetValue(target, out var tPool);

            if (has)
            {
                if (tPool.Count > 0)
                {
                    //tPool.TryDequeue(out obj);
                    //obj = tPool.Dequeue();
                    //obj = tPool.Last();
                    obj = Fetch(tPool);
                }
            }
            else
            {
                tPool = CreatePool();
                _pool[target] = tPool;
            }

            if (obj == null)
            {
                if (ctorArgs == null || ctorArgs.Length < 1)
                    obj = CreateInstance(type);
                else
                    obj = CreateInstance(type, ctorArgs);
            }

            InitializeObject(obj);
            //Debug.Log($"目标是 {target} ,\r\n有池子 {has}，\t从池子里取出的 <color=yellow>{_o}</color> ，\t最终得到的 {obj}");

            return obj;
        }

        /// <summary>获取 <see cref="List{T}"/></summary>
        public List<T> GetList<T>() => Get<List<T>>();
        /// <summary>获取 <see cref="List{T}"/></summary>
        public IList GetList(Type itemType)
        {
            _tempTypes1[0] = itemType;
            Type target = typeof(List<>).MakeGenericType(_tempTypes1);
            return Get(target) as IList;
        }

        /// <summary>获取 <see cref="Dictionary{TKey, TValue}"/></summary>
        public Dictionary<TKey, TValue> GetDic<TKey, TValue>() => Get<Dictionary<TKey, TValue>>();
        /// <summary>获取 <see cref="Dictionary{TKey, TValue}"/></summary>
        public IDictionary GetDic(Type keyType, Type valueType)
        {
            _tempTypes2[0] = keyType;
            _tempTypes2[1] = valueType;
            Type target = typeof(Dictionary<,>).MakeGenericType(_tempTypes2);
            return Get(target) as IDictionary;
        }

        /// <summary>获取 <see cref="Queue{T}"/></summary>
        public Queue<T> GetQueue<T>() => Get<Queue<T>>();
        /// <summary>获取 <see cref="Stack{T}"/></summary>
        public Stack<T> GetStack<T>() => Get<Stack<T>>();
        /// <summary>获取 <see cref="HashSet{T}"/></summary>
        public HashSet<T> GetHashSet<T>() => Get<HashSet<T>>();

        /// <summary>获取 <typeparamref name="T"/>[]</summary>
        //public T[] GetArray<T>(int length)
        //{
        //    object obj = null;

        //    var target = typeof(T[]);
        //    var has = _pool.TryGetValue(target, out var tPool);

        //    if (has && tPool.Count > 0)
        //    {
        //        int index = 0;
        //        for (var i = 0; i < tPool.Count; i++)
        //        {
        //            if ((tPool[i] as T[]).Length == length)
        //            {
        //                index = i;
        //                break;
        //            }
        //        }
        //        obj = Fetch(tPool, index);
        //    }
        //    else
        //    {
        //        tPool = CreatePool();
        //        _pool[target] = tPool;
        //    }

        //    if (obj == null)
        //    {
        //        obj = Array.CreateInstance(typeof(T), length);
        //    }

        //    return obj as T[];
        //}
        public T[] GetArray<T>(int length) => GetArray(typeof(T), length) as T[];
        /// <summary>获取 <see cref="Array"/></summary>
        public Array GetArray(Type elementType, int length)
        {
            if (elementType == null) throw new ArgumentNullException("元素类型为空");
            if (length < 0) throw new IndexOutOfRangeException("索引小于 0");

            object obj = null;

            Type target = elementType.MakeArrayType();
            bool has = _pool.TryGetValue(target, out var tPool);

            if (has)
            {
                if (tPool.Count > 0)
                {
                    int index = -1;
                    for (var i = 0; i < tPool.Count; i++)
                    {
                        if ((tPool[i] as Array).Length == length)
                        {
                            index = i;
                            break;
                        }
                    }

                    if (index > -1)
                        obj = Fetch(tPool, index);
                }
            }
            else
            {
                tPool = CreatePool();
                _pool[target] = tPool;
            }

            if (obj == null)
            {
                obj = Array.CreateInstance(elementType, length);
            }

            return obj as Array;
        }

        #region 填入元素的数组
        /// <summary>获取 <typeparamref name="T"/>[]，并填入指定元素</summary>
        public T[] GetArrayE<T>(T e1)
        {
            var r = GetArray<T>(1);
            int i = -1;
            r[++i] = e1;
            return r;
        }
        /// <summary>获取 <typeparamref name="T"/>[]，并填入指定元素</summary>
        public T[] GetArrayE<T>(T e1, T e2)
        {
            var r = GetArray<T>(2);
            int i = -1;
            r[++i] = e1;
            r[++i] = e2;
            return r;
        }
        /// <summary>获取 <typeparamref name="T"/>[]，并填入指定元素</summary>
        public T[] GetArrayE<T>(T e1, T e2, T e3)
        {
            var r = GetArray<T>(3);
            int i = -1;
            r[++i] = e1;
            r[++i] = e2;
            r[++i] = e3;
            return r;
        }
        /// <summary>获取 <typeparamref name="T"/>[]，并填入指定元素</summary>
        public T[] GetArrayE<T>(T e1, T e2, T e3, T e4)
        {
            var r = GetArray<T>(4);
            int i = -1;
            r[++i] = e1;
            r[++i] = e2;
            r[++i] = e3;
            r[++i] = e4;
            return r;
        }
        /// <summary>获取 <typeparamref name="T"/>[]，并填入指定元素</summary>
        public T[] GetArrayE<T>(T e1, T e2, T e3, T e4, T e5)
        {
            var r = GetArray<T>(5);
            int i = -1;
            r[++i] = e1;
            r[++i] = e2;
            r[++i] = e3;
            r[++i] = e4;
            r[++i] = e5;
            return r;
        }
        /// <summary>获取 <typeparamref name="T"/>[]，并填入指定元素</summary>
        public T[] GetArrayE<T>(T e1, T e2, T e3, T e4, T e5, T e6)
        {
            var r = GetArray<T>(6);
            int i = -1;
            r[++i] = e1;
            r[++i] = e2;
            r[++i] = e3;
            r[++i] = e4;
            r[++i] = e5;
            r[++i] = e6;
            return r;
        }
        /// <summary>获取 <typeparamref name="T"/>[]，并填入指定元素</summary>
        public T[] GetArrayE<T>(T e1, T e2, T e3, T e4, T e5, T e6, T e7)
        {
            var r = GetArray<T>(7);
            int i = -1;
            r[++i] = e1;
            r[++i] = e2;
            r[++i] = e3;
            r[++i] = e4;
            r[++i] = e5;
            r[++i] = e6;
            r[++i] = e7;
            return r;
        }
        /// <summary>获取 <typeparamref name="T"/>[]，并填入指定元素</summary>
        public T[] GetArrayE<T>(T e1, T e2, T e3, T e4, T e5, T e6, T e7, T e8)
        {
            var r = GetArray<T>(8);
            int i = -1;
            r[++i] = e1;
            r[++i] = e2;
            r[++i] = e3;
            r[++i] = e4;
            r[++i] = e5;
            r[++i] = e6;
            r[++i] = e7;
            r[++i] = e8;
            return r;
        }
        /// <summary>获取 <typeparamref name="T"/>[]，并填入指定元素</summary>
        public T[] GetArrayE<T>(T e1, T e2, T e3, T e4, T e5, T e6, T e7, T e8, T e9)
        {
            var r = GetArray<T>(9);
            int i = -1;
            r[++i] = e1;
            r[++i] = e2;
            r[++i] = e3;
            r[++i] = e4;
            r[++i] = e5;
            r[++i] = e6;
            r[++i] = e7;
            r[++i] = e8;
            r[++i] = e9;
            return r;
        }
        /// <summary>获取 <typeparamref name="T"/>[]，并填入指定元素</summary>
        public T[] GetArrayE<T>(T e1, T e2, T e3, T e4, T e5, T e6, T e7, T e8, T e9, T e10)
        {
            var r = GetArray<T>(10);
            int i = -1;
            r[++i] = e1;
            r[++i] = e2;
            r[++i] = e3;
            r[++i] = e4;
            r[++i] = e5;
            r[++i] = e6;
            r[++i] = e7;
            r[++i] = e8;
            r[++i] = e9;
            r[++i] = e10;
            return r;
        }
        /// <summary>获取 <typeparamref name="T"/>[]，并填入指定元素</summary>
        public T[] GetArrayE<T>(T e1, T e2, T e3, T e4, T e5, T e6, T e7, T e8, T e9, T e10, T e11)
        {
            var r = GetArray<T>(11);
            int i = -1;
            r[++i] = e1;
            r[++i] = e2;
            r[++i] = e3;
            r[++i] = e4;
            r[++i] = e5;
            r[++i] = e6;
            r[++i] = e7;
            r[++i] = e8;
            r[++i] = e9;
            r[++i] = e10;
            r[++i] = e11;
            return r;
        }
        /// <summary>获取 <typeparamref name="T"/>[]，并填入指定元素</summary>
        public T[] GetArrayE<T>(T e1, T e2, T e3, T e4, T e5, T e6, T e7, T e8, T e9, T e10, T e11, T e12)
        {
            var r = GetArray<T>(12);
            int i = -1;
            r[++i] = e1;
            r[++i] = e2;
            r[++i] = e3;
            r[++i] = e4;
            r[++i] = e5;
            r[++i] = e6;
            r[++i] = e7;
            r[++i] = e8;
            r[++i] = e9;
            r[++i] = e10;
            r[++i] = e11;
            r[++i] = e12;
            return r;
        }
        /// <summary>获取 <typeparamref name="T"/>[]，并填入指定元素</summary>
        public T[] GetArrayE<T>(T e1, T e2, T e3, T e4, T e5, T e6, T e7, T e8, T e9, T e10, T e11, T e12, T e13)
        {
            var r = GetArray<T>(13);
            int i = -1;
            r[++i] = e1;
            r[++i] = e2;
            r[++i] = e3;
            r[++i] = e4;
            r[++i] = e5;
            r[++i] = e6;
            r[++i] = e7;
            r[++i] = e8;
            r[++i] = e9;
            r[++i] = e10;
            r[++i] = e11;
            r[++i] = e12;
            r[++i] = e13;
            return r;
        }
        /// <summary>获取 <typeparamref name="T"/>[]，并填入指定元素</summary>
        public T[] GetArrayE<T>(T e1, T e2, T e3, T e4, T e5, T e6, T e7, T e8, T e9, T e10, T e11, T e12, T e13, T e14)
        {
            var r = GetArray<T>(14);
            int i = -1;
            r[++i] = e1;
            r[++i] = e2;
            r[++i] = e3;
            r[++i] = e4;
            r[++i] = e5;
            r[++i] = e6;
            r[++i] = e7;
            r[++i] = e8;
            r[++i] = e9;
            r[++i] = e10;
            r[++i] = e11;
            r[++i] = e12;
            r[++i] = e13;
            r[++i] = e14;
            return r;
        }
        /// <summary>获取 <typeparamref name="T"/>[]，并填入指定元素</summary>
        public T[] GetArrayE<T>(T e1, T e2, T e3, T e4, T e5, T e6, T e7, T e8, T e9, T e10, T e11, T e12, T e13, T e14, T e15)
        {
            var r = GetArray<T>(15);
            int i = -1;
            r[++i] = e1;
            r[++i] = e2;
            r[++i] = e3;
            r[++i] = e4;
            r[++i] = e5;
            r[++i] = e6;
            r[++i] = e7;
            r[++i] = e8;
            r[++i] = e9;
            r[++i] = e10;
            r[++i] = e11;
            r[++i] = e12;
            r[++i] = e13;
            r[++i] = e14;
            r[++i] = e15;
            return r;
        }
        /// <summary>获取 <typeparamref name="T"/>[]，并填入指定元素</summary>
        public T[] GetArrayE<T>(T e1, T e2, T e3, T e4, T e5, T e6, T e7, T e8, T e9, T e10, T e11, T e12, T e13, T e14, T e15, T e16)
        {
            var r = GetArray<T>(16);
            int i = -1;
            r[++i] = e1;
            r[++i] = e2;
            r[++i] = e3;
            r[++i] = e4;
            r[++i] = e5;
            r[++i] = e6;
            r[++i] = e7;
            r[++i] = e8;
            r[++i] = e9;
            r[++i] = e10;
            r[++i] = e11;
            r[++i] = e12;
            r[++i] = e13;
            r[++i] = e14;
            r[++i] = e15;
            r[++i] = e16;
            return r;
        }
        /// <summary>获取 <typeparamref name="T"/>[]，并填入指定元素</summary>
        public T[] GetArrayE<T>(T e1, T e2, T e3, T e4, T e5, T e6, T e7, T e8, T e9, T e10, T e11, T e12, T e13, T e14, T e15, T e16, T e17)
        {
            var r = GetArray<T>(17);
            int i = -1;
            r[++i] = e1;
            r[++i] = e2;
            r[++i] = e3;
            r[++i] = e4;
            r[++i] = e5;
            r[++i] = e6;
            r[++i] = e7;
            r[++i] = e8;
            r[++i] = e9;
            r[++i] = e10;
            r[++i] = e11;
            r[++i] = e12;
            r[++i] = e13;
            r[++i] = e14;
            r[++i] = e15;
            r[++i] = e16;
            r[++i] = e17;
            return r;
        }
        /// <summary>获取 <typeparamref name="T"/>[]，并填入指定元素</summary>
        public T[] GetArrayE<T>(T e1, T e2, T e3, T e4, T e5, T e6, T e7, T e8, T e9, T e10, T e11, T e12, T e13, T e14, T e15, T e16, T e17, T e18)
        {
            var r = GetArray<T>(18);
            int i = -1;
            r[++i] = e1;
            r[++i] = e2;
            r[++i] = e3;
            r[++i] = e4;
            r[++i] = e5;
            r[++i] = e6;
            r[++i] = e7;
            r[++i] = e8;
            r[++i] = e9;
            r[++i] = e10;
            r[++i] = e11;
            r[++i] = e12;
            r[++i] = e13;
            r[++i] = e14;
            r[++i] = e15;
            r[++i] = e16;
            r[++i] = e17;
            r[++i] = e18;
            return r;
        }
        /// <summary>获取 <typeparamref name="T"/>[]，并填入指定元素</summary>
        public T[] GetArrayE<T>(T e1, T e2, T e3, T e4, T e5, T e6, T e7, T e8, T e9, T e10, T e11, T e12, T e13, T e14, T e15, T e16, T e17, T e18, T e19)
        {
            var r = GetArray<T>(19);
            int i = -1;
            r[++i] = e1;
            r[++i] = e2;
            r[++i] = e3;
            r[++i] = e4;
            r[++i] = e5;
            r[++i] = e6;
            r[++i] = e7;
            r[++i] = e8;
            r[++i] = e9;
            r[++i] = e10;
            r[++i] = e11;
            r[++i] = e12;
            r[++i] = e13;
            r[++i] = e14;
            r[++i] = e15;
            r[++i] = e16;
            r[++i] = e17;
            r[++i] = e18;
            r[++i] = e19;
            return r;
        }
        /// <summary>获取 <typeparamref name="T"/>[]，并填入指定元素</summary>
        public T[] GetArrayE<T>(T e1, T e2, T e3, T e4, T e5, T e6, T e7, T e8, T e9, T e10, T e11, T e12, T e13, T e14, T e15, T e16, T e17, T e18, T e19, T e20)
        {
            var r = GetArray<T>(20);
            int i = -1;
            r[++i] = e1;
            r[++i] = e2;
            r[++i] = e3;
            r[++i] = e4;
            r[++i] = e5;
            r[++i] = e6;
            r[++i] = e7;
            r[++i] = e8;
            r[++i] = e9;
            r[++i] = e10;
            r[++i] = e11;
            r[++i] = e12;
            r[++i] = e13;
            r[++i] = e14;
            r[++i] = e15;
            r[++i] = e16;
            r[++i] = e17;
            r[++i] = e18;
            r[++i] = e19;
            r[++i] = e20;
            return r;
        }

        /// <summary>获取 <see cref="Array"/>，并填入指定元素</summary>
        public Array GetArrayE(Type elementType, object e1)
        {
            var r = GetArray(elementType, 1);
            int i = -1;
            r.SetValue(e1, ++i);
            return r;
        }
        /// <summary>获取 <see cref="Array"/>，并填入指定元素</summary>
        public Array GetArrayE(Type elementType, object e1, object e2)
        {
            var r = GetArray(elementType, 2);
            int i = -1;
            r.SetValue(e1, ++i);
            r.SetValue(e2, ++i);
            return r;
        }
        /// <summary>获取 <see cref="Array"/>，并填入指定元素</summary>
        public Array GetArrayE(Type elementType, object e1, object e2, object e3)
        {
            var r = GetArray(elementType, 3);
            int i = -1;
            r.SetValue(e1, ++i);
            r.SetValue(e2, ++i);
            r.SetValue(e3, ++i);
            return r;
        }
        /// <summary>获取 <see cref="Array"/>，并填入指定元素</summary>
        public Array GetArrayE(Type elementType, object e1, object e2, object e3, object e4)
        {
            var r = GetArray(elementType, 4);
            int i = -1;
            r.SetValue(e1, ++i);
            r.SetValue(e2, ++i);
            r.SetValue(e3, ++i);
            r.SetValue(e4, ++i);
            return r;
        }
        /// <summary>获取 <see cref="Array"/>，并填入指定元素</summary>
        public Array GetArrayE(Type elementType, object e1, object e2, object e3, object e4, object e5)
        {
            var r = GetArray(elementType, 5);
            int i = -1;
            r.SetValue(e1, ++i);
            r.SetValue(e2, ++i);
            r.SetValue(e3, ++i);
            r.SetValue(e4, ++i);
            r.SetValue(e5, ++i);
            return r;
        }
        /// <summary>获取 <see cref="Array"/>，并填入指定元素</summary>
        public Array GetArrayE(Type elementType, object e1, object e2, object e3, object e4, object e5, object e6)
        {
            var r = GetArray(elementType, 6);
            int i = -1;
            r.SetValue(e1, ++i);
            r.SetValue(e2, ++i);
            r.SetValue(e3, ++i);
            r.SetValue(e4, ++i);
            r.SetValue(e5, ++i);
            r.SetValue(e6, ++i);
            return r;
        }
        /// <summary>获取 <see cref="Array"/>，并填入指定元素</summary>
        public Array GetArrayE(Type elementType, object e1, object e2, object e3, object e4, object e5, object e6, object e7)
        {
            var r = GetArray(elementType, 7);
            int i = -1;
            r.SetValue(e1, ++i);
            r.SetValue(e2, ++i);
            r.SetValue(e3, ++i);
            r.SetValue(e4, ++i);
            r.SetValue(e5, ++i);
            r.SetValue(e6, ++i);
            r.SetValue(e7, ++i);
            return r;
        }
        /// <summary>获取 <see cref="Array"/>，并填入指定元素</summary>
        public Array GetArrayE(Type elementType, object e1, object e2, object e3, object e4, object e5, object e6, object e7, object e8)
        {
            var r = GetArray(elementType, 8);
            int i = -1;
            r.SetValue(e1, ++i);
            r.SetValue(e2, ++i);
            r.SetValue(e3, ++i);
            r.SetValue(e4, ++i);
            r.SetValue(e5, ++i);
            r.SetValue(e6, ++i);
            r.SetValue(e7, ++i);
            r.SetValue(e8, ++i);
            return r;
        }
        /// <summary>获取 <see cref="Array"/>，并填入指定元素</summary>
        public Array GetArrayE(Type elementType, object e1, object e2, object e3, object e4, object e5, object e6, object e7, object e8, object e9)
        {
            var r = GetArray(elementType, 9);
            int i = -1;
            r.SetValue(e1, ++i);
            r.SetValue(e2, ++i);
            r.SetValue(e3, ++i);
            r.SetValue(e4, ++i);
            r.SetValue(e5, ++i);
            r.SetValue(e6, ++i);
            r.SetValue(e7, ++i);
            r.SetValue(e8, ++i);
            r.SetValue(e9, ++i);
            return r;
        }
        /// <summary>获取 <see cref="Array"/>，并填入指定元素</summary>
        public Array GetArrayE(Type elementType, object e1, object e2, object e3, object e4, object e5, object e6, object e7, object e8, object e9, object e10)
        {
            var r = GetArray(elementType, 10);
            int i = -1;
            r.SetValue(e1, ++i);
            r.SetValue(e2, ++i);
            r.SetValue(e3, ++i);
            r.SetValue(e4, ++i);
            r.SetValue(e5, ++i);
            r.SetValue(e6, ++i);
            r.SetValue(e7, ++i);
            r.SetValue(e8, ++i);
            r.SetValue(e9, ++i);
            r.SetValue(e10, ++i);
            return r;
        }
        /// <summary>获取 <see cref="Array"/>，并填入指定元素</summary>
        public Array GetArrayE(Type elementType, object e1, object e2, object e3, object e4, object e5, object e6, object e7, object e8, object e9, object e10, object e11)
        {
            var r = GetArray(elementType, 11);
            int i = -1;
            r.SetValue(e1, ++i);
            r.SetValue(e2, ++i);
            r.SetValue(e3, ++i);
            r.SetValue(e4, ++i);
            r.SetValue(e5, ++i);
            r.SetValue(e6, ++i);
            r.SetValue(e7, ++i);
            r.SetValue(e8, ++i);
            r.SetValue(e9, ++i);
            r.SetValue(e10, ++i);
            r.SetValue(e11, ++i);
            return r;
        }
        /// <summary>获取 <see cref="Array"/>，并填入指定元素</summary>
        public Array GetArrayE(Type elementType, object e1, object e2, object e3, object e4, object e5, object e6, object e7, object e8, object e9, object e10, object e11, object e12)
        {
            var r = GetArray(elementType, 12);
            int i = -1;
            r.SetValue(e1, ++i);
            r.SetValue(e2, ++i);
            r.SetValue(e3, ++i);
            r.SetValue(e4, ++i);
            r.SetValue(e5, ++i);
            r.SetValue(e6, ++i);
            r.SetValue(e7, ++i);
            r.SetValue(e8, ++i);
            r.SetValue(e9, ++i);
            r.SetValue(e10, ++i);
            r.SetValue(e11, ++i);
            r.SetValue(e12, ++i);
            return r;
        }
        /// <summary>获取 <see cref="Array"/>，并填入指定元素</summary>
        public Array GetArrayE(Type elementType, object e1, object e2, object e3, object e4, object e5, object e6, object e7, object e8, object e9, object e10, object e11, object e12, object e13)
        {
            var r = GetArray(elementType, 13);
            int i = -1;
            r.SetValue(e1, ++i);
            r.SetValue(e2, ++i);
            r.SetValue(e3, ++i);
            r.SetValue(e4, ++i);
            r.SetValue(e5, ++i);
            r.SetValue(e6, ++i);
            r.SetValue(e7, ++i);
            r.SetValue(e8, ++i);
            r.SetValue(e9, ++i);
            r.SetValue(e10, ++i);
            r.SetValue(e11, ++i);
            r.SetValue(e12, ++i);
            r.SetValue(e13, ++i);
            return r;
        }
        /// <summary>获取 <see cref="Array"/>，并填入指定元素</summary>
        public Array GetArrayE(Type elementType, object e1, object e2, object e3, object e4, object e5, object e6, object e7, object e8, object e9, object e10, object e11, object e12, object e13, object e14)
        {
            var r = GetArray(elementType, 14);
            int i = -1;
            r.SetValue(e1, ++i);
            r.SetValue(e2, ++i);
            r.SetValue(e3, ++i);
            r.SetValue(e4, ++i);
            r.SetValue(e5, ++i);
            r.SetValue(e6, ++i);
            r.SetValue(e7, ++i);
            r.SetValue(e8, ++i);
            r.SetValue(e9, ++i);
            r.SetValue(e10, ++i);
            r.SetValue(e11, ++i);
            r.SetValue(e12, ++i);
            r.SetValue(e13, ++i);
            r.SetValue(e14, ++i);
            return r;
        }
        /// <summary>获取 <see cref="Array"/>，并填入指定元素</summary>
        public Array GetArrayE(Type elementType, object e1, object e2, object e3, object e4, object e5, object e6, object e7, object e8, object e9, object e10, object e11, object e12, object e13, object e14, object e15)
        {
            var r = GetArray(elementType, 15);
            int i = -1;
            r.SetValue(e1, ++i);
            r.SetValue(e2, ++i);
            r.SetValue(e3, ++i);
            r.SetValue(e4, ++i);
            r.SetValue(e5, ++i);
            r.SetValue(e6, ++i);
            r.SetValue(e7, ++i);
            r.SetValue(e8, ++i);
            r.SetValue(e9, ++i);
            r.SetValue(e10, ++i);
            r.SetValue(e11, ++i);
            r.SetValue(e12, ++i);
            r.SetValue(e13, ++i);
            r.SetValue(e14, ++i);
            r.SetValue(e15, ++i);
            return r;
        }
        /// <summary>获取 <see cref="Array"/>，并填入指定元素</summary>
        public Array GetArrayE(Type elementType, object e1, object e2, object e3, object e4, object e5, object e6, object e7, object e8, object e9, object e10, object e11, object e12, object e13, object e14, object e15, object e16)
        {
            var r = GetArray(elementType, 16);
            int i = -1;
            r.SetValue(e1, ++i);
            r.SetValue(e2, ++i);
            r.SetValue(e3, ++i);
            r.SetValue(e4, ++i);
            r.SetValue(e5, ++i);
            r.SetValue(e6, ++i);
            r.SetValue(e7, ++i);
            r.SetValue(e8, ++i);
            r.SetValue(e9, ++i);
            r.SetValue(e10, ++i);
            r.SetValue(e11, ++i);
            r.SetValue(e12, ++i);
            r.SetValue(e13, ++i);
            r.SetValue(e14, ++i);
            r.SetValue(e15, ++i);
            r.SetValue(e16, ++i);
            return r;
        }
        /// <summary>获取 <see cref="Array"/>，并填入指定元素</summary>
        public Array GetArrayE(Type elementType, object e1, object e2, object e3, object e4, object e5, object e6, object e7, object e8, object e9, object e10, object e11, object e12, object e13, object e14, object e15, object e16, object e17)
        {
            var r = GetArray(elementType, 17);
            int i = -1;
            r.SetValue(e1, ++i);
            r.SetValue(e2, ++i);
            r.SetValue(e3, ++i);
            r.SetValue(e4, ++i);
            r.SetValue(e5, ++i);
            r.SetValue(e6, ++i);
            r.SetValue(e7, ++i);
            r.SetValue(e8, ++i);
            r.SetValue(e9, ++i);
            r.SetValue(e10, ++i);
            r.SetValue(e11, ++i);
            r.SetValue(e12, ++i);
            r.SetValue(e13, ++i);
            r.SetValue(e14, ++i);
            r.SetValue(e15, ++i);
            r.SetValue(e16, ++i);
            r.SetValue(e17, ++i);
            return r;
        }
        /// <summary>获取 <see cref="Array"/>，并填入指定元素</summary>
        public Array GetArrayE(Type elementType, object e1, object e2, object e3, object e4, object e5, object e6, object e7, object e8, object e9, object e10, object e11, object e12, object e13, object e14, object e15, object e16, object e17, object e18)
        {
            var r = GetArray(elementType, 18);
            int i = -1;
            r.SetValue(e1, ++i);
            r.SetValue(e2, ++i);
            r.SetValue(e3, ++i);
            r.SetValue(e4, ++i);
            r.SetValue(e5, ++i);
            r.SetValue(e6, ++i);
            r.SetValue(e7, ++i);
            r.SetValue(e8, ++i);
            r.SetValue(e9, ++i);
            r.SetValue(e10, ++i);
            r.SetValue(e11, ++i);
            r.SetValue(e12, ++i);
            r.SetValue(e13, ++i);
            r.SetValue(e14, ++i);
            r.SetValue(e15, ++i);
            r.SetValue(e16, ++i);
            r.SetValue(e17, ++i);
            r.SetValue(e18, ++i);
            return r;
        }
        /// <summary>获取 <see cref="Array"/>，并填入指定元素</summary>
        public Array GetArrayE(Type elementType, object e1, object e2, object e3, object e4, object e5, object e6, object e7, object e8, object e9, object e10, object e11, object e12, object e13, object e14, object e15, object e16, object e17, object e18, object e19)
        {
            var r = GetArray(elementType, 19);
            int i = -1;
            r.SetValue(e1, ++i);
            r.SetValue(e2, ++i);
            r.SetValue(e3, ++i);
            r.SetValue(e4, ++i);
            r.SetValue(e5, ++i);
            r.SetValue(e6, ++i);
            r.SetValue(e7, ++i);
            r.SetValue(e8, ++i);
            r.SetValue(e9, ++i);
            r.SetValue(e10, ++i);
            r.SetValue(e11, ++i);
            r.SetValue(e12, ++i);
            r.SetValue(e13, ++i);
            r.SetValue(e14, ++i);
            r.SetValue(e15, ++i);
            r.SetValue(e16, ++i);
            r.SetValue(e17, ++i);
            r.SetValue(e18, ++i);
            r.SetValue(e19, ++i);
            return r;
        }
        /// <summary>获取 <see cref="Array"/>，并填入指定元素</summary>
        public Array GetArrayE(Type elementType, object e1, object e2, object e3, object e4, object e5, object e6, object e7, object e8, object e9, object e10, object e11, object e12, object e13, object e14, object e15, object e16, object e17, object e18, object e19, object e20)
        {
            var r = GetArray(elementType, 20);
            int i = -1;
            r.SetValue(e1, ++i);
            r.SetValue(e2, ++i);
            r.SetValue(e3, ++i);
            r.SetValue(e4, ++i);
            r.SetValue(e5, ++i);
            r.SetValue(e6, ++i);
            r.SetValue(e7, ++i);
            r.SetValue(e8, ++i);
            r.SetValue(e9, ++i);
            r.SetValue(e10, ++i);
            r.SetValue(e11, ++i);
            r.SetValue(e12, ++i);
            r.SetValue(e13, ++i);
            r.SetValue(e14, ++i);
            r.SetValue(e15, ++i);
            r.SetValue(e16, ++i);
            r.SetValue(e17, ++i);
            r.SetValue(e18, ++i);
            r.SetValue(e19, ++i);
            r.SetValue(e20, ++i);
            return r;
        }
        #endregion

        /// <summary>返回对象池</summary>
        public virtual void Return<T>(T obj) where T : class
        {
            if (obj == null) return;

            /* 
            这里避坑，
            要用实例 obj.GetType() 来获取类型，
            而不能用 typeof(T) 来获取类型。

            原因是：如果池中同时存有父子类，
                而 obj 传入的是子类，
                 T 却很有可能是父类，我这里遇到的情况是 T 必然是父类型，
                这时候获取到的类型就不一样，会把子类误存到父类的池子里，
                这样在下次取得时候，从父类池子里取到了一个子类对象，这必然是错误的。
            */
            //var target = typeof(T);
            var target = obj.GetType();
            var has = _pool.TryGetValue(target, out var tPool);

            if (!has)
            {
                tPool = CreatePool();
                _pool[target] = tPool;
            }

            if (!tPool.Contains(obj))
            {
                //tPool.Enqueue(obj);
                tPool.Add(obj);

                CleanupObject(obj);
            }
        }

        // 以下处理基本容器类型
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
        public void Return(ArrayList v)
        {
            if (v == null) return;
            v.Clear();
            Return<ArrayList>(v);
        }
        /// <summary>返回对象池</summary>
        /// <remarks>会清空</remarks>
        public void Return<TKey, TValue>(Dictionary<TKey, TValue> v)
        {
            if (v == null) return;
            v.Clear();
            Return<Dictionary<TKey, TValue>>(v);
        }
        /// <summary>返回对象池</summary>
        /// <remarks>会将元素置为默认值</remarks>
        public void Return<T>(T[] v)
        {
            if (v == null) return;
            for (var i = 0; i < v.Length; i++) v[i] = default;
            Return<T[]>(v);
        }
        /// <summary>返回对象池，建议使用 <see cref="Return{T}(T[])"/></summary>
        /// <remarks>会将元素置为默认值</remarks>
        public void Return(Array v)
        {
            if (v == null) return;
            var t = v.GetType();
            var et = t.GetElementType();
            bool isValueType = et.IsValueType;
            for (var i = 0; i < v.Length; i++)
            {
                if (isValueType)
                {
                    v.SetValue(CreateInstance(et), i);
                }
                else
                {
                    v.SetValue(default, i);
                }
            }
            Return<Array>(v);
        }
        /// <summary>返回对象池</summary>
        /// <remarks>会清空</remarks>
        public void Return<T>(Queue<T> v)
        {
            if (v == null) return;
            v.Clear();
            Return<Queue<T>>(v);
        }
        /// <summary>返回对象池</summary>
        /// <remarks>会清空</remarks>
        public void Return(Queue v)
        {
            if (v == null) return;
            v.Clear();
            Return<Queue>(v);
        }
        /// <summary>返回对象池</summary>
        /// <remarks>会清空</remarks>
        public void Return<T>(Stack<T> v)
        {
            if (v == null) return;
            v.Clear();
            Return<Stack<T>>(v);
        }
        /// <summary>返回对象池</summary>
        /// <remarks>会清空</remarks>
        public void Return(Stack v)
        {
            if (v == null) return;
            v.Clear();
            Return<Stack>(v);
        }
        /// <summary>返回对象池</summary>
        /// <remarks>会清空</remarks>
        public void Return<T>(HashSet<T> v)
        {
            if (v == null) return;
            v.Clear();
            Return<HashSet<T>>(v);
        }
        /// <summary>返回对象池</summary>
        /// <remarks>会清空</remarks>
        public void Return(Hashtable v)
        {
            if (v == null) return;
            v.Clear();
            Return<Hashtable>(v);
        }
        /// <summary>返回对象池</summary>
        /// <remarks>会清空</remarks>
        public void Return<T>(LinkedList<T> v)
        {
            if (v == null) return;
            v.Clear();
            Return<LinkedList<T>>(v);
        }
        /// <summary>返回对象池</summary>
        /// <remarks>会清空</remarks>
        public void Return(StringBuilder v)
        {
            if (v == null) return;
            v.Clear();
            Return<StringBuilder>(v);
        }

        /// <summary>返回对象池</summary>
        /// <remarks>会清空</remarks>
        public void Return(IList v)
        {
            if (v == null) return;
            v.Clear();
            Return<IList>(v);
        }
        /// <summary>返回对象池</summary>
        /// <remarks>会清空</remarks>
        public void Return(IDictionary v)
        {
            if (v == null) return;
            v.Clear();
            Return<IDictionary>(v);
        }
        /// <summary>返回对象池</summary>
        /// <remarks>会清空</remarks>
        public void Return<T>(ICollection<T> v)
        {
            if (v == null) return;
            v.Clear();
            Return<ICollection<T>>(v);
        }


        /// <summary>取出最后一个元素，避免中间的元素挪动影响性能，取出的元素会被移除</summary>
        protected object Fetch(List<object> tPool)
        {
            if (tPool.Count <= 0) return null;
            return Fetch(tPool, tPool.Count - 1);
        }
        /// <summary>取出指定索引处元素，取出的元素会被移除</summary>
        protected object Fetch(List<object> tPool, int index)
        {
            if (tPool.Count <= 0) return null;
            var _obj = tPool[index];
            tPool.RemoveAt(index);
            return _obj;
        }
        protected List<object> CreatePool()
        {
            return new List<object>(5);
        }

        /// <summary>
        /// 清理 <see cref="ITypePoolObject.Clear()"/>
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void CleanupObject(object obj)
        {
            if (obj is ITypePoolObject tpo) tpo.Clear();
        }
        /// <summary>
        /// 初始 <see cref="ITypePoolObjectInit.Init()"/>
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void InitializeObject(object obj)
        {
            if (obj is ITypePoolObjectInit tpo) tpo.Init();
        }

        /// <summary>清除对象池</summary>
        public virtual void Clear()
        {
            _pool.Clear();
        }
    }

}