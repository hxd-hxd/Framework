using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Runtime;
using System;

namespace Framework.Test
{
    public class TestPropertyVariable1 : MonoBehaviour
    {
        [TextArea(3, 10)]
        public int _int_TextArea;

        [Range(3, 10)]
        public string _string_Range;

        void Start()
        {

        }

    }
}