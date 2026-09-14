using UnityEngine;
using EchoZero.Core;
using EchoZero.Narrative.Mira;

namespace EchoZero.Narrative
{
    /// <summary>
    /// Handles the final narrative reveal sequence.
    /// TASK: TASK-014
    /// </summary>
    public class RevealSequence : MonoBehaviour
    {
        [Tooltip("The Mira instance to disable during the reveal.")]
        [SerializeField] private MiraDialogueSelector _miraInstance;

        [Tooltip("The HUD to hide during the reveal.")]
        [SerializeField] private GameObject _playerHUD;

        private bool _hasTriggered = false;

        private void OnTriggerEnter(Collider other)
        {
            if (_hasTriggered) return;

            if (other.CompareTag("Player") || other.GetComponent<EchoZero.Gameplay.PlayerController>() != null)
            {
                TriggerReveal();
            }
        }

        public void TriggerReveal()
        {
            if (_hasTriggered) return;
            _hasTriggered = true;

            var narrativeState = ServiceLocator.Get<NarrativeState>();
            if (narrativeState != null)
            {
                narrativeState.SetFlag(NarrativeFlags.RevealTriggered, true);
                Debug.Log("[RevealSequence] Reveal flag set in NarrativeState.");
            }

            if (_miraInstance != null)
            {
                _miraInstance.gameObject.SetActive(false);
                Debug.Log("[RevealSequence] Mira instance disabled.");
            }

            if (_playerHUD != null)
            {
                _playerHUD.SetActive(false);
                Debug.Log("[RevealSequence] Player HUD hidden.");
            }

            // In a real scenario, we might start a timeline or post-processing volume transition here.
            Debug.Log("[RevealSequence] Final reveal sequence rolling...");
        }
    }
}
