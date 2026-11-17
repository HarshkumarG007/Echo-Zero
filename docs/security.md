# Security Posture & Threat Model — ECHO//ZERO

**Document Version:** 1.0  
**Status:** [CONFIRMED] Active Security Baseline  
**Author:** AI Application Security Reviewer & Technical Lead

---

## 1. Security Scope & Trust Boundaries

ECHO//ZERO is an offline, single-player desktop application with zero external network connectivity. Threats associated with web servers, multi-tenant databases, authentication services, or cloud infrastructure are explicitly out of scope.

### Trust Boundaries Diagram
```
┌─────────────────────────────────────────────────────────────┐
│  TRUSTED: Developer Machine                                 │
│  • Source code, C# projects, Editor scripts                 │
└──────────────────────────────┬──────────────────────────────┘
                               │ git commit / push
                               ▼
┌─────────────────────────────────────────────────────────────┐
│  SEMI-TRUSTED: Git Repository (GitHub Public)               │
│  • Must contain zero secrets, API keys, or machine paths    │
│  • Commits must be auditable and traceable to task IDs       │
└──────────────────────────────┬──────────────────────────────┘
                               │ standalone compilation
                               ▼
┌─────────────────────────────────────────────────────────────┐
│  UNTRUSTED: End-User Desktop                                │
│  • Executable binary & data folder                          │
│  • Local save file (`save_01.json`)                         │
│  • Local telemetry log (`telemetry_session.json`)           │
│  • User has full local administrative read/write access     │
└─────────────────────────────────────────────────────────────┘
```

---

## 2. Threat Register & Mitigations

| Threat ID | Threat Description | Asset at Risk | Mitigation Strategy | Verification Method |
| :--- | :--- | :--- | :--- | :--- |
| **SEC-01** | Telemetry PII Leakage | User Privacy | Strict payload schema. No `Environment.UserName`, IP addresses, MAC addresses, or device IDs serialized. | Automated test in `TelemetryTests.cs` verifying payload properties. |
| **SEC-02** | Save File Corruption | Game Progress | Defensive JSON deserializer. Malformed files catch exceptions, log a warning, and fall back to clean new game state. | Automated test in `SaveCorruptionTests.cs`. |
| **SEC-03** | Save State Injection | Narrative Integrity | Sanitized deserialization. Fragment IDs validated against known definitions; invalid IDs silently ignored. | `SaveLoadTests.cs`. |
| **SEC-04** | Input Spam / State Corruption | Game FSM | Rate-limiting cooldown on `RecallAbility` prevents rapid raycast spamming. | Automated test in `RecallAbilityTests.cs`. |
| **SEC-05** | Credential Exposure in Git | Repository Security | Strict `.gitignore` covering `.env`, `*.secret`, `*.key`, and machine-specific IDE directories. | Git pre-commit checks and regular repository scans. |
| **SEC-06** | Dependency Supply Chain | Runtime Safety | Package additions restricted to official Unity Registry. Every dependency addition requires an explicit ADR. | ADR governance and `Packages/manifest.json` lock. |

---

## 3. Threat Model Adjustments & Residual Risk Acceptance

### 3.1 Save File Tampering (ADR-0006)
- **Original Spec:** Cryptographic SHA-256 checksums were originally considered to detect save-file tampering.
- **Architectural Adjustment:** Formally revoked by **ADR-0006**. In a single-player offline narrative game, the user editing their own save file is not an adversary. Enforcing checksums added unnecessary complexity and caused legitimate player progress loss.
- **Residual Risk:** Accepted. A player who hand-edits `save_01.json` can unlock fragments or alter flags; this poses no security or financial risk to any system.

### 3.2 Offline Telemetry
- Telemetry events are appended solely to a local file (`Application.persistentDataPath/telemetry_session.json`).
- No socket, HTTP client, or external API call exists in the codebase.
- The player is free to inspect or delete this file at any time without affecting gameplay.
