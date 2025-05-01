using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Framework.Core.Network
{
    /// <summary>
    /// 协议处理接口
    /// </summary>
    public interface IProtocolHandle
    {
        /// <summary>需要由此协议处理的套接字连接</summary>
        public Socket socket { get; set; }

        /// <summary>接收条件事件</summary>
        Func<bool> receiveConditionEvent { get; set; }
        /// <summary>接收停止事件</summary>
        Action<SocketError> receiveStopEvent { get; set; }

    }

}