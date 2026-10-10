using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 用于记录 <see cref="GameObjectPool"/> 信息
    /// </summary>
    [Serializable]
    public class GameObjectPoolRecord : ITypePoolObject
    {
        [NonSerialized]
        public GameObjectPool pool;
        public GameObject template;
        /// <summary>通过 <see cref="template"/> 实例化的实例，可选，视自己的使用方式而定</summary>
        public GameObject instance;
        public Transform parent;

        public GameObjectPoolRecord()
        {

        }
        public GameObjectPoolRecord(GameObjectPool pool, GameObject template)
        {
            this.pool = pool;
            this.template = template;
        }
        public GameObjectPoolRecord(GameObjectPool pool, GameObject template, GameObject instance)
        {
            this.pool = pool;
            this.template = template;
            this.instance = instance;
        }
        public GameObjectPoolRecord(GameObjectPool pool, GameObject template, GameObject instance, Transform parent)
        {
            this.pool = pool;
            this.template = template;
            this.instance = instance;
            this.parent = parent;
        }

        /// <summary>
        /// 是否有效记录
        /// </summary>
        public bool IsValid()
        {
            bool r = pool != null && template != null;
            return r;
        }

        /// <summary>
        /// 返回对象池
        /// </summary>
        public bool Return()
        {
            if (IsValid() && instance)
            {
                pool.Return(instance, template, parent);
                return true;
            }
            return false;
        }

        public void Clear()
        {
            pool = null;
            template = null;
            instance = null;
            parent = null;
        }
    }

}
