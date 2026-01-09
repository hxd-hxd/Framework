// -------------------------
// 创建日期：2022/10/27 10:12:48
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Object = UnityEngine.Object;
using System.Threading.Tasks;

namespace Framework
{
    /// <summary>
    /// UGUI 界面管理器，用于管理派生自 <see cref="UIPanelBase"/> 的界面
    /// </summary>
    public class UIPanelManager : Singleton<UIPanelManager>
    {
        public static string panelLoadPath = "Prefabs/UI/";

        static Dictionary<Type, UIPanelBase> panelDic = new Dictionary<Type, UIPanelBase>();

        /// <summary>
        /// 主画布
        /// </summary>
        private static Canvas mainCanvas;
        private static bool _findMainCanvas = true;

        static GameObject otherUIParent;

        static int maxSortingOrder = -1;
        /// <summary>
        /// 画布排序上限
        /// <para>ps：-1：表示无上限</para>
        /// </summary>
        public static int MaxSortingOrder { get => maxSortingOrder; set => maxSortingOrder = value; }
        /// <summary>
        /// 其他 UI 的父节点（其他 UI 界面会加载到此节点下）
        /// </summary>
        public static GameObject OtherUIParent
        {
            get
            {
                if (!otherUIParent)// 检查是否存在
                {
                    otherUIParent = new GameObject("OtherUIParent");
                    Object.DontDestroyOnLoad(otherUIParent);
                }
                return otherUIParent;
            }
            set => otherUIParent = value;
        }
        /// <summary>
        /// 主画布
        /// </summary>
        public static Canvas MainCanvas
        {
            get
            {
                if (_findMainCanvas)
                {
                    _findMainCanvas = false;
                    if (!mainCanvas)
                    {
                        MainCanvas mc;
#if UNITY_2020_3_OR_NEWER
                        mc = Object.FindObjectOfType<MainCanvas>(true);
#else
                    mc = Object.FindObjectOfType<MainCanvas>();
#endif
                        if (mc)
                            mainCanvas = mc.m_mainCanvas;
                    }
                }

                //if (!mainCanvas)
                //{
                //    var mc = ResourcesManager.Load<GameObject>(Path.Combine(panelLoadPath, "MainCanvas"));
                //    if (mc)
                //        mainCanvas = Object.Instantiate(mc).GetComponent<Canvas>();
                //}

                return mainCanvas;
            }
            set
            {
                mainCanvas = value;
            }
        }

        /// <summary>
        /// 指定界面是否存在
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool ExistPanel<T>() where T : UIPanelBase
        {
            UIPanelBase panel = GetPanel(typeof(T));
            return panel != null;
        }
        /// <summary>
        /// 指定界面是否存在
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool ExistPanel(Type type)
        {
            UIPanelBase panel = GetPanel(type);
            return panel != null;
        }

        /// <summary>
        /// 注册界面
        /// </summary>
        /// <param name="panel"></param>
        public static void Register<T>(T panel) where T : UIPanelBase
        {
            Register(panel.GetType(), panel);
        }
        /// <summary>
        /// 注册界面
        /// </summary>
        /// <param name="panel"></param>
        public static void Register(Type type, UIPanelBase panel)
        {
            if (!panelDic.ContainsKey(type))
            {
                panelDic.Add(type, panel);

                //Debug.Log($"界面 {panel.name} 注册成功！");
            }
            else
            {
                if (panelDic[type] == panel) return;

                //Debug.Log($"已注册过界面 {panel.name} ， 旧界面为 {panelDic[type]} ， 将替换旧的已注册界面！");

                panelDic[type] = panel;
            }
        }

        /// <summary>
        /// 获取界面
        /// <para>ps：预制体名称要与类名一致，才能自动加载成功</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetPanel<T>() where T : UIPanelBase
        {
            UIPanelBase panel = GetPanel(typeof(T));

            if (panel) return (T)panel;

            return null;
        }
        /// <summary>
        /// 获取界面
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetName">用于加载，界面资产名</param>
        /// <returns></returns>
        public static T GetPanel<T>(string assetName) where T : UIPanelBase
        {
            UIPanelBase panel = GetPanel(typeof(T), assetName);

            if (panel) return (T)panel;

            return null;
        }
        /// <summary>
        /// 获取界面
        /// <para>ps：预制体名称要与类名一致，才能自动加载成功</para>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static UIPanelBase GetPanel(Type type)
        {
            return GetPanel(type, type.Name);
        }
        /// <summary>
        /// 获取界面
        /// </summary>
        /// <param name="type"></param>
        /// <param name="assetName">界面资产名</param>
        /// <returns></returns>
        public static UIPanelBase GetPanel(Type type, string assetName)
        {
            panelDic.TryGetValue(type, out UIPanelBase panel);

            if (!panel)
            {
                // 查看是否在场景中但未注册
                panel = GameObject.FindObjectOfType(type, true) as UIPanelBase;

                if (!panel)
                {
                    // 如果场景中没有，就实例化出来
                    string path = assetName;
                    GameObject prefab = ResourcesManager.LoadAssetObject<GameObject>(path);
                    if (prefab != null)
                    {
                        panel = InstantiatePanel(prefab);
                    }
                    else
                    {
                        Log.Warning($"UI 界面实例化失败，请检查是否包含资源 <color=yellow>“{type.Name}”</color>");
                    }
                }

                if (panel)
                {
                    Register(type, panel);

                    GetCanvas(panel);
                }
            }
            else
            {
                if (panel.IsOccupy)
                {
                    panel = null;
                    Log.Warning($"尝试获取的界面 {type.Name} 已被占用，获取失败");
                }
            }

            return panel;
        }

        /// <summary>
        /// 获取界面
        /// <para>ps：预制体名称要与类名一致，才能自动加载成功</para>
        /// </summary>
        public static async Task<T> GetPanelAsync<T>() where T : UIPanelBase
        {
            UIPanelBase panel = await GetPanelAsync(typeof(T));

            if (panel) return (T)panel;

            return null;
        }
        /// <summary>
        /// 获取界面
        /// </summary>
        /// <param name="assetName">用于加载，界面资产名</param>
        public static async Task<T> GetPanelAsync<T>(string assetName) where T : UIPanelBase
        {
            UIPanelBase panel = await GetPanelAsync(typeof(T), assetName);

            if (panel) return (T)panel;

            return null;
        }
        /// <summary>
        /// 获取界面
        /// <para>ps：预制体名称要与类名一致，才能自动加载成功</para>
        /// </summary>
        public static async Task<UIPanelBase> GetPanelAsync(Type type)
        {
            return await GetPanelAsync(type, type.Name);
        }
        /// <summary>
        /// 获取界面
        /// </summary>
        /// <param name="assetName">界面资产名</param>
        public static async Task<UIPanelBase> GetPanelAsync(Type type, string assetName)
        {
            panelDic.TryGetValue(type, out UIPanelBase panel);

            if (!panel)
            {
                // 查看是否在场景中但未注册
                panel = GameObject.FindObjectOfType(type, true) as UIPanelBase;

                if (!panel)
                {
                    // 如果场景中没有，就实例化出来
                    string path = assetName;
                    var opertion = ResourcesManager.LoadAssetAsync<GameObject>(path);
                    await opertion.Task;
                    GameObject prefab = (GameObject)opertion.AssetObject;

                    if (prefab != null)
                    {
                        panel = InstantiatePanel(prefab);
                    }
                    else
                    {
                        Log.Warning($"UI 界面实例化失败，请检查是否包含资源 <color=yellow>“{type.Name}”</color>");
                    }

                    opertion.Release();
                }

                if (panel)
                {
                    Register(type, panel);

                    GetCanvas(panel);
                }
            }
            else
            {
                if (panel.IsOccupy)
                {
                    panel = null;
                    Log.Warning($"尝试获取的界面 {type.Name} 已被占用，获取失败");
                }
            }

            return panel;
        }
         
        private static UIPanelBase InstantiatePanel(GameObject temlate)
        {
            Transform p = null;
            if (MainCanvas)
                p = MainCanvas.transform;// 将新界面设置到 UI 节点
            GameObject go = Object.Instantiate(temlate, p);

            //GameObject go = Object.Instantiate(temlate);
            ////go.transform.SetParent(OtherUIParent.transform);// 将新界面设置到 UI 节点
            //if (MainCanvas)
            //{
            //    go.transform.SetParent(MainCanvas.transform);// 将新界面设置到 UI 节点
            //    go.transform.localScale = Vector3.one;
            //    go.transform.localPosition = Vector3.zero;

            //    var rectT = go.GetComponent<RectTransform>();
            //    if (rectT)
            //    {
            //        rectT.pivot = new Vector2(0.5f, 0.5f);
            //        rectT.anchorMax = Vector2.one;
            //    }
            //}

            var panel = go.GetComponent<UIPanelBase>();
            if (!panel) panel = go.GetComponentInChildren<UIPanelBase>(true);
            return panel;
        }

        /// <summary>
        /// 获取画布
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Canvas GetCanvas<T>() where T : UIPanelBase
        {
            return GetCanvas(typeof(T));
        }
        /// <summary>
        /// 获取画布
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Canvas GetCanvas(Type type)
        {
            panelDic.TryGetValue(type, out UIPanelBase panel);

            return GetCanvas(panel);
        }
        /// <summary>
        /// 获取画布
        /// </summary>
        /// <param name="panel"></param>
        /// <returns></returns>
        public static Canvas GetCanvas(UIPanelBase panel)
        {
            // 检查是否自带 画布 的界面
            Canvas canvas = panel.canvas;

            if (!canvas)
                canvas = panel.GetComponent<Canvas>();

            if (!canvas)
                canvas = panel.GetComponentInParent<Canvas>();// 检查画布是否在父节点上

            if (!canvas)
                canvas = panel.GetComponentInChildren<Canvas>();// 检查画布是否在子节点上

            if (canvas)
            {
                if (!panel.canvas) panel.canvas = canvas;
            }
            else
            {
                panel.canvas = MainCanvas;
                panel.transform.SetParent(MainCanvas?.transform);// 设置到主画布下
            }

            return canvas;
        }

        /// <summary>
        /// 销毁界面
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool DestroyPanel<T>() where T : UIPanelBase
        {
            return DestroyPanel(typeof(T));
        }
        /// <summary>
        /// 销毁界面
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool DestroyPanel(Type type)
        {
            panelDic.TryGetValue(type, out UIPanelBase panel);

            return DestroyPanel(panel);
        }
        /// <summary>
        /// 销毁界面
        /// </summary>
        /// <param name="panel"></param>
        /// <returns></returns>
        public static bool DestroyPanel(UIPanelBase panel)
        {
            if (panel)
            {
                RemovePanel(panel.GetType());
                Object.Destroy(panel.gameObject);
                return true;
            }

            Debug.LogWarning($"试图销毁 不存在的界面 {panel}");

            return false;
        }

        /// <summary>
        /// 移除界面
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool RemovePanel<T>() where T : UIPanelBase
        {
            return RemovePanel(typeof(T));
        }
        /// <summary>
        /// 移除界面
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool RemovePanel(Type type)
        {
            if (panelDic.ContainsKey(type))
            {
                return panelDic.Remove(type);
            }

            Debug.LogWarning($"试图移除 不存在的界面 {type.Name}");

            return false;
        }
        /// <summary>
        /// 移除界面
        /// </summary>
        /// <param name="panel"></param>
        /// <returns></returns>
        public static bool RemovePanel(UIPanelBase panel)
        {
            return RemovePanel(panel.GetType());
        }

        /// <summary>
        /// 将指定界面调整到最上层
        /// </summary>
        public static void PanelTopmost<T>() where T : UIPanelBase
        {
            PanelTopmost(typeof(T));
        }
        /// <summary>
        /// 将指定界面调整到最上层
        /// </summary>
        /// <param name="type"></param>
        public static void PanelTopmost(Type type)
        {
            if (panelDic.TryGetValue(type, out UIPanelBase panel))
            {
                PanelTopmost(panel);
            }
        }
        /// <summary>
        /// 将指定界面调整到最上层
        /// </summary>
        /// <param name="panel"></param>
        public static void PanelTopmost(UIPanelBase panel)
        {
            PanelTopmost(panel, false, true);
        }
        /// <summary>
        /// 将指定界面调整到最上层
        /// </summary>
        public static void PanelTopmost(UIPanelBase panel, bool ignoreSortOrderLevel)
        {
            PanelTopmost(panel, false, ignoreSortOrderLevel);
        }
        /// <summary>
        /// 将指定界面调整到最上层
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="includeAllCanvas">包括所有 canvas（将所有 canvas 纳入计算范围，效率不高，谨慎频繁使用）</param>
        /// <param name="ignoreSortOrderLevel">
        /// 忽略排序等级的影响
        /// <para>注意：flase：会以此界面作为分水岭</para>
        /// </param>
        public static void PanelTopmost(UIPanelBase panel, bool includeAllCanvas, bool ignoreSortOrderLevel)
        {
            if (!panel)
            {
                Debug.LogWarning($"试图调整 不存在的界面 {panel}");
                return;
            }

            int max = panel.canvas.sortingOrder;

            if (includeAllCanvas)// 是否将所有 canvas 纳入到排序范围
            {
                Canvas[] canvasArr = Object.FindObjectsOfType<Canvas>();
                for (int i = 0; i < canvasArr.Length; i++)
                {
                    Canvas canvas = canvasArr[i];
                    if (canvas == panel.canvas) continue;

                    // 考虑 SortOrderLevel 的影响


                    int sortingOrder = canvas.sortingOrder;
                    if (sortingOrder >= max)
                    {
                        max = sortingOrder + 1;
                    }
                }
            }
            else
            {
                // 调整 Canvas.SortOrder
                foreach (var item in panelDic)
                {
                    if (item.Value == null) continue;
                    if (item.Value == panel) continue;
                    if (item.Value.canvas == null) continue;
                    if (item.Value.canvas == panel.canvas) continue;

                    int sortingOrder = item.Value.canvas.sortingOrder;

                    if (ignoreSortOrderLevel)
                    {
                        if (item.Value.gameObject.activeSelf && sortingOrder >= max)
                        {
                            max = sortingOrder + 1;
                        }
                    }
                    else
                    {
                        // 只在同级、低级中排序
                        // 这里要做分水岭处理
                        if (item.Value.SortOrderLevel > panel.SortOrderLevel)
                        {

                            continue;
                        }

                    }
                }
            }

            if (MaxSortingOrder >= 0)
            {
                max = MaxSortingOrder;
            }

            panel.canvas.sortingOrder = max;

            if (ignoreSortOrderLevel)
            {
                // 调整顺序到最后
                panel.transform.SetAsLastSibling();
            }
            else
            {
                // 考虑到 SortOrderLevel 不能直接调整顺序到最后
                foreach (var item in panelDic)
                {
                    if (item.Value == null) continue;
                    if (item.Value == panel) continue;
                    if (item.Value.canvas == null) continue;
                    if (item.Value.canvas != panel.canvas) continue;

                    int panelSiblingIndex = panel.transform.GetSiblingIndex();
                    int otherSiblingIndex = item.Value.transform.GetSiblingIndex();
                    int siblingIndex = 0;

                    // 这里要做分水岭处理
                    if (item.Value.SortOrderLevel > panel.SortOrderLevel)
                    {
                        if (panelSiblingIndex > otherSiblingIndex)
                        {

                        }
                    }

                    //item.Value.transform.SetSiblingIndex();
                }
            }
        }

        /// <summary>
        /// 检查是否阻挡射线的界面启用
        /// </summary>
        /// <returns></returns>
        public static bool HasResistRayPanelEnable()
        {
            bool h = ResistRayPanelEnableNum() > 0;
            return h;
        }
        /// <summary>
        /// 获取是否有阻挡射线的界面启用的数量
        /// </summary>
        /// <returns></returns>
        public static int ResistRayPanelEnableNum()
        {
            int num = 0;
            foreach (var item in panelDic)
            {
                if (item.Value == null) continue;
                if (item.Value.IsResistRay)
                {
                    num++;
                }
            }
            return num;
        }

        /// <summary>
        /// 检查是否有非常驻界面启用
        /// </summary>
        /// <returns></returns>
        public static bool HasNoPermanentPanelEnable()
        {
            bool h = NoPermanentPanelEnableNum() > 0;
            return h;
        }

        /// <summary>
        /// 获取非常驻界面启用的数量
        /// </summary>
        /// <returns></returns>
        public static int NoPermanentPanelEnableNum()
        {
            int num = 0;
            foreach (var item in panelDic)
            {
                if (item.Value == null) continue;
                if (item.Value.Permanent && item.Value.isEnable)
                {
                    num++;
                }
            }
            return num;
        }

        /// <summary>
        /// 获取非常驻界面数量
        /// </summary>
        /// <returns></returns>
        public static int GetNoPermanentPanelNum()
        {
            int num = 0;
            foreach (var item in panelDic)
            {
                if (!item.Value.Permanent)
                {
                    num++;
                }
            }
            return num;
        }

    }


    public static class UIPanelManagerExtend
    {

        /// <summary>
        /// 获取画布
        /// </summary>
        /// <param name="panel"></param>
        /// <returns></returns>
        public static Canvas GetCanvas(this UIPanelBase panel)
        {
            return UIPanelManager.GetCanvas(panel);
        }

        /// <summary>
        /// 销毁界面
        /// </summary>
        /// <param name="panel"></param>
        /// <returns></returns>
        public static bool DestroyPanel(this UIPanelBase panel)
        {
            return UIPanelManager.DestroyPanel(panel);
        }

        /// <summary>
        /// 移除界面
        /// </summary>
        /// <param name="panel"></param>
        /// <returns></returns>
        public static bool RemovePanel(this UIPanelBase panel)
        {
            return UIPanelManager.DestroyPanel(panel);
        }

        /// <summary>
        /// 将指定界面调整到最上层
        /// </summary>
        /// <param name="panel"></param>
        public static void PanelTopmost(this UIPanelBase panel)
        {
            UIPanelManager.PanelTopmost(panel, false, true);
        }

        /// <summary>
        /// 将指定界面调整到最上层
        /// </summary>
        public static void PanelTopmost(this UIPanelBase panel, bool ignoreSortOrderLevel)
        {
            UIPanelManager.PanelTopmost(panel, false, ignoreSortOrderLevel);
        }

        /// <summary>
        /// 将指定界面调整到最上层
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="includeAllCanvas">包括所有 canvas（将所有 canvas 纳入计算范围，效率不高，谨慎频繁使用）</param>
        /// <param name="ignoreSortOrderLevel">
        /// 忽略排序等级的影响
        /// <para>注意：flase：会以此界面作为分水岭</para>
        /// </param>
        public static void PanelTopmost(this UIPanelBase panel, bool includeAllCanvas, bool ignoreSortOrderLevel)
        {
            UIPanelManager.PanelTopmost(panel, includeAllCanvas, ignoreSortOrderLevel);
        }

    }
}