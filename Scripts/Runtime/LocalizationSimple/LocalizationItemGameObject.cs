using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.LocalizationSimple
{
    using LocalizationData = LocalizationDataGameObject;

    /// <summary>游戏对象本地化项，不实例化，只切换激活，逻辑功能导向的</summary>
    [Serializable]
    public class LocalizationItemGameObject : LocalizationItemBase<LocalizationData>
    {
        [Header("当前激活的游戏对象")]
        [SerializeField]
        private GameObject _curGO;
        [SerializeField]
        private List<LocalizationData> _datas = new List<LocalizationData>();

        public override List<LocalizationData> datas { get => _datas; set => _datas = value; }

        protected override void Execute(LocalizationData data)
        {
            var go = data?._gameObject;
            if (data == null || go == null) return;

            // 为了保证正确性，直接把其他的全部隐藏（兼容 Data / Provider 两种取数方式）
            if (TryGetAllDatas(out var allDatas) && allDatas != null)
            {
                foreach (var d in allDatas)
                {
                    if (d == null || d._gameObject == null || d._gameObject == go || !d._gameObject.activeSelf) continue;
                    d._gameObject.SetActive(false);
                }
            }

            _curGO = go;
            _curGO.SetActive(true);
        }
    }
}
