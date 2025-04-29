using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Framework.Core.Network
{

    /// <summary>
    /// 默认协议头处理
    /// <code>
    /// 规则
    /// 只处理消息体大小，不包含其他的规则
    /// 使用
    /// 1、先通过 <see cref="DefaultHeadHandle.length"/> 获取头部长度对应的数据
    /// 2、调用 <see cref="DefaultHeadHandle.Handle"/> 处理解析协议头数据
    ///    调用 <see cref="DefaultHeadHandle.Get"/> 获取协议头
    /// 3、最后通过 <see cref="DefaultHeadHandle.msgLength"/> 获取解析的消息长度
    /// </code>
    /// </summary>
    public class DefaultHeadHandle : HeadHandleBase
    {

        public override int length
        {
            get => 4;
        }

        public override void Get(byte[] bytes, int msgLength)
        {
            buffer.Write(msgLength);
            buffer.ReadBytes(bytes);

            buffer.Clear();
        }

        public override void Get(ArraySegment<byte> bytes, int msgLength)
        {
            buffer.Write(msgLength);
            buffer.ReadBytes(bytes);

            buffer.Clear();
        }

        public override void Get(ByteBuffer buffer, int msgLength)
        {
            buffer.Write(msgLength);
        }

        public override bool Get(ByteBuffer buffer, object msg)
        {
            if (msg is int msgLength)
            {
                buffer.Write(msgLength);
                return true;
            }

            return false;
        }

        public override void Handle(ByteBuffer buffer)
        {
            Handle(buffer, (out object result) =>
            {
                if (buffer == null) throw new ArgumentNullException(nameof(buffer));
                if (buffer.ReadableBytesLength() < length) throw new Exception($"数据长度不够 {length}");

                msgLength = buffer.ReadInt();
                result = msgLength;
                return true;
            });
        }
    }
}