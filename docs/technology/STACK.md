# Technology stack and version policy

## Implemented P04 baseline — 2026-11-21

| Technology | Current placement | Verified configuration |
| --- | --- | --- |
| C# / .NET SDK | All eight projects | SDK 10.0.402; global.json uses latestPatch and allowPrerelease=false |
| Windows Forms | Server/Client, shared Presentation and native App.Tests | net10.0-windows, UseWindowsForms=true, programmatic native controls |
| Core / Core.Tests | Independent library and test infrastructure | net10.0; pure UsernameRules, MessageRules and ChatMessage with 31 cases, no UI/network dependencies |
| Microsoft.NET.Test.Sdk | Core.Tests, Networking.Tests and App.Tests | Exact version 18.12.0 |
| xunit | Core.Tests, Networking.Tests and App.Tests | Exact version 2.9.3 |
| xunit.runner.visualstudio | All three test projects; private build/test dependency | Exact version 4.0.1 |
| PowerShell | scripts/dev.ps1 and check-docs.ps1 | Locally executed on Windows PowerShell 5.1.22621.6133; CI uses pwsh |
| Git / GitHub Actions | Reviewed phase branches and bounded Windows jobs | Git 2.53.0.windows.1; existing docs CI plus Windows .NET check |
| VS Code | Shared build/check tasks and role/compound debug configurations | C# and C# Dev Kit recommended; physical F5 not performed |

The development machine reports Windows 10.0.22631 and Microsoft.NETCore.App / Microsoft.WindowsDesktop.App 10.0.13. A compatible SDK/runtime is required on another computer; this observation is not a second-computer result.

## Reproducibility and dependency policy

[global.json](../../global.json) selects a stable 10.0.402 SDK with latestPatch servicing within its feature band. SDK selection is separate from target frameworks. [Directory.Build.props](../../Directory.Build.props) enables nullable references, implicit usings, deterministic builds, warnings as errors and NuGet auditing of all dependencies at low severity and above.

[NuGet.Config](../../NuGet.Config) clears inherited package/audit sources and uses the official nuget.org v3 feed. No credentials are stored. Test packages are exact direct version pins verified by an actual restore; transitive versions are resolved by NuGet and are not locked by a packages.lock.json. Production projects have no external PackageReference.

Official package metadata: [test SDK 18.12.0](https://www.nuget.org/packages/Microsoft.NET.Test.Sdk/18.12.0), [xUnit 2.9.3](https://www.nuget.org/packages/xunit/2.9.3) and [VS adapter 4.0.1](https://www.nuget.org/packages/xunit.runner.visualstudio/4.0.1). Restore/build/discovery succeeded with this combination. P04 executes 31 Core, 80 Networking and 28 serial native App cases, including message validation/delivery/order/pending failure and native rendering/retention. Networking/Networking.Tests target net10.0; no new package or version upgrade was needed.

On 2026-11-22, restore auditing and the explicit direct/transitive advisory query against nuget.org reported no known vulnerable packages. This is a point-in-time advisory result, not a security certification. Recheck when changing dependencies.

## Implemented transport and remaining work

TcpListener/TcpClient, NetworkStream, async/await, CancellationToken/SemaphoreSlim, System.Text.Json and BinaryPrimitives/strict Encoding are implemented using the BCL. P03 now uses Guid, TimeProvider/DateTimeOffset for authoritative server messages and native labels/panels for rendering. No production or test package upgrade was required.

Networking owns cancellable framed I/O; Core stays independent. Shared Presentation references Core for validated immutable message values; native App.Tests references both role executables and Presentation. No database, ORM, web framework, SignalR, browser engine, cloud API or external UI toolkit is required.

See [SDK selection](https://learn.microsoft.com/en-us/dotnet/core/tools/global-json), [Windows Forms](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/overview/), [thread-safe UI calls](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/controls/how-to-make-thread-safe-calls) and [TCP overview](https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/sockets/tcp-classes).

Actual commands/results and pending physical DPI/debug review are in [ENVIRONMENT](../setup/ENVIRONMENT.md) and [TESTING](../quality/TESTING.md).
