using NUnit.Framework;
using UnityEngine;
using EchoZero.Core;
using EchoZero.Core.Audio;

namespace EchoZero.Tests.EditMode
{
    public class AudioManagerTests
    {
        private GameObject _audioGO;
        private AudioManager _audioManager;

        [SetUp]
        public void SetUp()
        {
            ServiceLocator.Clear();
            _audioGO = new GameObject("AudioManager");
            _audioManager = _audioGO.AddComponent<AudioManager>();
            
            // Trigger Awake manually to register with ServiceLocator and create AudioSources
            var awakeMethod = typeof(AudioManager).GetMethod("Awake", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            awakeMethod?.Invoke(_audioManager, null);
        }

        [TearDown]
        public void TearDown()
        {
            ServiceLocator.Clear();
            Object.DestroyImmediate(_audioGO);
        }

        [Test]
        public void Awake_RegistersWithServiceLocator()
        {
            Assert.IsNotNull(ServiceLocator.Get<IAudioManager>());
            Assert.AreEqual(_audioManager, ServiceLocator.Get<IAudioManager>());
        }

        [Test]
        public void Awake_CreatesAudioSources()
        {
            var sources = _audioGO.GetComponents<AudioSource>();
            Assert.AreEqual(3, sources.Length); // SFX, Music, Voice
        }

        [Test]
        public void PlayVoiceover_SetsClipAndPlays()
        {
            var clip = AudioClip.Create("TestClip", 44100, 1, 44100, false);
            _audioManager.PlayVoiceover(clip);

            // In EditMode, we can't easily check if it's 'playing' (isPlaying often returns false in EditMode)
            // But we can check if the clip was assigned correctly to one of the AudioSources.
            var sources = _audioGO.GetComponents<AudioSource>();
            bool clipAssigned = false;
            foreach (var s in sources)
            {
                if (s.clip == clip) clipAssigned = true;
            }

            Assert.IsTrue(clipAssigned);
        }
    }
}
