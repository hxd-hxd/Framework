
namespace Framework.Core
{
    /// <summary>
    /// 日志
    /// </summary>
    public class Log
    {
        internal static ILog _logger;
        internal static bool _enableLog = true;

        public static bool enableLog { get => _enableLog; set => _enableLog = value; }

        public static void SetLogger(ILog logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 消息日志
        /// </summary>
        /// <param name="msg"></param>
        public static void Info(object msg)
        {
            if (!enableLog) return;
            _logger?.Info(msg);
        }

        /// <summary>
        /// 警告日志
        /// </summary>
        /// <param name="msg"></param>
        public static void Warning(object msg)
        {
            if (!enableLog) return;
            _logger?.Warning(msg);
        }

        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="msg"></param>
        public static void Error(object msg)
        {
            if (!enableLog) return;
            _logger?.Error(msg);
        }
    }
}
