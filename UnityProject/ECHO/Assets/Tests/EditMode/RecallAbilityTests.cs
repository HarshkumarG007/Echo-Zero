using NUnit.Framework;
using UnityEngine;
using EchoZero.Core;
using EchoZero.Gameplay;
using EchoZero.Core.Events;
using NSubstitute;

namespace EchoZero.Tests.EditMode
{
    public class RecallAbilityTests
    {
        private GameObject _playerObj;
        private RecallAbility _recallAbility;
        private Camera _camera;
        private IConfigService _configMock;
        private ITelemetryService _telemetryMock;

        [SetUp]
        public void SetUp()
        {
            ServiceLocator.Clear();

            _configMock = Substitute.For<IConfigService>();
            _configMock.RecallCooldown.Returns(1.0f); // 1 second cooldown
            _configMock.RecallRange.Returns(10f);
            ServiceLocator.Register<IConfigService>(_configMock);

            _telemetryMock = Substitute.For<ITelemetryService>();
            ServiceLocator.Register<ITelemetryService>(_telemetryMock);

            _playerObj = new GameObject("Player");
            
            var camObj = new GameObject("Camera");
            camObj.transform.SetParent(_playerObj.transform);
            _camera = camObj.AddComponent<Camera>();

            _recallAbility = _playerObj.AddComponent<RecallAbility>();
            
            // Use reflection to set the private _recallCamera field
            var field = typeof(RecallAbility).GetField("_recallCamera", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field.SetValue(_recallAbility, _camera);
            
            // Manually call Awake via reflection
            var awake = typeof(RecallAbility).GetMethod("Awake", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            awake.Invoke(_recallAbility, null);
        }

        [TearDown]
        public void TearDown()
        {
            ServiceLocator.Clear();
            Object.DestroyImmediate(_playerObj);
        }

        [Test]
        public void TriggerRecall_Spamming_RespectsCooldown()
        {
            // Reset time manually by tweaking _lastRecallTime if needed, but it starts at -999f
            float startTime = Time.time;
            
            // First call should succeed (it fires raycast, even if nothing is hit, it logs to telemetry)
            _recallAbility.TriggerRecall();
            
            // Second call immediately after should be blocked by cooldown
            _recallAbility.TriggerRecall();
            _recallAbility.TriggerRecall();

            // We can verify that TryRecall(null) was only called once, 
            // but we can't easily mock RecallSystem since it's instantiated in Awake.
            // However, RecallSystem.TryRecall(null) publishes OnNothingFound.
            // Let's just verify _configMock.RecallRange was only accessed once (on the successful raycast)
            // Actually, the cooldown check happens before accessing RecallRange.
            
            _configMock.Received(1).RecallRange;
        }
    }
}
