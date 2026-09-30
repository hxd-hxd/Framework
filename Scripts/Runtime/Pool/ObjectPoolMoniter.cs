using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Core;
using Framework.Runtime;
using UnityEngine;

namespace Framework.ObjectPool
{
    /// <summary>对象池监视器</summary>
    public class ObjectPoolMoniter : MonoSingleton<ObjectPoolMoniter>
    {
        [SerializeField]
        private TypePoolPosMarker _typePoolMarker = new TypePoolPosMarker();
        [SerializeField]
        private GOPoolPosMarker _GOPoolMarker = new GOPoolPosMarker();

        [SerializeField]
        private List<TypeMarkerInfo> typeMarkerInfos;
        [SerializeField]
        private List<ArrayMarkerInfo> arrayMarkerInfos;
        [SerializeField]
        private List<GOMarkerInfo> gOMarkerInfos;

        private void Start()
        {
            _typePoolMarker.pool = TypePool.root;
            _typePoolMarker.Init();
            _GOPoolMarker.pool = GameObjectPool.root;
            _GOPoolMarker.Init();

            foreach (var item in typeMarkerInfos)
            {
                var type = FindType(item.typeName);
                if (type == null) continue;
                _typePoolMarker.AddMarker(type, item.info);
            }

            foreach (var item in arrayMarkerInfos)
            {
                var type = FindType(item.typeName);
                if (type == null) continue;
                _typePoolMarker.AddMarker(type, item.length, item.info);
            }

            foreach (var item in gOMarkerInfos)
            {
                var template = item.template;
                if (template == null) continue;
                _GOPoolMarker.AddMarker(template, item.info);
            }
        }

        private void Update()
        {
            _typePoolMarker.Update(Time.deltaTime, Time.unscaledDeltaTime);
            _GOPoolMarker.Update(Time.deltaTime, Time.unscaledDeltaTime);
        }

        /// <summary>按类型全名查找（带缓存）。</summary>
        static Type FindType(string fullName)
        {
            return TypeCache.Get(fullName);
        }
    }
}
