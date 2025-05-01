using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Framework.Core.Network
{

    /// <summary>
    /// 协议消息头处理接口
    /// </summary>
    public interface IHeadHandle : IMsgHandle
    {
        /// <summary>表示需要处理的头部长度</summary>
        new int length { get; }

        /// <summary>具体的消息长度</summary>
        int msgLength { get; set; }

        /// <summary>写入处理</summary>
        void WriteHandle();

        ///// <summary>
        ///// 将头数据添加到 <paramref name="buffer"/>
        ///// </summary>
        //void WriteHandle(ByteBuffer buffer, int msgLength);

        ///// <summary>
        ///// 将头数据添加到 <see cref="length"/> 大小的 <paramref name="bytes"/>
        ///// </summary>
        //void WriteHandle(byte[] bytes, int msgLength);

        ///// <summary>
        ///// 将头数据添加到 <see cref="length"/> 大小的 <paramref name="bytes"/>
        ///// </summary>
        //void WriteHandle(ArraySegment<byte> bytes, int msgLength);
    }

}