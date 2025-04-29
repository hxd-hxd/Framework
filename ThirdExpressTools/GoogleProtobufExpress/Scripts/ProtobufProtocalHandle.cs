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
    public class ProtobufProtocalHandle : ProtocalHandleBase
    {
        public ProtobufProtocalHandle() : this(null)
        {

        }
        public ProtobufProtocalHandle(Socket socket)
        {
            _socket = socket;
            _writeBuffer = new ByteBuffer(1024);
            _readBuffer = new ByteBuffer(1024);
            _headHandle = new DefaultHeadHandle();
            _msgHandle = new ProtobufMsgHandle();
        }

        protected override bool SendAbstract(object msg)
        {
            return msgHandle.Get(_writeBuffer, msg);
        }
    }
}