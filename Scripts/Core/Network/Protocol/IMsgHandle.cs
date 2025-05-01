using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Framework.Core.Network
{

    /// <summary>
    /// Э����Ϣ�崦��ӿ�
    /// </summary>
    public interface IMsgHandle : IDisposable
    {
        /// <summary>��ʾ��Ҫ����ĳ���</summary>
        int length { get; set; }

        /// <summary>��ʾ��Ϣ��ʵ�����ݳ���</summary>
        int dataLength { get; }

        /// <summary>��������¼�</summary>
        Action<object> HandleCompletedEvent { get; set; }
        /// <summary>��������¼�</summary>
        Action<object> HandleErrorEvent { get; set; }

        /// <summary>
        /// �ڲ������õ� <see cref="ByteBuffer"/> �ֽڻ�����
        /// </summary>
        public ByteBuffer buffer { get; }

        /// <summary>��ȡ����</summary>
        void ReadHandle();
        /// <summary>��ȡ����ע��ʵ��ʱӦ�����˷�����Ϊ���մ���</summary>
        void ReadHandle(ByteBuffer buffer);

        /// <summary>
        /// ����Ϣд�뵽�ڲ�������
        /// </summary>
        bool WriteHandle(object msg);
        /// <summary>
        /// ����Ϣд�뵽������
        /// </summary>
        bool WriteHandle(ByteBuffer buffer, object msg);

        /// <summary>���ã�������Ϣ�ڲ�״̬�ָ�����ʼ</summary>
        void Reset();

        /// <summary>���</summary>
        void Clear();
    }

}
