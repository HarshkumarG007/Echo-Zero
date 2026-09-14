namespace EchoZero.Core
{
    /// <summary>Interface for the configuration service. Allows test substitution.</summary>
    public interface IConfigService
    {
        /// <summary>Player movement speed in metres per second.</summary>
        float PlayerMoveSpeed { get; }

        /// <summary>Maximum RECALL raycast distance in metres.</summary>
        float RecallRange { get; }

        /// <summary>Seconds of sustained RECALL needed to stabilise the Drift.</summary>
        float DriftStabiliseDuration { get; }

        /// <summary>Drift detection sphere radius in metres.</summary>
        float DriftDetectionRadius { get; }

        /// <summary>Drift pursuit movement speed in metres per second.</summary>
        float DriftPursuitSpeed { get; }
    }

    /// <summary>
    /// Runtime configuration service.
    /// Phase 3: returns hardcoded defaults. Phase 5+: loads from JSON config asset.
    ///
    /// All values are tunable without code changes in Phase 5 (config SO or JSON file).
    ///
    /// TASK: TASK-003
    /// </summary>
    public class ConfigService : IConfigService
    {
        public float PlayerMoveSpeed       => 5f;
        public float RecallRange           => 4f;
        public float DriftStabiliseDuration => 3f;
        public float DriftDetectionRadius  => 8f;
        public float DriftPursuitSpeed     => 3f;
    }
}
