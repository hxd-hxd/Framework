using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Core;
using System;

namespace Framework
{
    /// <summary>
    /// ��������־
    /// </summary>
    public class CLogger : ILog
    {
        void ILog.Error(object msg)
        {
            Debug.LogError(msg);
        }

        void ILog.Info(object msg)
        {
            Debug.Log(msg);
        }

        void ILog.Warning(object msg)
        {
            Debug.LogWarning(msg);
        }
    }
}
