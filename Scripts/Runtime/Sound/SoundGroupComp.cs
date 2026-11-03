using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Sound
{
    /// <summary>声音组</summary>
    public class SoundGroupComp : SoundGroupCompBase
    {
        private float _volume = 1f;

        public override float volume
        {
            get => _volume;
            set => _volume = value;
        }

        public override void AddSound(ISoundItem soundItem)
        {
            throw new System.NotImplementedException();
        }

        public override void Clear()
        {
            throw new System.NotImplementedException();
        }

        public override bool IsPlaying(string name)
        {
            throw new System.NotImplementedException();
        }

        public override void Pause(string name)
        {
            throw new System.NotImplementedException();
        }

        public override void Pause()
        {
            throw new System.NotImplementedException();
        }

        public override void Play(string name)
        {
            throw new System.NotImplementedException();
        }

        public override void Recover(string name)
        {
            throw new System.NotImplementedException();
        }

        public override void Recover()
        {
            throw new System.NotImplementedException();
        }

        public override void RemoveSound(ISoundItem soundItem)
        {
            throw new System.NotImplementedException();
        }

        public override void Stop(string name)
        {
            throw new System.NotImplementedException();
        }

        public override void Stop()
        {
            throw new System.NotImplementedException();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
