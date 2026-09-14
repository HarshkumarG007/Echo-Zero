using UnityEngine;
using EchoZero.Core.Events;
using EchoZero.Core.Events.Narrative;

namespace EchoZero.AI.Drift
{
    public enum DriftState
    {
        Patrol,
        Alerted,
        Pursuing,
        Destabilizing,
        Stabilized
    }

    /// <summary>
    /// Pure C# state machine for Drift behavior.
    /// TASK: TASK-010
    /// </summary>
    public class DriftStateMachine
    {
        public DriftState CurrentState { get; private set; } = DriftState.Patrol;

        private float _timeInState = 0f;
        private float _recallDuration = 0f;

        private const float DetectionRadius = 15f;
        private const float DestabilizeRadius = 3f;
        private const float RequiredRecallTime = 3f;

        public void Update(float deltaTime, float distanceToPlayer, bool isBeingRecalled)
        {
            if (CurrentState == DriftState.Stabilized) return;

            _timeInState += deltaTime;

            if (isBeingRecalled)
            {
                _recallDuration += deltaTime;
                if (_recallDuration >= RequiredRecallTime)
                {
                    Stabilize();
                    return;
                }
            }
            else
            {
                _recallDuration = Mathf.Max(0, _recallDuration - deltaTime * 2f);
            }

            switch (CurrentState)
            {
                case DriftState.Patrol:
                    if (distanceToPlayer <= DetectionRadius)
                    {
                        TransitionTo(DriftState.Alerted);
                    }
                    break;
                case DriftState.Alerted:
                    if (_timeInState >= 1f) // Brief pause
                    {
                        if (distanceToPlayer <= DetectionRadius)
                            TransitionTo(DriftState.Pursuing);
                        else
                            TransitionTo(DriftState.Patrol);
                    }
                    break;
                case DriftState.Pursuing:
                    if (distanceToPlayer <= DestabilizeRadius)
                        TransitionTo(DriftState.Destabilizing);
                    else if (distanceToPlayer > DetectionRadius * 1.5f) // Hysteresis
                        TransitionTo(DriftState.Patrol);
                    break;
                case DriftState.Destabilizing:
                    if (distanceToPlayer > DestabilizeRadius * 1.2f)
                        TransitionTo(DriftState.Pursuing);
                    break;
            }
        }

        private void TransitionTo(DriftState newState)
        {
            CurrentState = newState;
            _timeInState = 0f;
        }

        public void Stabilize()
        {
            if (CurrentState == DriftState.Stabilized) return;
            
            TransitionTo(DriftState.Stabilized);
            EventBus<DriftStabilizedEvent>.Publish(new DriftStabilizedEvent { RecallAttemptCount = 1 });
            Debug.Log("[DriftStateMachine] Drift Stabilized.");
        }
    }
}
