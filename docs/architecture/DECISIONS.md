# Design decisions

Decisions are proposals until implemented/validated in their named phase. P00 implements the initialization subset. P01 implements shared native presentation in ADR-014/015. P02 implements the connection protocol and lifetime/UI boundary in ADR-016/017/018. P03 implements authoritative messages, pending acceptance and native message presentation in ADR-019/020/021.

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

## ADR-016 — Implement only the connection protocol subset — 2026-11-21

Implemented Core UsernameRules plus UI-free Networking for hello/welcome/error/disconnect. Keep strict four-byte big-endian framing, 16 KiB allocation cap, strict UTF-8/JSON/depth/duplicate validation and phase-direction checks. chat/message remain unsupported until P03. Server revalidates names; Client validates the welcome name/session before Connected.

## ADR-017 — Bounded ownership and one admission slot — 2026-11-21

Server owns one accept task and one pending/accepted peer task; busy rejection runs serially with a bounded initial-frame drain so a normal client's outgoing hello does not cause reset/discard of server_busy. No per-arrival fire-and-forget task. PeerCompletion/Completion expose owned work and are joined on shutdown.

ConnectionOptions implements connect/handshake/send/shutdown 5-second defaults and a 10-second frame deadline starting after the first byte. Idle accepted reads do not expire. Cancel/close acts on the underlying socket/stream before joining. Shutdown failure retains observable work and is not reported as successful cleanup; awaited observers must honor cancellation.

## ADR-018 — Real state projection with guarded UI close — 2026-11-21

Immutable snapshots and serialized awaited observers contain no controls. Apps use cancellable InvokeAsync with closing guards and own command Tasks; framework async-void event handlers await handled operations. Normal close aborts I/O, joins commands, awaits service disposal, then disposes controls/fonts. Presentation remains independent of Networking/Core.

Enable only real connection actions; retain disabled composer/Send and empty transcript. Show accepted name/state without claiming P04 detailed metadata completion. Local friendly messages map controlled codes, never arbitrary peer strings. Native tests were updated where P01 all-disabled/Tab assumptions became obsolete, while layout/resource regressions remain.

## ADR-019 — Authoritative ordered messaging — 2026-11-21

P03 implements ADR-005/006 message contracts with immutable validated ChatMessage and pure MessageRules. Core does not generate time/IDs. ClientSession derives author, stamps once using injectable TimeProvider and serializes peer/operator write plus local notification through one dispatch gate. Client renders only the server echo. Equal text with distinct GUIDs remains distinct. Strict framing, duplicate-key/depth/direction/deadline protections remain.

## ADR-020 — One pending submission and explicit uncertain failure — 2026-11-21

Allow one client submission, bound to generation/session/text, and reject parallel sends instead of queuing. Confirm after awaited display of the matching own echo. Unrelated Server messages do not confirm it. Cancel/fault pending work on close; uncertain write/acceptance failure cancels that captured lifetime/socket and joins its reader. No retry/reconnect/replay is automated. Register send ownership under the lifecycle gate, perform I/O outside it and never make a reader await its pending sender.

Recent receive IDs and visible-card IDs are each bounded to 500. This prevents recent duplicate rows without storing an unbounded replay ledger or claiming exactly-once delivery across sessions.

## ADR-021 — Native bounded message view and composer — 2026-11-21

Presentation now references Core because it renders actual immutable values, superseding ADR-018's P02 no-Core-reference boundary. It remains independent of Networking. Separate literal author/time/body labels expose full local date/offset, use per-window fonts and apply incoming/outgoing tokens. Keep latest 500 cards; remove/dispose older controls and show a persistent notice. Reconnect keeps this bounded view; close releases it.

One guarded UI send path serves button/Enter, locks composition during acceptance, counts UTF-16 units and clears only a confirmed unchanged draft. Respect Shift+Enter, Tab and active IME composition. Cache width/DPI measurement to avoid remeasuring every card on each append. Tests distinguish synthetic key/native automation from physical keyboard/IME acceptance.

## ADR-022 — Accepted-session metadata stays local — 2026-11-22

Capture immutable ConnectedClientInfo after successful welcome: canonical name, existing session GUID, actual remote socket IPv4/port and server TimeProvider UTC acceptance time. Carry it in ConnectionSnapshot without changing v1 packets. Server renders one Connected row; pending/busy/rejected peers have none. Clear live metadata on peer close/stop; join the old session and check callback source identity before replacement.

Use native ListView columns measured from current font/DPI, horizontal scrolling, tooltip and complete accessible description. The accepted row's Connected state is distinct from the listener's Listening state. No historical participant list or verified identity claim is added.

## ADR-023 — Join terminal work and preserve failure guidance — 2026-11-22

Regression tests reproduced Start/Connect rejection while final terminal callbacks were still finishing, and a Disconnecting observer failure that left the reader active. Explicit Start/Connect now join previous lifetime/send ownership before replacing fields, with the existing shutdown budget. Any Stop/Disconnect exception still aborts owned I/O. A simultaneous stop/disconnect run also exposed cancellation escaping the final peer notification; suppress it only when the server lifetime actually requested cancellation.

UI command busy guards prevent repeated command queues and allow cleanup retry after failure. Keep unconfirmed-send guidance separately from current connection guidance until a successful confirmed send, so later snapshots/field edits cannot erase it. No automatic reconnect/resend is authorized.

## ADR-024 — Batched native layout and unambiguous dates — 2026-11-22

A fully visible 512-message burst passed alone but exceeded the existing 20-second native fixture deadline during a full run. Keep that deadline: use explicit scrolling-canvas bounds and suspend/resume append/trim layout instead of repeatedly docking all retained cards. Preserve native labels, 500-card/ID bounds, immediate removed-card disposal and scroll access. The visible regression covers burst, trim, resize and close; it is a synthetic local bound, not performance certification.

Actual separate EXEs on an Arabic Windows locale displayed a Hijri year inside yyyy-MM-dd. Format fixed numeric local dates/times with invariant culture, preserving local timezone/offset. Verify the metadata row under ar-SA and keep physical input/accessibility/DPI checks separate.
