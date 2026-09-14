# docs/technical-design.md — ECHO//ZERO

> Detailed technical specification for all systems in the vertical slice.
> Complements ARCHITECTURE.md (which covers structure) with implementation detail.

---

## 1. EventBus\<T\> — Full Specification

```csharp
namespace EchoZero.Core.Events
{
    /// <summary>
    /// Static typed publish-subscribe event bus.
    /// T must be a struct to guarantee allocation-free publishing.
    /// </summary>
    public static class EventBus<T> where T : struct
    {
        private static Action<T> _onEvent;

        public static void Subscribe(Action<T> handler)   => _onEvent += handler;
        public static void Unsubscribe(Action<T> handler) => _onEvent -= handler;
        public static void Publish(T evt)                 => _onEvent?.Invoke(evt);

        /// <summary>Called by tests in [TearDown] to prevent subscriber leakage.</summary>
        public static void Clear()                        => _onEvent = null;
    }
}
```

**All event structs** live in `EchoZero.Core.Events`. No exceptions.

Defined event structs for the vertical slice:

```csharp
// Gameplay events
public struct RecallAttemptedEvent    { public IRecallable Target; }
public struct RecallCompletedEvent    { public IRecallable Target; public bool Success; }

// Narrative events
public struct FragmentCollectedEvent  { public string FragmentId; }
public struct NarrativeFlagSetEvent   { public string FlagKey; public int Value; }
public struct ChoiceMadeEvent         { public string ChosenFragmentId; }
public struct AnchorRebuiltEvent      { public string AnchorId; }

// World events
public struct SceneLoadedEvent        { public string SceneName; }
public struct SceneUnloadedEvent      { public string SceneName; }

// System events
public struct SaveCompletedEvent      { public bool Success; }
public struct LoadCompletedEvent      { public bool Success; public GameSaveData Data; }
```

---

## 2. ServiceLocator — Full Specification

```csharp
namespace EchoZero.Core
{
    /// <summary>
    /// Simple explicit service registry. All services are registered
    /// in Bootstrap and retrieved by type. No reflection, no magic.
    /// </summary>
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new();

        public static void Register<T>(T service)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            _services[typeof(T)] = service;
        }

        public static T Get<T>()
        {
            if (_services.TryGetValue(typeof(T), out var svc))
                return (T)svc;
            throw new InvalidOperationException(
                $"[ServiceLocator] Service of type {typeof(T).Name} is not registered. " +
                $"Register it in Bootstrap before use.");
        }

        public static bool TryGet<T>(out T service)
        {
            if (_services.TryGetValue(typeof(T), out var svc)) { service = (T)svc; return true; }
            service = default; return false;
        }

        /// <summary>Used by tests in [TearDown] only.</summary>
        public static void Clear() => _services.Clear();
    }
}
```

---

## 3. IRecallable Interface

```csharp
namespace EchoZero.Gameplay.Recall
{
    /// <summary>
    /// Implemented by any world object that can be targeted by RECALL.
    /// Three implementing types in the vertical slice:
    /// MemoryFragmentPickup, ReconstructionAnchor, DriftController.
    /// </summary>
    public interface IRecallable
    {
        /// <summary>Whether RECALL can currently be applied to this object.</summary>
        bool CanRecall { get; }

        /// <summary>
        /// Called by RecallSystem when RECALL is successfully applied.
        /// Implementors publish their own domain events via EventBus.
        /// </summary>
        void OnRecall();
    }
}
```

---

## 4. RecallSystem — Full Specification

```csharp
namespace EchoZero.Gameplay.Recall
{
    public class RecallSystem
    {
        private readonly RecallConfig _config;

        public RecallSystem(RecallConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Attempts to apply RECALL to the given target.
        /// Returns true if successful; false if target is null or CanRecall is false.
        /// </summary>
        public bool TryRecall(IRecallable target)
        {
            if (target == null)
            {
                Debug.LogWarning("[RecallSystem] TryRecall called with null target.");
                return false;
            }

            EventBus<RecallAttemptedEvent>.Publish(new RecallAttemptedEvent { Target = target });

            if (!target.CanRecall)
            {
                EventBus<RecallCompletedEvent>.Publish(
                    new RecallCompletedEvent { Target = target, Success = false });
                return false;
            }

            target.OnRecall();
            EventBus<RecallCompletedEvent>.Publish(
                new RecallCompletedEvent { Target = target, Success = true });
            return true;
        }
    }
}
```

---

## 5. NarrativeState Service

```csharp
namespace EchoZero.Narrative
{
    /// <summary>
    /// Central store for all narrative flags and counters.
    /// Registered in ServiceLocator at bootstrap.
    /// All mutations publish NarrativeFlagSetEvent.
    /// </summary>
    public class NarrativeState
    {
        private readonly Dictionary<string, int> _flags = new();

        public void SetFlag(string key, int value = 1)
        {
            _flags[key] = value;
            EventBus<NarrativeFlagSetEvent>.Publish(
                new NarrativeFlagSetEvent { FlagKey = key, Value = value });
        }

        public int GetFlag(string key) =>
            _flags.TryGetValue(key, out var v) ? v : 0;

        public bool HasFlag(string key) => GetFlag(key) > 0;

        public NarrativeStateData ToSaveData() =>
            new NarrativeStateData { Flags = new Dictionary<string, int>(_flags) };

        public void LoadFrom(NarrativeStateData data)
        {
            _flags.Clear();
            if (data?.Flags != null)
                foreach (var kv in data.Flags) _flags[kv.Key] = kv.Value;
        }
    }
}
```

**Defined flag keys** (string constants in `NarrativeFlags.cs`):

```csharp
public static class NarrativeFlags
{
    public const string Fragment_01_Collected   = "frag_01_collected";
    public const string Fragment_02_Collected   = "frag_02_collected";
    public const string Fragment_03_Collected   = "frag_03_collected";
    public const string Walkway_Rebuilt         = "walkway_rebuilt";
    public const string Choice_Made             = "choice_made";
    public const string Choice_FragmentA        = "choice_fragment_a"; // 1 if A chosen
    public const string Drift_Stabilized        = "drift_stabilized";
    public const string Reveal_Triggered        = "reveal_triggered";
}
```

---

## 6. DriftFSM — State Machine Specification

States: `Patrol → Alerted → Pursuing → Destabilizing → Stabilized`

| State | Entry Condition | Update Behaviour | Exit Condition |
|-------|----------------|-----------------|---------------|
| Patrol | Initial or player exits radius | Move between anchor points | Player enters detection radius |
| Alerted | Player detected | Face player, pause 1.5s | Pause complete |
| Pursuing | Post-alert | Move toward player at pursuit speed | Player enters RECALL range OR leaves outer radius |
| Destabilizing | Player in RECALL range | Interrupt RECALL (push event), push player back slowly | Player sustains RECALL 3s → Stabilized; player leaves radius → Pursuing |
| Stabilized | RECALL sustained 3s | Dissolve animation, crystal formation completes | Terminal — no exit |

**DriftFSM** is a plain C# class (not MonoBehaviour). `DriftController` MonoBehaviour ticks it.

---

## 7. Save System — Data Model

```csharp
// EchoZero.Data.Save
[JsonSerializable(typeof(GameSaveData))]
[JsonSerializable(typeof(NarrativeStateData))]
public partial class EchoZeroJsonContext : JsonSerializerContext { }

public class GameSaveData
{
    public string SaveVersion      { get; set; } = "1.0";
    public string SessionId        { get; set; }
    public SerializableVector3 PlayerPosition { get; set; }
    public string ActiveSceneName  { get; set; }
    public NarrativeStateData NarrativeState { get; set; }
    public List<string> CollectedFragmentIds { get; set; } = new();
    public string ChosenFragmentId { get; set; } // null if choice not yet made
}

public class NarrativeStateData
{
    public Dictionary<string, int> Flags { get; set; } = new();
}

public class SerializableVector3
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    public static SerializableVector3 From(Vector3 v) => new() { X = v.x, Y = v.y, Z = v.z };
    public Vector3 ToVector3() => new(X, Y, Z);
}
```

**Save file path**: `Path.Combine(Application.persistentDataPath, "save_01.json")`
**Checksum path**: Same directory, `"save_01.checksum"`
**Checksum algorithm**: SHA-256 of UTF-8 bytes of the serialized JSON string

---

## 8. Telemetry Event Model

All events written to `telemetry.jsonl` (append-only, one JSON object per line).

```json
// session_start
{ "t": "2026-09-14T05:08:30Z", "e": "session_start", "sid": "a1b2c3d4" }

// fragment_collected
{ "t": "2026-09-14T05:11:42Z", "e": "fragment_collected", "sid": "a1b2c3d4", "fid": "frag_01" }

// choice_made
{ "t": "2026-09-14T05:18:00Z", "e": "choice_made", "sid": "a1b2c3d4", "chosen": "frag_02" }

// recall_used
{ "t": "2026-09-14T05:09:15Z", "e": "recall_used", "sid": "a1b2c3d4", "target_type": "fragment", "success": true }

// drift_stabilized
{ "t": "2026-09-14T05:15:00Z", "e": "drift_stabilized", "sid": "a1b2c3d4", "attempts": 2 }

// scene_loaded
{ "t": "2026-09-14T05:12:00Z", "e": "scene_loaded", "sid": "a1b2c3d4", "scene": "AerieLowerScene", "load_ms": 890 }
```

Fields: `t` (UTC ISO-8601), `e` (event type), `sid` (session UUID — not tied to identity). No PII.

---

## 9. Assembly Definitions

Each script layer has its own `.asmdef` for compile isolation and faster iteration.

| Assembly | Folder | References |
|----------|--------|-----------|
| `EchoZero.Core` | Scripts/Core/ | None (no Unity deps except UnityEngine) |
| `EchoZero.Data` | Scripts/Data/ | EchoZero.Core |
| `EchoZero.Narrative` | Scripts/Narrative/ | EchoZero.Core, EchoZero.Data |
| `EchoZero.AI` | Scripts/AI/ | EchoZero.Core, EchoZero.Narrative |
| `EchoZero.World` | Scripts/World/ | EchoZero.Core, EchoZero.Data |
| `EchoZero.Gameplay` | Scripts/Gameplay/ | EchoZero.Core, EchoZero.World, EchoZero.Narrative |
| `EchoZero.UI` | Scripts/UI/ | EchoZero.Core, EchoZero.Gameplay, EchoZero.Narrative |
| `EchoZero.Tools` | Scripts/Tools/ | All (editor-only) |
| `EchoZero.Tests.EditMode` | Tests/EditMode/ | All above + NSubstitute (test-only) |
| `EchoZero.Tests.PlayMode` | Tests/PlayMode/ | All above + NSubstitute (test-only) |

---

*Last updated: Phase 2 — Foundation complete.*
