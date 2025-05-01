using System;
using System.Collections;
using System.Collections.Generic;

namespace Framework.Core.Network
{
    /// <summary>
    /// 默认协议，由头和体组成的完整协议
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