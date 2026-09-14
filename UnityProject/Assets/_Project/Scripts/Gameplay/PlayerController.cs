using UnityEngine;
using EchoZero.Core;
using EchoZero.Gameplay.Recall;

namespace EchoZero.Gameplay
{
    /// <summary>
    /// Thin MonoBehaviour controller for the player character.
    /// Reads from Input System (via PlayerInput component or action references),
    /// delegates movement to PlayerMovement and RECALL to RecallAbility.
    ///
    /// Contains no business logic. Business logic lives in RecallSystem and PlayerMovement.
    ///
    /// TASK: TASK-004
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Transform _cameraRoot;
        [SerializeField] private float _mouseSensitivity = 2f;

        private PlayerMovement _movement;
        private RecallAbility  _recallAbility;

        private void Awake()
        {
            var config = ServiceLocator.Get<IConfigService>();
            var cc     = GetComponent<CharacterController>();

            _movement      = new PlayerMovement(cc, _cameraRoot, config);
            _recallAbility = GetComponent<RecallAbility>();

            if (_recallAbility == null)
                Debug.LogError("[PlayerController][Error] RecallAbility component missing.");
        }

        // Called by Unity Input System via the Player Input component (Send Messages mode disabled —
        // use Action-based callbacks wired in PlayerInputAdapter instead).
        // TODO(TASK-004): wire Input System action references and call these from PlayerInputAdapter.

        public void OnMove(UnityEngine.InputSystem.InputValue value)
            => _movement.SetMoveInput(value.Get<Vector2>());

        public void OnLook(UnityEngine.InputSystem.InputValue value)
            => _movement.SetLookInput(value.Get<Vector2>(), _mouseSensitivity);

        public void OnRecall()
            => _recallAbility?.TriggerRecall();

        private void Update()
            => _movement.Tick(Time.deltaTime);
    }
}
