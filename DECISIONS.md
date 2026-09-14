# Decisions — ECHO//ZERO

New ADRs get appended here, numbered sequentially, only when a decision actually needs to be locked in — not speculatively. Three exist so far: the ones that block starting the vertical slice at all.

---

## ADR-0001: Unity 6.3 LTS with URP

**Status:** Accepted

**Context:** Development happens on a single laptop (RTX 4060 Laptop GPU, 8GB VRAM, 16GB RAM, i7-14650HX). The slice needs moody, layered lighting for a crystalline/organic sci-fi-fantasy look, but has to run smoothly enough to iterate on constantly, by one person, with no second machine or render farm to fall back on.

**Decision:** Unity 6.3 LTS (not 6.0 LTS), Universal Render Pipeline (URP), not HDRP.

**Why:** Unity 6.0 LTS support ends October 2026 — a month from now — while 6.3 LTS is supported through December 2027, so 6.0 isn't really an option for a project starting today. URP is the lighter, more scalable pipeline and the safer choice for a laptop-class mobile GPU with only 8GB VRAM. HDRP targets higher-end desktop GPUs and is considerably heavier on VRAM and shader complexity — workable in principle here, but it leaves little headroom for the crystalline-formation shaders, the Drift VFX, or the texture budget, and it slows iteration, which is the resource a solo dev can least afford to lose. URP's constraints (fewer full-screen post effects, simpler shader graphs) also impose useful discipline early.

**Consequences:** Some HDRP-exclusive effects (certain volumetrics, some ray-traced options) aren't available. The crystalline-growth look needs a custom shader graph and careful use of URP's volume/post-processing stack instead of HDRP's built-in tools.

**Rejected:** HDRP (too heavy for the constraint machine); Built-in Render Pipeline (deprecated path, no upside over URP for a new project); Unity 6.0 LTS (support window closes next month).

---

## ADR-0002: JSON save file, not ScriptableObjects or a database

**Status:** Accepted

**Context:** The slice needs to persist: which memory fragments have been found, which of the two contradictory fragments (if any) was validated, RECALL unlock state, Mira's current dialogue state, player position, and basic settings. A small, single-player, single-save-slot problem, not a live-service data problem.

**Decision:** One JSON save file per save slot, via Newtonsoft.Json (`com.unity.nuget.newtonsoft-json`), loaded into a plain C# save-data class.

**Why:** The fragment/contradiction state is naturally a set of small nested collections — exactly what Unity's built-in `JsonUtility` handles poorly (no dictionary support, no polymorphism) and Newtonsoft handles well. JSON is human-readable, which matters for a solo dev debugging save issues by eye. At this data scale (a few KB per save), JSON vs. binary performance is irrelevant.

**Consequences:** Save files are technically editable by players — not a concern for a single-player slice. Needs basic defensive parsing on load so a hand-edited or corrupted file fails gracefully instead of crashing.

**Rejected:** ScriptableObjects (asset-time data, not built for frequent runtime read/write); a local database like SQLite (real overkill at this size); binary serialization (harder to debug, no benefit at this scale).

---

## ADR-0003: Five folders, not eight layers

**Status:** Accepted

**Context:** A full eight-to-ten-layer architecture (Presentation, Gameplay, Game Systems, Simulation, AI/Agent, Data, Infrastructure, Observability, Evaluation, Security) was on the table. The vertical slice has no multiplayer, no simulation separate from gameplay, no learned AI, and no network-facing surface — several of those layers would be empty scaffolding.

**Decision:** `Assets/_Project` split into five folders: `Core/` (bootstrap, scene loading, save/load), `Gameplay/` (player controller, RECALL, Drift encounter), `World/` (Aerie scene content, memory-formation shader/logic), `Narrative/` (Mira dialogue, fragment data as ScriptableObjects), `UI/`.

**Why:** Every folder maps to something that actually exists in the slice today. Growing into more layers later is a cheap, mechanical refactor once there's a real second system that needs the separation — separating prematurely just adds indirection with no present payoff.

**Consequences:** If the game later adds real learned AI or multiplayer, expect a deliberate refactor (a new ADR) to introduce the layers that are genuinely needed then, instead of guessing their shape now.

**Rejected:** The full layer stack (right for a large team building a persistent platform, not a 20–30 minute solo slice); a flat single-folder structure (too little separation even at this size — `Gameplay` and `Narrative` would tangle almost immediately, since Mira's dialogue has to react to RECALL state).
