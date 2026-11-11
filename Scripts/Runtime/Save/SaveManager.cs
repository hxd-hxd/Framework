using System;
using System.Collections;
using System.Collections.Generic;

namespace Framework.Save
{
    /// <summary>存档管理器</summary>
    public class SaveManager : Singleton<SaveManager>
    {
        // TOOD：待实现，存档备份

        private ISaveDataProvider _dataProvider;

        /// <summary>存档数据提供者</summary>
        public ISaveDataProvider dataProvider { get => _dataProvider; set => _dataProvider = value; }

        /// <summary>存档槽</summary>
        public int saveSlot { get => dataProvider.saveSlot; set => dataProvider.saveSlot = value; }

        /// <summary>获取自定义对象数据</summary>
        public T GetData<T>(string key)
        {
            if (dataProvider == null) return default;
            return dataProvider.GetData<T>(key);
        }

        /// <summary>获取数据</summary>
        object GetData(string key, Type type)
        {
            if (dataProvider == null) return default;
            return dataProvider.GetData(key, type);
        }

        /// <summary>尝试获取数据</summary>
        public bool TryGetData<T>(string key, out T value)
        {
            value = default;
            if (dataProvider == null) return default;
            return dataProvider.TryGetData(key, out value);
        }

        /// <summary>尝试获取数据</summary>
        public bool TryGetData(string key, Type type, out object value)
        {
            value = default;
            if (dataProvider == null) return default;
            return dataProvider.TryGetData(key, type, out value);
        }

        /// <summary>自定义对象数据</summary>
        public void SetData<T>(string key, T value)
        {
            if (dataProvider == null) return;
            if (value == null) return;
            dataProvider.SetData(key, value);
        }

        /// <summary>移除数据</summary>
        public void RemoveData(string key)
        {
            if (dataProvider == null) return;
            dataProvider.RemoveData(key);
        }

        /// <summary>移除存档</summary>
        public void RemoveSaveSlot(int saveSlot)
        {
            if (dataProvider == null) return;
            dataProvider.RemoveSaveSlot(saveSlot);
        }

        /// <summary>删除所有数据</summary>
        public void RemoveAll()
        {
            if (dataProvider == null) return;
            dataProvider.RemoveAll();
        }

        /// <summary>是否存在数据键</summary>
        public bool HasKey(string key)
        {
            if (dataProvider == null) return default;
            return dataProvider.HasKey(key);
        }

        /// <summary>保存</summary>
        public void Save()
        {
            if (dataProvider == null) return;
            EventSaveBefore.Send(this);
            dataProvider.Save();
            EventSaveAfter.Send(this);
        }

        /// <summary>获取所有已保存的存档槽</summary>
        public void GetAllSaveSlots(List<int> saveSlots)
        {
            if (dataProvider == null) return;
            dataProvider.GetAllSaveSlots(saveSlots);
        }
    }
}