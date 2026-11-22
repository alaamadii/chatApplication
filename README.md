# YMH219 · Chat Application

A native Windows Forms client–server chat application for **Nesne Tabanlı Programlama (YMH219)**, by **Alaa Madi**.

**Current status: P04 client information and local robustness implemented; owner review and trusted-LAN verification pending.** P03 [PR #5](https://github.com/alaamadii/chatApplication/pull/5) is merged. Separate Server and Client executables exchange text through a strict TCP protocol. Final submission evidence remains P06.

## Features

- Start/Stop a loopback TCP server; connect with a validated display name and disconnect explicitly.
- View the accepted client's name, remote IP/port, local connection date/time and state. Pending/rejected connections never create a client row.
- Exchange messages with the Server operator, with server-assigned author, GUID and UTC timestamp.
- Display each accepted message once, in the same acceptance order at both windows. Two deliberate sends of identical text remain two distinct messages.
- Compose Turkish, Arabic, emoji and multiline text, up to 2,000 UTF-16 code units. Enter sends; Shift+Enter adds a line; Tab moves focus.
- Show local message times, literal plain text and the latest 500 messages per window, with a persistent notice after older entries are removed.
- Retain a draft if a send is unconfirmed. Reconnection/retry is explicit; there is no automatic resend or saved history.

The baseline supports **one pending or accepted client** plus the Server operator. Additional clients receive server_busy. Multiple-client rooms, private messaging and file history remain optional and deferred.

## Build and run

Use Windows with the .NET 10 SDK and WindowsDesktop runtime. From PowerShell in this repository:

```powershell
./scripts/dev.ps1 doctor
./scripts/dev.ps1 check
./scripts/dev.ps1 run-server
```

In a second terminal:

```powershell
./scripts/dev.ps1 run-client
```

1. Start Server on 127.0.0.1:5000, or choose another free matching port.
2. Enter a supported username in Client and select Connect. Connected appears only after the server accepts the name.
3. Send a message from Client, then reply from Server. Each window shows the author, body and local HH:mm:ss.
4. Inspect Server's Connected client list. Scroll horizontally for all columns or use the row tooltip; its accessible description includes the full local date/offset.
5. Disconnect/reconnect explicitly. The previous row disappears and a fresh accepted session supplies the next row. Stop Server or close either window to end its owned connection.

Send is available only with an accepted peer and valid nonblank text. One submission is pending at a time; the composer locks until confirmation or failure. The client clears its submitted draft after its own authoritative server echo is displayed. A different Server message cannot confirm that submission. Unconfirmed-send guidance survives later connection-state renders and input edits until a successful confirmed send. Cleanup failures preserve observable owned work and offer explicit retry; Start/Connect join a previous terminal notification before replacing its session.

TCP is **plaintext**, and display names are **not authenticated**. Use loopback or a deliberately configured trusted LAN. No firewall/router configuration is changed automatically. Completion does not promise human receipt, a read acknowledgment or exactly-once delivery across reconnect.

## Engineering and validation

SDK 10.0.402, C#/.NET 10, native Windows Forms, TcpListener/TcpClient and BCL JSON/framing. Core and Networking target net10.0; the Windows executables, shared Presentation and App tests target net10.0-windows. No production package, database, cloud service or external UI framework is required.

The development check restores, builds Release, runs **139 cases: 31 Core, 80 Networking and 28 native App**, verifies formatting and validates documentation. Real loopback tests cover authoritative messaging, accepted socket metadata, pending/busy admission, terminal-callback races, interrupted sends, repeated simultaneous stop/disconnect and cleanup-deadline failure. Native tests include a visible 512-message burst/trim/resize/close. Separate actual EXEs passed bidirectional messaging, fresh/cleared client information, explicit recovery after an intentionally terminated Client and normal Server/replacement-Client exit 0. Actual 125% captures were inspected. Physical keyboard/IME, accessibility, VS Code F5, 100%/150%, monitor transitions, LAN and another Windows computer remain pending. See [TESTING](docs/quality/TESTING.md) for exact evidence and limitations.

## Documentation

| Document | Purpose |
| --- | --- |
| [Documentation index](docs/README.md) | Ordered guide and document ownership |
| [Overview](docs/product/OVERVIEW.md) / [Requirements](docs/product/REQUIREMENTS.md) | Scope, course criteria and traceability |
| [Stack](docs/technology/STACK.md) / [Environment](docs/setup/ENVIRONMENT.md) | Tools, versions, setup, run and demo steps |
| [Architecture](docs/architecture/ARCHITECTURE.md) / [Protocol](docs/architecture/PROTOCOL.md) | Boundaries, lifecycle and exact wire contract |
| [UX](docs/frontend/UX.md) / [Design system](docs/frontend/DESIGN_SYSTEM.md) / [File guide](docs/frontend/FILE_GUIDE.md) | Controls, colors, layout, accessibility and file ownership |
| [Plan](docs/planning/PLAN.md) / [Tasks](docs/planning/TASKS.md) / [Workflow](docs/planning/WORKFLOW.md) | Phases, separate task ledger and review process |
| [Security](docs/quality/SECURITY.md) / [Testing](docs/quality/TESTING.md) | Data boundaries, actual checks and pending owner review |
| [Decisions](docs/architecture/DECISIONS.md) / [Development](docs/history/DEVELOPMENT.md) | Rationale and dated evidence |
| [Submission](docs/submission/CHECKLIST.md) / [Sources](docs/references/SOURCES.md) | Remaining submission actions and course authority |

## Screenshots and submission

[Client](docs/frontend/assets/client-wireframe.svg) and [Server](docs/frontend/assets/server-wireframe.svg) wireframes remain **design proposals with synthetic content**. Local P03/P04 captures are genuine review images, kept ignored; final submission still needs at least two owner-approved captures of the completed application.

Repository: [alaamadii/chatApplication](https://github.com/alaamadii/chatApplication). The instructor recommends YMH219-ChatApp; the owner's selected name is retained. Deadline: **2026-11-30, 23:59**, institutional timezone unspecified. The student number is omitted by owner direction, with no instructor waiver claimed. The owner submits their GitHub profile link; the assistant does not submit to Classroom or merge automatically.

See the [working agreement](AGENTS.md).
