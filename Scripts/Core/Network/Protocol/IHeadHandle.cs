using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Framework.Core.Network
{

    /// <summary>
    /// Э����Ϣͷ����ӿ�
    /// </summary>
    public interface IHeadHandle : IMsgHandle
    {
        /// <summary>��ʾ��Ҫ�����ͷ������</summary>
        new int length { get; }

        /// <summary>�������Ϣ����</summary>
        int msgLength { get; set; }

        /// <summary>д�봦��</summary>
        void WriteHandle();

        ///// <summary>
        ///// ��ͷ������ӵ� <paramref name="buffer"/>
        ///// </summary>
        //void WriteHandle(ByteBuffer buffer, int msgLength);

        ///// <summary>
        ///// ��ͷ������ӵ� <see cref="length"/> ��С�� <paramref name="bytes"/>
        ///// </summary>
        //void WriteHandle(byte[] bytes, int msgLength);

        ///// <summary>
        ///// ��ͷ������ӵ� <see cref="length"/> ��С�� <paramref name="bytes"/>
        ///// </summary>
        //void WriteHandle(ArraySegment<byte> bytes, int msgLength);
    }

}