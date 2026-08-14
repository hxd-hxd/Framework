using System.Collections;
using System.Collections.Generic;
using Framework.Localization;
using UnityEngine;

namespace Framework.LocalizationSimple
{
    /// <summary>本地化设置数据提供者</summary>
    public class LocalizationSetDataProvider : MonoBehaviour
    {
        [SerializeField]
        private LocalizationDataProviderCompBase _dataProvider;

        [SerializeField]
        private LocalizationSetBase _localizationSet;

        [SerializeField]
        private bool _awakeAutoSetToGlobal = true;

        [SerializeField]
        private bool _awakeAutoSetToItem = false;

        private void Awake()
        {
            if (!_localizationSet)
            {
                TryGetComponent(out _localizationSet);
            }

            if (Application.isPlaying)
            {
                if (_awakeAutoSetToGlobal && _dataProvider)
                    SetToGlobal();
                if (_awakeAutoSetToItem && _dataProvider)
                    SetToItem();
            }
        }

        void Start()
        {

        }

        /// <summary>设置数据提供者到全局</summary>
        public void SetToGlobal()
        {
            LocalizationSetManager.Instance.globalDataProvider = _dataProvider;
        }

        /// <summary>设置数据提供者到全局</summary>
        public void SetToGlobal(LocalizationDataProviderCompBase dataProvider)
        {
            _dataProvider = dataProvider;
            LocalizationSetManager.Instance.globalDataProvider = _dataProvider;
        }

        /// <summary>设置数据提供者到本地化项</summary>
        public void SetToItem()
        {
            if (_localizationSet == null) return;

            var items = TypePool.root.GetList<ILocalizationItem>();
            foreach (var localization in _localizationSet.localizations)
            {
                localization.GetAllItem(ref items);

                foreach (var item in items)
                {
                    item.dataProvider = _dataProvider;
                }

                items.Clear();
            }
            TypePool.root.Return(items);
        }
    }
}
