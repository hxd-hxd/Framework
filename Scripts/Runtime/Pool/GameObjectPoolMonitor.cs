using UnityEngine;

namespace Framework
{

    /// <summary>
    /// <see cref="GameObjectPool"/> 监视器
    /// </summary>
    public class GameObjectPoolMonitor : MonoBehaviour
    {
        private void OnApplicationQuit()
        {
            GameObjectPool.NotifyQuitting();
        }

        private void OnDestroy()
        {
            GameObjectPool.NotifyMonitorDestroyed(this);
        }
    }

}
