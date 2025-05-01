using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Framework.Core.Network
{

    /// <summary>
    /// 协议处理基类
    /// <code>
    /// 如何自定义协议：
    /// 协议的包头和包体应当是成对出现的
    /// </code>
    /// </summary>
    public abstract class ProtocolHandleBase : IProtocolHandle
    {
        protected Socket _socket;

        protected IProtocol _readProtocol;// 用于接收的协议
        protected IProtocol _writeProtocol;// 用于发送的协议

        /// <summary>需要由此协议处理的套接字连接</summary>
        public Socket socket { get => _socket; set => _socket = value; }
        /// <summary>写入数据缓冲区</summary>
        protected ByteBuffer writeBuffer { get => _writeProtocol.msg.buffer; }
        /// <summary><see cref="socket"/> 是否处于连接状态</summary>
        public bool isConnected => _socket != null ? _socket.Connected : false;

        /// <summary>接收条件事件</summary>
        public Func<bool> ReceiveConditionEvent;
        /// <summary>接收停止事件</summary>
        public Action<SocketError> ReceiveStopEvent;

        protected ProtocolHandleBase(Socket socket)
        {
            _socket = socket;
        }

        protected static Task _receiveDelayTask = Task.Delay(1);

        protected bool isRuning => ReceiveConditionEvent != null ? ReceiveConditionEvent() : true;

        /// <summary>处理接收数据</summary>
        public virtual void Receive(Socket socket)
        {
            _socket = socket;
            Receive();
        }
        /// <summary>处理接收数据</summary>
        public virtual async void Receive()
        {
            // 考虑引入局部变量，如果之前的 Socket 被替换，还需继续完成之前的处理
            //Socket _socket = this._socket;

            while (isRuning)
            {
                try
                {
                    await _receiveDelayTask;

                    //TODO：处理半包、粘包

                    //_readBuffer.Clear();
                    _readProtocol.Reset();

                    if (_readProtocol == null)
                    {
                        Log.Error($"无法处理接收消息，因为没有设置 协议");
                        return;
                    }

                    var headHandle = _readProtocol.head;
                    var msgHandle = _readProtocol.msg;

                    /// 读取消息头
                    if (headHandle == null)
                    {
                        Log.Error($"无法处理接收消息，因为没有设置 消息头 处理器");
                        return;
                    }
                    var hR = await ReceiveAsync(headHandle, (received, data) =>
                    {
                        ReadHead();

                        Log.Info($"接收的 消息头长度：{received}，消息长度：{headHandle.msgLength}");
                    });

                    if (!hR) break;

                    /// 读取消息体
                    msgHandle.length = headHandle.msgLength;// 解析出的消息体长度
                    if (msgHandle == null)
                    {
                        Log.Error($"无法处理消息，因为没有设置 消息 处理器");
                        return;
                    }
                    bool bR = await ReceiveAsync(msgHandle, (received, data) =>
                    {
                        msgHandle.HandleCompletedEvent = (result) =>
                        Log.Info($"接收的 消息长度：{received}，消息内容：{result}");

                        ReadMsg();
                    });

                    if (!bR) break;

                }
                catch (SocketException ex)
                {
                    ReceiveStopEvent?.Invoke(ex.SocketErrorCode);
                    throw;
                }
            }

        }
        /// <summary>异步接受指定的消息长度</summary>
        protected virtual async Task<bool> ReceiveAsync(IMsgHandle msgHandle, Action<int, ArraySegment<byte>> completedCallback)
        {
            var bData = msgHandle.buffer.GetWriteArraySegment(msgHandle.length);

            int bReceived = await _socket.ReceiveAsync(bData, SocketFlags.None); // 尝试读取数据

            if (bReceived == 0)
            {
                // 没有数据可读，对方可能已经关闭了连接
                ReceiveStopEvent?.Invoke(SocketError.Success);
                return false;
            }
            else
            {
                // 处理接收到的数据
                completedCallback?.Invoke(bReceived, bData);
            }

            return true;
        }

        /// <summary>发送数据</summary>
        public virtual void Send(object msg)
        {
            if (msg == null) return;

            if (!SendWrite(msg))
            {
                Log.Error($"不支持发送的消息 {msg.GetType()}");
                return;
            }

            Send();
        }
        /// <summary>写入发送数据</summary>
        protected virtual bool SendWrite(object msg)
        {
            return _writeProtocol.msg.WriteHandle(msg);
        }

        #region TODO：有待商榷是否去掉
        /// <summary>发送数据</summary>
        public virtual void Send(string msg)
        {
            writeBuffer.Write(msg);

            Send();
        }
        /// <summary>发送数据</summary>
        public virtual void Send(byte[] bytes)
        {
            writeBuffer.Write(bytes);

            Send();
        } 
        #endregion

        /// <summary>发送</summary>
        public virtual async void Send()
        {
            if (_writeProtocol.msg.buffer.GetReadableBytesLength() <= 0) return;

            WriteHead();
            WriteMsg();
            var msg = _writeProtocol.GetDataArraySegment();// 最终要发的完整消息

            Log.Info($"要发送的消息包体长度：{_writeProtocol.length}，实际包体数据长度：{msg.Count}");

            try
            {
                await _socket.SendAsync(msg, SocketFlags.None);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                _writeProtocol.Reset();
            }
        }


        /// <summary>读取解析消息头数据</summary>
        protected virtual void ReadHead()
        {
            _readProtocol.head.ReadHandle();
        }
        /// <summary>读取解析消息体数据</summary>
        protected virtual void ReadMsg()
        {
            _readProtocol.msg.ReadHandle();
        }

        /// <summary>写入消息头数据</summary>
        protected virtual void WriteHead()
        {
            // 如果自定义协议中有其他规则可以在这里添加，如下
            //_headHandle.type = "string";

            // 设置消息体长度
            //_writeProtocol.head.msgLength = _writeProtocol.msg.length;
            _writeProtocol.head.msgLength = _writeProtocol.msg.dataLength;

            _writeProtocol.head.WriteHandle();

            Log.Info($"要发送的消息头长度：{_writeProtocol.head.dataLength}");
        }
        /// <summary>写入消息体数据</summary>
        protected virtual void WriteMsg()
        {
            Log.Info($"要发送的消息体长度：{_writeProtocol.msg.dataLength}");
            //_readProtocol.msg.WriteHandle();
        }

        /// <summary>
        /// 设置协议
        /// <code>
        /// 以下方式的简便写法
        ///  <see cref="_readProtocol"/> = new <typeparamref name="T"/>();
        ///  <see cref="_writeProtocol"/> = new <typeparamref name="T"/>();
        /// </code>
        /// </summary>
        /// <typeparam name="T"><see cref="IProtocol"/></typeparam>
        protected void SetProtocol<T>() where T : IProtocol, new()
        {
            _readProtocol = new T();
            _writeProtocol = new T();
        }
    }
}