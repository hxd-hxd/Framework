using System;
using System.Collections;
using System.Collections.Generic;

namespace Framework.Core.Network
{
    /// <summary>
    /// 协议，由头和体组成的完整协议
    /// </summary>
    public class Protocol<HeadT, MsgT> : IProtocol<HeadT, MsgT>
        where HeadT : IHeadHandle
        where MsgT : IMsgHandle
    {
        // 协议包头
        private HeadT _head;
        // 协议包体
        private MsgT _msg;

        public Protocol()
        {

        }
        public Protocol(HeadT head, MsgT msg)
        {
            _head = head;
            _msg = msg;
        }

        public HeadT head
        {
            get { return _head; }
            set { _head = value; }
        }

        public MsgT msg
        {
            get { return _msg; }
            set { _msg = value; }
        }

        IHeadHandle IProtocol.head { get => _head; set => _head = (HeadT)value; }
        IMsgHandle IProtocol.msg { get => _msg; set => _msg = (MsgT)value; }

        public int length => head.buffer.GetReadableBytesLength() + msg.buffer.GetReadableBytesLength();

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

        public void Clear()
        {
            head?.Clear();
            msg?.Clear();
        }

        public void Reset()
        {
            head?.Reset();
            msg?.Reset();
        }
    }
}