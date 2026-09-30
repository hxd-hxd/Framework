using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Framework.Test;

namespace Framework.Prob
{

    [CustomEditor(typeof(TestProb))]
    public class TestProbInspector : UnityEditor.Editor
    {
        TestProb testProb;
        IProb prob;
        static string tValue;
        static string branchName;
        float probValue;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUI.BeginDisabledGroup(!Application.isPlaying);
            {
                testProb = (TestProb)target;
                serializedObject.Update();

                EditorGUILayout.BeginVertical("box");
                {
                    EditorGUILayout.LabelField("获取概率项信息（调用 FindProbItem(string)）");
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.Space(10, false);
                        tValue = EditorGUILayout.TextField(tValue);
                        if (GUILayout.Button("获取信息", GUILayout.MinWidth(100)))
                        {
                            prob = testProb.pdd.FindItem(tValue);

                            probValue = prob == null ? 0 : prob.RealProb();
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.LabelField("获取分支信息（调用 FindProbItem(string)）");
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.Space(10, false);
                        branchName = EditorGUILayout.TextField(branchName);
                        if (GUILayout.Button("获取信息", GUILayout.MinWidth(100)))
                        {
                            prob = testProb.pdd.FindBranch(tValue);

                            probValue = prob == null ? 0 : prob.RealProb();
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.LabelField("（完整节点 调用 GetPath()）");
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.Space(25, false);
                        EditorGUILayout.LabelField("完整节点", "BoldLabel", GUILayout.MaxWidth(100));
                        EditorGUILayout.LabelField(prob == null ? "" : prob.GetPath());
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.LabelField("（真实概率 调用 RealProb()）");
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.Space(25, false);
                        EditorGUILayout.LabelField("真实概率", GUILayout.MaxWidth(100));
                        EditorGUILayout.LabelField(probValue.ToString() + "%");

                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.LabelField("此处随机 调用的是 GetRandomItem()");
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("随机"))
                    {
                        var item = testProb.pdd.GetRandomItem();
                        Debug.Log($"{item.value}，\t概率：{item.RealProb()}%，路径：{item.GetPath()}");
                    }
                    if (GUILayout.Button("随机 100 次"))
                    {
                        for (int i = 0; i < 100; i++)
                        {
                            var item = testProb.pdd.GetRandomItem();
                            Debug.Log($"{item.value}，\t概率：{item.RealProb()}%，路径：{item.GetPath()}");
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("保存成 json "))
                    {
                        testProb.SaveJson();
                        AssetDatabase.Refresh();
                    }
                    if (GUILayout.Button("读取 json "))
                    {
                        testProb.ReadJson();
                    }
                    if (GUILayout.Button("清除"))
                    {
                        testProb.Clear();
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.EndDisabledGroup();
        }

    }
}