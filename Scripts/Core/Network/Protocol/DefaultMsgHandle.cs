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

        public override bool WriteHandle(ByteBuffer buffer, object msg)
        {
            bool r = true;
            if (msg is string strMsg)
            {
                buffer.Write(strMsg);
            }
            else
            {
                r = false;
            }

            //length = dataLength;
            return r;
        }

        public override void ReadHandle(ByteBuffer buffer)
        {
            ReadHandle(buffer, (out object result) =>
            {
                result = null;
                if (buffer == null) throw new ArgumentNullException(nameof(buffer));
                //if (buffer.GetReadableBytesLength() < 1) throw new Exception($"���ݳ��Ȳ���");
                if (buffer.GetReadableBytesLength() < 1) return false;

                result = buffer.ReadString();

                if (buffer.GetReadableBytesLength() < 1) return false;

                return true;
            });
        }
    }
}