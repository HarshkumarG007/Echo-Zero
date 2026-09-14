using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace EchoZero.Core.Audio
{
    public interface IAudioManager
    {
        void PlaySFX(AudioClip clip, float volume = 1f);
        void PlayMusic(AudioClip clip, float fadeDuration = 1f);
        void PlayVoiceover(AudioClip clip);
        void SetMasterVolume(float volume);
    }

    /// <summary>
    /// Centralized Audio Manager using ServiceLocator.
    /// TASK: TASK-015
    /// </summary>
    public class AudioManager : MonoBehaviour, IAudioManager
    {
        [SerializeField] private AudioMixer _mainMixer;
        [SerializeField] private AudioMixerGroup _sfxGroup;
        [SerializeField] private AudioMixerGroup _musicGroup;
        [SerializeField] private AudioMixerGroup _voiceGroup;

        private AudioSource _sfxSource;
        private AudioSource _musicSource;
        private AudioSource _voiceSource;

        private void Awake()
        {
            _sfxSource = gameObject.AddComponent<AudioSource>();
            _sfxSource.outputAudioMixerGroup = _sfxGroup;
            _sfxSource.playOnAwake = false;

            _musicSource = gameObject.AddComponent<AudioSource>();
            _musicSource.outputAudioMixerGroup = _musicGroup;
            _musicSource.playOnAwake = false;
            _musicSource.loop = true;

            _voiceSource = gameObject.AddComponent<AudioSource>();
            _voiceSource.outputAudioMixerGroup = _voiceGroup;
            _voiceSource.playOnAwake = false;

            ServiceLocator.Register<IAudioManager>(this);
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<IAudioManager>();
        }

        public void PlaySFX(AudioClip clip, float volume = 1f)
        {
            if (clip == null) return;
            _sfxSource.PlayOneShot(clip, volume);
        }

        public void PlayMusic(AudioClip clip, float fadeDuration = 1f)
        {
            if (clip == null) return;
            if (_musicSource.clip == clip && _musicSource.isPlaying) return;

            StartCoroutine(CrossfadeMusic(clip, fadeDuration));
        }

        public void PlayVoiceover(AudioClip clip)
        {
            if (clip == null) return;
            _voiceSource.Stop();
            _voiceSource.clip = clip;
            _voiceSource.Play();
        }

        public void SetMasterVolume(float volume)
        {
            if (_mainMixer != null)
            {
                // Convert linear volume [0.0001, 1] to decibels [-80, 0]
                float db = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
                _mainMixer.SetFloat("MasterVolume", db);
            }
        }

        private IEnumerator CrossfadeMusic(AudioClip newClip, float duration)
        {
            if (_musicSource.isPlaying)
            {
                float startVolume = _musicSource.volume;
                for (float t = 0; t < duration; t += Time.deltaTime)
                {
                    _musicSource.volume = Mathf.Lerp(startVolume, 0f, t / duration);
                    yield return null;
                }
                _musicSource.Stop();
            }

            _musicSource.clip = newClip;
            _musicSource.Play();

            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                _musicSource.volume = Mathf.Lerp(0f, 1f, t / duration);
                yield return null;
            }
            _musicSource.volume = 1f;
        }
    }
}
