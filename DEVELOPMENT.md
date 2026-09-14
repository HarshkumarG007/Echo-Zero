# DEVELOPMENT.md — ECHO//ZERO

> Step-by-step setup for a new developer or a new machine.
> Follow this exactly. If a step fails, fix the instructions before continuing.

---

## 1. Prerequisites

Install these before cloning:

| Tool | Version | Where |
|------|---------|-------|
| Unity Hub | Latest | https://unity.com/download |
| Unity 6.3 LTS | 6.3.x | Via Unity Hub → Installs → Add |
| Git | 2.40+ | https://git-scm.com |
| VS Code or Rider | Latest | Optional but recommended |
| .NET SDK | 8.0+ | https://dotnet.microsoft.com |

**Unity 6.3 LTS modules required** (select in Unity Hub):
- Windows Build Support (IL2CPP)
- WebGL Build Support (optional — Phase 10)

---

## 2. Clone and Open

```powershell
# Clone
git clone https://github.com/[YOUR_USERNAME]/echo-zero.git
cd echo-zero

# Open Unity project
# In Unity Hub: Add → Browse → select Echo/UnityProject/
# Do NOT double-click the folder — use Hub to avoid project settings issues
```

---

## 3. First-Time Unity Project Setup (TASK-001)

If the `UnityProject/` folder does not exist yet, TASK-001 has not been completed.
Follow TASK-001 in TASKS.md exactly. Do not improvise.

If `UnityProject/` exists:
1. Open in Unity Hub with Unity 6.3 LTS
2. Wait for package import (first open may take 2–5 minutes)
3. Verify: no compile errors in the Console
4. Verify: `Window → General → Test Runner` → EditMode → Run All → all pass

---

## 4. NSubstitute Setup

NSubstitute DLL must be in `Assets/Plugins/NSubstitute/`.

1. Download `NSubstitute.x.y.z.nupkg` from https://www.nuget.org/packages/NSubstitute
   (VERIFY: confirm version compatible with Unity 6.3 / .NET Standard 2.1)
2. Rename `.nupkg` to `.zip`, extract, find `lib/netstandard2.0/NSubstitute.dll`
3. Copy `NSubstitute.dll` to `UnityProject/Assets/Plugins/NSubstitute/NSubstitute.dll`
4. Also copy `Castle.Core.dll` from the same folder if required by NSubstitute
5. In Unity Editor: select `NSubstitute.dll` → Inspector → set Platform to "Editor only"
6. Run `NSubstituteVerificationTests` in Test Runner to confirm

---

## 5. Environment Variables

No environment variables are required in Phase 3. The game uses no external services.

If a future phase adds external services, create a `.env` file (excluded from git via `.gitignore`).
Document required keys in this file under a `## Required Environment Variables` section.

---

## 6. Running Tests

```
Unity Editor → Window → General → Test Runner
→ EditMode tab → Run All   (fast, ~seconds)
→ PlayMode tab → Run All   (slower, requires play mode)
```

All tests must pass before any commit. See TESTING.md for full test standards.

---

## 7. Commit Workflow

Every commit must follow the format in AGENTS.md §8:

```
feat(gameplay): implement RECALL focus trigger
Task: TASK-004
```

Before committing:
1. `git status` — confirm only expected files changed
2. Run all tests — confirm all pass
3. Update TASKS.md status if the task is complete

---

## 8. Antigravity Agent Sessions

When starting an Antigravity agent session:

1. Confirm the agent has read `AGENTS.md` — ask it to state which TASK_ID it is working on
2. Confirm the agent has stated which files it expects to change
3. After the session: review git diff before accepting any changes
4. Do not accept changes that include files outside the task's `FILES_EXPECTED_TO_CHANGE` list

**VERIFY**: Confirm whether Antigravity 2.0 reads workspace rules from `AGENTS.md` or `GEMINI.md`.
Update this section when confirmed.

---

## 9. Build (Phase 10+)

Do not build the production bundle until Phase 10.
In Phase 3–9, use `File → Build and Run` only for specific verification tasks.

When building: use IL2CPP backend, Managed Stripping Level = Minimal until NSubstitute AOT compatibility confirmed.

---

*Last updated: Phase 2 — Foundation complete.*
