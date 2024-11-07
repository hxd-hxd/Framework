// -------------------------
// 创建日期：2022/10/27 11:47:04
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

using Object = UnityEngine.Object;

namespace Framework
{
    /// <summary>
    /// 默认的 Unity 资源异步操作
    /// </summary>
    public class DefaultResourcesOperation : IAssetOperation
    {
        internal AsyncOperation _assetHandle;

        internal string _sceneNameOrPath;

        internal Object _assetObject;
        internal Object[] _assetObjects;
        internal Action<IAssetOperation> _completed;

        public DefaultResourcesOperation()
        {
            //Resources
            //SceneManager
        }
        public DefaultResourcesOperation(AsyncOperation assetHandle)
        {
            this.assetHandle = assetHandle;
        }
        public DefaultResourcesOperation(AsyncOperation assetHandle, string sceneNameOrPath)
        {
            this.assetHandle = assetHandle;
            this._sceneNameOrPath = sceneNameOrPath;
        }

        internal AsyncOperation assetHandle
        {
            get { return _assetHandle; }
            set
            {
                _assetHandle = value;
                if (value != null)
                {
                    value.completed -= OnCompleted;
                    value.completed += OnCompleted;
                }
            }
        }
        internal ResourceRequest resourceRequest
        {
            get { return _assetHandle as ResourceRequest; }
        }

        public bool IsDone => _assetHandle?.isDone ?? true;

        public float Progress => _assetHandle?.progress ?? 1;

        public bool IsValid => _assetHandle != null;

        public string DebugText => null;

        public AsyncOperationStatus Status
        {
            get
            {
                if (_assetHandle != null)
                {
                    if (IsDone)
                    {
                        return AsyncOperationStatus.Succeed;
                    }
                    return AsyncOperationStatus.Processing;
                }
                else
                {
                    //return AsyncOperationStatus.Failed;
                    return AsyncOperationStatus.None;
                }
            }
        }

        public UnityEngine.Object AssetObject => _assetObject;
        public UnityEngine.Object[] AssetObjects => _assetObjects;

        public Scene? SceneObject
        {
            get
            {
                if (string.IsNullOrEmpty(_sceneNameOrPath)) return null;

                Scene s;
                s = SceneManager.GetSceneByPath(_sceneNameOrPath);
                if (!s.IsValid() || string.IsNullOrEmpty(s.path))
                    s = SceneManager.GetSceneByName(_sceneNameOrPath);

                return s;
            }
        }


        public event Action<IAssetOperation> Completed
        {
            add
            {
                if (IsDone)
                {
                    value(this);
                    return;
                }
                _completed += value;
            }
            remove { _completed -= value; }
        }

        internal void OnCompleted(AsyncOperation handle)
        {
            _assetObject ??= resourceRequest?.asset;
            _assetObjects ??= new Object[] { AssetObject };

            // 注意：如果完成回调内发生异常，会导致Task无限期等待
            _completed?.Invoke(this);

            if (_taskCompletionSource != null)
                _taskCompletionSource.TrySetResult(null);
        }

        public T GetAssetObject<T>() where T : UnityEngine.Object
        {
            return AssetObject as T;
        }
        public T[] GetAllAssetObjects<T>() where T : UnityEngine.Object
        {
            if (_assetObjects == null) return default;

            var objects = new List<T>();
            foreach (T tObj in _assetObjects)
            {
                if (tObj != null)
                {
                    objects.Add(tObj);
                }
            }
            return objects.ToArray();
        }

        public void Release()
        {
            _assetHandle = null;
        }

        public void WaitForAsyncComplete()
        {
            if (_assetHandle != null)
            {
                while (!IsDone) ;
            }
        }

        public void ActivateScene()
        {
            if (_assetHandle != null)
            {
                _assetHandle.allowSceneActivation = true;
            }
        }

        #region 异步操作

        TaskCompletionSource<object> _taskCompletionSource;
        public Task Task
        {
            get
            {
                if (_taskCompletionSource == null)
                {
                    _taskCompletionSource = new TaskCompletionSource<object>();
                    if (IsDone)
                        _taskCompletionSource.SetResult(null);
                }
                return _taskCompletionSource.Task;
            }
        }

        object IEnumerator.Current => _assetHandle;
        bool IEnumerator.MoveNext()
        {
            return !IsDone;
        }
        void IEnumerator.Reset() { }
        #endregion

        public static DefaultResourcesOperation Get()
        {
            var r = new DefaultResourcesOperation();
            return r;
        }
        public static DefaultResourcesOperation Get(AsyncOperation handle)
        {
            var r = new DefaultResourcesOperation(handle);
            return r;
        }
        public static DefaultResourcesOperation Get(AsyncOperation handle, string sceneNameOrPath)
        {
            var r = new DefaultResourcesOperation(handle, sceneNameOrPath);
            return r;
        }
    }
}