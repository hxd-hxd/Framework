using System.Collections;
using System.Collections.Generic;

namespace Framework.Core
{
    /// <summary>任务接口</summary>
    public interface ITask
    {
        /// <summary>是否完成</summary>
        bool IsDone { get; set; }

        /// <summary>执行此任务</summary>
        void Execute();
    }
}
