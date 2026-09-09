// -------------------------
// 创建日期：2023/10/19 1:41:25
// -------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;

namespace Framework
{
    /// <summary>
    /// 类型池
    /// </summary>
    [Serializable]
    public class TypePool
    {
        internal static List<TypePool> _pools = new List<TypePool>();

        /// <summary>公共池，不管理自己的对象池时使用</summary>
        public static TypePool root { get; } = new TypePool();

        /// <summary>清理所有 <see cref="TypePool"/> 对象池</summary>
        public static void ClearAllPool()
        {
            foreach (var pool in _pools)
            {
                pool.Clear();
            }
        }

        protected Dictionary<Type, List<object>> _pool;
        protected ArrayPool _arrayPool;

        protected Type[] _tempTypes1 = new Type[1], _tempTypes2 = new Type[2];

        public TypePool() : this(1)
        {
        }

        public TypePool(int capacity)
        {
            _pool = new Dictionary<Type, List<object>>(capacity);
            _arrayPool = new ArrayPool();

            _pools.Add(this);
        }

        public Dictionary<Type, List<object>> pool => _pool;

        /// <summary>数组池</summary>
        public ArrayPool arrayPool => _arrayPool;

        /// <summary>池子数量，每个类型对应一个池子（含数组类型）</summary>
        public virtual int poolCount => _pool.Count + _arrayPool.poolCount;

        /// <summary>池子里的对象数量（含数组）</summary>
        public virtual int itemCount
        {
            get
            {
                int sum = _arrayPool.itemSize;
                foreach (var item in _pool)
                {
                    sum += item.Value.Count;
                }
                return sum;
            }
        }

        /// <summary>创建实例</summary>
        protected T CreateInstance<T>() => Activator.CreateInstance<T>();

        /// <summary>创建实例</summary>
        protected object CreateInstance(Type type) => Activator.CreateInstance(type);

        /// <summary>创建实例</summary>
        protected object CreateInstance(Type type, params object[] args) => Activator.CreateInstance(type, args);

        /// <summary>从对象池获取
        /// <para>注意：要获取 <see cref="Array"/> 请使用 <see cref="GetArray{T}(int)"/></para>
        /// </summary>
        public T Get<T>()
        {
            var target = typeof(T);
            T obj = (T)Get(target, null);
            return obj;
        }

        /// <summary>从对象池获取
        /// <para><paramref name="ctorArgs"/>：仅用于创建对象实例时，向构造函数传递的参数</para>
        /// <para>注意：要获取 <see cref="Array"/> 请使用 <see cref="GetArray{T}(int)"/></para>
        /// </summary>
        public T Get<T>(params object[] ctorArgs)
        {
            var target = typeof(T);
            T obj = (T)Get(target, ctorArgs);
            return obj;
        }

        /// <summary>从对象池获取
        /// <para>注意：要获取 <see cref="Array"/> 请使用 <see cref="GetArray(Type, int)"/></para>
        /// </summary>
        public virtual object Get(Type type)
        {
            TryGet(type, out var obj, null);
            return obj;
        }

        /// <summary>从对象池获取
        /// <para><paramref name="ctorArgs"/>：仅用于创建对象实例时，向构造函数传递的参数</para>
        /// <para>注意：要获取 <see cref="Array"/> 请使用 <see cref="GetArray(Type, int)"/></para>
        /// </summary>
        public virtual object Get(Type type, params object[] ctorArgs)
        {
            TryGet(type, out var obj, ctorArgs);
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
        public T[] GetArray<T>(int length) => GetArray(typeof(T), length) as T[];

        /// <summary>获取 <see cref="Array"/></summary>
        public Array GetArray(Type elementType, int length)
        {
            if (elementType == null) throw new ArgumentNullException(nameof(elementType), "元素类型为空");
            if (length < 0) throw new ArgumentOutOfRangeException(nameof(length), "数组长度小于 0");

            TryGetArray(elementType, length, out var obj);
            return obj;
        }

        /// <summary>从对象池获取
        /// <para><paramref name="ctorArgs"/>：仅用于创建对象实例时，向构造函数传递的参数</para>
        /// <para>注意：要获取 <see cref="Array"/> 请使用 <see cref="TryGetArray{T}(int, out T[])"/></para>
        /// </summary>
        public bool TryGet<T>(out T obj)
        {
            var r = TryGet(typeof(T), out var o, null);
            obj = (T)o;
            return r;
        }

        /// <summary>从对象池获取
        /// <para><paramref name="ctorArgs"/>：仅用于创建对象实例时，向构造函数传递的参数</para>
        /// <para>注意：要获取 <see cref="Array"/> 请使用 <see cref="TryGetArray{T}(int, out T[])"/></para>
        /// </summary>
        public bool TryGet<T>(out T obj, params object[] ctorArgs)
        {
            var r = TryGet(typeof(T), out var o, ctorArgs);
            obj = (T)o;
            return r;
        }

        /// <summary>从对象池获取
        /// <para>注意：要获取 <see cref="Array"/> 请使用 <see cref="TryGetArray(Type, int, out Array)"/></para>
        /// </summary>
        public bool TryGet(Type type, out object obj)
        {
            return TryGet(type, out obj, null);
        }

        /// <summary>从对象池获取
        /// <para><paramref name="ctorArgs"/>：仅用于创建对象实例时，向构造函数传递的参数</para>
        /// <para>注意：要获取 <see cref="Array"/> 请使用 <see cref="TryGetArray(Type, int, out Array)"/></para>
        /// </summary>
        public virtual bool TryGet(Type type, out object obj, params object[] ctorArgs)
        {
            obj = null;
            if (type == null) return false;
            // 数组必须指定长度，请使用 GetArray / TryGetArray
            if (type.IsArray) return false;

            var target = type;
            var has = _pool.TryGetValue(target, out var tPool);

            // TODO：需判断是否引用类型

            if (has)
            {
                if (tPool.Count > 0)
                {
                    obj = Fetch(tPool);
                }
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

            return true;
        }

        /// <summary>从对象池获取</summary>
        public bool TryGetArray<T>(int length, out T[] obj)
        {
            var r = TryGetArray(typeof(T), length, out var a);
            obj = a as T[];
            return r;
        }

        /// <summary>从对象池获取</summary>
        public bool TryGetArray(int length, out object[] obj)
        {
            var r = TryGetArray(typeof(object), length, out var a);
            obj = a as object[];
            return r;
        }

        /// <summary>从对象池获取</summary>
        public virtual bool TryGetArray(Type elementType, int length, out Array a)
        {
            return _arrayPool.TryGetArray(elementType, length, out a);
        }

        #region 转换成数组
        /// <summary>转换成可重复利用的 <typeparamref name="T"/>[] 数组</summary>
        public T[] ToArray<T>(ICollection<T> v) => _arrayPool.ToArray(v);

        /// <summary>转换成可重复利用的数组
        /// <para><typeparamref name="T"/>：要转换的目标数组元素类型</para>
        /// </summary>
        public T[] ToArray<T, S>(ICollection<S> v) => _arrayPool.ToArray<T, S>(v);

        /// <summary>转换成可重复利用的 <typeparamref name="T"/>[] 数组，自行处理元素的转换结果</summary>
        public T[] ToArray<T>(ICollection<T> v, Func<T, T> handle) => _arrayPool.ToArray<T>(v, handle);

        /// <summary>转换成可重复利用的数组，自行处理元素的转换结果
        /// <para><typeparamref name="T"/>：要转换的目标数组元素类型</para>
        /// </summary>
        public T[] ToArray<T, S>(ICollection<S> v, Func<S, T> handle) => _arrayPool.ToArray(v, handle);

        #endregion


        #region 返回对象池

        /// <summary>返回对象池</summary>
        /// <remarks>对于 <see cref="ITypePoolObject"/> 对象会做清理工作</remarks>
        public virtual void Return<T>(T obj) where T : class
        {
            if (obj == null) return;
            if (TryReturnArray(obj)) return;

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

        /// <summary>返回对象池</summary>
        /// <remarks>对于 <see cref="ITypePoolObject"/> 对象会做清理工作</remarks>
        public virtual void Return(object obj)
        {
            if (obj == null) return;
            if (TryReturnArray(obj)) return;

            var target = obj.GetType();
            if (target.IsValueType) return;

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

        /// <summary>若是数组则返回数组池</summary>
        protected bool TryReturnArray(object obj)
        {
            if (obj is Array arr)
            {
                _arrayPool.Return(arr);
                return true;
            }
            return false;
        }

        #region 不同对象返回对象池的处理
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
            _arrayPool.Return(v);
        }
        /// <summary>返回对象池，建议使用 <see cref="Return{T}(T[])"/></summary>
        /// <remarks>会将元素置为默认值</remarks>
        public void Return(Array v)
        {
            _arrayPool.Return(v);
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
            if (TryReturnArray(v)) return;
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
            if (TryReturnArray(v)) return;
            v.Clear();
            Return<ICollection<T>>(v);
        }

        #endregion

        #region 返回元素
        /// <summary>将元素返回对象池
        /// <para>会清空</para>
        /// </summary>
        public void ReturnE<T>(List<T> v) where T : class
        {
            if (v == null) return;
            foreach (var item in v)
            {
                Return(item);
            }
            v.Clear();
        }
        /// <summary>将元素返回对象池
        /// <para>会清空</para>
        /// </summary>
        public void ReturnE(ArrayList v)
        {
            if (v == null) return;
            foreach (var item in v)
            {
                Return(item);
            }
            v.Clear();
        }
        /// <summary>将元素返回对象池
        /// <para>会清空</para>
        /// </summary>
        public void ReturnE<TKey, TValue>(Dictionary<TKey, TValue> v)
        {
            if (v == null) return;
            foreach (var item in v.Keys)
            {
                Return(item);
            }
            foreach (var item in v.Values)
            {
                Return(item);
            }
            v.Clear();
        }
        /// <summary>将元素返回对象池
        /// <para>会将元素置为默认值</para>
        public void ReturnE<T>(T[] v) where T : class
        {
            if (v == null) return;
            foreach (var item in v)
            {
                Return(item);
            }
            Array.Clear(v, 0, v.Length);
        }
        /// <summary>将元素返回对象池，建议使用 <see cref="ReturnE{T}(T[])"/>
        /// <para>会将元素置为默认值</para>
        /// </summary>
        public void ReturnE(Array v)
        {
            if (v == null) return;
            foreach (var item in v)
            {
                Return(item);
            }
            //var t = v.GetType();
            //var et = t.GetElementType();
            //bool isValueType = et.IsValueType;
            //for (var i = 0; i < v.Length; i++)
            //{
            //    if (isValueType)
            //    {
            //        v.SetValue(CreateInstance(et), i);
            //    }
            //    else
            //    {
            //        v.SetValue(default, i);
            //    }
            //}
            Array.Clear(v, 0, v.Length);
        }
        /// <summary>将元素返回对象池
        /// <para>会清空</para>
        /// </summary>
        public void ReturnE<T>(Queue<T> v) where T : class
        {
            if (v == null) return;
            foreach (var item in v)
            {
                Return(item);
            }
            v.Clear();
        }
        /// <summary>将元素返回对象池
        /// <para>会清空</para>
        /// </summary>
        public void ReturnE(Queue v)
        {
            if (v == null) return;
            foreach (var item in v)
            {
                Return(item);
            }
            v.Clear();
        }
        /// <summary>将元素返回对象池
        /// <para>会清空</para>
        /// </summary>
        public void ReturnE<T>(Stack<T> v) where T : class
        {
            if (v == null) return;
            foreach (var item in v)
            {
                Return(item);
            }
            v.Clear();
        }
        /// <summary>将元素返回对象池
        /// <para>会清空</para>
        /// </summary>
        public void ReturnE(Stack v)
        {
            if (v == null) return;
            foreach (var item in v)
            {
                Return(item);
            }
            v.Clear();
        }
        /// <summary>将元素返回对象池
        /// <para>会清空</para>
        /// </summary>
        public void ReturnE<T>(HashSet<T> v) where T : class
        {
            if (v == null) return;
            foreach (var item in v)
            {
                Return(item);
            }
            v.Clear();
        }
        /// <summary>将元素返回对象池
        /// <para>会清空</para>
        /// </summary>
        public void ReturnE(Hashtable v)
        {
            if (v == null) return;
            foreach (var item in v)
            {
                Return(item);
            }
            v.Clear();
        }
        /// <summary>将元素返回对象池
        /// <para>会清空</para>
        /// </summary>
        public void ReturnE<T>(LinkedList<T> v) where T : class
        {
            if (v == null) return;
            foreach (var item in v)
            {
                Return(item);
            }
            v.Clear();
        }

        /// <summary>将元素返回对象池
        /// <para>会清空</para>
        /// </summary>
        public void ReturnE(IList v)
        {
            if (v == null) return;
            foreach (var item in v)
            {
                Return(item);
            }
            v.Clear();
        }

        /// <summary>将元素返回对象池
        /// <para>会清空</para>
        /// </summary>
        public void ReturnE(IDictionary v)
        {
            if (v == null) return;
            foreach (var item in v.Keys)
            {
                Return(item);
            }
            foreach (var item in v.Values)
            {
                Return(item);
            }
            v.Clear();
        }

        /// <summary>将元素返回对象池
        /// <para>会清空</para>
        /// </summary>
        public void ReturnE<T>(ICollection<T> v) where T : class
        {
            if (v == null) return;
            foreach (var item in v)
            {
                Return(item);
            }
            v.Clear();
        }
        #endregion
        #endregion

        /// <summary>获取池中指定类型实例的可用数量</summary>
        public virtual int GetFreeCount<T>()
        {
            return GetFreeCount(typeof(T));
        }

        /// <summary>获取池中指定类型实例的可用数量</summary>
        public virtual int GetFreeCount(Type type)
        {
            if (type == null) return 0;
            if (type.IsArray) return _arrayPool.GetFreeCount(type);

            int count = 0;
            var has = _pool.TryGetValue(type, out var tPool);

            if (has)
            {
                count = tPool.Count;
            }
            return count;
        }

        /// <summary>清除对象池</summary>
        public virtual void Clear()
        {
            foreach (var item in _pool)
            {
                item.Value?.Clear();
            }
            _pool.Clear();
            _arrayPool.Clear();
        }

        /// <summary>销毁</summary>
        public void Destroy()
        {
            Clear();
            _pools.Remove(this);
        }


        /// <summary>取出最后一个元素，避免中间的元素挪动影响性能，取出的元素会被移除</summary>
        protected T Fetch<T>(List<T> tPool)
        {
            if (tPool.Count <= 0) return default;
            return Fetch(tPool, tPool.Count - 1);
        }
        /// <summary>取出指定索引处元素，取出的元素会被移除</summary>
        protected T Fetch<T>(List<T> tPool, int index)
        {
            if (tPool.Count <= 0) return default;
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
    }
}