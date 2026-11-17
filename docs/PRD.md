# Product Requirements Document (PRD) — ECHO//ZERO

**Document Version:** 1.0  
**Status:** [CONFIRMED] Baseline Documented  
**Author:** AI Technical Lead & Senior Software Architect  
**Project:** ECHO//ZERO (Vertical Slice)

---

## 1. Product Overview & Vision

**ECHO//ZERO** is a third-person narrative sci-fi/fantasy vertical slice built in Unity 6 LTS. The player controls **ZERO**, an entity navigating **The Aerie**—a cliffside research facility suspended in an incomplete state of reconstruction by **ECHO**, a grieving artificial intelligence attempting to piece together the memory of a lost companion named **Mira**.

### Core Thesis
> *"RECALL is grief made mechanical, and ZERO is what grief builds when it cannot let go."*

The vertical slice functions as a 20–30 minute playable experience demonstrating disciplined, testable software engineering, deterministic narrative mechanics, and decoupled game architecture.

---

## 2. Target Audience & Personas

### 2.1 Target Audience
- Narrative-first adventure game players who value emotional coherence, environmental storytelling, and thematic gameplay over reflex-heavy combat.
- Engineering hiring managers and technical recruiters evaluating portfolio-grade software engineering, decoupled architecture, and automated testing in game development.

### 2.2 User Personas
1. **The Narrative Explorer (Player):** Wants an atmospheric, immersive exploration experience where choices feel earned, interactions are deliberate, and the story reveals its emotional core through player action.
2. **The Senior Software Reviewer (Evaluator):** Wants to inspect a clean, professional codebase with strong separation of concerns, test coverage, explicit architectural boundaries, and reproducible builds.

---

## 3. Scope: Minimum Viable Product (MVP) Slice

### 3.1 In-Scope (Phase 3–11 Vertical Slice)
- **Playable Character (ZERO):** Third-person deliberate locomotion (walk only; no run, no jump).
- **Primary Verb (RECALL):** Raycast-based temporal/memory activation with three discrete context targets:
  1. *Memory Fragments:* Triggers narrative audio/text and updates the internal world state.
  2. *Reconstruction Anchors:* Progressively rebuilds physical structures (the collapsed walkway) upon corroboration.
  3. *The Drift:* Sustained focus stabilizes a hostile, destabilized memory anomaly.
- **Narrative Contradiction & The Choice:** Two mutually contradictory memory fragments regarding Mira's departure. Validating one permanently locks the narrative outcome and dynamically shifts subsequent dialogue.
- **NPC (Mira):** A deterministic, flag-driven reconstructed NPC whose dialogue subtly alters based on player choices and fragment sequence.
- **The Core Reveal:** A climactic discovery sequence in the core chamber where ZERO's true identity and origin are unveiled.
- **Persistence:** Local plain-JSON save/load state.
- **UI & HUD:** UI Toolkit-based reticle, dialogue display box, and pause/settings menu.
- **Performance:** 60 FPS sustained on target hardware (RTX 4060 Laptop, 16 GB RAM).

### 3.2 Out-of-Scope (Explicitly Excluded)
- Inventory management systems.
- Health, damage models, combat mechanics, and death screens.
- Multiple player abilities or branching skill trees.
- Voiced player character (ZERO has no spoken lines).
- Pre-rendered cutscenes (all sequences happen in-engine).
- Multiplayer, networking, and cloud persistence.
- External analytics or third-party web tracking.

---

## 4. Functional Requirements

### 4.1 Locomotion & Controls
- **FR-01 (Movement):** The player must be able to move along horizontal axes with smooth acceleration and collision detection via `CharacterController`.
- **FR-02 (Camera):** Third-person follow camera with collision dampening.
- **FR-03 (Input Mapping):** All player controls must route through the Unity Input System (`com.unity.inputsystem`), supporting standard keyboard/mouse and gamepad.

### 4.2 RECALL Mechanics
- **FR-04 (Targeting):** When the player presses RECALL, a raycast from camera center checks for objects implementing `IRecallable`.
- **FR-05 (Cooldown / Rate Limiting):** RECALL must enforce an input cooldown timer (`GameConfig.RecallCooldownSeconds`) to prevent raycast spamming.
- **FR-06 (Fragment Registration):** Successful RECALL on a `MemoryFragmentPickup` must fire `FragmentCollectedEvent`, register the fragment in `FragmentRegistry`, and update world flags in `NarrativeState`.
- **FR-07 (Reconstruction Anchor):** RECALL targeting a `ReconstructionAnchor` must evaluate whether required corroborating fragments are present, trigger mesh/visual reconstruction, and publish `AnchorRebuiltEvent`.
- **FR-08 (Drift Interaction):** Sustaining RECALL on the Drift entity for 3 consecutive seconds must transition it into the `Stabilized` state and publish `DriftStabilizedEvent`.

### 4.3 Narrative System
- **FR-09 (Contradiction Detection):** When two contradicting fragments are collected, `FragmentRegistry` must flag an active `IsPlayerMustChooseState`.
- **FR-10 (Choice Validation):** Interacting with a `ChoicePedestal` validates one fragment, permanently invalidates the conflicting counterpart, and publishes `ChoiceMadeEvent`.
- **FR-11 (Deterministic Dialogue):** `MiraDialogueSelector` must evaluate active narrative flags and return the corresponding authored line without random variance.
- **FR-12 (Reveal Sequence):** Entering the core trigger volume must fire `RevealTriggeredEvent`, disable the player HUD, and trigger the final narrative exposition.

### 4.4 Systems & Persistence
- **FR-13 (Save State):** `SaveService` must serialize player coordinates, collected fragment IDs, validated choice ID, and active narrative flags into a readable JSON file.
- **FR-14 (Safe Load):** Malformed, missing, or corrupted save files must safely fail and return a clean default game state rather than throwing an unhandled exception.
- **FR-15 (Telemetry Baseline):** Key in-game events (`DriftStabilized`, `ChoiceMade`, `RecallFired`) must be logged locally to an anonymous JSON file without any user-identifying data.

---

## 5. Non-Functional Requirements

### 5.1 Performance & Resource Budget
- **NFR-01 (Framerate):** Maintain a stable 60 FPS (frame time $\le 16.6$ ms) on an RTX 4060 Laptop (8 GB VRAM, 16 GB RAM).
- **NFR-02 (Main Thread CPU):** CPU main-thread execution time must not exceed 8.0 ms per frame.
- **NFR-03 (VRAM & Memory):** Total VRAM consumption under 5.0 GB; total system RAM under 10.0 GB.
- **NFR-04 (Draw Calls):** Draw calls must stay $\le 150$ per frame through effective batching and occluded culling.

### 5.2 Architecture & Code Quality
- **NFR-05 (Decoupling):** Zero usage of `FindObjectOfType`, `SendMessage`, or global singletons. Cross-system communication must occur exclusively via `EventBus<T>` or `ServiceLocator`.
- **NFR-06 (Thin MonoBehaviours):** Game logic must reside in plain C# classes; MonoBehaviours are restricted to view/controller bridging.
- **NFR-07 (Modularity):** Independent C# assemblies (`.asmdef`) for `Core`, `Data`, `Gameplay`, `Narrative`, `AI`, `UI`, and `World`.

### 5.3 Privacy & Security
- **NFR-08 (Privacy):** Absolute zero collection of Personally Identifiable Information (PII) in telemetry or logs.
- **NFR-09 (Offline Isolation):** Zero external HTTP/network calls initiated at runtime.

---

## 6. Success Metrics & Verification Criteria

1. **Test Pass Rate:** 100% pass rate across all automated EditMode and PlayMode test suites.
2. **Deterministic Playthrough:** A full playthrough from spawn to reveal completes without scene errors or unhandled exceptions.
3. **Choice Consequence:** Replaying the slice with an alternate memory choice produces a measurably different narrative state and dialogue response.
