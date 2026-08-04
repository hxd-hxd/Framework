using System;
using System.Collections;
using System.Collections.Generic;
using Framework;
using UnityEngine;

namespace Framework
{
    using SelectItem = SelectItemBase;

    /// <summary>选择器</summary>
    [Serializable]
    public class Selecter
    {
        [SerializeField]
        private bool _allowMultipleSelections;

        [SerializeField]
        private bool _allowCancelSelections;

        [SerializeField]
        private bool _allowLoopSelections;

        [SerializeField]
        private List<SelectItem> _items = new List<SelectItem>();

        [SerializeField]
        private List<SelectItem> _curItems = new List<SelectItem>();

        public SelectItem this[int index] => _items[index];

        /// <summary>选择项总数量</summary>
        public int Count => _items.Count;

        /// <summary>可选项总数量</summary>
        public int CanSelectCount
        {
            get
            {
                int count = 0;
                for (int i = 0; i < _items.Count; i++)
                {
                    var item = _items[i];
                    if (item && item.canSelect) count++;
                }
                return count;
            }
        }

        /// <summary>所有选择项列表</summary>
        public List<SelectItem> items
        {
            get => _items;
            set => _items = value;
        }

        /// <summary>当前选择项列表</summary>
        public List<SelectItem> curItems
        {
            get => _curItems;
            set => _curItems = value;
        }

        /// <summary>当前选择项，多选情况下永远指向最后选择那个</summary>
        public SelectItem curItem
        {
            get => _curItems.Count > 0 ? _curItems[_curItems.Count - 1] : null;
            set
            {
                SetCurItemInternal(value, false);
            }
        }

        /// <summary>当前项的下一个</summary>
        public SelectItem nextItem
        {
            get
            {
                SelectItem item = GetNextItem(curItem, item => item && item.canSelect, _allowLoopSelections);
                return item;
            }
        }

        /// <summary>当前项的前一个</summary>
        public SelectItem prevItem
        {
            get
            {
                SelectItem item = GetPrevItem(curItem, item => item && item.canSelect, _allowLoopSelections);
                return item;
            }
        }

        /// <summary>当前项是否最后一个</summary>
        public bool curItemIsLast
        {
            get
            {
                if (_items.Count <= 0) return false;
                return _items.IndexOf(curItem) == _items.Count - 1;
            }
        }

        /// <summary>当前项是否第一个</summary>
        public bool curItemIsFirst
        {
            get
            {
                if (_items.Count <= 0) return false;
                return _items.IndexOf(curItem) == 0;
            }
        }

        /// <summary>是否允许多选</summary>
        public bool allowMultipleSelections
        {
            get => _allowMultipleSelections;
            set
            {
                _allowMultipleSelections = value;

                // 如果不允许多选，其他多选项需额外操作
                if (!value)
                {
                    // 记录当前选项
                    var cur = curItem;
                    if (cur == null) return;

                    // 先移除当前项，方便后面批量操作其他选项，这当前项的规定是最后一个，也可以直接移除最后一个索引这样性能更好
                    //_curItems.Remove(cur);
                    _curItems.RemoveAt(_curItems.Count - 1);
                    // 再取消其他项的选中状态
                    for (int i = 0; i < _curItems.Count; i++)
                    {
                        var item = _curItems[i];
                        if (item)
                        {
                            item.SetIsSelectWithoutNotifyInternal(false);
                        }
                    }
                    _curItems.Clear();
                    // 再添加当前项
                    _curItems.Add(cur);
                }
            }
        }

        /// <summary>是否允许取消选中，只影响单选，因为多选本身就允许取消，也就是 <see cref="allowMultipleSelections"/> 为 false 时</summary>
        public bool allowCancelSelections
        {
            get => _allowCancelSelections;
            set
            {
                _allowCancelSelections = value;

                // 不允许取消的时候，自动选择第一个
                if (!value)
                {
                    if (curItem == null && _items.Count > 0)
                    {
                        SelectChange(0);
                    }
                }
            }
        }

        public bool allowLoopSelections
        {
            get => _allowLoopSelections;
            set
            {
                _allowLoopSelections = value;
            }
        }

        public void Init()
        {
            RegisterAllEvent();
        }

        public void Add(SelectItem item)
        {
            if (item && !_items.Contains(item))
            {
                _items.Add(item);
                // 需要更新其他选项的状态
                item.onSelectChangedInternal += OnSelectChange;
            }
        }

        public void Remove(SelectItem item)
        {
            if (item && _items.Contains(item))
            {
                _items.Remove(item);
                item.onSelectChangedInternal -= OnSelectChange;
            }
        }

        public void Clear()
        {
            UnRegisterAllEvent();
            _items.Clear();
            _curItems.Clear();
        }

        public void RegisterAllEvent()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                var item = _items[i];
                if (item)
                {
                    // 需要更新其他选项的状态
                    item.onSelectChangedInternal -= OnSelectChange;
                    item.onSelectChangedInternal += OnSelectChange;
                }
            }
        }

        public void UnRegisterAllEvent()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                var item = _items[i];
                if (item)
                {
                    // 需要更新其他选项的状态
                    item.onSelectChangedInternal -= OnSelectChange;
                }
            }
        }

        /// <summary>选择当前项的偏移项</summary>
        public void SelectAdd(int offset)
        {
            if (offset == 0) return;
            var items = _items;
            if (items.Count <= 0) return;

            int r = 0;
            var cur = curItem;

            if (cur)
            {
                bool isLoop = _allowLoopSelections;

                // 多选不允许循环选择
                if (_allowMultipleSelections) isLoop = false;

                if (offset > 0)
                {
                    // 选择下一个要从第一个选择的索引开始计算
                    // 因为选中列表中的顺序是不固定的，所以需要获取在所有列表中的索引
                    int min = int.MaxValue;
                    for (int i = 0; i < _curItems.Count; i++)
                    {
                        int tempI = items.IndexOf(_curItems[i]);
                        if (tempI < min) min = tempI;
                    }
                    if (_allowMultipleSelections)
                    {
                        min = GetNextItem(min, item => item && item.canSelect && !item.isSelect, isLoop);
                        if (min == -1) return;
                        min -= 1;
                    }
                    r = min;

                    for (int i = 0; i < offset; i++)
                    {
                        r = GetNextItem(r, item => item && item.canSelect, isLoop);
                    }

                    // 下一个是最后一个，不做操作，不然会取消，与正常偏移选择逻辑不符
                    if (_allowMultipleSelections && r == -1) return;
                }
                else if (offset < 0)
                {
                    // 选择下一个要从最后一个选择的索引开始计算
                    // 因为选中列表中的顺序是不固定的，所以需要获取在所有列表中的索引
                    int max = -1;
                    for (int i = 0; i < _curItems.Count; i++)
                    {
                        int tempI = items.IndexOf(_curItems[i]);
                        if (tempI > max) max = tempI;
                    }
                    if (_allowMultipleSelections)
                    {
                        max = GetPrevItem(max, item => item && item.canSelect && !item.isSelect, isLoop);
                        if (max == -1) return;
                        max += 1;
                    }
                    r = max;

                    for (int i = 0; i > offset; i--)
                    {
                        r = GetPrevItem(r, item => item && item.canSelect, isLoop);
                    }

                    // 下一个是第一个，不做操作，不然会取消，与正常偏移选择逻辑不符
                    if (_allowMultipleSelections && r == -1) return;
                }
            }

            //SelectChange(r);
            SelectChangeInternal(r, true);
        }

        /// <summary>切换指定索引选项的选中状态</summary>
        public void SelectChange(int i)
        {
            SelectChangeInternal(i, true);
        }

        /// <summary>获取指定项的上一个
        /// <para></para><paramref name="condition"/>：自定义条件
        /// </summary>
        public SelectItem GetPrevItem(SelectItem item, Func<SelectItem, bool> condition, bool isLoop)
        {
            if (_items.Count <= 0) return null;
            SelectItem resultItem = null;
            var cur = item;
            if (cur)
            {
                int r = _items.IndexOf(cur);
                var index = GetPrevItem(r, condition, isLoop);
                if (index >= 0) resultItem = _items[index];
            }
            return resultItem;
        }

        /// <summary>获取指定项的上一个
        /// <para></para><paramref name="condition"/>：自定义条件
        /// </summary>
        public int GetPrevItem(int index, Func<SelectItem, bool> condition, bool isLoop)
        {
            if (_items.Count <= 0) return -1;
            SelectItem resultItem = null;
            int resultIndex = -1;
            var cur = _items[index];
            if (cur)
            {
                int r = index;
                int min = 0;
                int max = _items.Count - 1;

                for (int i = 0; i < _items.Count; i++)
                {
                    r -= 1;
                    if (isLoop)
                    {
                        // 超过下限直接获取最后一个
                        if (r < min) r = max;
                    }
                    else
                    {
                        if (r < min) return -1;
                    }
                    resultIndex = r;
                    resultItem = _items[r];

                    if (resultItem)
                    {
                        if (condition?.Invoke(resultItem) ?? true)
                        {
                            break;
                        }
                    }
                }
            }
            return resultIndex;
        }

        /// <summary>获取指定项的下一个
        /// <para></para><paramref name="condition"/>：自定义条件
        /// </summary>
        public SelectItem GetNextItem(SelectItem item, Func<SelectItem, bool> condition, bool isLoop)
        {
            if (_items.Count <= 0) return null;
            SelectItem resultItem = null;
            var cur = item;
            if (cur)
            {
                int r = _items.IndexOf(cur);
                var index = GetNextItem(r, condition, isLoop);
                if (index >= 0) resultItem = _items[index];
            }
            return resultItem;
        }

        /// <summary>获取指定项的下一个
        /// <para></para><paramref name="condition"/>：自定义条件
        /// </summary>
        public int GetNextItem(int index, Func<SelectItem, bool> condition, bool isLoop)
        {
            if (_items.Count <= 0) return -1;
            SelectItem resultItem = null;
            int resultIndex = -1;
            var cur = _items[index];
            if (cur)
            {
                int r = index;
                int min = 0;
                int max = _items.Count - 1;

                for (int i = 0; i < _items.Count; i++)
                {
                    r += 1;
                    if (isLoop)
                    {
                        // 超过上限直接获取第一个
                        if (r > max) r = min;
                    }
                    else
                    {
                        if (r > max) return -1;
                    }
                    resultIndex = r;
                    resultItem = _items[r];

                    if (resultItem)
                    {
                        if (condition?.Invoke(resultItem) ?? true)
                        {
                            break;
                        }
                    }
                }
            }
            return resultIndex;
        }

        private void OnSelectChange(SelectItem item, bool isSelect)
        {
            if (item == null) return;
            int index = _items.IndexOf(item);
            if (index < 0) return;
            SelectChangeInternal(index, true);
        }

        private void SetCurItemInternal(SelectItem value, bool withoutNotify)
        {
            // 最后选择的索引
            int i = _curItems.Count - 1;

            // 旧的
            var oldItem = curItem;
            if (oldItem)
                SetIsSelectInternal(oldItem, false, withoutNotify);

            if (value != null)
            {
                SetIsSelectInternal(value, true, withoutNotify);
                if (i < 0)
                {
                    _curItems.Add(value);
                }
                else
                {
                    _curItems[i] = value;
                }
            }
            else
            {
                if (_curItems.Count > 0)
                    _curItems.RemoveAt(i);
            }
        }

        private void SelectChangeInternal(int i, bool withoutNotify)
        {
            var items = _items;
            if (items.Count <= 0 || i < 0 || i >= items.Count) return;

            var item = items[i];
            // 单选
            if (!_allowMultipleSelections)
            {
                do
                {
                    // 如果允许取消
                    if (_allowCancelSelections)
                    {
                        // 只有再次选中当前项才会取消选中
                        if (curItem == item)
                        {
                            SetIsSelectInternal(item, false, withoutNotify);
                            _curItems.Remove(item);
                            break;
                        }
                    }
                    SetCurItemInternal(item, withoutNotify);
                } while (false);
            }
            // 多选
            else
            {
                // 已选取消
                if (_curItems.Contains(item))
                {
                    SetIsSelectInternal(item, false, withoutNotify);
                    _curItems.Remove(item);
                }
                // 未选选中
                else
                {
                    SetIsSelectInternal(item, true, withoutNotify);
                    _curItems.Add(item);
                }
            }
        }

        private void SetIsSelectInternal(SelectItem item, bool value, bool withoutNotify)
        {
            if (item == null) return;
            if (withoutNotify)
            {
                item.SetIsSelectWithoutNotifyInternal(value);
            }
            else
            {
                item.isSelect = value;
            }
        }
    }
}
