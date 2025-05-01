using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Framework.Core.Network
{
    /// <summary>
    /// Э�鴦��ӿ�
    /// </summary>
    public interface IProtocolHandle
    {
        /// <summary>��Ҫ�ɴ�Э�鴦����׽�������</summary>
        public Socket socket { get; set; }

        /// <summary>���������¼�</summary>
        Func<bool> receiveConditionEvent { get; set; }
        /// <summary>����ֹͣ�¼�</summary>
        Action<SocketError> receiveStopEvent { get; set; }

    }

}