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
    /// Google Protocal Buffer 协议消息体处理
    /// </summary>
    public class ProtobufMsgHandle : MsgHandleBase
    {

        public override bool WriteHandle(ByteBuffer buffer, object msg)
        {
            bool r = true;
            if (msg is string strMsg)
            {
                buffer.Write(strMsg);
            }
            else
            if (msg is Any anyMsg)
            {
                var vs = ProtobufTools.Serialize(anyMsg);
                buffer.Write(vs);
            }
            else
            if (msg is IMessage iMsg)
            {
                var vs = ProtobufTools.SerializeByAny(iMsg);
                buffer.Write(vs);
            }
            else
            {
                r = false;
            }

            length = dataLength;
            return r;
        }

        public override void ReadHandle(ByteBuffer buffer)
        {
            //Handle(buffer.ReadableArraySegment());
            ReadHandle(buffer, (out object result) =>
            {
                result = null;
                try
                {
                    buffer.MarkReadIndex();
                    result = ProtobufTools.DeserializeByAny(buffer.ReadSurplusArraySegment());

                }
                catch (Exception ex)
                {
                    Log.Error($"解析 Protobuf 类型出错：{ex.Message}");
                    buffer.ResetReadIndex();
                    result = buffer.ReadString();
                }
                return true;
            });
        }
    }
}