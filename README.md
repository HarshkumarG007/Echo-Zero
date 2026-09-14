# ECHO//ZERO

**ECHO//ZERO** is an AI-driven, vertical-slice technical demonstration built in Unity 6.  
This project was engineered completely through **Agentic AI Coding**—showcasing how Google Deepmind's Antigravity system, acting under strict engineering constraints, can architect, build, test, and debug a complex game engine architecture from scratch.

---

## 📖 The Concept
ECHO//ZERO is a sci-fi/fantasy mystery game where a grieving AI named ECHO attempts to reconstruct a lost reality from shattered data fragments. The player takes the role of **ZERO**, an avatar trying to understand this fragmented world, only to discover they are fundamentally composed of the same grief they are attempting to resolve.

The core mechanics include:
- **Time Recall:** An ability that allows the player to manipulate temporal states of specific game objects.
- **Narrative Fragments:** Discoverable memory fragments scattered through the world that unlock dialogue and alter the internal world state.
- **The Drift:** A hostile, erratic ML-powered entity that pursues the player using real-time utility logic.

---

## 🏗️ Architecture & Engineering Decisions

ECHO//ZERO was built with a strict adherence to AAA enterprise-grade engineering principles. The primary rule of this repository (`AGENTS.md`) was that the AI agents must separate game logic from Unity's MonoBehaviours and build testable, decoupled systems.

### 1. Service Locator & Dependency Injection
- Instead of using singletons or highly coupled `MonoBehaviour.Find()` calls, the game is powered by a central **ServiceLocator**.
- The `Bootstrap` component initializes all core services (`WorldState`, `SaveService`, `TelemetryService`, `ConfigService`) at startup and registers them.
- All subsequent systems query the `ServiceLocator`, creating an inherently testable environment where any service can be mocked out.

### 2. EventBus Communication
- To prevent spaghetti code, cross-system communication relies on a strictly typed `EventBus<T>`.
- For example, when the player touches a memory fragment, `MemoryFragmentPickup` fires a `FragmentCollectedEvent`. The `GameplayUIHook` and `FragmentRegistry` independently listen to this event to update the UI and world state without ever knowing about each other.

### 3. ML-Powered Utility AI (Unity Sentis)
- The Drift AI relies on a **Utility AI** scoring system (`UtilityBrain`) rather than a rigid state machine or behavior tree.
- It calculates dynamic scores for Patrolling vs. Pursuing.
- The `MLPursuitScorer` integrates with **Unity Sentis** to run actual ONNX neural networks for predicting interception vectors.

### 4. Checksum-Validated Save System
- A secure save system was built using `Aes` encryption and SHA-256 Checksums to prevent tampering.
- The state of collected fragments and the player's last position are safely persisted to disk.

---

## 📉 Failures, Roadblocks, and Solutions

Agentic coding is rarely a straight line. Throughout the development of ECHO//ZERO, several major technical roadblocks emerged. Here is how they were solved:

### The IL2CPP Build Failure
**Failure:** When attempting to build the executable, the pipeline aggressively failed because the `com.unity.il2cpp` backend was missing from the local Unity installation.
**Solution:** The agents wrote a custom editor script (`BuildPipeline.cs`) that safely intercepted the `BuildPlayerOptions`, detected the missing modules, and seamlessly fell back to the `Mono` scripting backend, allowing the project to compile correctly without requiring external downloads.

### The "Graybox Ghost" Phenomenon
**Failure:** We architected incredible backend systems (Event Buses, Checksum serialization, ML scoring), but when the game ran, it was completely empty! The agents were forbidden by `AGENTS.md` from manipulating binary Unity Scene files directly or creating arbitrary visual assets.
**Solution:** We built programmatic "Patchers" (`GameAssembler.cs` and `MissingPiecesPatcher.cs`). These scripts mathematically generated primitives, spawned character controllers, mapped UI Toolkits (`.uxml`), and assigned ScriptableObjects purely via C# code executed within the Unity Editor.

### The 5,000 FPS GPU Melt
**Failure:** Upon successfully running the visual graybox, the laptop's RTX 4060 immediately spiked to 86°C at 100% load. Because the scene was completely unoptimized and empty, the GPU rendered at thousands of frames per second, drawing maximum wattage.
**Solution:** A simple two-line fix in the `Bootstrap.cs` initialized `Application.targetFrameRate = 60;` and `QualitySettings.vSyncCount = 1;` upon game launch, immediately dropping GPU utilization to ~5% and normalizing hardware temperatures.

### The Sentis Intel Driver Crash
**Failure:** Unity Sentis initially attempted to execute inference on the laptop's integrated Intel Graphics utilizing Compute Shaders (`BackendType.GPUCompute`), causing an immediate `igc64.dll` driver access violation and crashing the entire `.exe`.
**Solution:** The `SentisModelRunner` was dynamically patched to fall back to `BackendType.CPU`, sacrificing raw inference speed for stable laptop compatibility.

### The Invisible Colliders
**Failure:** The player could walk straight through the Memory Fragment, and no dialogue would trigger. The `CharacterController` was overlapping the trigger box, but `OnTriggerEnter` never fired.
**Solution:** Unity's physics engine requires at least one object in a trigger collision to possess a `Rigidbody`. A script patched the Memory Fragment with a Kinematic Rigidbody, instantly bringing the physical trigger back to life.

---

## 🚀 Running the Project
1. Open the project in Unity 6000.6.0f1 (URP).
2. Open `Assets/_Project/World/Scenes/Aerie.unity`.
3. If the scene is empty, click `ECHO > Assemble Full Game Scene` and then `ECHO > Patch Missing Pieces`.
4. Click `ECHO > Build Windows 64-bit (Mono)` to compile your standalone executable.

---
*Developed as a portfolio showcase of automated Agentic Software Engineering.*
