using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Runtime;
using UnityEngine;

namespace Framework.ObjectPool
{
    /// <summary>对象池管理器</summary>
    public class ObjectPoolManager : MonoSingleton<ObjectPoolManager>
    {
        [SerializeField]
        private TypePoolPosMarker _typePoolMarker = new TypePoolPosMarker();
        [SerializeField]
        private GOPoolPosMarker _GOPoolMarker = new GOPoolPosMarker();

        [SerializeField]
        private List<TypeInfo> typeInfos;
        [SerializeField]
        private List<ArrayInfo> arrayInfos;
        [SerializeField]
        private List<GOInfo> gOInfos;

        private void Start()
        {
            _typePoolMarker.pool = TypePool.root;
            _typePoolMarker.Init();
            _GOPoolMarker.pool = GameObjectPool.root;
            _GOPoolMarker.Init();
        }

        private void Update()
        {
            _typePoolMarker.Update(Time.deltaTime, Time.unscaledDeltaTime);
            _GOPoolMarker.Update(Time.deltaTime, Time.unscaledDeltaTime);
        }

    }
}
