using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.UI
{
    /// <summary>UI 分组注册器</summary>
    [ExecuteInEditMode]
    public sealed class UIGroupRegister : MonoBehaviour
    {
        [Header("将分组注册到 UIManager")]
        [SerializeField]
        private UIGroup _group;

        private void Awake()
        {
            if (!_group) TryGetComponent(out _group);
            if (_group)
            {
                if (_group.isRootGroup)
                    UIManager.Instance.RegisterGroup(_group);
            }
        }

        private void Start()
        {

        }

        private void OnDestroy()
        {
            if (_group)
            {
                UIManager.Instance.UnregisterGroup(_group);
            }
        }
    }
}
