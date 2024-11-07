// -------------------------
// 创建日期：2023/5/15 17:24:28
// -------------------------

#pragma warning disable 0414
#pragma warning disable 0219

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Framework.Editor
{
    /*
	 * 需配合 Framework.Editor.GUIUtilityExtend 使用
	 */

    /// <summary>
    /// 查看 untiy 图标
    /// </summary>
    public class GUIIconViewer : EditorWindow
	{
	    /*
	    * 这里的菜单路径自行修改
	    */
	    [MenuItem("Framework/Window/Unity 内置图标查看器")]
	    static void Open()
	    {
	        GUIIconViewer my = GetWindow<GUIIconViewer>("Unity 内置图标");
	        my.minSize = new Vector2(900, 800);
	
	    }
	
	    // 滑动条
	    Vector2 sv = Vector2.zero;
	    // 搜索
	    private string search = "";
	    private int currentNum;// 当前数量
	    // 图标
	    private List<GUIContent> icons;
	
	    // 窗口打开时调用
	    private void OnEnable()
	    {
			icons = GUIUtilityExtend.GetUnityIcons();
            //icons.OrderBy(p => p.tooltip.Substring(0, 1)).GroupBy(p => p.tooltip.Substring(0, 1)).ToList();
        }
	
	    // 编辑器 UI
	    private void OnGUI()
	    {
			GUILayout.Label("需配合 Framework.Editor.GUIUtility 使用\r\n点击一下图标可复制 名称，通过 unityIcons 或 unityIconDic 可获取对应名称的 GUIContent");
            GUILayout.Space(8);

            // 搜索
            currentNum = 0;
	        GUILayout.BeginHorizontal("HelpBox");
	        {
	            GUILayout.Space(10);
	            EditorGUILayout.LabelField("搜索", GUILayout.MaxWidth(36));
	            search = EditorGUILayout.TextField("", search, "SearchTextField", GUILayout.MaxWidth(400));
	
	            foreach (var item in icons)
	            {
	                if (item.tooltip.ToLower().Contains(search.ToLower()))
	                {
	                    currentNum++;
	                }
	            }
	            EditorGUILayout.LabelField(string.Format("{0}/{1}", currentNum, icons.Count));
	            //GUILayout.Label("取消", "SearchCancelButtonEmpty");
	            if (!string.IsNullOrEmpty(search))
	                if (GUILayout.Button("", "SearchCancelButton"))
	                {
	                    search = "";
	                }
	        }
	        GUILayout.EndHorizontal();
	
	        sv = EditorGUILayout.BeginScrollView(sv);
	        //Debug.Log(sv);
	
	        int num = 0;
	        int noeLineNum = 20;
	        for (int i = 0; i < icons.Count; i += noeLineNum)
	        {
	            GUILayout.BeginHorizontal();
	            for (int j = 0; j < noeLineNum; j++)
	            {
	                if (i + j < icons.Count)
	                {
	                    num++;
	                    var item = icons[i + j];
						//item.text = (i + j).ToString();
	                    if (item.tooltip.ToLower().Contains(search.ToLower()))
	                    {
							//GUILayout.Label((i + j).ToString(), GUILayout.MinWidth(34));
	                        if(GUILayout.Button(item, GUILayout.Width(40), GUILayout.Height(40)))
	                        {
	                            TextEditor textEditor = new TextEditor();
	                            textEditor.text = item.tooltip;
	                            textEditor.OnFocus();
	                            textEditor.Copy();

								Debug.Log($"{textEditor.text}  已复制到剪切板");
	                        }
	
	                    }
	                }
	            }
	            GUILayout.EndHorizontal();
	        }
	
	        EditorGUILayout.EndScrollView();
	    }
	}
}