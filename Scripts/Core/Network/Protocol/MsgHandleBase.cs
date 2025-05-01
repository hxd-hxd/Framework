using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Framework.Core.Network
{

    /// <summary>
    /// Э����Ϣ�崦�����
    /// </summary>
    public abstract class MsgHandleBase : IMsgHandle
    {
        private Action<object> _handleCompletedEvent;
        private Action<object> _handleErrorEvent;

        /// <summary>��ȡ�����¼�</summary>
        public Action<ByteBuffer> ReadHandleEvent;

        private ByteBuffer _buffer;

        private bool disposedValue;

        public virtual int length { get; set; }
        public virtual int dataLength { get => buffer.GetReadableBytesLength(); }

        /// <summary>
        /// �ڲ������õ� <see cref="ByteBuffer"/>
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
        /// <summary>��������¼�</summary>
        public Action<object> HandleCompletedEvent
        {
            get => _handleCompletedEvent;
            set => _handleCompletedEvent = value;
        }
        /// <summary>��������¼�</summary>
        public Action<object> HandleErrorEvent
        {
            get => _handleErrorEvent;
            set => _handleErrorEvent = value;
        }

        /// <summary>���� <see cref="HandleCompletedEvent"/></summary>
        protected void CallHandleCompletedEvent(object v) => HandleCompletedEvent?.Invoke(v);
        /// <summary>���� <see cref="HandleErrorEvent"/></summary>
        protected void CallHandleErrorEvent(object v) => HandleErrorEvent?.Invoke(v);


        public virtual bool WriteHandle(object msg)
        {
            return WriteHandle(buffer, msg);
        }

        /// <summary>
        /// ����Ϣд�뵽������
        /// </summary>
        public abstract bool WriteHandle(ByteBuffer buffer, object msg);


        /// <summary>��ȡ����
        /// <para>ע�⣺�����Ƿ���ɹ������ݶ��ᱻ�����</para>
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

        /// <summary>��ȡ����
        /// <para>ע�⣺ʵ��ʱӦ�����˷�����Ϊ���մ���</para>
        /// </summary>
        public abstract void ReadHandle(ByteBuffer buffer);

        /// <summary>
        /// ͨ�ô���
        /// </summary>
        /// <param name="func">����ص��������Ƿ�ɹ��������õ��Ķ���</param>
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
                    // �ͷ��й�״̬(�йܶ���)
                    ((IDisposable)buffer)?.Dispose();
                    _buffer = null;
                    HandleCompletedEvent = null;
                    HandleErrorEvent = null;
                }

                // �ͷ�δ�йܵ���Դ(δ�йܵĶ���)����д�ս���
                // �������ֶ�����Ϊ null
                disposedValue = true;
            }
        }

        // // ������Dispose(bool disposing)��ӵ�������ͷ�δ�й���Դ�Ĵ���ʱ������ս���
        // ~MsgHandleBase()
        // {
        //     // ��Ҫ���Ĵ˴��롣�뽫���������롰Dispose(bool disposing)��������
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // ��Ҫ���Ĵ˴��롣�뽫���������롰Dispose(bool disposing)��������
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// ����ص�
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected delegate bool HandleCallback(out object result);
    }
}