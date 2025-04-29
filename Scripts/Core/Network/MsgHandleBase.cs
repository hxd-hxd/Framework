using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Framework.Core.Network
{

    /// <summary>
    /// Э����Ϣ�崦�����
    /// </summary>
    public abstract class MsgHandleBase : IMsgHandle, IDisposable
    {
        /// <summary>��������¼�</summary>
        public Action<object> HandleCompletedEvent;
        /// <summary>��������¼�</summary>
        public Action<object> HandleErrorEvent;

        /// <summary>�����¼�</summary>
        public Action<ByteBuffer> HandleEvent;

        protected ByteBuffer _buffer = new ByteBuffer(4);

        private bool disposedValue;

        /// <summary>
        /// �ڲ������õ� <see cref="ByteBuffer"/>
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

        /// <summary>���� <see cref="HandleCompletedEvent"/></summary>
        protected void CallHandleCompletedEvent(object v) => HandleCompletedEvent?.Invoke(v);
        /// <summary>���� <see cref="HandleErrorEvent"/></summary>
        protected void CallHandleErrorEvent(object v) => HandleErrorEvent?.Invoke(v);

        /// <summary>
        /// ������Ϣ
        /// </summary>
        public virtual void ResetMsg()
        {
            _buffer.Clear();
        }

        /// <summary>
        /// ����Ϣд�뵽������
        /// </summary>
        public abstract bool Get(ByteBuffer buffer, object msg);

        /// <summary>����</summary>
        public virtual void Handle(byte[] bytes)
        {
            buffer.Write(bytes);
            Handle(buffer);

            buffer.Clear();
        }

        /// <summary>����</summary>
        public virtual void Handle(ArraySegment<byte> bytes)
        {
            buffer.Write(bytes);
            Handle(buffer);

            buffer.Clear();
        }

        /// <summary>����ע��ʵ��ʱӦ�����˷�����Ϊ���մ���</summary>
        public abstract void Handle(ByteBuffer buffer);

        /// <summary>
        /// ͨ�ô���
        /// </summary>
        /// <param name="func">����ص��������Ƿ�ɹ��������õ��Ķ���</param>
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
                    // �ͷ��й�״̬(�йܶ���)
                    ((IDisposable)_buffer)?.Dispose();
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
            Dispose(disposing: true);
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