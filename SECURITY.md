# SECURITY.md — ECHO//ZERO

> Security constraints for this project. Applies to all code, configuration, and agents.

---

## 1. What Needs Protecting

This is a single-player offline game. The threat surface is narrow but real:

| Asset | Risk |
|-------|------|
| Save files | Tampering to unlock story content or exploit progression |
| Telemetry data | Privacy — must contain no PII |
| Project repository | Accidental secret leakage via commits |
| Development machine | Agent behaviour constraints |

---

## 2. Save File Integrity

Every save file is accompanied by a SHA-256 checksum file.

**Write path**:
1. Serialize `GameSaveData` to JSON string
2. Compute SHA-256 of the JSON string (UTF-8 bytes)
3. Write JSON to `save_01.json`
4. Write hex checksum to `save_01.checksum`

**Read path**:
1. Load `save_01.json` and `save_01.checksum`
2. Recompute SHA-256 of loaded JSON bytes
3. If checksum mismatch: log warning, do NOT load, offer new game
4. If checksum missing: treat as corrupted

Note: this prevents casual tampering, not determined cheating. A determined player can still
regenerate a valid checksum. This is accepted risk for a single-player game — the goal is
detecting accidental corruption, not preventing all tampering.

---

## 3. No Hardcoded Credentials

Absolute rule: no API keys, tokens, passwords, or secrets anywhere in the repository.

If a future phase requires an external service:
1. Write an ADR before integrating the service
2. Use environment variables or a local secrets file excluded from `.gitignore`
3. Document the required environment variable in `DEVELOPMENT.md`

---

## 4. Telemetry Privacy

Phase 3 telemetry is local-only (JSONL file on disk, no network calls).

Telemetry events MUST NOT contain:
- Player name, email, or any identifier
- Machine name or username
- IP address or location
- Any data that could identify the player

Permitted telemetry fields:
- `session_id` (random UUID generated per session, not tied to identity)
- `timestamp` (UTC ISO-8601)
- `event_type` (string enum)
- `game_state_snapshot` (fragment IDs, flags — no PII)

---

## 5. Agent Security Constraints

AI coding agents MUST NOT:
- Read or write files outside the project workspace
- Execute network requests from within Unity runtime without an ADR
- Commit `.env`, `*.key`, `*.pem`, `*.secret` files
- Add dependencies from unverified or unsigned package sources

The `.gitignore` MUST exclude:
```
.env
*.secret
*.key
*.pem
/Secrets/
Library/
Temp/
Logs/
UserSettings/
```

---

## 6. Dependency Security

Before adding any Unity package or NuGet dependency:
1. Check the package source (Unity Package Manager official registry preferred)
2. Check last update date — abandoned packages are a risk
3. Check for known CVEs if the package handles user data
4. Write an ADR entry

---

## 7. Build Security

- IL2CPP builds strip managed code — reduces reverse-engineering risk
- Do NOT include editor-only scripts in release builds
- Verify no `Debug.Log` calls leak sensitive data in release builds

---

*Last updated: Phase 2 — Foundation.*
*Next review: Phase 9 — Red Team.*
