# Frontend and integration file guide

## Role and shared presentation

| File | Implemented ownership |
| --- | --- |
| [ServerForm](../../src/ChatApplication.Server/Forms/ServerForm.cs) / [layout](../../src/ChatApplication.Server/Forms/ServerForm.Layout.cs) | Listener settings, role composition and actual accepted-client information list |
| [Server client information](../../src/ChatApplication.Server/Forms/ServerForm.ClientInfo.cs) | Accepted-only row projection, clearing, full accessible/tooltip metadata and measured native columns |
| [Server connection](../../src/ChatApplication.Server/Forms/ServerForm.Connection.cs) | Start/Stop commands, real state, peer summary and awaited close |
| [Server messaging](../../src/ChatApplication.Server/Forms/ServerForm.Messaging.cs) | Service Send and awaited message-to-UI callback |
| [ClientForm](../../src/ChatApplication.Client/Forms/ClientForm.cs) / [layout](../../src/ChatApplication.Client/Forms/ClientForm.Layout.cs) | Identity/endpoint settings and composition |
| [Client connection](../../src/ChatApplication.Client/Forms/ClientForm.Connection.cs) | Connect/Cancel/Disconnect, accepted state and awaited close |
| [Client messaging](../../src/ChatApplication.Client/Forms/ClientForm.Messaging.cs) | Echo-based Send and awaited message-to-UI callback |
| [ChatWindow](../../src/ChatApplication.Presentation/ChatWindow.cs) / [layout](../../src/ChatApplication.Presentation/ChatWindow.Layout.cs) | Shared measured canvas, native controls and per-window fonts |
| [Window connection](../../src/ChatApplication.Presentation/ChatWindow.Connection.cs) | Guarded cancellable UI dispatch and close sequencing |
| [Window feedback](../../src/ChatApplication.Presentation/ChatWindow.Feedback.cs) | Retained unconfirmed-send guidance combined with current connection guidance |
| [Window messaging](../../src/ChatApplication.Presentation/ChatWindow.Messaging.cs) | One pending UI send, validation/count, confirmation clearing, failure retention and role alignment |
| [TranscriptView](../../src/ChatApplication.Presentation/Controls/TranscriptView.cs) | Latest 500 cards/IDs, visible count, scrolling, persistent trim notice and removed-card disposal |
| [MessageCard](../../src/ChatApplication.Presentation/Controls/MessageCard.cs) | Literal separate author/time/body labels, wrapping and borrowed fonts |
| [MessageComposer](../../src/ChatApplication.Presentation/Controls/MessageComposer.cs) | Native multiline input, shared Enter request and IME composition guard |
| [ChatTheme](../../src/ChatApplication.Presentation/Theme/ChatTheme.cs) / [OwnedFonts](../../src/ChatApplication.Presentation/Theme/OwnedFonts.cs) | Shared semantic colors/system fallback; fonts owned per window |
| [UiLayout](../../src/ChatApplication.Presentation/Layout/UiLayout.cs) | Native label/field/action builders |
| [ConnectionBadge](../../src/ChatApplication.Presentation/Controls/ConnectionBadge.cs) / [ConnectionFeedback](../../src/ChatApplication.Presentation/ConnectionFeedback.cs) | Real state text/tones and local controlled-code guidance |

STA/PerMonitorV2 entry points remain separate. Apps own Networking services; Presentation references Core message values and no Networking project. Normal close guards callbacks, aborts I/O, joins commands/service and pending send, then disposes child controls before fonts. Direct disposal aborts I/O; awaited normal close is the verified application path.

Message cards are original native rectangular panels. They own their labels, borrow fonts and use theme tokens; they allocate no disposable static drawing resources. Transcript resize/DPI wrapping reflows existing cards, while stable-width append measures only the new card. Explicit canvas bounds and batched native layout avoid repeated Dock layout across retained cards. Dropped cards are removed from all live transcript collections and disposed immediately.

## Domain and network implementation

| File | Responsibility |
| --- | --- |
| [UsernameRules](../../src/ChatApplication.Core/UsernameRules.cs) | Pure canonical display-name rules |
| [MessageRules](../../src/ChatApplication.Core/MessageRules.cs) | Pure 2,000 UTF-16-unit, whitespace, surrogate and control checks |
| [ChatMessage](../../src/ChatApplication.Core/ChatMessage.cs) | Immutable validated ID/author/body/UTC value and author-role enum |
| [MessageProtocol](../../src/ChatApplication.Networking/Protocol/MessageProtocol.cs) / [ConnectionPacket](../../src/ChatApplication.Networking/Protocol/ConnectionPacket.cs) | Strict directional v1 hello/welcome/error/disconnect/chat/message |
| [FramedConnection](../../src/ChatApplication.Networking/Protocol/FramedConnection.cs) | Bounded exact framed I/O, first-byte completion deadline and serialized writer |
| [ChatServer](../../src/ChatApplication.Networking/ChatServer.cs) | Listener/admission, injected TimeProvider, operator Send and owned stop |
| [ClientSession](../../src/ChatApplication.Networking/ClientSession.cs) | Accepted peer/name, continuous read and single authoritative dispatch gate |
| [ChatClient](../../src/ChatApplication.Networking/ChatClient.cs) | Connect/read, one generation/session-bound pending echo, bounded recent IDs and joined disconnect |
| [MessageUpdates](../../src/ChatApplication.Networking/MessageUpdates.cs) | Awaited message observer with cancellation/deadline and controlled failure |
| [ConnectedClientInfo](../../src/ChatApplication.Networking/ConnectedClientInfo.cs) | Immutable server-local accepted name/session, actual remote address/port, UTC time and Connected state |
| [ConnectionSnapshot](../../src/ChatApplication.Networking/ConnectionSnapshot.cs) / [ConnectionUpdates](../../src/ChatApplication.Networking/ConnectionUpdates.cs) | Immutable state and serialized awaited notifications |
| [ConnectionOptions](../../src/ChatApplication.Networking/ConnectionOptions.cs) / [EndpointInput](../../src/ChatApplication.Networking/EndpointInput.cs) | Validated deadline defaults and dotted-decimal IPv4/port input |
| [ConnectionFailureException](../../src/ChatApplication.Networking/ConnectionFailureException.cs) | Controlled failure code with preserved internal cause |

## Tests and remaining work

Pure assertions: [username](../../tests/ChatApplication.Core.Tests/UsernameRulesTests.cs) and [messages](../../tests/ChatApplication.Core.Tests/MessageTests.cs). Networking: [framing](../../tests/ChatApplication.Networking.Tests/ProtocolTests.cs), [lifecycle](../../tests/ChatApplication.Networking.Tests/ConnectionTests.cs), [message protocol](../../tests/ChatApplication.Networking.Tests/MessagingProtocolTests.cs) and [real messaging](../../tests/ChatApplication.Networking.Tests/MessagingTests.cs). Native Windows: [interface regressions](../../tests/ChatApplication.App.Tests/InterfaceTests.cs) and [messaging/retention/lifetime](../../tests/ChatApplication.App.Tests/MessagingInterfaceTests.cs).

P04 completes accepted-client metadata/list population. Additional regressions live in [terminal lifecycle](../../tests/ChatApplication.Networking.Tests/LifecycleRegressionTests.cs), [metadata/robustness](../../tests/ChatApplication.Networking.Tests/PeerInformationTests.cs) and [native metadata/recovery/visible burst](../../tests/ChatApplication.App.Tests/RobustnessInterfaceTests.cs). P04-004 trusted-LAN verification remains pending. No empty future handler, persistence or optional room model is introduced. [Client](assets/client-wireframe.svg) and [Server](assets/server-wireframe.svg) wireframes remain proposals; [tokens](theme.json) remain a design inventory. Local genuine review captures are ignored.

See [UX](UX.md), [architecture](../architecture/ARCHITECTURE.md) and [testing](../quality/TESTING.md).
