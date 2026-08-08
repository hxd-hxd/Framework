using System.Collections;
using System.Collections.Generic;

namespace Framework.Core
{
    /// <summary>任务状态</summary>
    public enum TaskStatus
    {
        /// <summary>未执行</summary>
        Todo = 0,

        /// <summary>执行中</summary>
        Doing,

        /// <summary>完成</summary>
        Done,
    }
}
