using System;
using System.Collections;
using System.Collections.Generic;

namespace Framework.Core.Network
{
    /// <summary>
    /// Ĭ��Э�飬��ͷ������ɵ�����Э��
    /// </summary>
    public class DefaultProtocol : Protocol<DefaultHeadHandle, DefaultMsgHandle>
    {
        public DefaultProtocol()
        {
            head = new DefaultHeadHandle();
            msg = new DefaultMsgHandle();
        }
    }
}