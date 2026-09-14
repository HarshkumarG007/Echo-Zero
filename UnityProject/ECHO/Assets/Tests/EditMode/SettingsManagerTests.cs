using NUnit.Framework;
using UnityEngine;
using EchoZero.Core;
using EchoZero.Core.Settings;
using EchoZero.Core.Events;

namespace EchoZero.Tests.EditMode
{
    public class SettingsManagerTests
    {
        private SettingsManager _settingsManager;

        [SetUp]
        public void SetUp()
        {
            ServiceLocator.Clear();
            PlayerPrefs.DeleteKey("EchoZero_Settings");
            _settingsManager = new SettingsManager();
            ServiceLocator.Register<SettingsManager>(_settingsManager);
        }

        [TearDown]
        public void TearDown()
        {
            ServiceLocator.Clear();
            PlayerPrefs.DeleteKey("EchoZero_Settings");
            EventBus<SettingsChangedEvent>.Clear();
        }

        [Test]
        public void LoadSettings_WhenNoPrefs_CreatesDefault()
        {
            Assert.IsNotNull(_settingsManager.CurrentSettings);
            Assert.AreEqual(1f, _settingsManager.CurrentSettings.MasterVolume);
        }

        [Test]
        public void SaveSettings_PersistsToJson()
        {
            _settingsManager.SetVolume(0.5f, 0.8f, 0.9f);
            
            Assert.IsTrue(PlayerPrefs.HasKey("EchoZero_Settings"));
            string json = PlayerPrefs.GetString("EchoZero_Settings");
            
            Assert.IsTrue(json.Contains("\"MasterVolume\":0.5"));
        }

        [Test]
        public void LoadSettings_ParsesExistingJson()
        {
            string json = "{\"MasterVolume\":0.3,\"MusicVolume\":1.0,\"SFXVolume\":1.0,\"ResolutionIndex\":-1,\"Fullscreen\":true}";
            PlayerPrefs.SetString("EchoZero_Settings", json);
            
            _settingsManager.LoadSettings();
            
            Assert.AreEqual(0.3f, _settingsManager.CurrentSettings.MasterVolume);
        }

        [Test]
        public void SaveSettings_FiresChangedEvent()
        {
            bool eventFired = false;
            EventBus<SettingsChangedEvent>.Subscribe(e => eventFired = true);

            _settingsManager.SetVolume(0.5f, 0.8f, 0.9f);

            Assert.IsTrue(eventFired);
        }
    }
}
