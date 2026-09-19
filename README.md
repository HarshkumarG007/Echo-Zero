# ECHO//ZERO

> *"A grieving AI named ECHO reconstructs reality from memory.  
> ZERO, the player, discovers they are made of the same grief they came to resolve."*

**ECHO//ZERO** is a sci-fi/fantasy mystery vertical slice — engineered entirely through **Agentic AI Coding** using Google DeepMind's Antigravity system. Every system, test, and architecture decision was authored by AI agents operating under a strict engineering contract defined in `AGENTS.md`. A human defined the rules and the vision. The agents built the game.

---

## 📑 Documentation Index

*Everything you need to understand this project is one click away. Here's why each file exists.*

| Document | What it is | Why you should open it |
|---|---|---|
| [`AGENTS.md`](./AGENTS.md) | The law of this repo | **Start here.** Defines every constraint the AI agents must follow — role separation, forbidden behaviours, commit format, testing rules, performance budgets. If you want to contribute or run agents against this repo, this file is mandatory reading. |
| [`ARCHITECTURE.md`](./ARCHITECTURE.md) | The system map | Shows how the layers talk to each other — Service Locator, EventBus, Narrative pipeline, AI layer. Read this before touching any code to understand what owns what. |
| [`DECISIONS.md`](./DECISIONS.md) | 6 ADRs that shaped everything | Every non-obvious technical decision is recorded here with its full context, reasoning, and what was explicitly rejected. ADR-0005 documents the Sentis driver crash that rewrote the AI layer. ADR-0006 explains why the checksum save system was removed. |
| [`ROADMAP.md`](./ROADMAP.md) | 12-phase delivery plan | Phases 0–11 from raw idea to portfolio release. Each phase has a quality gate that blocked the next phase from starting. Shows the project's full arc at a glance. |
| [`TASKS.md`](./TASKS.md) | 38 implementation tasks | Every TASK-XXX committed to the codebase maps to an entry here. Dependency-ordered, with acceptance criteria and test requirements. The living record of what was built and in what order. |
| [`CODING_STANDARDS.md`](./CODING_STANDARDS.md) | Style rules agents enforced | Namespace conventions, MonoBehaviour rules, method length caps, naming contracts. Agents that violated these were instructed to self-correct before committing. |
| [`TESTING.md`](./TESTING.md) | Test philosophy and rules | Explains why every business-logic class has a paired EditMode test, how NSubstitute mocks are used, and what PlayMode tests are for. |
| [`SECURITY.md`](./SECURITY.md) | Security posture and threat model | Documents the PII audit, save-file security decisions, and telemetry anonymisation strategy. |
| [`RELEASE_NOTES.md`](./RELEASE_NOTES.md) | What shipped and what didn't | The honest account of what made it into the final build, known limitations, and what was deliberately deferred to a later phase. |
| [`DEVELOPMENT.md`](./DEVELOPMENT.md) | How to set up and run the project | Unity version, required packages, Editor menu steps, and what to do when the scene is empty. |

---

## 🗃️ Repository Tree

```
Echo/
├── AGENTS.md                          ← Agent law — read before anything else
├── ARCHITECTURE.md                    ← Layer contracts and system boundaries
├── CODING_STANDARDS.md                ← Style rules enforced across all commits
├── DECISIONS.md                       ← ADR-0001 through ADR-0006
├── DEVELOPMENT.md                     ← Setup and run guide
├── README.md                          ← You are here
├── ROADMAP.md                         ← 12-phase delivery plan with quality gates
├── RELEASE_NOTES.md                   ← Shipped build: what's in, what's not
├── SECURITY.md                        ← Threat model and PII audit
├── TASKS.md                           ← 38 TASK-XXX entries, dependency-ordered
├── TESTING.md                         ← Test strategy and rules
│
├── config/
│   ├── antigravity.project.json       ← Machine-readable project manifest for agents
│   └── project.yaml                   ← Project metadata
│
├── docs/
│   ├── ai-architecture.md             ← Deep dive: Utility AI, Sentis, EventBus wiring
│   ├── game-design.md                 ← Design thesis, mechanic breakdown, narrative pillars
│   ├── performance-budget.md          ← Per-system frame time and memory targets
│   ├── technical-design.md            ← Low-level design: serialization, streaming, input
│   ├── evaluation/
│   │   ├── experiment-registry.md     ← ML experiment log (Phase 7)
│   │   ├── reproducibility-checklist.md
│   │   └── evaluation-report.md       ← Phase 8 results vs Phase 6 baseline
│   ├── portfolio/
│   │   ├── career-assets.md           ← STAR-format interview bullets, resume points
│   │   ├── demo-script.md             ← 20-minute walkthrough script with VO cues
│   │   └── devlog-postmortem.md       ← Honest retrospective: what worked, what didn't
│   └── release/
│       └── itch-io-page.md            ← Storefront copy and installation instructions
│
└── UnityProject/
    └── ECHO/
        ├── Assets/
        │   ├── _Project/
        │   │   ├── Scripts/
        │   │   │   ├── AI/                      ← Drift FSM, Utility Brain, ML scorer (Phase 7)
        │   │   │   │   ├── Drift/
        │   │   │   │   │   ├── DriftController.cs
        │   │   │   │   │   ├── DriftState.cs
        │   │   │   │   │   ├── DriftUtilityBrain.cs
        │   │   │   │   │   └── MLPursuitScorer.cs  ← Reserved Phase 7 (ADR-0005)
        │   │   │   │   └── Utility/
        │   │   │   │       ├── UtilityAction.cs
        │   │   │   │       └── UtilityScorer.cs
        │   │   │   ├── App/
        │   │   │   │   └── Bootstrap.cs             ← Composition root, service wiring
        │   │   │   ├── Core/
        │   │   │   │   ├── Audio/AudioManager.cs
        │   │   │   │   ├── Events/EventBus.cs        ← Typed pub/sub backbone
        │   │   │   │   ├── ML/                       ← Sentis runner (Phase 7)
        │   │   │   │   │   ├── ISentisModelRunner.cs
        │   │   │   │   │   └── SentisModelRunner.cs
        │   │   │   │   ├── Settings/SettingsManager.cs
        │   │   │   │   ├── WorldState/WorldState.cs
        │   │   │   │   └── ServiceLocator.cs
        │   │   │   ├── Data/
        │   │   │   │   ├── Save/SaveService.cs       ← Plain JSON, ADR-0002/0006
        │   │   │   │   └── Telemetry/TelemetryService.cs
        │   │   │   ├── Gameplay/
        │   │   │   │   ├── Player/PlayerMovement.cs
        │   │   │   │   ├── Recall/RecallAbility.cs
        │   │   │   │   └── VFX/MaterialPropertyFader.cs
        │   │   │   ├── Narrative/
        │   │   │   │   ├── ChoicePedestal.cs
        │   │   │   │   ├── FragmentRegistry.cs
        │   │   │   │   ├── MemoryFragmentPickup.cs
        │   │   │   │   ├── Mira/MiraDialogueSelector.cs
        │   │   │   │   ├── NarrativeState.cs
        │   │   │   │   └── RevealSequence.cs
        │   │   │   ├── Tools/
        │   │   │   │   ├── Benchmarking/PerformanceBenchmark.cs
        │   │   │   │   └── Editor/
        │   │   │   │       ├── BuildPipeline.cs      ← Headless Mono build script
        │   │   │   │       ├── GameAssembler.cs      ← Procedural scene constructor
        │   │   │   │       ├── GameVisualizer.cs
        │   │   │   │       └── MissingPiecesPatcher.cs
        │   │   │   ├── UI/
        │   │   │   │   ├── GameplayUI.cs
        │   │   │   │   ├── GameplayUIHook.cs
        │   │   │   │   └── PauseMenuUI.cs
        │   │   │   └── World/
        │   │   │       └── ReconstructionAnchor.cs
        │   │   ├── Data/
        │   │   │   └── Narrative/
        │   │   │       └── Fragment_TheFirstMemory.asset
        │   │   ├── UI/
        │   │   │   ├── GameplayUI.uxml
        │   │   │   └── GameplayUI.uss
        │   │   └── World/Scenes/
        │   │       └── Aerie.unity
        │   └── Tests/
        │       └── EditMode/              ← 15+ test classes (NUnit + NSubstitute)
        └── ProjectSettings/
```

---

## 🧩 Complete Architecture

```
┌─────────────────────────────────────────────────────────────────────┐
│                         ECHO//ZERO Runtime                          │
│                                                                     │
│  BootstrapScene (loads first, never unloads)                        │
│  ┌───────────────────────────────────────────────────────────────┐  │
│  │  Bootstrap.cs  [DefaultExecutionOrder(-1000)]                 │  │
│  │                                                               │  │
│  │  Registers into ServiceLocator:                               │  │
│  │    • IConfigService     (ConfigService)                       │  │
│  │    • ISaveService       (SaveService  — plain JSON)           │  │
│  │    • ITelemetryService  (TelemetryService)                    │  │
│  │    • ISceneLoader       (SceneLoader)                         │  │
│  │    • WorldState                                               │  │
│  │    • NarrativeState                                           │  │
│  │    • GameStateManager                                         │  │
│  │    • SettingsManager                                          │  │
│  └───────────────────────────────────────────────────────────────┘  │
│                              │                                       │
│                    Additive load ▼                                   │
│  ┌───────────────────────────────────────────────────────────────┐  │
│  │  Aerie.unity  (game scene, loaded additively)                 │  │
│  │                                                               │  │
│  │  [PLAYER]                     [DRIFT]                         │  │
│  │  ├─ PlayerMovement            ├─ DriftController              │  │
│  │  ├─ RecallAbility             │   └─ DriftUtilityBrain        │  │
│  │  └─ CharacterController       │       ├─ Patrol  (0.1 score)  │  │
│  │                               │       ├─ Pursuing (dist < 15m)│  │
│  │  [NARRATIVE]                  │       ├─ Destabilizing (<3m)  │  │
│  │  ├─ MemoryFragmentPickup      │       └─ Stabilized (RECALL)  │  │
│  │  ├─ ChoicePedestal            └─ CharacterController          │  │
│  │  ├─ ReconstructionAnchor                                      │  │
│  │  └─ RevealSequence            [UI]                            │  │
│  │                               ├─ UIDocument (UI Toolkit)      │  │
│  │  [WORLD]                      ├─ GameplayUI.uxml              │  │
│  │  └─ SceneLoader               ├─ GameplayUIHook               │  │
│  │      (additive streaming)     └─ PauseMenuUI                  │  │
│  └───────────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────────┘

─────────────────────── CROSS-CUTTING: EventBus<T> ───────────────────

  Publisher                    Event                     Subscriber(s)
  ─────────────────────────────────────────────────────────────────────
  MemoryFragmentPickup  ──►  FragmentCollectedEvent  ──► FragmentRegistry
                                                     ──► GameplayUIHook
  ChoicePedestal        ──►  ChoiceMadeEvent         ──► NarrativeState
  RecallAbility         ──►  RecallFiredEvent         ──► TelemetryService
  DriftUtilityBrain     ──►  DriftStabilizedEvent     ──► NarrativeState
  SaveService           ──►  SaveCompletedEvent       ──► GameStateManager
  NarrativeState        ──►  NarrativeFlagSetEvent    ──► MiraDialogueSelector
  RevealSequence        ──►  RevealTriggeredEvent     ──► GameplayUI (hides HUD)

─────────────────────── SERVICE LOCATOR (singleton dict) ─────────────

  ServiceLocator.Register<T>(impl)   ← Bootstrap only, Awake()
  ServiceLocator.TryGet<T>(out var)  ← anywhere in game code (safe)
  ServiceLocator.Clear()             ← tests only (SetUp / TearDown)

─────────────────────── NARRATIVE PIPELINE ───────────────────────────

  WorldState          ← registers which objects have which fragments
        │
        ▼
  RecallAbility  ──► raycast ──► IRecallable.OnRecall()
                                       │
                          ┌────────────┴───────────────┐
                          ▼                            ▼
                  MemoryFragment               DriftController
                  FragmentRegistry         (sustain = Stabilized)
                          │
                          ▼
                  NarrativeState.SetFlag()
                          │
                          ▼
                  MiraDialogueSelector
                  (returns authored string by flag priority)

─────────────────────── SAVE / LOAD (ADR-0002, 0006) ─────────────────

  GameSaveData (plain C# class)
    ├─ SaveVersion, SessionId
    ├─ PlayerPosition  (SerializableVector3)
    ├─ ActiveSceneName
    ├─ NarrativeState  (flag dict)
    ├─ CollectedFragmentIds  []
    └─ ChosenFragmentId

  JSON serialized via System.Text.Json (AOT-safe source gen)
  Written to: Application.persistentDataPath/save_01.json
  On corrupt / malformed JSON → returns null → new game (no crash)

─────────────────────── AI LAYER (ADR-0005) ──────────────────────────

  Phase 3  →  DriftUtilityBrain  (deterministic heuristics only)
  ┌─────────────────────────────────────────────────────────────┐
  │  Action       Scorer                       Score            │
  │  Patrol       DelegateScorer               always 0.1       │
  │  Pursuing     DelegateScorer               dist<15m → 1.0   │
  │  Destabilize  DelegateScorer               dist< 3m → 2.0   │
  │  Stabilized   DelegateScorer               recall≥3s → 100  │
  └─────────────────────────────────────────────────────────────┘

  Phase 7  →  MLPursuitScorer  (ONNX via Unity Sentis, CPU backend)
              Reserved. Not wired into Phase 3 build.
              See: ADR-0004, ADR-0005, MLPursuitScorer.cs

─────────────────────── ASSEMBLY GRAPH ───────────────────────────────

  EchoZero.App  ──► EchoZero.Core
                ──► EchoZero.Data
                ──► EchoZero.Gameplay
                ──► EchoZero.Narrative
                ──► EchoZero.UI
                ──► EchoZero.World
                ──► EchoZero.AI

  EchoZero.AI   ──► EchoZero.Core
                ──► EchoZero.Gameplay
                    (Unity.InferenceEngine removed — ADR-0005)

  EchoZero.Tests.EditMode ──► all assemblies above
                          ──► Unity.TestFramework
                          ──► NSubstitute
```

---

## 📉 Failures, Roadblocks, and Solutions

Agentic coding is rarely a straight line. Every failure below was real, verifiable, and fixed — not papered over.

### 🔴 IL2CPP Build Failure
**What happened:** The build pipeline targeted IL2CPP but the local Unity installation was missing the IL2CPP backend module. Hard crash, no `.exe`.  
**Fix:** `BuildPipeline.cs` now detects the missing module at build time and silently falls back to Mono scripting backend, completing the build without any manual intervention.

### 👻 The Graybox Ghost
**What happened:** The entire backend (EventBus, services, AI brain, save system) was fully implemented and tested — but the game was visually empty. No player. No Drift. No fragment. `AGENTS.md` forbids binary scene file edits.  
**Fix:** `GameAssembler.cs` and `MissingPiecesPatcher.cs` — pure C# Editor scripts that procedurally construct the scene's full runtime graph: spawn player capsule, wire up CharacterController, add UIDocument, assign ScriptableObjects, create the Drift capsule, place the memory fragment cube — all without touching a `.unity` file by hand.

### 🌡️ The 5,000 FPS GPU Melt
**What happened:** An empty URP scene with no frame cap rendered at 5,000+ FPS, driving the RTX 4060 to 86°C and 100% utilization within seconds.  
**Fix:** Two lines in `Bootstrap.Awake()`:
```csharp
Application.targetFrameRate = 60;
QualitySettings.vSyncCount  = 1;
```
GPU utilization dropped to ~5%. Temperatures normalized.

### ⚡ Sentis Intel Driver Crash (`igc64.dll`)
**What happened:** Unity Sentis defaulted to `BackendType.GPUCompute` and attempted inference on the Intel integrated GPU instead of the RTX 4060. Immediate access violation in `igc64.dll`. `.exe` crash.  
**Fix:** `SentisModelRunner` pinned to `BackendType.CPU`. This was later formalized as ADR-0005, which removed Sentis from the Phase 3 build path entirely.

### 🔲 The Invisible Trigger Colliders
**What happened:** The player walked through the memory fragment. `OnTriggerEnter` never fired despite the trigger volume being correctly placed.  
**Fix:** Unity physics requires at least one participant in a trigger collision to have a `Rigidbody`. The fragment received a **kinematic** `Rigidbody` component (no physics simulation, just trigger enabling). Collision worked instantly.

---

## 🚀 Running the Project

1. Open in **Unity 6000.6.0f1** (URP — required).
2. Open `Assets/_Project/World/Scenes/Aerie.unity`.
3. If the scene is empty:
   - `ECHO > Assemble Full Game Scene`
   - `ECHO > Patch Missing Pieces`
4. Hit **Play**.
5. To produce a standalone build: `ECHO > Build Windows 64-bit (Mono)`.

---

## 🔀 Git History (abridged)

```
aa05d53  fix(ai,data): ADR-0005 remove Sentis from Phase 3 Drift; ADR-0006 revert save to plain JSON
5edd8c0  chore(docs): Final Polish and Completed Architecture
bfea151  fix(tools): point BuildPipeline to the correct existing Aerie scene
9b181cf  fix(tools): add Mono build option as fallback for environments without IL2CPP installed
687b714  fix(tests): resolve remaining compilation errors in test suites
0143523  docs: finalize portfolio assets and overhaul README (TASK-035 - 038)
69f0e22  chore(release): configure IL2CPP build and draft release notes (TASK-032, 033, 034)
168c4a4  docs: mark Phase 9 (Red Team) complete
7d93317  test(core): add automated tests for save corruption recovery (TASK-031)
2803420  test(core): add telemetry PII security audit test (TASK-030)
57d0e46  fix(narrative): gracefully reject invalid pedestal RECALL (TASK-029)
31ccee8  feat(gameplay): add RECALL input rate limiting (TASK-028)
a7b63c1  docs(evaluation): finalize Phase 8 evaluation report (TASK-027)
9e34c4f  test(benchmarking): add ML inference performance benchmark (TASK-026)
58aa805  docs(evaluation): add experiment registry and reproducibility checklist (TASK-025)
01e0a68  feat(ai): ML-driven Drift Navigation Policy (TASK-023)
7c1fc1e  feat(ml): integrate Unity Sentis for local ML inference (TASK-022)
2ff79fe  fix(audit): resolve all P1/P2/P3 issues from code audit
436b0f9  docs: complete Phase 6 Tasks 19-21 and pass Quality Gate 6
d00b129  feat(phase6): implement Utility AI, advanced Mira dialogue, and telemetry (TASKS 19-21)
684c5cf  feat(phase5): add AudioManager, SettingsManager, VFX Fader, UI Polish (TASKS 15-18)
5f443b4  feat(phase4): implement SceneStreamer, PauseMenuUI, RevealSequence (TASKS 12-14)
4390c9d  feat(phase4): implement GameplayUI, Drift FSM, ChoicePedestal (TASKS 9-11)
de314ba  feat(phase3): complete TASK-006, TASK-007, TASK-008 for Phase 3 closure
75ec702  feat(narrative): implement fragment registry and contradiction logic (TASK-005)
8aa1684  feat(core): implement WorldState and update RecallSystem (TASK-004)
4f6c925  feat(gameplay): implement PlayerMovement playmode test and Aerie scene builder
9228070  test(save): implement save/load tests
8d7977b  feat(world): add empty Aerie scene
325f2df  chore(project): initial Unity project scaffold
```

---

## 🗺️ Phases at a Glance

| Phase | Name | Status |
|---|---|---|
| 0 | Discovery — prove the idea | ✅ Complete |
| 1 | Architecture — design before building | ✅ Complete |
| 2 | Foundation — agent-ready workspace | ✅ Complete |
| 3 | Build — core gameplay (player, RECALL, save) | ✅ Complete |
| 4 | Vertical Slice — playable start to finish | ✅ Complete |
| 5 | Polish — shaders, audio, UI transitions | ✅ Complete |
| 6 | AI — Utility Brain, Mira dialogue weights | ✅ Complete |
| 7 | ML Experiments — ONNX Sentis (conditional) | ✅ Complete |
| 8 | Evaluation — benchmarks, experiment registry | ✅ Complete |
| 9 | Red Team — corruption, spam, PII audit | ✅ Complete |
| 10 | Release — build pipeline, release notes | ✅ Complete |
| 11 | Portfolio — README, devlog, demo script | ✅ Complete |

---

*Developed as a portfolio showcase of Agentic Software Engineering.*  
*Engine: Unity 6000.6.0f1 · Pipeline: URP · Backend: Mono · AI: Google DeepMind Antigravity*
