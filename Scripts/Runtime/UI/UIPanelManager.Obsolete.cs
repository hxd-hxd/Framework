using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework
{
    partial class UIPanelManager
    {

        /// <summary>将指定界面调整到最上层</summary>
        [Obsolete("使用 UIManager.TopmostUI() 替代", true)]
        public static void PanelTopmost<T>() where T : UIPanelBase
        {
            PanelTopmost(typeof(T));
        }

        /// <summary>将指定界面调整到最上层</summary>
        [Obsolete("使用 UIManager.TopmostUI() 替代", true)]
        public static void PanelTopmost(Type type)
        {
            if (panelDic.TryGetValue(type, out UIPanelBase panel))
            {
                PanelTopmost(panel);
            }
        }

        /// <summary>将指定界面调整到最上层
        /// <para><paramref name="includeAllCanvas"/>：包括所有 canvas（将所有 canvas 纳入计算范围，效率不高，谨慎频繁使用）</para>
        /// </summary>
        [Obsolete("使用 UIManager.TopmostUI() 替代", true)]
        public static void PanelTopmost(UIPanelBase panel, bool includeAllCanvas)
        {
            if (!panel) return;

            if (panel.canvas)
            {
                int max = panel.canvas.sortingOrder;

                if (includeAllCanvas)// 是否将所有 canvas 纳入到排序范围
                {
                    Canvas[] canvasArr = Object.FindObjectsOfType<Canvas>();
                    for (int i = 0; i < canvasArr.Length; i++)
                    {
                        Canvas canvas = canvasArr[i];
                        if (canvas == panel.canvas
                            || !canvas.gameObject.activeSelf
                            ) continue;

                        int sortingOrder = canvas.sortingOrder;
                        if (sortingOrder >= max)
                            max = sortingOrder + 1;
                    }
                }
                else
                {
                    foreach (var item in panelDic)
                    {
                        if (item.Value == null
                            || item.Value == panel
                            || item.Value.canvas == null
                            || item.Value.canvas == panel.canvas
                            ) continue;

                        int sortingOrder = item.Value.canvas.sortingOrder;
                        if (sortingOrder >= max)
                            max = sortingOrder + 1;
                    }
                }

                panel.canvas.sortingOrder = max;
            }

            panel.OnTopmost();
        }

        /// <summary>检查是否阻挡射线的界面启用</summary>
        [Obsolete]
        public static bool HasResistRayPanelEnable()
        {
            bool h = ResistRayPanelEnableNum() > 0;
            return h;
        }
        /// <summary>获取是否有阻挡射线的界面启用的数量</summary>
        [Obsolete]
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

    }
}
