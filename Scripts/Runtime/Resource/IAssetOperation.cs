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
    /// 资源异步操作
    /// </summary>
    public interface IAssetOperation : IEnumerator
    {
        /// <summary>
        /// 是否结束
        /// </summary>
        bool IsDone {  get; }
        /// <summary>
        /// 进度
        /// </summary>
        float Progress {  get; }
        /// <summary>
        /// 是否有效
        /// </summary>
        bool IsValid {  get; }
        /// <summary>
        /// 调试信息
        /// </summary>
        string DebugText {  get; }

        /// <summary>
        /// 资源对象
        /// </summary>
        Object AssetObject { get; }

        /// <summary>
        /// 所有资源对象
        /// </summary>
        Object[] AssetObjects { get; }

        /// <summary>
        /// 场景对象
        /// </summary>
        Scene? SceneObject { get; }

        /// <summary>
        /// 异步操作状态
        /// </summary>
        AsyncOperationStatus Status { get; }

        /// <summary>
        /// 异步操作完成回调
        /// </summary>
        event Action<IAssetOperation> Completed;

        /// <summary>
        /// 异步操作任务
        /// </summary>
        System.Threading.Tasks.Task Task { get; }

        /// <summary>
        /// 释放
        /// </summary>
        void Release();
        /// <summary>
        /// 等待异步操作完成
        /// </summary>
        void WaitForAsyncComplete();
        /// <summary>
        /// 激活场景
        /// </summary>
        void ActivateScene();

        /// <summary>
        /// 获取资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetAssetObject<T>() where T : Object;
        /// <summary>
        /// 获取所有资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T[] GetAllAssetObjects<T>() where T : Object;
    }

    /// <summary>
    /// 异步操作状态
    /// </summary>
    public enum AsyncOperationStatus
    {
        None,
        /// <summary>进行中</summary>
        Processing,
        /// <summary>成功</summary>
        Succeed,
        /// <summary>失败</summary>
        Failed
    }
}