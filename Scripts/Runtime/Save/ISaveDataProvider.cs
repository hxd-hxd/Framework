using System;
using System.Collections.Generic;

namespace Framework.Save
{
    /// <summary>存档数据提供者</summary>
    public interface ISaveDataProvider
    {
        /// <summary>存档槽</summary>
        int saveSlot { get; set; }

        /// <summary>获取数据</summary>
        T GetData<T>(string key);

        /// <summary>获取数据</summary>
        object GetData(string key, Type type);

        /// <summary>尝试获取数据</summary>
        bool TryGetData<T>(string key, out T value);

        /// <summary>尝试获取数据</summary>
        bool TryGetData(string key, Type type, out object value);

        /// <summary>设置数据</summary>
        void SetData<T>(string key, T value);

        /// <summary>移除数据</summary>
        void RemoveData(string key);

        /// <summary>移除存档</summary>
        void RemoveSaveSlot(int saveSlot);

        /// <summary>删除所有数据</summary>
        void RemoveAll();

        /// <summary>存在数据键</summary>
        bool HasKey(string key);

        /// <summary>保存</summary>
        void Save();

        /// <summary>获取所有已用的存档槽</summary>
        void GetAllSaveSlots(List<int> saveSlots);

        /// <summary>清理缓存</summary>
        void ClearCache();
    }
}
