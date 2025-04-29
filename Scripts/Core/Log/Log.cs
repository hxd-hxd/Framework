using System.Collections;
using System.Collections.Generic;

namespace Framework.Core
{
    /// <summary>
    /// ��־
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
        /// ��Ϣ��־
        /// </summary>
        /// <param name="msg"></param>
        public static void Info(object msg)
        {
            if (!enableLog) return;
            _logger?.Info(msg);
        }
        /// <summary>
        /// ������־
        /// </summary>
        /// <param name="msg"></param>
        public static void Warning(object msg)
        {
            if (!enableLog) return;
            _logger?.Warning(msg);
        }
        /// <summary>
        /// ������־
        /// </summary>
        /// <param name="msg"></param>
        public static void Error(object msg)
        {
            if (!enableLog) return;
            _logger?.Error(msg);
        }
    }
}
