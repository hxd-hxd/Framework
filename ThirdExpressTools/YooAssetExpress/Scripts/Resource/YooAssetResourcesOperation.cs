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
using YooAsset;

using Object = UnityEngine.Object;

namespace Framework
{
    /// <summary>
    /// 资源异步操作
    /// </summary>
    public class YooAssetResourcesOperation : IAssetOperation
    {
        private AssetHandle _assetHandle;
        private AllAssetsHandle _allAssetsHandle;
        private SceneHandle _sceneHandle;

        private Object[] _assetObjects;
        internal Action<YooAssetResourcesOperation> _completed;

        public YooAssetResourcesOperation()
        {

        }
        public YooAssetResourcesOperation(AssetHandle assetHandle)
        {
            this.assetHandle = assetHandle;
        }
        public YooAssetResourcesOperation(AllAssetsHandle allAssetsHandle)
        {
            this.allAssetsHandle = allAssetsHandle;
        }
        public YooAssetResourcesOperation(SceneHandle sceneHandle)
        {
            this.sceneHandle = sceneHandle;
        }

        internal AssetHandle assetHandle
        {
            get { return _assetHandle; }
            set
            {
                _assetHandle = value;
                if (value != null)
                {
                    value.Completed -= OnCompleted;
                    value.Completed += OnCompleted;
                }
            }
        }
        internal AllAssetsHandle allAssetsHandle
        {
            get { return _allAssetsHandle; }
            set
            {
                _allAssetsHandle = value;
                if (value != null)
                {
                    value.Completed -= OnCompleted;
                    value.Completed += OnCompleted;
                }
            }
        }
        internal SceneHandle sceneHandle
        {
            get { return _sceneHandle; }
            set
            {
                _sceneHandle = value;
                if (value != null)
                {
                    value.Completed -= OnCompleted;
                    value.Completed += OnCompleted;
                }
            }
        }
        internal HandleBase handleBase
        {
            get
            {
                //if (assetHandle != null) return assetHandle;
                //if (allAssetsHandle != null) return allAssetsHandle;
                //if (sceneHandle != null) return sceneHandle;
                return assetHandle ?? allAssetsHandle ?? sceneHandle as HandleBase;
            }
        }

        public bool IsDone => handleBase?.IsDone ?? true;

        public float Progress => handleBase?.Progress ?? 1;

        public bool IsValid => handleBase?.IsValid ?? false;

        public string DebugText => handleBase?.LastError ?? "空";

        public AsyncOperationStatus Status
        {
            get
            {
                if (handleBase == null) return AsyncOperationStatus.None;

                switch (handleBase.Status)
                {
                    case EOperationStatus.None:
                        return AsyncOperationStatus.None;
                    case EOperationStatus.Processing:
                        return AsyncOperationStatus.Processing;
                    case EOperationStatus.Succeed:
                        return AsyncOperationStatus.Succeed;
                    case EOperationStatus.Failed:
                        return AsyncOperationStatus.Failed;
                    default:
                        return AsyncOperationStatus.None;
                }
            }
        }

        public UnityEngine.Object AssetObject => _assetHandle?.AssetObject;
        public UnityEngine.Object[] AssetObjects => _assetObjects;

        public Scene? SceneObject => sceneHandle?.SceneObject ?? null;

        public event Action<IAssetOperation> Completed
        {
            add { _completed += value; }
            remove { _completed -= value; }
        }

        private void OnCompleted(AssetHandle handle)
        {
            _assetObjects = new Object[] { _assetHandle.AssetObject };

            _completed?.Invoke(this);
        }
        private void OnCompleted(AllAssetsHandle handle)
        {
            _assetObjects = _allAssetsHandle?.AllAssetObjects as Object[];

            _completed?.Invoke(this);
        }
        private void OnCompleted(SceneHandle handle)
        {
            _completed?.Invoke(this);
        }

        public T GetAssetObject<T>() where T : UnityEngine.Object
        {
            return _assetHandle.GetAssetObject<T>();
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
            _assetHandle?.Release();
            _allAssetsHandle?.Release();
        }

        public void WaitForAsyncComplete()
        {
            _assetHandle?.WaitForAsyncComplete();
            _allAssetsHandle?.WaitForAsyncComplete();
        }

        public void ActivateScene()
        {
            _sceneHandle?.ActivateScene();
        }


        #region 异步操作
        public Task Task => handleBase.Task;

        object IEnumerator.Current => (handleBase as IEnumerator).Current;

        bool IEnumerator.MoveNext()
        {
            return (handleBase as IEnumerator).MoveNext();
        }

        void IEnumerator.Reset()
        {
            (handleBase as IEnumerator).Reset();
        }
        #endregion


        public static YooAssetResourcesOperation Get(AssetHandle handle)
        {
            var r = new YooAssetResourcesOperation(handle);
            return r;
        }
        public static YooAssetResourcesOperation Get(AllAssetsHandle handle)
        {
            var r = new YooAssetResourcesOperation(handle);
            return r;
        }
        public static YooAssetResourcesOperation Get(SceneHandle handle)
        {
            var r = new YooAssetResourcesOperation(handle);
            return r;
        }

    }
}