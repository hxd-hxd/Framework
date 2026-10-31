using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Save
{
    /// <summary>存档数据提供者注册器</summary>
    public class SaveDataProviderRegister : MonoBehaviour
    {
        [SerializeField]
        private SaveDataProviderCompBase _dataProvider;

        public SaveDataProviderCompBase dataProvider { get => _dataProvider; set => _dataProvider = value; }

        private void Awake()
        {
            SaveManager.Instance.dataProvider = _dataProvider;
        }

        void Start()
        {

        }

        private void OnDestroy()
        {
            if (Equals(SaveManager.Instance.dataProvider, _dataProvider))
            {
                SaveManager.Instance.dataProvider = null;
            }
        }
    }
}
