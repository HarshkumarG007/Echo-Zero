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
    /// Implements <see cref="ITelemetryService"/> so it can be resolved from ServiceLocator.
    /// TASK: TASK-021
    /// </summary>
    public class LocalTelemetryService : MonoBehaviour, ITelemetryService
    {
        private TelemetrySession _session;
        private string _filePath;
        private int _totalRecallsUsed = 0;
        private int _totalFragmentsCollected = 0;

        private void Awake()
        {
            _session = new TelemetrySession
            {
                SessionId = Guid.NewGuid().ToString()
            };

            _filePath = Path.Combine(Application.persistentDataPath, "telemetry_session.json");

            ServiceLocator.Register<ITelemetryService>(this);

            EventBus<DriftStabilizedEvent>.Subscribe(OnDriftStabilized);
            EventBus<ChoiceMadeEvent>.Subscribe(OnChoiceMade);
        }

        private void OnDestroy()
        {
            EventBus<DriftStabilizedEvent>.Unsubscribe(OnDriftStabilized);
            EventBus<ChoiceMadeEvent>.Unsubscribe(OnChoiceMade);
            ServiceLocator.Unregister<ITelemetryService>();
            Flush();
        }

        // ── ITelemetryService ──────────────────────────────────────────────

        public void TrackSessionStart()
            => LogEvent("SessionStart", string.Empty);

        public void TrackFragmentCollected(string fragmentId)
        {
            _totalFragmentsCollected++;
            LogEvent("FragmentCollected", $"Fragment: {fragmentId}");
        }

        public void TrackChoiceMade(string chosenFragmentId)
            => LogEvent("ChoiceMade", $"Fragment: {chosenFragmentId}");

        public void TrackRecallUsed(string targetType, bool success)
        {
            _totalRecallsUsed++;
            LogEvent("RecallUsed", $"Target: {targetType}, Success: {success}");
        }

        public void TrackDriftStabilized(int attemptCount)
            => LogEvent("DriftStabilized", $"Attempts: {attemptCount}");

        public void TrackSceneLoaded(string sceneName, float durationMs)
            => LogEvent("SceneLoaded", $"Scene: {sceneName}, Duration: {durationMs:F1}ms");

        public float GetAggressionScore()
        {
            if (_totalFragmentsCollected == 0) return 0.5f;
            float ratio = (float)_totalRecallsUsed / _totalFragmentsCollected;
            return Mathf.Clamp01(ratio / 2f);
        }

        // ── EventBus listeners ─────────────────────────────────────────────

        private void OnDriftStabilized(DriftStabilizedEvent evt)
            => TrackDriftStabilized(evt.RecallAttemptCount);

        private void OnChoiceMade(ChoiceMadeEvent evt)
            => TrackChoiceMade(evt.ChosenFragmentId);

        // ── Internal ───────────────────────────────────────────────────────

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
