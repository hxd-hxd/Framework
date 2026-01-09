using System.Collections;
using System.Collections.Generic;

namespace Framework.Core
{
    public interface ILog
    {
        /// <summary>
        /// 消息日志
        /// </summary>
        /// <param name="msg"></param>
        void Info(object msg);

        /// <summary>
        /// 警告日志
        /// </summary>
        /// <param name="msg"></param>
        void Warning(object msg);

        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="msg"></param>
        void Error(object msg);
    }
}
