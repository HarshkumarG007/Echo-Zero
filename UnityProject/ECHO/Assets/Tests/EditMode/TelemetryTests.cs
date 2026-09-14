using NUnit.Framework;
using System.IO;
using UnityEngine;
using EchoZero.Core.Events;
using EchoZero.Core.Events.Narrative;
using EchoZero.Core.Telemetry;

namespace EchoZero.Tests.EditMode
{
    public class TelemetryTests
    {
        private GameObject _telemetryGO;
        private string _expectedPath;

        [SetUp]
        public void SetUp()
        {
            _expectedPath = Path.Combine(Application.persistentDataPath, "telemetry_session.json");
            if (File.Exists(_expectedPath))
            {
                File.Delete(_expectedPath);
            }

            _telemetryGO = new GameObject("Telemetry");
            var telemetry = _telemetryGO.AddComponent<LocalTelemetryService>();
            
            // Trigger Awake manually to subscribe to events
            var awakeMethod = typeof(LocalTelemetryService).GetMethod("Awake", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            awakeMethod?.Invoke(telemetry, null);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_telemetryGO);
            if (File.Exists(_expectedPath))
            {
                File.Delete(_expectedPath);
            }
        }

        [Test]
        public void ChoiceMadeEvent_WritesToJsonFile()
        {
            EventBus<ChoiceMadeEvent>.Publish(new ChoiceMadeEvent { ChosenFragmentId = "Frag_Test" });
            
            Assert.IsTrue(File.Exists(_expectedPath));
            string json = File.ReadAllText(_expectedPath);
            Assert.IsTrue(json.Contains("ChoiceMade"));
            Assert.IsTrue(json.Contains("Frag_Test"));
        }

        [Test]
        public void DriftStabilizedEvent_WritesToJsonFile()
        {
            EventBus<DriftStabilizedEvent>.Publish(new DriftStabilizedEvent { RecallAttemptCount = 3 });
            
            Assert.IsTrue(File.Exists(_expectedPath));
            string json = File.ReadAllText(_expectedPath);
            Assert.IsTrue(json.Contains("DriftStabilized"));
            Assert.IsTrue(json.Contains("Attempts: 3"));
        }

        [Test]
        public void TelemetryPayload_ContainsNoPII()
        {
            EventBus<ChoiceMadeEvent>.Publish(new ChoiceMadeEvent { ChosenFragmentId = "SafeData" });
            
            Assert.IsTrue(File.Exists(_expectedPath));
            string json = File.ReadAllText(_expectedPath);
            
            // Assert that common PII fields are NOT present
            Assert.IsFalse(json.Contains(SystemInfo.deviceUniqueIdentifier), "Payload should not contain device unique ID.");
            Assert.IsFalse(json.Contains(System.Environment.UserName), "Payload should not contain Environment.UserName.");
            
            // Assert session ID is an anonymous GUID
            var session = JsonUtility.FromJson<TelemetrySession>(json);
            Assert.IsTrue(System.Guid.TryParse(session.SessionId, out _), "Session ID should be an anonymous GUID.");
        }
    }
}
