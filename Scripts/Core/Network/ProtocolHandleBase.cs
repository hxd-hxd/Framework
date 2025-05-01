using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Framework.Core.Network
{

    /// <summary>
    /// Э�鴦�����
    /// <code>
    /// ����Զ���Э�飺
    /// Э��İ�ͷ�Ͱ���Ӧ���ǳɶԳ��ֵ�
    /// </code>
    /// </summary>
    public abstract class ProtocolHandleBase : IProtocolHandle
    {
        protected Socket _socket;

        protected IProtocol _readProtocol;// ���ڽ��յ�Э��
        protected IProtocol _writeProtocol;// ���ڷ��͵�Э��

        /// <summary>��Ҫ�ɴ�Э�鴦����׽�������</summary>
        public Socket socket { get => _socket; set => _socket = value; }
        /// <summary>д�����ݻ�����</summary>
        protected ByteBuffer writeBuffer { get => _writeProtocol.msg.buffer; }
        /// <summary><see cref="socket"/> �Ƿ�������״̬</summary>
        public bool isConnected => _socket != null ? _socket.Connected : false;

        /// <summary>���������¼�</summary>
        public Func<bool> ReceiveConditionEvent;
        /// <summary>����ֹͣ�¼�</summary>
        public Action<SocketError> ReceiveStopEvent;

        protected ProtocolHandleBase(Socket socket)
        {
            _socket = socket;
        }

        protected static Task _receiveDelayTask = Task.Delay(1);

        protected bool isRuning => ReceiveConditionEvent != null ? ReceiveConditionEvent() : true;

        /// <summary>�����������</summary>
        public virtual void Receive(Socket socket)
        {
            _socket = socket;
            Receive();
        }
        /// <summary>�����������</summary>
        public virtual async void Receive()
        {
            // ��������ֲ����������֮ǰ�� Socket ���滻������������֮ǰ�Ĵ���
            //Socket _socket = this._socket;

            while (isRuning)
            {
                try
                {
                    await _receiveDelayTask;

                    //TODO����������ճ��

                    //_readBuffer.Clear();
                    _readProtocol.Reset();

                    if (_readProtocol == null)
                    {
                        Log.Error($"�޷����������Ϣ����Ϊû������ Э��");
                        return;
                    }

                    var headHandle = _readProtocol.head;
                    var msgHandle = _readProtocol.msg;

                    /// ��ȡ��Ϣͷ
                    if (headHandle == null)
                    {
                        Log.Error($"�޷����������Ϣ����Ϊû������ ��Ϣͷ ������");
                        return;
                    }
                    var hR = await ReceiveAsync(headHandle, (received, data) =>
                    {
                        ReadHead();

                        Log.Info($"���յ� ��Ϣͷ���ȣ�{received}����Ϣ���ȣ�{headHandle.msgLength}");
                    });

                    if (!hR) break;

                    /// ��ȡ��Ϣ��
                    msgHandle.length = headHandle.msgLength;// ����������Ϣ�峤��
                    if (msgHandle == null)
                    {
                        Log.Error($"�޷�������Ϣ����Ϊû������ ��Ϣ ������");
                        return;
                    }
                    bool bR = await ReceiveAsync(msgHandle, (received, data) =>
                    {
                        msgHandle.HandleCompletedEvent = (result) =>
                        Log.Info($"���յ� ��Ϣ���ȣ�{received}����Ϣ���ݣ�{result}");

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
        /// <summary>�첽����ָ������Ϣ����</summary>
        protected virtual async Task<bool> ReceiveAsync(IMsgHandle msgHandle, Action<int, ArraySegment<byte>> completedCallback)
        {
            var bData = msgHandle.buffer.GetWriteArraySegment(msgHandle.length);

            int bReceived = await _socket.ReceiveAsync(bData, SocketFlags.None); // ���Զ�ȡ����

            if (bReceived == 0)
            {
                // û�����ݿɶ����Է������Ѿ��ر�������
                ReceiveStopEvent?.Invoke(SocketError.Success);
                return false;
            }
            else
            {
                // ������յ�������
                completedCallback?.Invoke(bReceived, bData);
            }

            return true;
        }

        /// <summary>��������</summary>
        public virtual void Send(object msg)
        {
            if (msg == null) return;

            if (!SendWrite(msg))
            {
                Log.Error($"��֧�ַ��͵���Ϣ {msg.GetType()}");
                return;
            }

            Send();
        }
        /// <summary>д�뷢������</summary>
        protected virtual bool SendWrite(object msg)
        {
            return _writeProtocol.msg.WriteHandle(msg);
        }

        #region TODO���д���ȶ�Ƿ�ȥ��
        /// <summary>��������</summary>
        public virtual void Send(string msg)
        {
            writeBuffer.Write(msg);

            Send();
        }
        /// <summary>��������</summary>
        public virtual void Send(byte[] bytes)
        {
            writeBuffer.Write(bytes);

            Send();
        } 
        #endregion

        /// <summary>����</summary>
        public virtual async void Send()
        {
            if (_writeProtocol.msg.buffer.GetReadableBytesLength() <= 0) return;

            WriteHead();
            WriteMsg();
            var msg = _writeProtocol.GetDataArraySegment();// ����Ҫ����������Ϣ

            Log.Info($"Ҫ���͵���Ϣ���峤�ȣ�{_writeProtocol.length}��ʵ�ʰ������ݳ��ȣ�{msg.Count}");

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


        /// <summary>��ȡ������Ϣͷ����</summary>
        protected virtual void ReadHead()
        {
            _readProtocol.head.ReadHandle();
        }
        /// <summary>��ȡ������Ϣ������</summary>
        protected virtual void ReadMsg()
        {
            _readProtocol.msg.ReadHandle();
        }

        /// <summary>д����Ϣͷ����</summary>
        protected virtual void WriteHead()
        {
            // ����Զ���Э�������������������������ӣ�����
            //_headHandle.type = "string";

            // ������Ϣ�峤��
            //_writeProtocol.head.msgLength = _writeProtocol.msg.length;
            _writeProtocol.head.msgLength = _writeProtocol.msg.dataLength;

            _writeProtocol.head.WriteHandle();

            Log.Info($"Ҫ���͵���Ϣͷ���ȣ�{_writeProtocol.head.dataLength}");
        }
        /// <summary>д����Ϣ������</summary>
        protected virtual void WriteMsg()
        {
            Log.Info($"Ҫ���͵���Ϣ�峤�ȣ�{_writeProtocol.msg.dataLength}");
            //_readProtocol.msg.WriteHandle();
        }

        /// <summary>
        /// ����Э��
        /// <code>
        /// ���·�ʽ�ļ��д��
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