using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Framework.Core;
using Framework.Core.Network;

namespace Framework.GoogleProtobufExpress
{
    /// <summary>
    /// Google Protocal Buffer 协议处理
    /// </summary>
    public class ProtobufProtocolHandle : ProtocolHandleBase
    {
        public ProtobufProtocolHandle() : this(null)
        {

        }
        public ProtobufProtocolHandle(Socket socket) : base(socket)
        {
            SetProtocol<ProtobufProtocol>();
        }

    }
}