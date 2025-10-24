using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using Framework.Core;
using Framework.Core.Network;
using Google.Protobuf.WellKnownTypes;

namespace Framework.GoogleProtobufExpress
{
    /// <summary>
    /// Google Protocal Buffer Э����Ϣ�崦��
    /// </summary>
    public class ProtobufMsgHandle : MsgHandleBase
    {

        public override bool WriteHandle(ByteBuffer buffer, object msg)
        {
            bool r = true;
            byte[] vs = null;
            if (msg is string strMsg)
            {
                buffer.Write(strMsg);
            }
            else
            if (msg is Any anyMsg)
            {
                vs = ProtobufTools.Serialize(anyMsg);
            }
            else
            if (msg is IMessage iMsg)
            {
                vs = ProtobufTools.SerializeByAny(iMsg);
            }
            else
            {
                r = false;
            }

            if (vs != null)
            {
                int msgLength = vs.Length;
                buffer.Write(msgLength);// д����Ϣ����
                buffer.Write(vs);// д����Ϣ

                //Log.Info($"д����Ϣ��{msg}");
            }

            //length = dataLength;
            return r;
        }

        public override void ReadHandle(ByteBuffer buffer)
        {
            //Handle(buffer.ReadableArraySegment());
            ReadHandle(buffer, (out object result) =>
            {
                result = null;
                if (buffer.GetReadableBytesLength() < 1) return false;

                try
                {
                    buffer.MarkReadIndex();
                    //result = ProtobufTools.DeserializeByAny(buffer.ReadSurplusArraySegment());
                    int msgLength = buffer.ReadInt();
                    result = ProtobufTools.DeserializeByAny(buffer.GetReadArraySegment(msgLength));

                }
                catch (Exception ex)
                {
                    Log.Error($"���� Protobuf ���ͳ���{ex.Message}\r\n��Ϊ�������ַ�������");
                    buffer.ResetReadIndex();
                    result = buffer.ReadString();
                }

                if (buffer.GetReadableBytesLength() < 1) return false;

                return true;
            });
        }
    }
}