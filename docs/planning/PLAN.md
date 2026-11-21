# Phased delivery plan

PLAN owns sequence, deliverables and review gates. [TASKS](TASKS.md) owns individual actions, dependencies/status and acceptance evidence. Do not use the two files as duplicated checklists.

## Current delivery

PR #3 was merged by the owner before P02. Real TCP connection/username handshake, bounded framing and owned async cleanup are now implemented and locally exercised with actual separate executables. Fixed implementation publication and both hosted workflows have passed in [PR #4](https://github.com/alaamadii/chatApplication/pull/4); owner P02 acceptance remains pending. Physical UI/F5/assistive/LAN checks carry forward; no message exchange or detailed participant-list completion is claimed.

| Phase | Deliverable | Depends on | Current state |
| --- | --- | --- | --- |
| Preparation | Organized docs, frontend proposals, architecture/protocol and backlog | Owner's current request | PR #1 merged by owner; foundation reviewed |
| P00 | Solution, initialization windows, environment and .NET CI | Foundation reviewed/requested | PR #2 merged by owner |
| P01 | Server/Client interfaces and shared presentation | P00 accepted | PR #3 merged by owner |
| P02 | Bounded TCP protocol and real connection/username lifecycle | P01 accepted | Implemented; owner review pending |
| P03 | Real-time timestamped messaging and UI integration | P02 accepted | Pending |
| P04 | Client information, disconnect/error/retry and resource regression | P03 accepted | Pending |
| P05 | Separately selected optional features | Mandatory baseline accepted and explicit owner choice | Deferred |
| P06 | Final QA, actual screenshots, clean checkout and submission review | P04 accepted; selected optional work reviewed | Pending |

The order is a dependency plan, not invented calendar history. Course deadline: 2026-11-30 at 23:59; institutional timezone unspecified. Prioritize mandatory behavior before optional work.

## P00 — Initialization

Deliver separate Server/Client WinForms executables, independent Core and prepared Core test infrastructure. Pin/reconfirm SDK, references, nullability/warnings, official package/audit sources, narrow exclusions, development script, VS Code build/debug and Windows CI.

Windows open/close with truthful initialization text; no working connection controls or fake sample messages. Add Networking/Presentation when later phases need real code, not empty placeholder classes now.

Validate documented doctor/check commands, actual two executable launches/normal close, references/output exclusions and exact-head CI. Zero behavioral tests are acceptable only when no behavior exists and must be reported honestly.

Gate: owner reviews setup, scaffold and actual windows before P01.

## P01 — Native interfaces

Build both role layouts, shared theme/fonts/status components, empty transcript and composer, validation presentation and state/action matrix. Network/send actions remain disabled and explicitly Not implemented.

Validate native bounds/navigation/mnemonics, default/minimum/wide layout, long input/error text and supported physical DPI/high-contrast checks where available. Mockups do not replace actual UI review.

Gate: owner reviews the real appearance against UX/tokens/file guide.

## P02 — Protocol and connections

Create Networking when needed. Implement framing/validation, hello/welcome, one-client admission, server Start/Stop, client Connect/Cancel/Disconnect and minimum cancellation/disposal from the beginning. Connect UI actions to actual transport; Send remains unavailable until P03.

Validate split/coalesced/truncated frames, limits/UTF-8/invalid directions, username acceptance, failed endpoints, port conflicts, clean EOF, cancellation and repeat connect/stop on ephemeral loopback ports with bounded waits.

Gate: real Server/Client instances connect with an accepted username and close cleanly. No claim of message exchange yet.

## P03 — Messaging

Implement immutable message values/validation, authoritative server ID/time/author and a single client echo-rendering path. Integrate server operator replies, UI-thread dispatch, multiline/Unicode text and send state. No optimistic duplicate rows or automatic uncertain resend.

Validate both directions through actual processes, timestamp display, repeated identical intentional messages, send failure retention and responsive close/callback handling. Keep transcript bounded.

Gate: owner observes one visible message per accepted send, meaningful timestamps and safe nonblocking behavior.

## P04 — Required robustness and peer information

Finish the server's client list/metadata and its lifecycle. Harden abrupt/normal disconnect, retry/reconnect, stopping while accept/read/write/dispatch is active, closing Forms and controlled error feedback. Verify limits and transcript trim notice.

Use controlled loopback and deliberate trusted-LAN tests; no automatic firewall/router changes. Add regression assertions for reproducible defects, not placeholder coverage.

Gate: owner confirms required behavior/clear recovery; mandatory requirement matrix has actual evidence. Basic cleanup was not deferred from P02.

## P05 — Optional scope gate

Multiple-client rooms, private messages and saved history are proposals, not automatic deliverables. Owner selects each explicitly after mandatory acceptance. Give approved features separate tasks/contracts/tests and a reviewed PR; keep others Deferred.

P06 can proceed with unselected optional items deferred. Do not let a speculative extra delay mandatory course readiness.

## P06 — Final evidence and submission review

Audit requirements against source/tests; reconcile exact setup/run instructions and package versions, code explanation, limitations and public repository history. Capture at least two genuine Server/Client images with source/date/method/DPI/hashes.

From an exact published commit, clone/restore/check/run and verify actual shutdown. Record physical keyboard/focus/display/F5/error tests and a second Windows computer with date, OS/SDK, commit/results. Separate automated/scripted/human evidence.

Gate: owner approves images, remaining manual evidence and final PR. Keep a draft final PR while required evidence is missing. The owner submits their GitHub profile link; no automatic merge or Classroom action.

## Completion rule

A phase is implemented only with code/docs and executed checks; it is accepted only after owner review. CI green alone does not supply human evidence. Update TASKS, requirements mapping, decisions and dated DEVELOPMENT in every phase.
