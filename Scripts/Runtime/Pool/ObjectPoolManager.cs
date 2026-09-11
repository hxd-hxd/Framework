using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.ObjectPool
{
    /// <summary>对象池管理器</summary>
    public class ObjectPoolManager : MonoSingleton<ObjectPoolManager>
    {
        [SerializeField]
        private TypePoolPosMarker _typePoolMarker = new TypePoolPosMarker();

        private void Start()
        {
            _typePoolMarker.pool = TypePool.root;
            _typePoolMarker.Init();
        }

        private void Update()
        {
            _typePoolMarker.Update(Time.deltaTime, Time.unscaledDeltaTime);
        }

    }

}
