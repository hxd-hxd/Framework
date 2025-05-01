using System;
using System.Collections;
using System.Collections.Generic;

namespace Framework.Core.Network
{
    /// <summary>
    /// 协议，由头和体组成的完整协议
    /// </summary>
    public class Protocol : IProtocol
    {
        // 协议包头
        private IHeadHandle _head;
        // 协议包体
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
            // 合并为完整的消息
            head.buffer.Write(msg.buffer);

            return head.buffer.ToArraySegment();
        }

        public byte[] GetDatas()
        {
            // 合并为完整的消息
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