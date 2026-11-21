# Architecture

## Implemented P02 structure

[ChatApplication.slnx](../../ChatApplication.slnx) contains eight projects:

| Project | Actual target / responsibility |
| --- | --- |
| Core | net10.0; pure UsernameRules, no UI/drawing/socket/timer/clock dependency |
| Networking | net10.0; framed connection protocol, validation, service/session ownership and cancellation |
| Presentation | net10.0-windows; native shell/theme/fonts, guarded UI dispatch/close and local error copy; no Networking/Core reference |
| Server / Client | net10.0-windows WinExe; separate composition, inputs/actions and real connection state projection |
| Core.Tests | net10.0; actual supported-name behavior |
| Networking.Tests | net10.0; codec/endpoint/deadline/real loopback lifecycle cases |
| App.Tests | net10.0-windows; bounded serial native layout/action/handshake/close cases |

Actual dependencies: Networking -> Core; Apps -> Core/Networking/Presentation; Core.Tests -> Core; Networking.Tests -> Networking; App.Tests -> Apps/Presentation (Networking transitively). Core and Presentation have no project references.

### Wire and state ownership

MessageProtocol/ConnectionPacket implement only hello, welcome, error and disconnect. FramedConnection owns its stream and one-reader contract, bounds allocation/JSON and serializes full writes. The name MessageProtocol describes the wire protocol; it contains no chat message model. See [PROTOCOL](PROTOCOL.md) for exact byte/direction rules.

ChatServer owns the listener, accept task and at most one pending/accepted ClientSession task. It reserves that slot before handshake I/O. Busy arrivals are processed serially by the accept loop: send server_busy and consume at most one bounded initial frame before close, avoiding a TCP reset discarding the response. No unbounded rejection/handshake task is created.

ClientSession owns accepted socket/stream, server-side name validation, welcome and accepted idle read. PeerCompletion exposes the owned task; cleanup releases the slot after task completion. Listening is the server state; an accepted peer name/session is separate data.

ChatClient owns one connect/handshake/read task. ConnectAsync completes successfully only after a validated welcome with the requested normalized name and a nonempty GUID. Completion remains active during the idle read. EOF/disconnect clears live-peer data; reconnect is explicit and creates a new session.

### Notifications, cancellation and cleanup

ConnectionSnapshot is immutable state/name/session/error data. ConnectionUpdates serializes awaited observer calls with cancellation. Observers must honor cancellation and never perform network cleanup reentrantly from the notification. Unexpected observer exceptions are converted to notification_failed and disable that failed observer; failures are not represented as successful connection.

Deadlines are configured in ConnectionOptions: connect/handshake/send/shutdown 5 seconds; frame completion 10 seconds after its first byte. Idle accepted reads do not expire. Shutdown uses an overall budget including lifecycle gate, orderly-close attempt, joined tasks and final notification. Owned I/O is canceled/closed before joining; a failed deadline retains observable completion rather than reporting successful cleanup.

Apps marshal snapshots with cancellable Control.InvokeAsync and closing/disposal guards. Async-void is limited to framework UI event boundaries, whose command Tasks are stored/awaited and errors controlled. Normal Form close guards updates, aborts I/O, joins outstanding commands, awaits service disposal, then closes/disposes controls and per-window fonts. Direct Form disposal aborts I/O; normal awaited close is the verified application path.

### Presentation

The inherited ChatWindow owns per-window fonts and the shared measured virtual canvas. Construction remains layout-suspended until role controls exist. Default/minimum/wide and small-working-area behavior retain the P01 regression coverage.

Connection badges display actual state in text with neutral/success/warning/error tones and system-color fallback. Client input locks and Connect/Cancel/Disconnect follow real state. Server settings lock while active. Composer/Send remain disabled, transcript stays empty, and the detailed metadata list is clearly deferred to P04 even while an accepted name is shown elsewhere.

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

The target table includes future classes/projects. The P02 projects exist as described above; Networking and Networking.Tests now exist; only messaging/full metadata behavior remains planned. Chat models/cards, message dispatch and complete participant metadata remain future behavior; the P02 transport/connection states are implemented above.

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
