# Demo Recording Script: ECHO//ZERO

**Target Length:** 15-20 Minutes
**Format:** Gameplay walkthrough with live voiceover commentary focusing on technical implementation.

---

## 1. Introduction & The Aerie Upper (0:00 - 3:00)
* **Visual:** Player spawns in the Bootstrap scene, transitioning into the Aerie Upper. The UI initializes.
* **Action:** Walk around the environment, demonstrating smooth camera control and locomotion.
* **Voiceover:** "Welcome to ECHO//ZERO. What you're looking at is a Unity 6 URP vertical slice. The architecture here relies heavily on a decoupled Service Locator and Event Bus pattern. Notice the UI—it's built entirely in UI Toolkit and listens for strongly-typed events rather than relying on direct MonoBehaviour references or `Update()` polling."

## 2. The Core Mechanic: RECALL (3:00 - 6:00)
* **Visual:** Player approaches the first Memory Fragment. Holds Right Mouse Button to activate the RECALL visual effect. The fragment reconstructs.
* **Action:** Collect the fragment. Mira's dialogue box appears.
* **Voiceover:** "This is the RECALL ability. Under the hood, this uses a raycast that queries a centralized `WorldState` service. When a fragment is collected, it registers with the `FragmentRegistry`. To prevent players from spamming the input to skew our telemetry data, this ability is rate-limited by a configurable cooldown injected via `IConfigService`."

## 3. The Utility AI: The Drift (6:00 - 10:00)
* **Visual:** Player enters the Aerie Lower. A Drift enemy is patrolling, spots the player, and begins pursuit.
* **Action:** Kite the Drift around the room, showing how it cuts off the player's path rather than following directly behind.
* **Voiceover:** "Here is our primary enemy, the Drift. Early in development, this was a simple Finite State Machine. We upgraded it to a Utility AI Brain. Instead of hardcoded state transitions, it scores actions dynamically. More importantly, its pursuit logic is driven by a local Machine Learning model. We integrated Unity Sentis to run an ONNX model locally, taking the player's velocity vector to predict trajectory, allowing the Drift to intercept rather than just follow—all while maintaining a sub-1ms AI budget."

## 4. Adaptive Narrative: The Choice Pedestal (10:00 - 15:00)
* **Visual:** Player approaches the two Choice Pedestals containing contradictory fragments.
* **Action:** Target one pedestal with RECALL to lock in the choice. Talk to Mira.
* **Voiceover:** "The core narrative revolves around contradictory memories. The `FragmentRegistry` evaluates these contradictions mathematically. Depending on how I've played so far—tracked via an anonymous `AggressionScore` in our local telemetry service—Mira's dialogue weights actually shift. Since I charged directly at the Drift earlier, the ML model biases her responses toward the 'Aggressive' variants, seamlessly adapting the narrative to my playstyle."

## 5. Security and Conclusion (15:00 - 18:00)
* **Visual:** Player pauses the game, clicks 'Save Game', then exits to the main menu.
* **Voiceover:** "Finally, a note on robustness. Our save system relies on JSON serialization, but it's protected by a SHA-256 checksum. If a player—or a bug—tampers with the `save_01.json` file on disk without updating the checksum, the `SaveService` detects the mismatch and gracefully falls back to a clean state. This, along with the entire project, was built autonomously by an AI agent adhering to strict architectural constraints."
