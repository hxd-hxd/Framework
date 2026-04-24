// -------------------------
// 创建日期：2024/9/19 10:13:20
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using Object = UnityEngine.Object;

namespace Framework.Editor
{
    public class ExportUnitypackage
    {
        const string Export_Expanded_Name = ".unitypackage";
        const string Export_File_Default_Name = "Unitypackage" + Export_Expanded_Name;

        const string Root_Menu = "Assets/导出 Package/";
        const int Root_MenuIndex = 21;

        //[MenuItem(Root_Menu + "导出", false, Root_MenuIndex)]
        //public static void Export()
        //{
        //    var objs = Selection.objects;
        //    var fs = objs.Select(obj => AssetDatabase.GetAssetPath(obj)).ToArray();
        //    string outPath = null;
        //    outPath = EditorUtility.OpenFolderPanel("选择", $"{Path.GetDirectoryName(Application.dataPath)}", Export_File_Default_Name);
        //    AssetDatabase.ExportPackage(fs, outPath);
        //}

        [MenuItem(Root_Menu + "快速导出到当前文件夹（不包含依赖）", false, Root_MenuIndex)]
        public static void ExportToCurrentDir()
        {
            var obj = Selection.activeObject;

            if (obj)
            {
                var f = AssetDatabase.GetAssetPath(obj);
                string outPath = $"{Path.GetDirectoryName(f)}/{Path.GetFileNameWithoutExtension(Path.GetFileName(f))}{Export_Expanded_Name}";
                AssetDatabase.ExportPackage(f, outPath, ExportPackageOptions.Recurse);

                AssetDatabase.Refresh();

                var uPack = AssetDatabase.LoadAssetAtPath<Object>(outPath);
                Selection.activeObject = uPack;
                AssetDatabase.Refresh();
            }
        }

        [MenuItem(Root_Menu + "导出选中的所有文件到当前文件夹（不包含依赖）", false, Root_MenuIndex)]
        public static void ExportSelectAllToCurrentDir()
        {
            var obj = Selection.activeObject;

            if (obj)
            {
                var f = AssetDatabase.GetAssetPath(obj);
                string packName = Path.GetFileNameWithoutExtension(Path.GetFileName(f));

                // 获取选中的所有文件
                //Selection.assetGUIDs.ToList().ForEach(guid =>
                //{
                //    var path = AssetDatabase.GUIDToAssetPath(guid);
                //    Debug.Log($"选择的资产路径: {path}");
                //});
                var selectAssets = Selection.assetGUIDs.Select((guid, indexer) =>
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    return path;
                }).ToArray();

                // 用户自定义包名
                var popup = new UserPackNamePopupWindow(packName, _ExportPackageFunc);
                Rect buttonRect = default;
                buttonRect.position = new Vector2(Screen.width / 2, Screen.height / 2);
                buttonRect.size = popup.GetWindowSize();
                PopupWindow.Show(buttonRect, popup);

                //_ExportPackageFunc(packName);

                void _ExportPackageFunc(string packName)
                {
                    string outPath = $"{Path.GetDirectoryName(f)}/{packName}{Export_Expanded_Name}";
                    AssetDatabase.ExportPackage(selectAssets, outPath, ExportPackageOptions.Recurse);

                    AssetDatabase.Refresh();

                    var uPack = AssetDatabase.LoadAssetAtPath<Object>(outPath);
                    Selection.activeObject = uPack;
                    AssetDatabase.Refresh();
                }
            }
        }

        public class UserPackNamePopupWindow : PopupWindowContent
        {
            private string packName;
            public Action<string> onPackNameEntered;

            public UserPackNamePopupWindow(string defaultPackName, Action<string> onPackNameEntered)
            {
                packName = defaultPackName;
                this.onPackNameEntered = onPackNameEntered;
            }
            
            public override Vector2 GetWindowSize()
            {
                return new Vector2(200, 100);
            }

            public override void OnGUI(Rect rect)
            {
                Init();

                GUILayout.Label("请输入包名:");
                packName = GUILayout.TextField(packName);
                if (GUILayout.Button("导出"))
                {
                    // 处理确定按钮点击事件
                    // 例如，获取输入的包名并执行导出操作
                    onPackNameEntered?.Invoke(packName);
                    this.editorWindow.Close();
                }
            }

            bool isInitialized = false;
            private void Init()
            {
                if (!isInitialized)
                {
                    isInitialized = true;
                    //var pos = editorWindow.position;
                    //pos.position = UnityEngine.Event.current.mousePosition;
                    ////pos.position = GUILayoutUtility.GetLastRect().position;
                    //editorWindow.position = pos;
                }
            }
        }
    }
}