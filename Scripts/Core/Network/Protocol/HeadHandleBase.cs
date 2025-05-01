using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Framework.Core.Network
{
    // Э���ͷ����Ҳ����Ϣ

    /// <summary>
    /// Э��ͷ�������
    /// </summary>
    public abstract class HeadHandleBase : MsgHandleBase, IHeadHandle
    {
        int _msgLength;

        /// <summary>��ʾ��Ҫ�����ͷ������</summary>
        public override int length
        {
            get => 4;
        }

        /// <summary>�������Ϣ����</summary>
        public virtual int msgLength { get => _msgLength; protected set => _msgLength = value; }

        int IHeadHandle.msgLength { get => msgLength; set => msgLength = value; }

        //public override abstract void ReadHandle(ByteBuffer buffer);

        //public abstract void WriteHandle(byte[] bytes, int msgLength);

        //public abstract void WriteHandle(ArraySegment<byte> bytes, int msgLength);

        ///// <summary>
        ///// ��ͷ������ӵ� <see cref="length"/> ��С�� <paramref name="bytes"/>
        ///// </summary>
        //public virtual void WriteHandle(byte[] bytes, int msgLength)
        //{
        //    buffer.Write(msgLength);
        //    buffer.ReadBytes(bytes);

        //    buffer.Clear();
        //}

        ///// <summary>
        ///// ��ͷ������ӵ� <see cref="length"/> ��С�� <paramref name="bytes"/>
        ///// </summary>
        //public virtual void WriteHandle(ArraySegment<byte> bytes, int msgLength)
        //{
        //    buffer.Write(msgLength);
        //    buffer.ReadBytes(bytes);

        //    buffer.Clear();
        //}

        ///// <summary>
        ///// ��ͷ������ӵ� <paramref name="buffer"/>
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