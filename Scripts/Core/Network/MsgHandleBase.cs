using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Framework.Core.Network
{

    /// <summary>
    /// 协议消息体处理基类
    /// </summary>
    public abstract class MsgHandleBase : IMsgHandle, IDisposable
    {
        /// <summary>处理完成事件</summary>
        public Action<object> HandleCompletedEvent;
        /// <summary>处理错误事件</summary>
        public Action<object> HandleErrorEvent;

        /// <summary>处理事件</summary>
        public Action<ByteBuffer> HandleEvent;

        protected ByteBuffer _buffer = new ByteBuffer(4);

        private bool disposedValue;

        /// <summary>
        /// 内部处理用的 <see cref="ByteBuffer"/>
        /// </summary>
        public ByteBuffer buffer
        {
            get
            {
                //if (_buffer == null)
                //    lock (this)
                //    {
                //        if (_buffer == null) _buffer = new ByteBuffer(4);
                //    }
                return _buffer;
            }
        }

        /// <summary>调用 <see cref="HandleCompletedEvent"/></summary>
        protected void CallHandleCompletedEvent(object v) => HandleCompletedEvent?.Invoke(v);
        /// <summary>调用 <see cref="HandleErrorEvent"/></summary>
        protected void CallHandleErrorEvent(object v) => HandleErrorEvent?.Invoke(v);

        /// <summary>
        /// 重置消息
        /// </summary>
        public virtual void ResetMsg()
        {
            _buffer.Clear();
        }

        /// <summary>
        /// 将消息写入到缓冲区
        /// </summary>
        public abstract bool Get(ByteBuffer buffer, object msg);

        /// <summary>处理</summary>
        public virtual void Handle(byte[] bytes)
        {
            buffer.Write(bytes);
            Handle(buffer);

            buffer.Clear();
        }

        /// <summary>处理</summary>
        public virtual void Handle(ArraySegment<byte> bytes)
        {
            buffer.Write(bytes);
            Handle(buffer);

            buffer.Clear();
        }

        /// <summary>处理，注意实现时应当将此方法作为最终处理</summary>
        public abstract void Handle(ByteBuffer buffer);

        /// <summary>
        /// 通用处理
        /// </summary>
        /// <param name="func">处理回调，反回是否成功，处理后得到的对象</param>
        protected void Handle(ByteBuffer buffer, HandleCallback func)
        {
            object msg = null;
            try
            {
                if (func(out msg))
                {
                    HandleEvent?.Invoke(buffer);
                    CallHandleCompletedEvent(msg);
                }
                else
                {
                    CallHandleErrorEvent(msg);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                CallHandleErrorEvent(msg);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // 释放托管状态(托管对象)
                    ((IDisposable)_buffer)?.Dispose();
                    _buffer = null;
                    HandleCompletedEvent = null;
                    HandleErrorEvent = null;
                }

                // 释放未托管的资源(未托管的对象)并重写终结器
                // 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~MsgHandleBase()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// 处理回调
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected delegate bool HandleCallback(out object result);
    }
}