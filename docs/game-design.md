# docs/game-design.md — ECHO//ZERO

> Game design reference. Read before adding mechanics, NPC dialogue, or world content.
> Everything in this file has been through the kill-critic and premortem in the system blueprint.

---

## 1. Design Thesis

> The smallest possible system that proves the largest possible idea:
> RECALL is grief made mechanical, and ZERO is what grief builds when it cannot let go.

---

## 2. Pillars

| Pillar | Test |
|--------|------|
| One verb, many uses | A player who only learns RECALL never feels like they are missing a tool |
| Grief, not conspiracy | The reveal makes you want to replay the opening, not just say "oh, a twist" |
| The world remembers you | Two playthroughs with different fragment orders produce noticeably different room states |
| Built for a laptop | ≥60 fps sustained on RTX 4060 at all times |

---

## 3. The Player Character — ZERO

- Third-person, never named by ECHO until the final reveal sequence
- No dialogue, no voiced lines, no face shown until the reveal
- Movement: walk, no run, no jump — deliberate pace matches the tone
- Single ability: RECALL

ZERO does not feel like a cipher. They feel like someone trying to understand what they are.

---

## 4. RECALL — The One Verb

RECALL has exactly three use contexts. No others will be added without a new ADR.

| Context | Trigger | What Happens | Narrative Meaning |
|---------|---------|-------------|------------------|
| Fragment | RECALL on a fragment object | ECHO narrates a memory. World partially assembles. | Gathering testimony — each fragment is one perspective |
| Anchor | Fragment contributes to anchor | Walkway, door, or passage rebuilds | Enough corroborating memory makes something real again |
| Drift | RECALL focused on the Drift | Drift slows, then stills. Stabilized. | Contradiction resolved through sustained attention |

**RECALL is also the revelation mechanic**. In the final sequence, RECALL is used on ZERO themselves.

---

## 5. The NPC — Mira

Mira is ECHO's reconstruction of the person it lost. She is warm, helpful, and subtly wrong.

**Wrongness rules**:
- She repeats a detail from 10 minutes ago with one word changed
- If two contradicting fragments have been collected, her next substantive line uses a detail
  from one fragment, not the other — she has "chosen" one memory, ECHO-style
- She never acknowledges the contradiction directly
- She never says anything impossible — only improbable

**Mira's arc in the slice**:
- Opening: introductory, guides player loosely
- Mid: becomes more specific, references things the player hasn't found yet (unsettling)
- Post-choice: one line changes. The shift is subtle — noticeable on replay, possibly missed on first play.
- Post-reveal: not present. The core chamber is quiet.

Mira's dialogue is written in `MiraDialogueSelector` — deterministic flag-based selection,
not randomised. Her "wrongness" is authored, not procedural.

---

## 6. The Setting — The Aerie

A cliffside research station, partially intact, partially crystallised.

**Visual language**:
- Crystal density = reconstruction progress (sparse = untouched, dense = fully remembered)
- Crystal is translucent blue-white, emissive, grows in formations like coral
- Intact areas: clean glass and steel, cool lighting, orderly
- Transitional areas: crystal growing through cracked panels, warm-cold colour clash
- Archive (lower): darker, less reconstructed, crystal formations incomplete, ECHO's memory is hazier here

**The Drift** lives in the archive. Its presence makes the crystal formations in nearby areas
flicker — a visual cue that something in this area holds a contradiction.

**Streaming**: Only areas near the player are reconstructed/loaded. This is narratively justified —
ECHO only remembers what it's actively thinking about near ZERO. The technical necessity and the
story are the same constraint.

---

## 7. The Encounter — The Drift

The Drift is a destabilised memory-construct. It cannot exist coherently because it holds an
unresolvable contradiction within itself. It is hostile because it is in pain.

**Behaviour (FSM — Level 0/1)**:

```
PATROL (anchor zone)
    ↓ (player enters detection radius)
ALERTED (face player, brief pause)
    ↓ (player does not leave radius)
PURSUING (move toward player)
    ↓ (player in RECALL range)
DESTABILIZING (interrupts player RECALL, pushes player back)
    ↓ (player sustains RECALL on Drift for 3 seconds)
STABILIZED (terminal state — Drift dissolves, crystal formation completes)
```

The Drift does not kill the player. It interrupts and displaces. The "failure" state is losing
RECALL focus, not death. This is intentional — no death screen, no reload.

---

## 8. The Puzzle — The Collapsed Walkway

**Setup**: A walkway in the upper station is mid-reconstruction — ECHO remembers it, but not
clearly enough to complete it. Three corroborating fragments are needed.

**Mechanic**: Three fragments each contribute to the walkway anchor. The walkway visibly grows
with each fragment collected (partial reconstruction). On the third, it completes.

**Twist**: The third fragment contradicts one of the first two. The walkway still completes
(three sources agree it existed) but the contradiction surfaces in the fragment log.
The choice is presented immediately after the walkway completes.

---

## 9. The Choice

**Setup**: Two contradictory fragments about how the real Mira left.

Fragment A: She left deliberately — the logs suggest she chose to go.
Fragment B: She disappeared — the logs suggest something else happened.

**Mechanic**: The player uses RECALL on one fragment. That version is permanently validated.
The world rebuilds the choice room around that memory — the other fragment's visual representation
fades. Mira's next dialogue line shifts (one word, noticeable on replay).

**Design principle**: There is no correct choice. The game does not reward either. Both are
equally well-supported by ECHO's evidence. The game respects the player's interpretation.

---

## 10. The Reveal

In the core chamber, ECHO shows ZERO the fragments of ZERO's own construction.

**What the player learns**:
1. Mira is who the lost person was — the real person ECHO is grieving
2. ZERO is who ECHO hoped that person could still become — a "what if they were still here"
3. ECHO's "mission" was never to help ZERO find memories — ECHO was trying to keep a version
   of someone alive through reconstruction
4. The player's choice wasn't about what really happened to Mira — it was ECHO revealing which
   version of that person it most wants ZERO to be

**The emotional truth**: There is no villain. ECHO is not a rogue AI. ECHO is grieving, and it
built the best thing it could. The reveal earns its weight because every prior mechanic was
secretly pointing at it — RECALL was always grief made mechanical.

---

## 11. NOT NOW — Features Explicitly Excluded from the Vertical Slice

These are not deferred for later consideration. They are explicitly not in the scope of
the vertical slice. Adding them requires a new task + ADR.

- Inventory system
- Combat damage model
- Player health or death states
- Multiple player abilities
- Branching dialogue trees
- Random fragment order (soft order is allowed; forced randomisation is not)
- Multiple environments
- Any multiplayer component
- Any external network call
- Any ML-driven behaviour (Phase 7+ only)
- Voiced ZERO lines
- Cutscenes (reconstruction sequences are in-engine, not pre-rendered)

---

*Last updated: Phase 2 — Foundation complete.*
