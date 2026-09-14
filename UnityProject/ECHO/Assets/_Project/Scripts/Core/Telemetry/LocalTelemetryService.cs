using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using EchoZero.Core.Events;
using EchoZero.Core.Events.Narrative;

namespace EchoZero.Core.Telemetry
{
    [Serializable]
    public class TelemetryPayload
    {
        public string EventName;
        public string Timestamp;
        public string Data;
    }

    [Serializable]
    public class TelemetrySession
    {
        public string SessionId;
        public List<TelemetryPayload> Events = new();
    }

    /// <summary>
    /// Logs AI and narrative decisions to a local JSON file.
    /// TASK: TASK-021
    /// </summary>
    public class LocalTelemetryService : MonoBehaviour
    {
        private TelemetrySession _session;
        private string _filePath;

        private void Awake()
        {
            _session = new TelemetrySession
            {
                SessionId = Guid.NewGuid().ToString()
            };
            
            _filePath = Path.Combine(Application.persistentDataPath, "telemetry_session.json");
            
            EventBus<DriftStabilizedEvent>.Subscribe(OnDriftStabilized);
            EventBus<ChoiceMadeEvent>.Subscribe(OnChoiceMade);
        }

        private void OnDestroy()
        {
            EventBus<DriftStabilizedEvent>.Unsubscribe(OnDriftStabilized);
            EventBus<ChoiceMadeEvent>.Unsubscribe(OnChoiceMade);
            Flush();
        }

        private void OnDriftStabilized(DriftStabilizedEvent evt)
        {
            LogEvent("DriftStabilized", $"Attempts: {evt.RecallAttemptCount}");
        }

        private void OnChoiceMade(ChoiceMadeEvent evt)
        {
            LogEvent("ChoiceMade", $"Fragment: {evt.ChosenFragmentId}");
        }

        public void LogEvent(string eventName, string data)
        {
            var payload = new TelemetryPayload
            {
                EventName = eventName,
                Timestamp = DateTime.UtcNow.ToString("o"),
                Data = data
            };
            
            _session.Events.Add(payload);
            Flush();
        }

        private void Flush()
        {
            try
            {
                string json = JsonUtility.ToJson(_session, true);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[Telemetry] Failed to write telemetry: {ex.Message}");
            }
        }
    }
}
