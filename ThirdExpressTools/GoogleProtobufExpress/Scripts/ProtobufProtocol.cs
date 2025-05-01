using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Core.Network;

namespace Framework.GoogleProtobufExpress
{
    /// <summary>
    /// Ĭ��Э�飬��ͷ������ɵ�����Э��
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