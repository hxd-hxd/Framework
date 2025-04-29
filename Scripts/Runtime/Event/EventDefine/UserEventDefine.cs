
namespace Framework.Event
{
    /// <summary>热更新补丁用户操作事件定义</summary>
    public class UserEventDefine
    {
        /// <summary>
        /// 用户尝试再次初始化资源包
        /// </summary>
        public class UserTryInitialize : IEventMessage
        {
            public static void SendEventMessage()
            {
                var msg = new UserTryInitialize();
                EventCenter.SendType(msg);
            }
        }

        /// <summary>
        /// 用户开始下载网络文件
        /// </summary>
        public class UserBeginDownloadWebFiles : IEventMessage
        {
            public static void SendEventMessage()
            {
                var msg = new UserBeginDownloadWebFiles();
                EventCenter.SendType(msg);
            }
        }

        /// <summary>
        /// 用户尝试再次更新静态版本
        /// </summary>
        public class UserTryUpdatePackageVersion : IEventMessage
        {
            public static void SendEventMessage()
            {
                var msg = new UserTryUpdatePackageVersion();
                EventCenter.SendType(msg);
            }
        }

        /// <summary>
        /// 用户尝试再次更新补丁清单
        /// </summary>
        public class UserTryUpdatePatchManifest : IEventMessage
        {
            public static void SendEventMessage()
            {
                var msg = new UserTryUpdatePatchManifest();
                EventCenter.SendType(msg);
            }
        }

        /// <summary>
        /// 用户尝试再次下载网络文件
        /// </summary>
        public class UserTryDownloadWebFiles : IEventMessage
        {
            public static void SendEventMessage()
            {
                var msg = new UserTryDownloadWebFiles();
                EventCenter.SendType(msg);
            }
        }
    }
}