using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Framework.Editor;
using Framework.Test;

namespace Framework.TestEditor
{
    [CustomEditor(typeof(TestEditorResources))]
    public class TestEditorResourcesInspector : UnityEditor.Editor
    {
        List<Object> textAssetObjs1 = new List<Object>();
        List<Object> textAssetObjs2 = new List<Object>();
        List<Object> assetObjs1 = new List<Object>();
        List<TextAsset> textAssetTs1 = new List<TextAsset>();
        List<TextAsset> textAssetTs2 = new List<TextAsset>();

        Object asset1, asset2;
        TextAsset textAsset1, textAsset2;

        TestEditorResources my => (TestEditorResources)target;

        private void OnEnable()
        {
            Load();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var resName = my._resName;

            EditorGUILayout.Space();
            if (GUILayout.Button("测试加载资源"))
            {
                Load();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"通过资源名加载的资源：\"{resName}\" ", EditorStyles.boldLabel);
            EditorGUILayout.ObjectField("Load(string, Type = Object)", asset1, typeof(Object), false);
            EditorGUILayout.ObjectField("Load<T = TextAsset>(string)", textAsset1, typeof(TextAsset), false);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"加载所有同名资源：{textAssetObjs1.Count}");
            EditorGUI.indentLevel++;
            foreach (var item in textAssetObjs1)
            {
                EditorGUILayout.ObjectField(item, typeof(Object), false);
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.LabelField($"加载所有同名资源：{textAssetTs1.Count}");
            EditorGUI.indentLevel++;
            foreach (var item in textAssetTs1)
            {
                EditorGUILayout.ObjectField(item, typeof(TextAsset), false);
            }
            EditorGUI.indentLevel--;


            EditorGUILayout.LabelField($"通过资源类型加载的资源", EditorStyles.boldLabel);

            EditorGUILayout.ObjectField("Load(Type = Object)", asset2, typeof(Object), false);
            EditorGUILayout.ObjectField("Load<T = TextAsset>()", textAsset2, typeof(TextAsset), false);

            EditorGUILayout.LabelField($"加载所有同类型资源（Object）：{textAssetObjs2.Count}");
            EditorGUI.indentLevel++;
            foreach (var item in textAssetObjs2)
            {
                EditorGUILayout.ObjectField(item, typeof(Object), false);
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.LabelField($"加载所有同类型资源（TextAsset）：{textAssetTs2.Count}");
            EditorGUI.indentLevel++;
            foreach (var item in textAssetTs2)
            {
                EditorGUILayout.ObjectField(item, typeof(TextAsset), false);
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.LabelField($"加载指定目录（TestCfg）下所有同类型资源（Object）：{assetObjs1.Count}");
            EditorGUI.indentLevel++;
            foreach (var item in assetObjs1)
            {
                EditorGUILayout.ObjectField(item, typeof(TextAsset), false);
            }
            EditorGUI.indentLevel--;
        }

        private void Load()
        {
            var resName = my._resName;

            // 资源名加载
            asset1 = EditorResources.Load(resName, typeof(Object));
            textAsset1 = EditorResources.Load<TextAsset>(resName);

            textAssetObjs1.Clear();
            EditorResources.LoadAll(resName, typeof(Object), ref textAssetObjs1);

            textAssetTs1.Clear();
            EditorResources.LoadAll(resName, ref textAssetTs1);

            // 资源类型加载
            asset2 = EditorResources.Load(typeof(Object));
            textAsset2 = EditorResources.Load<TextAsset>();

            textAssetObjs2.Clear();
            EditorResources.LoadAll(typeof(Object), ref textAssetObjs2);

            textAssetTs2.Clear();
            EditorResources.LoadAll(ref textAssetTs2);

            assetObjs1.Clear();
            EditorResources.LoadAllAtDir("TestCfg", ref assetObjs1);

        }
    }
}
