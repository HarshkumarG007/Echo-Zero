namespace EchoZero.Core.Events.Gameplay
{
    // -------------------------------------------------------------------------
    // RECALL events
    // -------------------------------------------------------------------------

    /// <summary>Published when the player initiates a RECALL attempt on a target.</summary>
    public struct RecallAttemptedEvent
    {
        /// <summary>The IRecallable object the player is targeting. May be null if raycast missed.</summary>
        public object Target;
    }

    /// <summary>Published after RECALL succeeds or fails on a target.</summary>
    public struct RecallCompletedEvent
    {
        /// <summary>The IRecallable object that was targeted.</summary>
        public object Target;

        /// <summary>True if RECALL was applied; false if target was null or CanRecall was false.</summary>
        public bool Success;
    }

    /// <summary>Published by the Drift when it interrupts the player's RECALL.</summary>
    public struct RecallInterruptedEvent
    {
        /// <summary>World-space push direction applied to the player.</summary>
        public UnityEngine.Vector3 PushDirection;
    }
}

namespace EchoZero.Core.Events.Narrative
{
    // -------------------------------------------------------------------------
    // Narrative events
    // -------------------------------------------------------------------------

    /// <summary>Published when a MemoryFragment is successfully collected via RECALL.</summary>
    public struct FragmentCollectedEvent
    {
        /// <summary>The unique ID of the collected fragment (matches MemoryFragment.fragmentId).</summary>
        public string FragmentId;
    }

    /// <summary>Published whenever a NarrativeState flag is set.</summary>
    public struct NarrativeFlagSetEvent
    {
        /// <summary>The flag key that was set (use constants from NarrativeFlags).</summary>
        public string FlagKey;

        /// <summary>The value the flag was set to.</summary>
        public int Value;
    }

    /// <summary>Published when the player makes the one permanent choice in the slice.</summary>
    public struct ChoiceMadeEvent
    {
        /// <summary>The fragment ID the player validated as "true".</summary>
        public string ChosenFragmentId;
    }

    /// <summary>Published when a reconstruction anchor completes its world-build sequence.</summary>
    public struct AnchorRebuiltEvent
    {
        /// <summary>The unique ID of the anchor that completed (matches ReconstructionAnchorSO.anchorId).</summary>
        public string AnchorId;
    }

    /// <summary>Published when the Drift is stabilised by the player via RECALL.</summary>
    public struct DriftStabilizedEvent
    {
        /// <summary>Number of RECALL attempts the player made before stabilising the Drift.</summary>
        public int RecallAttemptCount;
    }
}

namespace EchoZero.Core.Events.World
{
    // -------------------------------------------------------------------------
    // World / scene events
    // -------------------------------------------------------------------------

    /// <summary>Published by SceneLoader when an additive scene finishes loading.</summary>
    public struct SceneLoadedEvent
    {
        /// <summary>The name of the scene that was loaded.</summary>
        public string SceneName;

        /// <summary>Time taken to load the scene in milliseconds.</summary>
        public float LoadDurationMs;
    }

    /// <summary>Published by SceneLoader when an additive scene is unloaded.</summary>
    public struct SceneUnloadedEvent
    {
        /// <summary>The name of the scene that was unloaded.</summary>
        public string SceneName;
    }

    // -------------------------------------------------------------------------
    // Data / system events
    // -------------------------------------------------------------------------

    /// <summary>Published by SaveService after a save operation completes.</summary>
    public struct SaveCompletedEvent
    {
        /// <summary>True if the save succeeded; false if an error occurred.</summary>
        public bool Success;
    }

    /// <summary>Published by SaveService after a load operation completes.</summary>
    public struct LoadCompletedEvent
    {
        /// <summary>True if a valid save was found and loaded; false for new game or error.</summary>
        public bool Success;
    }
}
