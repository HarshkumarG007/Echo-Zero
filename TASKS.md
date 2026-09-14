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

### TASK-003: Player controller and Aerie gray-box

**Objective:** Basic movement and camera in a gray-boxed (untextured) version of the Aerie's first room — enough to walk the critical path of the slice end to end once other systems land.

**Files:** `Gameplay/Player/PlayerController.cs`, `Gameplay/Player/PlayerCamera.cs`, `World/Scenes/Aerie.unity` (gray-box geometry).

**Dependencies:** TASK-001.

**Acceptance criteria:** Player can walk, look around, and not fall through geometry, at a stable frame time on the target laptop. No PC-only input assumptions — use the Input System, not hardcoded `KeyCode`.

**Test:** PlayMode test — spawn the player, apply movement input for N frames, assert position changed in the expected direction and the player stayed within the gray-box bounds.

---

### TASK-004: RECALL skeleton

**Objective:** The core verb, data-flow only — no visuals yet. Player targets an object, RECALL queries World State for a fragment, fires a stub event either way. This is the piece everything else attaches to.

**Files:** `Gameplay/Recall/RecallController.cs`, `Core/WorldState/WorldState.cs`, `Core/WorldState/Tests/WorldStateTests.cs`.

**Dependencies:** TASK-002, TASK-003.

**Acceptance criteria:** Targeting a tagged object with a registered fragment fires `OnFragmentFound` with the correct fragment ID; targeting anything else fires `OnNothingFound`. No visual or audio work in this task.

**Test:** EditMode test on `WorldState` directly — register a fragment, query it, assert the correct result. Doesn't need the scene at all.

---

### TASK-005: Fragment data model and contradiction check

**Objective:** ScriptableObject-based fragment definitions, plus the logic that detects when two found fragments contradict each other — the piece the puzzle, the meaningful choice, and Mira's dialogue all depend on.

**Files:** `Narrative/Fragments/FragmentData.cs` (ScriptableObject), `Narrative/Fragments/FragmentRegistry.cs`, `Narrative/Fragments/Tests/ContradictionTests.cs`.

**Dependencies:** TASK-004.

**Acceptance criteria:** Two fragments flagged as contradicting each other correctly trigger the "player must choose" state; non-contradicting fragments don't; validating one fragment in a contradicting pair correctly invalidates the other.

**Test:** EditMode tests — the most logic-heavy, most testable piece in the whole slice. Aim for full coverage of contradiction resolution without touching the scene.
