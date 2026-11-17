# Architecture Decision Records (ADRs) — ECHO//ZERO

**Document Version:** 1.0  
**Status:** [CONFIRMED] Active Decision Log (ADR-0001 through ADR-0006)  
**Governance:** Master Prompt v2.0 Architecture Governance

---

## Index of Decisions

- **ADR-0001:** Unity 6.3 LTS with Universal Render Pipeline (URP)
- **ADR-0002:** JSON Save File Format via Newtonsoft/Text.Json
- **ADR-0003:** Five Primary Folders Architecture (No Speculative Layers)
- **ADR-0004:** Unity Sentis for Local ML Inference (Phase 7 Scope)
- **ADR-0005:** Pull Sentis/ML Out of Phase 3 Drift; Use Deterministic FSM/Heuristics
- **ADR-0006:** Save Data Stays Plain JSON; No Encryption or Checksums

---

## ADR-0001: Unity 6.3 LTS with Universal Render Pipeline (URP)

- **Date:** 2026-09-14
- **Status:** `Accepted`
- **Context:** Development occurs on a laptop (RTX 4060 Laptop GPU 8GB VRAM, 16GB RAM, i7-14650HX). The project requires atmospheric, layered lighting for a crystalline sci-fi/fantasy environment while guaranteeing rapid iteration and $\ge 60$ FPS performance.
- **Decision:** Use Unity 6.3 LTS (`6000.6.0f1`) and the Universal Render Pipeline (URP).
- **Justification:** Unity 6.3 LTS offers long-term support through December 2027. URP is lightweight, highly optimized, and leaves ample VRAM headroom for crystalline growth shaders, VFX, and post-processing on mobile/laptop GPUs.
- **Consequences:** Advanced HDRP-exclusive features (volumetric clouds, ray tracing) are unavailable. Visual style must rely on custom URP Shader Graphs.
- **Alternatives Rejected:**
  - *HDRP:* Too memory-intensive for an 8GB VRAM laptop; iteration bottlenecks.
  - *Built-in Render Pipeline:* Deprecated path with no long-term future.
  - *Unity 6.0 LTS:* Support window ended late 2026.

---

## ADR-0002: JSON Save File Format

- **Date:** 2026-09-14
- **Status:** `Accepted`
- **Context:** The game needs to persist player position, found fragment IDs, narrative flags, settings, and the validated choice.
- **Decision:** Store save data in a single plain JSON file per slot using standard serialization.
- **Justification:** The data payload is small ($< 10$ KB). JSON is human-readable, easily debugged in text editors, and supports nested collections.
- **Consequences:** Save files can be hand-edited by the player. Requires defensive loading so malformed JSON falls back gracefully to a new game.
- **Alternatives Rejected:**
  - *ScriptableObjects:* Built for design-time asset storage, not runtime persistence.
  - *SQLite / Relational Database:* Overkill for a local single-player vertical slice.
  - *Binary Serialization:* Opaque, difficult to debug during development.

---

## ADR-0003: Five Primary Folders Architecture

- **Date:** 2026-09-14
- **Status:** `Accepted`
- **Context:** A ten-layer enterprise game architecture was proposed (Presentation, Simulation, Agent, Data, Infrastructure, etc.). For a 20–30 minute vertical slice without multiplayer or cloud servers, many of these layers would be empty scaffolding.
- **Decision:** Organize `Assets/_Project` into cohesive domain folders: `Core/`, `Gameplay/`, `World/`, `Narrative/`, and `UI/`.
- **Justification:** Every folder directly maps to active functionality. Refactoring into further layers can occur organically when a new subsystem genuinely demands it.
- **Consequences:** Clear assembly definitions (`.asmdef`) prevent unwanted circular dependencies across folders.
- **Alternatives Rejected:** Full enterprise 10-layer architecture (premature abstraction).

---

## ADR-0004: Unity Sentis for Local ML Inference (Phase 7 Scope)

- **Date:** 2026-09-14
- **Status:** `Accepted` (Scoped to Phase 7)
- **Context:** For advanced research (Phase 7), we explored running lightweight ONNX neural models to predict player movement vectors and dynamically weight dialogue.
- **Decision:** Integrate Unity Sentis (`com.unity.sentis`) for in-engine ONNX inference.
- **Justification:** Sentis executes inside the Unity process without requiring external Python daemons or network calls.
- **Consequences:** Inference must be profiled rigorously to avoid exceeding the 8.0 ms main-thread CPU budget.
- **Alternatives Rejected:** Python/FastAPI local sidecars; Cloud AI APIs (network latency and operational cost).

---

## ADR-0005: Pull Sentis/ML Out of Phase 3 Drift; Use Deterministic Heuristics

- **Date:** 2026-09-15
- **Status:** `Accepted` (Corrective Decision)
- **Context:** ADR-0004 had reserved Sentis for Phase 7 experimentation. However, live Sentis inference was prematurely wired into the Phase 3 Drift entity. This led to a critical crash: Sentis attempted `GPUCompute` inference on the laptop's integrated Intel graphics, throwing an unhandled `igc64.dll` driver access violation.
- **Decision:** Completely pull Unity Sentis out of the Phase 3 Drift build path. Implement the Drift using deterministic distance-based utility heuristics (`DriftUtilityBrain` with delegate scorers). Preserve `MLPursuitScorer.cs` in the repository solely for Phase 7 research.
- **Justification:** An encounter with a single enemy entity stabilized by player focus does not require an ML model. Deterministic heuristics are 100% stable, easily debugged, and cannot crash the graphics driver.
- **Consequences:** Removed `Unity.InferenceEngine` from `EchoZero.AI.asmdef`. Removed `SentisModelRunner` registration from `Bootstrap.cs`. Pinned Sentis fallback to `BackendType.CPU`.
- **Alternatives Rejected:** Keeping Sentis active in the vertical slice.

---

## ADR-0006: Save Data Stays Plain JSON; No Encryption or Checksums

- **Date:** 2026-09-15
- **Status:** `Accepted` (Corrective Decision)
- **Context:** A SHA-256 checksum mechanism was previously added to `SaveService` to detect file tampering. In practice, this violated ADR-0002. In a single-player offline narrative slice, the player editing their own save file is not a threat. The checksum caused legitimate progress loss when players or developers edited saves for testing.
- **Decision:** Revert `SaveService` to plain JSON without checksum verification or cryptographic hashing.
- **Justification:** Avoids unnecessary complexity (no sidecar checksum files). Keeps save files human-readable and editable.
- **Consequences:** `SaveCorruptionTests.cs` was updated to verify that malformed JSON recovers gracefully, while valid hand-edited JSON loads successfully.
- **Alternatives Rejected:** Retaining cryptographic checksums.
