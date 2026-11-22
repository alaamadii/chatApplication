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
| SUB-001 | Separate public repository with source and .gitignore | Existing public chatApplication; initialization source added in P00-001; P04 baseline merged; P06 final-review preparation and missing external evidence tracked separately |
| SUB-002 | README purpose/tech/setup/features/limitations | P06 README/ENVIRONMENT cover actual chat build/run/recovery; exact published verification is recorded in TESTING/final PR (P06-001) |
| SUB-003 | At least two authentic application screenshots | Three genuine captures/provenance embedded in README; owner approved all three on 2026-11-22 (P06-002); wireframes never count |
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

Product decisions: separate Server/Client executables, shared native presentation, one-client baseline, server-authoritative message echo/time, bounded frames/input/transcripts and loopback by default. These are implemented baseline engineering choices, not professor-prescribed class names.

PR #6 is merged. P04 implements FR-006 with server-local accepted name, socket IP/port, acceptance time/state and live-row removal. FR-004/005 gain tested command recovery, terminal-callback joining, interrupted-send guidance and simultaneous stop/disconnect cleanup. Current evidence is 139 local cases plus separate actual EXEs, including abrupt process termination and explicit recovery; P06 final acceptance remains pending; merging P04 does not perform its carried-forward LAN check. FR-001 has real same-computer exchange evidence; deliberate trusted-LAN verification (P04-004) remains pending. Physical accessibility and second-computer evidence remain pending. Genuine P06 images have source/hash provenance; the owner approved all three on 2026-11-22. Exact published-checkout verification passed; final receipt-commit outcomes belong in PR #7. See [tasks](../planning/TASKS.md) and [test matrix](../quality/TESTING.md); passing documentation checks cannot accept application behavior.

## P06 source and evidence audit — 2026-11-22

This audit separates implemented behavior and executed same-computer evidence from missing physical/external acceptance. It does not add optional scope.

| Criterion | Actual source | Behavioral evidence / remaining acceptance |
| --- | --- | --- |
| FR-001 | [ChatServer](../../src/ChatApplication.Networking/ChatServer.cs), [ChatClient](../../src/ChatApplication.Networking/ChatClient.cs) | Owned real TCP loopback and actual separate EXEs; trusted LAN remains Pending |
| FR-002 | [UsernameRules](../../src/ChatApplication.Core/UsernameRules.cs), [ClientSession](../../src/ChatApplication.Networking/ClientSession.cs) | [Connection tests](../../tests/ChatApplication.Networking.Tests/ConnectionTests.cs): valid/rejected/mismatched handshake and cancellation |
| FR-003 | [ChatMessage](../../src/ChatApplication.Core/ChatMessage.cs), [MessageProtocol](../../src/ChatApplication.Networking/Protocol/MessageProtocol.cs), [MessageCard](../../src/ChatApplication.Presentation/Controls/MessageCard.cs) | [Messaging tests](../../tests/ChatApplication.Networking.Tests/MessagingTests.cs): authoritative author/time/order, equal text distinct; actual images in README |
| FR-004 | [Client commands](../../src/ChatApplication.Client/Forms/ClientForm.Connection.cs), [shared composer](../../src/ChatApplication.Presentation/ChatWindow.Messaging.cs) | [Native messaging tests](../../tests/ChatApplication.App.Tests/MessagingInterfaceTests.cs); physical keyboard/IME still Not performed |
| FR-005 | [FramedConnection](../../src/ChatApplication.Networking/Protocol/FramedConnection.cs), [ConnectionFeedback](../../src/ChatApplication.Presentation/ConnectionFeedback.cs) | [Lifecycle regressions](../../tests/ChatApplication.Networking.Tests/LifecycleRegressionTests.cs), controlled actual-process errors/recovery |
| FR-006 | [ConnectedClientInfo](../../src/ChatApplication.Networking/ConnectedClientInfo.cs), [Server information view](../../src/ChatApplication.Server/Forms/ServerForm.ClientInfo.cs) | [Peer information tests](../../tests/ChatApplication.Networking.Tests/PeerInformationTests.cs): actual socket/clock, accepted-only data and removal; actual UIA/MSAA check |
| NFR-001 | Core/Networking/Presentation/Apps dependency boundaries in ARCHITECTURE | No UI/socket/clock in Core; awaited ownership, UI dispatch and normal close reviewed/tested |
| NFR-002 | [ChatWindow](../../src/ChatApplication.Presentation/ChatWindow.cs), [TranscriptView](../../src/ChatApplication.Presentation/Controls/TranscriptView.cs) | [Native robustness](../../tests/ChatApplication.App.Tests/RobustnessInterfaceTests.cs): geometry/visible trim/close; physical accessibility/F5/other DPI pending |

The owner reported all requested manual/external checks Not performed on 2026-11-22. P06 can prepare source/documentation/images and a draft PR, but final acceptance and submission readiness remain incomplete until the checklist evidence is supplied.
