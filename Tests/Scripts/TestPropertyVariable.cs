using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Runtime;
using System;

namespace Framework.Test
{
    public class TestPropertyVariable : MonoBehaviour
    {
        public Test<int> _testPV_int;
        public Test<List<int>> _testPV_listInt;
        public Test<int[]> _testPV_arrayInt;
        
        public Test<string> _testPV_string;
        public Test<List<string>> _testPV_listString;
        public Test<string[]> _testPV_arrayString;
        
        public Test<Vector4> _testPV_vec4;
        public Test<List<Vector4>> _testPV_listVec4;
        public Test<Vector4[]> _testPV_arrayVec4;

        public int a;
        public int b;
        public PropertyVariable<Vector4> _pv_vec41;
        public int c;
        [TextArea(3, 10)]
        public int c_int_TextArea;

        [HideInInspector]
        [PropertyVariableHideEvent]
        public PropertyVariable<int> _pv_int_hide;

        [PropertyVariableHideEvent]
        public PropertyVariable<Vector4> _pv_vec41_HideEvent;

        [TextArea(3, 10)]
        public PropertyVariable<string> _pv_string_TextArea;

        [Range(0, 10)]
        public PropertyVariable<int> _pv_int_Range;

        [MinMaxRange(0, 10)]
        public PropertyVariable<int> _pv_int_MinMaxRange;
        [MinMaxRange(0, 10)]
        public PropertyVariable<Vector4> _pv_vec41_MinMaxRange;
        [MinMaxRange(0, 10)]
        public PropertyVariable<string> _pv_string_MinMaxRange;

        public PropertyVariable<Vector2> _pv_vec2;
        public PropertyVariable<Vector3> _pv_vec3;
        public PropertyVariable<Vector4> _pv_vec4;

        public PropertyVariable<Color> _pv_color;

        void Start()
        {

        }

        [Serializable]
        public class Test<T>
        {
            public T a;
            public T b;

            public PropertyVariable<T> _pv;

            public T c;

            [Tooltip("说明")]
            public PropertyVariable<T> _pv_Tooltip;

            [PropertyVariableHideEvent]
            public PropertyVariable<T> _pv_HideEvent;

            [MinMaxRange(0, 10)]
            public PropertyVariable<T> _pv_MinMaxRange;

            [MinMaxRange(0, 10)]
            [PropertyVariableHideEvent]
            public PropertyVariable<T> _pv_HideEvent_MinMaxRange;

            [Range(0, 10)]
            public PropertyVariable<T> _pv_Range;

            [Range(0, 10)]
            [PropertyVariableHideEvent]
            public PropertyVariable<T> _pv_HideEvent_Range;

            [Header("使用 TextArea")]
            [TextArea(3, 10)]
            public PropertyVariable<T> _pv_TextArea;

            [TextArea(3, 10)]
            [PropertyVariableHideEvent]
            public PropertyVariable<T> _pv_HideEvent_TextArea;

            [HideInInspector]
            [PropertyVariableHideEvent]
            public PropertyVariable<T> _pv_hide;

        }
    }
}