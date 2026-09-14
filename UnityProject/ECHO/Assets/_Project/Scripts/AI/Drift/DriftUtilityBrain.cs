using System.Collections.Generic;
using UnityEngine;
using EchoZero.Core.Events;
using EchoZero.Core.Events.Narrative;
using EchoZero.AI.Utility;
using System.Linq;

namespace EchoZero.AI.Drift
{
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
    /// TASK: TASK-019 (ADR-010)
    /// </summary>
    public class DriftUtilityBrain
    {
        public DriftState CurrentState { get; private set; } = DriftState.Patrol;

        private List<DriftAction> _actions = new();
        private float _recallDuration = 0f;
        private const float RequiredRecallTime = 3f;

        public DriftUtilityBrain()
        {
            // Initialize Utility Actions
            var patrol = new DriftAction(DriftState.Patrol, "Patrol");
            patrol.AddScorer(new UtilityScorerWrapper(() => 0.1f)); // Base score

            var alert = new DriftAction(DriftState.Alerted, "Alerted");
            alert.AddScorer(new UtilityScorerWrapper(() => 
            {
                // Unused in raw utility without memory, but we can simulate hysteresis via state.
                return 0f; 
            }));

            var pursue = new DriftAction(DriftState.Pursuing, "Pursue");
            pursue.AddScorer(new UtilityScorerWrapper(() => _lastDistanceToPlayer < 15f ? 1f : 0f));

            var destabilize = new DriftAction(DriftState.Destabilizing, "Destabilize");
            destabilize.AddScorer(new UtilityScorerWrapper(() => _lastDistanceToPlayer <= 3f ? 2f : 0f));

            var stabilize = new DriftAction(DriftState.Stabilized, "Stabilize");
            stabilize.AddScorer(new UtilityScorerWrapper(() => _recallDuration >= RequiredRecallTime ? 100f : 0f));

            _actions.Add(patrol);
            _actions.Add(alert);
            _actions.Add(pursue);
            _actions.Add(destabilize);
            _actions.Add(stabilize);
        }

        private float _lastDistanceToPlayer;

        public void Update(float deltaTime, float distanceToPlayer, bool isBeingRecalled)
        {
            if (CurrentState == DriftState.Stabilized) return;

            _lastDistanceToPlayer = distanceToPlayer;

            if (isBeingRecalled)
            {
                _recallDuration += deltaTime;
            }
            else
            {
                _recallDuration = Mathf.Max(0, _recallDuration - deltaTime * 2f);
            }

            // Evaluate Actions
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

    public class UtilityScorerWrapper : UtilityScorer
    {
        private System.Func<float> _evaluator;

        public UtilityScorerWrapper(System.Func<float> evaluator) : base(null)
        {
            _evaluator = evaluator;
        }

        public override float Score()
        {
            return _evaluator();
        }
    }
}
