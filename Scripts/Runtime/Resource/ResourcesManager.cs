// -------------------------
// 创建日期：2022/10/27 10:12:12
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Framework
{
    /// <summary>
    /// Resources 资源管理器
    /// </summary>
    public class ResourcesManager
    {
        static IResourcesHandler _handler;

        /// <summary>
        /// 获取资源，<see cref="Resources.Load{T}(string)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T Load<T>(string path) where T : UnityEngine.Object
        {
            T obj = Resources.Load<T>(path);
            return obj;
        }
        /// <summary>
        /// 获取资源，<see cref="Resources.Load"/>
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Object Load(string path)
        {
            Object obj = Resources.Load(path);
            return obj;
        }

        /// <summary>初始化，需要在 <see cref="SetHandler{T}()"/>、<see cref="SetHandler(IResourcesHandler)"/> 等之后调用</summary>
        public static void Initialize()
        {
            if (_handler == null) throw new NullReferenceException("未设置资源处理器");
            _handler.Initialize();
        }

        /// <summary>同步加载资源对象</summary>
        public static Object LoadAssetObject(string path)
        {
            if (_handler == null) throw new NullReferenceException("未设置资源处理器");
            return _handler.LoadAssetObject(path);
        }
        /// <summary>同步加载资源对象</summary>
        public static TObject LoadAssetObject<TObject>(string path) where TObject : Object
        {
            if (_handler == null) throw new NullReferenceException("未设置资源处理器");
            return _handler.LoadAssetObject<TObject>(path);
        }
        /// <summary>同步加载资源对象</summary>
        public static Object LoadAssetObject(string path, Type type)
        {
            if (_handler == null) throw new NullReferenceException("未设置资源处理器");
            return _handler.LoadAssetObject(path, type);
        }
        /// <summary>同步加载资源</summary>
        public static IAssetOperation LoadAsset(string path)
        {
            if (_handler == null) throw new NullReferenceException("未设置资源处理器");
            return _handler.LoadAsset(path);
        }
        /// <summary>同步加载资源</summary>
        public static IAssetOperation LoadAsset<TObject>(string path) where TObject : Object
        {
            if (_handler == null) throw new NullReferenceException("未设置资源处理器");
            return _handler.LoadAsset<TObject>(path);
        }
        /// <summary>同步加载资源</summary>
        public static IAssetOperation LoadAsset(string path, Type type)
        {
            if (_handler == null) throw new NullReferenceException("未设置资源处理器");
            return _handler.LoadAsset(path, type);
        }

        /// <summary>异步加载资源</summary>
        public static IAssetOperation LoadAssetAsync(string path)
        {
            if (_handler == null) throw new NullReferenceException("未设置资源处理器");
            return _handler.LoadAssetAsync(path);
        }
        /// <summary>异步加载资源</summary>
        public static IAssetOperation LoadAssetAsync<TObject>(string path) where TObject : Object
        {
            if (_handler == null) throw new NullReferenceException("未设置资源处理器");
            return _handler.LoadAssetAsync<TObject>(path);
        }
        /// <summary>异步加载资源</summary>
        public static IAssetOperation LoadAssetAsync(string path, Type type)
        {
            if (_handler == null) throw new NullReferenceException("未设置资源处理器");
            return _handler.LoadAssetAsync(path, type);
        }

        /// <summary>同步加载所有资源对象</summary>
        public static Object[] LoadAllAssetObjects(string path)
        {
            if (_handler == null) throw new NullReferenceException("未设置资源处理器");
            return _handler.LoadAllAssetObjects(path);
        }
        /// <summary>同步加载所有资源对象</summary>
        public static TObject[] LoadAllAssetObjects<TObject>(string path) where TObject : Object
        {
            if (_handler == null) throw new NullReferenceException("未设置资源处理器");
            return _handler.LoadAllAssetObjects<TObject>(path);
        }
        /// <summary>同步加载所有资源对象</summary>
        public static Object[] LoadAllAssetObjects(string path, Type type)
        {
            if (_handler == null) throw new NullReferenceException("未设置资源处理器");
            return _handler.LoadAllAssetObjects(path, type);
        }
        /// <summary>同步加载所有资源</summary>
        public static IAssetOperation LoadAllAssets(string path)
        {
            if (_handler == null) throw new NullReferenceException("未设置资源处理器");
            return _handler.LoadAllAssets(path);
        }
        /// <summary>同步加载所有资源</summary>
        public static IAssetOperation LoadAllAssets<TObject>(string path) where TObject : Object
        {
            if (_handler == null) throw new NullReferenceException("未设置资源处理器");
            return _handler.LoadAllAssets<TObject>(path);
        }
        /// <summary>同步加载所有资源</summary>
        public static IAssetOperation LoadAllAssets(string path, Type type)
        {
            if (_handler == null) throw new NullReferenceException("未设置资源处理器");
            return _handler.LoadAllAssets(path, type);
        }
        /// <summary>异步加载所有资源对象</summary>
        public IAssetOperation LoadAllAssetsAsync(string path)
        {
            if (_handler == null) throw new NullReferenceException("未设置资源处理器");
            return _handler.LoadAllAssetsAsync(path);
        }
        /// <summary>异步加载所有资源对象</summary>
        public IAssetOperation LoadAllAssetsAsync<TObject>(string path) where TObject : Object
        {
            if (_handler == null) throw new NullReferenceException("未设置资源处理器");
            return _handler.LoadAllAssetsAsync<TObject>(path);
        }
        /// <summary>异步加载所有资源对象</summary>
        public IAssetOperation LoadAllAssetsAsync(string path, Type type)
        {
            if (_handler == null) throw new NullReferenceException("未设置资源处理器");
            return _handler.LoadAllAssetsAsync(path, type);
        }

        /// <summary>同步加载场景</summary>
        public static Scene LoadSceneObject(string path, LoadSceneMode sceneMode = LoadSceneMode.Single, LocalPhysicsMode physicsMode = LocalPhysicsMode.None)
        {
            if (_handler == null) throw new NullReferenceException("未设置资源处理器");
            return _handler.LoadSceneObject(path, sceneMode, physicsMode);
        }
        /// <summary>同步加载场景</summary>
        public static IAssetOperation LoadScene(string path, LoadSceneMode sceneMode = LoadSceneMode.Single, LocalPhysicsMode physicsMode = LocalPhysicsMode.None)
        {
            if (_handler == null) throw new NullReferenceException("未设置资源处理器");
            return _handler.LoadScene(path, sceneMode, physicsMode);
        }
        /// <summary>异步加载场景</summary>
        /// <param name="path"></param>
        /// <param name="sceneMode"></param>
        /// <param name="physicsMode"></param>
        /// <param name="allowSceneActivation">允许激活场景</param>
        /// <param name="priority">优先级</param>
        /// <returns></returns>
        public static IAssetOperation LoadSceneAsync(string path, LoadSceneMode sceneMode = LoadSceneMode.Single, LocalPhysicsMode physicsMode = LocalPhysicsMode.None, bool allowSceneActivation = true, uint priority = 100)
        {
            if (_handler == null) throw new NullReferenceException("未设置资源处理器");
            return _handler.LoadSceneAsync(path, sceneMode, physicsMode, allowSceneActivation, priority);
        }

        /// <summary>
        /// 设置资源处理器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void SetHandler<T>() where T : IResourcesHandler, new()
        {
            if (_handler == null)
            {
                _handler = new T();
            }
        }
        /// <summary>
        /// 设置资源处理器
        /// </summary>
        /// <param name="handler"></param>
        public static void SetHandler(IResourcesHandler handler)
        {
            _handler = handler;
        }
    }
}