using UnityEngine;
using EchoZero.Core;

namespace EchoZero.Narrative
{
    /// <summary>
    /// Handles the final narrative reveal sequence.
    /// TASK: TASK-014
    /// </summary>
    public class RevealSequence : MonoBehaviour
    {
        [Tooltip("The Mira host GameObject to disable during the reveal.")]
        [SerializeField] private GameObject _miraGameObject;

        [Tooltip("The HUD to hide during the reveal.")]
        [SerializeField] private GameObject _playerHUD;

        private bool _hasTriggered = false;

        private void OnTriggerEnter(Collider other)
        {
            if (_hasTriggered) return;

            if (other.CompareTag("Player"))
            {
                TriggerReveal();
            }
        }

        public void TriggerReveal()
        {
            if (_hasTriggered) return;
            _hasTriggered = true;

            if (ServiceLocator.TryGet<NarrativeState>(out var narrativeState))
            {
                narrativeState.SetFlag(NarrativeFlags.RevealTriggered, true);
                Debug.Log("[RevealSequence] Reveal flag set in NarrativeState.");
            }
            else
            {
                Debug.LogWarning("[RevealSequence] NarrativeState not registered in ServiceLocator.");
            }

            if (_miraGameObject != null)
            {
                _miraGameObject.SetActive(false);
                Debug.Log("[RevealSequence] Mira host GameObject disabled.");
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
