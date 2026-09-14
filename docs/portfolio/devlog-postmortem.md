# Postmortem: Architecting ECHO//ZERO with AI Agents

ECHO//ZERO wasn't just built; it was *negotiated*. Using Google Antigravity, the development of this Unity 6 vertical slice was a constant dialogue between creative intent and rigid architectural constraints. 

Here is a retrospective on the systems we built, what succeeded, and where the friction lay.

## What Worked: The Decoupled Core
Early on, we enforced ADR-0004 (Service Locator) and ADR-0005 (Event Bus). In a traditional indie project, the `PlayerController` might directly call `SaveManager.Save()`. Here, that was strictly forbidden. 

By isolating logic into testable C# classes and communicating entirely via `EventBus<T>.Publish()`, the agents were able to write EditMode tests for 90% of the game's logic without ever touching a Unity Scene. When the Red Team phase required us to implement strict cooldowns on the `RecallAbility` to prevent telemetry spam, the agent could confidently inject the `IConfigService` into the ability and write a unit test without risking a ripple effect across the physics or audio systems.

**Takeaway:** Agentic workflows thrive on strict interfaces. Ambiguity is the enemy of autonomous coding.

## The Friction: Transitioning to Utility AI
In Phase 3, the "Drift" enemy was governed by a Finite State Machine (FSM). It was predictable and easy to test. However, in Phase 6, we transitioned to a Utility AI Brain.

The friction didn't come from the code—it came from the *design tuning*. An agent can write a perfect `UtilityScorer` that evaluates a distance curve, but tuning those animation curves so the Drift feels "threatening but fair" is inherently subjective. We had to rely heavily on benchmarking scripts (TASK-026) to mathematically prove the AI was improving, replacing subjective "feel" with objective metrics (e.g., time-to-intercept).

**Takeaway:** When AI agents build AI behavior, you must establish mathematical evaluation criteria (Quality Gates) rather than relying on human playtesting loops.

## The Experiment: Unity Sentis & ML
Integrating Unity Sentis to run local ONNX models for the Drift's pursuit logic (TASK-023) was a gamble. We wanted the Drift to predict player trajectories rather than just follow them, but cloud API latency was unacceptable for a real-time action game.

By compiling the model to ONNX and running it locally via Sentis, we maintained a sub-1ms AI budget. The constraint was the model size—we had to keep it extremely lightweight to avoid stalling the main thread on lower-end GPUs, which required the ML engineer agent to heavily quantize the model before integration.

**Takeaway:** Local ML in Unity is viable for real-time gameplay, but the performance budget must be aggressively policed at the tensor allocation level.

## Conclusion
ECHO//ZERO proves that an AI coding agent isn't just a fast typist. By establishing a robust `AGENTS.md` rulebook, an agent can act as a disciplined Technical Director, enforcing architectural boundaries that ensure a project remains scalable and stable from prototype to IL2CPP release.
