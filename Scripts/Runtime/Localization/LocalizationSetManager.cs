using System.Collections;
using System.Collections.Generic;

namespace Framework.Localization
{
    /// <summary>本地化设置管理器</summary>
    public class LocalizationSetManager : Singleton<LocalizationSetManager>
    {
        private List<ILocalizationSet> _sets;

        /// <summary>已注册的数量</summary>
        public int count => _sets.Count;

        /// <summary>已注册的本地化设置列表</summary>
        public List<ILocalizationSet> sets => _sets;

        public LocalizationSetManager()
        {
            _sets = new List<ILocalizationSet>();
        }

        /// <summary>注册</summary>
        public void RegisterSet(ILocalizationSet set)
        {
            if (set == null || _sets.Contains(set)) return;
            _sets.Add(set);
        }

        /// <summary>注销</summary>
        public void UnregisterSet(ILocalizationSet set)
        {
            _sets.Remove(set);
        }

        /// <summary>设置本地化</summary>
        public void Set()
        {
            foreach (var set in _sets)
            {
                if (set != null)
                {
                    set.Set();
                }
            }
        }
    }
}