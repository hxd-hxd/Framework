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

        public override bool Get(ByteBuffer buffer, object msg)
        {
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
                return false;
            }

            return true;
        }

        public override void Handle(ByteBuffer buffer)
        {
            //Handle(buffer.ReadableArraySegment());
            Handle(buffer, (out object result) =>
            {
                result = null;
                try
                {
                    buffer.MarkReadIndex();
                    result = ProtobufTools.DeserializeByAny(buffer.ReadableArraySegment());

                }
                catch (Exception)
                {
                    buffer.ResetReadIndex();
                    result = buffer.ReadString();
                }
                return true;
            });
        }
    }
}