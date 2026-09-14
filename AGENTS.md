# AGENTS.md — ECHO//ZERO

> **Every AI coding agent MUST read this file before modifying any file in this repository.**
> This document is the single authoritative source of rules for agentic development.

---

## 1. Project Purpose

**ECHO//ZERO** is a Unity-based sci-fi/fantasy mystery game in which a grieving AI named **ECHO** reconstructs reality from memories, and the player character **ZERO** — also reconstructed — gradually discovers they are made of the same grief they are trying to resolve.

This project simultaneously serves as:
- A genuinely playable, emotionally coherent vertical slice (20–30 min)
- A technically disciplined Unity engineering project
- A future AI/ML experimentation platform
- A portfolio-grade demonstration of agentic software engineering using Google Antigravity + Google AI Pro

**The target right now is Phase 3: Build — implementing the Aerie vertical slice.**

---

## 2. Repository Structure

```
Echo/
├── AGENTS.md                  ← YOU ARE HERE
├── ARCHITECTURE.md
├── DECISIONS.md
├── TASKS.md
├── ROADMAP.md
├── README.md
├── CODING_STANDARDS.md
├── SECURITY.md
├── TESTING.md
├── config/
│   ├── project.yaml
│   └── antigravity.project.json
├── docs/
│   ├── game-design.md
│   ├── technical-design.md
│   ├── ai-architecture.md
│   └── performance-budget.md
└── UnityProject/
    └── Assets/
        ├── _Project/
        │   └── Scripts/
        │       ├── Core/
        │       ├── Gameplay/
        │       ├── World/
        │       ├── Narrative/
        │       ├── AI/
        │       ├── Data/
        │       ├── UI/
        │       └── Tools/
        └── Tests/
            ├── EditMode/
            └── PlayMode/
```

---

## 3. Agent Roles

| Role | Owns | May NOT |
|------|------|---------|
| ARCHITECT | System design, ADRs | Write gameplay code |
| UNITY_ENGINEER | Unity project setup, packages, build | Design new systems without an ADR |
| GAMEPLAY_ENGINEER | Player, RECALL, interaction, Drift encounter | Touch Core/ without architect approval |
| AI_ENGINEER | Drift behaviour, Mira fragment logic | Add ML deps without ML_ENGINEER sign-off |
| QA_ENGINEER | Tests, acceptance criteria | Modify production code directly |
| PERFORMANCE_ENGINEER | Profiling, budgets | Change architecture without a new ADR |
| DOCUMENTATION_ENGINEER | All .md files, code comments | Change production code |
| REVIEWER | Code review before integration | Approve their own work |

No agent silently overrides another's domain.

---

## 4. Mandatory Workflow

Every meaningful change MUST follow:

PLAN → TASK → IMPLEMENT → TEST → REVIEW → VERIFY → COMMIT

Before starting any task:
1. Read the task definition in TASKS.md (use TASK_ID)
2. Read the relevant ADR(s) in DECISIONS.md
3. Read the affected section of ARCHITECTURE.md
4. State which files you expect to change and why

During implementation:
- Work on ONE task at a time. Never combine unrelated changes.
- If a decision arises not covered by an ADR, stop and write a new ADR first.
- Compile and run tests after every meaningful change — not at the end.

Before committing:
- Verify acceptance criteria in the task definition are met
- Confirm no unrelated files were changed
- Confirm all tests pass
- Update TASKS.md status

---

## 5. Forbidden Behaviours

An AI coding agent MUST NOT:
- Execute arbitrary shell commands outside npm, git, dotnet, or approved Unity batch-mode
- Modify ProjectSettings/ without a specific task authorizing it
- Add Unity packages without an ADR entry
- Rewrite Git history (--force, --amend on pushed commits)
- Create speculative abstractions — only build what the current task requires
- Generate code without corresponding tests
- Silently rename or delete existing files
- Commit secrets, machine-local paths, or API keys
- Modify more than one architectural layer in a single commit
- Skip the PLAN step and go straight to code

---

## 6. Unity Rules

- Engine: Unity 6.3 LTS (ADR-0001)
- Pipeline: URP (ADR-0002)
- No ECS/DOTS unless justified by profiler result and new ADR
- No Burst/Jobs in Phase 3 — profile first, optimize later
- ScriptableObjects for all static configuration data
- Events via typed EventBus<T> — no UnityEvent in code, no SendMessage
- UI: UI Toolkit only (no uGUI)
- Input: Input System package only (no legacy Input class)
- Scene management: Additive scene loading only
- No FindObjectOfType in production code
- MonoBehaviours are thin controllers only — business logic in plain C# classes

---

## 7. Coding Standards (summary)

- Namespaces: EchoZero.<Layer>.<Feature>
- No public fields on MonoBehaviours — use [SerializeField] private
- Prefer composition over inheritance
- Interfaces for anything that will be tested or swapped
- Max method length: ~30 lines
- Meaningful names — no unexplained abbreviations
- XML summary docs on all public and internal APIs
- No TODO without a linked TASK_ID

---

## 8. Commit Format

<type>(<scope>): <short description>

[optional body]

Task: TASK-XXX

Types: feat, fix, refactor, test, docs, perf, security, research, chore

---

## 9. Testing Rules

- Every new class with business logic gets a corresponding EditMode test class
- PlayMode tests for anything involving Unity lifecycle
- Test class name: <ClassName>Tests
- Use NSubstitute for mocks
- No test may depend on scene state without explicit setup/teardown

---

## 10. Performance Budgets (RTX 4060 Laptop / 16 GB RAM / 8 GB VRAM)

| Metric | Budget |
|--------|--------|
| Frame time | ≤ 16.6 ms (60 fps target) |
| CPU main thread / frame | ≤ 8 ms |
| VRAM | ≤ 5 GB |
| RAM | ≤ 10 GB |
| Draw calls / frame | ≤ 150 |
| Triangles / frame | ≤ 800 K |
| Max texture size | 2048 × 2048 |

---

## 11. Security Rules

- No hardcoded credentials, tokens, or API keys
- No telemetry payload may include PII
- Save files are checksummed — tampering must be detectable
- No agent may invoke external network calls from Unity runtime without an ADR

---

## 12. Definition of Done

A task is DONE only when ALL of the following are true:
- [ ] Implementation exists and compiles cleanly (zero errors, zero new warnings)
- [ ] All acceptance criteria in the task definition are met
- [ ] Tests pass (EditMode and/or PlayMode as required)
- [ ] No unrelated files were modified
- [ ] TASKS.md status updated to [x]
- [ ] Commit message follows the format in §8
- [ ] If a new architectural decision was made, an ADR exists in DECISIONS.md
- [ ] Performance impact is understood

---

## 13. How to Add a Feature

1. Check TASKS.md — does a task already exist?
2. If not, append a new task using the template at the bottom of TASKS.md
3. Write or update the relevant ADR in DECISIONS.md
4. Follow §4 mandatory workflow
5. Reference the task in every commit

---

*Last updated: Phase 2 — Foundation complete. Phase 3: Build in progress.*
