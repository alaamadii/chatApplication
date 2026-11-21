# Task ledger

Status vocabulary: Done = listed artifact/check exists; In progress = active work; Pending = not started; Deferred = not selected/authorized. Done is not owner phase acceptance.

IDs are stable references used by REQUIREMENTS. Dependencies must be satisfied or explicitly deferred; record exact command/commit evidence in TESTING/DEVELOPMENT/PR rather than inventing results here.

## Documentation preparation

| ID | Task | Depends on | Evidence / acceptance | Status |
| --- | --- | --- | --- | --- |
| DOC-001 | Read course/owner scope and map requirements | None | product/REQUIREMENTS and references | Done |
| DOC-002 | Document stack, architecture and protocol decisions | DOC-001 | technology/STACK and architecture documents | Done |
| DOC-003 | Specify frontend files, controls, palette and two wireframes | DOC-002 | frontend documents/assets/tokens | Done |
| DOC-004 | Separate milestone plan and actionable backlog | DOC-001 | PLAN and this ledger | Done |
| DOC-005 | Validate docs/design metadata, publish reviewed foundation | DOC-002, DOC-003, DOC-004 | Actual local/hosted checks and PR receipt | Done |

## P00 — Initialization

| ID | Task | Depends on | Evidence / acceptance | Status |
| --- | --- | --- | --- | --- |
| P00-001 | Create Server/Client/Core solution and explicit references | DOC-005 + owner review | Four-project solution, correct references/targets, Release build; TESTING | Done |
| P00-002 | Add truthful initialization windows, dev script, VS Code and Windows CI | P00-001 | doctor/check, actual EXE start/close, shared configs; hosted checks passed, exact receipt in PR #2 | Done |
| P00-003 | Pin/audit test packages; prepare meaningful test infrastructure | P00-001 | STACK pins; restore/advisory query; zero cases, no placeholders; TESTING | Done |

## P01 — Interfaces

| ID | Task | Depends on | Evidence / acceptance | Status |
| --- | --- | --- | --- | --- |
| P01-001 | Shared theme/font ownership and Server role layout | P00-002, P00-003 + owner review | Presentation/Server layout, disabled actions, resource lifetime assertions; TESTING | Done |
| P01-002 | Client identity/endpoint/transcript/composer layout | P01-001 | Client layout, accessible native fields, empty transcript and guidance; TESTING | Done |
| P01-003 | Review navigation, minimum/wide/DPI and long text | P01-001, P01-002 | 15 native UI cases; actual EXE/visual review at 125%; CI receipt pending; physical checks remain owner actions | In progress |

## P02 — Transport and connection

| ID | Task | Depends on | Evidence / acceptance | Status |
| --- | --- | --- | --- | --- |
| P02-001 | Implement bounded codec and hello/welcome contract | P01-003 + owner review | Boundary/Unicode/split/coalesced/truncated/direction cases | Pending |
| P02-002 | Add ChatServer/ClientSession with one-client admission | P02-001 | Owned listener/session/task/cancellation; no leaked accept loop | Pending |
| P02-003 | Add ChatClient username handshake and real connect/disconnect UI | P02-001, P02-002 | Connected only after welcome; canceled/failed connect safe | Pending |
| P02-004 | Prove async ownership, stop/cancel and loopback lifecycle | P02-002, P02-003 | Bounded ephemeral-port integration; repeated cleanup; UI remains responsive | Pending |

## P03 — Messaging

| ID | Task | Depends on | Evidence / acceptance | Status |
| --- | --- | --- | --- | --- |
| P03-001 | Add immutable messages, text rules and server stamping/dispatch | P02-004 + owner review | Correct author/time/order and serialized frames | Pending |
| P03-002 | Bind one rendering path and awaited UI dispatch for both roles | P03-001 | No local-copy/echo duplicates or disposed-control callbacks | Pending |
| P03-003 | Verify actual two-process Unicode/multiline bidirectional exchange | P03-002 | One accepted send/one visible message; timestamps and input retention | Pending |

## P04 — Peer information and robustness

| ID | Task | Depends on | Evidence / acceptance | Status |
| --- | --- | --- | --- | --- |
| P04-001 | Harden disconnect/abort/reconnect and state/action recovery | P03-003 + owner review | Repeated lifecycle and readable recoverable failures | Pending |
| P04-002 | Verify close during I/O/dispatch, deadlines and bounded transcript | P04-001 | Owned cleanup, trim notice, no hangs/late UI updates | Pending |
| P04-003 | Complete server client-info list and remove stale entries | P04-001 | Username/endpoint/time/state reflect the live accepted session | Pending |
| P04-004 | Exercise deliberate trusted-LAN setup | P04-001, P04-003 | Actual addresses/date/commit/results; firewall changes not automated | Pending |

## P05 — Optional owner decisions

| ID | Task | Depends on | Evidence / acceptance | Status |
| --- | --- | --- | --- | --- |
| P05-001 | Decide/implement multi-client room only if selected | P04 acceptance + explicit choice | Revised admission/order/fan-out tests and owner review | Deferred |
| P05-002 | Decide/implement private messages only if selected | Explicit choice and routing design | Recipient/delivery validation and no disclosure to other peers | Deferred |
| P05-003 | Decide/implement local history only if selected | Explicit choice and data policy | Save/cancel/permissions/error/encoding tests; no silent autosave | Deferred |

## P06 — Final QA

| ID | Task | Depends on | Evidence / acceptance | Status |
| --- | --- | --- | --- | --- |
| P06-001 | Audit requirements/source, versions, setup and clean published checkout | P04 accepted; selected P05 work reviewed | Exact commit/doctor/check/run/close/current CI and limitations | Pending |
| P06-002 | Capture genuine Server/Client images and reconcile guides | P06-001 | Two authentic images, provenance/hashes and owner approval | Pending |
| P06-003 | Record physical UI and second-computer results | P06-001 | Date, Windows/SDK, commit, actual commands/outcomes | Pending |
| P06-004 | Owner final PR acceptance and GitHub-profile submission | P06-002, P06-003 | Owner decision; no assistant merge/Classroom submission | Pending |

## Update rule

When a task changes, add evidence and update the phase gate in PLAN. P00 is accepted through owner merge of PR #2. P01 artifacts/checks may be Done while owner acceptance remains pending. P01-003 awaits hosted verification; physical checks remain explicit owner actions. Optional tasks remain Deferred until separately chosen; they do not quietly become mandatory.
