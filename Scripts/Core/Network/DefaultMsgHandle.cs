using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Framework.Core.Network
{

    /// <summary>
    /// Ĭ��Э����Ϣ�崦��ֻ֧�� string
    /// </summary>
    public class DefaultMsgHandle : MsgHandleBase
    {

        public override bool Get(ByteBuffer buffer, object msg)
        {
            if (msg is string strMsg)
            {
                buffer.Write(strMsg);
            }
            else
            {
                return false;
            }

            return true;
        }

        public override void Handle(ByteBuffer buffer)
        {
            Handle(buffer, (out object result) =>
            {
                if (buffer == null) throw new ArgumentNullException(nameof(buffer));
                if (buffer.ReadableBytesLength() < 1) throw new Exception($"���ݳ��Ȳ���");

                result = buffer.ReadString();
                return true;
            });
        }
    }
}