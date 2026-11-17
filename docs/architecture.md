# Architecture Specification — ECHO//ZERO

**Document Version:** 1.0  
**Status:** [CONFIRMED] Aligned with Codebase Reality (Post ADR-0005 & ADR-0006)  
**Author:** AI Technical Lead & Senior Software Architect

---

## 1. System Overview & Style

ECHO//ZERO is designed as a **decoupled, layer-oriented architecture** targeting Unity 6. The core philosophy separates domain logic from Unity's execution lifecycle:
- **Pure C# Domain Classes:** Business logic, state calculation, serialization, and narrative decisions are implemented as testable C# classes independent of `UnityEngine.Object`.
- **Thin MonoBehaviours:** Components attached to GameObjects act strictly as bridges between Unity events (physics triggers, collision, input) and domain systems.
- **Inversion of Control (IoC):** Dependencies are managed via a central `ServiceLocator`.
- **Event-Driven Communication:** Cross-layer decoupled messaging is handled exclusively via a type-safe generic `EventBus<T>`.

---

## 2. Technology Stack & Justification

| Technology | Selection | Justification |
| :--- | :--- | :--- |
| **Engine** | Unity 6.3 LTS (`6000.6.0f1`) | Long-term support window through December 2027 (ADR-0001). |
| **Render Pipeline** | Universal Render Pipeline (URP 17.6.0) | High performance headroom for mobile RTX 4060 GPU and crystalline shader graphs (ADR-0001). |
| **Scripting Backend** | Mono (Windows 64-bit) | Reliable, fast local builds without requiring external IL2CPP toolchains. |
| **UI Framework** | UI Toolkit (`com.unity.ugui` + UI Elements) | Data-driven layout via UXML and stylesheet transitions via USS. |
| **Input System** | Unity Input System (`com.unity.inputsystem` 1.20.0) | Modern action-map-based input abstraction avoiding deprecated `Input.GetKey`. |
| **Testing** | NUnit + NSubstitute | Rapid EditMode unit testing with isolated interface mocking. |
| **Inference Engine** | Unity Sentis (`com.unity.sentis` 2.1.2) | Reserved for Phase 7 ML experiments (ADR-0004); pulled from Phase 3 build path (ADR-0005). |

---

## 3. Layer Model & Boundaries

The codebase is partitioned into distinct assembly definitions (`.asmdef`):

```
┌────────────────────────────────────────────────────────┐
│                      EchoZero.App                      │
│                  (Composition Root)                    │
└────────┬───────────────────────────────┬───────────────┘
         │                               │
         ▼                               ▼
┌──────────────────┐           ┌──────────────────┐
│ EchoZero.Gameplay│           │ EchoZero.Narrative│
└────────┬─────────┘           └────────┬─────────┘
         │                              │
         ▼                              ▼
┌──────────────────┐           ┌──────────────────┐
│   EchoZero.UI    │           │   EchoZero.World │
└────────┬─────────┘           └────────┬─────────┘
         │                              │
         ▼                              ▼
┌──────────────────┐           ┌──────────────────┐
│   EchoZero.Data  │           │   EchoZero.AI    │
└────────┬─────────┘           └────────┬─────────┘
         │                              │
         └───────────────┬──────────────┘
                         ▼
               ┌───────────────────┐
               │   EchoZero.Core   │
               │ (Contracts & Bus) │
               └───────────────────┘
```

### Layer Responsibilities
- **Core (`EchoZero.Core`):** Fundamental abstractions (`ServiceLocator`, `EventBus<T>`), settings manager, world state, and shared interfaces (`IRecallable`, `ISaveService`, `ITelemetryService`).
- **App (`EchoZero.App`):** `Bootstrap.cs` composition root initializing services at `[DefaultExecutionOrder(-1000)]`.
- **Data (`EchoZero.Data`):** Serialization and persistence (`SaveService`), local telemetry persistence (`TelemetryService`).
- **Gameplay (`EchoZero.Gameplay`):** Player movement (`PlayerMovement`), RECALL raycasting controller (`RecallAbility`), material animation (`MaterialPropertyFader`).
- **Narrative (`EchoZero.Narrative`):** Fragment management (`FragmentRegistry`), contradiction pedestals (`ChoicePedestal`), dynamic dialogue selection (`MiraDialogueSelector`), and the reveal coordinator (`RevealSequence`).
- **AI (`EchoZero.AI`):** The Drift entity utility brain (`DriftUtilityBrain`), heuristic scorers, and state tracking.
- **UI (`EchoZero.UI`):** HUD controller (`GameplayUI`), dialogue hooks (`GameplayUIHook`), and pause menu (`PauseMenuUI`).
- **World (`EchoZero.World`):** Environmental reconstruction anchors (`ReconstructionAnchor`), scene streaming.
- **Tools (`EchoZero.Tools`):** Editor batch build pipeline (`BuildPipeline.cs`), procedural scene builders (`GameAssembler.cs`, `MissingPiecesPatcher.cs`).

---

## 4. Communication Architecture: EventBus<T>

Systems communicate horizontally without direct dependencies by publishing and subscribing to immutable structs via `EventBus<T>`:

```
  Publisher                    Event Struct                Subscriber(s)
  ─────────────────────────────────────────────────────────────────────────────
  MemoryFragmentPickup  ──►  FragmentCollectedEvent  ──► FragmentRegistry
                                                     ──► GameplayUIHook
  ChoicePedestal        ──►  ChoiceMadeEvent         ──► NarrativeState
  RecallAbility         ──►  RecallFiredEvent         ──► LocalTelemetryService
  DriftUtilityBrain     ──►  DriftStabilizedEvent     ──► NarrativeState
  SaveService           ──►  SaveCompletedEvent       ──► GameStateManager
  NarrativeState        ──►  NarrativeFlagSetEvent    ──► MiraDialogueSelector
  RevealSequence        ──►  RevealTriggeredEvent     ──► GameplayUI
```

---

## 5. Subsystem Architectures

### 5.1 Service Locator & Lifecycle
All core services register in `ServiceLocator` during `Bootstrap.Awake()`:
- `ISaveService` $\rightarrow$ `SaveService`
- `ITelemetryService` $\rightarrow$ `LocalTelemetryService`
- `IConfigService` $\rightarrow$ `ConfigService`
- `WorldState`, `NarrativeState`, `GameStateManager`, `SettingsManager`

Consumption in game code uses defensive queries:
```csharp
if (ServiceLocator.TryGet<ISaveService>(out var saveService))
{
    saveService.Save(data);
}
```

### 5.2 Narrative & Contradiction Pipeline
1. `RecallAbility` executes a centered screen raycast.
2. Target implements `IRecallable.OnRecall()`.
3. `MemoryFragmentPickup` fires `FragmentCollectedEvent`.
4. `FragmentRegistry` evaluates collected fragment IDs against registered contradiction pairs.
5. If a contradiction is detected, `FragmentRegistry.IsPlayerMustChooseState` becomes `true`.
6. Interacting with a `ChoicePedestal` invokes `ValidateFragment()`, invalidating the alternate fragment.
7. `MiraDialogueSelector` receives `NarrativeFlagSetEvent` and updates its active authored dialogue tree.

### 5.3 The Drift AI (ADR-0005)
The Drift AI evaluates decisions dynamically through `DriftUtilityBrain` using deterministic heuristic delegate scorers:
- **Patrol:** Constant baseline score (0.1).
- **Pursuing:** Activates when player distance $< 15.0$ m (score 1.0).
- **Destabilizing:** Activates when player distance $< 3.0$ m (score 2.0).
- **Stabilized:** Activates when player sustains RECALL for $\ge 3.0$ seconds (score 100.0, terminal).

*Note: Per ADR-0005, the ONNX/Sentis ML model (`MLPursuitScorer`) was removed from the active Phase 3 build path and preserved in `EchoZero.AI` for Phase 7 experimentation.*

### 5.4 Persistence Architecture (ADR-0002 & ADR-0006)
- Plain JSON file persisted to `Application.persistentDataPath/save_01.json`.
- Serializes `GameSaveData`: position (`SerializableVector3`), collected fragment IDs, validated choice ID, and narrative flags.
- **Fail-Safe Loading:** Corrupted or malformed JSON falls back gracefully to a clean new-game state without crashing. Cryptographic checksums and encryption were explicitly rejected per ADR-0006.

---

## 6. Procedural Scene Reconstruction

Per `AGENTS.md` guidelines, AI agents do not modify binary Unity scene files directly. Runtime scene hierarchy, colliders, CharacterControllers, and UI Toolkit attachments are constructed via editor automation:
- `GameAssembler.cs`: Builds the complete graybox geometry, triggers, and camera rigs.
- `MissingPiecesPatcher.cs`: Automatically patches missing kinematic Rigidbodies, assigns ScriptableObjects, and binds `GameplayUIHook`.
