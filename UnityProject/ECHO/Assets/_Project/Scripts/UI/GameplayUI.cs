using UnityEngine;
using UnityEngine.UIElements;

namespace EchoZero.UI
{
    /// <summary>
    /// Manages the HUD and Dialogue UI using UI Toolkit.
    /// TASK: TASK-009
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class GameplayUI : MonoBehaviour
    {
        private UIDocument _uiDocument;
        private VisualElement _reticle;
        private VisualElement _dialogueContainer;
        private Label _dialogueText;

        public bool IsInitialized { get; private set; }

        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
            InitializeUI();
        }

        public void InitializeUI()
        {
            if (_uiDocument == null || _uiDocument.rootVisualElement == null) return;

            var root = _uiDocument.rootVisualElement;

            _reticle = root.Q<VisualElement>("reticle");
            _dialogueContainer = root.Q<VisualElement>("dialogue-container");
            _dialogueText = root.Q<Label>("dialogue-text");

            if (_dialogueContainer != null)
            {
                _dialogueContainer.style.display = DisplayStyle.None; // Hidden by default
            }

            IsInitialized = true;
        }

        /// <summary>
        /// Displays the dialogue box with the specified text.
        /// </summary>
        public void ShowDialogue(string text)
        {
            if (_dialogueContainer == null || _dialogueText == null)
            {
                Debug.LogWarning("[GameplayUI] Dialogue elements not found in UI Document.");
                return;
            }

            _dialogueText.text = text;
            _dialogueContainer.style.display = DisplayStyle.Flex;
        }

        /// <summary>
        /// Hides the dialogue box.
        /// </summary>
        public void HideDialogue()
        {
            if (_dialogueContainer != null)
            {
                _dialogueContainer.style.display = DisplayStyle.None;
            }
        }
    }
}
