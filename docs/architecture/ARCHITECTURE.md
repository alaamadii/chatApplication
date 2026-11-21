# Architecture

## Implemented P00 structure

[ChatApplication.slnx](../../ChatApplication.slnx) contains four projects only. [Core](../../src/ChatApplication.Core/ChatApplication.Core.csproj) is an empty net10.0 assembly. [Server](../../src/ChatApplication.Server/ChatApplication.Server.csproj) and [Client](../../src/ChatApplication.Client/ChatApplication.Client.csproj) are independent net10.0-windows WinExe projects referencing Core. [Core.Tests](../../tests/ChatApplication.Core.Tests/ChatApplication.Core.Tests.csproj) references Core with pinned test tooling and no cases.

Each executable has its own STA entry point, PerMonitorV2 initialization and role Form. Forms own their native layout/labels through the control tree; no custom fonts/drawing resources are allocated. DPI autoscaling starts at 96 logical DPI, the client area is 800 x 480 and the minimum outer size is 640 x 400. Anchored, wrapping labels use the system font and remain within a fill-docked TableLayoutPanel. Physical scaling review is pending.

Actual dependency direction is Server -> Core, Client -> Core and Core.Tests -> Core. Core contains no models or Windows/drawing/socket/timer/clock dependencies. There are no background tasks, listeners or gameplay-style timers. Networking and shared Presentation do not exist yet.

The remainder describes the target architecture for future phases; only the initialization entry points/Forms and four projects above exist.

## Dependency direction

```text
ChatApplication.Server ──┬──> ChatApplication.Networking ──> ChatApplication.Core
                        └──> ChatApplication.Presentation ─> ChatApplication.Core

ChatApplication.Client ──┬──> ChatApplication.Networking ──> ChatApplication.Core
                        └──> ChatApplication.Presentation ─> ChatApplication.Core
```

Server/Client may also reference Core directly. Core must never reference another project. Networking must never reference WinForms/Presentation. Presentation must never open sockets.

| Project | Target / responsibility | Create when |
| --- | --- | --- |
| ChatApplication.Core | net10.0; immutable messages and pure input validation | P00 scaffold, behavior only when needed |
| ChatApplication.Server | net10.0-windows; operator Form/composition | P00 |
| ChatApplication.Client | net10.0-windows; user Form/composition | P00 |
| ChatApplication.Presentation | net10.0-windows library; shared theme/cards/transcript | P01 when shared UI is built |
| ChatApplication.Networking | net10.0; protocol and async transport/lifetime | P02 |
| Core.Tests / Networking.Tests | net10.0; pure and bounded loopback cases | P00 infrastructure / P02 behavior |
| App.Tests | net10.0-windows; native STA presentation/lifecycle | P01 when meaningful contracts exist |

Do not create empty future classes. P00 implements only the four projects listed above; the Networking/Presentation/App.Tests/Networking.Tests rows remain planned.

## Class ownership

| Planned class | Responsibility | Must not own |
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
