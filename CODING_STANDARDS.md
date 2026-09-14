# CODING_STANDARDS.md — ECHO//ZERO

> Binding standards for all C# code in this project.
> If an AI agent generates code that violates these, it MUST fix the violation before committing.

---

## 1. Namespaces

Every type must be in a namespace. Pattern: `EchoZero.<Layer>.<Feature>`

```csharp
// Correct
namespace EchoZero.Core.Events { }
namespace EchoZero.Gameplay.Recall { }
namespace EchoZero.World.Streaming { }
namespace EchoZero.Narrative.Fragments { }
namespace EchoZero.AI.Drift { }
namespace EchoZero.Data.Save { }
namespace EchoZero.UI.HUD { }

// Wrong
namespace Scripts.Gameplay { }
namespace EchoGame { }
// (no namespace) — forbidden
```

---

## 2. MonoBehaviour Rules

MonoBehaviours are **thin controllers**. Business logic lives in plain C# classes.

```csharp
// Correct
public class RecallAbility : MonoBehaviour
{
    [SerializeField] private RecallConfig _config;

    private RecallSystem _recallSystem;

    private void Awake()
    {
        _recallSystem = new RecallSystem(_config);
    }

    private void OnRecallInput()
    {
        if (Physics.Raycast(/* ... */out var hit))
        {
            if (hit.collider.TryGetComponent<IRecallable>(out var target))
                _recallSystem.TryRecall(target);
        }
    }
}

// Wrong — business logic in MonoBehaviour
public class RecallAbility : MonoBehaviour
{
    private void OnRecallInput()
    {
        // 80 lines of inline logic here — FORBIDDEN
    }
}
```

**No public fields on MonoBehaviours**. Use `[SerializeField] private`.

---

## 3. Interfaces

Use interfaces for anything that:
- Will be tested (mock via NSubstitute)
- Could have multiple implementations
- Crosses a layer boundary

```csharp
// Correct
public interface IRecallable
{
    bool CanRecall { get; }
    void OnRecall();
}

public interface ISaveService
{
    void Save(GameSaveData data);
    GameSaveData Load();
}
```

---

## 4. Method Length

Max ~30 lines per method. If longer, extract into named helper methods.

Rationale: a 30-line method can be read in one screen. Longer methods hide bugs.

---

## 5. Naming

| Element | Convention | Example |
|---------|-----------|---------|
| Classes | PascalCase | `RecallSystem` |
| Interfaces | IPascalCase | `IRecallable` |
| Methods | PascalCase | `TryRecall()` |
| Private fields | _camelCase | `_recallSystem` |
| Parameters | camelCase | `target` |
| Properties | PascalCase | `CanRecall` |
| Constants | UPPER_SNAKE | `MAX_RECALL_RANGE` |
| Events (structs) | PascalCase + Event suffix | `RecallCompletedEvent` |

No abbreviations except established acronyms: `UI`, `ADR`, `ID`, `FSM`, `NPC`.

---

## 6. Documentation

All `public` and `internal` APIs require XML summary docs:

```csharp
/// <summary>
/// Attempts to recall the given target. If successful, publishes
/// <see cref="RecallCompletedEvent"/> via <see cref="EventBus{T}"/>.
/// </summary>
/// <param name="target">The object to recall. Must not be null.</param>
/// <returns>True if the recall succeeded; false if target is null or CanRecall is false.</returns>
public bool TryRecall(IRecallable target)
```

---

## 7. TODO Policy

No `TODO` comment without a linked TASK_ID:

```csharp
// Wrong
// TODO: add audio here

// Correct
// TODO(TASK-008): trigger ECHO voiceover on fragment collect
```

---

## 8. Event Subscription Discipline

Every subscriber MUST unsubscribe in `OnDestroy` or `OnDisable`:

```csharp
private void OnEnable()
{
    EventBus<FragmentCollectedEvent>.Subscribe(OnFragmentCollected);
}

private void OnDisable()
{
    EventBus<FragmentCollectedEvent>.Unsubscribe(OnFragmentCollected);
}
```

---

## 9. Null Safety

- Prefer null-conditional operators: `target?.OnRecall()`
- Validate parameters at public method boundaries, log a warning if null
- Do NOT silently swallow nulls with empty catch blocks

```csharp
public bool TryRecall(IRecallable target)
{
    if (target == null)
    {
        Debug.LogWarning("[RecallSystem] TryRecall called with null target.");
        return false;
    }
    // ...
}
```

---

## 10. Composition Over Inheritance

Prefer composing behaviours via interfaces and injected dependencies over deep inheritance trees.

```csharp
// Prefer this
public class DriftFSM
{
    private readonly IPerceptionSystem _perception;
    private readonly INavigator _navigator;
    // ...
}

// Avoid this
public class AdvancedDrift : BasicDrift : EnemyBase : CharacterBase { }
```

Exception: Unity's own inheritance (MonoBehaviour, ScriptableObject) is obviously necessary.

---

## 11. Performance-Sensitive Code

- No `new` allocations in Update() hot paths — use object pools or pre-allocate
- No `FindObjectOfType` or `GetComponent` in Update() — cache in Awake/Start
- No `string` concatenation in per-frame paths — use `StringBuilder` or structured logging
- Add `[ProfilerMarker]` to any method called more than once per second

---

*Last updated: Phase 2 — Foundation complete.*
