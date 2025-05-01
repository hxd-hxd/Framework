using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Framework.Core.Network
{

    /// <summary>
    /// 协议消息体处理基类
    /// </summary>
    public abstract class MsgHandleBase : IMsgHandle
    {
        private Action<object> _handleCompletedEvent;
        private Action<object> _handleErrorEvent;

        /// <summary>读取处理事件</summary>
        public Action<ByteBuffer> ReadHandleEvent;

        private ByteBuffer _buffer;

        private bool disposedValue;

        public virtual int length { get; set; }
        public virtual int dataLength { get => buffer.GetReadableBytesLength(); }

        /// <summary>
        /// 内部处理用的 <see cref="ByteBuffer"/>
        /// </summary>
        public ByteBuffer buffer
        {
            get
            {
                if (_buffer == null)
                    lock (this)
                    {
                        if (_buffer == null) _buffer = new ByteBuffer(4);
                    }
                return _buffer;
            }
        }
        /// <summary>处理完成事件</summary>
        public Action<object> HandleCompletedEvent
        {
            get => _handleCompletedEvent;
            set => _handleCompletedEvent = value;
        }
        /// <summary>处理错误事件</summary>
        public Action<object> HandleErrorEvent
        {
            get => _handleErrorEvent;
            set => _handleErrorEvent = value;
        }

        /// <summary>调用 <see cref="HandleCompletedEvent"/></summary>
        protected void CallHandleCompletedEvent(object v) => HandleCompletedEvent?.Invoke(v);
        /// <summary>调用 <see cref="HandleErrorEvent"/></summary>
        protected void CallHandleErrorEvent(object v) => HandleErrorEvent?.Invoke(v);


        public virtual bool WriteHandle(object msg)
        {
            return WriteHandle(buffer, msg);
        }

        /// <summary>
        /// 将消息写入到缓冲区
        /// </summary>
        public abstract bool WriteHandle(ByteBuffer buffer, object msg);


        /// <summary>读取处理
        /// <para>注意：不管是否处理成功，数据都会被清除掉</para>
        /// </summary>
        public virtual void ReadHandle()
        {
            try
            {
                ReadHandle(buffer);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                buffer.Clear();
            }
        }

        /// <summary>读取处理
        /// <para>注意：实现时应当将此方法作为最终处理</para>
        /// </summary>
        public abstract void ReadHandle(ByteBuffer buffer);

        /// <summary>
        /// 通用处理
        /// </summary>
        /// <param name="func">处理回调，反回是否成功，处理后得到的对象</param>
        protected void ReadHandle(ByteBuffer buffer, HandleCallback func)
        {
            object msg = null;
            try
            {
                if (func(out msg))
                {
                    ReadHandleEvent?.Invoke(buffer);
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


        public virtual void Reset()
        {
            buffer?.Clear();
        }

        public virtual void Clear()
        {
            _buffer?.Clear();
            HandleCompletedEvent = null;
            HandleErrorEvent = null;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // 释放托管状态(托管对象)
                    ((IDisposable)buffer)?.Dispose();
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
            Dispose(true);
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