using UnityEngine;
using EchoZero.Core.Events;

namespace EchoZero.Core.Settings
{
    public struct SettingsChangedEvent : IEvent { }

    [System.Serializable]
    public class SettingsConfig
    {
        public float MasterVolume = 1f;
        public float MusicVolume = 1f;
        public float SFXVolume = 1f;
        public int ResolutionIndex = -1;
        public bool Fullscreen = true;
    }

    /// <summary>
    /// Manages persistent game settings.
    /// TASK: TASK-016
    /// </summary>
    public class SettingsManager
    {
        private const string PrefsKey = "EchoZero_Settings";
        public SettingsConfig CurrentSettings { get; private set; }

        public SettingsManager()
        {
            LoadSettings();
        }

        public void LoadSettings()
        {
            if (PlayerPrefs.HasKey(PrefsKey))
            {
                string json = PlayerPrefs.GetString(PrefsKey);
                CurrentSettings = JsonUtility.FromJson<SettingsConfig>(json);
            }
            else
            {
                CurrentSettings = new SettingsConfig();
            }
            
            ApplySettings();
        }

        public void SaveSettings()
        {
            string json = JsonUtility.ToJson(CurrentSettings);
            PlayerPrefs.SetString(PrefsKey, json);
            PlayerPrefs.Save();
            
            ApplySettings();
        }

        public void SetVolume(float master, float music, float sfx)
        {
            CurrentSettings.MasterVolume = master;
            CurrentSettings.MusicVolume = music;
            CurrentSettings.SFXVolume = sfx;
            SaveSettings();
        }

        private void ApplySettings()
        {
            EventBus<SettingsChangedEvent>.Publish(new SettingsChangedEvent());
            
            // AudioManager handles the actual AudioMixer updates via its own event listener, 
            // or we could push directly if ServiceLocator resolves it.
            var audio = ServiceLocator.Get<EchoZero.Core.Audio.IAudioManager>();
            audio?.SetMasterVolume(CurrentSettings.MasterVolume);
            
            if (CurrentSettings.ResolutionIndex >= 0 && CurrentSettings.ResolutionIndex < Screen.resolutions.Length)
            {
                var res = Screen.resolutions[CurrentSettings.ResolutionIndex];
                Screen.SetResolution(res.width, res.height, CurrentSettings.Fullscreen);
            }
        }
    }
}
