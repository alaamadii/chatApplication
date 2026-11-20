# Environment and command contract

## What is available now

The repository currently contains docs/design and scripts/check-docs.ps1 only. On 2026-11-20, the development computer reported .NET SDK 10.0.402 and Git 2.53.0.windows.1. That observation is not an application build or second-computer test.

Clone the current branch while its PR is unmerged:

```powershell
git clone https://github.com/alaamadii/chatApplication.git
cd chatApplication
git switch docs/project-foundation
./scripts/check-docs.ps1
```

PowerShell 5.1 or newer and Git are enough for documentation checks; they do not restore .NET packages or open network listeners. GitHub CI uses pwsh on Windows.

## Planned Phase 00 environment

Windows with a compatible .NET 10 SDK/WindowsDesktop runtime, Git and VS Code. Reconfirm SDK 10.0.402, then pin stable servicing within its feature band using global.json. First restore/audit needs official NuGet connectivity.

Create a .slnx solution and independent project references. Add nullable references/warnings-as-errors/deterministic builds, exact test packages and NuGet audit policy after actual restore verification. No database, API keys, workloads or third-party UI framework are required by the baseline.

Shared VS Code configurations will provide build/check tasks, separate Server/Client launches and a compound debugging option. Do not claim F5 works until physically executed. Keep personal editor settings out of Git.

## Planned script contract — not runnable yet

Phase 00 creates scripts/dev.ps1 with:

| Command | Future behavior |
| --- | --- |
| doctor | Selected SDK/runtime/Git and meaningful prerequisites |
| restore | Restore solution using recorded sources |
| build | Release build |
| test | Meaningful tests/TRX when they exist; honest zero discovery |
| check | Restore, build, test, format and docs check |
| run-server | Build/launch Server executable |
| run-client | Build/launch Client executable |

Resolve dotnet from PATH; temporarily use matching DOTNET_ROOT and ignored .local CLI state, then restore inherited values/location. Script errors must fail CI. Do not create tracked machine-specific configuration.

## Later loopback/LAN procedure

After transport/messages exist: start Server on 127.0.0.1:5000, enter client name/address/port, connect, then send synthetic text. Port conflicts must be a readable recoverable error.

LAN tests use deliberate literal IPv4 bind/peer addresses on a trusted network. Follow the machine's existing policy; do not weaken execution policy or add firewall/router rules automatically.

## Troubleshooting boundaries

| Situation | Correct interpretation |
| --- | --- |
| No solution/dev.ps1/F5 configuration | Expected in documentation preparation; implement P00 first |
| Design asset shows Connected | Illustrative future state, not working network behavior |
| Docs check passes | Links/tokens/proposal structure pass, not application coverage |
| Future SDK/restore unavailable | Report actual version/feed/error; never commit credentials or fake results |
| Future remote connection fails | Check endpoint/listener/network policy explicitly; no hidden auto-retry |

See [stack](../technology/STACK.md), [workflow](../planning/WORKFLOW.md) and [tasks](../planning/TASKS.md).
