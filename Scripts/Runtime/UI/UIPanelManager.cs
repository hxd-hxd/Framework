// -------------------------
// 创建日期：2022/10/27 10:12:48
// -------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework
{
    /// <summary>UGUI 界面管理器，用于管理派生自 <see cref="UIPanelBase"/> 的界面</summary>
    public partial class UIPanelManager : MonoSingleton<UIPanelManager>
    {
        /// <summary>主画布</summary>
        private static Canvas mainCanvas;
        private static bool _findMainCanvas = true;

        private static GameObject otherUIParent;

        /// <summary>其他 UI 的父节点（其他 UI 界面会加载到此节点下）</summary>
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

        /// <summary>主画布</summary>
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
                return mainCanvas;
            }
            set
            {
                mainCanvas = value;
            }
        }

        /// <summary>指定界面是否存在</summary>
        public static bool ExistPanel<T>()
        {
            UIPanelBase panel = GetPanel(typeof(T));
            return panel != null;
        }

        /// <summary>指定界面是否存在</summary>
        public static bool ExistPanel(Type type)
        {
            UIPanelBase panel = GetPanel(type);
            return panel != null;
        }

        /// <summary>注册界面</summary>
        public static void Register<T>(T panel) where T : UIPanelBase
        {
            UIManager.Instance.Register(panel);
        }

        /// <summary>注册界面</summary>
        [Obsolete("使用 UIPanelManager.Register<T>(T panel) 替代")]
        public static void Register(Type type, UIPanelBase panel)
        {
            UIManager.Instance.Register(panel);
        }

        /// <summary>获取界面
        /// <para>注意：预制体名称要与类名一致，才能加载成功</para>
        /// </summary>
        public static T GetPanel<T>() where T : UIPanelBase
        {
            UIPanelBase panel = GetPanel(typeof(T));

            if (panel) return (T)panel;

            return null;
        }
        /// <summary>获取界面</summary>
        /// <param name="assetName">用于加载，界面资产名</param>
        public static T GetPanel<T>(string assetName) where T : UIPanelBase
        {
            UIPanelBase panel = GetPanel(typeof(T), assetName);

            if (panel) return (T)panel;

            return null;
        }

        /// <summary>获取界面
        /// <para>注意：预制体名称要与类名一致，才能加载成功</para>
        /// </summary>
        public static UIPanelBase GetPanel(Type type)
        {
            return GetPanel(type, type.Name);
        }

        /// <summary>获取界面</summary>
        /// <param name="assetName">界面资产名，仅用于加载界面资源</param>
        public static UIPanelBase GetPanel(Type type, string assetName)
        {
            var panel = (UIPanelBase)UIManager.Instance.GetUI(type);

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
                    Register(panel);

                    GetCanvas(panel);
                }
            }

            return panel;
        }

        /// <summary>获取界面
        /// <para>注意：预制体名称要与类名一致，才能自动加载成功</para>
        /// </summary>
        public static async Task<T> GetPanelAsync<T>() where T : UIPanelBase
        {
            UIPanelBase panel = await GetPanelAsync(typeof(T));

            if (panel) return (T)panel;

            return null;
        }

        /// <summary>获取界面</summary>
        /// <param name="assetName">用于加载，界面资产名</param>
        public static async Task<T> GetPanelAsync<T>(string assetName) where T : UIPanelBase
        {
            UIPanelBase panel = await GetPanelAsync(typeof(T), assetName);

            if (panel) return (T)panel;

            return null;
        }

        /// <summary>获取界面
        /// <para>注意：预制体名称要与类名一致，才能自动加载成功</para>
        /// </summary>
        public static async Task<UIPanelBase> GetPanelAsync(Type type)
        {
            return await GetPanelAsync(type, type.Name);
        }

        /// <summary>获取界面</summary>
        /// <param name="assetName">界面资产名</param>
        public static async Task<UIPanelBase> GetPanelAsync(Type type, string assetName)
        {
            var panel = (UIPanelBase)UIManager.Instance.GetUI(type);

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
                    Register(panel);

                    GetCanvas(panel);
                }
            }

            return panel;
        }

        /// <summary>关闭所有界面</summary>
        [Obsolete("不再支持", true)]
        public static void CloseAllPanel()
        {
            
        }

        private static UIPanelBase InstantiatePanel(GameObject temlate)
        {
            Transform p = null;
            if (MainCanvas)
                p = MainCanvas.transform;// 将新界面设置到 UI 节点
            GameObject go = Object.Instantiate(temlate, p);

            var panel = go.GetComponent<UIPanelBase>();
            if (!panel) panel = go.GetComponentInChildren<UIPanelBase>(true);
            return panel;
        }

        /// <summary>获取画布</summary>
        public static Canvas GetCanvas<T>() where T : UIPanelBase
        {
            var panel = GetPanel<T>();
            return GetCanvas(panel);
        }

        /// <summary>获取画布</summary>
        public static Canvas GetCanvas(Type type)
        {
            var panel = GetPanel(type);
            return GetCanvas(panel);
        }

        /// <summary>获取画布</summary>
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

        /// <summary>销毁界面</summary>
        public static bool DestroyPanel<T>() where T : UIPanelBase
        {
            var panel = GetPanel<T>();
            return DestroyPanel(panel);
        }

        /// <summary>销毁界面</summary>
        public static bool DestroyPanel(Type type)
        {
            var panel = GetPanel(type);
            return DestroyPanel(panel);
        }

        /// <summary>销毁界面</summary>
        public static bool DestroyPanel(UIPanelBase panel)
        {
            if (panel)
            {
                RemovePanel(panel);
                Object.Destroy(panel.gameObject);
                return true;
            }
            return false;
        }

        /// <summary>移除界面</summary>
        public static bool RemovePanel<T>() where T : UIPanelBase
        {
            return UIManager.Instance.Unregister<T>();
        }

        /// <summary>移除界面</summary>
        public static bool RemovePanel(Type type)
        {
            return UIManager.Instance.Unregister(type);
        }

        /// <summary>移除界面</summary>
        public static bool RemovePanel(UIPanelBase panel)
        {
            return UIManager.Instance.Unregister(panel);
        }

        /// <summary>将指定界面调整到最上层</summary>
        public static void PanelTopmost(UIPanelBase panel)
            => UIManager.Instance.TopmostUI(panel);

    }


    public static class UIPanelExtend
    {
        /// <summary>获取画布</summary>
        public static Canvas GetCanvas(this UIPanelBase panel)
        {
            return UIPanelManager.GetCanvas(panel);
        }

        /// <summary>销毁界面</summary>
        public static bool DestroyPanel(this UIPanelBase panel)
        {
            return UIPanelManager.DestroyPanel(panel);
        }

        /// <summary>移除界面</summary>
        public static bool RemovePanel(this UIPanelBase panel)
        {
            return UIPanelManager.DestroyPanel(panel);
        }

        /// <summary>将指定界面调整到最上层</summary>
        public static void PanelTopmost(this UIPanelBase panel)
        {
            UIPanelManager.PanelTopmost(panel);
        }
    }
}