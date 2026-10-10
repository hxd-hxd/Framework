using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 可用于记录 <see cref="GameObjectPoolRecord"/>
    /// </summary>
    public class PoolRecordComponent : MonoBehaviour
    {
        public GameObjectPoolRecord record = new GameObjectPoolRecord();

        /// <summary>
        /// 通过 <see cref="record"/> 记录的对象池信息放入对象池
        /// </summary>
        public bool Return()
        {
            if (record == null) return false;
            return record.Return();
        }
    }
}
