using System;
using System.Collections;
using System.Collections.Generic;
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
    public abstract class ProtocalHandleBase : IProtocalHandle
    {
        protected Socket _socket;
        // 数据缓冲区
        // _writeBuffer 仅用于写入数据，也就是消息体
        // _sendBuffer 最终会将消息头和消息体合并到此发送
        protected ByteBuffer _writeBuffer, _readBuffer, _sendBuffer = new ByteBuffer(1024);

        protected HeadHandleBase _headHandle;
        protected MsgHandleBase _msgHandle;

        /// <summary>需要由此协议处理的套接字连接</summary>
        public Socket socket { get => _socket; set => _socket = value; }
        /// <summary>写入数据缓冲区</summary>
        public ByteBuffer writeBuffer { get => _writeBuffer; set => _writeBuffer = value; }
        /// <summary>读取数据缓冲区</summary>
        public ByteBuffer readBuffer { get => _readBuffer; set => _readBuffer = value; }
        /// <summary>用于最终发送数据的缓冲区</summary>
        protected ByteBuffer sendBuffer { get => _sendBuffer; set => _sendBuffer = value; }
        /// <summary>消息头处理</summary>
        public HeadHandleBase headHandle { get => _headHandle; protected set => _headHandle = value; }
        /// <summary>消息体处理</summary>
        public MsgHandleBase msgHandle { get => _msgHandle; protected set => _msgHandle = value; }
        /// <summary><see cref="socket"/> 是否处于连接状态</summary>
        public bool isConnected => _socket != null ? _socket.Connected : false;

        /// <summary>接收条件事件</summary>
        public Func<bool> ReceiveConditionEvent;
        /// <summary>接收停止事件</summary>
        public Action<SocketError> ReceiveStopEvent;

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

                    _readBuffer.Clear();

                    /// 读取消息头
                    if (_headHandle == null)
                    {
                        Log.Error($"无法处理消息，因为没有设置 消息头 处理器");
                        return;
                    }

                    int msgLen = 0;// 解析出的消息体长度
                    var hR = await _func_ReceiveAsync(_headHandle.length, (received, data) =>
                    {
                        ReadHead();
                        msgLen = headHandle.msgLength;

                        Log.Info($"接收的 消息头长度：{received}，消息长度：{msgLen}");
                    });

                    if (!hR) break;

                    /// 读取消息体
                    if (_msgHandle == null)
                    {
                        Log.Error($"无法处理消息，因为没有设置 消息 处理器");
                        return;
                    }
                    bool bR = await _func_ReceiveAsync(msgLen, (received, data) =>
                    {
                        _msgHandle.HandleCompletedEvent = (result) =>
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

            async Task<bool> _func_ReceiveAsync(int msgLen, Action<int, ArraySegment<byte>> completedCallback)
            {
                var bData = _readBuffer.GetWriteArraySegment(msgLen);
                var bTask = _socket.ReceiveAsync(bData, SocketFlags.None); // 尝试读取数据

                await bTask;
                int bReceived = bTask.Result;
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
        }

        /// <summary>处理发送数据</summary>
        public virtual void Send(object msg)
        {
            if (msg == null) return;

            if (!SendAbstract(msg))
            {
                Log.Error($"不支持发送的消息 {msg.GetType()}");
                return;
            }

            Send();
        }
        /// <summary>处理发送数据</summary>
        protected abstract bool SendAbstract(object msg);

        /// <summary>处理发送数据</summary>
        public virtual void Send(string msg)
        {
            _writeBuffer.Write(msg);

            Send();
        }
        /// <summary>处理发送数据</summary>
        public virtual void Send(byte[] bytes)
        {
            _writeBuffer.Write(bytes);

            Send();
        }
        /// <summary>发送 <see cref="writeBuffer"/> 里的数据，也可直接向 <see cref="writeBuffer"/> 中写入数据，然后调用此方法一起发送</summary>
        public virtual async void Send()
        {
            if (_writeBuffer.ReadableBytesLength() <= 0) return;

            //// 消息体
            //var msg = _writeBuffer.ToArraySegment();
            //// 消息头
            //var head = _sendBuffer.GetWriteArraySegment(_headHandle.length);
            //_headHandle.Get(head, msg.Count);// 获取消息头并添加到发送缓冲区
            //// 合并头和体
            //_sendBuffer.Write(msg);
            //msg = _sendBuffer.ToArraySegment();// 最终要发的完整消息

            WriteHead();
            WriteMsg();
            var msg = _sendBuffer.ToArraySegment();// 最终要发的完整消息

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
                _writeBuffer.Clear();
                _sendBuffer.Clear();
            }
        }

        protected virtual void ReadHead()
        {
            // 通过消息头解析出消息长度
            _headHandle.Handle(_readBuffer);
        }
        protected virtual void ReadMsg()
        {
            
            _msgHandle.Handle(_readBuffer);
        }
        protected virtual void WriteHead()
        {
            ////TODO：如果自定义协议中有其他规则可以在这里添加，如下
            //_headHandle.type = "string";

            int msgLength = _writeBuffer.ReadableBytesLength();

            var head = _sendBuffer.GetWriteArraySegment(_headHandle.length);
            _headHandle.Get(head, msgLength);// 获取消息头并添加到发送缓冲区

        }
        protected virtual void WriteMsg()
        {
            // 合并头和体
            _sendBuffer.Write(_writeBuffer);
        }
    }
}