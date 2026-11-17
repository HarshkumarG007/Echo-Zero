# Design System & UI/UX Standards — ECHO//ZERO

**Document Version:** 1.0  
**Status:** [CONFIRMED] Baseline Documented  
**Author:** AI UI/UX Engineer & Technical Lead

---

## 1. Design Philosophy & Visual Language

ECHO//ZERO balances hard sci-fi industrial architecture with ethereal, crystalline memory constructs.

### Core Thematic Anchors
- **Crystal Density = Reconstruction Progress:** Areas touched by ECHO's reconstruction are encrusted with geometric, translucent blue-white crystals. Sparse crystal indicates untouched or raw states; dense, radiating crystal indicates fully recalled memories.
- **Cool vs. Warm Clash:** Intact facility areas feature cold industrial steel, clean glass, and muted blues. Corrupted or contradictory areas (such as the Archive where the Drift lurks) flicker with unstable amber, harsh shadows, and flickering emissions.
- **Minimalist Restraint:** The UI must never clutter the player's view. There is no minimap, no health bar, and no floating damage numbers. The interface exists solely to convey targeting, dialogue, and interaction feedback.

---

## 2. Color Palette & Lighting

| Role | Color Name | Hex Code | Usage |
| :--- | :--- | :--- | :--- |
| **Primary Accent** | Crystal Emissive | `#64C8FF` | Memory fragment aura, RECALL reticle active state, UI highlights |
| **Secondary Accent**| Memory Gold | `#FFD264` | Reconstruction anchor active state, validated choice highlight |
| **Warning / Drift** | Contradiction Amber| `#FF6B4A` | Drift destabilization aura, contradictory fragment warning |
| **Background / Dark**| Deep Slate | `#0D1117` | UI dialogue panels, pause menu overlay background |
| **Surface** | Industrial Steel | `#1E2530` | Panel borders, inactive button states |
| **Typography** | Memory White | `#F0F6FC` | Primary dialogue text, button labels |
| **Typography Dim** | Muted Subtext | `#8B949E` | Secondary captions, keybinding hints |

---

## 3. UI Toolkit Component Standards

The user interface is built entirely using Unity's **UI Toolkit** (UXML + USS), avoiding legacy uGUI canvases.

### 3.1 Central HUD Reticle (`GameplayUI.uxml`)
- Positioned strictly at screen center (`50%`, `50%`).
- **Default State:** Subtle circular dot (diameter 8px, opacity 0.4).
- **Targeting State (`.reticle-active`):** Grows to 24px ring with crystal blue accent (`#64C8FF`) and 1.0 opacity when pointing at an `IRecallable` object within range (15m).
- **Hold Progress State:** Radial or thickness tween indicating the 3-second hold required to stabilize the Drift.

### 3.2 Dialogue Overlay Box
- Positioned in lower third of screen, centered horizontally.
- Semi-transparent Deep Slate backdrop (`rgba(13, 17, 23, 0.85)`) with a 1px border (`#64C8FF` at 0.3 opacity).
- **Typography:** Clean sans-serif, 18pt size, 1.4 line-height for readability.
- **Transitions (`.dialogue-open`, `.dialogue-closed`):** Smooth opacity tween (0.2s duration) rather than immediate binary toggles (`display: none`).

### 3.3 Pause & Settings Menus (`PauseMenuUI.uxml`)
- Fullscreen backdrop blur with dark tint (`rgba(10, 12, 16, 0.75)`).
- Modal card centered with vertical button stack (Resume, Save Game, Settings, Quit).
- Button hover states shift background color smoothly from `#1E2530` to `#283344` with a 2px left border accent.

---

## 4. Interaction Patterns & Feedback

### 4.1 The RECALL Verb
1. **Raycast Detection:** As ZERO points towards an interactable object, the reticle scales and lights up.
2. **Key Press:** Pressing RECALL triggers an audio chime (`SFX/Recall_Ping`) and initiates particle/shader convergence.
3. **Execution:**
   - On **Fragments:** Instant activation; fragment object dissolves into a crystalline burst; dialogue overlay appears.
   - On **Anchors:** Physical structure materializes over 1.5 seconds via `MaterialPropertyFader`.
   - On **The Drift:** Sustained hold required; camera experiences subtle vibration; Drift audio shifts from dissonant static to a clean harmonic chord upon stabilization.

### 4.2 The Choice Pedestal
- Two distinct physical pedestals stand in the choice chamber.
- Targeting Pedestal A highlights Fragment A in the HUD dialogue box.
- Initiating RECALL requires a 1.0-second confirmation hold to prevent accidental inputs.
- Upon completion, the chosen pedestal emits a permanent golden glow (`#FFD264`) while the unselected pedestal crumbles into translucent dust.
