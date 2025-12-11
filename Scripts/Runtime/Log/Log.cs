// -------------------------
// 创建日期：2022/11/3 15:12:53
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class Log
    {
        /// <summary>
        /// 日志模式
        /// </summary>
        public enum LogMode
        {
            //None = 0,
            /// <summary>
            /// 使用正常日志
            /// </summary>
            Normal,
            /// <summary>
            /// 不引发警告、异常等，使用普通日志但以颜色标识
            /// </summary>
            NoIncur,
        }

        public static LogMode mode = LogMode.NoIncur;

        public static void Info(object msg)
        {
            Debug.Log(msg);
        }

        public static void Client(object msg)
        {
            Debug.Log($"Client：{msg}");
        }
        public static void Server(object msg)
        {
            Debug.Log($"Server：{msg}");
        }


        public static void Info(object msg, string color)
        {
            Debug.Log($"<color={color}>{msg}</color>");
        }
        public static void Client(object msg, string color)
        {
            Debug.Log($"<color={color}>Client：{msg}</color>");
        }
        public static void Server(object msg, string color)
        {
            Debug.Log($"<color={color}>Server：{msg}</color>");
        }


        public static void Green(object msg)
        {
            Info(msg, "green");
        }
        public static void Yellow(object msg)
        {
            Info(msg, "yellow");
        }
        public static void Red(object msg)
        {
            Info(msg, "red");
        }

        /// <summary>
        /// 醒目
        /// </summary>
        /// <param name="msg"></param>
        public static void Striking(object msg)
        {
            Striking(msg, mode);
        }
        /// <summary>
        /// 醒目
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="_mode"></param>
        public static void Striking(object msg, LogMode _mode)
        {
            switch (_mode)
            {
                case LogMode.Normal:
                    Info(msg, "#32FF00");
                    break;
                case LogMode.NoIncur:
                    Info($"Striking：{msg}", "#32FF00");
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="msg"></param>
        public static void Warning(object msg)
        {
            Warning(msg, mode);
        }
        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="_mode"></param>
        public static void Warning(object msg, LogMode _mode)
        {
            switch (_mode)
            {
                case LogMode.Normal:
                    Debug.LogWarning(msg);
                    break;
                case LogMode.NoIncur:
                    //Debuger($"Warning：\r\n\t{msg}", "yellow");
                    Info($"Warning：{msg}", "#FDB30B");
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="msg"></param>
        public static void Error(object msg)
        {
            Error(msg, mode);
        }
        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="_mode"></param>
        public static void Error(object msg, LogMode _mode)
        {
            switch (_mode)
            {
                case LogMode.Normal:
                    Debug.LogError(msg);
                    break;
                case LogMode.NoIncur:
                    //Debuger($"Error：\t{msg}", "red");
                    ErrorNoIncur(msg);
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="msg"></param>
        public static void ErrorNoIncur(object msg)
        {
            Debug.LogFormat("<color=red>Error：{0}</color>", msg);
        }
    }
}