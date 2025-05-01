using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core.Network
{
    /// <summary>
    /// 默认协议处理
    /// </summary>
    public class DefaultProtocolHandle : ProtocolHandleBase
    {
        public DefaultProtocolHandle() : this(null)
        {

        }
        public DefaultProtocolHandle(Socket socket) : base(socket) 
        {
            SetProtocol<DefaultProtocol>();
        }

    }
}