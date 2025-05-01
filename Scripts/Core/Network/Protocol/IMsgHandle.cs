using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Framework.Core.Network
{

    /// <summary>
    /// 协议消息体处理接口
    /// </summary>
    public interface IMsgHandle : IDisposable
    {
        /// <summary>表示需要处理的长度</summary>
        int length { get; set; }

        /// <summary>表示消息的实际数据长度</summary>
        int dataLength { get; }

        /// <summary>处理完成事件</summary>
        Action<object> HandleCompletedEvent { get; set; }
        /// <summary>处理错误事件</summary>
        Action<object> HandleErrorEvent { get; set; }

        /// <summary>
        /// 内部处理用的 <see cref="ByteBuffer"/> 字节缓冲区
        /// </summary>
        public ByteBuffer buffer { get; }

        /// <summary>读取处理</summary>
        void ReadHandle();
        /// <summary>读取处理，注意实现时应当将此方法作为最终处理</summary>
        void ReadHandle(ByteBuffer buffer);

        /// <summary>
        /// 将消息写入到内部缓冲区
        /// </summary>
        bool WriteHandle(object msg);
        /// <summary>
        /// 将消息写入到缓冲区
        /// </summary>
        bool WriteHandle(ByteBuffer buffer, object msg);

        /// <summary>重置，用于消息内部状态恢复到初始</summary>
        void Reset();

        /// <summary>清除</summary>
        void Clear();
    }

}
