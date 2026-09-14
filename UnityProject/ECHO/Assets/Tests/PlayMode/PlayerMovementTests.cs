using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using EchoZero.Core;
using EchoZero.Gameplay;

namespace EchoZero.Tests.PlayMode
{
    public class PlayerMovementTests
    {
        private class TestConfigService : IConfigService
        {
            public float PlayerMoveSpeed => 5f;
            public float RecallRange => 4f;
            public float RecallCooldown => 1f;
            public float DriftStabiliseDuration => 3f;
            public float DriftDetectionRadius => 8f;
            public float DriftPursuitSpeed => 3f;
        }

        private GameObject _playerObject;
        private CharacterController _characterController;
        private Transform _cameraRoot;
        private PlayerMovement _playerMovement;

        [SetUp]
        public void SetUp()
        {
            _playerObject = new GameObject("Player");
            _playerObject.transform.position = Vector3.zero;
            
            _characterController = _playerObject.AddComponent<CharacterController>();
            
            var camObj = new GameObject("CameraRoot");
            camObj.transform.SetParent(_playerObject.transform);
            _cameraRoot = camObj.transform;
            
            var config = new TestConfigService();
            _playerMovement = new PlayerMovement(_characterController, _cameraRoot, config);
        }

        [TearDown]
        public void TearDown()
        {
            if (_playerObject != null)
            {
                Object.Destroy(_playerObject);
            }
        }

        [UnityTest]
        public IEnumerator PlayerMovement_WithForwardInput_MovesForward()
        {
            // Apply forward input
            _playerMovement.SetMoveInput(new Vector2(0, 1));
            
            // Wait for 10 frames
            for (int i = 0; i < 10; i++)
            {
                _playerMovement.Tick(Time.fixedDeltaTime);
                yield return new WaitForFixedUpdate();
            }
            
            // Expected to have moved forward in Z direction
            Assert.Greater(_playerObject.transform.position.z, 0.1f);
            Assert.AreEqual(0f, _playerObject.transform.position.x, 0.01f);
        }

        [UnityTest]
        public IEnumerator PlayerMovement_WithLookInput_RotatesPlayerAndCamera()
        {
            // Apply look input
            _playerMovement.SetLookInput(new Vector2(1, 1), 2f); // 2 degrees right, 2 degrees up

            // Tick once to apply rotation
            _playerMovement.Tick(Time.deltaTime);
            yield return null;

            // Yaw should apply to player (Y axis rotation)
            Assert.AreEqual(2f, _playerObject.transform.eulerAngles.y, 0.01f);
            
            // Pitch should apply to camera root (X axis rotation) -> pitch decreases, so negative rotation means looking up (in euler angles it can be 360-pitch)
            // -2 degrees = 358 in euler angles
            Assert.AreEqual(358f, _cameraRoot.eulerAngles.x, 0.01f);
        }

        [UnityTest]
        public IEnumerator PlayerMovement_AppliesGravity_WhenNotGrounded()
        {
            _playerObject.transform.position = new Vector3(0, 5, 0); // Start in air
            
            // Wait for 10 frames
            for (int i = 0; i < 10; i++)
            {
                _playerMovement.Tick(Time.fixedDeltaTime);
                yield return new WaitForFixedUpdate();
            }
            
            // Expected to have fallen down
            Assert.Less(_playerObject.transform.position.y, 5f);
        }
        [UnityTest]
        public IEnumerator PlayerMovement_CannotWalkThroughWalls()
        {
            // Spawn a wall in front of the player
            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.transform.position = new Vector3(0, 1, 2); // 2 units in front
            wall.transform.localScale = new Vector3(5, 5, 1);
            
            _playerObject.transform.position = new Vector3(0, 1, 0);

            // Wait a frame for physics to register the wall
            yield return new WaitForFixedUpdate();

            // Try to move forward into the wall
            _playerMovement.SetMoveInput(new Vector2(0, 1));
            
            // Wait for 10 frames of movement
            for (int i = 0; i < 10; i++)
            {
                _playerMovement.Tick(Time.fixedDeltaTime);
                yield return new WaitForFixedUpdate();
            }

            // Player should be blocked and not have reached z = 2
            Assert.Less(_playerObject.transform.position.z, 1.5f);
            
            Object.Destroy(wall);
        }
    }
}
