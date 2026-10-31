using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Save
{
    /// <summary>存档监视器</summary>
    public class SaveMonitor : MonoBehaviour
    {
        [Header("自动保存时间，小于等于 0 不会自动保存")]
        [SerializeField]
        private float _autoSaveTime = -1;

        private float _autoSaveTimer;

        /// <summary>自动保存时间，小于等于 0 不会自动保存</summary>
        public float autoSaveTime { get => _autoSaveTime; set => _autoSaveTime = value; }

        void Start()
        {

        }

        void Update()
        {
            if (_autoSaveTime > 0)
            {
                _autoSaveTimer += Time.unscaledDeltaTime;
                if (_autoSaveTimer >= _autoSaveTime)
                {
                    //_autoSaveTimer -= _autoSaveTime;
                    _autoSaveTimer = 0;
                    AutoSave();
                }
            }
            //else if (_autoSaveTime == 0)
            //{
            //    AutoSave();
            //}
        }

        private void AutoSave()
        {
            SaveManager.Instance.Save();
            EventAutoSave.Send(this);
        }
    }
}
