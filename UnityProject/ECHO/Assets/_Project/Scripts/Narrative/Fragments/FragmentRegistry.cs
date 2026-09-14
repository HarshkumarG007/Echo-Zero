using System;
using System.Collections.Generic;
using EchoZero.Core.Events;
using EchoZero.Core.Events.Narrative;

namespace EchoZero.Narrative.Fragments
{
    /// <summary>
    /// Tracks all registered fragment data models (MemoryFragmentSO) and manages contradiction logic.
    /// TASK: TASK-005
    /// </summary>
    public class FragmentRegistry : IDisposable
    {
        private readonly Dictionary<string, MemoryFragmentSO> _allFragments = new();
        private readonly HashSet<string> _collectedFragmentIds = new();
        private readonly List<string> _collectedOrder = new();
        
        // Key is the conflicting fragment ID, Value is the list of fragments in contradiction state
        private readonly Dictionary<string, string> _activeContradictions = new();

        public bool IsPlayerMustChooseState { get; private set; }
        public IReadOnlyList<string> CollectionHistory => _collectedOrder;

        public FragmentRegistry(IEnumerable<MemoryFragmentSO> fragments)
        {
            foreach (var frag in fragments)
            {
                if (frag != null && !string.IsNullOrEmpty(frag.fragmentId))
                {
                    _allFragments[frag.fragmentId] = frag;
                }
            }
            EventBus<FragmentCollectedEvent>.Subscribe(OnFragmentCollected);
        }

        public void Dispose()
        {
            EventBus<FragmentCollectedEvent>.Unsubscribe(OnFragmentCollected);
        }

        public void RegisterFragment(MemoryFragmentSO fragment)
        {
            if (fragment == null || string.IsNullOrEmpty(fragment.fragmentId)) return;
            _allFragments[fragment.fragmentId] = fragment;
        }

        private void OnFragmentCollected(FragmentCollectedEvent evt)
        {
            if (!_allFragments.TryGetValue(evt.FragmentId, out var fragment))
            {
                UnityEngine.Debug.LogWarning($"[FragmentRegistry] Collected unknown fragment: {evt.FragmentId}");
                return;
            }

            _collectedFragmentIds.Add(evt.FragmentId);
            _collectedOrder.Add(evt.FragmentId);

            // Check for contradiction
            if (!string.IsNullOrEmpty(fragment.contradictsFragmentId) &&
                _collectedFragmentIds.Contains(fragment.contradictsFragmentId))
            {
                IsPlayerMustChooseState = true;
                _activeContradictions[fragment.fragmentId] = fragment.contradictsFragmentId;
                _activeContradictions[fragment.contradictsFragmentId] = fragment.fragmentId;
                UnityEngine.Debug.Log($"[FragmentRegistry] Contradiction detected between {fragment.fragmentId} and {fragment.contradictsFragmentId}. Player must choose.");
            }
        }

        public bool ValidateFragment(string chosenFragmentId)
        {
            if (!IsPlayerMustChooseState) return false;
            if (!_activeContradictions.TryGetValue(chosenFragmentId, out string conflictingFragmentId))
            {
                return false; // The chosen fragment is not part of a contradiction
            }

            // Invalidate the conflicting fragment
            _collectedFragmentIds.Remove(conflictingFragmentId);
            _collectedOrder.Remove(conflictingFragmentId);
            
            // Resolve the contradiction
            _activeContradictions.Remove(chosenFragmentId);
            _activeContradictions.Remove(conflictingFragmentId);
            IsPlayerMustChooseState = _activeContradictions.Count > 0;

            EventBus<ChoiceMadeEvent>.Publish(new ChoiceMadeEvent { ChosenFragmentId = chosenFragmentId });
            
            UnityEngine.Debug.Log($"[FragmentRegistry] Choice made: {chosenFragmentId} validated. {conflictingFragmentId} invalidated.");
            
            return true;
        }

        public bool IsCollected(string fragmentId) => _collectedFragmentIds.Contains(fragmentId);
    }
}
