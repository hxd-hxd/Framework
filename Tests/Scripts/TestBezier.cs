using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Test
{
    public class TestBezier : MonoBehaviour
    {
        public Transform start, mid, end;
        public int count = 10;

        private List<Vector3> paths = new List<Vector3>();

        void Start()
        {
            
        }

        void Update()
        {
            paths.Clear();
            BezierUtility.GetBezierPath(start, mid, end, count, paths);
            if (paths.Count > 1)
            {
                for (int i = 0; i < paths.Count - 1; i++)
                {
                    Debug.DrawLine(paths[i], paths[i + 1], Color.green);
                }
            }
        }

        public class Spline
        {
            public int density = 10;

            public List<SplinePoint> points = new List<SplinePoint>();


        }

        public class SplinePoint
        {
            public Vector3 point;
            public Vector3 lHandle, rHandle;

        }
    }
}
