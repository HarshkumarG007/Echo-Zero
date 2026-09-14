# README.md — ECHO//ZERO

<div align="center">

# ECHO//ZERO

*A grieving AI reconstructs reality from memories.*
*The player discovers they are made of the same grief.*

**Unity 6.3 LTS · URP · Phase 3: Build**

</div>

---

## What is this?

**ECHO//ZERO** is a 20–30 minute sci-fi/fantasy mystery vertical slice built in Unity.

You play as **ZERO** — a reconstruction assembled by an AI named **ECHO** who is trying to hold onto someone it lost. Your single ability, **RECALL**, lets you focus on objects and places to make ECHO rebuild its memories of them. The world literally assembles itself around what you choose to remember.

The setting is **The Aerie** — a cliffside research station half-reclaimed by crystalline memory-formations that grow like coral wherever ECHO's reconstruction is active.

By the end of the slice, you understand what ZERO is, what ECHO lost, and why you were built.

---

## Why does this project exist?

Three reasons:

1. **It is a game worth making.** The premise — an AI's grief as the engine of a world — is emotionally specific enough to be memorable and mechanically tractable enough to actually finish.

2. **It is a disciplined engineering demonstration.** The goal is not "AI built a game." The goal is: one developer, using AI-assisted engineering correctly, ships a technically clean vertical slice with tests, ADRs, measured performance, and zero speculative scope.

3. **It is a research platform foundation.** The layered architecture is designed so that AI/ML experimentation can be added in Phase 6–8 without rewriting the game. The evaluation infrastructure comes after the baseline, not before.

---

## Repository Structure

```
Echo/
├── AGENTS.md              ← Agent rules — READ FIRST before touching code
├── ARCHITECTURE.md        ← System architecture
├── DECISIONS.md           ← Architecture Decision Records (ADRs)
├── TASKS.md               ← Active task backlog
├── ROADMAP.md             ← Phase roadmap
├── CODING_STANDARDS.md
├── SECURITY.md
├── TESTING.md
├── config/
│   ├── project.yaml
│   └── antigravity.project.json  ← Machine-readable project manifest
├── docs/
│   ├── game-design.md
│   ├── technical-design.md
│   ├── ai-architecture.md
│   └── performance-budget.md
└── UnityProject/
    └── Assets/
        ├── _Project/Scripts/     ← Layered C# — Core, Gameplay, World, Narrative, AI, Data, UI
        └── Tests/                ← EditMode + PlayMode test assemblies
```

---

## Current Status

| Phase | Name | Status |
|-------|------|--------|
| P0 | Discovery | ✅ Complete |
| P1 | Architecture | ✅ Complete |
| P2 | Foundation | ✅ Complete |
| **P3** | **Build — Core Gameplay** | 🔨 **In Progress** |
| P4 | Vertical Slice | 📋 Planned |
| P5–P11 | Polish → Portfolio | 📋 Planned |

**Active tasks**: TASK-001 through TASK-005 (see [TASKS.md](TASKS.md))

---

## Technology Stack

| Concern | Technology |
|---------|-----------|
| Engine | Unity 6.3 LTS |
| Render Pipeline | URP |
| Input | Unity Input System |
| UI | UI Toolkit |
| Events | Typed EventBus\<T\> (hand-rolled) |
| DI | ServiceLocator (hand-rolled) |
| Testing | Unity Test Framework + NSubstitute |
| Serialization | System.Text.Json (saves) + JsonUtility (config) |
| AI (Phase 3) | FSM (plain C#) — Level 0/1 only |
| Dev tooling | Google Antigravity + Google AI Pro |

---

## For AI Coding Agents

Read [AGENTS.md](AGENTS.md) **before modifying any file**.

The mandatory workflow is:

```
PLAN → TASK → IMPLEMENT → TEST → REVIEW → VERIFY → COMMIT
```

One task at a time. Tests before commits. ADR before any architectural change. No exceptions.

---

## For Hiring Managers

This project demonstrates:

- **Disciplined agentic engineering** — AI agents accelerate implementation; humans own direction and acceptance criteria
- **Architecture-first thinking** — 10 ADRs written before any gameplay code
- **Hardware-aware design** — every budget decision is anchored to a real RTX 4060 laptop
- **Scope control** — a vertical slice that proves the idea without scaling to an impossible universe
- **Testability as a constraint** — no code without a corresponding test class

The engineering decisions are more important than the amount of code.

---

*Phase 3 in progress. See TASKS.md for what is being built right now.*
