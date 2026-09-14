# ROADMAP.md — ECHO//ZERO

> This roadmap is milestone-based, not calendar-based.
> A phase does not begin until the previous phase passes its quality gate.
> Phases 7-8 are conditional — they only begin if Phase 6 baseline is measured and worth improving.

---

## Phase 0 — Discovery ✅

**Goal**: Prove the idea is worth building before building anything.

**Deliverables**:
- Design thesis (one sentence)
- 3 design directions evaluated, one chosen
- Kill-critic pass on the chosen direction
- Vertical slice concept defined (setting, NPC, ability, encounter, reveal)

**Status**: COMPLETE

---

## Phase 1 — Architecture ✅

**Goal**: Design the system before touching Unity.

**Deliverables**:
- ARCHITECTURE.md — layer model, system contracts, data flows
- DECISIONS.md — 10 ADRs covering engine, pipeline, input, UI, DI, events, serialization, scenes, AI level
- Technology stack locked

**Status**: COMPLETE

---

## Phase 2 — Foundation ✅

**Goal**: Establish the repository as an agent-ready workspace.

**Deliverables**:
- AGENTS.md — agent rules, forbidden behaviours, workflow
- TASKS.md — Phase 3 task backlog (5 tasks, dependency-ordered)
- README.md, ROADMAP.md, CODING_STANDARDS.md, SECURITY.md, TESTING.md
- config/antigravity.project.json — machine-readable project manifest
- Folder structure scaffolded

**Status**: COMPLETE

---

## Phase 3 — Build: Core Gameplay 🔨

**Goal**: A Unity project that compiles, where a player can move and use RECALL to collect a fragment.

**Tasks**:
- TASK-001: Unity project initialisation
- TASK-002: EventBus + ServiceLocator (with full test coverage)
- TASK-003: Bootstrap scene + service initialisation
- TASK-004: Player controller + RECALL input
- TASK-005: MemoryFragment ScriptableObject + RECALL integration
- TASK-006: NarrativeState service + flag system [to add]
- TASK-007: ReconstructionAnchor + world-build sequence [to add]
- TASK-008: Mira stub NPC + dialogue display [to add]

**Quality Gate G3**:
- Player moves in Aerie with RECALL working end-to-end
- All EditMode and PlayMode tests pass
- No compile errors or warnings

---

## Phase 4 — Vertical Slice

**Goal**: The full 20–30 minute slice is playable from opening to final reveal.

**Scope**:
- Complete Aerie environment (upper + lower, streaming)
- Mira with contradiction-aware dialogue
- Three corroborating fragments for collapsed walkway puzzle
- The Choice (two contradictory fragments, one validated via RECALL)
- The Drift encounter (FSM, stabilised by RECALL)
- The major reveal sequence
- Save/load working
- Basic UI (HUD, dialogue, pause)
- Audio placeholders

**Quality Gate G4**:
- Slice playable start-to-finish without errors
- Choice has measurable consequence on world state
- Drift can be stabilised
- Save/load round-trip works

---

## Phase 5 — Polish

**Goal**: The slice feels premium, not prototype.

**Scope**:
- Memory-formation shader (crystalline emission, growth animation)
- RECALL visual effect (world assembling from nothing)
- Mira character design and animation
- ECHO voiceover audio
- Environmental audio (Aerie ambience, memory sequences)
- UI polish (UI Toolkit animations, transitions)
- Performance profiling pass (hit all budgets from AGENTS.md §10)
- Settings: resolution, audio levels, keybind rebinding

**Quality Gate G5**:
- 60 fps sustained in Aerie on development machine
- All performance budgets met (see AGENTS.md §10)
- Visual and audio review signed off

---

## Phase 6 — AI

**Goal**: Demonstrate measurable improvement over Phase 3 Level 0/1 AI baseline.

**Scope**:
- Drift: improve from FSM to utility-scored behaviour
- Mira: extend dialogue selection to account for fragment order and contradiction weight
- Measure: task completion, encounter coherence, player feedback

**Quality Gate G6**:
- Baseline (Phase 3) vs Phase 6 AI comparison documented
- Improvement is measurable, not claimed

---

## Phase 7 — ML Experiments (Conditional) ✅

**Status**: COMPLETE (Integrated Sentis, ONNX-based Pursuit Scorer, Adaptive Mira Dialogue)

**Possible experiments**:
- Player behaviour prediction (fragment collection order)
- Drift navigation via learned policy (small model, ONNX inference via Unity Sentis)
- Adaptive Mira dialogue weights via player modelling

**Quality Gate G7**:
- Experiment registry entry for every experiment
- Results compared against Phase 6 baseline
- Compute cost within budget (RTX 4060 local inference or justified cloud)

---

## Phase 8 — Evaluation (Conditional) ✅

**Goal**: Evaluation framework that produces defensible, reproducible results.

**Status**: COMPLETE (Experiment Registry, Reproducibility Checklist, and Benchmark Suite finalized)

**Deliverables**:
- Experiment registry
- Reproducibility checklist
- Benchmark suite
- Evaluation report

---

## Phase 9 — Red Team

**Goal**: The slice is resilient to bad input, save corruption, and edge cases.

**Scope**:
- Save file corruption detection (checksum verification)
- Invalid game states (out-of-order fragment collection)
- Drift edge cases (multiple Drifts, stuck FSM)
- Security audit of telemetry (no PII)
- Input edge cases (rapid RECALL spam, simultaneous inputs)

**Quality Gate G8/G9**:
- All identified vulnerabilities mitigated or accepted with documented risk
- Threat model updated

---

## Phase 10 — Release

**Goal**: A build that can be distributed and run on target hardware without a Unity editor.

**Deliverables**:
- IL2CPP build pipeline configured
- Build tested on clean machine (no Unity Editor)
- Release notes
- Itch.io or equivalent page

---

## Phase 11 — Portfolio

**Goal**: The project communicates engineering decisions, not just shipped code.

**Deliverables**:
- README updated with technical achievements and measured results
- Architecture article / devlog (honest about what worked and what didn't)
- Demo recording script (20-minute walkthrough)
- Resume bullets (skills demonstrated with evidence, not claims)
- Interview talking points

---

*Current phase: P9 — Red Team. See TASKS.md for active work.*
