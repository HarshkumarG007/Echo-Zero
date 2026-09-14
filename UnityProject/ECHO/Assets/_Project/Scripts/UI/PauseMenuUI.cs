using UnityEngine;
using UnityEngine.UIElements;
using EchoZero.Core;
using EchoZero.Core.Events;

namespace EchoZero.UI
{
    /// <summary>
    /// UI Toolkit controller for the Pause Menu.
    /// TASK: TASK-013
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class PauseMenuUI : MonoBehaviour
    {
        private UIDocument _uiDocument;
        private VisualElement _pauseContainer;
        private Button _resumeButton;
        private Button _saveButton;

        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
            EventBus<GamePausedEvent>.Subscribe(OnGamePaused);
            InitializeUI();
        }

        private void OnDestroy()
        {
            EventBus<GamePausedEvent>.Unsubscribe(OnGamePaused);
            if (_resumeButton != null) _resumeButton.clicked -= OnResumeClicked;
            if (_saveButton != null) _saveButton.clicked -= OnSaveClicked;
        }

        private void InitializeUI()
        {
            if (_uiDocument == null || _uiDocument.rootVisualElement == null) return;

            var root = _uiDocument.rootVisualElement;

            _pauseContainer = root.Q<VisualElement>("pause-container");
            _resumeButton = root.Q<Button>("resume-button");
            _saveButton = root.Q<Button>("save-button");

            if (_pauseContainer != null)
            {
                _pauseContainer.AddToClassList("menu-closed");
                _pauseContainer.RemoveFromClassList("menu-open");
            }

            if (_resumeButton != null) _resumeButton.clicked += OnResumeClicked;
            if (_saveButton != null) _saveButton.clicked += OnSaveClicked;
        }

        private void OnGamePaused(GamePausedEvent evt)
        {
            if (_pauseContainer == null) return;
            
            if (evt.IsPaused)
            {
                _pauseContainer.RemoveFromClassList("menu-closed");
                _pauseContainer.AddToClassList("menu-open");
            }
            else
            {
                _pauseContainer.RemoveFromClassList("menu-open");
                _pauseContainer.AddToClassList("menu-closed");
            }
        }

        private void OnResumeClicked()
        {
            if (ServiceLocator.TryGet<GameStateManager>(out var gameState))
                gameState.SetPause(false);
        }

        private void OnSaveClicked()
        {
            if (ServiceLocator.TryGet<ISaveService>(out var saveService))
            {
                saveService.SaveGame();
                Debug.Log("[PauseMenuUI] Game saved from pause menu.");
            }
            else
            {
                Debug.LogWarning("[PauseMenuUI] SaveService not found.");
            }
        }
    }
}
