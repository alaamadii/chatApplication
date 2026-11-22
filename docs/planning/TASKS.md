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
| P01-003 | Review navigation, minimum/wide/DPI and long text | P01-001, P01-002 | 15 native UI cases; actual EXE/visual review at 125%; passing hosted checks in PR #3; physical owner checks remain pending | Done |

## P02 — Transport and connection

| ID | Task | Depends on | Evidence / acceptance | Status |
| --- | --- | --- | --- | --- |
| P02-001 | Implement bounded codec and hello/welcome contract | P01-003 + owner review | Core/Networking tests: length/UTF-8/JSON/depth/directions; TESTING | Done |
| P02-002 | Add ChatServer/ClientSession with one-client admission | P02-001 | Real loopback admission/busy/timeout/restart and joined tasks; TESTING | Done |
| P02-003 | Add ChatClient username handshake and real connect/disconnect UI | P02-001, P02-002 | Native and actual EXE handshake/reconnect/close with disabled Send; TESTING | Done |
| P02-004 | Prove async ownership, stop/cancel and loopback lifecycle | P02-002, P02-003 | 82 local/hosted cases; actual two-process checks passed; exact receipt in PR #4 | Done |

## P03 — Messaging

| ID | Task | Depends on | Evidence / acceptance | Status |
| --- | --- | --- | --- | --- |
| P03-001 | Add immutable messages, text rules and server stamping/dispatch | P02-004 + owner review | Core/protocol/real loopback assertions; authoritative identity/clock, matching order, equal-text distinct IDs | Done |
| P03-002 | Bind one rendering path and awaited UI dispatch for both roles | P03-001 | Echo-only native UI; pending send, literal labels, retention/disposal and close regressions; TESTING | Done |
| P03-003 | Verify actual two-process Unicode/multiline bidirectional exchange | P03-002 | Actual separate EXEs: both directions, Unicode/multiline, author/time/count, equal text twice, reconnect/stop/close; TESTING | Done |

## P04 — Peer information and robustness

| ID | Task | Depends on | Evidence / acceptance | Status |
| --- | --- | --- | --- | --- |
| P04-001 | Harden disconnect/abort/reconnect and state/action recovery | P03-003 + owner review | Terminal-callback and notification-failure regressions; 12 simultaneous stop/disconnect/restart cycles; persistent uncertain-send feedback | Done |
| P04-002 | Verify close during I/O/dispatch, deadlines and bounded transcript | P04-001 | Deadline failure retains owned tasks; active native close/port release and visible 512-message trim/resize/close regressions; TESTING | Done |
| P04-003 | Complete server client-info list and remove stale entries | P04-001 | Actual socket/clock metadata, pending/busy rejection, fresh sessions, native one-row/clearing and actual EXE evidence; TESTING | Done |
| P04-004 | Exercise deliberate trusted-LAN setup | P04-001, P04-003 | Not performed: no second Windows machine/authorized remote target; exact owner procedure in ENVIRONMENT; no firewall changes | Pending |

## P05 — Optional owner decisions

| ID | Task | Depends on | Evidence / acceptance | Status |
| --- | --- | --- | --- | --- |
| P05-001 | Decide/implement multi-client room only if selected | P04 acceptance + explicit choice | Revised admission/order/fan-out tests and owner review | Deferred |
| P05-002 | Decide/implement private messages only if selected | Explicit choice and routing design | Recipient/delivery validation and no disclosure to other peers | Deferred |
| P05-003 | Decide/implement local history only if selected | Explicit choice and data policy | Save/cancel/permissions/error/encoding tests; no silent autosave | Deferred |

## P06 — Final QA

| ID | Task | Depends on | Evidence / acceptance | Status |
| --- | --- | --- | --- | --- |
| P06-001 | Audit requirements/source, versions, setup and clean published checkout | P04 accepted; selected P05 work reviewed | Source audit and baseline checks passed; exact published-checkout/final-head receipts follow execution in TESTING/PR | In progress |
| P06-002 | Capture genuine Server/Client images and reconcile guides | P06-001 | Three actual captures, manifest hashes/source/DPI and README embeds prepared; owner approval Pending | In progress |
| P06-003 | Record physical UI and second-computer results | P06-001 | Owner reported Not performed on 2026-11-22; no physical/second-machine pass inferred | Pending |
| P06-004 | Owner final PR acceptance and GitHub-profile submission | P06-002, P06-003 | Owner decision; no assistant merge/Classroom submission | Pending |

## Update rule

When a task changes, add evidence and update the phase gate in PLAN. P00 is accepted through owner merge of PR #2. P01 was merged in PR #3. P02 is accepted through owner merge of PR #4. P03 is accepted through owner merge of PR #5. P04 implementation is owner-merged in PR #6; P04-004 remains Pending. P06 preparation does not complete missing manual/external evidence or owner acceptance. Exact final-head publication/CI receipts belong in the final-review PR. Physical checks and phase acceptance remain owner actions. Optional tasks remain Deferred until separately chosen; they do not quietly become mandatory.
