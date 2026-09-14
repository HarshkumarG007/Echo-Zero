# Reproducibility Checklist

To evaluate or reproduce the ML experiments in ECHO//ZERO, follow these steps.

## Environment Setup
- [ ] Ensure Unity 6000.6+ is installed.
- [ ] Verify the active rendering pipeline is URP.
- [ ] Ensure `com.unity.ai.inference` (v2.2+) is present in `Packages/manifest.json`.

## EXP-001 / EXP-002: ML Pursuit Scorer
1. Place a `.onnx` model asset in the project (e.g., `Assets/_Project/Data/ML/DriftModel.onnx`).
2. Open the Bootstrap or active scene and locate the `SentisModelRunner` initialisation in the `ServiceLocator`.
3. Ensure the model asset is assigned.
4. Enter Play Mode.
5. Move the player. The Drift AI should transition to `Pursuing` state if the model's confidence threshold is met.
6. **Fallback Test:** Remove the `.onnx` asset from the inspector and enter Play Mode. Verify the AI still pursues using distance-based heuristics without throwing exceptions.

## EXP-003: Adaptive Dialogue
1. Enter Play Mode.
2. Rapidly spam the RECALL button without collecting any fragments (drives Aggression Score > 0.7).
3. Interact with Mira. Verify she outputs the `DefaultLineAggressive`.
4. Restart Play Mode. Collect fragments slowly without unnecessary RECALLs (drives Aggression Score < 0.3).
5. Interact with Mira. Verify she outputs the `DefaultLineExplorer`.

## Automated Validation
- Run the EditMode test suite (`Window > General > Test Runner`).
- Ensure `MLPursuitScorerTests` and `MiraDialogueSelectorTests` pass successfully.
