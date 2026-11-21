# YMH219 : Chat Application

A C#/.NET Windows Forms client-server chat assignment for **Nesne Tabanlı Programlama (YMH219)**, by **Alaa Madi**.

**Current status: P01 interfaces implemented in [PR #3](https://github.com/alaamadii/chatApplication/pull/3); owner review pending.** Initialization [PR #2](https://github.com/alaamadii/chatApplication/pull/2) is merged. Separate Server and Client windows now provide editable connection settings, a shared conversation layout and explicit unavailable actions. Networking, username acceptance, messaging and participant tracking are not implemented. SVG wireframes remain design proposals.

## Idea and required outcome

Run two desktop programs: a Server console and a Client. The client supplies a username, connects over TCP, exchanges timestamped text with the server operator and disconnects explicitly. The server shows its connected client's basic information. Failures must be reported without crashing or blocking the interface.

The mandatory baseline is **one server and at least one client** on the same computer or trusted LAN. Multiple-client rooms, private messages and saved history are optional; they remain behind an owner decision after the baseline works.

## Start with the documentation

| Read | Purpose |
| --- | --- |
| [Documentation index](docs/README.md) | Ordered guide to every document and its owner |
| [Product overview](docs/product/OVERVIEW.md) | Users, scope and complete demonstration journey |
| [Requirements](docs/product/REQUIREMENTS.md) | Course criteria, acceptance and traceability |
| [Technology stack](docs/technology/STACK.md) | Frameworks, versions, dependencies and rationale |
| [Architecture](docs/architecture/ARCHITECTURE.md) | Project boundaries, class responsibilities and lifecycle |
| [Protocol](docs/architecture/PROTOCOL.md) | Proposed framing, packet contract, limits and message flow |
| [Frontend UX](docs/frontend/UX.md) | Server/client layouts, controls, states and interactions |
| [Design system](docs/frontend/DESIGN_SYSTEM.md) | Exact colors, typography, spacing and accessibility |
| [Frontend file guide](docs/frontend/FILE_GUIDE.md) | Existing initialization files and planned UI responsibilities |
| [Plan](docs/planning/PLAN.md) | Phase order, deliverables and review gates |
| [Tasks](docs/planning/TASKS.md) | Separate actionable backlog with dependencies/status |
| [Workflow](docs/planning/WORKFLOW.md) | Branches, commits, checks and owner review |
| [Testing](docs/quality/TESTING.md) | Executed P00 checks and future test matrix |
| [Security](docs/quality/SECURITY.md) | Trust boundaries, data handling and limitations |
| [Environment](docs/setup/ENVIRONMENT.md) | Prerequisites, build/run/debug commands and troubleshooting |
| [Submission](docs/submission/CHECKLIST.md) | Final evidence and owner actions |

## Current interfaces

| Window | Available presentation | Unavailable actions |
| --- | --- | --- |
| Server | Editable loopback bind/port, empty client-information list, empty conversation and guidance | Start, Stop, composer and Send |
| Client | Empty username, editable loopback address/port, empty conversation and guidance | Connect, Disconnect, composer and Send |

Inputs only edit local controls. No port is opened, no username accepted and no messages or client rows are created. Connection actions arrive in P02; messaging arrives in P03.

## Design previews

[Client design proposal](docs/frontend/assets/client-wireframe.svg) and [Server design proposal](docs/frontend/assets/server-wireframe.svg) show the intended light desktop layout. Their illustrative conversation is synthetic. P01 implements the light sidebar/conversation layout with empty states. Connected-state badges, conversation examples and message cards in the wireframes remain future designs.

## Technology

SDK 10.0.402 with stable latestPatch selection; C#/.NET 10 and native Windows Forms. Server and Client target net10.0-windows and reference a behavior-free net10.0 Core. Exact test tooling pins are documented in [STACK](docs/technology/STACK.md). Shared Presentation now owns native layout/theme/fonts. Networking will be introduced in P02. TCP/async messaging is planned; no database, cloud service or external UI framework is required.

## What runs now

From PowerShell in this repository:

```powershell
./scripts/dev.ps1 doctor
./scripts/dev.ps1 check
./scripts/dev.ps1 run-server
```

In another PowerShell terminal, run:

```powershell
./scripts/dev.ps1 run-client
```

Use Windows with a compatible .NET 10 SDK/WindowsDesktop runtime. The check command restores, builds Release, discovers tests, verifies formatting and runs the existing documentation checker. Core has no domain behavior and still discovers **zero cases**. App.Tests separately runs **15 native UI cases** covering layout, scrolling, unavailable actions, Tab traversal and window/font lifetime. These are presentation checks, not chat/transport coverage.

Both executables use STA, PerMonitorV2, visual styles and native resizable layouts. The shared canvas supports a 1120 x 720 logical default, 900 x 620 design minimum, working-area bounds and scrolling for smaller displays/expanded guidance. Actual EXE startup/disabled-action/normal-close checks passed; captured default/minimum/wide layouts were visually reviewed at the existing 125% display setting. Shared VS Code role/compound launches remain available; physical F5, keyboard/assistive review, 100%/150% and monitor transitions remain pending. Windows .NET CI runs the same development check alongside preserved docs CI. See [TESTING](docs/quality/TESTING.md) for exact evidence and [ENVIRONMENT](docs/setup/ENVIRONMENT.md) for setup.

## Screenshots, identity and submission

Final submission screenshots remain pending. Local P01 window captures were used for review and remain ignored; final delivery needs at least two genuine captures of the completed application with owner approval. Wireframes do not satisfy that requirement.

The selected public repository is [alaamadii/chatApplication](https://github.com/alaamadii/chatApplication). The instructor recommends YMH219-ChatApp; the owner's selected name is retained. The course deadline is **2026-11-30, 23:59**, with institutional timezone unspecified.

The student number is omitted by owner direction; the sheet's original requirement is recorded separately and no waiver is claimed. The owner submits their GitHub profile link through Classroom. No license is selected automatically.

See [decisions](docs/architecture/DECISIONS.md), [dated development evidence](docs/history/DEVELOPMENT.md), [sources](docs/references/SOURCES.md) and [working agreement](AGENTS.md).
