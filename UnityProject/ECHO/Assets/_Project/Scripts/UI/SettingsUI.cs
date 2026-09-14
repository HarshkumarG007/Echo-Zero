using UnityEngine;
using UnityEngine.UIElements;
using EchoZero.Core;
using EchoZero.Core.Settings;

namespace EchoZero.UI
{
    /// <summary>
    /// UI Toolkit controller for the Settings Menu.
    /// TASK: TASK-016
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class SettingsUI : MonoBehaviour
    {
        private UIDocument _uiDocument;
        private Slider _masterVolumeSlider;
        private Slider _musicVolumeSlider;
        private Slider _sfxVolumeSlider;
        private Button _closeButton;

        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
            InitializeUI();
        }

        private void OnDestroy()
        {
            if (_masterVolumeSlider != null) _masterVolumeSlider.UnregisterValueChangedCallback(OnVolumeChanged);
            if (_musicVolumeSlider != null) _musicVolumeSlider.UnregisterValueChangedCallback(OnVolumeChanged);
            if (_sfxVolumeSlider != null) _sfxVolumeSlider.UnregisterValueChangedCallback(OnVolumeChanged);
            if (_closeButton != null) _closeButton.clicked -= OnCloseClicked;
        }

        private void InitializeUI()
        {
            if (_uiDocument == null || _uiDocument.rootVisualElement == null) return;

            var root = _uiDocument.rootVisualElement;

            _masterVolumeSlider = root.Q<Slider>("master-volume-slider");
            _musicVolumeSlider = root.Q<Slider>("music-volume-slider");
            _sfxVolumeSlider = root.Q<Slider>("sfx-volume-slider");
            _closeButton = root.Q<Button>("settings-close-button");

            var settingsManager = ServiceLocator.Get<SettingsManager>();
            if (settingsManager != null)
            {
                if (_masterVolumeSlider != null) _masterVolumeSlider.value = settingsManager.CurrentSettings.MasterVolume;
                if (_musicVolumeSlider != null) _musicVolumeSlider.value = settingsManager.CurrentSettings.MusicVolume;
                if (_sfxVolumeSlider != null) _sfxVolumeSlider.value = settingsManager.CurrentSettings.SFXVolume;
            }

            if (_masterVolumeSlider != null) _masterVolumeSlider.RegisterValueChangedCallback(OnVolumeChanged);
            if (_musicVolumeSlider != null) _musicVolumeSlider.RegisterValueChangedCallback(OnVolumeChanged);
            if (_sfxVolumeSlider != null) _sfxVolumeSlider.RegisterValueChangedCallback(OnVolumeChanged);
            if (_closeButton != null) _closeButton.clicked += OnCloseClicked;
        }

        private void OnVolumeChanged(ChangeEvent<float> evt)
        {
            var settingsManager = ServiceLocator.Get<SettingsManager>();
            if (settingsManager != null)
            {
                float master = _masterVolumeSlider != null ? _masterVolumeSlider.value : 1f;
                float music = _musicVolumeSlider != null ? _musicVolumeSlider.value : 1f;
                float sfx = _sfxVolumeSlider != null ? _sfxVolumeSlider.value : 1f;
                
                settingsManager.SetVolume(master, music, sfx);
            }
        }

        private void OnCloseClicked()
        {
            // Usually we'd hide the settings container or pop the UI stack
            if (_uiDocument != null && _uiDocument.rootVisualElement != null)
            {
                var container = _uiDocument.rootVisualElement.Q<VisualElement>("settings-container");
                if (container != null) container.style.display = DisplayStyle.None;
            }
        }
    }
}
