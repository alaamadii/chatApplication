# YMH219 : Chat Application

A planned C#/.NET Windows Forms client-server chat assignment for **Nesne Tabanlı Programlama (YMH219)**, by **Alaa Madi**.

**Current status: documentation/design foundation, awaiting owner review.** There is no application source, executable, connection, message exchange or application test suite yet. The SVG previews are original design proposals, not screenshots of working software.

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
| [Frontend file guide](docs/frontend/FILE_GUIDE.md) | Planned UI files, purpose and creation phase |
| [Plan](docs/planning/PLAN.md) | Phase order, deliverables and review gates |
| [Tasks](docs/planning/TASKS.md) | Separate actionable backlog with dependencies/status |
| [Workflow](docs/planning/WORKFLOW.md) | Branches, commits, checks and owner review |
| [Testing](docs/quality/TESTING.md) | Current documentation checks and future test matrix |
| [Security](docs/quality/SECURITY.md) | Trust boundaries, data handling and limitations |
| [Environment](docs/setup/ENVIRONMENT.md) | Current prerequisites and future run/debug commands |
| [Submission](docs/submission/CHECKLIST.md) | Final evidence and owner actions |

## Design previews

[Client design proposal](docs/frontend/assets/client-wireframe.svg) and [Server design proposal](docs/frontend/assets/server-wireframe.svg) show the intended light desktop layout. Their illustrative conversation is synthetic. The palette and control descriptions are reviewable now; application implementation follows later.

## Planned technology

C# / .NET 10, Windows Forms, TcpListener/TcpClient, NetworkStream, async/await and System.Text.Json. Separate Core, Networking and UI ownership; no database, hosted service, web frontend or external UI framework is required. The installed SDK observed on 2026-11-20 is 10.0.402; Phase 00 will pin and verify the actual application toolchain.

## What runs now

From PowerShell in this repository:

```powershell
./scripts/check-docs.ps1
```

This checks documentation structure/local links, task references, SVG proposal labeling and the specified text-color contrast pairs. Documentation CI runs the same command. These checks are not chat functionality tests or a complete accessibility audit.

There is no scripts/dev.ps1, .NET solution, F5 launch configuration or application run command yet. Their planned contract is in ENVIRONMENT; no setup step is presented as working before it exists.

## Screenshots, identity and submission

Actual application screenshots are pending. Final delivery needs at least two genuine Server/Client captures; wireframes do not satisfy that requirement.

The selected public repository is [alaamadii/chatApplication](https://github.com/alaamadii/chatApplication). The instructor recommends YMH219-ChatApp; the owner's selected name is retained. The course deadline is **2026-11-30, 23:59**, with institutional timezone unspecified.

The student number is omitted by owner direction; the sheet's original requirement is recorded separately and no waiver is claimed. The owner submits their GitHub profile link through Classroom. No license is selected automatically.

See [decisions](docs/architecture/DECISIONS.md), [dated development evidence](docs/history/DEVELOPMENT.md), [sources](docs/references/SOURCES.md) and [working agreement](AGENTS.md).
