# Design decisions

All application decisions are proposed until implemented/validated in the named phase.

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

Open decisions: exact test package pins in P00-003; performance/assistive review of message cards in P01/P03; optional feature selection in P05. These are not blockers to producing this reviewable documentation proposal.

See [architecture](ARCHITECTURE.md), [protocol](PROTOCOL.md), [stack](../technology/STACK.md) and [tasks](../planning/TASKS.md).
