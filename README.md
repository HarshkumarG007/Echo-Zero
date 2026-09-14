# ECHO//ZERO

> **"You are made of the same grief you are trying to resolve."**

**ECHO//ZERO** is a Unity 6 (URP) sci-fi/fantasy mystery vertical slice that doubles as a technical showcase for **local machine learning integration** and **AI-agentic software engineering**. 

Developed from architecture to final IL2CPP build using [Google Antigravity](https://deepmind.google/technologies/antigravity/) and Google AI Pro, this repository demonstrates how modern autonomous coding agents can adhere to strict architectural constraints (ADRs), maintain performance budgets, and build stable, test-driven gameplay systems.

---

## Technical Highlights

### 1. Local ML Inference (Unity Sentis)
We bypassed cloud API dependency by embedding lightweight ONNX neural networks directly into the game using **Unity Sentis**, maintaining a strict < 1.0ms AI compute budget per frame on local GPU hardware.
- **ML Pursuit AI:** The Drift enemy utilizes a trained model to predict the player's trajectory, replacing rigid A* pathing with dynamic interception.
- **Adaptive Narrative:** Mira, the companion NPC, alters her dialogue weights based on an `AggressionScore` telemetry model derived from the player's playstyle.

### 2. Utility-Based AI
Replaced a traditional Finite State Machine with a **Utility Brain** architecture for enemies. Actions (Patrol, Pursue, Stabilize) are scored dynamically via animation curves evaluating player distance and RECALL intensity, resulting in highly emergent combat encounters.

### 3. Decoupled Architecture
- **Service Locator:** All core systems (Save, Telemetry, Config, Narrative) are decoupled and injected via a Service Locator pattern.
- **Event Bus:** Cross-system communication relies entirely on strongly typed structs passing through a central Event Bus, eliminating `SendMessage` and tightly coupled `MonoBehaviour` references.
- **Save Security:** Checksum-validated (SHA-256) JSON serialization protects narrative progression and gracefully resets corrupted states.

### 4. Agentic Development Workflow
This project was built strictly adhering to `AGENTS.md` rules. Every feature was preceded by an Architectural Decision Record (ADR), developed via Test-Driven Development (NUnit/EditMode), and measured against strict performance gates.

---

## Repository Map
- `AGENTS.md` - The foundational rulebook for AI agents modifying this repository.
- `ARCHITECTURE.md` - High-level system interaction and data flow diagrams.
- `DECISIONS.md` - The architectural decision records (ADRs) logging *why* we built it this way.
- `TASKS.md` - The historical backlog of all 38 executed feature tasks.
- `docs/portfolio/` - Devlogs, demo scripts, and STAR-format interview talking points.

## Building and Running
1. Open the project in **Unity 6.3 LTS (URP)**.
2. Open `Assets/_Project/World/Scenes/Bootstrap.unity`.
3. Press Play, or use the `ECHO//ZERO > Build Windows 64-bit (IL2CPP)` menu item to generate a standalone executable.

---
*Created as a demonstration of Google Antigravity Advanced Agentic Coding.*
