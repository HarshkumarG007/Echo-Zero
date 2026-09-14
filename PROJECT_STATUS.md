# PROJECT_STATUS.md — ECHO//ZERO

> Live status dashboard. Update after every quality gate or phase transition.
> This file is NOT documentation — it is a ground-truth snapshot.

---

## Current Phase: P3 — Build (Core Gameplay)

| Field | Value |
|-------|-------|
| Phase | P3 — Build |
| Started | 2026-09-14 |
| Target completion | TBD (task-driven, not calendar-driven) |
| Blocking task | TASK-001 (Unity project must exist before anything else) |
| Last updated | 2026-09-14 |

---

## Quality Gate Status

| Gate | Name | Status | Evidence |
|------|------|--------|---------|
| G0 | Idea Validation | ✅ PASS | Blueprint §2–5, kill-critic, premortem complete |
| G1 | Architecture | ✅ PASS | 10 ADRs, ARCHITECTURE.md, layer model |
| G2 | Foundation | ✅ PASS | 14 repo files, antigravity.project.json, TASKS.md |
| G3 | Core Gameplay | ⬜ NOT STARTED | — |
| G4 | Vertical Slice | ⬜ NOT STARTED | — |
| G5 | Performance | ⬜ NOT STARTED | — |
| G6 | AI | ⬜ NOT STARTED | — |
| G7 | ML Evaluation | ➖ NOT APPLICABLE | Conditional on Phase 6 baseline |
| G8 | Security | ⬜ NOT STARTED | — |
| G9 | Safety | ⬜ NOT STARTED | — |
| G10 | Reproducibility | ⬜ NOT STARTED | — |
| G11 | Documentation | ⬜ NOT STARTED | — |
| G12 | Release | ⬜ NOT STARTED | — |

---

## Task Status

| Task | Title | Status | Assigned Role |
|------|-------|--------|--------------|
| TASK-001 | Unity Project Initialisation | ⬜ Not started | UNITY_ENGINEER |
| TASK-002 | EventBus + ServiceLocator | ⬜ Not started | UNITY_ENGINEER |
| TASK-003 | Bootstrap Scene + Services | ⬜ Not started | UNITY_ENGINEER |
| TASK-004 | Player Controller + RECALL Input | ⬜ Not started | GAMEPLAY_ENGINEER |
| TASK-005 | MemoryFragment + IRecallable | ⬜ Not started | GAMEPLAY_ENGINEER |

---

## File Count

| Category | Count |
|----------|-------|
| Documentation files (root) | 9 |
| Config files | 2 |
| Docs files | 5 |
| C# source scripts | 0 (pre-authored, pending Unity project) |
| Unity scenes | 0 |
| ScriptableObject assets | 0 |
| Tests | 0 |

---

## Known Risks (open)

| Risk | Severity | Mitigation | Status |
|------|----------|-----------|--------|
| NSubstitute AOT/IL2CPP compat with Unity 6.3 | Medium | Verify before IL2CPP build (Phase 10) | Open |
| Cinemachine version for Unity 6.3 | Low | Verify on TASK-001 Unity Hub install | Open |
| Crystal shader VRAM cost | Medium | Profile at G5 before commit | Open |
| UI Toolkit data-binding API in 6.3 | Low | Verify in TASK-001 | Open |
| Antigravity 2.0 AGENTS.md vs GEMINI.md precedence | Low | Check current Antigravity docs | Open |

---

## Decisions Log (summary)

10 ADRs recorded. See DECISIONS.md for full text.
Next ADR to write: ADR-0011 when a new architectural decision arises in Phase 3.

---

*Update this file whenever a task completes or a gate changes status.*
