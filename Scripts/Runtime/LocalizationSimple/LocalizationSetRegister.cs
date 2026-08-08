using System.Collections;
using System.Collections.Generic;
using Framework.Localization;
using UnityEngine;

namespace Framework.LocalizationSimple
{
    /// <summary>本地化设置注册器组件</summary>
    public class LocalizationSetRegister : MonoBehaviour
    {
        [SerializeField]
        private bool _isDisableUnregister = true;
        private ILocalizationSet _set;

        /// <summary>是否在禁用时取消注册，这样在禁用时就不会受到管理器控制</summary>
        public bool isDisableUnregister
        {
            get { return _isDisableUnregister; }
            set
            {
                _isDisableUnregister = value;
                if (value)
                {
                    // 如果已经被禁用，则取消注册
                    if (!gameObject.activeInHierarchy)
                    {
                        Unregister();
                    }
                }
                else
                {
                    // 如果已经启用，则添加注册
                    if (gameObject.activeInHierarchy)
                    {
                        Register();
                    }
                }
            }
        }

        void Start()
        {
            if (!_isDisableUnregister) Register();
        }

        private void OnEnable()
        {
            if (_isDisableUnregister) Register();
        }

        private void OnDisable()
        {
            if (_isDisableUnregister) Unregister();
        }

        private void OnDestroy()
        {
            // 禁用在销毁之前执行，如果禁用时没注销，则销毁一定要注销
            if (!_isDisableUnregister) Unregister();
        }

        public void Register()
        {
            if (ObjectUtility.IsNull(_set)) _set = GetComponent<ILocalizationSet>();
            LocalizationSetManager.Instance.RegisterSet(_set);
        }

        public void Unregister()
        {
            if (ObjectUtility.IsNull(_set)) _set = GetComponent<ILocalizationSet>();
            LocalizationSetManager.Instance.UnregisterSet(_set);
        }

    }
}
