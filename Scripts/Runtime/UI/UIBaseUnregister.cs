using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.UI
{
    /// <summary>注销 <see cref="UIBase"/></summary>
    public sealed class UIBaseUnregister : MonoBehaviour
    {
        private void OnDestroy()
        {
            if (TryGetComponent(out UIBase ui))
            {
                UIManager.Instance.Unregister(ui);
            }
        }
    }
}
