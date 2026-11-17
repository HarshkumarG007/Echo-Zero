# Development Standards & Engineering Rules — ECHO//ZERO

**Document Version:** 1.0  
**Status:** [CONFIRMED] Active Engineering Policy  
**Purpose:** Unify repository engineering standards, coding conventions, and human-centred AI development protocols.

---

## 1. Master Operating Principles (Non-Negotiable)

### 1.1 Evidence Over Assumptions
Never present an assumption, proposal, or inference as an established fact. Every factual claim must use explicit classifications:
- **[CONFIRMED]:** Explicitly verified by the operator or confirmed in the repository.
- **[OBSERVED]:** Directly observed through command output, tests, or file inspection.
- **[INFERRED]:** Reasonably derived from available data but not independently confirmed.
- **[ASSUMPTION]:** Temporarily assumed for planning purposes.
- **[PROPOSED]:** A recommendation requiring human consideration or approval.
- **[UNKNOWN]:** Missing or unestablished information.
- **[NOT APPLICABLE]:** Explicitly justified as outside scope.

### 1.2 Repository Reality is Authoritative
Inspect the repository before making claims. Code, tests, manifests, and runtime behavior override speculative documentation.

### 1.3 One Task at a Time
Execute exactly one clearly defined task at a time. Never bundle unrelated changes, expand scope without permission, or automatically jump to the next task.

### 1.4 Human Ownership & Understanding
Every significant architectural choice, algorithm, or pattern must be explained. Track human vs. AI contributions explicitly. Never falsely attribute AI code as human-written.

### 1.5 Verification Over Confidence
Never mark a task complete simply because code compiled. Concrete evidence of expected runtime or test behavior is mandatory.

---

## 2. Forbidden Behaviors

An AI engineering collaborator MUST NOT:
- Execute arbitrary shell commands outside `git`, `dotnet`, standard editor scripts, or authorized batch processes.
- Modify binary Unity scene files (`.unity`) directly by hand—use programmatic editor scripts (`GameAssembler`, `MissingPiecesPatcher`).
- Modify `ProjectSettings/` without an explicit task authorizing it.
- Add third-party packages or dependencies without a dedicated ADR in `docs/decisions.md`.
- Rewrite shared Git history (`--force`, `--amend` on pushed commits).
- Create speculative abstractions—only build what the active task requires.
- Generate business logic code without corresponding EditMode or PlayMode tests.
- Commit secrets, tokens, system environment variables, or machine-local absolute paths.
- Silently modify more than one architectural layer in a single commit.
- Skip the research/plan phases and jump straight into code generation.

---

## 3. C# Coding Standards

### 3.1 Namespaces
Every file must declare an explicit namespace matching the folder layer:
```csharp
// Pattern: EchoZero.<Layer>.<Feature>
namespace EchoZero.Core.Events { }
namespace EchoZero.Gameplay.Recall { }
namespace EchoZero.Narrative.Fragments { }
namespace EchoZero.AI.Drift { }
namespace EchoZero.Data.Save { }
namespace EchoZero.UI { }
```

### 3.2 MonoBehaviour Rules
- **MonoBehaviours are thin controllers.** Business logic belongs in pure, testable C# classes.
- **No public fields.** Use `[SerializeField] private` with backing properties where external read access is needed.
- **No `FindObjectOfType`, `FindWithTag`, or `SendMessage`.** Query `ServiceLocator` or listen to `EventBus<T>`.

### 3.3 Method Length & Composition
- **Method Length Cap:** Approximately 30 lines. Methods exceeding this must extract cohesive private helpers.
- **Composition Over Inheritance:** Prefer interfaces (`IRecallable`, `ISaveService`) and component composition over deep class hierarchies.

### 3.4 API Documentation
- XML summary documentation (`/// <summary>`) is required on all public and internal interfaces, structs, and service methods.

---

## 4. Git Conventions & Commit Standards

### 4.1 Commit Message Format
```text
<type>(<scope>): <short description>

[optional detailed body]

Task: TASK-XXX
```

- **Allowed Types:** `feat`, `fix`, `refactor`, `test`, `docs`, `perf`, `security`, `chore`.
- **Scope:** Architectural layer (`core`, `gameplay`, `narrative`, `ai`, `data`, `ui`, `tools`, `world`).
- **Traceability:** Every commit must reference an authorized `Task: TASK-XXX`.

---

## 5. Change Management & Quality Gates

Before any task is considered `VERIFIED`:
1. **Gate A (Correctness):** Acceptance criteria verified by unit tests or reproducible manual test steps.
2. **Gate B (Quality):** Compiles with zero errors and zero new warnings. Adheres to namespace and method length caps.
3. **Gate C (Testing):** All relevant EditMode/PlayMode tests pass.
4. **Gate D (Security):** No PII in telemetry, no hardcoded secrets, defensive error handling on persistence.
5. **Gate E (Documentation):** Canonical documents updated (`task.md`, `memory.md`, and others as relevant).
6. **Gate F (Human Understanding):** Human operator has reviewed the technical explanations and verified the results.
