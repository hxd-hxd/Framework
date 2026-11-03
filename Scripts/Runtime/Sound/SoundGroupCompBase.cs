using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Sound
{
    /// <summary>声音组组件基类</summary>
    public abstract class SoundGroupCompBase : MonoBehaviour, ISoundGroup
    {
        public abstract float volume { get; set; }

        public abstract void AddSound(ISoundItem soundItem);
        public abstract void Clear();
        public abstract bool IsPlaying(string name);
        public abstract void Pause(string name);
        public abstract void Pause();
        public abstract void Play(string name);
        public abstract void Recover(string name);
        public abstract void Recover();
        public abstract void RemoveSound(ISoundItem soundItem);
        public abstract void Stop(string name);
        public abstract void Stop();
    }
}
