using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.LocalizationSimple
{
    using LocalizationData = LocalizationDataGameObject;

    /// <summary>本地化项</summary>
    [Serializable]
    public class LocalizationItemGameObject : LocalizationItemBase<LocalizationData>
    {
        [Header("当前显示的游戏对象")]
        [SerializeField]
        private GameObject _curGO;
        [SerializeField]
        private List<LocalizationData> _datas = new();

        public override List<LocalizationData> datas { get => _datas; set => _datas = value; }

        protected override void Execute(LocalizationData data)
        {
            var go = data._gameObject;
            if (data != null && go != null)
            {
                //if (_curGO != null) _curGO.SetActive(false);
                // 说明第一次设置，要保证只显示不执行的
                //else
                {
                    // 为了保证正确性，直接把其他的全部隐藏
                    foreach (var d in datas)
                    {
                        if (d == null || d._gameObject == null || d._gameObject == go || !d._gameObject.activeSelf) continue;
                        d._gameObject.SetActive(false);
                    }
                }
                _curGO = data._gameObject;
                _curGO.SetActive(true);
            }
        }
    }
}
