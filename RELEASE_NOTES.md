# ECHO//ZERO - Vertical Slice Release Notes

## Overview
This is the final standalone build of the ECHO//ZERO vertical slice, concluding Phase 10 of our agent-driven development roadmap. The slice demonstrates a 20-30 minute gameplay loop focusing on memory reconstruction, dynamic narrative choices, and utility-based AI driven by local ML models.

## New Features & Integrations
* **Local ML Inference (Unity Sentis):** Integrated the Unity Sentis package to run ONNX models locally without cloud dependency.
* **ML-Driven Pursuit AI:** The Drift enemy now utilizes a trained model to predict the player's future trajectory based on current velocity, resulting in smarter interception paths.
* **Adaptive Dialogue Weights:** Mira's dialogue selection is now influenced by a local telemetry Aggression Score, biasing her responses to match the player's playstyle.
* **Utility AI System:** Replaced the rigid Drift State Machine with a dynamic Utility Brain, allowing smooth transitions between patrolling, pursuing, and destabilizing behaviors.

## Hardening & Security (Red Team)
* **Save File Security:** Implemented SHA-256 checksum validation for `save_01.json`. Tampered saves are automatically detected and safely rejected to prevent game-breaking states.
* **Telemetry Anonymization:** Conducted a security audit to ensure zero Personally Identifiable Information (PII) is logged. The telemetry payload relies exclusively on anonymous GUID session IDs.
* **Input Rate Limiting:** Applied strict cooldowns to the RECALL ability to prevent macro spamming from artificially skewing ML adaptation metrics.
* **Narrative State Protection:** Hardened the `ChoicePedestal` interaction to gracefully reject out-of-order attempts, preventing progression soft-locks.

## Known Limitations
* **Placeholder Assets:** Some audio and visual effects remain as block-out placeholders (Phase 5 Polish was scoped out for this ML-focused release).
* **Save Slot Limit:** Only one save slot (`save_01`) is currently supported.
* **Performance:** Optimized for RTX 4060 class hardware. Lower-end systems may experience frame drops during Sentis tensor allocation if run on CPU backend.

## Instructions
1. Run `ECHO_ZERO.exe`.
2. Press `P` or `Esc` to pause and access settings/save menu.
3. Hold `Right Mouse Button` to activate RECALL.
