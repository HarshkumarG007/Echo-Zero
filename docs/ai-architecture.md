# docs/ai-architecture.md — ECHO//ZERO

> AI system design across all phases.
> Phase 3 implements Level 0/1 only. This document plans the progression.
> Do not implement above the current phase level without an ADR and a measured baseline.

---

## 1. AI Progression Model

| Level | Description | Phase | Prerequisite |
|-------|-------------|-------|-------------|
| 0 | Deterministic logic (flag lookup, scripted behaviour) | P3 | None |
| 1 | Finite State Machine — reactive, state-aware | P3 | None |
| 2 | Utility AI — weighted scoring over world state | P6 | Level 1 baseline measured |
| 3 | World-state-aware memory + planning | P6 | Level 2 measured |
| 4 | Learned components — ONNX inference via Sentis | P7 | Level 3 baseline, evaluation framework |
| 5 | Hybrid: deterministic planner + learned policy | P8 | Level 4 measured, ML evaluation passed |

**Rule**: Never jump levels. Every level must produce a measurable improvement over the previous.

---

## 2. Phase 3 — Level 0/1 Implementation

### 2.1 The Drift (Level 1 — FSM)

```
States: Patrol → Alerted → Pursuing → Destabilizing → Stabilized

Patrol:
  - Move between 2–3 anchor waypoints in the archive
  - Speed: 1.5 m/s
  - Detection: sphere overlap radius 8m

Alerted:
  - Face player
  - Duration: 1.5 seconds
  - Transition: → Pursuing

Pursuing:
  - Move toward player at 3 m/s
  - If player exits outer radius (12m): → Patrol
  - If player in RECALL range (3m): → Destabilizing

Destabilizing:
  - Publish RecallInterruptedEvent to PlayerController
  - Apply slow push force away from Drift direction
  - If player sustains RECALL for 3 seconds: → Stabilized
  - If player exits RECALL range: → Pursuing

Stabilized (terminal):
  - Stop movement
  - Trigger dissolve animation
  - Publish DriftStabilizedEvent
  - Set NarrativeFlag: drift_stabilized = 1
```

**Implementation**: Plain C# class `DriftFSM`. No Unity NavMesh in Phase 3 — direct transform movement. `DriftController` MonoBehaviour ticks the FSM.

**Test**: `DriftFSMTests.cs` (EditMode) — each state transition unit-tested with NSubstitute mocks.

### 2.2 Mira (Level 0 — Deterministic flag lookup)

```
MiraDialogueSelector.SelectLine(NarrativeState state):
  1. Check which fragments are collected
  2. Check whether contradiction has been seen (2 conflicting fragments collected)
  3. Check whether choice has been made
  4. Return the appropriate DialogueLine from the authored set
```

No randomness. No weighted scoring. Deterministic and testable.

**The wrongness is authored**: Every "wrong" line in Mira's dialogue is a specific, authored string,
not a procedural corruption. The selector picks which authored line to show based on state.

---

## 3. Phase 6 — Level 2/3 Planning

### 3.1 Drift — Utility AI Upgrade

Replace the binary FSM state transitions with a utility scorer over:
- Distance to player
- Time since last RECALL attempt by player
- Number of active memory contradictions in the zone
- Player's fragment collection progress

The Drift "chooses" its state based on weighted utility. A Drift near many contradictions becomes
more agitated (faster, more frequent interrupts). A Drift in a well-reconstructed zone calms.

**This changes the Drift from reactive to world-state-aware.**

Prerequisite: Level 1 FSM baseline must be measured first. What is the encounter completion rate?
Average RECALL attempts before stabilisation? Time-to-stabilise? These become the comparison metrics.

### 3.2 Mira — Weighted Dialogue

Replace deterministic flag lookup with a weighted selection system:
- Each dialogue line has a weight per flag state
- Weight is updated as player collects fragments in different orders
- Mira's "contradictory memory" emerges from which fragments have higher accumulated weight

**This makes Mira's wrongness responsive to play order, not just binary flags.**

---

## 4. Phase 7 — Level 4: Learned Components

**CONDITIONAL**: Only if Phase 6 baseline shows a learnable signal.

### 4.1 Candidate: Drift Navigation (Learned Policy)

- Train a small navigation policy using Unity ML-Agents (or offline IL data)
- Export to ONNX
- Run inference via Unity Sentis (VERIFY_CURRENT_CAPABILITY: confirm Sentis version for Unity 6.3)
- Compare: does the learned policy produce more interesting encounter paths than FSM?

**Evaluation metric**: Path diversity score across 100 encounters (different start positions,
player behaviours). FSM baseline vs. learned policy.

### 4.2 Candidate: Player Behaviour Prediction

- Track fragment collection order across sessions
- Train a lightweight sequence model (LSTM or Transformer, <1M params)
- Predict next fragment the player will seek
- Use prediction to pre-load AerieMemoryScene for that fragment before player arrives

**Evaluation metric**: Pre-load hit rate vs. random pre-load. Latency improvement on fragment RECALL.

### 4.3 Candidate: Adaptive Mira Dialogue Weights

- Use player behaviour data (fragment order, RECALL frequency, time spent near Mira) to
  adjust dialogue weights at runtime
- Compare: does the adaptive system produce more "right" wrongness per player cohort?

**Evaluation metric**: Playtester survey — does Mira feel more unsettling to players who
approached the archive before collecting Mira's fragments?

---

## 5. AI Agent Contract (for any Phase 6+ agent)

Every autonomous agent must have this contract defined before implementation:

```yaml
agent: DriftAgent_v1
observation:
  - player_position: Vector3
  - player_in_recall: bool
  - time_since_last_recall: float
  - contradiction_count_in_zone: int
  - own_position: Vector3
  - own_state: enum (Patrol, Pursuing, Destabilizing, Stabilized)
state:
  - current_fsm_state
  - current_utility_scores
goals:
  - primary: Prevent player from stabilising without effort
  - secondary: Embody incoherence — behaviour should feel unsettled, not mechanical
constraints:
  - Cannot leave archive zone boundary
  - Cannot directly damage player (no health system)
  - Update frequency: max 10 Hz (every 3 frames at 30 fps — not every frame)
action_space:
  - Move(direction: Vector3, speed: float)
  - InterruptRecall()
  - TransitionState(newState: DriftState)
memory: None (Phase 7 — stateless per encounter)
safety:
  - If player exits play area: → Patrol immediately
  - If Stabilized state: stop all actions immediately, no exit
  - Kill switch: DriftController.Disable() stops all FSM ticks
telemetry:
  - state_transitions (with timestamp)
  - recall_interrupt_count
  - time_to_stabilization
evaluation:
  - encounter_completion_rate
  - mean_recall_attempts_to_stabilize
  - path_diversity_score (Phase 7+)
```

---

## 6. AI Safety Constraints (Phase 6+)

No Phase 6+ AI may be enabled without:

- [ ] Bounded action space documented (as above)
- [ ] Kill switch implemented (`DriftController.Disable()`)
- [ ] All state transitions logged to TelemetryService
- [ ] Human-readable debug overlay implemented (Tools layer)
- [ ] Idempotency tested: calling the same action twice does not corrupt state
- [ ] Graceful degradation: if FSM enters unexpected state, → Patrol (safe default)

---

## 7. Evaluation Framework (Phase 8)

Before any "the AI is better" claim can be made:

```
Baseline (Level N)
    ↓
Define metrics (from AI Agent Contract)
    ↓
Collect baseline measurements (≥30 sessions minimum)
    ↓
Implement Level N+1
    ↓
Collect measurements under same conditions
    ↓
Compare: is improvement statistically meaningful?
    ↓
Document in experiment registry (see docs/experiment-protocol.md)
```

**Never claim improvement without a comparison.**
Never compare against a different environment version without noting the environment delta.

---

*Last updated: Phase 2 — Foundation complete.*
*Level 0/1 implementation begins at TASK-009 (DriftFSM).*
