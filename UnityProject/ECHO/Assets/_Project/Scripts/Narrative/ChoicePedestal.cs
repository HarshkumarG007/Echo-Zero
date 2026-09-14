using UnityEngine;
using EchoZero.Core;
using EchoZero.Core.Events;
using EchoZero.Core.Events.Narrative;
using EchoZero.Gameplay.Recall;
using EchoZero.Narrative.Fragments;

namespace EchoZero.Narrative
{
    /// <summary>
    /// Interactive world object representing a contradictory memory choice.
    /// TASK: TASK-011
    /// </summary>
    public class ChoicePedestal : MonoBehaviour, IRecallable
    {
        [SerializeField] private MemoryFragmentSO _assignedFragment;
        [SerializeField] private ChoicePedestal _alternatePedestal;
        
        private bool _isLocked = false;

        public bool CanRecall => !_isLocked && _assignedFragment != null;

        public string ObjectId => gameObject.GetInstanceID().ToString();

        public void OnRecall()
        {
            if (_isLocked || _assignedFragment == null) return;

            var registry = ServiceLocator.Get<FragmentRegistry>();
            if (registry == null)
            {
                Debug.LogWarning("[ChoicePedestal] FragmentRegistry not found.");
                return;
            }

            // Attempt to validate the fragment
            bool success = registry.ValidateFragment(_assignedFragment.fragmentId);

            if (success)
            {
                _isLocked = true;
                if (_alternatePedestal != null)
                {
                    _alternatePedestal.Lock();
                }

                // Fire the choice made event
                EventBus<ChoiceMadeEvent>.Publish(new ChoiceMadeEvent
                {
                    ChosenFragmentId = _assignedFragment.fragmentId
                });

                Debug.Log($"[ChoicePedestal] Choice made: {_assignedFragment.fragmentId}");
            }
            else
            {
                Debug.Log($"[ChoicePedestal] Failed to validate choice: {_assignedFragment.fragmentId}");
            }
        }

        public void Lock()
        {
            _isLocked = true;
        }
    }
}
