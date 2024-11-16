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
using System.IO.Compression;

namespace Framework.Editor
{
    public class ExportZipPackage
    {
        const string Export_Expanded_Name = ".zip";
        const string Export_File_Default_Name = "Unitypackage" + Export_Expanded_Name;

        const string Root_Menu = "Assets/导出 Zip/";
        const int Root_MenuIndex = 22;

        [MenuItem(Root_Menu + "导出到当前文件夹（不包含依赖）", false, Root_MenuIndex)]
        public static void ExportToCurrentDir()
        {
            var obj = Selection.activeObject;

            if (obj)
            {
                var f = AssetDatabase.GetAssetPath(obj);
                string outPath = $"{Path.GetDirectoryName(f)}/{Path.GetFileNameWithoutExtension(Path.GetFileName(f))}{Export_Expanded_Name}";

                //AssetDatabase.ExportPackage(f, outPath, ExportPackageOptions.Recurse);

                if(File.Exists(outPath)) File.Delete(outPath);
                ZipFile.CreateFromDirectory(f, outPath);

                AssetDatabase.Refresh();

                var uPack = AssetDatabase.LoadAssetAtPath<Object>(outPath);
                Selection.activeObject = uPack;
                AssetDatabase.Refresh();
            }
        }
    }
}