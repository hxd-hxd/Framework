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
    /// 2������ <see cref="DefaultHeadHandle.ReadHandle"/> �������Э��ͷ����
    ///    ���� <see cref="DefaultHeadHandle.WriteHandle"/> ��ȡЭ��ͷ
    /// 3�����ͨ�� <see cref="DefaultHeadHandle.msgLength"/> ��ȡ��������Ϣ����
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
            // ����Ϣ��ĳ���д�뵽��Ϣͷ��
            buffer.Write(0, msgLength);

            // ����ע�⣬��Ϊͷ��λ�úʹ�С���ǹ̶��ģ�һ��Ҫ�޸Ļ�������д������
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
                if (buffer.GetReadableBytesLength() < length) throw new Exception($"���ݳ��Ȳ��� {length}");

                msgLength = buffer.ReadInt();
                result = msgLength;
                return true;
            });
        }
    }
}