using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Framework.Core.Network
{

    /// <summary>
    /// Э����Ϣ�������
    /// </summary>
    public abstract class MsgHandleBase : IMsgHandle
    {
        private Action<object> _handleCompletedEvent;
        private Action<object> _handleErrorEvent;

        private ByteBuffer _buffer;
        private int _length;

        private bool disposedValue;

        public virtual int length { get => _length; set => _length = value; }
        public virtual int dataLength { get => buffer.GetReadableBytesLength(); }

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
        /// <summary>��ȡ����¼�</summary>
        public Action<object> readCompletedEvent
        {
            get => _handleCompletedEvent;
            set => _handleCompletedEvent = value;
        }
        /// <summary>��ȡ�����¼�</summary>
        public Action<object> readErrorEvent
        {
            get => _handleErrorEvent;
            set => _handleErrorEvent = value;
        }

        /// <summary>���� <see cref="readCompletedEvent"/></summary>
        protected void CallReadCompletedEvent(object v) => readCompletedEvent?.Invoke(v);
        /// <summary>���� <see cref="readErrorEvent"/></summary>
        protected void CallReadErrorEvent(object v) => readErrorEvent?.Invoke(v);


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
        /// ͨ�ô����ɴ��������Ϣ
        /// <para><paramref name="func"/>������ص��������Ƿ��������������������ڴ��������Ϣ��result�������õ��Ķ���</para>
        /// </summary>
        protected void ReadHandle(ByteBuffer buffer, HandleCallback func)
        {
            object msg = null;

            try
            {
                do
                {
                    bool r = func(out msg);

                    CallReadCompletedEvent(msg);

                    if (!r)
                    {
                        //CallReadErrorEvent(msg);
                        break;
                    }
                }
                while (true);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                CallReadErrorEvent(msg);
            }
        }


        public virtual void Reset()
        {
            buffer?.Clear();
        }

        public virtual void Clear()
        {
            _buffer?.Clear();
            readCompletedEvent = null;
            readErrorEvent = null;
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
                    readCompletedEvent = null;
                    readErrorEvent = null;
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
        /// <returns>�Ƿ��������</returns>
        protected delegate bool HandleCallback(out object result);
    }
}