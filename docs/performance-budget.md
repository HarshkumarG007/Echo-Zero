# docs/performance-budget.md — ECHO//ZERO

> Performance budget for the vertical slice.
> All values target the development machine: RTX 4060 Laptop / 16 GB RAM / 8 GB VRAM.
> Values marked [NOT MEASURED] must be filled in at Quality Gate G5 (Phase 5).
> No budget may be "estimated" at G5 — all must be measured on device.

---

## 1. Target Configuration

| Item | Value |
|------|-------|
| Device | Lenovo Legion Pro 5i |
| GPU | RTX 4060 Laptop 8 GB VRAM |
| CPU | Intel i7-14650HX |
| RAM | 16 GB |
| OS | Windows |
| Resolution | 1920 × 1080 |
| Target FPS | 60 |
| V-Sync | Off (for profiling) / On (for player) |
| Render pipeline | URP |

---

## 2. Frame Budgets

| Metric | Budget | Measured | Gate | Tool |
|--------|--------|---------|------|------|
| Total frame time | ≤ 16.6 ms | [NOT MEASURED] | G5 | Unity Profiler |
| CPU main thread | ≤ 8 ms | [NOT MEASURED] | G5 | Unity Profiler |
| GPU frame time | ≤ 8 ms | [NOT MEASURED] | G5 | GPU profiler |
| Render thread CPU | ≤ 3 ms | [NOT MEASURED] | G5 | Unity Profiler |
| Physics step | ≤ 1 ms | [NOT MEASURED] | G5 | Unity Profiler |
| Script update total | ≤ 3 ms | [NOT MEASURED] | G5 | Unity Profiler |
| RECALL coroutine (worst case) | ≤ 2 ms | [NOT MEASURED] | G4 | ProfilerMarker |
| DriftFSM.Update | ≤ 0.5 ms | [NOT MEASURED] | G4 | ProfilerMarker |
| SaveService.Save | ≤ 16 ms total (async) | [NOT MEASURED] | G4 | ProfilerMarker |
| Scene load (AerieLower) | ≤ 2000 ms | [NOT MEASURED] | G4 | SceneManager timing |

---

## 3. Memory Budgets

| Metric | Budget | Measured | Gate |
|--------|--------|---------|------|
| VRAM total | ≤ 5 GB | [NOT MEASURED] | G5 |
| VRAM textures | ≤ 3 GB | [NOT MEASURED] | G5 |
| VRAM render targets | ≤ 1 GB | [NOT MEASURED] | G5 |
| RAM total | ≤ 10 GB | [NOT MEASURED] | G5 |
| Managed heap | ≤ 512 MB | [NOT MEASURED] | G5 |
| Asset bundle (AerieLower) | ≤ 300 MB uncompressed | [NOT MEASURED] | G4 |

---

## 4. Draw Call and Geometry Budgets

| Metric | Budget | Measured | Gate |
|--------|--------|---------|------|
| Draw calls / frame | ≤ 150 | [NOT MEASURED] | G5 |
| Shadow caster draw calls | ≤ 50 | [NOT MEASURED] | G5 |
| Triangles / frame | ≤ 800 K | [NOT MEASURED] | G5 |
| Vertices / frame | ≤ 1 M | [NOT MEASURED] | G5 |

---

## 5. Texture Standards

| Standard | Value | Enforced |
|----------|-------|---------|
| Max texture size | 2048 × 2048 | G3 (import settings) |
| Albedo compression | DXT5 / BC7 | G3 |
| Normal maps | BC5 | G3 |
| Crystal emissive texture | 1024 × 1024 max | G3 |
| Mira character texture | 2048 × 2048 | G3 |
| Mipmaps | Enabled on all world textures | G3 |
| Anisotropic filtering | Level 4 (world), Level 1 (skybox) | G3 |

---

## 6. Profiler Markers Required

Every system in this list MUST have a `ProfilerMarker` wrapping its primary operation.
Verified at G3 that markers exist; measured at G5.

```csharp
// Examples — must exist in production code
private static readonly ProfilerMarker s_RecallMarker =
    new ProfilerMarker("RecallSystem.TryRecall");

private static readonly ProfilerMarker s_DriftMarker =
    new ProfilerMarker("DriftFSM.Update");

private static readonly ProfilerMarker s_SaveMarker =
    new ProfilerMarker("SaveService.Save");

private static readonly ProfilerMarker s_LoadSceneMarker =
    new ProfilerMarker("SceneLoader.LoadAdditiveAsync");

private static readonly ProfilerMarker s_MiraDialogueMarker =
    new ProfilerMarker("MiraDialogueSelector.SelectLine");
```

---

## 7. Crystal Shader — Specific Risk

The memory-formation crystal shader is the highest performance risk in the project.
It involves: emissive HDR values, transparency/translucency, growth animation (vertex shader),
and potentially screen-space effects.

**Constraints**:
- No screen-space refraction in Phase 3 — deferred to Phase 5 visual polish pass
- Crystal growth driven by a single float uniform (animation), not per-vertex simulation
- Alpha test only (no alpha blend) in Phase 3 — reduces overdraw
- Max crystal formations visible simultaneously: 20 (enforced by scene streaming unit size)
- Crystal shader must be profiled on device before Phase 5 commit — not assumed to be cheap

---

## 8. Profiling Checkpoints by Gate

### G3 — End of Phase 3 (Core Gameplay)
- [ ] Texture import settings enforced (max 2048, mipmaps, compression)
- [ ] All ProfilerMarker declarations present in production code
- [ ] Editor frame time baseline recorded (empty Aerie scene, no assets)
- [ ] No allocations in RecallSystem.TryRecall (verified via Profiler Allocations view)

### G4 — End of Phase 4 (Vertical Slice)
- [ ] Scene load time (AerieLower): measured, ≤ 2000 ms
- [ ] Save/load round-trip: measured end-to-end latency
- [ ] RECALL coroutine frame cost: measured in ProfilerMarker
- [ ] DriftFSM.Update: measured, ≤ 0.5 ms

### G5 — Phase 5 Performance Pass (dedicated)
- [ ] ALL metrics in §2, §3, §4 measured on device (not editor, not estimated)
- [ ] Memory Profiler snapshot taken: AerieUpper + AerieLower loaded simultaneously
- [ ] Crystal shader profiled: draw calls, VRAM cost, frame time contribution
- [ ] GPU profiler trace taken during Drift encounter (highest complexity moment)
- [ ] Frame time histogram: ≥ 95% of frames under 16.6 ms over a 5-minute session
- [ ] No measured metric may exceed its budget — gate FAILS if any do

---

## 9. What Happens If a Budget Is Exceeded at G5

1. Identify the worst offender via Unity Profiler + Memory Profiler
2. Write a micro-task to address it (specific, targeted, one system at a time)
3. Profile after the fix to confirm improvement
4. Do NOT proceed to Phase 6 until G5 is PASS

Common fixes that do NOT require an ADR:
- Reduce texture sizes below maximum
- Reduce crystal formation count
- Simplify crystal shader for Phase 3 (defer to Phase 5 polish)
- Reduce DriftFSM update frequency from every frame to every 3 frames

Fixes that DO require a new ADR:
- Switching from URP to HDRP (rejected in ADR-0002 — high bar to reverse)
- Adding Jobs/Burst to any system
- Changing additive scene layout

---

*Last updated: Phase 2 — Foundation complete. All values [NOT MEASURED] until G3–G5.*
