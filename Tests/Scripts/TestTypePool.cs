// -------------------------
// 创建日期：2024/7/17 11:34:24
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using static Framework.Test.TestComponentViewer;

namespace Framework.Test
{
    public class TestTypePool : MonoBehaviour
    {
        public TypePool pool = new TypePool();

        public int[] _ints;
        public Array _intArray;

        // Start is called before the first frame update
        void Start()
        {
            _ints = pool.GetArray<int>(10);
            for (int i = 0; i < _ints.Length; i++)
                _ints[i] = i + 1;
            Log(_ints);

            _intArray = pool.GetArray(typeof(int), 5);
            for (int i = 0; i < _intArray.Length; i++)
                _intArray.SetValue(i + 1, i);
            Log(_intArray);
        }

        public void Log(IList a)
        {
            StringBuilder sb = new StringBuilder($"数组长度：{a.Count}。元素：");
            for (int i = 0; i < a.Count; i++)
            {
                sb.Append(a[i].ToString()).Append("，");
            }
            Debug.Log(sb.ToString());
        }
    }
}