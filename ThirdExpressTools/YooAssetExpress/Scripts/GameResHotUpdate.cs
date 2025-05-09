﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using YooAsset;
using Framework.Event;

namespace Framework.YooAssetExpress
{
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEngine.UI;

    [CustomEditor(typeof(GameResHotUpdate))]
    class GameResHotUpdateInspector : Editor
    {
        GUIStyle _labelStyle;
        GUIStyle labelStyle { get => _labelStyle ??= new GUIStyle(EditorStyles.label) { wordWrap = true, richText = true, stretchHeight = true }; }

        GameResHotUpdate my => (GameResHotUpdate)target;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            //Undo.RecordObject(target, "");
            //Undo.RecordObject(this, "");

            if (my)
            {
                EditorGUILayout.Space();
                EditorGUILayout.BeginVertical("box");

                EditorGUILayout.LabelField(@$"<b>服务器资源地址</b>
  地址格式：""资源下载服务器地址/资源在服务器上的根路径/当前平台/游戏版本""
  完整地址：{my.GetHostServerURL()}", labelStyle);
                //              EditorGUILayout.SelectableLabel(@$"<b>服务器资源地址</b>
                //地址格式：""资源下载服务器地址/资源在服务器上的根路径/当前平台/游戏版本""
                //完整地址：{my.GetHostServerURL()}
                //", labelStyle);
                if (GUILayout.Button("打开链接"))
                {
                    Application.OpenURL(my.GetHostServerURL());
                }

                EditorGUILayout.EndVertical();
            }

            //if (GUI.changed)
            //{
            //     EditorUtility.SetDirty(target);
            //}
        }
    }
#endif

    public class GameResHotUpdate : MonoBehaviour
    {
        [Header("资源下载服务器地址")]
        [TextArea]
        //string hostServerIP = "http://10.0.2.2"; //安卓模拟器地址
        //string hostServerIP = "http://127.0.0.1";
        public string hostServerIP = "http://10.0.0.29";
        [Header("资源在服务器上的根路径")]
        [TextArea]
        public string resPath = "CDN";      // 资源在服务器上的根路径
        [Header("版本号路径")]
        public string gameVersion = "v1.0"; // 要更新的版本号
        // 资源包名，可能会有多个资源包
        [Header("资源包名，可能会有多个资源包")]
        public string packageName = "DefaultPackage";

        [Header("自动开始热更新流程")]
        public bool autoStartHotUpdate = true;

        /// <summary>
        /// 资源系统模式
        /// </summary>
        [Header("资源系统模式")]
        public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;

        [Header("需要动态加载的热更新 dll 文件，例如要加载\r\n“Assembly-CSharp.dll.bytes”填\r\n“Assembly-CSharp.dll”或者\r\n“Assets/HotUpdateAssemblies/Use/Assembly-CSharp.dll.bytes”")]
        public List<string> loadHotUpdateDlls = new List<string>() { "Game.HotUpdate.dll" };

        //[Header("最大尝试下载次数")]
        //[SerializeField] protected int maxTryDownloadNum = 3;
        //[Header("已尝试下载次数")]
        //[SerializeField] protected int tryDownloadNum = 0;

        //protected EDefaultBuildPipeline buildPipeline = EDefaultBuildPipeline.BuiltinBuildPipeline;

        /// <summary>
        /// 资源包的版本信息
        /// </summary>
        public string PackageVersion { set; get; }

        /// <summary>
        /// 下载器
        /// </summary>
        public ResourceDownloaderOperation Downloader { set; get; }

        EventGroup _eventGroup = new EventGroup();

        protected virtual void Awake()
        {
            // 注册监听事件
            _eventGroup.AddListener<UserEventDefine.UserTryInitialize>(OnHandleEventMessage);
            _eventGroup.AddListener<UserEventDefine.UserBeginDownloadWebFiles>(OnHandleEventMessage);
            _eventGroup.AddListener<UserEventDefine.UserTryUpdatePackageVersion>(OnHandleEventMessage);
            _eventGroup.AddListener<UserEventDefine.UserTryUpdatePatchManifest>(OnHandleEventMessage);
            _eventGroup.AddListener<UserEventDefine.UserTryDownloadWebFiles>(OnHandleEventMessage);

            ResourcesManager.SetHandler<YooAssetResourcesHandler>();
            ResourcesManager.Initialize();
            //YooAssets.Initialize();
            YooAssets.SetOperationSystemMaxTimeSlice(30);

        }

        protected virtual void Start()
        {
            if (autoStartHotUpdate)
                StartHotUpdate();
        }

        protected virtual void OnDestroy()
        {
            _eventGroup.Clear();
        }

        #region 资源热更流程
        /// <summary>
        /// 处理事件
        /// </summary>
        /// <param name="message"></param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void OnHandleEventMessage(IEventMessage message)
        {
            if (message is UserEventDefine.UserTryInitialize)
            {
                PatchEventDefine.PatchStatesChange.SendEventMessage("初始化资源包！");
                StartHotUpdate();
            }
            else if (message is UserEventDefine.UserBeginDownloadWebFiles)
            {
                PatchEventDefine.PatchStatesChange.SendEventMessage("开始下载补丁文件！");
                StartCoroutine(BeginDownload());
            }
            else if (message is UserEventDefine.UserTryUpdatePackageVersion)
            {
                PatchEventDefine.PatchStatesChange.SendEventMessage("获取最新的资源版本 !");
                StartCoroutine(UpdatePackageVersion());
            }
            else if (message is UserEventDefine.UserTryUpdatePatchManifest)
            {
                PatchEventDefine.PatchStatesChange.SendEventMessage("更新资源清单！");
                StartCoroutine(UpdateManifest());
            }
            else if (message is UserEventDefine.UserTryDownloadWebFiles)
            {
                PatchEventDefine.PatchStatesChange.SendEventMessage("创建补丁下载器！");
                StartCoroutine(CreateDownloader());
            }
            else
            {
                throw new System.NotImplementedException($"{message.GetType()}");
            }
        }

        /// <summary>
        /// 开始热更新流程
        /// </summary>
        public virtual void StartHotUpdate()
        {
            StopAllCoroutines();
            StartCoroutine(InitPackage());
        }

        // 初始化资源包
        protected IEnumerator InitPackage()
        {
            //yield return new WaitForSeconds(1);

            EPlayMode _playMode = PlayMode;

            Debug.Log("------------------------------------初始化资源包------------------------------------");

            // 创建默认的资源包
            var package = YooAssets.TryGetPackage(packageName);
            if (package == null)
            {
                // 没有则创建
                package = YooAssets.CreatePackage(packageName);
                YooAssets.SetDefaultPackage(package);
            }

            InitializationOperation initializationOperation = null;
            switch (_playMode)
            {
                // 编辑器下的模拟模式
                case EPlayMode.EditorSimulateMode:
                    {
                        //var simulateBuildResult = EditorSimulateModeHelper.SimulateBuild(buildPipeline, packageName);
                        //var createParameters = new EditorSimulateModeParameters();
                        //createParameters.EditorFileSystemParameters = FileSystemParameters.CreateDefaultEditorFileSystemParameters(simulateBuildResult);
                        //initializationOperation = package.InitializeAsync(createParameters);

                        var buildResult = EditorSimulateModeHelper.SimulateBuild(packageName);
                        var packageRoot = buildResult.PackageRootDirectory;
                        var createParameters = new EditorSimulateModeParameters();
                        createParameters.EditorFileSystemParameters = FileSystemParameters.CreateDefaultEditorFileSystemParameters(packageRoot);
                        initializationOperation = package.InitializeAsync(createParameters);
                    }
                    break;

                // 单机运行模式
                case EPlayMode.OfflinePlayMode:
                    {
                        var createParameters = new OfflinePlayModeParameters();
                        createParameters.BuildinFileSystemParameters = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
                        initializationOperation = package.InitializeAsync(createParameters);
                    }
                    break;

                // 联机运行模式
                case EPlayMode.HostPlayMode:
                    {
                        string defaultHostServer = GetHostServerURL();
                        string fallbackHostServer = GetHostServerURL();
                        IRemoteServices remoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
                        var createParameters = new HostPlayModeParameters();
                        createParameters.BuildinFileSystemParameters = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
                        createParameters.CacheFileSystemParameters = FileSystemParameters.CreateDefaultCacheFileSystemParameters(remoteServices);
                        initializationOperation = package.InitializeAsync(createParameters);
                    }
                    break;
                // WebGL运行模式
                case EPlayMode.WebPlayMode:
                    {
                        //var createParameters = new WebPlayModeParameters();
                        //createParameters.WebFileSystemParameters = FileSystemParameters.CreateDefaultWebFileSystemParameters();
                        //initializationOperation = package.InitializeAsync(createParameters);

#if UNITY_WEBGL && WEIXINMINIGAME && !UNITY_EDITOR
                        var createParameters = new WebPlayModeParameters();
			            string defaultHostServer = GetHostServerURL();
                        string fallbackHostServer = GetHostServerURL();
                        string packageRoot = $"{WeChatWASM.WX.env.USER_DATA_PATH}/__GAME_FILE_CACHE"; //注意：如果有子目录，请修改此处！
                        IRemoteServices remoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
                        createParameters.WebServerFileSystemParameters = WechatFileSystemCreater.CreateFileSystemParameters(packageRoot, remoteServices);
                        initializationOperation = package.InitializeAsync(createParameters);
#else
                        var createParameters = new WebPlayModeParameters();
                        createParameters.WebServerFileSystemParameters = FileSystemParameters.CreateDefaultWebServerFileSystemParameters();
                        initializationOperation = package.InitializeAsync(createParameters);
#endif
                    }
                    break;
                default:
                    break;
            }

            yield return initializationOperation;

            // 初始化成功开始更新版本
            if (package.InitializeStatus == EOperationStatus.Succeed)
            {
                yield return UpdatePackageVersion();
            }
            else
            {
                Debug.LogError($"创建默认的资源包失败：{initializationOperation.Error}");

                // 尝试再次初始化
                //yield return InitPackage();
                PatchEventDefine.InitializeFailed.SendEventMessage();
            }
        }

        // 获取资源更新版本
        protected IEnumerator UpdatePackageVersion()
        {
            Debug.Log("------------------------------------获取最新的资源版本 !------------------------------------");

            //yield return new WaitForSecondsRealtime(0.5f);

            var package = YooAssets.GetPackage(packageName);
            var operation = package.RequestPackageVersionAsync();
            yield return operation;

            if (operation.Status == EOperationStatus.Succeed)
            {
                PackageVersion = operation.PackageVersion;

                yield return UpdateManifest();
            }
            else
            {
                Debug.LogError($"获取资源更新版本失败：{operation.Error}");

                // 用户尝试再次更新静态版本
                //yield return UpdatePackageVersion();
                PatchEventDefine.PackageVersionUpdateFailed.SendEventMessage();
            }
        }

        // 更新资源清单！
        protected IEnumerator UpdateManifest()
        {
            Debug.Log("------------------------------------更新资源清单！------------------------------------");

            //yield return new WaitForSecondsRealtime(0.5f);

            var package = YooAssets.GetPackage(packageName);
            var operation = package.UpdatePackageManifestAsync(PackageVersion);
            yield return operation;

            if (operation.Status == EOperationStatus.Succeed)
            {
                yield return CreateDownloader();
            }
            else
            {
                Debug.LogError($"创建补丁下载器失败：{operation.Error}");

                // 用户尝试再次更新更新补丁清单
                //yield return UpdateManifest();
                PatchEventDefine.PatchManifestUpdateFailed.SendEventMessage();
            }
        }

        // 创建补丁下载器！
        protected IEnumerator CreateDownloader()
        {
            Debug.Log("------------------------------------创建补丁下载器！------------------------------------");

            //yield return new WaitForSecondsRealtime(0.5f);

            //tryDownloadNum++;

            int downloadingMaxNum = 10;
            int failedTryAgain = 3;
            Downloader = YooAssets.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);
            //var package = YooAssets.GetPackage(packageName);
            //Downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);

            if (Downloader.TotalDownloadCount == 0)
            {
                Debug.Log("没有找到任何下载文件！");

                HotUpdateDone();
            }
            else
            {
                //Debug.Log($"总共 {Downloader.TotalDownloadCount} 个文件需要下载!");

                //// 处理更新信息
                //// 注意：开发者需要在下载前检测磁盘空间不足
                //string totalSizeMB = ToMB(Downloader.TotalDownloadBytes);
                //Debug.Log($"发现更新补丁文件，总数 {Downloader.TotalDownloadCount} 个总大小 {totalSizeMB}MB");

                //yield return BeginDownload();

                int totalDownloadCount = Downloader.TotalDownloadCount;
                long totalDownloadBytes = Downloader.TotalDownloadBytes;
                PatchEventDefine.FoundUpdateFiles.SendEventMessage(totalDownloadCount, totalDownloadBytes);
            }

            yield break;
        }

        // 开始处理下载
        protected IEnumerator BeginDownload()
        {
            // 开始下载
            //Downloader.OnStartDownloadFileCallback = (string fileName, long sizeBytes) =>
            //{
            //    Log.Green($"开始下载资源：{fileName}  大小 {ToMB(sizeBytes)}MB");
            //};

            // 下载失败
            //Downloader.OnDownloadErrorCallback = (fileName, error) =>
            Downloader.DownloadErrorCallback = (data) =>
            {
                //Log.Error($"下载文件失败： {fileName}\r\n{error}");

                //if (tryDownloadNum >= maxTryDownloadNum)
                //{
                //    Log.Warning($"已超过最大尝试下载次数： {maxTryDownloadNum}\r\n请检查网络！");
                //    return;
                //}

                //Debug.Log("------------------------------------尝试再次下载网络文件！------------------------------------");
                //StartCoroutine(CreateDownloader());
                PatchEventDefine.WebFileDownloadFailed.SendEventMessage(data.FileName, data.ErrorInfo);
            };

            // 下载进度
            //Downloader.OnDownloadProgressCallback = PatchEventDefine.DownloadProgressUpdate.SendEventMessage;
            Downloader.DownloadUpdateCallback += (data) =>
            {
                PatchEventDefine.DownloadProgressUpdate.SendEventMessage(data.TotalDownloadCount, data.CurrentDownloadCount, data.TotalDownloadBytes, data.CurrentDownloadBytes);
            };

            Downloader.BeginDownload();
            yield return Downloader;

            if (Downloader.Status != EOperationStatus.Succeed)
            {
                yield break;
            }

            //Log.Green("资源更新完毕！！！");

            DownloadOver();
        }

        // 下载完毕
        protected void DownloadOver()
        {
            Debug.Log("------------------------------------下载完毕------------------------------------");

            ClearCache();
        }

        // 清理未使用的缓存文件
        protected void ClearCache()
        {
            Debug.Log("清理未使用的缓存文件");

            var package = YooAssets.GetPackage(packageName);
            //var operation = package.ClearUnusedBundleFilesAsync();
            var operation = package.ClearCacheFilesAsync(EFileClearMode.ClearUnusedBundleFiles);
            operation.Completed += Operation_Completed;
        }

        // 流程更新完毕
        protected void Operation_Completed(AsyncOperationBase obj)
        {
            HotUpdateDone();
        }

        protected void HotUpdateDone()
        {

            Debug.Log("------------------------------------热更新完毕------------------------------------");

            //LoadDLL.LoadMetadataForAOTAssemblies();
            // 加载程序集
#if !UNITY_EDITOR
            StartCoroutine(LoadDllFuncCoroutine(PatchEventDefine.UpdateDone.SendEventMessage));
#else
            PatchEventDefine.UpdateDone.SendEventMessage();
#endif
        }

        #endregion

        /// <summary>
        ///  加载 dll 程序集
        /// </summary>
        private IEnumerator LoadDllFuncCoroutine(Action loadedAction)
        {
            if (!isLoadDll) yield break;
            isLoadDll = false;

            //LoadDllFunc("Assembly-CSharp.dll.bytes");
            //LoadDllFunc("Assets/HotUpdateAssemblies/Use/Assembly-CSharp.dll.bytes");

            for (int i = 0; i < loadHotUpdateDlls.Count; i++)
            {
                string dllName = loadHotUpdateDlls[i];
                yield return LoadDllFuncCoroutine(dllName);
            }

            loadedAction?.Invoke();
        }
        /// <summary>
        ///  加载 dll 程序集
        /// </summary>
        private IEnumerator LoadDllFuncCoroutine(string dllLocation)
        {
            Log.Yellow($"开始加载 dll 程序集；{dllLocation}");

            //var aoh = YooAssets.LoadAssetSync<TextAsset>(dllLocation);// WebGL 不支持同步加载
            var aoh = YooAssets.LoadAssetAsync<TextAsset>(dllLocation);
            //aoh.WaitForAsyncComplete();
            yield return aoh;

            Debug.Log($"处理器：{aoh}");
            var assetInfo = aoh.GetAssetInfo();
            if (assetInfo != null)
                Debug.Log($"资产信息，Address：{assetInfo.Address}，AssetPath：{assetInfo.AssetPath}，AssetType：{assetInfo.AssetType}");

            TextAsset csDll = aoh?.GetAssetObject<TextAsset>();

            Debug.Log($"转换为文本资源：{csDll != null}");

            byte[] dllDatas = csDll ? csDll.bytes : null;

            //Debug.Log($"资源元数据：{dllDatas}，大小：{dllDatas.Length}，{location}");

            if (dllDatas != null)
            {
                Debug.Log($"开始加载程序集 ---》{dllLocation}《---");
                System.Reflection.Assembly.Load(dllDatas);
                Log.Striking($"加载程序集 ---》{dllLocation}《--- 完毕");
            }
            else
            {
                Log.Error($"不存在程序集 ---》{dllLocation}《---");
            }
        }
        bool isLoadDll = true;


        /// <summary>
        /// 获取资源服务器地址
        /// </summary>
        public virtual string GetHostServerURL()
        {

            //#if UNITY_EDITOR
            //            if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.Android)
            //                return $"{hostServerIP}/CDN/Android/{gameVersion}";
            //            else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.iOS)
            //                return $"{hostServerIP}/CDN/IPhone/{gameVersion}";
            //            else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.WebGL)
            //                return $"{hostServerIP}/CDN/WebGL/{gameVersion}";
            //            else
            //                return $"{hostServerIP}/CDN/PC/{gameVersion}";
            //#else
            //			if (Application.platform == RuntimePlatform.Android)
            //				return $"{hostServerIP}/CDN/Android/{gameVersion}";
            //			else if (Application.platform == RuntimePlatform.IPhonePlayer)
            //				return $"{hostServerIP}/CDN/IPhone/{gameVersion}";
            //			else if (Application.platform == RuntimePlatform.WebGLPlayer)
            //				return $"{hostServerIP}/CDN/WebGL/{gameVersion}";
            //			else
            //				return $"{hostServerIP}/CDN/PC/{gameVersion}";
            //#endif


            //return $"{hostServerIP}/{resPath}/{PlatformUtility.platform}/{gameVersion}";
            return Path.Combine(hostServerIP, resPath, PlatformUtility.platform.ToString(), gameVersion);
        }

    }
}
