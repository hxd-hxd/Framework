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
    /// 2、调用 <see cref="DefaultHeadHandle.ReadHandle"/> 处理解析协议头数据
    ///    调用 <see cref="DefaultHeadHandle.WriteHandle"/> 获取协议头
    /// 3、最后通过 <see cref="DefaultHeadHandle.msgLength"/> 获取解析的消息长度
    /// </code>
    /// </summary>
    public class DefaultHeadHandle : HeadHandleBase
    {

        public override int length
        {
            get => 4;
        }

        public override void WriteHandle()
        {
            // 将消息体的长度写入到消息头中
            buffer.Write(0, msgLength);

            // 这里注意，因为头的位置和大小都是固定的，一定要修改缓冲区的写入索引
            buffer.SetWriteIndex(length);

        }

        public override bool WriteHandle(ByteBuffer buffer, object msg)
        {
            //if (msg is int msgLength)
            //{
            //    buffer.Write(0, msgLength);
            //    return true;
            //}

            return false;
        }

        public override void ReadHandle(ByteBuffer buffer)
        {
            ReadHandle(buffer, (out object result) =>
            {
                if (buffer == null) throw new ArgumentNullException(nameof(buffer));
                if (buffer.GetReadableBytesLength() < length) throw new Exception($"数据长度不够 {length}");

                msgLength = buffer.ReadInt();
                result = msgLength;
                return true;
            });
        }
    }
}