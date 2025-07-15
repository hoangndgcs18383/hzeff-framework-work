/*using DG.Tweening;
using SAGE.Framework.Core.Extensions;
using Sirenix.OdinInspector;

namespace SAGE.Framework.Core
{
    using System;
    using MEC;
    using UnityEngine;
    using System.Collections.Generic;

    [Serializable]
    public class Sound
    {
        public string soundKey;
        public AudioClip clip;
        public float volume = 1f;
        public float pitch = 1f;
        public bool loop;
        public bool ignoreTimeScale;
    }

    [Serializable]
    public struct SoundGroup
    {
        public string groupName;
        public List<Sound> clips;
    }

    public class AudioManager : BehaviorSingleton<AudioManager>
    {
        [Title("Pool Settings")] [SerializeField]
        private int _initialPoolSize = 10;

        [SerializeField] private bool _allowPoolExpansion = true;

        [Title("Sound Settings")] [TableList] [SerializeField]
        private List<SoundGroup> _soundGroups = new List<SoundGroup>();

        private Queue<AudioSource> _availableSources = new Queue<AudioSource>();
        private List<AudioSource> _allSources = new List<AudioSource>();
        private AudioSource _backgroundMusicSource;

        protected override void Awake()
        {
            base.Awake();
            InitializePool();
        }

        private void InitializePool()
        {
            for (int i = 0; i < _initialPoolSize; i++)
            {
                CreateNewAudioSource();
            }

            EnableBackgroundMusic(PlayerPrefs.GetInt(PrefKey.EnableMusic, 1) == 1);
        }

        private void InitializeBackgroundMusicSource()
        {
            CreateBackgroundMusicSource();
            _backgroundMusicSource.loop = true;
            _backgroundMusicSource.playOnAwake = false;
        }

        private AudioSource CreateNewAudioSource()
        {
            GameObject audioObject = new GameObject("PooledAudioSource");
            audioObject.transform.SetParent(transform);
            AudioSource source = audioObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            audioObject.SetActive(false);
            _availableSources.Enqueue(source);
            _allSources.Add(source);
            return source;
        }

        private AudioSource CreateBackgroundMusicSource()
        {
            GameObject audioObject = new GameObject("BackgroundMusicSource");
            audioObject.transform.SetParent(transform);
            AudioSource source = audioObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = true;
            _backgroundMusicSource = source;
            return source;
        }

        public AudioSource GetAvailableAudioSource()
        {
            if (_availableSources.Count > 0)
            {
                return _availableSources.Dequeue();
            }

            if (_allowPoolExpansion)
            {
                Debug.LogWarning("Audio pool expanded! Consider increasing initial size.");
                return CreateNewAudioSource();
            }

            Debug.LogError("No available AudioSources in the pool!");
            return null;
        }

        public void ReturnAudioSource(AudioSource source)
        {
            source.Stop();
            source.clip = null;
            source.gameObject.SetActive(false);
            _availableSources.Enqueue(source);
        }

        public AudioSource PlaySound(string soundKey, float volume = 1f, float pitch = 1f, bool loop = false,
            bool ignoreTimeScale = false)
        {
            Sound sound = GetSound(soundKey);
            if (sound == null) return null;

            return PlaySound(sound.clip, volume, pitch, loop, ignoreTimeScale);
        }

        public AudioSource PlayBackgroundMusic(string soundKey, float volume = 0.5f, float pitch = 1f)
        {
            if (_backgroundMusicSource == null)
            {
                InitializeBackgroundMusicSource();
            }

            Sound sound = GetSound(soundKey);
            if (sound == null) return null;

            _backgroundMusicSource.gameObject.SetActive(true);
            _backgroundMusicSource.clip = sound.clip;
            _backgroundMusicSource.pitch = pitch;
            _backgroundMusicSource.loop = true;
            _backgroundMusicSource.volume = 0;
            _backgroundMusicSource.Play();
            _backgroundMusicSource.DOFade(volume, 0.5f).SetEase(Ease.Linear);

            return _backgroundMusicSource;
        }

        private Sound GetSound(string soundKey)
        {
            foreach (SoundGroup group in _soundGroups)
            {
                Sound sound = group.clips.Find(s => s.soundKey == soundKey);
                if (sound != null) return sound;
            }

            Debug.LogError($"Sound with key {soundKey} not found!");
            return null;
        }

        private AudioSource PlaySound(AudioClip clip, float volume = 1f, float pitch = 1f, bool loop = false,
            bool ignoreTimeScale = false)
        {
            if (PlayerPrefs.GetInt(PrefKey.EnableSound, 1) == 0)
            {
                return null;
            }

            if (clip == null)
            {
                Debug.LogError("Audio clip is null!");
                return null;
            }

            AudioSource source = GetAvailableAudioSource();
            if (source == null)
            {
                Debug.LogError("No available AudioSources in the pool!");
                return null;
            }

            source.gameObject.SetActive(true);
            source.clip = clip;
            source.volume = volume;
            source.pitch = pitch;
            source.loop = loop;
            source.ignoreListenerPause = ignoreTimeScale;
            source.PlayOneShot(clip);

            if (!loop)
            {
                Timing.RunCoroutine(ReturnToPoolAfterPlayback(source),
                    ignoreTimeScale ? Segment.RealtimeUpdate : Segment.Update);
            }

            return source;
        }

        private IEnumerator<float> ReturnToPoolAfterPlayback(AudioSource source)
        {
            yield return Timing.WaitUntilTrue(() => !source.isPlaying);
            ReturnAudioSource(source);
        }

        public void EnableBackgroundMusic(bool enable)
        {
            if (_backgroundMusicSource == null)
            {
                InitializeBackgroundMusicSource();
            }
            
            _backgroundMusicSource.mute = !enable;
        }
    }
}*/