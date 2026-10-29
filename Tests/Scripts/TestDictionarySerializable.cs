using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Test
{
    public class TestDictionarySerializable : MonoBehaviour
    {
        [ColorUsage(true, true)]
        public int _int;

        public DictionarySerializable<Vector3, GameObject> _v3_go;
        public DictionarySerializable<string, GameObject> _string_go;
        public DictionarySerializable<int, GameObject> _int_go;

        public DictionarySerializable<Vector3, TestItem> _v3_class;
        [TextArea]
        public DictionarySerializable<string, TestItem> _string_class;
        [Range(0, 100)]
        public DictionarySerializable<int, TestItem> _int_class;

        [Range(0, 100)]
        [TextArea]
        public DictionarySerializable<int, string> _int_string;

        [Range(0, 100)]
        public DictionarySerializable<int, int> _int_int;

        [TextArea]
        public DictionarySerializable<TestMode, string> _enum_string;

        public DictionarySerializable<string, AnimationCurve> _string_animCurve;

        [ColorUsage(true, true)]
        public DictionarySerializable<int, Color> _int_color;

        [GradientUsage(true, ColorSpace.Uninitialized)]
        public DictionarySerializable<int, Gradient> _int_gradient;

        public DictionarySerializable<int, LayerMask> _int_layerMask;

        public DictionarySerializable<TestKey, TestValue> _tKey_tValue;

        void Start()
        {
        
        }

        [Serializable]
        public class TestItem
        {
            public float _num;
            public GameObject _go;
        }

        [Serializable]
        public struct TestKey
        {
            public int _id;
            [TextArea]
            public string _name;
        }

        [Serializable]
        public struct TestValue
        {
            public GameObject _go;
        }

        [Flags]
        public enum TestMode
        {
            None,
            Mode1,
            Mode2,
        }
    }
}
