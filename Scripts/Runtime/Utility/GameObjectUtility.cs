using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Framework.Runtime
{
    /// <summary>GameObject 实用程序</summary>
    public class GameObjectUtility
    {
        /// <summary>查找物体，不管有没有被隐藏都能找到
        /// <para>可用于替代 <see cref="GameObject.Find(string)"/>，其在物体初始被隐藏时查找不到</para>
        /// </summary>
        public static GameObject Find(string name)
        {
            return Find(name, out _);
        }

        /// <summary>查找物体，不管有没有被隐藏都能找到
        /// <para>可用于替代 <see cref="GameObject.Find(string)"/>，其在物体被隐藏时查找不到</para>
        /// </summary>
        public static GameObject Find(string name, out string sceneName)
        {
            GameObject result = null;
            sceneName = null;
            List<GameObject> rootGOs = TypePool.root.GetList<GameObject>();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                rootGOs.Clear();
                scene.GetRootGameObjects(rootGOs);

                result = rootGOs.Find(x => x.name == name);
                if (result)
                {
                    sceneName = scene.name;
                    break;
                }
                else
                {
                    foreach (var item in rootGOs)
                    {
                        if (item.TryFindOf(name, out result))
                        {
                            sceneName = scene.name;
                            break;
                        }
                    }
                }
            }
            TypePool.root.Return(rootGOs);
            return result;
        }
    }
}
