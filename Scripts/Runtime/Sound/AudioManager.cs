// -------------------------
// 创建日期：2024/8/22 11:31:54
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Framework
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(AudioManager))]
    [CanEditMultipleObjects]
    class AudioManagerEditor : Editor
    {
        AudioManager my => (AudioManager)target;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            EditorGUI.BeginDisabledGroup(!Application.isPlaying);
            {
                EditorGUILayout.LabelField("声音设置", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                // 注意：以下设置的都是原始值，最终应用的是经过处理的属性值
                // 其中主音量采用分级制，其他音量采用自由制

                EditorGUILayout.BeginVertical("box");
                SoundGUI("主声音"
                    , ref my.soundSettings._mainVolume, ref my.soundSettings._mainSwitch
                    , () => my.mainVolume = my.soundSettings.mainVolume
                    , () => my.mainSwitch = my.soundSettings.mainSwitch
                    , 10);

                SoundGUI("背景声音"
                    , ref my.soundSettings._bgmVolume, ref my.soundSettings._bgmSwitch
                    , () => my.bgmVolume = my.soundSettings.bgmVolume
                    , () => my.bgmSwitch = my.soundSettings.bgmSwitch
                    );

                SoundGUI("其他声音（UI、3D等）"
                    , ref my.soundSettings._soundVolume, ref my.soundSettings._soundSwitch
                    , () => my.soundVolume = my.soundSettings.soundVolume
                    , () => my.soundSwitch = my.soundSettings.soundSwitch
                    );
                EditorGUILayout.EndVertical();

                EditorGUI.indentLevel--;

                EditorGUILayout.LabelField("声音信息", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                if (my._audioSourceBGM)
                {
                    EditorGUILayout.BeginVertical("box");
                    EditorGUILayout.LabelField("背景音乐", EditorStyles.boldLabel);

                    var audioS = my._audioSourceBGM;

                    EditorGUI.BeginChangeCheck();
                    var bgmC = audioS ? audioS.clip : null;
                    audioS.clip = (AudioClip)EditorGUILayout.ObjectField("音频剪辑", bgmC, typeof(AudioClip), false);
                    if (EditorGUI.EndChangeCheck())
                    {
                        audioS.Play();
                    }

                    float bgmCLenght = audioS.clip ? audioS.clip.length : 0;
                    audioS.time = EditorGUILayout.Slider(audioS.time, 0, bgmCLenght);
                    var s = new GUIStyle(EditorStyles.label);
                    s.wordWrap = true;
                    s.richText = true;
                    EditorGUILayout.LabelField($"剪辑时长：\t{bgmCLenght} s\r\n播放标准进度：\t{Mathf.FloorToInt(bgmCLenght != 0 ? audioS.time / bgmCLenght * 100 : 0)}%\r\n播放状态：\t{(audioS.isPlaying ? "<color=#54FF00>播放</color>" : "<color=#FF5454>暂停</color>")}", s);

                    EditorGUILayout.EndVertical();

                    //EditorGUILayout.LabelField("音效", EditorStyles.boldLabel);

                }

                EditorGUI.indentLevel--;
            }
            EditorGUI.EndDisabledGroup();
        }

        private void SoundGUI(string label, ref float volume, ref bool aSwitch, Action changeVolume, Action changeSwitch, int level = 0)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUI.BeginChangeCheck();
                // 设置原始值
                if (level > 0)
                    volume = EditorGUILayout.IntSlider(label, (int)(volume * level), 0, level) / (float)level;
                else
                    volume = EditorGUILayout.Slider(label, volume, 0, 1);

                if (EditorGUI.EndChangeCheck())
                {
                    // 应用处理的属性值
                    changeVolume();
                }

                EditorGUI.BeginChangeCheck();
                aSwitch = EditorGUILayout.Toggle(aSwitch, GUILayout.MaxWidth(40));
                if (EditorGUI.EndChangeCheck())
                {
                    changeSwitch();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
#endif

    /// <summary>
    /// 音频管理器
    /// </summary>
    public class AudioManager : MonoSingleton<AudioManager>
    {
        [SerializeField]
        internal SoundSettings _soundSettings = new SoundSettings();

        [Header("音源")]
        //public AudioMixer
        [SerializeField]
        internal AudioSource _audioSourceBGM;
        [SerializeField]
        [Tooltip("通用音源池")]
        internal AudioSourcePool _audioSourcePool = new AudioSourcePool();

        [Header("剪辑")]
        public AudioClip button;
        public List<AudioClip> _clips = new List<AudioClip>();
        public List<AudioClip> _bgmClips = new List<AudioClip>();

        public SoundSettings soundSettings { get => _soundSettings; }
        public AudioSource audioSourceBGM
        {
            get
            {
                if (!_audioSourceBGM)
                {
                    _audioSourceBGM = new GameObject("AudioSource - BGM").AddComponent<AudioSource>();
                    _audioSourceBGM.transform.SetParent(transform);
                }
                return _audioSourceBGM;
            }
        }

        public float mainVolume
        {
            get
            {
                float v = _soundSettings.mainVolume;
                return v;
            }
            set
            {
                _soundSettings.mainVolume = value;
                _audioSourcePool.SetVolume(_soundSettings.soundVolume);
                if (audioSourceBGM)
                    audioSourceBGM.volume = _soundSettings.bgmVolume;
            }
        }
        /// <summary>始终不超过主音量
        /// </summary>
        public float bgmVolume
        {
            get
            {
                float v = _soundSettings.bgmVolume;
                return v;
            }
            set
            {
                _soundSettings.bgmVolume = value;
                if (audioSourceBGM)
                    audioSourceBGM.volume = _soundSettings.bgmVolume;
            }
        }
        /// <summary>始终不超过主音量
        /// </summary>
        public float soundVolume
        {
            get
            {
                float v = _soundSettings.soundVolume;
                return v;
            }
            set
            {
                _soundSettings.soundVolume = value;
                _audioSourcePool.SetVolume(_soundSettings.soundVolume);
            }
        }

        public bool mainSwitch
        {
            get
            {
                return _soundSettings.mainSwitch;
            }
            set
            {
                _soundSettings.mainSwitch = value;
                if (audioSourceBGM)
                    if (_soundSettings.bgmSwitch)
                    {
                        audioSourceBGM.Play();
                    }
                    else
                    {
                        //audioSourceBGM.Stop();
                        audioSourceBGM.Pause();
                    }
                if (!_soundSettings.soundSwitch)
                {
                    _audioSourcePool.Stop();
                }
            }
        }
        /// <summary>
        /// 如果主开关关闭，则本开关始终关闭
        /// </summary>
        public bool bgmSwitch
        {
            get
            {
                return _soundSettings.bgmSwitch;
            }
            set
            {
                _soundSettings.bgmSwitch = value;
                if (audioSourceBGM)
                    if (_soundSettings.bgmSwitch)
                    {
                        audioSourceBGM.Play();
                    }
                    else
                    {
                        //audioSourceBGM.Stop();
                        audioSourceBGM.Pause();
                    }
            }
        }
        /// <summary>
        /// 如果主开关关闭，则本开关始终关闭
        /// </summary>
        public bool soundSwitch
        {
            get
            {
                return _soundSettings.soundSwitch;
            }
            set
            {
                _soundSettings.soundSwitch = value;
                if (!_soundSettings.soundSwitch)
                {
                    _audioSourcePool.Stop();
                }
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            if (Application.isPlaying)
            {
                mainVolume = _soundSettings.mainVolume;
                bgmVolume = _soundSettings.bgmVolume;
                soundVolume = _soundSettings.soundVolume;

                mainSwitch = _soundSettings.mainSwitch;
                bgmSwitch = _soundSettings.bgmSwitch;
                soundSwitch = _soundSettings.soundSwitch;

                if (!_audioSourcePool._parent)
                {
                    _audioSourcePool._parent = new GameObject("AudioSource - Common").GetComponent<Transform>();
                    _audioSourcePool._parent.SetParent(transform);
                }
            }

        }

        public AudioClip GetClipByName(string name)
        {
            if (!Instance) return null;
            var c = _clips.Find(x => x.name == name);
            return c;
        }

        /// <summary>
        /// 使用通用音源播放音效
        /// </summary>
        /// <param name="clip"></param>
        public void PlayAudioByName(string name)
        {
            if (!Instance) return;
            if (!soundSwitch) return;

            PlayAudio(GetClipByName(name));
        }

        /// <summary>
        /// 使用通用音源播放音效
        /// </summary>
        /// <param name="clip"></param>
        public void PlayAudioByName(GameObject source, string name)
        {
            if (!Instance) return;
            if (!soundSwitch) return;

            PlayAudio(source, GetClipByName(name));
        }

        /// <summary>
        /// 使用通用音源播放音效
        /// </summary>
        /// <param name="clip"></param>
        public void PlayAudio(AudioClip clip)
        {
            if (!Instance) return;
            if (!soundSwitch) return;

            var source = _audioSourcePool.Play(clip);
            source.volume = soundVolume;
        }

        /// <summary>
        /// 指定物体播放 3d 音效
        /// <para></para>2d、全局音效用 <see cref="PlayAudio(AudioClip)"/>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="clip"></param>
        public void PlayAudio(GameObject source, AudioClip clip)
        {
            if (!soundSwitch) return;

            AudioSource a = null;

            a = source.GetComponent<AudioSource>();
            if (!a)
            {
                a = source.AddComponent<AudioSource>();
                a.playOnAwake = false;
                // 设置 3D 音效
                a.maxDistance = 30;
                a.spatialBlend = 1;
                a.rolloffMode = AudioRolloffMode.Custom;
            }

            a.volume = soundVolume;
            Play(a, clip);
        }

        public void PlayAudio(Vector3 position, AudioClip clip)
        {
            if (!soundSwitch) return;

            AudioSource.PlayClipAtPoint(clip, position, soundVolume);
        }

        public void PlayBGM(int index)
        {
            if (!Instance) return;

            if (!_bgmClips.TryIndex(index, out var a))
            {
                _bgmClips.TryIndex(0, out a);
            }

            PlayBGM(a);
        }

        /// <summary>
        /// 播放 bgm
        /// </summary>
        /// <param name="clip"></param>
        public void PlayBGM(AudioClip clip)
        {
            if (!Instance) return;
            if (!bgmSwitch) return;

            audioSourceBGM.volume = bgmVolume;
            Play(audioSourceBGM, clip);
        }

        protected void Play(AudioSource source, AudioClip clip)
        {
            if (!mainSwitch) return;

            source.Stop();
            source.clip = clip;
            source.Play();
        }
    }

    [Serializable]
    public class SoundSettings
    {
        [Tooltip("主音量决定所有声音的上限")]
        [SerializeField]
        [Range(0f, 1f)]
        public float _mainVolume = 1;

        [Tooltip("背景音乐音量")]
        [SerializeField]
        [Range(0f, 1f)]
        public float _bgmVolume = 1;

        [Tooltip("其他效果音量（UI、3D场景等）")]
        [SerializeField]
        [Range(0f, 1f)]
        public float _soundVolume = 1;

        [Tooltip("主开关，决定所有声音是否能播放")]
        [SerializeField]
        public bool _mainSwitch = true;

        [Tooltip("背景音乐开关")]
        [SerializeField]
        public bool _bgmSwitch = true;

        [Tooltip("其他效果开关")]
        [SerializeField]
        public bool _soundSwitch = true;

        /// <summary>主音量</summary>
        public float mainVolume
        {
            get
            {
                float v = _mainVolume;
                return v;
            }
            set
            {
                _mainVolume = Mathf.Clamp01(value);
            }
        }

        /// <summary>背景音量，始终不超过主音量
        /// <para></para>注意：此属性经过处理，获取原始值 <see cref="_bgmVolume"/>
        /// </summary>
        public float bgmVolume
        {
            get
            {
                float v = _bgmVolume > _mainVolume ? _mainVolume : _bgmVolume;
                return v;
            }
            set
            {
                _bgmVolume = Mathf.Clamp01(value);
            }
        }

        /// <summary>声音音量，始终不超过主音量
        /// <para></para>注意：此属性经过处理，获取原始值 <see cref="_soundVolume"/>
        /// </summary>
        public float soundVolume
        {
            get
            {
                float v = _soundVolume > _mainVolume ? _mainVolume : _soundVolume;
                return v;
            }
            set
            {
                _soundVolume = Mathf.Clamp01(value);
            }
        }

        public bool mainSwitch
        {
            get
            {
                return _mainSwitch;
            }
            set
            {
                _mainSwitch = value;

            }
        }

        /// <summary>
        /// 如果主开关关闭，则本开关始终关闭
        /// <para></para>注意：此属性经过处理，获取原始值 <see cref="_bgmSwitch"/>
        /// </summary>
        public bool bgmSwitch
        {
            get
            {
                bool v = _mainSwitch ? _bgmSwitch : _mainSwitch;
                return v;
            }
            set
            {
                _bgmSwitch = value;
            }
        }
        /// <summary>
        /// 如果主开关关闭，则本开关始终关闭
        /// <para></para>注意：此属性经过处理，获取原始值 <see cref="_soundSwitch"/>
        /// </summary>
        public bool soundSwitch
        {
            get
            {
                bool v = _mainSwitch ? _soundSwitch : _mainSwitch;
                return v;
            }
            set
            {
                _soundSwitch = value;
            }
        }
    }

    [Serializable]
    public class AudioSourcePool
    {
        [Tooltip("父节点")]
        public Transform _parent;

        [Tooltip("模板，未指定则直接创建")]
        public AudioSource _template;

        [Range(0f, 1f)]
        [Tooltip("音量")]
        public float _volume = 1;

        public List<AudioSource> _pool = new List<AudioSource>();

        public void SetVolume(float v)
        {
            _volume = v;
            foreach (var item in _pool)
            {
                item.volume = _volume;
            }
        }

        public void Stop()
        {
            foreach (var item in _pool)
            {
                if (item)
                {
                    item.Stop();
                }
            }
        }
        public AudioSource Play(AudioClip clip)
        {
            var a = PoolFetch();
            a.clip = clip;
            a.Play();
            return a;
        }

        /// <summary>
        /// 取出
        /// </summary>
        /// <returns></returns>
        public AudioSource PoolFetch()
        {
            AudioSource r = default;

            for (int i = 0; i < _pool.Count; i++)
            {
                var a = _pool[i];
                if (a && !a.isPlaying)
                {
                    r = a;
                    break;
                }
            }
            if (r == default)
            {
                r = CreateAudioSource();
                r.name = $"Audio {_pool.Count}";
                _pool.Add(r);
            }

            return r;
        }

        protected virtual AudioSource CreateAudioSource()
        {
            AudioSource r = default;
            if (_template != null)
            {
                r = GameObject.Instantiate(_template.gameObject).GetComponent<AudioSource>();
                r.gameObject.SetActive(true);
                r.enabled = true;
            }
            else
            {
                r = new GameObject().AddComponent<AudioSource>();
            }
            r.transform.SetParent(_parent);

            return r;
        }
    }

    /// <summary>音频代理</summary>
    [Serializable]
    public class AudioAgent : AudioAgentBase
    {
        private AudioSource _audioSource;
        private AudioClip _clip;

        public override bool isWorking
        {
            get
            {
                return _audioSource != null && _audioSource.isPlaying;
            }
        }

        public AudioSource audioSource
        {
            get
            {
                return _audioSource;
            }
        }

        public void Init(AudioSource audioSource, AudioClip clip)
        {
            _audioSource = audioSource;
            _clip = clip;
        }

        public void Init(AudioSource audioSource)
        {
            _audioSource = audioSource;
        }

        public override void Init()
        {
            if (_audioSource)
                _audioSource.Stop();
        }

        public override void Start()
        {
            if (_audioSource)
            {
                if (_clip) _audioSource.clip = _clip;
                _audioSource.Play();
            }
        }

        public override void Update()
        {

        }

        public override void Recover()
        {
            if (_audioSource)
            {
                _audioSource.UnPause();
            }
        }

        public override void Pause()
        {
            if (_audioSource)
            {
                _audioSource.Pause();
            }
        }

        public override void Stop()
        {
            if (_audioSource)
            {
                _audioSource.Stop();
            }
        }

        public override void Clear()
        {
            _audioSource = null;
            _clip = null;
        }
    }

    /// <summary>音频代理基类</summary>
    public abstract class AudioAgentBase
    {
        /// <summary>工作中</summary>
        public virtual bool isWorking
        {
            get
            {
                return false;
            }
        }

        /// <summary>优先级</summary>
        public virtual int priority
        {
            get
            {
                return 0;
            }
        }

        /// <summary>初始化代理</summary>
        public abstract void Init();

        /// <summary>开始</summary>
        public abstract void Start();

        /// <summary>更新</summary>
        public abstract void Update();

        /// <summary>恢复</summary>
        public abstract void Recover();

        /// <summary>暂停</summary>
        public abstract void Pause();

        /// <summary>停止</summary>
        public abstract void Stop();

        /// <summary>清理</summary>
        public abstract void Clear();
    }
}