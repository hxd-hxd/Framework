// -------------------------
// 创建日期：2023/5/9 15:33:17
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using UnityEditor;
using UnityEditorInternal;
using UnityEditor.Compilation;

using HybridCLR.Editor;
using HybridCLR.Editor.Settings;

using FGUIUtility = Framework.Editor.GUIUtilityExtend;

#pragma warning disable IDE0051 // 删除未使用的私有成员
#pragma warning disable IDE0044 // 添加只读修饰符
#pragma warning disable 0414

namespace Framework.HybridCLRExpress
{
    using static AssembliesUtility;

    /// <summary>
    /// 处理由 HybridCLR 生成的程序集
    /// </summary>
    public class AssembliesWindow : EditorWindow
    {
        [MenuItem("Framework/HybridCLRAssembliesWindow")]
        static void Open()
        {
            var window = GetWindow<AssembliesWindow>("处理由 HybridCLR 生成的程序集");
            window.minSize = new Vector2(450, 450);
        }

        FlatFoldoutBranch rootFoldout;
        bool showAllDll = false;// 显示所有平台 dll
        bool onlyShowCurrentPlatformDll = true;// 只显示当前平台 dll
        bool dllCfgFoldout = false;
        Vector2 rootFoldoutSV = Vector2.zero;
        Vector2 fullSV = Vector2.zero;

        // 拷贝
        bool copyAllDll = false;        // 拷贝所有 DLL
        bool autoCopyUseDll = true;     // 自动拷贝 DLL 到使用目录
        bool copyCurPlatformDll = true; // 拷贝当前平台 DLL
        string copyPath { get => AssembliesUtility.cfg.copyPath; set => AssembliesUtility.cfg.copyPath = value; }
        string currentUsePathName { get => AssembliesUtility.cfg.currentUsePathName; set => AssembliesUtility.cfg.currentUsePathName = value; }
        //string copyPath = "Assets/HotUpdateAssemblies"; 

        // HybridCLR 设置
        private SerializedObject _HybridCLR_SO;
        private SerializedProperty _HybridCLR_OS;


        GUIStyle _labelStyle, _rightLabelStyleRed;
        GUIStyle whiteStyle = new GUIStyle();
        GUIStyle _whiteButtonStyle;
        GUIStyle _yellowButtonStyle;

        GUIStyle labelStyle
        {
            get
            {
                GUIStyle v = _labelStyle;
                if (v == null)
                {
                    v = new GUIStyle(EditorStyles.whiteLabel);
                    v.richText = true;
                }
                return _labelStyle = v;
            }
        }
        GUIStyle rightLabelStyleRed
        {
            get
            {
                GUIStyle v = _rightLabelStyleRed;
                if (v == null)
                {
                    v = "RightLabel";
                    v.normal.textColor = Color.red;
                    v.richText = true;
                }
                return _rightLabelStyleRed = v;
            }
        }
        GUIStyle whiteButtonStyle
        {
            get
            {
                GUIStyle v = _whiteButtonStyle;
                if (v == null)
                {
                    v = new GUIStyle(EditorStyles.miniButton);
                    v.normal.textColor = Color.white;
                    v.fontSize = 14;
                    v.stretchHeight = true;
                    v.fixedHeight = 32;
                }
                return _whiteButtonStyle = v;
            }
        }
        GUIStyle yellowButtonStyle
        {
            get
            {
                GUIStyle v = _yellowButtonStyle;
                if (v == null)
                {
                    v = new GUIStyle(whiteButtonStyle);
                }
                return _yellowButtonStyle = v;
            }
        }

        BuildTarget buildTarget => EditorUserBuildSettings.activeBuildTarget;

        private void OnEnable()
        {
            string dir = hotUpdateDLLDir;

            rootFoldout = new FlatFoldoutBranch(dir);
            rootFoldout.path = dir;
            //foreach (var b in rootFoldout.subBranchDic)
            //{
            //    rootFoldout.Expect(b.Key).foldout = b.Value.name == EditorUserBuildSettings.activeBuildTarget.ToString();
            //}

            // 样式
            whiteStyle.normal.textColor = Color.white;
            whiteStyle.fontSize = 14;

            //var cfg = AssembliesUtility.cfg; 
        }

        private void OnGUI()
        {
            fullSV = EditorGUILayout.BeginScrollView(fullSV);

            bool hotUpdateDLLDirExists = Directory.Exists(hotUpdateDLLDir);// 检查根目录是否存在

            if (GUILayout.Button("打开 HybridCLR 设置", whiteButtonStyle))
            {
                //SettingsService.OpenProjectSettings("Project/HybridCLR Settings");
                MenuProvider.OpenSettings();
            }

            GUILayout.Space(8);

            // 根目录
            EditorGUILayout.BeginVertical("box");
            {
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Label("热更新 dll 所在根目录", GUILayout.MaxWidth(141));
                    EditorGUILayout.LabelField(hotUpdateDLLDir);
                    if (!hotUpdateDLLDirExists) GUILayout.Label("不存在", "RightLabel", GUILayout.MaxWidth(48));
                    if (GUILayout.Button("打开目录", GUILayout.MaxWidth(72)))
                    {
                        EditorUtility.RevealInFinder(hotUpdateDLLDir);
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Space(16);
                    GUILayout.Label("当前平台", GUILayout.MaxWidth(125));
                    EditorGUILayout.LabelField(buildTarget.ToString());
                    if (!Directory.Exists(hotUpdateDLLDirCurPlatform)) GUILayout.Label("不存在", "RightLabel", GUILayout.MaxWidth(48));
                    if (GUILayout.Button("打开目录", GUILayout.MaxWidth(72)))
                    {
                        EditorUtility.RevealInFinder(hotUpdateDLLDirCurPlatform);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            GUILayout.Space(8);

            // DLL 资源文件
            dllCfgFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(dllCfgFoldout, "已配置的热更新 dll 文件");
            EditorGUILayout.EndFoldoutHeaderGroup();
            //EditorGUILayout.LabelField("已配置的热更新 dll 文件", EditorStyles.boldLabel);
            if (dllCfgFoldout)
            {
                var dlls = GetHotUpdateAssemblyFiles();
                EditorGUILayout.BeginVertical("box");
                {
                    foreach (var dllFile in dlls)
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            GUILayout.Space(16);
                            //GUILayout.Label(dllFile);
                            EditorGUILayout.LabelField(dllFile);
                            if (!File.Exists(dllFile)) GUILayout.Label("文件不存在", rightLabelStyleRed, GUILayout.MaxWidth(64));
                            if (GUILayout.Button("打开目录", GUILayout.MaxWidth(72)))
                            {
                                EditorUtility.RevealInFinder(dllFile);
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndVertical();
            }

            GUILayout.Space(8);

            // 拷贝操作
            EditorGUILayout.LabelField("拷贝热更新 dll 文件", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(16);
                GUILayout.Label("为方便统一做热更新，可将 dll 拷贝到 Assets 子目录下，用以配置 AssetsBundle", whiteStyle);
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(8);

            // 配置文件
            EditorGUILayout.ObjectField("相关保存配置", AssembliesUtility.cfg, AssembliesUtility.cfg.GetType(), false);

            //GUILayout.Space(8);

            // 拷贝
            //var r = EditorGUILayout.GetControlRect();
            //Debug.Log(r);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("热更新程序集作为资源存放的路径");
            GUILayout.Label("当前要使用的 dll 存放的路径名");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            copyPath = EditorGUILayout.TextArea(copyPath);
            currentUsePathName = EditorGUILayout.TextField(currentUsePathName);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            copyAllDll = EditorGUILayout.Toggle("拷贝所有 dll", copyAllDll);
            autoCopyUseDll = EditorGUILayout.Toggle("自动拷贝 DLL 到使用目录", autoCopyUseDll);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("拷贝 当前 平台 dll", whiteButtonStyle))
            {
                AssembliesUtility.CopyHotUpdateAssembliesToDir(copyPath, copyAllDll);
                if (autoCopyUseDll) AssembliesUtility.CopyToUseDir();

                AssetDatabase.Refresh();
            }
            if (GUILayout.Button("拷贝 所有 平台 dll", whiteButtonStyle))
            {
                AssembliesUtility.CopyAllHotUpdateAssembliesToDir(copyPath, copyAllDll);
                if (autoCopyUseDll) AssembliesUtility.CopyToUseDir();

                AssetDatabase.Refresh();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("拷贝当前平台 dll 到使用目录", whiteButtonStyle))
            {
                AssembliesUtility.CopyToUseDir();

                AssetDatabase.Refresh();
            }
            if (GUILayout.Button("清除使用目录", whiteButtonStyle))
            {
                AssembliesUtility.ClearUseDir();

                AssetDatabase.Refresh();
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(8);

            // 树状结构
            GUILayout.Label("查看 dll ", EditorStyles.boldLabel);
            //showAllDll = EditorGUILayout.ToggleLeft("显示所有 DLL", showAllDll);
            showAllDll = EditorGUILayout.Toggle("显示所有 dll", showAllDll);
            EditorGUILayout.BeginVertical("box");
            {
                rootFoldoutSV = EditorGUILayout.BeginScrollView(rootFoldoutSV);
                if (hotUpdateDLLDirExists)
                {
                    // 检查存在
                    ClearInvalidBranch(rootFoldout);

                    // 树状结构
                    BranchTree(rootFoldout, 0);
                }
                else
                {
                    GUILayout.Label("未生成任何 dll");
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndScrollView();
        }

        #region GUI

        static Texture _fileIconUnity;
        static Texture fileIconUnity
        {
            get
            {
                if (_fileIconUnity == null) _fileIconUnity = AssetDatabase.GetCachedIcon("Assets");
                return _fileIconUnity;
            }
        }

        // 显示树状结构分支
        public void BranchTree(FlatFoldoutBranch branchRoot, int level = 0)
        {
            var dllPlatformDirs = Directory.GetDirectories(branchRoot.path);// 获取所有子目录
            var dllPlatformFilesUsable = AssembliesUtility.GetDllFiles(branchRoot.path);// 获取过滤后的 dll 文件

            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(16 * level);// 计算前方空隙
            GUIContent folderGUI = null;
            //fileGUI = new GUIContent(branchRoot.name, fileIconUnity);

            bool folderEmpty = dllPlatformDirs.Length <= 0 && dllPlatformFilesUsable.Count <= 0;
            var dlls = GetHotUpdateAssemblyFiles();// 热更的 dll 文件
            if (branchRoot.IsVacancyBranch)
            {
                folderGUI = FGUIUtility.IconNew.FolderEmptyOn;
            }
            else
            {
                //if (branchRoot.name == "StandaloneWindows64")
                //{
                //    Debug.Log("");
                //}

                // 筛分
                // 检查是否包含热更新文件，以判断是否显示为空文件夹
                if (!showAllDll && buildTarget.ToString() != branchRoot.name)
                {
                    foreach (var dllFile in dllPlatformFilesUsable)
                    {
                        if (dlls.Contains(dllFile))
                        {
                            folderEmpty = false;
                            break;
                        }
                        folderEmpty = true;
                    }
                }

                if (folderEmpty)
                {
                    folderGUI = FGUIUtility.IconNew.FolderEmptyOn;// 空文件夹
                }
                else
                {
                    if (branchRoot.foldout) folderGUI = FGUIUtility.IconNew.FolderOpened;   // 展开
                    else folderGUI = FGUIUtility.IconNew.Folder;                            // 折叠
                }
            }
            folderGUI.tooltip = null;
            folderGUI.text = branchRoot.name;

            //Debug.Log($"检查 {branchRoot.name}  文件夹：{dllPlatformDirs.Length}  可用文件：{dllPlatformFilesUsable.Count}  是否空显示：{folderEmpty}");

            //if (folderEmpty)
            //{
            //    EditorGUILayout.LabelField(folderGUI);
            //}
            //else
            {
                //branchRoot.foldout = EditorGUILayout.Foldout(branchRoot.foldout, folderGUI, true);
                branchRoot.foldout = EditorGUILayout.BeginFoldoutHeaderGroup(branchRoot.foldout, folderGUI);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.EndHorizontal();

            // 目录分支
            if (branchRoot.foldout)
            {
                level++;
                foreach (var dllDir in dllPlatformDirs)
                {
                    string dllDirName = Path.GetFileName(dllDir);
                    var branch = branchRoot.Expect(dllDirName);
                    branch.path = dllDir;
                    branch.isBranch = true;

                    // 继续向下分支
                    BranchTree(branch, level);
                }

                // 非分支
                foreach (var dllFile in dllPlatformFilesUsable)
                {
                    string fileName = Path.GetFileName(dllFile);
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

                    bool hotUpdate = dlls.Contains(dllFile);

                    void dllShowItem_func()
                    {
                        // 分支
                        var branch = branchRoot.Expect(fileName);
                        branch.path = dllFile;
                        branch.isBranch = false;
                        branch.foldout = false;

                        // 程序集
                        //var assemblys = CompilationPipeline.GetAssemblies();
                        // 获取程序集定义文件路径
                        string asmdefPath = CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyName(fileNameWithoutExtension);// 根据名称获取 unity 的程序集定义文件
                        bool isAsmdef = !string.IsNullOrEmpty(asmdefPath);// 是否 unity 的程序集定义文件，否则是 dll

                        // gui
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(16 * level);

                        GUIContent icon = null;// 程序集图标
                        if (isAsmdef)
                            icon = FGUIUtility.IconNew.AssemblyDefinitionAsset;
                        else icon = FGUIUtility.IconNew.Assembly;
                        icon.tooltip = dllFile;
                        icon.text = branch.name
                        + (hotUpdate ? "  <color=green>(已配置热更新)</color>" : null);
                        //gui.text += branch.name;
                        EditorGUILayout.LabelField(icon, labelStyle);

                        /// 更改 HybridCLR 热更新程序集设置
                        EditorGUI.BeginChangeCheck();
                        bool t = EditorGUILayout.Toggle(hotUpdate, GUILayout.MaxWidth(16));
                        //HybridCLRSettings
                        //HybridCLRSettingsProvider
                        // 需要区分是 dll 文件还是 untiy 的程序集定义文件
                        if (EditorGUI.EndChangeCheck())
                        {
                            // 添加到热更新
                            if (t)
                            {
                                if (isAsmdef)
                                {
                                    var hotUpdateAssemblyDefinitions = new List<AssemblyDefinitionAsset>(HybridCLRSettings.Instance.hotUpdateAssemblyDefinitions);
                                    var ada = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(asmdefPath);
                                    hotUpdateAssemblyDefinitions.Add(ada);
                                    HybridCLRSettings.Instance.hotUpdateAssemblyDefinitions = hotUpdateAssemblyDefinitions.ToArray();
                                }
                                else
                                {
                                    var hotUpdateAssemblies = new List<string>(HybridCLRSettings.Instance.hotUpdateAssemblies);
                                    hotUpdateAssemblies.Add(fileNameWithoutExtension);
                                    HybridCLRSettings.Instance.hotUpdateAssemblies = hotUpdateAssemblies.ToArray();
                                }
                            }
                            // 从热更新移除
                            else
                            {
                                if (isAsmdef)
                                {
                                    var hotUpdateAssemblyDefinitions = new List<AssemblyDefinitionAsset>(HybridCLRSettings.Instance.hotUpdateAssemblyDefinitions);
                                    var ada = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(asmdefPath);
                                    hotUpdateAssemblyDefinitions.Remove(ada);
                                    HybridCLRSettings.Instance.hotUpdateAssemblyDefinitions = hotUpdateAssemblyDefinitions.ToArray();
                                }
                                else
                                {
                                    var hotUpdateAssemblies = new List<string>(HybridCLRSettings.Instance.hotUpdateAssemblies);
                                    hotUpdateAssemblies.Remove(fileNameWithoutExtension);
                                    HybridCLRSettings.Instance.hotUpdateAssemblies = hotUpdateAssemblies.ToArray();
                                }
                            }

                            HybridCLRSettings.Save();
                        }

                        EditorGUILayout.EndHorizontal();
                    };


                    //// 显示所有程序集
                    //dllShowItem_func();

                    if (showAllDll)
                    {
                        // 显示所有程序集
                        dllShowItem_func();
                    }
                    else
                    {
                        //// 只显示热更新程序集
                        //if (hotUpdate) dllShowItem_func();

                        // 只当前平台程序集
                        if (branchRoot.name == buildTarget.ToString()) dllShowItem_func();
                    }
                }
            }
            EditorGUILayout.EndVertical();
        }
        // dll 文件分支
        public void BranchTreeDll(FlatFoldoutBranch branchRoot)
        {
            string[] dllPlatformDirs = Directory.GetDirectories(branchRoot.path);// 获取所有子目录
            List<string> dllPlatformFilesUsable = AssembliesUtility.GetDllFiles(branchRoot.path);// 获取过滤后的所有 dll 文件

            bool folderEmpty = dllPlatformDirs.Length <= 0 && dllPlatformFilesUsable.Count <= 0;
            var dlls = GetHotUpdateAssemblyFiles();// 热更的 dll 文件
            if (branchRoot.IsVacancyBranch)
            {

            }
            else
            {
                // 筛分
                // 检查是否包含热更新文件，以判断是否显示为空文件夹
                if (!showAllDll)
                {
                    foreach (var dllFile in dllPlatformFilesUsable)
                    {
                        if (dlls.Contains(dllFile))
                        {
                            folderEmpty = false;
                            break;
                        }
                        folderEmpty = true;
                    }
                }

            }

            //Debug.Log($"检查 {branchRoot.name}  文件夹：{dllPlatformDirs.Length}  可用文件：{dllPlatformFilesUsable.Count}  是否空显示：{folderEmpty}");

            // 非分支
            foreach (var dllFile in dllPlatformFilesUsable)
            {
                string fileName = Path.GetFileName(dllFile);

                var branch = branchRoot.Expect(fileName);
                branch.path = dllFile;
                branch.isBranch = false;
                branch.foldout = false;
            }

            // 目录分支
            foreach (var dllDir in dllPlatformDirs)
            {
                string dllDirName = Path.GetFileName(dllDir);
                var branch = branchRoot.Expect(dllDirName);
                branch.path = dllDir;
                branch.isBranch = true;

                // 继续向下分支
                BranchTreeDll(branch);
            }
        }
        // 清除无效的分支
        public void ClearInvalidBranch(FlatFoldoutBranch branchRoot)
        {
            List<string> invalids = new List<string>();
            foreach (var item in branchRoot.subBranchDic)
            {
                var _b = item.Value as FlatFoldoutBranch;
                if (_b.isBranch)
                {
                    if (!Directory.Exists(_b.path))
                    {
                        invalids.Add(item.Key);
                    }

                    // 子分支
                    ClearInvalidBranch(_b);
                }
                else
                {
                    if (!File.Exists(_b.path))
                    {
                        invalids.Add(item.Key);
                    }
                }
            }

            // 移除
            branchRoot.Remove(invalids);
        }

        /// <summary>
        /// 平面结构折叠分支
        /// <para>用于做树状结构</para>
        /// </summary>
        [Serializable]
        public class FlatFoldoutBranch : FlatBranch<FlatFoldoutBranch>
        {

            protected bool _foldout = true;
            /// <summary>
            /// 折叠
            /// </summary>
            public bool foldout
            {
                get
                {
                    if (!isBranch) _foldout = false;
                    return _foldout;
                }
                set
                {
                    if (isBranch)
                        _foldout = value;
                }
            }
            public string path { get; set; }

            //public new Dictionary<string, FlatFoldoutBranch> subBranchDic { get; set; } = new Dictionary<string, FlatFoldoutBranch>();

            public FlatFoldoutBranch() : base()
            {

            }

            public FlatFoldoutBranch(string name) : base(name)
            {

            }

            /// <summary>
            /// 折叠所有
            /// </summary>
            /// <param name="isFoldout"></param>
            public void FoldoutAllBranch(bool isFoldout)
            {
                FoldoutAllBranch(this, isFoldout);
            }
            protected void FoldoutAllBranch(FlatFoldoutBranch branch, bool isFoldout)
            {
                if (branch == null) return;

                branch.foldout = isFoldout;
                foreach (var item in branch.subBranchDic)
                {
                    var ffb = item.Value as FlatFoldoutBranch;
                    if (ffb != null && ffb.isBranch)
                    {
                        FoldoutAllBranch(ffb, isFoldout);
                    }
                }
            }

            public new FlatFoldoutBranch Expect(string name)
            {
                return base.Expect<FlatFoldoutBranch>(name);
            }
        }

        /// <summary>
        /// 平面结构分支
        /// <para>用于做树状结构</para>
        /// </summary>
        [Serializable]
        public class FlatBranch<B> : Branch
            where B : Branch
        {
            public FlatBranch() : base()
            {

            }

            public FlatBranch(string name) : base(name)
            {

            }
        }

        /// <summary>
        /// 结构分支
        /// </summary>
        [Serializable]
        public class Branch
        {
            /// <summary>
            /// 是否分支
            /// </summary>
            public bool isBranch = true;

            protected string _name;

            protected int _id;

            public int id { get => _id; protected set => _id = value; }
            public string name { get => _name; protected set => _name = value; }

            public Dictionary<string, Branch> subBranchDic { get; set; } = new Dictionary<string, Branch>();

            public Branch this[string name]
            {
                get => Get(name);
                set => subBranchDic[name] = value;
            }
            public Branch this[int id]
            {
                get
                {
                    foreach (var item in subBranchDic)
                    {
                        if (item.Value.id == id)
                        {
                            return item.Value;
                        }
                    }
                    return null;
                }
                //set => subBranchDic[id.ToString()] = value;
            }
            /// <summary>
            /// 是否空分支
            /// </summary>
            public bool IsVacancyBranch
            {
                get
                {
                    if (isBranch)
                    {
                        return subBranchDic.Count < 0;
                    }
                    return true;
                }
            }

            #region 构造

            public Branch()
            {
                id = GetHashCode();
                name = id.ToString();
            }

            protected Branch(string name)
            {
                id = GetHashCode();
                _name = name;
            }

            protected Branch(int id)
            {
                _id = id;
                _name = id.ToString();
            }

            protected Branch(string name, int id)
            {
                _name = name;
                _id = id;
            }

            #endregion

            public virtual Branch Expect(string name)
            {
                return (Get(name) ?? Add(name));
            }
            public virtual B Expect<B>(string name) where B : Branch, new()
            {
                return (Get(name) ?? Add<B>(name)) as B;
            }

            public virtual Branch Get(string name)
            {
                return subBranchDic.ContainsKey(name) ? subBranchDic[name] : null;
            }
            public virtual B Get<B>(string name) where B : Branch
            {
                return Get(name) as B;
            }

            public virtual Branch Add(string name)
            {
                return Add(name);
            }
            public virtual B Add<B>(string name) where B : Branch, new()
            {
                if (subBranchDic.ContainsKey(name)) return this[name] as B;
                return Add(new B() { name = name });
            }
            public virtual B Add<B>(B branch) where B : Branch
            {
                string name = branch.name;
                if (!subBranchDic.ContainsKey(name))
                    subBranchDic.Add(name, branch);
                return branch;
            }

            public virtual void Remove(string name)
            {
                if (subBranchDic.ContainsKey(name))
                {
                    subBranchDic.Remove(name);
                }
            }
            public virtual void Remove(List<string> names)
            {
                foreach (var item in names)
                {
                    Remove(item);
                }
            }
            public virtual void Remove<B>(B branch) where B : Branch
            {
                if (subBranchDic.ContainsValue(branch))
                {
                    subBranchDic.Remove(branch.name);
                }
            }
        }
        #endregion

        #region HybridCLR


        #endregion
    }
}

#pragma warning restore IDE0044 // 添加只读修饰符
#pragma warning restore IDE0051 // 删除未使用的私有成员
#pragma warning restore 0414
