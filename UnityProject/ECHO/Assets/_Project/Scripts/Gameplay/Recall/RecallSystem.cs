using UnityEngine;
using EchoZero.Core;
using EchoZero.Core.Events;
using EchoZero.Core.Events.Gameplay;

namespace EchoZero.Gameplay.Recall
{
    /// <summary>
    /// Pure C# system that processes RECALL attempts.
    /// Contains no Unity lifecycle — fully unit-testable in EditMode.
    ///
    /// Called by RecallAbility (MonoBehaviour) when the player triggers RECALL input.
    ///
    /// TASK: TASK-004
    /// </summary>
    public class RecallSystem
    {
        private static readonly Unity.Profiling.ProfilerMarker s_RecallMarker =
            new("RecallSystem.TryRecall");

        private readonly IConfigService _config;

        public RecallSystem(IConfigService config)
        {
            _config = config ?? throw new System.ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Attempts to apply RECALL to the given target.
        /// Publishes RecallAttemptedEvent before checking CanRecall,
        /// and RecallCompletedEvent after resolution.
        /// </summary>
        /// <returns>True if RECALL was applied; false otherwise.</returns>
        public bool TryRecall(IRecallable target)
        {
            using var _ = s_RecallMarker.Auto();

            EventBus<RecallAttemptedEvent>.Publish(new RecallAttemptedEvent { Target = target });

            if (target == null)
            {
                Debug.LogWarning("[RecallSystem][Warning] TryRecall called with null target.");
                EventBus<NothingFoundEvent>.Publish(new NothingFoundEvent());
                Publish(target, success: false);
                return false;
            }

            if (!target.CanRecall)
            {
                Debug.Log($"[RecallSystem][Debug] Target CanRecall=false: {target}");
                Publish(target, success: false);
                return false;
            }
            
            // Check WorldState for a fragment
            var worldState = ServiceLocator.Get<EchoZero.Core.WorldState.WorldState>();
            if (worldState != null && worldState.TryGetFragment(target.ObjectId, out string fragmentId))
            {
                EventBus<FragmentFoundEvent>.Publish(new FragmentFoundEvent { FragmentId = fragmentId });
                Debug.Log($"[RecallSystem][Debug] FragmentFoundEvent published: {fragmentId}");
            }
            else
            {
                EventBus<NothingFoundEvent>.Publish(new NothingFoundEvent());
            }

            target.OnRecall();
            Publish(target, success: true);
            return true;
        }

        // ------------------------------------------------------------------ //

        private static void Publish(IRecallable target, bool success)
        {
            EventBus<RecallCompletedEvent>.Publish(new RecallCompletedEvent
            {
                Target  = target,
                Success = success
            });
        }
    }
}
