using UnityEngine;
using EchoZero.Core;
using EchoZero.Core.Events;
using EchoZero.Core.Events.World;

namespace EchoZero.Data.Telemetry
{

    /// <summary>
    /// Local telemetry service. Phase 3: writes structured log to Unity Console only.
    /// Phase 5+: appends to telemetry.jsonl in Application.persistentDataPath.
    ///
    /// No PII. All events use session UUID, not any player identifier.
    ///
    /// TASK: TASK-003
    /// </summary>
    public class TelemetryService : ITelemetryService
    {
        private readonly string _sessionId;

        public TelemetryService()
        {
            _sessionId = System.Guid.NewGuid().ToString("N")[..8];
            // Subscribe to scene load events for automatic tracking
            EventBus<SceneLoadedEvent>.Subscribe(OnSceneLoaded);
        }

        // ------------------------------------------------------------------ //
        // Public tracking methods
        // ------------------------------------------------------------------ //

        public void TrackSessionStart()
            => Log("session_start", string.Empty);

        public void TrackFragmentCollected(string fragmentId)
            => Log("fragment_collected", $"\"fid\":\"{fragmentId}\"");

        public void TrackChoiceMade(string chosenFragmentId)
            => Log("choice_made", $"\"chosen\":\"{chosenFragmentId}\"");

        public void TrackRecallUsed(string targetType, bool success)
            => Log("recall_used", $"\"target\":\"{targetType}\",\"success\":{success.ToString().ToLower()}");

        public void TrackDriftStabilized(int attemptCount)
            => Log("drift_stabilized", $"\"attempts\":{attemptCount}");

        public void TrackSceneLoaded(string sceneName, float durationMs)
            => Log("scene_loaded", $"\"scene\":\"{sceneName}\",\"load_ms\":{durationMs:F0}");

        // ------------------------------------------------------------------ //
        // Private helpers
        // ------------------------------------------------------------------ //

        private void OnSceneLoaded(SceneLoadedEvent evt)
            => TrackSceneLoaded(evt.SceneName, evt.LoadDurationMs);

        private void Log(string eventType, string extraFields)
        {
            var ts   = System.DateTime.UtcNow.ToString("o");
            var core = $"{{\"t\":\"{ts}\",\"e\":\"{eventType}\",\"sid\":\"{_sessionId}\"";
            var body = string.IsNullOrEmpty(extraFields) ? core + "}" : $"{core},{extraFields}}}";
            Debug.Log($"[Telemetry] {body}");
            // TODO(TASK-006): append body to Application.persistentDataPath/telemetry.jsonl
        }
    }
}
