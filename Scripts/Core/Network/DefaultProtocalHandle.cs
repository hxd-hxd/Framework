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
    public class DefaultProtocalHandle : ProtocalHandleBase
    {
        public DefaultProtocalHandle() : this(null)
        {

        }
        public DefaultProtocalHandle(Socket socket)
        {
            _socket = socket;
            _writeBuffer = new ByteBuffer(1024);
            _readBuffer = new ByteBuffer(1024);
            _headHandle = new DefaultHeadHandle();
            _msgHandle = new DefaultMsgHandle();
        }

        protected override bool SendAbstract(object msg)
        {
            return msgHandle.Get(_writeBuffer, msg);
        }
    }
}