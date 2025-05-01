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
        int _msgLength;

        /// <summary>表示需要处理的头部长度</summary>
        public override int length
        {
            get => 4;
        }

        /// <summary>具体的消息长度</summary>
        public virtual int msgLength { get => _msgLength; protected set => _msgLength = value; }

        int IHeadHandle.msgLength { get => msgLength; set => msgLength = value; }

        //public override abstract void ReadHandle(ByteBuffer buffer);

        //public abstract void WriteHandle(byte[] bytes, int msgLength);

        //public abstract void WriteHandle(ArraySegment<byte> bytes, int msgLength);

        ///// <summary>
        ///// 将头数据添加到 <see cref="length"/> 大小的 <paramref name="bytes"/>
        ///// </summary>
        //public virtual void WriteHandle(byte[] bytes, int msgLength)
        //{
        //    buffer.Write(msgLength);
        //    buffer.ReadBytes(bytes);

        //    buffer.Clear();
        //}

        ///// <summary>
        ///// 将头数据添加到 <see cref="length"/> 大小的 <paramref name="bytes"/>
        ///// </summary>
        //public virtual void WriteHandle(ArraySegment<byte> bytes, int msgLength)
        //{
        //    buffer.Write(msgLength);
        //    buffer.ReadBytes(bytes);

        //    buffer.Clear();
        //}

        ///// <summary>
        ///// 将头数据添加到 <paramref name="buffer"/>
        ///// </summary>
        //public abstract void WriteHandle(ByteBuffer buffer, int msgLength);

        public abstract void WriteHandle();

        public override void Reset()
        {
            base.Reset();

            msgLength = 0;
        }

        public override void Clear()
        {
            base.Clear();

            msgLength = 0;
        }

    }
}