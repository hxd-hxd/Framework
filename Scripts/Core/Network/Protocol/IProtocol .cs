using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Framework.Core.Network
{

    /// <summary>
    /// 协议接口
    /// </summary>
    public interface IProtocol
    {
        /// <summary>构成协议消息的整体长度</summary>
        int length { get; }

        /// <summary>协议包头</summary>
        IHeadHandle head { get; set; }

        /// <summary>协议包体</summary>
        IMsgHandle msg { get; set; }

        /// <summary>
        /// 获取完整的协议数据包
        /// </summary>
        /// <returns></returns>
        byte[] GetDatas();
        /// <summary>
        /// 获取完整的协议数据包
        /// </summary>
        /// <returns></returns>
        ArraySegment<byte> GetDataArraySegment();

        /// <summary>重置，用于消息内部状态恢复到初始</summary>
        void Reset();

        /// <summary>清除</summary>
        void Clear();
    }

    /// <summary>
    /// 协议接口
    /// </summary>
    public interface IProtocol<THead, TMsg> : IProtocol
        where THead : IHeadHandle
        where TMsg : IMsgHandle
    {
        /// <summary>协议包头</summary>
        new THead head { get; set; }

        /// <summary>协议包体</summary>
        new TMsg msg { get; set; }

    }
}