using System.Collections;
using System.Collections.Generic;

namespace Framework.Core
{
    public interface ILog
    {
        void Info(object msg);
        void Warning(object msg);
        void Error(object msg);
    }
}
