# Technology stack and version policy

Status: proposed application stack; documentation tooling is the only implemented tooling.

| Technology | Purpose / placement | Version and dependency policy |
| --- | --- | --- |
| C# / .NET | Desktop/runtime and BCL | Plan net10.0; installed SDK observed 10.0.402 on 2026-11-20 |
| Windows Forms | Separate Server/Client apps and shared Presentation | net10.0-windows, programmatic native controls; no Designer dependency |
| TcpListener / TcpClient | Accept/connect a TCP peer | Built-in System.Net.Sockets; no networking NuGet package |
| NetworkStream + async/await | Cancellable framed reads/writes | Built-in async APIs; one reader/serialized writer per connection |
| System.Text.Json | Versioned packet payloads | Built-in UTF-8 JSON; bounded framing is specified separately |
| BinaryPrimitives / Encoding | Big-endian length and strict UTF-8 | BCL; reject malformed lengths/encoding |
| CancellationToken / SemaphoreSlim | Lifetime, deadlines and write ordering | BCL; no blocking UI waits |
| TimeProvider / DateTimeOffset | Testable server UTC timestamps | App/Networking supplies time; Core stores values |
| xUnit + Microsoft.NET.Test.Sdk | Planned Core/transport/native UI tests | Pin exact stable package versions during P00-003 after restore/audit; none installed now |
| PowerShell | Current documentation validation; future dev commands | Windows PowerShell 5.1-compatible scripts, pwsh in CI |
| Git / GitHub Actions | Reviewed branches and repeatable checks | Current docs CI; Windows .NET CI added in Phase 00 |
| VS Code / C# tooling | Owner's editing/debugging workflow | Shared configs planned; install extensions deliberately |

No database, ORM, web framework, SignalR, browser engine, cloud API, external UI toolkit or account service is needed for the baseline. Do not install packages merely to reserve architecture.

## SDK and package reproducibility

Phase 00 will create global.json selecting 10.0.402, latestPatch and allowPrerelease=false after reconfirming availability. LatestPatch stays within the same SDK feature band; SDK selection is separate from target frameworks. See [Microsoft SDK policy](https://learn.microsoft.com/en-us/dotnet/core/tools/global-json).

Use explicit test package versions and official NuGet package/audit sources. Enable nullable references, warnings as errors, deterministic builds and all-dependency advisory checking. Record exact restored versions and results; do not claim proposed packages are installed or vulnerability-free.

## UI, concurrency and transport

[Windows Forms](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/overview/) provides the required native desktop surface. Shared presentation uses native controls/original simple drawing, with fonts/resources owned and disposed.

Transport runs with async I/O rather than wrapping blocking socket calls in Task.Run. Network callbacks never receive controls. Forms await UI-thread dispatch via [Control.InvokeAsync guidance](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/controls/how-to-make-thread-safe-calls), guard closing/disposal and cancel their lifetime before releasing resources.

TCP helpers simplify stream access; they do not define application messages. Our [protocol](../architecture/PROTOCOL.md) supplies framing, limits and handshake rules. See [Microsoft TCP overview](https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/sockets/tcp-classes).

## Current evidence and deferred decisions

Observed SDK/Git availability is recorded in [DEVELOPMENT](../history/DEVELOPMENT.md). There is no application build, dependency restore, behavior test or F5 result yet.

Open implementation details: exact test package pins, measured message-control performance and actual DPI layouts. Resolve them in the listed phase/task, update decisions and preserve the owner review gate.
