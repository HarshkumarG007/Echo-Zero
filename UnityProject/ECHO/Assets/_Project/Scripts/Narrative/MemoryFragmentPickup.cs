using UnityEngine;
using EchoZero.Core;
using EchoZero.Core.Events;
using EchoZero.Core.Events.Narrative;
using EchoZero.Gameplay.Recall;

namespace EchoZero.Narrative
{
    /// <summary>
    /// MonoBehaviour placed on world objects that hold a memory fragment.
    /// Implements IRecallable — collected when the player uses RECALL on it.
    ///
    /// After collection:
    ///   1. Sets the fragment's narrative flag in NarrativeState
    ///   2. Publishes FragmentCollectedEvent
    ///   3. Disables itself (cannot be recalled twice)
    ///
    /// TASK: TASK-005
    /// </summary>
    public class MemoryFragmentPickup : MonoBehaviour, IRecallable
    {
        [SerializeField] private MemoryFragmentSO _fragment;
        [SerializeField] private GameObject       _visual;

        private bool _collected;

        private void Start()
        {
            if (_fragment != null)
            {
                ServiceLocator.Get<EchoZero.Core.WorldState.WorldState>()?.RegisterFragment(ObjectId, _fragment.fragmentId);
            }
        }

        private void OnDestroy()
        {
            ServiceLocator.Get<EchoZero.Core.WorldState.WorldState>()?.UnregisterFragment(ObjectId);
        }

        // ------------------------------------------------------------------ //
        // IRecallable
        // ------------------------------------------------------------------ //

        /// <inheritdoc/>
        public string ObjectId => gameObject.GetHashCode().ToString();

        /// <inheritdoc/>
        public bool CanRecall => !_collected && _fragment != null;

        /// <inheritdoc/>
        public void OnRecall()
        {
            if (_collected) return;
            if (_fragment == null)
            {
                Debug.LogError("[MemoryFragmentPickup][Error] No MemoryFragmentSO assigned.", this);
                return;
            }

            _collected = true;

            // Set narrative flag
            var narrative = ServiceLocator.Get<NarrativeState>();
            if (!string.IsNullOrEmpty(_fragment.narrativeFlagOnCollect))
                narrative.SetFlag(_fragment.narrativeFlagOnCollect);

            // Publish collection event
            EventBus<FragmentCollectedEvent>.Publish(new FragmentCollectedEvent
            {
                FragmentId = _fragment.fragmentId
            });

            // Telemetry
            ServiceLocator.Get<ITelemetryService>().TrackFragmentCollected(_fragment.fragmentId);

            // Hide visual
            if (_visual != null) _visual.SetActive(false);

            Debug.Log($"[MemoryFragmentPickup][Info] Collected: {_fragment.fragmentId}");
        }

        // ------------------------------------------------------------------ //
        // Editor validation
        // ------------------------------------------------------------------ //

        private void OnValidate()
        {
            if (_fragment == null)
                Debug.LogWarning($"[MemoryFragmentPickup] No MemoryFragmentSO assigned on {name}.", this);
        }
    }
}
