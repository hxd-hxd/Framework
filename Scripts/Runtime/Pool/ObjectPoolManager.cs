using System.Collections;
using System.Collections.Generic;

namespace Framework.ObjectPool
{
    /// <summary>对象池管理器</summary>
    public class ObjectPoolManager : Singleton<ObjectPoolManager>
    {
        private TypePoolMarker _typePoolMarker = new TypePoolMarker();

        public void Update(float elapseTime, float realElapseTime)
        {
            _typePoolMarker.Update(elapseTime, realElapseTime);
        }

    }

    public class TypePoolMarker
    {
        public void Update(float elapseTime, float realElapseTime)
        {
        }
    }
}
