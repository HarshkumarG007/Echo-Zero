# Testing Strategy & Verification Guide — ECHO//ZERO

**Document Version:** 1.0  
**Status:** [CONFIRMED] Active Testing Strategy  
**Author:** AI QA & Test Engineer

---

## 1. Testing Philosophy

In ECHO//ZERO, automated testing is the foundational quality gate. Because game logic is partitioned into pure C# classes decoupled from `MonoBehaviour`, tests run primarily in **EditMode** without the overhead of scene loading, physics loops, or rendering pipelines.

### Test Categories
| Category | Purpose | Location | Execution Speed |
| :--- | :--- | :--- | :--- |
| **EditMode Tests** | Pure C# domain logic, algorithms, state machines, pub/sub dispatch | `Assets/Tests/EditMode/` | Sub-second ($< 1$ ms per test) |
| **PlayMode Tests** | Unity lifecycle, physics triggers, coroutines, CharacterController movement | `Assets/Tests/PlayMode/` | Multi-second (requires scene setup) |

---

## 2. Testing Conventions & Standards

### 2.1 File & Method Naming
- **Test Class:** `<ClassName>Tests.cs`
- **Method Pattern:** `<MethodUnderTest>_<StateOrScenario>_<ExpectedOutcome>`
```csharp
[Test]
public void TryRecall_WhenCooldownActive_IgnoresRaycastAndReturnsFalse()
```

### 2.2 Mocking with NSubstitute
Mock external interfaces using `NSubstitute` (`Substitute.For<T>()`) to isolate the class under test:
```csharp
var mockRecallable = Substitute.For<IRecallable>();
mockRecallable.CanRecall.Returns(true);

recallSystem.TryRecall(mockRecallable);

mockRecallable.Received(1).OnRecall();
```

### 2.3 Strict Test Isolation & Teardown
Static services and pub/sub buses must be cleared between tests to prevent test pollution:
```csharp
[TearDown]
public void TearDown()
{
    ServiceLocator.Clear();
    EventBus<FragmentCollectedEvent>.Clear();
    EventBus<ChoiceMadeEvent>.Clear();
    EventBus<DriftStabilizedEvent>.Clear();
}
```

---

## 3. Inventory of EditMode Test Suites

The codebase contains 25 EditMode test suites verifying all core gameplay and narrative systems:

| Test Class | Target Component | Verifications |
| :--- | :--- | :--- |
| `EventBusTests.cs` | `EventBus<T>` | Subscribe, publish, unsubscribe, multiple handlers, struct payloads |
| `ServiceLocatorTests.cs` | `ServiceLocator` | Register, TryGet, Get (defensive), Clear, type safety |
| `WorldStateTests.cs` | `WorldState` | Fragment registration, querying, state tracking |
| `RecallSystemTests.cs` | `RecallSystem` | Valid/invalid targets, range limits, event firing |
| `RecallAbilityTests.cs` | `RecallAbility` | Input rate-limiting, cooldown timer, raycast execution |
| `ContradictionTests.cs` | `FragmentRegistry` | Contradiction detection, mutual exclusion, choice resolution |
| `ChoicePedestalTests.cs` | `ChoicePedestal` | Pedestal interaction, validation guards, alternate invalidation |
| `NarrativeStateTests.cs` | `NarrativeState` | Flag setting, state querying, serialization round-trips |
| `MiraDialogueTests.cs` | `MiraDialogueSelector` | Basic flag-to-dialogue mapping |
| `MiraDialogueSelectorTests.cs`| `MiraDialogueSelector` | Detailed priority and fallback dialogue branches |
| `AdvancedMiraTests.cs` | `MiraDialogueSelector` | Contradiction-aware dialogue shifts and sequence weighting |
| `ReconstructionAnchorTests.cs`| `ReconstructionAnchor` | Required fragment check, anchor assembly events |
| `DriftUtilityBrainTests.cs` | `DriftUtilityBrain` | Heuristic scoring (Patrol vs. Pursue vs. Destabilize vs. Stabilize) |
| `MLPursuitScorerTests.cs` | `MLPursuitScorer` | Heuristic fallback when ML inference is unavailable (ADR-0005) |
| `SentisModelRunnerTests.cs` | `SentisModelRunner` | Model loading, tensor allocation, CPU execution safety |
| `SaveLoadTests.cs` | `SaveService` | Serialization round-trip, coordinate accuracy |
| `SaveCorruptionTests.cs` | `SaveService` | Corrupted JSON handling, graceful fallback (ADR-0006) |
| `TelemetryTests.cs` | `LocalTelemetryService` | Event logging, JSON formatting, PII absence |
| `GameplayUITests.cs` | `GameplayUI` | Reticle states, dialogue display visibility |
| `PauseMenuTests.cs` | `PauseMenuUI` | TimeScale toggling, resume/save invocations |
| `SettingsManagerTests.cs` | `SettingsManager` | Audio mixer volumes, resolution persistence |
| `AudioManagerTests.cs` | `AudioManager` | AudioSource routing to Mixer groups, volume curves |
| `SceneStreamerTests.cs` | `SceneStreamer` | Additive loading trigger logic |
| `RevealSequenceTests.cs` | `RevealSequence` | HUD disabling, final flag triggering |
| `NSubstituteVerificationTests.cs`| Test Infrastructure | Verifies NSubstitute DLL bindings and Castle.Core dynamic proxy |

---

## 4. Execution Commands

### In Unity Editor
1. Open `Window > General > Test Runner`.
2. Select the **EditMode** tab.
3. Click **Run All** (runs all 25 test suites).

### Headless CLI Batch Mode
To execute tests headlessly from command line:
```powershell
& "C:\Program Files\Unity\Hub\Editor\6000.6.0f1\Editor\Unity.exe" `
  -batchmode -quit `
  -projectPath "C:\Users\Lenovo\Downloads\Echo\UnityProject\ECHO" `
  -runTests `
  -testPlatform EditMode `
  -testResults "test_results.xml" `
  -logFile "test_run.log"
```

---

## 5. Scope of Testing & Exclusions

### Tested
- All business logic, algorithms, state machines, and data persistence.
- Event dispatch and decoupled cross-system communication.
- Defensive error recovery and edge cases.

### Intentionally Excluded from Automated Tests
- Custom Editor inspectors and GUI windows.
- Third-party package internal code.
- Visual/aesthetic rendering correctness (shader graph appearance, bloom quality, camera feel)—these are verified through human playtesting and recorded video reviews.
