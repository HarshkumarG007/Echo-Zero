namespace EchoZero.Core
{
    /// <summary>
    /// Interface for telemetry tracking. Defined in Core so Gameplay, Narrative, and App
    /// can record telemetry without depending directly on the Data assembly.
    ///
    /// TASK: TASK-003
    /// </summary>
    public interface ITelemetryService
    {
        void TrackSessionStart();
        void TrackFragmentCollected(string fragmentId);
        void TrackChoiceMade(string chosenFragmentId);
        void TrackRecallUsed(string targetType, bool success);
        void TrackDriftStabilized(int attemptCount);
        void TrackSceneLoaded(string sceneName, float durationMs);
    }
}
