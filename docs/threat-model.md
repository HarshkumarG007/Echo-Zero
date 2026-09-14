# docs/threat-model.md — ECHO//ZERO

> Threat model for the vertical slice.
> Scope: single-player offline game. Threats beyond this scope are explicitly out of scope.
> Review at Phase 9 (Red Team gate G8/G9).

---

## 1. Assets and Trust Boundaries

```
┌─────────────────────────────────────────────────────┐
│  TRUSTED: Development machine (dev only)             │
│  - Source code                                       │
│  - Unity project                                     │
│  - Editor tools                                      │
└───────────────────┬─────────────────────────────────┘
                    │ git push
                    ▼
┌─────────────────────────────────────────────────────┐
│  SEMI-TRUSTED: Git repository                        │
│  - Must contain no secrets                           │
│  - Must contain no machine-local paths               │
└───────────────────┬─────────────────────────────────┘
                    │ distributed build
                    ▼
┌─────────────────────────────────────────────────────┐
│  UNTRUSTED: Player machine                           │
│  - Executable binary                                 │
│  - Save files (player can modify)                    │
│  - Telemetry log (player can read)                   │
│  - No network access in Phase 3                      │
└─────────────────────────────────────────────────────┘
```

---

## 2. Threat Register

| ID | Threat | Asset | Attack Vector | Impact | Likelihood | Mitigation | Verification |
|----|--------|-------|--------------|--------|-----------|-----------|-------------|
| T-01 | Save file tampering | Narrative flags / choice | Player edits save_01.json | Bypasses the one meaningful choice | Medium (motivated players) | SHA-256 checksum on every save; reject load on mismatch | Unit test: corrupt JSON → load → returns null |
| T-02 | Save file deletion | Player progress | Player deletes save file | Crash on load attempt | Low (accidental or intentional) | Null-safe load path; default to new game state | Unit test: no file → Load() → default state returned |
| T-03 | Save file injection | Game state | Player crafts save with invalid fragment IDs or impossible flags | NarrativeState inconsistency, unexpected dialogue | Low | Validate fragment IDs against known list on load; ignore unknown IDs | Unit test: load save with unknown fragment ID → warning logged, ID ignored |
| T-04 | RECALL input spam | DriftFSM state | Rapid RECALL button presses during Drift encounter | State machine corruption | Medium (accidental) | Idempotency: Stabilizing state ignores RECALL events after transition begins | Unit test: call Stabilize() N times → state = Stabilized, event published once |
| T-05 | Telemetry PII leak | Player privacy | Developer adds system path, username, or machine name to telemetry event | Privacy violation | Low (development error) | Code review + automated test: all telemetry event types inspected for string interpolation of system variables | EditMode test: verify no Environment.UserName, Application.dataPath in any event payload |
| T-06 | Secret in repository | Credentials | Developer accidentally commits .env, API key, or token | Credential exposure | Low (no Phase 3 external services) | .gitignore covers .env, *.key, *.secret; pre-commit scan added in Phase 10 | Manual: git log --all -p \| grep -i "api_key\|token\|secret" before every release |
| T-07 | Agent scope overrun | Project files | Antigravity agent modifies files outside its declared task scope | Unexpected architectural changes | Medium (incorrect prompt) | Every task defines FILES_EXPECTED_TO_CHANGE; review git diff before accepting | Pre-commit: compare git diff --name-only to task list |
| T-08 | Dependency supply chain | Runtime | Malicious or compromised UPM/NuGet package | Runtime exploit | Low (no external services; packages from official registry) | UPM packages from Unity official registry only; ADR required for any new package | Verify package source and last update date before adding |
| T-09 | Replay/telemetry file exfil | Play data | Player sends telemetry file to others | Not a concern in Phase 3 — local only, no PII | Very Low | No PII in telemetry (T-05 mitigation covers this) | — |

---

## 3. Out of Scope Threats (Phase 3)

The following are explicitly out of scope because the game has no network, no multiplayer, no server, and no authentication:

- Network-based attacks (MITM, replay, injection)
- Authentication bypass
- DDoS / resource exhaustion via network
- Multi-user privilege escalation
- Cloud credential theft at runtime

These are reconsidered if any Phase 6+ feature introduces external network calls (requires new ADR).

---

## 4. Mitigations Already Implemented

| Mitigation | Where | Status |
|-----------|-------|--------|
| SHA-256 save checksum | SaveService spec in technical-design.md | Designed — not yet coded (TASK-003+) |
| Null-safe load path | SaveService spec | Designed — not yet coded |
| Fragment ID validation on load | NarrativeState spec | Designed — not yet coded |
| No PII in telemetry | TelemetryService spec + SECURITY.md | Enforced by design |
| .gitignore for secrets | To be created in TASK-001 | Pending |
| Agent scope control | AGENTS.md §4 + task FILES_EXPECTED_TO_CHANGE | In effect now |
| Official-registry-only packages | ADR process | In effect now |

---

## 5. Residual Risk Acceptance

After all mitigations, the following residual risks are **accepted** for a single-player portfolio project:

| Risk | Reason for Acceptance |
|------|-----------------------|
| Determined player can regenerate save checksum | SHA-256 prevents accidental corruption; a determined player can still cheat. This is acceptable — there is no competitive environment and no monetisation. |
| Source code readable in IL2CPP build (partial) | Portfolio project — no proprietary algorithms worth protecting. |
| Telemetry file readable by player | Contains no PII; reading it reveals only anonymous gameplay data. Acceptable. |

---

*Last updated: Phase 2 — Foundation complete.*
*Full red team review: Phase 9 gate G8/G9.*
