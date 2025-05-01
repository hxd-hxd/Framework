using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Framework.Core.Network
{

    /// <summary>
    /// Э��ӿ�
    /// </summary>
    public interface IProtocol
    {
        /// <summary>����Э����Ϣ�����峤��</summary>
        int length { get; }

        /// <summary>Э���ͷ</summary>
        IHeadHandle head { get; set; }

        /// <summary>Э�����</summary>
        IMsgHandle msg { get; set; }

        /// <summary>
        /// ��ȡ������Э�����ݰ�
        /// </summary>
        /// <returns></returns>
        byte[] GetDatas();
        /// <summary>
        /// ��ȡ������Э�����ݰ�
        /// </summary>
        /// <returns></returns>
        ArraySegment<byte> GetDataArraySegment();

        /// <summary>���ã�������Ϣ�ڲ�״̬�ָ�����ʼ</summary>
        void Reset();

        /// <summary>���</summary>
        void Clear();
    }

    /// <summary>
    /// Э��ӿ�
    /// </summary>
    public interface IProtocol<THead, TMsg> : IProtocol
        where THead : IHeadHandle
        where TMsg : IMsgHandle
    {
        /// <summary>Э���ͷ</summary>
        new THead head { get; set; }

        /// <summary>Э�����</summary>
        new TMsg msg { get; set; }

    }
}