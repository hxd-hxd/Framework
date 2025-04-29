using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Framework.Core.Network
{

    /// <summary>
    /// Ĭ��Э��ͷ����
    /// <code>
    /// ����
    /// ֻ������Ϣ���С�������������Ĺ���
    /// ʹ��
    /// 1����ͨ�� <see cref="DefaultHeadHandle.length"/> ��ȡͷ�����ȶ�Ӧ������
    /// 2������ <see cref="DefaultHeadHandle.Handle"/> �������Э��ͷ����
    ///    ���� <see cref="DefaultHeadHandle.Get"/> ��ȡЭ��ͷ
    /// 3�����ͨ�� <see cref="DefaultHeadHandle.msgLength"/> ��ȡ��������Ϣ����
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
                if (buffer.ReadableBytesLength() < length) throw new Exception($"���ݳ��Ȳ��� {length}");

                msgLength = buffer.ReadInt();
                result = msgLength;
                return true;
            });
        }
    }
}