using System;
using System.Collections;
using System.Collections.Generic;

namespace Framework.Core.Network
{
    /// <summary>
    /// 协议，由头和体组成的完整协议
    /// </summary>
    public class Protocol<THead, TMsg> : IProtocol<THead, TMsg>
        where THead : IHeadHandle
        where TMsg : IMsgHandle
    {
        // 协议包头
        private THead _head;
        // 协议包体
        private TMsg _msg;

        public Protocol()
        {

        }
        public Protocol(THead head, TMsg msg)
        {
            _head = head;
            _msg = msg;
        }

        //THead IProtocol<THead, TMsg>.head { get => (THead)head; set => head = value; }
        //TMsg IProtocol<THead, TMsg>.msg { get => (TMsg)msg; set => msg = value; }

        public THead head
        {
            get { return _head; }
            set { _head = value; }
        }

        public TMsg msg
        {
            get { return _msg; }
            set { _msg = value; }
        }

        IHeadHandle IProtocol.head { get => _head; set => _head = (THead)value; }
        IMsgHandle IProtocol.msg { get => _msg; set => _msg = (TMsg)value; }

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