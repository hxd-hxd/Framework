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

        /// <summary>��ʾ��Ҫ�����ͷ������</summary>
        public virtual int length
        {
            get => 4;
        }

        /// <summary>�������Ϣ����</summary>
        public virtual int msgLength { get; protected set; }

        /// <summary>
        /// ���� <see cref="length"/> ���ȵ������
        /// </summary>
        public override abstract void Handle(ByteBuffer buffer);

        /// <summary>
        /// ��ͷ������ӵ� <see cref="length"/> ��С�� <paramref name="bytes"/>
        /// </summary>
        public abstract void Get(byte[] bytes, int msgLength);

        /// <summary>
        /// ��ͷ������ӵ� <see cref="length"/> ��С�� <paramref name="bytes"/>
        /// </summary>
        public abstract void Get(ArraySegment<byte> bytes, int msgLength);

        /// <summary>
        /// ��ͷ������ӵ� <paramref name="buffer"/>
        /// </summary>
        public abstract void Get(ByteBuffer buffer, int msgLength);

        public override void ResetMsg()
        {
            base.ResetMsg();

            msgLength = 0;
        }
    }
}