using System.Collections.Generic;
using Framework;
using Framework.Runtime;
using UnityEngine;

namespace Framework.Test
{
    public class TestFindSceneGO : MonoBehaviour
    {
        public GameObject go1, go2;

        // Start is called before the first frame update
        void Start()
        {
            go1 = GameObject.Find("1");
            go2 = GameObject.Find("2");
            Debug.Log($"找到: {go1}");
            if (go2)
                Debug.Log($"找到: {go2}");
            else
            {
                Debug.Log($"未找到 2，尝试在所有场景中查找");
                go2 = GameObjectUtility.Find("2", out var sceneName);
                if (go2)
                    Debug.Log($"在场景 {sceneName} 中找到: {go2}");
            }
        }
    }
}