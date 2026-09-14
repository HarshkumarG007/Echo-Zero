# ML Experiment Registry

This document catalogues the Machine Learning experiments integrated during Phase 7 of the ECHO//ZERO project.

## EXP-001: Local ONNX Inference via Unity.InferenceEngine
- **Status:** Integrated (Phase 7)
- **Goal:** Prove that local inference is viable within the project's performance budgets (no cloud dependency, no network latency).
- **Implementation:** `SentisModelRunner.cs` acts as the service wrapper around `Unity.InferenceEngine.Worker`. Loads a `.onnx` `ModelAsset` at runtime and executes via `Worker.Schedule()`.
- **Backend:** `BackendType.GPUCompute` (Compute Shader).

## EXP-002: ML-Driven Drift Pursuit Scorer
- **Status:** Integrated (Phase 7)
- **Goal:** Replace static distance-based pursuit logic with a lightweight predictive model.
- **Implementation:** `MLPursuitScorer.cs` feeds a tensor containing `[Distance, PlayerVelocityX, PlayerVelocityZ]` into `EXP-001`. The model outputs a confidence score for intercepting the player.
- **Fallback:** If the model fails or is missing, the system gracefully falls back to the Phase 6 hardcoded heuristic (Distance < 15m).

## EXP-003: Adaptive Mira Dialogue via Playstyle Classification
- **Status:** Integrated (Phase 7)
- **Goal:** Adjust narrative tone based on player behavior.
- **Implementation:** `TelemetryService` tracks a running ratio of "RECALL uses" vs "Fragments Collected" to compute an `AggressionScore`. `MiraDialogueSelector` uses this score to bias between `DefaultLineAggressive` and `DefaultLineExplorer`.

---

## Deprecated / Rejected Experiments
- *None currently.*
