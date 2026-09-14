# TESTING.md — ECHO//ZERO

> Testing strategy and standards. Every class with business logic gets a test class.

---

## 1. Test Types

| Type | When | Location | Speed |
|------|------|----------|-------|
| EditMode | Pure C# logic, no Unity lifecycle | `Assets/Tests/EditMode/` | Fast |
| PlayMode | Requires MonoBehaviour, scene, coroutines | `Assets/Tests/PlayMode/` | Slow |

Default to EditMode. Use PlayMode only when the test genuinely needs Unity's lifecycle.

---

## 2. Test Naming

- Test class: `<ClassName>Tests.cs`
- Test method: `<MethodName>_<Scenario>_<ExpectedResult>`

```csharp
// Example
[Test]
public void TryRecall_WithNullTarget_ReturnsFalseAndLogsWarning()

[Test]
public void TryRecall_WithValidRecallable_PublishesRecallCompletedEvent()

[Test]
public void EventBus_AfterUnsubscribe_DoesNotCallHandler()
```

---

## 3. Mocking with NSubstitute

```csharp
// Correct NSubstitute usage
var mockRecallable = Substitute.For<IRecallable>();
mockRecallable.CanRecall.Returns(true);

var system = new RecallSystem(config);
system.TryRecall(mockRecallable);

mockRecallable.Received(1).OnRecall();
```

---

## 4. Test Isolation

- Each test class uses `[SetUp]` and `[TearDown]`
- Clear EventBus subscriptions in `[TearDown]`: `EventBus<MyEvent>.Clear()`
- Clear ServiceLocator in `[TearDown]`: `ServiceLocator.Clear()`
- No shared mutable state between tests

```csharp
[TearDown]
public void TearDown()
{
    EventBus<RecallCompletedEvent>.Clear();
    ServiceLocator.Clear();
}
```

---

## 5. Acceptance Criteria = Tests

Every task's acceptance criteria should map to at least one test assertion.
If an acceptance criterion cannot be automated, document why and add a manual verification step.

---

## 6. What We Do NOT Test

- Unity Editor-only code paths (GUIs, custom inspectors)
- Third-party package internals
- Platform-specific rendering (visual correctness) — handled by manual review

---

## 7. Running Tests

In Unity Editor: `Window → General → Test Runner`
- EditMode: runs immediately, no scene load needed
- PlayMode: requires entering play mode, takes longer

CI target (Phase 10): Unity batch mode `-runTests -testPlatform EditMode`

---

*Last updated: Phase 2 — Foundation.*
