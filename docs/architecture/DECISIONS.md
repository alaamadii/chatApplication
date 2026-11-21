# Design decisions

Decisions are proposals until implemented/validated in their named phase. P00 implements the initialization subset. P01 implements shared native presentation, shell theme/resources and layout in ADR-014/015; transport, protocol and message behavior remain proposals.

| ADR | Decision / date | Rationale and consequence |
| --- | --- | --- |
| ADR-001 | Documentation first — 2026-11-20 | Owner requested requirements/UI/tech/architecture/plan/tasks before code. This PR has no app/source/test coverage. |
| ADR-002 | Two Windows Forms executables — 2026-11-20 | Matches owner's server/client project direction; two programs demonstrate bidirectional baseline without optional group chat. |
| ADR-003 | Core / Networking / Presentation boundaries — 2026-11-20 | Keep models pure, sockets UI-free and shared visual code reusable. Create layers only when their phase needs real behavior. |
| ADR-004 | .NET 10 and BCL transport/JSON — 2026-11-20 | SDK 10.0.402 is installed. Pin verified tools in P00; no database/cloud/UI package is needed. |
| ADR-005 | Bounded length-prefixed JSON — 2026-11-20 | Explicit byte framing handles split/coalesced reads; 16 KiB/depth/input limits bound parsing. Contract is in PROTOCOL. |
| ADR-006 | Server authority / single rendering path — 2026-11-20 | Server assigns sender/time/ID; Client displays its echo rather than an optimistic duplicate. No read/exactly-once guarantee across reconnect. |
| ADR-007 | Awaited notifications and owned cancellation — 2026-11-20 | Keep native UI thread updates safe and backpressured. Observe all work; stop cancels/disposes/awaits. |
| ADR-008 | Light native design with shared tokens — 2026-11-20 | Original blue/neutral palette, clear state text, native controls and shared UI files. High contrast/DPI/keyboard review remains physical evidence. |
| ADR-009 | One client / volatile bounded transcript — 2026-11-20 | Mandatory baseline first; retain last 500 visible messages with notice. Rooms/private/history remain explicit Phase 05 choices. |
| ADR-010 | Loopback default / deliberate LAN — 2026-11-20 | Local course demo, no automatic firewall/router changes, discovery or internet deployment. TCP is plaintext and username is not authentication. |
| ADR-011 | Evidence and owner identity — 2026-11-20 | Mockups are not screenshots; no fabricated checks/dates. Alaa Madi supplied; student number omitted by owner direction without claiming waiver. |
| ADR-012 | Separate plan and task ledger — 2026-11-20 | PLAN owns strategy/gates; TASKS owns granular IDs/dependencies/status. Completion requires actual evidence and owner review. |

## Change process

Record a new dated decision or explicitly supersede an old one. Explain the concrete reason, affected requirements/files/protocol/UI, migration impact and tests needed. Do not update task status merely because a design paragraph exists.

Open decisions: performance/assistive review of message cards in P01/P03 and optional feature selection in P05. P00 test package pins are now implemented and recorded in STACK.

See [architecture](ARCHITECTURE.md), [protocol](PROTOCOL.md), [stack](../technology/STACK.md) and [tasks](../planning/TASKS.md).

## ADR-013 — P00 implementation boundary and tooling — 2026-11-21

Implemented: separate WinForms WinExe projects reference only empty Core; Core.Tests references Core with exact Microsoft.NET.Test.Sdk 18.12.0, xunit 2.9.3 and xunit.runner.visualstudio 4.0.1. No Class1, placeholder test, protocol model, Networking or Presentation project is created. This keeps initialization explainable and leaves domain assertions for actual behavior.

Both programs explicitly use STA, PerMonitorV2 and visual styles. Native labels/system fonts are owned through the Form control tree; no custom drawing/font disposal is needed. A fill-docked table wraps initialization text; resizing is allowed with a sensible minimum. Physical F5/DPI acceptance remains pending.

global.json pins stable SDK 10.0.402 with latestPatch. Shared properties enforce nullable references, deterministic builds, warnings as errors and all-dependency NuGet audit. Official sources are explicit. The development script scopes DOTNET_ROOT/CLI state to the invocation, restores inherited state and fails on nonzero tools. Windows CI runs the same check. Actual EXE open/normal-close and zero-case discovery are reported separately from behavioral/manual coverage. See STACK and TESTING for execution evidence.

## ADR-014 — Shared native interface shell — 2026-11-21

Implemented P01: both role Forms inherit a shared ChatWindow, with role Layout partials and small native layout builders. Presentation now exists because both apps use real shared code; it has no unnecessary Core reference. Apps retain Core references from P00. No Networking, message models/cards, Connection/Messaging stubs or fake state is added.

Editable local settings coexist with disabled Start/Stop/Connect/Disconnect/composer/Send. Persistent visible P02/P03 guidance explains unavailable actions. Plain labels keep ampersands literal, while field/action labels deliberately use distinct mnemonics. Native controls preserve focus and disabled behavior; no enabled-primary button is simulated.

Each window owns Segoe UI point-unit fonts and disposes controls before those fonts. Semantic theme roles use the reviewed shell palette and reapply system-color fallback on system-color changes. High-contrast/assistive acceptance remains a physical owner check.

## ADR-015 — Measured, reachable DPI layout — 2026-11-21

Suspend construction until base and role controls exist, then resume DPI layout from the 96-DPI design baseline. This avoids scaling an incomplete Form and leaving later controls at inconsistent sizes. Fit outer default/minimum bounds to the working area while retaining a scrollable design canvas for smaller displays.

Use native docking/AutoScrollMinSize with measured wrapped content. Independent sidebar scrolling and a readable empty-transcript minimum prevent clipped guidance and overlapping composer/transcript content. DPI changes refit within the destination working area. Native regressions cover supported sizes, small working areas and long literal text; actual 125% captures were reviewed. No 100%/150% or monitor-transition result is inferred.

The initial minimum-size/long-guidance checks exposed layout defects; the final 15-case native suite verifies the fixes. Follow [Microsoft automatic scaling guidance](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/forms/autoscale) and keep physical acceptance separate from scripted geometry checks.
