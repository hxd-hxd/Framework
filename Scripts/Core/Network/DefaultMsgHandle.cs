using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Framework.Core.Network
{

    /// <summary>
    /// 默认协议消息体处理，只支持 string
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
                if (buffer.ReadableBytesLength() < 1) throw new Exception($"数据长度不够");

                result = buffer.ReadString();
                return true;
            });
        }
    }
}