// -------------------------
// 创建日期：2022/10/27 10:12:12
// -------------------------

using System;

using YooAsset;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Framework
{
    /// <summary>
    /// YooAsset 资源处理器
    /// </summary>
    public class YooAssetResourcesHandler : IResourcesHandler
    {
        public void Initialize()
        {
            YooAssets.Initialize();
        }

        public Object LoadAssetObject(string path)
        {
            var operation = LoadAsset(path);
            var obj = operation.AssetObject;
            operation.Release();
            return obj;
        }
        public TObject LoadAssetObject<TObject>(string path) where TObject : Object
        {
            var operation = LoadAsset<TObject>(path);
            var obj = operation.GetAssetObject<TObject>();
            operation.Release();
            return obj;
        }
        public Object LoadAssetObject(string path, Type type)
        {
            var operation = LoadAsset(path, type);
            var obj = operation.AssetObject;
            operation.Release();
            return obj;
        }
        public IAssetOperation LoadAsset(string path)
        {
            var h = YooAssets.LoadAssetSync(path);
            return YooAssetResourcesOperation.Get(h);
        }
        public IAssetOperation LoadAsset<TObject>(string path) where TObject : Object
        {
            var h = YooAssets.LoadAssetSync<TObject>(path);
            return YooAssetResourcesOperation.Get(h);
        }
        public IAssetOperation LoadAsset(string path, Type type)
        {
            var h = YooAssets.LoadAssetSync(path, type);
            return YooAssetResourcesOperation.Get(h);
        }

        public IAssetOperation LoadAssetAsync(string path)
        {
            var h = YooAssets.LoadAssetAsync(path);
            return YooAssetResourcesOperation.Get(h);
        }
        public IAssetOperation LoadAssetAsync<TObject>(string path) where TObject : Object
        {
            var h = YooAssets.LoadAssetAsync<TObject>(path);
            return YooAssetResourcesOperation.Get(h);
        }
        public IAssetOperation LoadAssetAsync(string path, Type type)
        {
            var h = YooAssets.LoadAssetAsync(path, type);
            return YooAssetResourcesOperation.Get(h);
        }

        public Object[] LoadAllAssetObjects(string path)
        {
            var operation = LoadAllAssets(path);
            var obj = operation.AssetObjects;
            operation.Release();
            return obj;
        }
        public TObject[] LoadAllAssetObjects<TObject>(string path) where TObject : Object
        {
            var operation = LoadAllAssets<TObject>(path);
            var obj = operation.GetAllAssetObjects<TObject>();
            operation.Release();
            return obj;
        }
        public Object[] LoadAllAssetObjects(string path, Type type)
        {
            var operation = LoadAllAssets(path, type);
            var obj = operation.AssetObjects;
            operation.Release();
            return obj;
        }
        public IAssetOperation LoadAllAssets(string path)
        {
            var h = YooAssets.LoadAllAssetsSync(path);
            return YooAssetResourcesOperation.Get(h);
        }
        public IAssetOperation LoadAllAssets<TObject>(string path) where TObject : Object
        {
            var h = YooAssets.LoadAllAssetsSync<TObject>(path);
            return YooAssetResourcesOperation.Get(h);
        }
        public IAssetOperation LoadAllAssets(string path, Type type)
        {
            var h = YooAssets.LoadAllAssetsSync(path, type);
            return YooAssetResourcesOperation.Get(h);
        }
        public IAssetOperation LoadAllAssetsAsync(string path)
        {
            var h = YooAssets.LoadAllAssetsAsync(path);
            return YooAssetResourcesOperation.Get(h);
        }
        public IAssetOperation LoadAllAssetsAsync<TObject>(string path) where TObject : Object
        {
            var h = YooAssets.LoadAllAssetsAsync<TObject>(path);
            return YooAssetResourcesOperation.Get(h);
        }
        public IAssetOperation LoadAllAssetsAsync(string path, Type type)
        {
            var h = YooAssets.LoadAllAssetsAsync(path, type);
            return YooAssetResourcesOperation.Get(h);
        }

        public Scene LoadSceneObject(string path, LoadSceneMode sceneMode, LocalPhysicsMode physicsMode)
        {
            var operation = LoadScene(path, sceneMode, physicsMode);
            var obj = operation.SceneObject != null ? operation.SceneObject.Value : default;
            operation.Release();
            return obj;
        }
        public IAssetOperation LoadScene(string path, LoadSceneMode sceneMode, LocalPhysicsMode physicsMode)
        {
            var h = YooAssets.LoadSceneSync(path, sceneMode, physicsMode);
            return YooAssetResourcesOperation.Get(h);
        }
        public IAssetOperation LoadSceneAsync(string path, LoadSceneMode sceneMode, LocalPhysicsMode physicsMode, bool allowSceneActivation, uint priority)
        {
            // 注意：YooAssets 这里的 suspendLoad 参数是在加载时挂起，从而不激活场景，而接口 allowSceneActivation 是允许激活，所以这里要反向操作
            var h = YooAssets.LoadSceneAsync(path, sceneMode, physicsMode, !allowSceneActivation, priority);
            return YooAssetResourcesOperation.Get(h);
        }


    }
}