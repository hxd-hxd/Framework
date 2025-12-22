using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;

namespace Framework.LogSystem
{
    // 日志文件处理
    public partial class LogInfo
    {
        const string _logFileName = "Log Last Run.txt";
        static string _logFileUniversalDir = "Log Info";// 通用目录
        // 带日期时间的名称
        static string _logFileName_DateTimeFormat = $"Log {DateTime.Now.ToDayFrontTimeText("-")}.txt";

        /// <summary>
        /// 编辑器
        /// </summary>
        public static string logFilePath_Eitor
        {
            get
            {
                string _path = _GetLogFilePath_SetRoot(Path.GetDirectoryName(Application.dataPath));
                return _path;
            }
        }

        /// <summary>
        /// 移动平台
        /// </summary>
        public static string logFilePath_Mobile
        {
            get
            {
                string _path = _GetLogFilePath_SetRoot(Application.persistentDataPath);
                return _path;
            }
        }

        /// <summary>
        /// 其他标准通用平台
        /// </summary>
        public static string logFilePath_Standard
        {
            get
            {
                string _path = _GetLogFilePath_SetRoot(Path.GetDirectoryName(Application.dataPath));
                return _path;
            }
        }

        /// <summary>
        /// 日志文件目录
        /// </summary>
        public static string logFilePath
        {
            get
            {
                string _path = null;
#if UNITY_EDITOR
                _path = logFilePath_Eitor;
#elif UNITY_ANDROID || UNITY_IPHONE || UNITY_WEBGL
            _path = logFilePath_Mobile;
#else
            _path = logFilePath_Standard;
#endif
                return _path;
            }
        }

        /// <summary>
        /// 日志文件路径 - 最后一次运行
        /// </summary>
        public static string logFilePath_LastRun
        {
            get
            {
                string _path = Path.Combine(logFilePath, _logFileName);
                if (!File.Exists(_path))
                {
                    File.Create(_path).Dispose();
                }
                return _path;
            }
        }

        /// <summary>
        /// 日志文件路径 - 每天运行的日期
        /// </summary>
        public static string logFilePath_DateTime
        {
            get
            {
                string _path = Path.Combine(logFilePath, _logFileName_DateTimeFormat);
                if (!File.Exists(_path))
                {
                    File.Create(_path).Dispose();
                }
                return _path;
            }
        }

        // 获取日志文件存放目录
        static string _GetLogFilePath_SetRoot(string rootDir)
        {
            string pDir = Path.Combine(rootDir, _logFileUniversalDir);
            if (!Directory.Exists(pDir))
            {
                Directory.CreateDirectory(pDir);
            }
            return pDir;
            //string path = _GetLogFilePath_SetRoot(rootDir, _logFileName_DateTimeFormat);
            //return path;
        }

        // 获取日志文件
        static string _GetLogFilePath_SetRoot(string rootDir, string logFileName)
        {
            string pDir = _GetLogFilePath_SetRoot(rootDir);
            string path = Path.Combine(pDir, logFileName);
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
            }
            return path;
        }

        /// <summary>
        /// 将当前所有日志转换成文件格式的文本
        /// </summary>
        /// <returns></returns>
        public static string ToFileFormatTextAll()
        {
            if (logInfoCount <= 0) return null;

            var sb = TypePool.root.Get<StringBuilder>();
            //sb.Append(logInfos[0].ToText());
            for (int i = 0; i < logInfos.Count; i++)
            {
                var info = logInfos[i];
                sb.Append(info.ToFileFormat());
            }

            string t = sb.ToString();
            TypePool.root.Return(sb);
            return t;
        }

        /// <summary>
        /// 添加日志到文件
        /// </summary>
        /// <param name="info"></param>
        public static void AddLogToFile(LogInfo info)
        {
            if (info != null)
            {
                string f = info.ToFileFormat();
                File.AppendAllText(logFilePath_DateTime, f);
                File.AppendAllText(logFilePath_LastRun, f);
            }
        }

        /// <summary>
        /// 清除最后一次运行的日志到文件
        /// </summary>
        /// <param name="info"></param>
        public static void ClearLastRun()
        {
            if (File.Exists(logFilePath_LastRun))
            {
                File.WriteAllText(logFilePath_LastRun, null);
            }
        }

        /// <summary>
        /// 转换成文件格式
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private string ToFileFormat()
        {
            // 分隔符
            // ========================================================================
            return $@"
{ToText()}";
        }
    }
}