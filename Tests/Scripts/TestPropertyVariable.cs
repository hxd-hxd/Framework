using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Runtime;

namespace Framework.Test
{
    public class TestPropertyVariable : MonoBehaviour
    {
        public PropertyVariable<Vector4> _pv_vec41;

        public PropertyVariable<string> _pv_string;
        public PropertyVariable<int> _pv_int;

        public PropertyVariable<Vector2> _pv_vec2;
        public PropertyVariable<Vector3> _pv_vec3;
        public PropertyVariable<Vector4> _pv_vec4;

        public PropertyVariable<Color> _pv_color;

        // Start is called before the first frame update
        void Start()
        {

        }

    }
}