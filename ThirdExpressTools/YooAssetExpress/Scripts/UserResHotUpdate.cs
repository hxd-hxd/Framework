// -------------------------
// 创建日期：2024/10/29 16:33:14
// -------------------------

#pragma warning disable 0414
#pragma warning disable 0219

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Event;
using UnityEngine.Diagnostics;

namespace Framework
{
    public class UserResHotUpdate : MonoBehaviour
    {
        EventGroup _eventGroup = new EventGroup();

        private void Awake()
        {

            _eventGroup.AddListener<PatchEventDefine.InitializeFailed>(OnHandleEventMessage);
            _eventGroup.AddListener<PatchEventDefine.PatchStatesChange>(OnHandleEventMessage);
            _eventGroup.AddListener<PatchEventDefine.FoundUpdateFiles>(OnHandleEventMessage);
            _eventGroup.AddListener<PatchEventDefine.DownloadProgressUpdate>(OnHandleEventMessage);
            _eventGroup.AddListener<PatchEventDefine.PackageVersionUpdateFailed>(OnHandleEventMessage);
            _eventGroup.AddListener<PatchEventDefine.PatchManifestUpdateFailed>(OnHandleEventMessage);
            _eventGroup.AddListener<PatchEventDefine.WebFileDownloadFailed>(OnHandleEventMessage);
            _eventGroup.AddListener<PatchEventDefine.UpdateDone>(OnHandleEventMessage);
        }

        void OnDestroy()
        {
            _eventGroup.Clear();
        }

        /// <summary>
        /// 接收事件
        /// </summary>
        private void OnHandleEventMessage(IEventMessage message)
        {
            if (message is PatchEventDefine.InitializeFailed)
            {
                Debug.Log("初始化包失败！");
                UserEventDefine.UserTryInitialize.SendEventMessage();
            }
            else if (message is PatchEventDefine.PatchStatesChange)
            {
                var msg = message as PatchEventDefine.PatchStatesChange;
                Debug.Log($"流程改变：{msg.Tips}");
            }
            else if (message is PatchEventDefine.FoundUpdateFiles)
            {
                var msg = message as PatchEventDefine.FoundUpdateFiles;
                float sizeMB = msg.TotalSizeBytes / 1048576f;
                sizeMB = Mathf.Clamp(sizeMB, 0.1f, float.MaxValue);
                string totalSizeMB = sizeMB.ToString("f1");
                Debug.Log($"存在可更新文件, 总数 <color=yellow>{msg.TotalCount}</color> 总大小 <color=yellow>{totalSizeMB}MB</color>");
                UserEventDefine.UserBeginDownloadWebFiles.SendEventMessage();
            }
            else if (message is PatchEventDefine.DownloadProgressUpdate)
            {
                var msg = message as PatchEventDefine.DownloadProgressUpdate;
                float p = (float)msg.CurrentDownloadCount / msg.TotalDownloadCount;
                string currentSizeMB = (msg.CurrentDownloadSizeBytes / 1048576f).ToString("f1");
                string totalSizeMB = (msg.TotalDownloadSizeBytes / 1048576f).ToString("f1");
                Debug.Log( $"当前下载进度：{p}。{msg.CurrentDownloadCount}/{msg.TotalDownloadCount} {currentSizeMB}MB/{totalSizeMB}MB");
            }
            else if (message is PatchEventDefine.PackageVersionUpdateFailed)
            {
                Debug.Log($"更新静态版本失败，请检查网络状态。");
                UserEventDefine.UserTryUpdatePackageVersion.SendEventMessage();
            }
            else if (message is PatchEventDefine.PatchManifestUpdateFailed)
            {
                Debug.Log($"下载文件失败更新补丁清单失败，请检查网络状态。");
                UserEventDefine.UserTryUpdatePatchManifest.SendEventMessage();
            }
            else if (message is PatchEventDefine.WebFileDownloadFailed)
            {
                var msg = message as PatchEventDefine.WebFileDownloadFailed;
                Debug.Log($"下载文件失败：{msg.FileName}\r\n{msg.Error}");
                UserEventDefine.UserTryDownloadWebFiles.SendEventMessage();
            }
            else if (message is PatchEventDefine.UpdateDone)
            {
                Debug.Log("更新完毕");
            }
            else
            {
                throw new System.NotImplementedException($"{message.GetType()}");
            }
        }

    }
}

#pragma warning restore 0414
#pragma warning restore 0219