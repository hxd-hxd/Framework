using System;
using System.Collections;
using System.Collections.Generic;
using Framework.UI;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework
{
    /// <summary>
    /// 用于管理 <see cref="IUI"/>
    /// </summary>
    public class UIManager : Singleton<UIManager>
    {
        internal Dictionary<Type, IUI> _uis = new Dictionary<Type, IUI>(20);
        internal Dictionary<string, IUIGroup> _groups = new Dictionary<string, IUIGroup>(4);

        /// <summary>指定界面是否存在实例，会尝试获取实例 <see cref="ExpectGetUI{T}"/></summary>
        public bool IsExistInstance<T>() where T : class, IUI
        {
            bool r = !ObjectUtility.IsNull(ExpectGetUI<T>());
            return r;
        }

        /// <summary>指定界面是否注册</summary>
        public bool IsRegister<T>() where T : class
        {
            Type type = typeof(T);
            if (_uis.TryGetValue(type, out var bui))
            {
                return ObjectUtility.IsNull(bui);
            }
            else
            {
                // 判断是否是通过基类或接口获取
                foreach (var item in _uis)
                {
                    if (item.Value is T)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>指定界面是否注册</summary>
        public bool IsRegister(Type type)
        {
            if (_uis.TryGetValue(type, out var bui))
            {
                return ObjectUtility.IsNull(bui);
            }
            else
            {
                // 判断是否是通过基类或接口获取
                foreach (var item in _uis)
                {
                    if (ObjectUtility.IsNull(item.Value)) continue;
                    if (type.IsAssignableFrom(item.Value.GetType()))
                        return true;
                }
            }
            return false;
        }

        /// <summary>指定界面是否注册</summary>
        public bool IsRegister<T>(T ui) where T : class
        {
            if (ObjectUtility.IsNull(ui)) return false;

            Type type = ui.GetType();
            if (_uis.TryGetValue(type, out var bui))
            {
                return Equals(bui, ui);
            }
            else
            {
                return false;
            }
        }

        /// <summary>注册</summary>
        public bool Register<T>(T ui) where T : class, IUI
        {
            if (ObjectUtility.IsNull(ui)) return false;

            Type type = ui.GetType();
            if (_uis.TryGetValue(type, out var bui))
            {
                if (Equals(bui, ui)) return false;
            }

            _uis[type] = ui;
            return true;
        }

        /// <summary>注销</summary>
        public bool Unregister<T>(T ui) where T : class, IUI
        {
            if (ObjectUtility.IsNull(ui)) return false;

            Type type = ui.GetType();
            if (_uis.TryGetValue(type, out var bui))
            {
                if (Equals(bui, ui))
                {
                    _uis.Remove(type);
                }
                else
                {
                    Debug.LogWarning($"不能注销 ui 实例 \"{ui}\"，和已注册的 \"{bui}\" 不是同一个 \"{typeof(T)}\" 实例，你持有的可能是已销毁的旧实例引用");
                    return false;
                }
            }

            return true;
        }

        /// <summary>注销</summary>
        public bool Unregister<T>() where T : class
        {
            Type type = typeof(T);
            if (!_uis.ContainsKey(type))
            {
                // 判断是否是通过基类或接口获取
                foreach (var item in _uis)
                {
                    if (ObjectUtility.IsNull(item.Value)) continue;
                    if (item.Value is T)
                    {
                        type = item.Value.GetType();
                        break;
                    }
                }
            }

            if (type != null)
                return _uis.Remove(type);
            return false;
        }

        /// <summary>注销</summary>
        public bool Unregister(Type type)
        {
            if (!_uis.ContainsKey(type))
            {
                // 判断是否是通过基类或接口获取
                foreach (var item in _uis)
                {
                    if (ObjectUtility.IsNull(item.Value)) continue;
                    if (type.IsAssignableFrom(item.Value.GetType()))
                    {
                        type = item.Value.GetType();
                        break;
                    }
                }
            }

            if (type != null)
                return _uis.Remove(type);
            return false;
        }

        /// <summary>获取实例</summary>
        public T GetUI<T>() where T : class
        {
            Type type = typeof(T);
            T ui = default;
            if (_uis.TryGetValue(type, out var bui))
            {
                if (!ObjectUtility.IsNull(bui))
                    ui = (T)bui;
            }
            else
            {
                // 判断是否是通过基类或接口获取
                foreach (var item in _uis)
                {
                    if (ObjectUtility.IsNull(item.Value)) continue;
                    if (item.Value is T t)
                    {
                        ui = t;
                        break;
                    }
                }
            }
            return ui;
        }

        /// <summary>获取实例</summary>
        public object GetUI(Type type)
        {
            object ui = default;
            if (_uis.TryGetValue(type, out var bui))
            {
                if (!ObjectUtility.IsNull(bui))
                {
                    ui = bui;
                }
            }
            else
            {
                // 判断是否是通过基类或接口获取
                foreach (var item in _uis)
                {
                    if (ObjectUtility.IsNull(item.Value)) continue;
                    if (type.IsAssignableFrom(item.Key))
                    {
                        ui = item.Value;
                        break;
                    }
                }
            }
            return ui;
        }

        /// <summary>期望获取实例
        /// <para>如果未从已注册列表中找到，则会尝试从已加载的 Unity 对象中找，找到后会将其注册到管理器</para>
        /// </summary>
        public T ExpectGetUI<T>() where T : class
        {
            Type type = typeof(T);
            T target = GetUI<T>();
            if (ObjectUtility.IsNull(target))
            {
#if UNITY_2020_1_OR_NEWER
                target = (T)(object)Object.FindObjectOfType(type, true);// 尝试从已加载的 Unity 对象中找
#else
                var uis = Resources.FindObjectsOfTypeAll(type);
                foreach (var item in uis)
                {
                    if (item is MonoBehaviour cui)
                    {
                        if(!string.IsNullOrEmpty(cui.gameObject.scene.path))// 排除预制体
                        {
                            target = item as T;
                            break;
                        }
                    }
                }
                //if(target == null) target = (uis.Length > 0 ? uis[0] : null) as T;
#endif
                if (target != null)
                {
                    Debug.Log($"要获取的 ui \"{type}\" 为空，找到已创建的实例并注册");
                    if (target is IUI ui)
                        Register(ui);
                }
                else
                {
                    Debug.LogWarning($"要获取的 ui \"{type}\" 为空，并且没有找到任何已创建的实例");
                }
            }
            return target;
        }

        /// <summary>尝试期望获取 <typeparamref name="T"/> 的实例
        /// <para>如果未从已注册列表中找到，则会尝试从已加载的 Unity 对象中找，找到后会将其注册到管理器</para>
        /// </summary>
        public bool TryExpectGetUI<T>(out T ui) where T : class, IUI
        {
            ui = ExpectGetUI<T>();
            bool r = !ObjectUtility.IsNull(ui);
            return r;
        }

        /// <summary>启用 ui</summary>
        public void EnableUI<T>(bool enable) where T : class, IUI
        {
            T ui = ExpectGetUI<T>();
            EnableUI(ui, enable);
        }

        /// <summary>启用 ui</summary>
        public void EnableUI(IUI ui, bool enable)
        {
            if (!ObjectUtility.IsNull(ui))
            {
                // TODO：启用，触发置顶、覆盖
                // TODO：禁用，触发恢复覆盖

                // 置顶
                // 全局置顶
                if (ui is IUIGroupItem groupItem && groupItem.group != null)
                {
                    // 目标启用
                    if (enable)
                    {
                        // 已启用
                        if (groupItem.isEnable)
                        {
                            // 全局搜索
                        }
                        // 已禁用
                        else
                        {

                        }
                    }
                    // 目标禁用
                    else
                    {

                    }
                }
                ui.OnEnabled(enable);

            }
        }

        /// <summary>UI 置顶</summary>
        public void TopmostUI<T>() where T : class
        {
            IUI ui = (IUI)GetUI<T>();
            TopmostUI(ui);
        }

        /// <summary>UI 置顶</summary>
        public void TopmostUI(Type type)
        {
            IUI ui = (IUI)GetUI(type);
            TopmostUI(ui);
        }

        /// <summary>UI 置顶，如果 ui 在组中，则只会在组内置顶</summary>
        public void TopmostUI(IUI ui)
        {
            /*
            置顶规则：
            全局：
                根 UI 在所有根 UI 和根组中调整排序。
            本地：
                只在组内置顶，不会影响组外 UI。
            */

            if (ObjectUtility.IsNull(ui)) return;

            // 这里获取当前的顶部 UI 有坑，
            // 如果在置顶之前启用了 ui，而 ui 再启用后正好是顶部 UI，那么覆盖逻辑就不对，
            // 应该在启用前就获取当前的顶部 UI，启用后先置顶
            var coveredUI = GetTopUI();
            if (ui == coveredUI) return;

            // 区分全局置顶和局部置顶
            if (true)
            {

            }
            int max = ui.sortOrder;
            foreach (var item in _uis)
            {
                if (ObjectUtility.IsNull(item.Value)
                    || item.Value == ui
                    ) continue;

                int sortingOrder = item.Value.sortOrder;
                if (sortingOrder >= max)
                {
                    max = sortingOrder + 1;
                }
            }
            ui.sortOrder = max;
            ui.OnTopmost();

            if (coveredUI != null) coveredUI.OnCovered();
        }

        /// <summary>注册分组</summary>
        public void RegisterGroup(IUIGroup group)
        {
            if (_groups.TryGetValue(group.groupName, out var oldGroup))
            {
                // TODO：处理同名旧分组，例如将旧分组里的项移到新分组
            }
            _groups[group.groupName] = group;
        }

        /// <summary>注销分组</summary>
        public void UnregisterGroup(IUIGroup group)
        {
            _groups.Remove(group.groupName);
        }

        /// <summary>获取分组</summary>
        public IUIGroup GetGroup(string name)
        {
            _groups.TryGetValue(name, out var group);
            return group;
        }

        /// <summary>尝试获取分组</summary>
        public void TryGetGroup(string name, out IUIGroup group)
        {
            _groups.TryGetValue(name, out group);
        }

        /// <summary>获取顶部 UI 分组</summary>
        public IUIGroup GetTopUIGroup()
        {
            IUIGroup topGroup = null;
            foreach (var item in _groups)
            {
                if (!item.Value.isEnable) continue;
                if (topGroup == null || topGroup.HasEnableItem() && topGroup.sortOrder <= item.Value.sortOrder)
                {
                    topGroup = item.Value;
                }
            }
            return topGroup;
        }

        /// <summary>获取顶部 <see cref="IUI"/></summary>
        public IUI GetTopUI()
        {
            // 这里的判断由于组的因素，变得更为复杂
            /* 
            判断排序高的组中有没有启用的 UI。
            考虑子组的因素。

            没有组的 UI 和组一起计算，有组的以组为基础，再判断内部项排序。
            */

            IUI topUI = default;
            // 先找出不在组中的顶部 UI
            IUI notGroupTopUI = default;
            foreach (var item in _uis)
            {
                var uil = item.Value;
                if (!uil.isEnable) continue;
                if (uil is IUIGroupItem groupItem && groupItem.group == null)
                {
                    if (notGroupTopUI == null || notGroupTopUI.sortOrder >= uil.sortOrder)
                    {
                        notGroupTopUI = uil;
                    }
                }
            }

            // 找出组中比非组 UI 排序小的顶部组 UI
            IUIGroup topGroup = default;
            foreach (var item in _groups)
            {
                var group = item.Value;
                if (!group.isEnable) continue;
                // 有非组 UI
                if (notGroupTopUI != null)
                {
                    // 没有比 UI 大的组就先找组
                    if (topGroup == null)
                    {
                        if (group.sortOrder > notGroupTopUI.sortOrder)
                        {
                            topGroup = group;
                        }
                    }
                    // 找到组后就在组之间找
                    else
                    {
                        if (group.sortOrder > topGroup.sortOrder)
                        {
                            topGroup = group;
                        }
                    }
                }
                else
                {
                    // 没有非组 UI 就在组之间找
                    if (topGroup == null || topGroup.HasEnableItem() && group.sortOrder > topGroup.sortOrder)
                    {
                        topGroup = group;
                    }
                }
            }

            if (topGroup != null)
            {
                topUI = topGroup.GetTopItem() as IUI;
            }
            else
            {
                topUI = notGroupTopUI;
            }

            return topUI;
        }
    }
}