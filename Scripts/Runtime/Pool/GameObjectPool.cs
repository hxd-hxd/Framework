// -------------------------
// 创建日期：2023/10/19 1:41:25
// -------------------------

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Framework
{
    /// <summary>
    /// <see cref="GameObject"/> 池
    /// </summary>
    [Serializable]
    public partial class GameObjectPool
    {

        internal static List<GameObjectPool> _pools = new List<GameObjectPool>();

        static GameObjectPoolMonitor _monitor;
        static GameObjectPool _root;
        static GameObject _GOManager;
        static bool _isQuitting;

        /// <summary>
        /// 监视器
        /// </summary>
        public static GameObjectPoolMonitor monitor
        {
            get
            {
                InitStatic();
                return _monitor;
            }
        }
        /// <summary>
        /// 公共池，不管理自己的对象池时使用
        /// </summary>
        //public static GameObjectPool root { get; } = new GameObjectPool();
        public static GameObjectPool root
        {
            get
            {
                if (_root == null) _root = new GameObjectPool();
                return _root;
            }
        }
        protected static GameObject GOManager
        {
            get
            {
                InitStatic();
                return _GOManager;
            }
            set { _GOManager = value; }
        }

        //static GameObjectPool()
        //{
        //    InitStatic();
        //}

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            _monitor = null;
            _GOManager = null;
            _root = null;
            _isQuitting = false;

            Application.quitting -= OnApplicationQuitting;
            Application.quitting += OnApplicationQuitting;

#if UNITY_EDITOR
            UnityEditor.EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
        }

        static void OnApplicationQuitting()
        {
            _isQuitting = true;
        }

#if UNITY_EDITOR
        static void OnPlayModeStateChanged(UnityEditor.PlayModeStateChange state)
        {
            // 退出 Play 时 Application.isPlaying 在 OnDestroy 里仍为 true，必须在销毁前禁止再创建物体
            if (state == UnityEditor.PlayModeStateChange.ExitingPlayMode)
                _isQuitting = true;
        }
#endif

        internal static void NotifyQuitting()
        {
            _isQuitting = true;
        }

        internal static void NotifyMonitorDestroyed(GameObjectPoolMonitor monitor)
        {
            if (_monitor == monitor)
            {
                _monitor = null;
                _GOManager = null;
            }
        }

        /// <summary>
        /// 退出 Play / 关场景时不可再 new GameObject，否则会触发
        /// “Some objects were not cleaned up when closing the scene”
        /// </summary>
        static bool CanSpawnRuntimeObjects()
        {
            if (_isQuitting)
                return false;
            if (!Application.isPlaying)
                return false;
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
                return false;
#endif
            return true;
        }

        // 不可以放在静态构造函数里执行，因为 Application.isPlaying 属性在静态构造函数之后更新
        protected static void InitStatic()
        {
            if (!CanSpawnRuntimeObjects())
                return;

            if (_monitor == null)
            {
                try
                {
                    _monitor = new GameObject($"<{nameof(GameObjectPoolMonitor)}>").AddComponent<GameObjectPoolMonitor>();
                    GameObject.DontDestroyOnLoad(_monitor.gameObject);
                }
                catch (Exception)
                {
                    return;
                }
            }
            if (_monitor == null)
                return;

            if (_GOManager == null)
            {
                try
                {
                    _GOManager = new GameObject("<GameObjectPool>");
                    _GOManager.SetActive(false);
                    _GOManager.transform.SetParent(_monitor.transform);

                    if (_GOManager.transform.root == _GOManager.transform)
                        GameObject.DontDestroyOnLoad(_GOManager);
                }
                catch (Exception)
                {
                }
            }

            //Debug.Log($"{nameof(GameObjectPool)} 静态初始化");
        }

        /// <summary>
        /// 清理所有对象池
        /// </summary>
        public static void ClearAllPool()
        {
            foreach (var pool in _pools)
            {
                pool.Clear();
            }
        }


        [SerializeField]
        GameObject _template;
        [SerializeField]
        Transform _returnParent;
        Dictionary<GameObject, List<GameObject>> _pool;
        List<Coroutine> _preCreateInstanceCoroutines = new List<Coroutine>();
        ///// <summary>
        ///// 实例化事件
        ///// </summary>
        //public event Func<GameObject> CreateInstanceEvent;

        public GameObjectPool()
        {
            _pool = new Dictionary<GameObject, List<GameObject>>(1);

            _pools.Add(this);
        }
        public GameObjectPool(int capacity)
        {
            _pool = new Dictionary<GameObject, List<GameObject>>(capacity);

            _pools.Add(this);
        }
        public GameObjectPool(GameObject template)
        {
            _pool = new Dictionary<GameObject, List<GameObject>>(1);
            this._template = template;

            _pools.Add(this);
        }
        public GameObjectPool(int capacity, GameObject template)
        {
            _pool = new Dictionary<GameObject, List<GameObject>>(capacity);
            this._template = template;

            _pools.Add(this);
        }

        /// <summary>
        /// 模板池
        /// </summary>
        public Dictionary<GameObject, List<GameObject>> pool => _pool;
        /// <summary>
        /// 默认模板
        /// </summary>
        public GameObject template { get => _template; set => _template = value; }
        /// <summary>
        /// 池中物体的父节点
        /// </summary>
        public Transform returnParent {
            get
            {
                if (_returnParent == null)
                {
                    var manager = GOManager;
                    return manager != null ? manager.transform : null;
                }
                return _returnParent;
            }
            set => _returnParent = value; 
        }

        /// <summary>
        /// 池子的数量
        /// </summary>
        public virtual int poolCount => _pool.Count;
        /// <summary>
        /// 池子里包含的对象数量
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
                return sum;
            }
        }
        /// <summary>
        /// 预创建的异步协程列表
        /// </summary>
        public List<Coroutine> preCreateInstanceCoroutines { get => _preCreateInstanceCoroutines; set => _preCreateInstanceCoroutines = value; }
        /// <summary>
        /// 预创建实例的协程数量
        /// </summary>
        public int preCreateInstanceCoroutineNum => _preCreateInstanceCoroutines.Count;

        //protected void Init(string name)
        //{
        //    var goRoot = new GameObject($"<>");
        //}

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <returns></returns>
        protected virtual GameObject CreateInstance(GameObject template)
        {
            if (template == null)
            {
                Debug.LogError("[GameObjectPool]：要实例化的目标模板是空对象");
                return null;
            }

            //GameObject resault = CreateInstanceEvent?.Invoke();
            //if(!resault) resault = GameObject.Instantiate(value);
            //return resault;
            return GameObject.Instantiate(template);
        }

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <returns></returns>
        protected virtual GameObject CreateInstance(GameObject template, Transform parent)
        {
            if (template == null)
            {
                Debug.LogError("[GameObjectPool]：要实例化的目标模板是空对象");
                return null;
            }

            //GameObject resault = CreateInstanceEvent?.Invoke();
            //if(!resault) resault = GameObject.Instantiate(value);
            //return resault;
            return GameObject.Instantiate(template, parent);
        }

        /// <summary>
        /// 预先为默认模板创建指定数量的实例
        /// </summary>
        public virtual void PreCreateInstance(int num) => PreCreateInstance(_template, num);
        /// <summary>
        /// 预先为对应模板创建指定数量的实例
        /// </summary>
        /// <param name="template"></param>
        /// <param name="num"></param>
        public virtual void PreCreateInstance(GameObject template, int num)
        {
            for (int i = 1; i <= num; i++)
            {
                var go = CreateInstance(template, returnParent);
                Return(go, template);
            }
        }
        /// <summary>
        /// 预先为对应模板创建指定数量的实例，该操作是异步的
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public virtual Coroutine PreCreateInstanceAsync(int num) => PreCreateInstanceAsync(_template, num);
        /// <summary>
        /// 预先为对应模板创建指定数量的实例，该操作是异步的
        /// </summary>
        /// <param name="template"></param>
        /// <param name="num"></param>
        public virtual Coroutine PreCreateInstanceAsync(GameObject template, int num)
        {
            var m = monitor;
            if (m == null) return null;
            var c = m.StartCoroutine(_PreCreateInstanceCoroutine(template, num));
            _preCreateInstanceCoroutines.Add(c);
            return c;
        }
        protected IEnumerator _PreCreateInstanceCoroutine(GameObject template, int num)
        {
            for (int i = 1; i <= num; i++)
            {
                var go = CreateInstance(template, returnParent);
                Return(go, template);
                yield return null;
            }
            yield break;
        }
        /// <summary>
        /// 取消所有预创建，仅取消任务，已创建的实例将保留
        /// </summary>
        public virtual void CancelPreCreateInstance()
        {
            var m = monitor;
            foreach (var item in _preCreateInstanceCoroutines)
            {
                if (item != null && m != null)
                    m.StopCoroutine(item);
            }
            _preCreateInstanceCoroutines.Clear();
        }

        /// <summary>从对象池获取，从默认模板 <see cref="template"/> 对应的池子里取</summary>
        public virtual GameObject Get()
        {
            return Get(_template, null);
        }

        /// <summary>从对象池获取，从默认模板 <see cref="template"/> 对应的池子里取</summary>
        public virtual GameObject Get(GameObject template)
        {
            return Get(template, null);
        }
        /// <summary>从对象池获取，从默认模板 <see cref="template"/> 对应的池子里取</summary>
        public virtual GameObject Get(Transform parent)
        {
            var obj = Get(_template, parent);
            return obj;
        }
        /// <summary>从对象池获取</summary>
        public virtual GameObject Get(GameObject template, Transform parent)
        {
            if (template == null) return null;

            GameObject obj = null;

            var pool = this._pool;
            var target = template;
            bool has = pool.TryGetValue(target, out var tPool);

            if (has)
            {
                while (tPool.Count > 0 && obj == null)
                {
                    //tPool.TryDequeue(out obj);
                    obj = FetchLast(tPool);
                    //if (obj != null)
                    //    obj.transform.SetParent(null);
                }
            }
            else
            {
                tPool = CreatePool();
                pool[target] = tPool;
            }
            if (!obj) obj = CreateInstance(target, parent);
            if (obj != null)
                obj.transform.SetParent(parent);

            var prc = obj.GetComponent<PoolRecordComponent>();
            if (!prc) prc = obj.AddComponent<PoolRecordComponent>();
            prc.record.pool = this;
            prc.record.template = template;
            prc.record.instance = obj;

            InitializeObject(obj);

            return obj;
        }

        /// <summary>从对象池获取，从默认模板 <see cref="template"/> 对应的池子里取</summary>
        public virtual GameObject Get(Vector3 position, Quaternion rotation)
        {
            var obj = Get();
            obj.transform.SetPositionAndRotation(position, rotation);
            return obj;
        }
        /// <summary>从对象池获取</summary>
        public virtual GameObject Get(GameObject template, Vector3 position, Quaternion rotation)
        {
            var obj = Get(template);
            obj.transform.SetPositionAndRotation(position, rotation);
            return obj;
        }

        /// <summary>返回对象池，默认返回到 <see cref="template"/> 对应的池子，如果不确定请使用 <see cref="Return(GameObject, GameObject)"/> 已指定返回到哪个池子</summary>
        /// <remarks>和 <see cref="TypePool.Return{T}(T)"/> 一样，会执行 <see cref="ITypePoolObject.Clear"/> 的清理操作，清理操作会在其他操作之后进行。</remarks>
        public virtual void Return(GameObject obj) => Return(obj, _template, returnParent);
        /// <summary>返回对象池</summary>
        /// <remarks>和 <see cref="TypePool.Return{T}(T)"/> 一样，会执行 <see cref="ITypePoolObject.Clear"/> 的清理操作，清理操作会在其他操作之后进行。</remarks>
        public virtual void Return(GameObject obj, GameObject template)
        {
            Return(obj, template, returnParent);
        }
        /// <summary>返回对象池</summary>
        /// <remarks>和 <see cref="TypePool.Return{T}(T)"/> 一样，会执行 <see cref="ITypePoolObject.Clear"/> 的清理操作，清理操作会在其他操作之后进行。</remarks>
        public virtual void Return(GameObject obj, Transform returnParent)
        {
            Return(obj, _template, returnParent);
        }
        /// <summary>返回对象池</summary>
        /// <remarks>和 <see cref="TypePool.Return{T}(T)"/> 一样，会执行 <see cref="ITypePoolObject.Clear"/> 的清理操作，清理操作会在其他操作之后进行。</remarks>
        public virtual void Return(GameObject obj, GameObject template, Transform returnParent)
        {
            if (obj == null) return;

            var target = template;
            bool has = _pool.TryGetValue(target, out var tPool);

            if (!has)
            {
                tPool = CreatePool();
                _pool[target] = tPool;
            }

            if (!tPool.Contains(obj))
            {
                tPool.Add(obj);
                obj.transform.SetParent(returnParent);
                var manager = GOManager;
                if (manager == null || returnParent != manager.transform)
                {
                    obj.SetActive(false);
                }

                // 清理操作
                CleanupObject(obj);
            }
        }

        /// <summary>
        /// 清理 <see cref="ITypePoolObject.Clear()"/>
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void CleanupObject(GameObject obj)
        {
            var tpos = TypePool.root.GetList<ITypePoolObject>();
            obj.GetComponents(tpos);
            if (tpos != null)
            {
                foreach (var tpo in tpos)
                {
                    tpo.Clear();
                }
            }
            TypePool.root.Return(tpos);
        }
        /// <summary>
        /// 初始 <see cref="ITypePoolObjectInit.Init()"/>
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void InitializeObject(GameObject obj)
        {
            var tpos = TypePool.root.GetList<ITypePoolObjectInit>();
            obj.GetComponents(tpos);
            if (tpos != null)
            {
                foreach (var tpo in tpos)
                {
                    tpo.Init();
                }
            }
            TypePool.root.Return(tpos);
        }

        protected GameObject FetchLast(List<GameObject> objs)
        {
            GameObject obj = null;
            if (objs.Count > 0)
            {
                int i = objs.Count - 1;
                obj = objs[i];
                objs.RemoveAt(i);
            }
            return obj;
        }

        /// <summary>清理对应模板的池</summary>
        public virtual void Clear(GameObject template)
        {
            var target = template;
            bool has = _pool.TryGetValue(target, out var tPool);

            if (has)
            {
                //tPool = new Queue<GameObject>();
                while (tPool.Count > 0)
                {
                    var _go = FetchLast(tPool);
                    GameObject.Destroy(_go);
                }
            }
        }

        /// <summary>清理对象池</summary>
        public virtual void Clear()
        {
            foreach (var t in _pool)
            {
                foreach (var _go in t.Value)
                {
                    if (_go)
                        GameObject.Destroy(_go);
                }
                t.Value.Clear();
            }
            _pool.Clear();
        }

        /// <summary>销毁对应模板的池</summary>
        public void Destroy(GameObject template)
        {
            Clear(template);
            _pool.Remove(template);
        }

        /// <summary>销毁对象池</summary>
        public void Destroy()
        {
            Clear();
            _pools.Remove(this);
        }

        protected static List<GameObject> CreatePool()
        {
            return new List<GameObject>(1);
        }
    }

    /// <summary>
    /// 用于记录 <see cref="GameObjectPool"/> 信息
    /// </summary>
    [Serializable]
    public class GameObjectPoolRecord : ITypePoolObject
    {
        [NonSerialized]
        public GameObjectPool pool;
        public GameObject template;
        /// <summary>
        /// 通过 <see cref="template"/> 实例化的实例，可选，视自己的使用方式而定
        /// </summary>
        public GameObject instance;

        public GameObjectPoolRecord()
        {

        }
        public GameObjectPoolRecord(GameObjectPool pool, GameObject template)
        {
            this.pool = pool;
            this.template = template;
        }
        public GameObjectPoolRecord(GameObjectPool pool, GameObject template, GameObject instance)
        {
            this.pool = pool;
            this.template = template;
            this.instance = instance;
        }

        /// <summary>
        /// 是否有效记录
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            bool r = pool != null && template != null;
            return r;
        }

        /// <summary>
        /// 返回对象池
        /// </summary>
        /// <returns></returns>
        public bool Return() => Return(instance);
        /// <summary>
        /// 返回对象池
        /// </summary>
        /// <returns></returns>
        public bool Return(GameObject instance)
        {
            if (IsValid() && instance)
            {
                pool.Return(instance, template);
                return true;
            }
            return false;
        }


        public void Clear()
        {
            pool = null;
            template = null;
            instance = null;
        }
    }

    /// <summary>
    /// 可用于记录 <see cref="GameObjectPoolRecord"/>
    /// </summary>
    public class PoolRecordComponent : MonoBehaviour
    {
        public GameObjectPoolRecord record = new GameObjectPoolRecord();

        /// <summary>
        /// 通过 <see cref="record"/> 记录的对象池信息放入对象池
        /// </summary>
        /// <returns></returns>
        public bool Return()
        {
            if (record == null) return false;
            return record.Return();
        }
    }

}