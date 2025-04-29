using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Framework.Core.Network
{
    // 协议包头本身也是消息

    /// <summary>
    /// 协议头处理基类
    /// </summary>
    public abstract class HeadHandleBase : MsgHandleBase, IHeadHandle
    {

        /// <summary>表示需要处理的头部长度</summary>
        public virtual int length
        {
            get => 4;
        }

        /// <summary>具体的消息长度</summary>
        public virtual int msgLength { get; protected set; }

        /// <summary>
        /// 处理 <see cref="length"/> 长度的数组段
        /// </summary>
        public override abstract void Handle(ByteBuffer buffer);

        /// <summary>
        /// 将头数据添加到 <see cref="length"/> 大小的 <paramref name="bytes"/>
        /// </summary>
        public abstract void Get(byte[] bytes, int msgLength);

        /// <summary>
        /// 将头数据添加到 <see cref="length"/> 大小的 <paramref name="bytes"/>
        /// </summary>
        public abstract void Get(ArraySegment<byte> bytes, int msgLength);

        /// <summary>
        /// 将头数据添加到 <paramref name="buffer"/>
        /// </summary>
        public abstract void Get(ByteBuffer buffer, int msgLength);

        public override void ResetMsg()
        {
            base.ResetMsg();

            msgLength = 0;
        }
    }
}