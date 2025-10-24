using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Core.Network;

namespace Framework.GoogleProtobufExpress
{
    /// <summary>
    /// 默认协议，由头和体组成的完整协议
    /// </summary>
    public class ProtobufProtocol : Protocol<DefaultHeadHandle, ProtobufMsgHandle>
    {
        public ProtobufProtocol()
        {
            head = new DefaultHeadHandle();
            msg = new ProtobufMsgHandle();
        }
    }
}