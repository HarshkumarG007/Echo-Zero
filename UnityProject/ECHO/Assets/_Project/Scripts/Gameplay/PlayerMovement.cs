using UnityEngine;
using EchoZero.Core;

namespace EchoZero.Gameplay
{
    /// <summary>
    /// Pure C# class containing all player movement logic.
    /// No Unity lifecycle — unit-testable.
    ///
    /// TASK: TASK-004
    /// </summary>
    public class PlayerMovement
    {
        private readonly CharacterController _cc;
        private readonly Transform           _cameraRoot;
        private readonly IConfigService      _config;

        private Vector2 _moveInput;
        private float   _yaw;
        private float   _pitch;

        private const float GravityScale = -9.8f;
        private float _verticalVelocity;

        public PlayerMovement(CharacterController cc, Transform cameraRoot, IConfigService config)
        {
            _cc         = cc ?? throw new System.ArgumentNullException(nameof(cc));
            _cameraRoot = cameraRoot;
            _config     = config ?? throw new System.ArgumentNullException(nameof(config));
        }

        /// <summary>Called every frame from PlayerController.Update().</summary>
        public void Tick(float deltaTime)
        {
            ApplyGravity(deltaTime);
            ApplyMovement(deltaTime);
        }

        public void SetMoveInput(Vector2 input) => _moveInput = input;

        public void SetLookInput(Vector2 delta, float sensitivity)
        {
            _yaw   += delta.x * sensitivity;
            _pitch -= delta.y * sensitivity;
            _pitch  = Mathf.Clamp(_pitch, -80f, 80f);

            _cc.transform.localEulerAngles = new Vector3(0f, _yaw, 0f);
            if (_cameraRoot != null)
                _cameraRoot.localEulerAngles = new Vector3(_pitch, 0f, 0f);
        }

        // ------------------------------------------------------------------ //

        private void ApplyMovement(float dt)
        {
            var horizontal = _cc.transform.right   * _moveInput.x;
            var vertical   = _cc.transform.forward * _moveInput.y;
            var move       = (horizontal + vertical).normalized * (_config.PlayerMoveSpeed * dt);
            move.y = _verticalVelocity * dt;
            _cc.Move(move);
        }

        private void ApplyGravity(float dt)
        {
            if (_cc.isGrounded && _verticalVelocity < 0f)
                _verticalVelocity = -2f;
            else
                _verticalVelocity += GravityScale * dt;
        }
    }
}
