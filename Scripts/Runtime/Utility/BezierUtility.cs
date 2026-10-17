// -------------------------
// 创建日期：2024/8/12 14:56:19
// -------------------------

using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>贝塞尔曲线</summary>
    public static class BezierUtility
    {
        /// <summary>计算曲线位置
        /// <para><paramref name="t"/>：曲线进度比率 0 到 1</para>
        /// </summary>
        public static Vector3 Bezier(Vector3 start, Vector3 mid, Vector3 end, float t)
        {
            Vector3 p0p1 = (1 - t) * start + t * mid;
            Vector3 p1p2 = (1 - t) * mid + t * end;
            Vector3 result = (1 - t) * p0p1 + t * p1p2;
            return result;
        }

        /// <summary>获取贝塞尔曲线路径，计算的路径点存入 paths</summary>
        public static void GetBezierPath(Vector3 start, Vector3 mid, Vector3 end, int step, List<Vector3> paths)
        {
            int num = step < 1 ? 1 : step;
            float u = 1f / num;
            for (float i = 0; i < 1; i += u)
            {
                var b = Bezier(start, mid, end, i);
                paths.Add(b);
            }
            paths.Add(end);
        }

        /// <summary>超过三个控制点的贝塞尔曲线</summary>
        public static List<Vector3> BezierCurveWithUnlimitPoints(Vector3[] handlesPositions, int vertexCount, List<Vector3> pointList)
        {
            for (float ratio = 0; ratio <= 1; ratio += 1.0f / vertexCount)
            {
                pointList.Add(UnlimitBezierCurve(handlesPositions, ratio));
            }
            pointList.Add(handlesPositions[handlesPositions.Length - 1]);

            return pointList;
        }

        public static Vector3 UnlimitBezierCurve(Vector3[] handlesPositions, float t)
        {
            int n = handlesPositions.Length - 1;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n - i; j++)
                {
                    handlesPositions[j] = Vector3.Lerp(handlesPositions[j], handlesPositions[j + 1], t);
                }
            }
            return handlesPositions[0];
        }
    }
}