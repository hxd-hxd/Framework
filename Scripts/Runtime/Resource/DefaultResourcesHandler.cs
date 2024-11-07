// -------------------------
// 创建日期：2022/10/27 10:12:12
// -------------------------

using System;
using UnityEngine;

using Object = UnityEngine.Object;
using UnityEngine.SceneManagement;

namespace Framework
{
    /// <summary>
    /// 默认的 Unity <see cref="Resources"/> 文件夹、<see cref="SceneManager"/> 等资源处理器
    /// </summary>
    public class DefaultResourcesHandler : IResourcesHandler
    {
        public void Initialize()
        {

        }

        public Object LoadAssetObject(string path)
        {
            var obj = Resources.Load(path);
            return obj;
        }
        public TObject LoadAssetObject<TObject>(string path) where TObject : Object
        {
            var obj = Resources.Load<TObject>(path);
            return obj;
        }
        public Object LoadAssetObject(string path, Type type)
        {
            var obj = Resources.Load(path, type);
            return obj;
        }
        public IAssetOperation LoadAsset(string path)
        {
            var obj = Resources.Load(path);
            var h = DefaultResourcesOperation.Get();
            h._assetObject = obj;
            return h;
        }
        public IAssetOperation LoadAsset<TObject>(string path) where TObject : Object
        {
            var obj = Resources.Load<TObject>(path);
            var h = DefaultResourcesOperation.Get();
            h._assetObject = obj;
            return h;
        }
        public IAssetOperation LoadAsset(string path, Type type)
        {
            var obj = Resources.Load(path, type);
            var h = DefaultResourcesOperation.Get();
            h._assetObject = obj;
            return h;
        }

        public IAssetOperation LoadAssetAsync(string path)
        {
            var uh = Resources.LoadAsync(path);
            var h = DefaultResourcesOperation.Get(uh);
            return h;
        }
        public IAssetOperation LoadAssetAsync<TObject>(string path) where TObject : Object
        {
            var uh = Resources.LoadAsync<TObject>(path);
            var h = DefaultResourcesOperation.Get(uh);
            return h;
        }
        public IAssetOperation LoadAssetAsync(string path, Type type)
        {
            var uh = Resources.LoadAsync(path, type);
            var h = DefaultResourcesOperation.Get(uh);
            return h;
        }

        public Object[] LoadAllAssetObjects(string path)
        {
            var objs = Resources.LoadAll(path);
            return objs;
        }
        public TObject[] LoadAllAssetObjects<TObject>(string path) where TObject : Object
        {
            var objs = Resources.LoadAll<TObject>(path);
            return objs;
        }
        public Object[] LoadAllAssetObjects(string path, Type type)
        {
            var objs = Resources.LoadAll(path, type);
            return objs;
        }
        public IAssetOperation LoadAllAssets(string path)
        {
            var objs = Resources.LoadAll(path);
            var h = DefaultResourcesOperation.Get();
            h._assetObjects = objs;
            return h;
        }
        public IAssetOperation LoadAllAssets<TObject>(string path) where TObject : Object
        {
            var objs = Resources.LoadAll<TObject>(path);
            var h = DefaultResourcesOperation.Get();
            h._assetObjects = objs;
            return h;
        }
        public IAssetOperation LoadAllAssets(string path, Type type)
        {
            var objs = Resources.LoadAll(path, type);
            var h = DefaultResourcesOperation.Get();
            h._assetObjects = objs;
            return h;
        }

        // 由于 Resources.LoadAll 没有异步操作，所以此处与同步操作一样
        public IAssetOperation LoadAllAssetsAsync(string path)
        {
            var objs = Resources.LoadAll(path);
            var h = DefaultResourcesOperation.Get();
            h._assetObjects = objs;
            //Coroutines.Delay(() => h.OnCompleted(null));
            return h;
        }
        public IAssetOperation LoadAllAssetsAsync<TObject>(string path) where TObject : Object
        {
            var objs = Resources.LoadAll<TObject>(path);
            var h = DefaultResourcesOperation.Get();
            h._assetObjects = objs;
            //Coroutines.Delay(() => h.OnCompleted(null));
            return h;
        }
        public IAssetOperation LoadAllAssetsAsync(string path, Type type)
        {
            var objs = Resources.LoadAll(path, type);
            var h = DefaultResourcesOperation.Get();
            h._assetObjects = objs;
            //Coroutines.Delay(() => h.OnCompleted(null));
            return h;
        }

        public Scene LoadSceneObject(string path, LoadSceneMode sceneMode, LocalPhysicsMode physicsMode)
        {
            var lsp = new LoadSceneParameters(sceneMode, physicsMode);
            var s = SceneManager.LoadScene(path, lsp);
            return s;
        }
        public IAssetOperation LoadScene(string path, LoadSceneMode sceneMode, LocalPhysicsMode physicsMode)
        {
            var lsp = new LoadSceneParameters(sceneMode, physicsMode);
            var s = SceneManager.LoadScene(path, lsp);
            var h = DefaultResourcesOperation.Get();
            h._sceneNameOrPath = path;
            return h;
        }
        public IAssetOperation LoadSceneAsync(string path, LoadSceneMode sceneMode, LocalPhysicsMode physicsMode, bool allowSceneActivation, uint priority)
        {
            var lsp = new LoadSceneParameters(sceneMode, physicsMode);
            var s = SceneManager.LoadSceneAsync(path, lsp);
            s.priority = (int)priority;
            s.allowSceneActivation = allowSceneActivation;
            var h = DefaultResourcesOperation.Get(s, path);
            h._sceneNameOrPath = path;
            return h;
        }
    }
}