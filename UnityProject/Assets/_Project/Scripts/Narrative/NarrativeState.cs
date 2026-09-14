using System.Collections.Generic;
using EchoZero.Core.Events;
using EchoZero.Core.Events.Narrative;

namespace EchoZero.Narrative
{
    /// <summary>
    /// Central store for all narrative flags and counters.
    /// Registered in ServiceLocator during Bootstrap.
    ///
    /// All mutations publish NarrativeFlagSetEvent via EventBus.
    /// State is serialized via ToSaveData() / LoadFrom() during save/load.
    ///
    /// TASK: TASK-006 (full implementation — stub available now for Bootstrap wiring)
    /// </summary>
    public class NarrativeState
    {
        private readonly Dictionary<string, int> _flags = new();

        /// <summary>
        /// Sets a flag to the given value (default 1).
        /// Publishes NarrativeFlagSetEvent after setting.
        /// </summary>
        public void SetFlag(string key, int value = 1)
        {
            if (string.IsNullOrEmpty(key)) return;
            _flags[key] = value;
            EventBus<NarrativeFlagSetEvent>.Publish(new NarrativeFlagSetEvent
            {
                FlagKey = key,
                Value   = value
            });
        }

        /// <summary>Returns the value of a flag, or 0 if not set.</summary>
        public int GetFlag(string key) =>
            string.IsNullOrEmpty(key) ? 0 :
            _flags.TryGetValue(key, out var v) ? v : 0;

        /// <summary>Returns true if the flag is set to any non-zero value.</summary>
        public bool HasFlag(string key) => GetFlag(key) != 0;

        /// <summary>Serialises current state for save system.</summary>
        public NarrativeStateData ToSaveData() =>
            new() { Flags = new Dictionary<string, int>(_flags) };

        /// <summary>Restores state from a loaded save.</summary>
        public void LoadFrom(NarrativeStateData data)
        {
            _flags.Clear();
            if (data?.Flags == null) return;
            foreach (var kv in data.Flags) _flags[kv.Key] = kv.Value;
        }
    }

    /// <summary>Serializable snapshot of NarrativeState flags.</summary>
    public class NarrativeStateData
    {
        public Dictionary<string, int> Flags { get; set; } = new();
    }

    /// <summary>
    /// Compile-time constants for all narrative flag keys.
    /// Use these constants — never raw strings.
    /// </summary>
    public static class NarrativeFlags
    {
        public const string Fragment01Collected = "frag_01_collected";
        public const string Fragment02Collected = "frag_02_collected";
        public const string Fragment03Collected = "frag_03_collected";
        public const string WalkwayRebuilt      = "walkway_rebuilt";
        public const string ChoiceMade          = "choice_made";
        public const string ChoiceFragmentA     = "choice_fragment_a";
        public const string DriftStabilized     = "drift_stabilized";
        public const string RevealTriggered     = "reveal_triggered";
    }
}
