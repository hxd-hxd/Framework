using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Save
{
    /// <summary>存档数据提供者组件基类</summary>
    public abstract class SaveDataProviderCompBase : MonoBehaviour, ISaveDataProvider
    {
        public abstract int saveSlot { get; set; }

        public abstract object GetData(string key, Type type);
        public abstract T GetData<T>(string key);
        public abstract bool TryGetData<T>(string key, out T value);
        public abstract bool TryGetData(string key, Type type, out object value);
        public abstract void SetData<T>(string key, T value);
        public abstract void RemoveData(string key);
        public abstract void RemoveSaveSlot(int saveSlot);
        public abstract void RemoveAll();
        public abstract bool HasKey(string key);
        public abstract void Save();
        public abstract void GetAllSaveSlots(List<int> saveSlots);
        public abstract void ClearCache();
    }
}
