# Frontend and integration file guide

## Existing role and shared files

| File | Implemented ownership |
| --- | --- |
| [ServerForm](../../src/ChatApplication.Server/Forms/ServerForm.cs) | Listener inputs, action fields, owned ChatServer and UI event subscriptions |
| [Server layout](../../src/ChatApplication.Server/Forms/ServerForm.Layout.cs) | Listener sidebar and explicitly deferred detailed peer list |
| [Server connection](../../src/ChatApplication.Server/Forms/ServerForm.Connection.cs) | Start/Stop commands, real listening/accepted-peer projection and awaited close |
| [ClientForm](../../src/ChatApplication.Client/Forms/ClientForm.cs) | Identity/endpoint inputs, action fields and owned ChatClient |
| [Client layout](../../src/ChatApplication.Client/Forms/ClientForm.Layout.cs) | Identity/endpoint sidebar |
| [Client connection](../../src/ChatApplication.Client/Forms/ClientForm.Connection.cs) | Connect/Cancel/Disconnect, input locks, accepted identity and awaited close |
| [ChatWindow](../../src/ChatApplication.Presentation/ChatWindow.cs) | Shared geometry, measured scrolling and per-window fonts |
| [Window layout](../../src/ChatApplication.Presentation/ChatWindow.Layout.cs) | Header, real status/context, empty transcript and unavailable composer |
| [Window connection boundary](../../src/ChatApplication.Presentation/ChatWindow.Connection.cs) | Cancellable guarded UI dispatch, reusable close sequencing and presentation-only status setter |
| [ConnectionFeedback](../../src/ChatApplication.Presentation/ConnectionFeedback.cs) | Local controlled-code guidance; never renders remote error prose |
| [ConnectionBadge](../../src/ChatApplication.Presentation/Controls/ConnectionBadge.cs) | Actual text-backed neutral/success/warning/error tones |
| [TranscriptView](../../src/ChatApplication.Presentation/Controls/TranscriptView.cs) | Empty conversation only; no message collection/cards |
| [ChatTheme](../../src/ChatApplication.Presentation/Theme/ChatTheme.cs) | Semantic shell palette and system-color fallback |
| [OwnedFonts](../../src/ChatApplication.Presentation/Theme/OwnedFonts.cs) | Per-window point fonts disposed after controls |
| [UiLayout](../../src/ChatApplication.Presentation/Layout/UiLayout.cs) | Native field/label/action builders without network behavior |

STA/PerMonitorV2 entry points remain separate. Role Forms own Networking; Presentation owns no socket/service and references no Networking/Core project. Networking never receives a Control.

Normal Form close first guards callbacks, aborts owned I/O, joins outstanding UI commands and awaits service disposal. Direct Form disposal aborts I/O and prevents disposed-control callbacks. Normal close is the tested awaited application lifecycle.

## Connection implementation

| File | Responsibility |
| --- | --- |
| [UsernameRules](../../src/ChatApplication.Core/UsernameRules.cs) | Pure supported-name validation/normalization |
| [MessageProtocol](../../src/ChatApplication.Networking/Protocol/MessageProtocol.cs) | Strict bounded JSON connection packets and directions |
| [FramedConnection](../../src/ChatApplication.Networking/Protocol/FramedConnection.cs) | Exact framed I/O, first-byte deadline and serialized writes |
| [ConnectionPacket](../../src/ChatApplication.Networking/Protocol/ConnectionPacket.cs) | Implemented hello/welcome/error/disconnect values and phases |
| [ChatServer](../../src/ChatApplication.Networking/ChatServer.cs) | Listener, one reserved peer, bounded busy rejection and joined stop |
| [ClientSession](../../src/ChatApplication.Networking/ClientSession.cs) | Accepted socket/stream, validated handshake and idle connection read |
| [ChatClient](../../src/ChatApplication.Networking/ChatClient.cs) | Owned connect/handshake/read/cancel/disconnect lifetime |
| [ConnectionSnapshot](../../src/ChatApplication.Networking/ConnectionSnapshot.cs) | Immutable state/name/session/error notification |
| [ConnectionUpdates](../../src/ChatApplication.Networking/ConnectionUpdates.cs) | Serialized awaited observer updates with cancellation |
| [ConnectionOptions](../../src/ChatApplication.Networking/ConnectionOptions.cs) | Verified defaults and short configurable test deadlines |
| [EndpointInput](../../src/ChatApplication.Networking/EndpointInput.cs) | Production dotted-decimal IPv4/nonzero-port parsing |
| [ConnectionFailureException](../../src/ChatApplication.Networking/ConnectionFailureException.cs) | Controlled protocol/local failure code and preserved exception cause |

Tests live in [Core.Tests](../../tests/ChatApplication.Core.Tests/UsernameRulesTests.cs), [Networking protocol tests](../../tests/ChatApplication.Networking.Tests/ProtocolTests.cs), [Networking lifecycle tests](../../tests/ChatApplication.Networking.Tests/ConnectionTests.cs) and [native App tests](../../tests/ChatApplication.App.Tests/InterfaceTests.cs).

## Future files — not created

ServerForm.Messaging.cs, ClientForm.Messaging.cs, ChatMessage/message validation and MessageCard remain P03. Detailed participant metadata/list population remains P04. No empty future handler or simulated message is introduced.

[Client](assets/client-wireframe.svg) and [Server](assets/server-wireframe.svg) wireframes remain synthetic design proposals; [tokens](theme.json) retain their design inventory role. Local review captures stay ignored.

See [UX](UX.md), [architecture](../architecture/ARCHITECTURE.md) and [testing](../quality/TESTING.md).
