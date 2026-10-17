using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.UI
{
    /// <summary>注册 <see cref="UIBase"/></summary>
    [ExecuteInEditMode]
    public sealed class UIBaseRegister : MonoBehaviour
    {
        [SerializeField]
        private UIBase _ui;

        private void Awake()
        {
            if (!_ui) TryGetComponent(out _ui);
            if (_ui)
            {
                UIManager.Instance.Register(_ui);
                _ui.ExpectComponent<UIBaseUnregister>();
            }
        }

        void Start()
        {

        }

    }
}
