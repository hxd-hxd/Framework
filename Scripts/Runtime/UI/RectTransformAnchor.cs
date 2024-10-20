// -------------------------
// 创建日期：2023/3/30 10:11:41
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Framework
{
    /// <summary>
    /// 操作锚点
    /// </summary>
    [ExecuteInEditMode]
    public class RectTransformAnchor : MonoBehaviour
    {
        RectTransform rt;

        public int width { get; private set; }
        public int height { get; private set; }

        private void Awake()
        {
            rt = GetComponent<RectTransform>();
        }

        void Start()
        {
            //Debug.Log($"屏幕分辨率：{Screen.width} x {Screen.height}");
            width = Screen.width;
            height = Screen.height;
        }

        void Update()
        {
            //Debug.Log($"屏幕分辨率：{Screen.width} x {Screen.height}");
            width = Screen.width;
            height = Screen.height;

            //UpdateAnchor();
        }

        // 刷新锚点
        void UpdateAnchor()
        {
            if (rt)
            {
                //Debug.Log($"width：{rt.rect.width}，height：{rt.rect.height}");
                Debug.Log($"Rect 位置：{rt.rect.position}，位置：{rt.position}"
                    + Environment.NewLine + $"width：{rt.rect.width}，height：{rt.rect.height}"
                    );
                Debug.Log($"相对于锚点的位置：{rt.anchoredPosition}");
                //rt.sizeDelta = Vector2.zero;


                // 锚点是从从屏幕的正中间起（原点）

                // 首先计算宽、高的一半
                float wHalf = rt.rect.width / 2;
                float hHalf = rt.rect.height / 2;


                // 接下来计算锚点四条边，即：上、下、左、右
                // 计算方式：
                // 锚点边 = （当前位置 +- 一半宽高） / 屏幕宽高

                // 锚点最大边
                // 计算：锚点边 = （当前位置 + 一半宽高） / 屏幕宽高
                Vector2 anchorMax = new Vector2((rt.position.x + wHalf) / Screen.width, (rt.position.y + hHalf) / Screen.height);
                rt.anchorMax = anchorMax;

                // 锚点最小边
                // 计算：锚点边 = （当前位置 - 一半宽高） / 屏幕宽高
                Vector2 anchorMin = new Vector2((rt.position.x - wHalf) / Screen.width, (rt.position.y - hHalf) / Screen.height);
                rt.anchorMin = anchorMin;
            }

        }

        /// <summary>
        /// 获取计算后的 <see cref="RectTransform.anchorMin"/>
        /// </summary>
        /// <returns></returns>
        public Vector2 GetAnchorMin()
        {
            // 首先计算宽、高的一半
            float wHalf = rt.rect.width / 2;
            float hHalf = rt.rect.height / 2;

            // 锚点最小边
            // 计算：锚点边 = （当前位置 - 一半宽高） / 屏幕宽高
            Vector2 anchorMin = new Vector2((rt.position.x - wHalf) / Screen.currentResolution.width, (rt.position.y - hHalf) / Screen.currentResolution.height);
            return anchorMin;
        }
        /// <summary>
        /// 获取计算后的 <see cref="RectTransform.anchorMax"/>
        /// </summary>
        /// <returns></returns>
        public Vector2 GetAnchorMax()
        {
            // 首先计算宽、高的一半
            float wHalf = rt.rect.width / 2;
            float hHalf = rt.rect.height / 2;

            // 锚点最大边
            // 计算：锚点边 = （当前位置 + 一半宽高） / 屏幕宽高
            Vector2 anchorMax = new Vector2((rt.position.x + wHalf) / Screen.currentResolution.width, (rt.position.y + hHalf) / Screen.currentResolution.height);
            return anchorMax;
        }

        public Vector2 GetScreenWH()
        {
            return new Vector2(Screen.width, Screen.height);
        }
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(RectTransformAnchor))]
    class RectTransformAnchorInspector : Editor
    {
        GUIStyle style = new GUIStyle();

        RectTransformAnchor my;
        RectTransform rt;

        Vector2 aMax, aMin;
        float width, height;
        bool aMaxEnable = true, aMinEnable = true;

        private void Awake()
        {
            style.normal.textColor = Color.white;
            style.wordWrap = true;
            style.stretchWidth = true;

            my = (RectTransformAnchor)target;

            rt = my.GetComponent<RectTransform>();
            aMax = rt.anchorMin;
            aMin = rt.anchorMax;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(16);

            #region 

            //// 检查锚点变换
            //EditorGUILayout.BeginHorizontal();
            //{
            //    EditorGUI.BeginChangeCheck();
            //    {
            //        GUI.enabled = aMaxEnable;
            //        aMin = EditorGUILayout.Vector2Field("锚点 Min", aMin);
            //        GUI.enabled = true;
            //    }
            //    if (EditorGUI.EndChangeCheck())
            //    {
            //        rt.anchorMin = aMin;
            //    }
            //    else
            //    {
            //        aMin = rt.anchorMin;
            //    }

            //    //    GUI.enabled = aMaxEnable;
            //    //var anchorMin = Vector2.zero;
            //    //Binding(ref aMin, ref anchorMin);
            //    //rt.anchorMin = anchorMin;
            //    //GUI.enabled = true;

            //    aMaxEnable = EditorGUILayout.Toggle(aMaxEnable, GUILayout.MaxWidth(32));
            //}
            //EditorGUILayout.EndHorizontal();

            //EditorGUILayout.BeginHorizontal();
            //{
            //    EditorGUI.BeginChangeCheck();
            //    {
            //        GUI.enabled = aMinEnable;
            //        aMax = EditorGUILayout.Vector2Field("锚点 Max", aMax);
            //        GUI.enabled = true;
            //    }
            //    if (EditorGUI.EndChangeCheck())
            //    {
            //        rt.anchorMax = aMax;
            //    }
            //    else
            //    {
            //        aMax = rt.anchorMax;
            //    }

            //    aMinEnable = EditorGUILayout.Toggle(aMinEnable, GUILayout.MaxWidth(32));
            //}
            //EditorGUILayout.EndHorizontal(); 
            #endregion


            GUILayout.Label("由于外部设置锚点，Unity会连同位置、宽高也修改，所以只能计算锚点值，自己手动将下列计算的值复制到 RectTransform 的 Anchors 以手动修改！", style);

            GUILayout.Space(8);

            // 计算锚点
            GUI.enabled = false;
            EditorGUILayout.Vector2Field("AnchorMin", my.GetAnchorMin());
            EditorGUILayout.Vector2Field("AnchorMax", my.GetAnchorMax());
            GUI.enabled = true;

            GUILayout.Space(8);

            if (GUILayout.Button("应用锚点", GUILayout.MinHeight(32)))
            {

            }

            GUILayout.Space(16);
            
            // 屏幕设定宽高
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("屏幕设定", GUILayout.MaxWidth(64));
                EditorGUILayout.LabelField("宽", GUILayout.MaxWidth(32));
                EditorGUILayout.SelectableLabel(Screen.currentResolution.width.ToString(), GUILayout.Width(64));
                EditorGUILayout.LabelField("高", GUILayout.MaxWidth(32));
                EditorGUILayout.SelectableLabel(Screen.currentResolution.height.ToString());
            }
            EditorGUILayout.EndHorizontal();

            // 屏幕宽高
            EditorGUILayout.BeginHorizontal();
            {
                //Debug.Log($"{Screen.width} x {Screen.height}");
                EditorGUILayout.LabelField("屏幕", GUILayout.MaxWidth(64));
                EditorGUILayout.LabelField("宽", GUILayout.MaxWidth(32));
                //EditorGUILayout.SelectableLabel(Screen.width.ToString(), GUILayout.Width(64));
                EditorGUILayout.SelectableLabel(my.width.ToString(), GUILayout.Width(64));
                EditorGUILayout.LabelField("高", GUILayout.MaxWidth(32));
                EditorGUILayout.SelectableLabel(my.height.ToString());
            }
            EditorGUILayout.EndHorizontal();

            // 宽高
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("当前UI", GUILayout.MaxWidth(64));
                EditorGUILayout.LabelField("宽", GUILayout.MaxWidth(32));
                EditorGUILayout.SelectableLabel(rt.rect.width.ToString(), GUILayout.Width(64));
                EditorGUILayout.LabelField("高", GUILayout.MaxWidth(32));
                EditorGUILayout.SelectableLabel(rt.rect.height.ToString());
            }
            EditorGUILayout.EndHorizontal();

            //if (GUILayout.Button("重置锚点", GUILayout.MinHeight(32)))
            //{
            //    rt.anchorMin = Vector2.one / 2;
            //    rt.anchorMax = Vector2.one / 2;
            //}
        }


        // 约束
        static void Binding(ref Vector2 value, ref Vector2 target)
        {
            EditorGUI.BeginChangeCheck();
            {
                value = EditorGUILayout.Vector2Field("锚点 Max", value);
            }
            if (EditorGUI.EndChangeCheck())
            {
                target = value;
            }
            else
            {
                value = target;
            }
        }
        // 约束
        static void Binding(ref float value, ref float target)
        {
            EditorGUI.BeginChangeCheck();
            {
                value = EditorGUILayout.FloatField("锚点 Max", value);
            }
            if (EditorGUI.EndChangeCheck())
            {
                target = value;
            }
            else
            {
                value = target;
            }
        }
    }
#endif

}