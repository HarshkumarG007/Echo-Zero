# Sequential Project Task Roadmap — ECHO//ZERO

**Document Version:** 1.0  
**Status:** [CONFIRMED] Baseline History Documented  
**Governance:** Master Prompt v2.0 Task Execution Protocol (One task at a time)

---

## 1. Task Status Definitions

- `NOT STARTED`: Task is defined but not yet scheduled.
- `BLOCKED`: Task cannot proceed due to missing dependencies or open architectural questions.
- `IN PROGRESS`: Currently active authorized task.
- `IMPLEMENTED — UNVERIFIED`: Code and assets exist but formal independent verification is pending.
- `VERIFIED`: Acceptance criteria verified with recorded test output, command logs, or manual demonstration.
- `DEFERRED`: Deliberately postponed to a future milestone.
- `REJECTED`: Abandoned or superseded by an ADR.

---

## 2. Completed Baseline Tasks (Tasks 001–038)

### Phase 3 — Scaffold & Core Gameplay
- **TASK-001: Unity Project Initialisation**  
  *Status:* `VERIFIED`  
  *Objective:* Scaffold Unity 6.3 LTS / URP project, establish folder layout per ADR-0003, configure initial git repository.
- **TASK-002: Save/Load Round Trip**  
  *Status:* `VERIFIED`  
  *Objective:* Implement plain JSON persistence for `SaveData` and defensive loading tests.
- **TASK-003: Player Controller & Graybox Movement**  
  *Status:* `VERIFIED`  
  *Objective:* Implement third-person `PlayerMovement` with `CharacterController` and Input System actions.
- **TASK-004: RECALL Skeleton & WorldState**  
  *Status:* `VERIFIED`  
  *Objective:* Implement `RecallAbility`, `IRecallable`, and `WorldState` event firing.
- **TASK-005: Fragment Registry & Contradiction Logic**  
  *Status:* `VERIFIED`  
  *Objective:* Create ScriptableObject fragment definitions and mutual contradiction evaluation logic.
- **TASK-006: NarrativeState Service & Flag Dispatch**  
  *Status:* `VERIFIED`  
  *Objective:* Implement `NarrativeState` service and `NarrativeFlagSetEvent` pub/sub.
- **TASK-007: ReconstructionAnchor & World-Build Sequence**  
  *Status:* `VERIFIED`  
  *Objective:* Implement `ReconstructionAnchor` listening for corroborating fragments to assemble structures.
- **TASK-008: Mira Stub NPC & Dialogue Selector**  
  *Status:* `VERIFIED`  
  *Objective:* Deterministic flag-based dialogue selection via `MiraDialogueSelector`.

### Phase 4 — Vertical Slice Assembly
- **TASK-009: Basic UI (HUD Reticle & Dialogue Display)**  
  *Status:* `VERIFIED`  
  *Objective:* Build UI Toolkit HUD overlay with reticle targeting and dialogue presentation.
- **TASK-010: The Drift Encounter (FSM)**  
  *Status:* `VERIFIED`  
  *Objective:* Implement Drift state transitions (Patrol $\rightarrow$ Alerted $\rightarrow$ Pursuing $\rightarrow$ Destabilizing $\rightarrow$ Stabilized).
- **TASK-011: The Choice Pedestal Interaction**  
  *Status:* `VERIFIED`  
  *Objective:* Build world object resolving contradiction state and permanently validating chosen memory.
- **TASK-012: Aerie Scene Streaming**  
  *Status:* `VERIFIED`  
  *Objective:* Additive scene loading and unloading triggers for upper and lower chambers.
- **TASK-013: Pause Menu & Game State Management**  
  *Status:* `VERIFIED`  
  *Objective:* Implement `GameStateManager` with time-scale pausing and UI Toolkit pause menu.
- **TASK-014: The Reveal Sequence Manager**  
  *Status:* `VERIFIED`  
  *Objective:* Core reveal trigger volume disabling HUD and triggering climactic exposition.

### Phase 5 — Polish & Systems Integration
- **TASK-015: Audio Management System**  
  *Status:* `VERIFIED`  
  *Objective:* Centralized `AudioManager` with mixer routing (Music, SFX, Voiceover).
- **TASK-016: Settings Manager & UI**  
  *Status:* `VERIFIED`  
  *Objective:* Persistent audio volume and display resolution preferences.
- **TASK-017: UI Toolkit Transitions & Styling**  
  *Status:* `VERIFIED`  
  *Objective:* Smooth USS state transitions for HUD dialogue and menus.
- **TASK-018: VFX & Material Property Fader**  
  *Status:* `VERIFIED`  
  *Objective:* Tween material emission and dissolving properties dynamically via Coroutines.

### Phase 6 — AI Refinement & Telemetry
- **TASK-019: Utility-Based AI for Drift**  
  *Status:* `VERIFIED`  
  *Objective:* Dynamic action evaluation via `DriftUtilityBrain` and environmental scorers.
- **TASK-020: Advanced Mira Dialogue Logic**  
  *Status:* `VERIFIED`  
  *Objective:* Sequence-aware dialogue evaluation accounting for collection order and contradiction weight.
- **TASK-021: Local Telemetry Service**  
  *Status:* `VERIFIED`  
  *Objective:* Anonymous session event logging to local JSON.

### Phase 7 & 8 — Machine Learning & Evaluation (Held / Isolated per ADR-0005)
- **TASK-022: Unity Sentis Integration**  
  *Status:* `VERIFIED`  
  *Objective:* Configure Sentis package and `SentisModelRunner` (pinned to CPU backend).
- **TASK-023: ML Pursuit Scorer**  
  *Status:* `VERIFIED` (Isolated)  
  *Objective:* Scorer logic using ONNX inference (reserved for Phase 7; excluded from Phase 3 build).
- **TASK-024: Adaptive Mira Dialogue Weights**  
  *Status:* `VERIFIED`  
  *Objective:* Playstyle-aware dialogue biasing logic.
- **TASK-025: Experiment Registry & Reproducibility Checklist**  
  *Status:* `VERIFIED`  
  *Objective:* Documentation records in `docs/evaluation/`.
- **TASK-026: Performance Benchmark Suite**  
  *Status:* `VERIFIED`  
  *Objective:* Headless inference benchmarking script (`PerformanceBenchmark.cs`).
- **TASK-027: Evaluation Report**  
  *Status:* `VERIFIED`  
  *Objective:* Technical analysis comparing baseline vs. ML-augmented behaviors.

### Phase 9 — Red Team & Hardening
- **TASK-028: RECALL Input Rate Limiting**  
  *Status:* `VERIFIED`  
  *Objective:* Add cooldown timer to `RecallAbility` to prevent raycast spamming.
- **TASK-029: Choice Pedestal Boundary Guards**  
  *Status:* `VERIFIED`  
  *Objective:* Gracefully reject RECALL interaction when not in an active contradiction state.
- **TASK-030: Telemetry Security & PII Strip**  
  *Status:* `VERIFIED`  
  *Objective:* Audit and test `TelemetryPayload` to guarantee zero PII serialization.
- **TASK-031: Save Corruption Fallback Verification**  
  *Status:* `VERIFIED`  
  *Objective:* Automated tests verifying graceful fallback to new game state upon corrupted save JSON.

### Phase 10 & 11 — Release Pipeline & Portfolio
- **TASK-032: Windows 64-bit Build Pipeline**  
  *Status:* `VERIFIED`  
  *Objective:* Headless standalone build pipeline (`BuildPipeline.cs`) with Mono fallback.
- **TASK-033: Release Notes**  
  *Status:* `VERIFIED`  
  *Objective:* Detailed patch notes and feature catalog in `RELEASE_NOTES.md`.
- **TASK-034: Storefront & Distribution Preparation**  
  *Status:* `VERIFIED`  
  *Objective:* Marketing copy and installation guides in `docs/release/itch-io-page.md`.
- **TASK-035: Documentation & Architecture Overhaul**  
  *Status:* `VERIFIED`  
  *Objective:* Overhaul `README.md` with complete ASCII architecture and doc indices.
- **TASK-036: Postmortem & Devlog**  
  *Status:* `VERIFIED`  
  *Objective:* Engineering retrospective in `docs/portfolio/devlog-postmortem.md`.
- **TASK-037: Walkthrough Video Script**  
  *Status:* `VERIFIED`  
  *Objective:* Choreographed 20-minute demonstration script with technical VO cues.
- **TASK-038: Career Portfolio Assets**  
  *Status:* `VERIFIED`  
  *Objective:* STAR-format interview responses and resume bullet points.

---

## 3. Pending & Proposed Tasks (Post-Baseline)

| Task ID | Title | Objective | Status | Dependencies |
| :--- | :--- | :--- | :--- | :--- |
| **TASK-039** | Canonical Documentation Alignment | Consolidate and maintain the 9 canonical documents in `docs/` under Master Prompt v2.0 governance. | `VERIFIED` | Tasks 001–038 |
| **TASK-040** | PlayMode Test Suite Expansion | Author end-to-end PlayMode tests for full scene walkthrough from spawn to core reveal. | `PROPOSED` | TASK-039 |
| **TASK-041** | 3D Visual Art & Asset Replacement | Replace procedural geometric graybox capsules with textured 3D environment assets and characters. | `PROPOSED` | Requires ADR on Art Pipeline |
| **TASK-042** | Custom URP Crystalline Shader Graph | Implement production crystalline growth and refraction shaders for memory reconstruction. | `PROPOSED` | TASK-041 |
