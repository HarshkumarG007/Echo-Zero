using UnityEngine;
using EchoZero.Gameplay.Recall;

namespace EchoZero.AI.Drift
{
    /// <summary>
    /// MonoBehaviour for the Drift NPC. Integrates DriftUtilityBrain and IRecallable.
    /// TASK: TASK-010
    /// </summary>
    public class DriftController : MonoBehaviour, IRecallable
    {
        [SerializeField] private Transform _playerTransform;
        
        private DriftUtilityBrain _brain;
        private float _lastRecallTime = -1f;

        public bool CanRecall => _brain != null && _brain.CurrentState != DriftState.Stabilized;
        public string ObjectId => gameObject.GetHashCode().ToString();
        public DriftState CurrentState => _brain != null ? _brain.CurrentState : DriftState.Patrol;

        private void Awake()
        {
            _brain = new DriftUtilityBrain();
        }

        private void Update()
        {
            if (_playerTransform == null) return;

            float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);
            
            // Assume if OnRecall was called in the last 0.1 seconds, it's being actively recalled
            bool isBeingRecalled = (Time.time - _lastRecallTime) < 0.1f;

            _brain.Update(Time.deltaTime, distanceToPlayer, isBeingRecalled);
        }

        public void OnRecall()
        {
            _lastRecallTime = Time.time;
        }

        // For testing purposes
        public void SetPlayerTransform(Transform playerTransform)
        {
            _playerTransform = playerTransform;
        }

        public void ForceUpdate(float deltaTime, float distanceToPlayer, bool isBeingRecalled)
        {
            _brain.Update(deltaTime, distanceToPlayer, isBeingRecalled);
        }
    }
}
