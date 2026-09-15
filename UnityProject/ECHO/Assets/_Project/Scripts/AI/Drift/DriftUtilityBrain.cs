using System.Collections.Generic;
using UnityEngine;
using EchoZero.Core.Events;
using EchoZero.Core.Events.Narrative;
using EchoZero.AI.Utility;

namespace EchoZero.AI.Drift
{
    /// <summary>
    /// Shared context passed into Drift utility scorers each tick.
    /// Avoids closures capturing outer fields, making scorers independently testable.
    /// TASK: TASK-019 (ARCH-001 fix)
    /// </summary>
    public struct DriftContext
    {
        public float DistanceToPlayer;
        public float RecallDuration;
        public Vector3 PlayerVelocity;
    }

    public class DriftAction : UtilityAction
    {
        public DriftState State { get; private set; }

        public DriftAction(DriftState state, string name) : base(name)
        {
            State = state;
        }
    }

    /// <summary>
    /// Utility-based brain for Drift behavior, replacing the FSM.
    /// Actions are scored via an explicit <see cref="DriftContext"/> object,
    /// not via closures capturing private fields.
    /// TASK: TASK-019 (ADR-010)
    /// </summary>
    public class DriftUtilityBrain
    {
        public DriftState CurrentState { get; private set; } = DriftState.Patrol;

        private readonly List<DriftAction> _actions = new();
        private DriftContext _ctx;
        private float _recallDuration;
        private const float RequiredRecallTime = 3f;
        private const float DetectionRadius = 15f;
        private const float DestabilizeRadius = 3f;

        public DriftUtilityBrain()
        {
            // Patrol — always has a baseline score so the Drift is never frozen
            var patrol = new DriftAction(DriftState.Patrol, "Patrol");
            patrol.AddScorer(new DelegateScorer(() => 0.1f));

            // Pursue — scores using simple distance heuristic (no ML)
            var pursue = new DriftAction(DriftState.Pursuing, "Pursue");
            pursue.AddScorer(new DelegateScorer(() => _ctx.DistanceToPlayer < DetectionRadius ? 1f : 0f));

            // Destabilize — scores hard when player is very close
            var destabilize = new DriftAction(DriftState.Destabilizing, "Destabilize");
            destabilize.AddScorer(new DelegateScorer(() =>
                _ctx.DistanceToPlayer <= DestabilizeRadius ? 2f : 0f));

            // Stabilize — veto score until recall threshold is met, then dominant
            var stabilize = new DriftAction(DriftState.Stabilized, "Stabilize");
            stabilize.AddScorer(new DelegateScorer(() =>
                _ctx.RecallDuration >= RequiredRecallTime ? 100f : 0f));

            _actions.Add(patrol);
            _actions.Add(pursue);
            _actions.Add(destabilize);
            _actions.Add(stabilize);
        }

        public void Update(float deltaTime, float distanceToPlayer, bool isBeingRecalled, Vector3 playerVelocity = default)
        {
            if (CurrentState == DriftState.Stabilized) return;

            _recallDuration = isBeingRecalled
                ? _recallDuration + deltaTime
                : Mathf.Max(0, _recallDuration - deltaTime * 2f);

            // Build context once per tick — no closure capture
            _ctx = new DriftContext
            {
                DistanceToPlayer = distanceToPlayer,
                RecallDuration   = _recallDuration,
                PlayerVelocity   = playerVelocity
            };

            // Evaluate all actions; pick highest scorer
            DriftAction bestAction = null;
            float highestScore = -1f;

            foreach (var action in _actions)
            {
                float score = action.ScoreAction();
                if (score > highestScore)
                {
                    highestScore = score;
                    bestAction = action;
                }
            }

            if (bestAction != null && bestAction.State != CurrentState)
            {
                TransitionTo(bestAction.State);
            }
        }

        private void TransitionTo(DriftState newState)
        {
            CurrentState = newState;

            if (newState == DriftState.Stabilized)
            {
                EventBus<DriftStabilizedEvent>.Publish(new DriftStabilizedEvent { RecallAttemptCount = 1 });
                Debug.Log("[DriftUtilityBrain] Drift Stabilized via Utility score.");
            }
        }
    }
}
