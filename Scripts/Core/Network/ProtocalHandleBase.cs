using System;
using System.Collections;
using System.Collections.Generic;
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
    public abstract class ProtocalHandleBase : IProtocalHandle
    {
        protected Socket _socket;
        // ���ݻ�����
        // _writeBuffer ������д�����ݣ�Ҳ������Ϣ��
        // _sendBuffer ���ջὫ��Ϣͷ����Ϣ��ϲ����˷���
        protected ByteBuffer _writeBuffer, _readBuffer, _sendBuffer = new ByteBuffer(1024);

        protected HeadHandleBase _headHandle;
        protected MsgHandleBase _msgHandle;

        /// <summary>��Ҫ�ɴ�Э�鴦����׽�������</summary>
        public Socket socket { get => _socket; set => _socket = value; }
        /// <summary>д�����ݻ�����</summary>
        public ByteBuffer writeBuffer { get => _writeBuffer; set => _writeBuffer = value; }
        /// <summary>��ȡ���ݻ�����</summary>
        public ByteBuffer readBuffer { get => _readBuffer; set => _readBuffer = value; }
        /// <summary>�������շ������ݵĻ�����</summary>
        protected ByteBuffer sendBuffer { get => _sendBuffer; set => _sendBuffer = value; }
        /// <summary>��Ϣͷ����</summary>
        public HeadHandleBase headHandle { get => _headHandle; protected set => _headHandle = value; }
        /// <summary>��Ϣ�崦��</summary>
        public MsgHandleBase msgHandle { get => _msgHandle; protected set => _msgHandle = value; }
        /// <summary><see cref="socket"/> �Ƿ�������״̬</summary>
        public bool isConnected => _socket != null ? _socket.Connected : false;

        /// <summary>���������¼�</summary>
        public Func<bool> ReceiveConditionEvent;
        /// <summary>����ֹͣ�¼�</summary>
        public Action<SocketError> ReceiveStopEvent;

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

                    _readBuffer.Clear();

                    /// ��ȡ��Ϣͷ
                    if (_headHandle == null)
                    {
                        Log.Error($"�޷�������Ϣ����Ϊû������ ��Ϣͷ ������");
                        return;
                    }

                    int msgLen = 0;// ����������Ϣ�峤��
                    var hR = await _func_ReceiveAsync(_headHandle.length, (received, data) =>
                    {
                        ReadHead();
                        msgLen = headHandle.msgLength;

                        Log.Info($"���յ� ��Ϣͷ���ȣ�{received}����Ϣ���ȣ�{msgLen}");
                    });

                    if (!hR) break;

                    /// ��ȡ��Ϣ��
                    if (_msgHandle == null)
                    {
                        Log.Error($"�޷�������Ϣ����Ϊû������ ��Ϣ ������");
                        return;
                    }
                    bool bR = await _func_ReceiveAsync(msgLen, (received, data) =>
                    {
                        _msgHandle.HandleCompletedEvent = (result) =>
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

            async Task<bool> _func_ReceiveAsync(int msgLen, Action<int, ArraySegment<byte>> completedCallback)
            {
                var bData = _readBuffer.GetWriteArraySegment(msgLen);
                var bTask = _socket.ReceiveAsync(bData, SocketFlags.None); // ���Զ�ȡ����

                await bTask;
                int bReceived = bTask.Result;
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
        }

        /// <summary>����������</summary>
        public virtual void Send(object msg)
        {
            if (msg == null) return;

            if (!SendAbstract(msg))
            {
                Log.Error($"��֧�ַ��͵���Ϣ {msg.GetType()}");
                return;
            }

            Send();
        }
        /// <summary>����������</summary>
        protected abstract bool SendAbstract(object msg);

        /// <summary>����������</summary>
        public virtual void Send(string msg)
        {
            _writeBuffer.Write(msg);

            Send();
        }
        /// <summary>����������</summary>
        public virtual void Send(byte[] bytes)
        {
            _writeBuffer.Write(bytes);

            Send();
        }
        /// <summary>���� <see cref="writeBuffer"/> ������ݣ�Ҳ��ֱ���� <see cref="writeBuffer"/> ��д�����ݣ�Ȼ����ô˷���һ����</summary>
        public virtual async void Send()
        {
            if (_writeBuffer.ReadableBytesLength() <= 0) return;

            //// ��Ϣ��
            //var msg = _writeBuffer.ToArraySegment();
            //// ��Ϣͷ
            //var head = _sendBuffer.GetWriteArraySegment(_headHandle.length);
            //_headHandle.Get(head, msg.Count);// ��ȡ��Ϣͷ����ӵ����ͻ�����
            //// �ϲ�ͷ����
            //_sendBuffer.Write(msg);
            //msg = _sendBuffer.ToArraySegment();// ����Ҫ����������Ϣ

            WriteHead();
            WriteMsg();
            var msg = _sendBuffer.ToArraySegment();// ����Ҫ����������Ϣ

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
            // ͨ����Ϣͷ��������Ϣ����
            _headHandle.Handle(_readBuffer);
        }
        protected virtual void ReadMsg()
        {
            
            _msgHandle.Handle(_readBuffer);
        }
        protected virtual void WriteHead()
        {
            ////TODO������Զ���Э�������������������������ӣ�����
            //_headHandle.type = "string";

            int msgLength = _writeBuffer.ReadableBytesLength();

            var head = _sendBuffer.GetWriteArraySegment(_headHandle.length);
            _headHandle.Get(head, msgLength);// ��ȡ��Ϣͷ����ӵ����ͻ�����

        }
        protected virtual void WriteMsg()
        {
            // �ϲ�ͷ����
            _sendBuffer.Write(_writeBuffer);
        }
    }
}