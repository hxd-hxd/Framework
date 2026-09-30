using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Test
{
    public class TestObjectPoolManager : MonoBehaviour
    {
        public GameObject[] templates;

        void Start()
        {
            for (int i = 0; i < 10; i++)
                TypePool.root.Return(new List<object>());
            TypePool.root.Return(new List<int>());

            GameObjectPool.root.PreCreateInstance(templates[0], 10);
        }

        void Update()
        {

        }
    }
}
