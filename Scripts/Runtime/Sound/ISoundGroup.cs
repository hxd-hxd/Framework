using System.Collections;
using System.Collections.Generic;

namespace Framework.Sound
{
    /// <summary>声音组接口</summary>
    public interface ISoundGroup
    {
        /// <summary>声音组名称</summary>
        string name { get; }

        /// <summary>声音组音量</summary>
        float volume { get; set; }

        /// <summary>添加声音项</summary>
        void AddSound(ISoundItem soundItem);

        /// <summary>添加声音项</summary>
        void RemoveSound(ISoundItem soundItem);

        /// <summary>播放声音</summary>
        void Play(string name);

        /// <summary>停止声音</summary>
        void Stop(string name);

        /// <summary>停止声音</summary>
        void Stop();

        /// <summary>暂停声音</summary>
        void Pause(string name);

        /// <summary>暂停声音</summary>
        void Pause();
        
        /// <summary>恢复声音</summary>
        void Recover(string name);
        
        /// <summary>恢复声音</summary>
        void Recover();

        /// <summary>是否正在播放</summary>
        bool IsPlaying(string name);

        /// <summary>移除所有声音</summary>
        void Clear();

    }
}
