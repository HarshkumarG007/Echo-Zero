# Tasks — ECHO//ZERO Vertical Slice

First five tasks only, in dependency order, small enough for one agent session each. More get appended here as these land, the same way `DECISIONS.md` grows — as needed, not all at once. Each task assumes `AGENTS.md` and `ARCHITECTURE.md` have already been read.

---

### [x] TASK-001: Unity project scaffold

**Objective:** Stand up the Unity 6.3 LTS / URP project with the five-folder structure from ADR-0003, an empty Aerie scene, and a git repo with `AGENTS.md` / `ARCHITECTURE.md` / `DECISIONS.md` at the root.

**Files:** New project; `Assets/_Project/{Core,Gameplay,World,Narrative,UI}/`; `Assets/_Project/World/Scenes/Aerie.unity` (empty); `.gitignore` (Unity-appropriate); root `AGENTS.md`, `ARCHITECTURE.md`, `DECISIONS.md`.

**Dependencies:** None.

**Acceptance criteria:** Project opens in Unity 6.3 LTS with URP active; folder structure matches ADR-0003 exactly; empty Aerie scene loads with no console errors; git log shows one clean initial commit.

**Test:** No logic yet — the acceptance criteria above are the verification.

---

### [x] TASK-002: Save/load round trip

**Objective:** Implement the JSON save/load path from ADR-0002 with a minimal `SaveData` class (player position, empty fragment list, empty settings) before there's real gameplay data, so the save path is tested from day one.

**Files:** `Core/Save/SaveData.cs`, `Core/Save/SaveManager.cs`, `Core/Save/Tests/SaveLoadTests.cs`.

**Dependencies:** TASK-001.

**Acceptance criteria:** Saving then loading returns an identical `SaveData` object. A corrupted or hand-edited save file fails gracefully — logs a warning, falls back to a new save — instead of throwing.

**Test:** EditMode test — serialize a `SaveData` instance, deserialize it, assert equality. Separately, assert a malformed JSON string doesn't throw uncaught.

---

### [x] TASK-003: Player controller and Aerie gray-box

**Objective:** Basic movement and camera in a gray-boxed (untextured) version of the Aerie's first room — enough to walk the critical path of the slice end to end once other systems land.

**Files:** `Gameplay/Player/PlayerController.cs`, `Gameplay/Player/PlayerCamera.cs`, `World/Scenes/Aerie.unity` (gray-box geometry).

**Dependencies:** TASK-001.

**Acceptance criteria:** Player can walk, look around, and not fall through geometry, at a stable frame time on the target laptop. No PC-only input assumptions — use the Input System, not hardcoded `KeyCode`.

**Test:** PlayMode test — spawn the player, apply movement input for N frames, assert position changed in the expected direction and the player stayed within the gray-box bounds.

---

### [x] TASK-004: RECALL skeleton

**Objective:** The core verb, data-flow only — no visuals yet. Player targets an object, RECALL queries World State for a fragment, fires a stub event either way. This is the piece everything else attaches to.

**Files:** `Gameplay/Recall/RecallController.cs`, `Core/WorldState/WorldState.cs`, `Core/WorldState/Tests/WorldStateTests.cs`.

**Dependencies:** TASK-002, TASK-003.

**Acceptance criteria:** Targeting a tagged object with a registered fragment fires `OnFragmentFound` with the correct fragment ID; targeting anything else fires `OnNothingFound`. No visual or audio work in this task.

**Test:** EditMode test on `WorldState` directly — register a fragment, query it, assert the correct result. Doesn't need the scene at all.

---

### [x] TASK-005: Fragment data model and contradiction check

**Objective:** ScriptableObject-based fragment definitions, plus the logic that detects when two found fragments contradict each other — the piece the puzzle, the meaningful choice, and Mira's dialogue all depend on.

**Files:** `Narrative/Fragments/FragmentData.cs` (ScriptableObject), `Narrative/Fragments/FragmentRegistry.cs`, `Narrative/Fragments/Tests/ContradictionTests.cs`.

**Dependencies:** TASK-004.

**Acceptance criteria:** Two fragments flagged as contradicting each other correctly trigger the "player must choose" state; non-contradicting fragments don't; validating one fragment in a contradicting pair correctly invalidates the other.

**Test:** EditMode tests — the most logic-heavy, most testable piece in the whole slice. Aim for full coverage of contradiction resolution without touching the scene.

---

### [x] TASK-006: NarrativeState service + flag system

**Objective:** Finalize and test the `NarrativeState` service. It was scaffolded earlier, but needs rigorous EditMode testing to ensure flags are set, read, and saved correctly, and that `NarrativeFlagSetEvent` fires properly.

**Files:** `Narrative/NarrativeState.cs`, `Narrative/Tests/NarrativeStateTests.cs`.

**Dependencies:** TASK-002.

**Acceptance criteria:** Setting a flag publishes the correct event; querying a flag returns the expected value; the state serializes and deserializes flawlessly to `NarrativeStateData`.

**Test:** EditMode tests — assert flag setting, event publishing, and serialization round-trips.

---

### [x] TASK-007: ReconstructionAnchor + world-build sequence

**Objective:** Implement the `ReconstructionAnchor` (the second `IRecallable` target type). Anchors listen for specific corroborating fragments. When all required fragments are collected, the anchor visually "rebuilds" and fires an `AnchorRebuiltEvent`.

**Files:** `World/ReconstructionAnchor.cs`, `World/ReconstructionAnchorSO.cs` (data definition), `World/Tests/ReconstructionAnchorTests.cs`.

**Dependencies:** TASK-004, TASK-005.

**Acceptance criteria:** Targeting an anchor with RECALL checks if required fragments are in `FragmentRegistry`. If yes, it completes the build sequence and fires `AnchorRebuiltEvent`. If no, it provides negative feedback.

**Test:** EditMode tests — mock `FragmentRegistry` state, trigger `OnRecall()`, assert correct event firing based on fragment presence.

---

### [x] TASK-008: Mira stub NPC + dialogue display

**Objective:** Create the deterministic dialogue selector for Mira. She should output a specific dialogue line based on the current `NarrativeState` flags (e.g., if a fragment is collected, she says line A; if a contradiction is active, she says line B).

**Files:** `Narrative/Mira/MiraDialogueSelector.cs`, `Narrative/Mira/Tests/MiraDialogueTests.cs`.

**Dependencies:** TASK-006.

**Acceptance criteria:** Given a specific set of active narrative flags, the selector deterministically returns the correct authored dialogue string.

**Test:** EditMode tests — setup various `NarrativeState` combinations and assert the correct dialogue string is returned.

---

### [x] TASK-009: Basic UI (HUD & Dialogue Display)

**Objective:** Implement the fundamental HUD using Unity's UI Toolkit (per ADR). This includes a simple reticle for RECALL targeting and a dialogue box that displays Mira's deterministic dialogue text.

**Files:** `UI/GameplayUI.cs`, `UI/Tests/GameplayUITests.cs`

**Dependencies:** TASK-008.

**Acceptance criteria:** The HUD renders a central reticle. The Dialogue UI can be toggled on and off via script and correctly displays text strings queried from `MiraDialogueSelector`.

**Test:** EditMode tests confirming the UI logic works without scene overhead.

---

### [x] TASK-010: The Drift Encounter (FSM)

**Objective:** Build the Drift AI controller using a finite state machine (Patrol -> Alerted -> Pursuing -> Destabilizing -> Stabilized). The Drift must implement `IRecallable` so that the player can stabilize it via RECALL.

**Files:** `AI/Drift/DriftController.cs`, `AI/Drift/DriftStateMachine.cs`, `AI/Drift/Tests/DriftStateMachineTests.cs`.

**Dependencies:** TASK-004.

**Acceptance criteria:** The Drift transitions states accurately based on player distance. Sustaining RECALL on it triggers the `Stabilized` state, firing a `DriftStabilizedEvent` and rendering it harmless.

**Test:** EditMode unit tests confirming state transitions and `IRecallable` interactions without needing physics/scene overhead.

---

### [x] TASK-011: The Choice Interaction

**Objective:** Build the physical world object that represents a contradictory memory choice. When targeted by RECALL, it must call `ValidateFragment` on the `FragmentRegistry` and permanently lock the narrative outcome.

**Files:** `Narrative/ChoicePedestal.cs`, `Narrative/Tests/ChoicePedestalTests.cs`.

**Dependencies:** TASK-005.

**Acceptance criteria:** RECALLing the pedestal successfully validates the assigned fragment in `FragmentRegistry`, publishes `ChoiceMadeEvent`, and disables the alternate pedestal.

**Test:** EditMode tests verifying `OnRecall()` interacts correctly with the `FragmentRegistry` in a choice state.

---

### [x] TASK-012: Aerie Scene Streaming

**Objective:** Implement additive scene loading to divide the Aerie into `Upper` and `Lower` environments (per ADR). Streaming is narratively justified (ECHO only remembers what's nearby) and technically required.

**Files:** `Core/Scenes/SceneStreamer.cs`, `Core/Scenes/Tests/SceneStreamerTests.cs`.

**Dependencies:** None.

**Acceptance criteria:** A `SceneStreamer` MonoBehaviour detects the player entering a trigger volume and asynchronously loads a specified scene additively, unloading the previous one when leaving a buffer zone.

**Test:** EditMode tests mocking `SceneManager` async operations or state changes.

---

### [x] TASK-013: Pause Menu & State Management

**Objective:** Implement the pause state (`Time.timeScale = 0`) and the Pause Menu UI. This menu must allow the player to trigger `SaveService.SaveGame()` and resume.

**Files:** `UI/PauseMenuUI.cs`, `Core/GameStateManager.cs`, `UI/Tests/PauseMenuTests.cs`.

**Dependencies:** TASK-002 (SaveSystem), TASK-009 (UI).

**Acceptance criteria:** Hitting the Pause input stops time and displays the pause UI. The UI offers "Resume" and "Save Game" options.

**Test:** EditMode tests verifying state toggles and correct `SaveService` invocation.

---

### [x] TASK-014: The Reveal Sequence Manager

**Objective:** Build the manager that handles the final narrative reveal. This sequence triggers when entering the core chamber, sets the `RevealTriggered` flag in `NarrativeState`, removes/disables Mira, and rolls the closing state.

**Files:** `Narrative/RevealSequence.cs`, `Narrative/Tests/RevealSequenceTests.cs`.

**Dependencies:** TASK-006 (NarrativeState).

**Acceptance criteria:** When triggered, the script sets `NarrativeFlags.RevealTriggered`, hides the player HUD/reticle, and disables `MiraDialogueSelector` outputs.

**Test:** EditMode tests ensuring all narrative flags and system state changes execute sequentially when `TriggerReveal()` is called.

---

### [x] TASK-015: Audio Management System

**Objective:** Build a centralized `AudioManager` using the `ServiceLocator` pattern that handles routing audio to the correct `AudioMixerGroup` (Music, SFX, Voiceover).

**Files:** `Core/Audio/AudioManager.cs`, `Core/Audio/Tests/AudioManagerTests.cs`.

**Dependencies:** None.

**Acceptance criteria:** `AudioManager` registers with `ServiceLocator`. It exposes methods to play SFX, Music, and Voiceover, correctly routing them through Unity's `AudioSource` components with basic fading logic.

**Test:** EditMode tests mocking `AudioSource` creation and verifying that play requests correctly assign clips and volume logic.

---

### [x] TASK-016: Settings System & UI

**Objective:** Implement persistent settings (Resolution, Master/Music/SFX Volume, and Input Rebinding).

**Files:** `Core/Settings/SettingsManager.cs`, `UI/SettingsUI.cs`, `Core/Settings/Tests/SettingsManagerTests.cs`.

**Dependencies:** TASK-013 (PauseMenuUI - to open Settings).

**Acceptance criteria:** Changing volume updates `AudioMixer` parameters. Settings are saved/loaded. Keybinds can be overridden in the Unity Input System.

**Test:** EditMode tests verifying setting values correctly update and fire change events.

---

### [x] TASK-017: UI Toolkit Polish (USS/Transitions)

**Objective:** Upgrade our basic `GameplayUI` and `PauseMenuUI` with smooth transitions and premium hover states.

**Files:** `UI/GameplayUI.cs`, `UI/PauseMenuUI.cs`.

**Dependencies:** TASK-009, TASK-013.

**Acceptance criteria:** UI controllers are updated to toggle USS classes (e.g., `.menu-open`, `.menu-closed`) instead of hardcoding `DisplayStyle.None`.

**Test:** EditMode tests verifying the correct CSS classes are added/removed upon state changes.

---

### [x] TASK-018: VFX & Shader Controllers

**Objective:** Provide the C# scripts necessary to drive the "Memory Formation" and "RECALL" shader effects dynamically over time.

**Files:** `Gameplay/VFX/MaterialPropertyFader.cs`.

**Dependencies:** None.

**Acceptance criteria:** A MonoBehaviour that takes a renderer, material property name, target float value, and duration, and tweens it via Coroutine.

**Test:** PlayMode or EditMode test verifying the material property reaches the target value.
