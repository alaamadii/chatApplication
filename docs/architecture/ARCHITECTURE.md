# Architecture

## Implemented P01 structure

[ChatApplication.slnx](../../ChatApplication.slnx) contains six projects. Core is still empty and independent; no Networking project exists.

```text
Server ------> Core
       ------> Presentation
Client ------> Core
       ------> Presentation
Core.Tests --> Core
App.Tests ---> Server, Client, Presentation
```

| Existing project | Target and current responsibility |
| --- | --- |
| [Core](../../src/ChatApplication.Core/ChatApplication.Core.csproj) | net10.0; behavior-free assembly, no UI/drawing/socket/timer/clock dependencies |
| [Server](../../src/ChatApplication.Server/ChatApplication.Server.csproj) | net10.0-windows WinExe; editable listener settings and empty client context |
| [Client](../../src/ChatApplication.Client/ChatApplication.Client.csproj) | net10.0-windows WinExe; editable username/endpoint settings |
| [Presentation](../../src/ChatApplication.Presentation/ChatApplication.Presentation.csproj) | net10.0-windows; shared native shell, layout, theme, per-window fonts and empty transcript |
| [Core.Tests](../../tests/ChatApplication.Core.Tests/ChatApplication.Core.Tests.csproj) | net10.0; pinned test tooling, zero cases while Core is empty |
| [App.Tests](../../tests/ChatApplication.App.Tests/ChatApplication.App.Tests.csproj) | net10.0-windows; serial bounded native UI integration checks |

### Presentation ownership

Each STA entry point still enables PerMonitorV2 and visual styles. ServerForm/ClientForm inherit ChatWindow and own their local inputs. Their Layout partials compose role-specific sidebars; there are no Connection/Messaging partials or placeholder handlers.

ChatWindow owns the common header, preview badge, conversation shell, disabled composer, feedback and scrollable canvas. It owns one OwnedFonts instance per window. Controls are disposed before those fonts; closing another window does not invalidate resources. Presentation references no application service, socket or Core type.

Layout construction is suspended until the role controls exist, then resumes against the 96-DPI baseline. Native autoscaling handles point fonts and logical spacing. The default client is 1120 x 720 logical pixels; the design minimum is 900 x 620. The actual outer minimum/default are clamped to the current working area. DPI changes refit to the scaled default within the destination working area; physical monitor transitions remain pending.

A docked virtual canvas takes the larger of the design minimum and measured header/conversation/composer/feedback requirements. Expanded guidance can grow scrollable space without overlapping siblings. A separate sidebar viewport keeps its longer settings/client list reachable. The empty native list uses horizontal scrolling for metadata columns. No messages, peer rows, accepted identity or network state are synthesized.

Native App.Tests use internal access to the role Forms/working-area fit operation without adding application test hooks. Tests exercise actual layout/lifetime/no-op/navigation contracts. Core's zero discovery is reported separately.

## Future dependency direction

```text
ChatApplication.Server ──┬──> ChatApplication.Networking ──> ChatApplication.Core
                        └──> ChatApplication.Presentation ─> ChatApplication.Core

ChatApplication.Client ──┬──> ChatApplication.Networking ──> ChatApplication.Core
                        └──> ChatApplication.Presentation ─> ChatApplication.Core
```

Server/Client currently reference Core directly. The target diagram permits a future Presentation-to-Core dependency only when real shared types require it; none is needed now. Core must never reference another project. Networking must never reference WinForms/Presentation. Presentation must never open sockets.

| Project | Target / responsibility | Create when |
| --- | --- | --- |
| ChatApplication.Core | net10.0; immutable messages and pure input validation | P00 scaffold, behavior only when needed |
| ChatApplication.Server | net10.0-windows; operator Form/composition | P00 |
| ChatApplication.Client | net10.0-windows; user Form/composition | P00 |
| ChatApplication.Presentation | net10.0-windows library; shared theme/cards/transcript | P01 when shared UI is built |
| ChatApplication.Networking | net10.0; protocol and async transport/lifetime | P02 |
| Core.Tests / Networking.Tests | net10.0; pure and bounded loopback cases | P00 infrastructure / P02 behavior |
| App.Tests | net10.0-windows; native STA presentation/lifecycle | P01 when meaningful contracts exist |

The target table includes future classes/projects. Presentation and App.Tests now exist as described above; Networking and Networking.Tests remain planned. Message models/cards, transport and state machines remain future behavior.

## Class ownership

| Class / target role (implemented subset above) | Responsibility | Must not own |
| --- | --- | --- |
| ChatMessage | Immutable message ID, author role/name, text, server UTC time | Controls, socket, clock |
| ChatInputRules | Pure username/text validation | Dialogs or disk |
| MessageProtocol / FrameCodec | Directional packet/JSON validation; framed bytes | UI or connection retries |
| ChatServer | Bind/listen, one-client admission, ordered accepted-message flow | Forms or global transcript history |
| ClientSession | Accepted TcpClient/stream, username/endpoint/time, read task/cancellation/write gate | Verified user authentication |
| ChatClient | Connect/handshake/read/send/disconnect | UI controls |
| ServerForm / ClientForm | Explicit actions/state/input and safe UI dispatch | Parsing network bytes or socket lifetime internals |
| TranscriptView / MessageCard | Bounded visible message layout/accessibility | Transport or persistence |
| ChatTheme / OwnedFonts | Shared tokens and explicit resource ownership | Global disposable fonts reused after disposal |

Use concrete classes first; add only small interfaces justified by test seams. Avoid generic repositories, service containers and speculative layers.

## Server/client baseline

One server and one client provide two conversation endpoints. The Server operator can reply, so group chat is not needed to demonstrate bidirectional messaging. Reject an extra client with bounded server_busy handling until optional multi-client scope is approved.

Connected means socket plus accepted username handshake. Only then can messages be sent and peer information be shown. Store one session's metadata in memory; remove it exactly once on disconnect.

## Message flow and duplicate prevention

Client Send validates text and writes a chat request without trusting a client-supplied author/time. Do not append an optimistic local transcript copy. Server stamps an accepted message, shows it once and echoes the authoritative message to Client. Client appends that echo once as its outgoing message.

A Server operator message enters the same serialized dispatch path, is stamped/sent and shown once. Serialization must keep each frame intact and prevent differing local/remote ordering. Do not echo a received message back again.

Write completion is not a proof that a human received/read the message. No read receipts, automatic retry or exactly-once delivery across reconnect is promised. Reconnect creates a new session; do not replay uncertain messages silently.

## Async ownership and UI boundary

Each connection has one read loop and a serialized frame writer. Own every running Task and CancellationTokenSource. No .Result/.Wait on the UI thread and no async-void networking event handlers.

Networking emits immutable notifications through an awaited async callback. App's callback awaits UI dispatch before accepting the next notification, providing backpressure rather than an unbounded posted-control queue. Network code has no Control parameter.

Server dispatch serializes stamping/send/notification across local/operator and remote/client sources. Never hold a synchronous lock while awaiting; use cancellable async gates. Keep UI handlers asynchronous so stop/close can make progress.

Start/connect returns after the intended state is reached, while owned lifetime tasks remain observed. Stop/disconnect cancels, closes owned streams/clients/listener, awaits work and clears session state. Closing Forms first prevent late callbacks, await cleanup without blocking the UI and then close. Detailed shutdown deadline behavior is tested in P04.

## Limits, time and resources

Target limits: one active client, 16,384 UTF-8 bytes/frame, 2,000 UTF-16 code units/message, 24 code units/username and 500 visible messages/window. A trimmed-history notice appears when older visible messages are removed; there is no hidden persistent transcript.

Server supplies UTC DateTimeOffset through TimeProvider. UI converts for local display, retains arrival order and exposes full date/offset in accessible text. Clock changes do not trigger message reordering.

Each Form owns its fonts/components/cancellation and removes subscriptions on disposal. Each ClientSession owns both stream and TcpClient; close both deliberately rather than depending on implicit ownership. All resource rules must be verified in code, not inferred from this proposal.

See [protocol](PROTOCOL.md), [decisions](DECISIONS.md), [frontend file guide](../frontend/FILE_GUIDE.md) and [test strategy](../quality/TESTING.md).
