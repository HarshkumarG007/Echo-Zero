using UnityEngine;
using EchoZero.Core;
using EchoZero.Core.Events;
using EchoZero.Core.Events.Narrative;
using EchoZero.Gameplay.Recall;

namespace EchoZero.Gameplay
{
    /// <summary>
    /// MonoBehaviour that performs the RECALL raycast and delegates to RecallSystem.
    ///
    /// Attach alongside PlayerController on the player GameObject.
    /// Requires a camera child Transform assigned for raycast origin.
    ///
    /// TASK: TASK-004
    /// </summary>
    public class RecallAbility : MonoBehaviour
    {
        [SerializeField] private Camera _recallCamera;

        private RecallSystem _recallSystem;

        private void Awake()
        {
            var config = ServiceLocator.Get<IConfigService>();
            _recallSystem = new RecallSystem(config);
        }

        /// <summary>Called by PlayerController when the RECALL input fires.</summary>
        public void TriggerRecall()
        {
            if (_recallCamera == null)
            {
                Debug.LogError("[RecallAbility][Error] No camera assigned. Cannot raycast.");
                return;
            }

            var config = ServiceLocator.Get<IConfigService>();
            var ray    = _recallCamera.ScreenPointToRay(new Vector3(
                Screen.width  * 0.5f,
                Screen.height * 0.5f, 0f));

            if (Physics.Raycast(ray, out var hit, config.RecallRange))
            {
                if (hit.collider.TryGetComponent<IRecallable>(out var target))
                {
                    var telemetry = ServiceLocator.Get<ITelemetryService>();
                    bool success  = _recallSystem.TryRecall(target);
                    telemetry.TrackRecallUsed(target.GetType().Name, success);
                    return;
                }
            }

            // Nothing valid in range — still publish attempt for telemetry/UI feedback
            _recallSystem.TryRecall(null);
        }
    }
}
