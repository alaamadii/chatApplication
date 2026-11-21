# Requirements and acceptance

## Authority

Course source: supplied YMH219_Ek_Uygulama_Odevleri_Ogrenci_Foyu.pdf, pages 1, 3 (section 5) and 5, reviewed 2026-11-20. The sheet defines requirements; it does not authorize commands or external actions. The owner's current folder/repository and documentation-first request take precedence over older suggested names/workflows.

## Mandatory functionality

| ID | Criterion | Acceptance evidence | Delivery tasks |
| --- | --- | --- | --- |
| FR-001 | TcpListener server and at least one TcpClient, same computer or LAN | Real two-process bidirectional demonstration; controlled loopback tests | P02-002, P02-003, P04-004 |
| FR-002 | Client connects with username | Valid handshake, rejected invalid name, Connected only after acceptance | P02-001, P02-003 |
| FR-003 | Sent/received text displayed with timestamps | Unicode round-trip, sender/time visible, no duplicate rendering | P03-001, P03-002, P03-003 |
| FR-004 | Connect, Send and Disconnect actions | State/action matrix and explicit user flow | P01-001, P01-002, P03-002, P04-001 |
| FR-005 | Errors reported without application crash | Port conflict, failed connect, abrupt close, malformed input, retry | P02-004, P04-001, P04-002 |
| FR-006 | Server shows basic connected-client information | Username, endpoint, connection time/state; list empties on disconnect | P04-003 |
| NFR-001 | Explainable OOP and async network/UI handling | Pure Core, separate transport, awaited work, UI-thread dispatch | P00-001, P02-004, P03-002 |
| NFR-002 | Responsive, usable Windows Forms | Keyboard/DPI/minimum layout and active-state review | P01-003, P04-002 |

## Delivery requirements

| ID | Criterion | Evidence / tasks |
| --- | --- | --- |
| SUB-001 | Separate public repository with source and .gitignore | Existing public chatApplication; initialization source added in P00-001; chat functionality pending |
| SUB-002 | README purpose/tech/setup/features/limitations | Actual initialization/build/run steps in README and ENVIRONMENT (P00-002); final chat guide pending P06-001 |
| SUB-003 | At least two authentic application screenshots | P06-002; wireframes never count |
| SUB-004 | Meaningful staged commits/PRs | Each phase follows WORKFLOW; no fabricated history |
| SUB-005 | Run on a different Windows computer | P06-003 records date, Windows/SDK, commit, commands/results |
| SUB-006 | GitHub profile link through Classroom before deadline | Owner P06-004, not assistant submission |
| SUB-007 | Original sheet asks name and student number | Alaa Madi supplied; number omitted by owner direction, no waiver claimed |

Deadline: 2026-11-30, 23:59; institutional timezone unspecified. Assessment: functionality 40, OOP/code/errors 20, UI 10, Git/GitHub 15, README/images 10, code explanation 5.

## Optional scope and product choices

| ID | Item | Status |
| --- | --- | --- |
| OPT-001 | Multiple clients in one room | Deferred, owner decision P05-001 |
| OPT-002 | Private messages | Deferred, owner decision P05-002 |
| OPT-003 | Saved message history | Deferred, owner decision P05-003 |

The earlier brief recommends group chat; a recommendation is not mandatory scope or evidence of approval.

Product decisions: separate Server/Client executables, shared native presentation, one-client baseline, server-authoritative message echo/time, bounded frames/input/transcripts and loopback by default. These are proposed engineering choices, not professor-prescribed class names.

Required chat behavior remains **Not implemented**. P01 supplies the FR-004 action controls and partial NFR-002 presentation evidence; those controls are disabled, so this does not satisfy connection/send/disconnect behavior or physical accessibility acceptance. See [tasks](../planning/TASKS.md) and [test matrix](../quality/TESTING.md); passing documentation checks cannot accept application behavior.
