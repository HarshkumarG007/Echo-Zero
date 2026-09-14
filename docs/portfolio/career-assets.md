# Career Assets: ECHO//ZERO

This document translates the ECHO//ZERO vertical slice into concrete, evidence-based bullet points for your resume and STAR-format talking points for technical interviews.

---

## Resume Bullets (Tailored for AI/Gameplay Engineer)

* **Engineered a Utility AI architecture** in Unity 6, replacing traditional Finite State Machines with dynamic action scorers to drive emergent enemy combat behaviors, evaluated via a custom headless benchmarking suite.
* **Integrated local Machine Learning inference** using Unity Sentis, deploying an ONNX model to predict player trajectories for enemy interception while maintaining a strict sub-1ms AI compute budget per frame.
* **Architected a highly decoupled codebase** utilizing the Service Locator and Event Bus patterns, enabling 90% EditMode test coverage for core systems without relying on fragile Unity Scene dependencies.
* **Developed a telemetry-driven adaptive narrative system** that dynamically adjusts NPC dialogue weights based on an anonymous playstyle `AggressionScore` processed securely without PII leakage.
* **Directed an Agentic Development Workflow** (Google Antigravity), managing autonomous AI coding agents by enforcing strict Architectural Decision Records (ADRs) and test-driven development (TDD) pipelines from prototype to IL2CPP release.

---

## Interview Talking Points (STAR Format)

### 1. Integrating Machine Learning into Real-Time Gameplay
**Situation:** We needed the Drift enemy to anticipate player movement rather than just pathfind toward their current position, but cloud-based AI APIs introduced unacceptable latency for an action game.
**Task:** Integrate a predictive model locally without violating our strict performance budget.
**Action:** I configured Unity Sentis to run a quantized ONNX model directly on the local hardware. I wrote an `MLPursuitScorer` that fed the player's recent velocity vector into the model to predict an interception point, heavily optimizing tensor allocations to prevent garbage collection spikes.
**Result:** The AI successfully intercepted the player dynamically, and benchmarking proved the inference ran in under 1ms per frame, ensuring a stable 60 FPS target.

### 2. Defending Architectural Scalability
**Situation:** As the project grew in complexity (save systems, telemetry, UI, AI), Unity `MonoBehaviour` scripts were at risk of becoming a tightly coupled spaghetti mess via `GetComponent` and singletons.
**Task:** Enforce a strict decoupling of systems so they could be developed and tested in isolation.
**Action:** I authored ADR-0004 and ADR-0005, enforcing a strict Service Locator and Event Bus pattern. I mandated that all cross-system communication rely on strongly typed structs (e.g., `ChoiceMadeEvent`) rather than direct method calls.
**Result:** This allowed us to write EditMode unit tests for narrative logic, save serialization, and utility AI without ever needing to load a Unity Scene, vastly accelerating the development velocity and completely eliminating `NullReferenceExceptions` caused by missing scene objects.

### 3. Securing Game State
**Situation:** The narrative of the game relies on a pristine state of memory fragments. If a player or a bug tampered with the JSON save file, it could cause soft-locks or crash the `FragmentRegistry`.
**Task:** Ensure the game can detect and recover from corrupted save files.
**Action:** I implemented a robust `SaveService` that hashes the JSON payload using SHA-256 and writes it to a `.checksum` file. On load, the system re-hashes the JSON and compares it.
**Result:** Any manual tampering or file corruption is instantly detected, blocking the load and gracefully defaulting to a clean "New Game" state, which we proved through automated corruption tests.
