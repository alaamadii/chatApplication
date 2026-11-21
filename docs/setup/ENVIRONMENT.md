# Development environment

## Prerequisites and checkout

Use Windows, a stable .NET 10 SDK matching global.json, the .NET 10 WindowsDesktop runtime, Git and PowerShell 5.1 or newer. VS Code with the recommended C# tooling is the intended editor. Official NuGet connectivity is needed for first restore and current advisories. No database, API keys, extra workloads or machine-wide policy changes are needed.

```powershell
git clone https://github.com/alaamadii/chatApplication.git
cd chatApplication
# While the P00 PR is open:
git switch chore/project-initialization
./scripts/dev.ps1 doctor
./scripts/dev.ps1 check
```

After owner merge, use main with a fast-forward pull. On 2026-11-21 the development machine reported SDK 10.0.402, WindowsDesktop runtime 10.0.13, Windows 10.0.22631, PowerShell 5.1.22621.6133 and Git 2.53.0.windows.1. Other computers must run doctor themselves.

## Commands available now

Run from PowerShell. scripts/dev.ps1 resolves its repository root, so it also works when invoked by an absolute path from another directory.

| Command | Behavior |
| --- | --- |
| ./scripts/dev.ps1 doctor | Reports selected dotnet path/SDK/runtimes, Windows, PowerShell and Git; rejects missing WindowsDesktop 10 runtime |
| ./scripts/dev.ps1 restore | Restores all four projects from configured sources with NuGet audit |
| ./scripts/dev.ps1 build | Restore then Release build |
| ./scripts/dev.ps1 test | Restore, Release build and test discovery; TRX under ignored TestResults |
| ./scripts/dev.ps1 check | Restore, Release build, discovery, format verification and existing documentation check |
| ./scripts/dev.ps1 run-server | Restore/build then run the Server until it closes |
| ./scripts/dev.ps1 run-client | Restore/build then run the Client until it closes |
| ./scripts/check-docs.ps1 | Documentation/link/task/token/SVG validation only |

Open two terminals to use the separate programs:

```powershell
./scripts/dev.ps1 run-server
```

```powershell
./scripts/dev.ps1 run-client
```

Each shows its role and an initialization notice. Networking, usernames, connection actions, message exchange and participant tracking do not exist yet. No port is opened by either application.

The script selects the first dotnet application on PATH, temporarily sets its matching DOTNET_ROOT and repository-local DOTNET_CLI_HOME under .local, suppresses first-run ASP.NET certificate generation for the invocation, and restores inherited values and location in finally. Native tool failures throw and fail CI. Build output, CLI state and test results stay ignored; no personal environment settings are committed.

## Shared VS Code workflow

Open this repository folder. Install the deliberately recommended C# tooling from the Extensions view if needed. Run Terminal > Run Build Task for Chat: build, or select Chat: check from Run Task.

Run and Debug offers Chat Server, Chat Client and Chat Server + Client. Both launches use the Release DLL output and the shared build prelaunch task. The compound starts two separate processes and stops both when debugging ends.

Configuration files exist and their paths were inspected. Physical VS Code F5, breakpoint/debugger behavior, keyboard/focus and DPI transitions remain owner checks; scripted EXE lifecycle verification does not replace them.

## Advisory query

From the repository root:

```powershell
dotnet list ChatApplication.slnx package --vulnerable --include-transitive
```

Use doctor to verify PATH first. This query uses official package/audit sources; results depend on the advisory data available at that time. It is not a claim of complete vulnerability detection.

## Troubleshooting

| Symptom | Action |
| --- | --- |
| SDK not found or incompatible | Install a stable SDK allowed by global.json; inspect dotnet resolution/doctor |
| WindowsDesktop runtime missing | Install the .NET 10 WindowsDesktop runtime; a Core-only runtime cannot launch WinForms |
| NuGet/advisory connection fails | Check access to the official feed and its reported error; do not commit tokens or suppress failures |
| No test is available | Expected in P00: Core is empty and there are zero behavioral cases; add meaningful tests with future behavior |
| Format check fails | Run dotnet format ChatApplication.slnx, inspect the diff, then rerun check |
| Initialization notice instead of a chat screen | Expected P00 behavior; full UI is P01, TCP is P02 and messaging P03 |
| Shell blocks script execution | Use the computer's approved PowerShell policy/workflow; no automatic policy weakening |
| Later remote connection fails | After transport exists, inspect explicit endpoint/listener/network policy; no hidden retry or automatic firewall changes |

Later loopback demonstration uses 127.0.0.1:5000; trusted-LAN bind/peer addresses are deliberate future configuration. No listener, firewall/router rule or network discovery is part of P00.

See [stack](../technology/STACK.md), [workflow](../planning/WORKFLOW.md) and [testing](../quality/TESTING.md).
