// -------------------------
// 创建日期：2022/10/27 11:47:04
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.SceneManagement;

namespace Framework
{
    /// <summary>
    /// 资源处理器
    /// </summary>
    public interface IResourcesHandler
    {
        /// <summary>初始化</summary>
        void Initialize();

        /// <summary>同步加载资源对象</summary>
        Object LoadAssetObject(string path);
        /// <summary>同步加载资源对象</summary>
        TObject LoadAssetObject<TObject>(string path) where TObject : Object;
        /// <summary>同步加载资源对象</summary>
        Object LoadAssetObject(string path, Type type);
        /// <summary>同步加载资源</summary>
        IAssetOperation LoadAsset(string path);
        /// <summary>同步加载资源</summary>
        IAssetOperation LoadAsset<TObject>(string path) where TObject : Object;
        /// <summary>同步加载资源</summary>
        IAssetOperation LoadAsset(string path, Type type);
        /// <summary>异步加载资源</summary>
        IAssetOperation LoadAssetAsync(string path);
        /// <summary>异步加载资源</summary>
        IAssetOperation LoadAssetAsync<TObject>(string path) where TObject : Object;
        /// <summary>异步加载资源</summary>
        IAssetOperation LoadAssetAsync(string path, Type type);

        /// <summary>同步加载所有资源对象</summary>
        Object[] LoadAllAssetObjects(string path);
        /// <summary>同步加载所有资源对象</summary>
        TObject[] LoadAllAssetObjects<TObject>(string path) where TObject : Object;
        /// <summary>同步加载所有资源对象</summary>
        Object[] LoadAllAssetObjects(string path, Type type);
        /// <summary>同步加载所有资源对象</summary>
        IAssetOperation LoadAllAssets(string path);
        /// <summary>同步加载所有资源对象</summary>
        IAssetOperation LoadAllAssets<TObject>(string path) where TObject : Object;
        /// <summary>同步加载所有资源对象</summary>
        IAssetOperation LoadAllAssets(string path, Type type);
        /// <summary>异步加载所有资源对象</summary>
        IAssetOperation LoadAllAssetsAsync(string path);
        /// <summary>异步加载所有资源对象</summary>
        IAssetOperation LoadAllAssetsAsync<TObject>(string path) where TObject : Object;
        /// <summary>异步加载所有资源对象</summary>
        IAssetOperation LoadAllAssetsAsync(string path, Type type);

        /// <summary>同步加载场景</summary>
        Scene LoadSceneObject(string path, LoadSceneMode sceneMode, LocalPhysicsMode physicsMode);
        /// <summary>同步加载场景</summary>
        IAssetOperation LoadScene(string path, LoadSceneMode sceneMode, LocalPhysicsMode physicsMode);
        /// <summary>异步加载场景</summary>
        /// <param name="path"></param>
        /// <param name="sceneMode"></param>
        /// <param name="physicsMode"></param>
        /// <param name="allowSceneActivation">允许激活场景</param>
        /// <param name="priority">优先级</param>
        /// <returns></returns>
        IAssetOperation LoadSceneAsync(string path, LoadSceneMode sceneMode, LocalPhysicsMode physicsMode, bool allowSceneActivation, uint priority);
        
    }
}