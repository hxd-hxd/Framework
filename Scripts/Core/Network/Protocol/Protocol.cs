using System;
using System.Collections;
using System.Collections.Generic;

namespace Framework.Core.Network
{
    /// <summary>
    /// Э�飬��ͷ������ɵ�����Э��
    /// </summary>
    public class Protocol : IProtocol
    {
        // Э���ͷ
        private IHeadHandle _head;
        // Э�����
        private IMsgHandle _msg;

        public Protocol()
        {

        }
        public Protocol(IHeadHandle head, IMsgHandle msg)
        {
            _head = head;
            _msg = msg;
        }

        public IHeadHandle head
        {
            get { return _head; }
            set { _head = value; }
        }

        public IMsgHandle msg
        {
            get { return _msg; }
            set { _msg = value; }
        }

        public int length => head.dataLength + msg.dataLength;

        public ArraySegment<byte> GetDataArraySegment()
        {
            // �ϲ�Ϊ��������Ϣ
            head.buffer.Write(msg.buffer);

            return head.buffer.ToArraySegment();
        }

        public byte[] GetDatas()
        {
            // �ϲ�Ϊ��������Ϣ
            head.buffer.Write(msg.buffer);

            return head.buffer.ToArray();
        }

        public void Reset()
        {
            head?.Reset();
            msg?.Reset();
        }

        public void Clear()
        {
            head?.Clear();
            msg?.Clear();
        }
    }
}