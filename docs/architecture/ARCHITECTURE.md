# Architecture

## Implemented baseline reviewed in P06

[ChatApplication.slnx](../../ChatApplication.slnx) contains eight projects.

| Project | Ownership |
| --- | --- |
| Core | Pure UsernameRules, MessageRules and immutable ChatMessage; no clock, socket, drawing or Windows dependency |
| Networking | TCP, framing, strict directional protocol, connection/session resources and authoritative dispatch |
| Presentation | Native shared window/theme/fonts, transcript/cards, composer and guarded UI dispatch; references Core, never Networking |
| Server / Client | Separate Windows executables; compose services with role Forms and bind explicit user commands |
| Core.Tests / Networking.Tests | Pure validation and bounded protocol/real loopback behavior |
| App.Tests | Serial native STA layout, message/input, retention, resource and close behavior |

Dependencies: Networking -> Core; Presentation -> Core; Apps -> Core/Networking/Presentation; tests reference their actual subjects. Core has no project references. Networking never receives controls. Presentation opens no socket.

## Connection ownership

ChatServer owns its listener, accept task and at most one pending/accepted ClientSession task. Admission is reserved before handshake I/O. The accept loop handles busy rejection serially with a bounded initial-frame drain; it creates no unbounded arrival tasks.

ClientSession owns its socket, framed stream, accepted name and cancellation source. After welcome succeeds it captures the actual socket endpoint and server TimeProvider UTC acceptance time in immutable ConnectedClientInfo. ConnectionSnapshot carries this server-local value alongside its compatible name/session fields. Pending/busy/rejected peers have no accepted metadata; server peer notifications check their source session. PeerCompletion releases the peer slot only after its read loop and any active operator dispatch have unwound. ChatClient owns one connect/handshake/continuous-read task. Connected requires a validated welcome, matching canonical name and nonempty session GUID. EOF/disconnect clears live peer state; reconnect is explicit. Start/Connect may see a terminal snapshot before its final observer returns: they now join the old lifetime (and pending Client sender) under the lifecycle gate before replacing fields, with a shutdown-budget deadline. These are explicit user commands, never automatic reconnects.

FramedConnection has one reader and a serialized complete-frame writer. It bounds allocation and parsing and owns stream disposal. See [PROTOCOL](PROTOCOL.md).

## Authoritative message flow and order

Core validates supplied values; it does not generate IDs or read a clock. The server session derives a client's author from its accepted name. The Server operator has role server and fixed name Server.

One per-session dispatch semaphore handles both incoming chat requests and operator sends. Under that gate, validate live admission, generate GUID/time once through the server's injectable TimeProvider, write the authoritative message, then await the local message observer. Both transcripts therefore see the same acceptance order. UTC clock changes never sort or reorder entries.

Client sends no optimistic transcript copy. Its continuous reader validates the authoritative message, awaits its UI callback, then completes the matching pending submission for its own echo. Server messages enter that same receive/render path without completing a client submission. Distinct GUIDs preserve intentional equal text.

## Pending sends and session boundaries

ChatClient allows one pending send, identified by connection generation, accepted session and submitted text. Parallel API submissions are rejected rather than queued. The send gate is registered while holding the lifecycle gate, but I/O and echo waits happen outside the lifecycle gate.

A single send budget covers write plus authoritative acceptance/display. Cancellation, timeout, write failure or peer close faults/cancels pending work and terminates the affected socket; no automatic resend. Failure cleanup captures that socket/task, so it cannot close a future connection. New Connect waits for previous lifetime/send ownership to finish. Disconnect cancels/joins the reader and pending send without making the reader await its sender.

A per-read-loop queue/set remembers at most 500 recent message IDs. Repeated recent IDs are ignored. This bounded protection is not an indefinite replay ledger or an exactly-once guarantee against a malicious server.

## Notifications and shutdown

ConnectionUpdates serializes awaited immutable state notifications. MessageUpdates awaits immutable messages with cancellation/deadline tokens. Observers must honor cancellation and must not reenter service cleanup from a notification. A failed message callback fails the session; owned Forms use cancellable InvokeAsync.

Default deadlines: connect/handshake/send/shutdown 5 seconds; frame completion 10 seconds after its first byte. Any Disconnect/Stop failure still aborts owned I/O. Session-final notification cancellation is normal only when its server lifetime was actually canceled; unrelated failures remain observable. Session abort is idempotent and its cancellation source is disposed after dispatch joins. Idle accepted reads do not expire. Shutdown budgets include lifecycle admission, close attempt, joining and final state notification. Failure retains observable completion instead of claiming successful cleanup.

Forms own their command Tasks and pending UI send. Async-void is confined to framework event boundaries that await handled commands. Normal close guards UI callbacks, aborts I/O, joins commands/service disposal and pending UI send, then disposes controls and fonts. Direct Form disposal aborts I/O; normal awaited close is the verified application path.

## Presentation and resource ownership

ChatWindow owns shared geometry, role composition, per-window fonts, TranscriptView and MessageComposer. MessageCard separates literal author, local time and body labels; outgoing alignment/color supplement explicit author text. Point fonts are borrowed from the window, disposed after its children.

ServerForm.ClientInfo projects one accepted row only, with measured DPI-aware column widths, native horizontal scrolling, a complete tooltip/accessibility description and keyboard access when populated. Connected is the row's accepted-peer state, distinct from the server's Listening state. Stopping/Stopped or peer removal clears the row.

The transcript retains only the latest 500 cards and corresponding IDs. Removing a card removes its collection/control references and disposes it immediately. A persistent trim notice explains the loss. Reconnect retains this bounded window view; closing the window releases it. There is no other transcript, history file or global message collection.

Native scrolling follows the latest appended message. Resize/DPI changes remeasure wrapping. At stable width only the new card is measured; width/DPI caches avoid repeated measurement of retained cards. P04 uses explicit scrolling-canvas bounds and batches append/trim layout to avoid repeated Dock layout across retained cards. The visible 512-message regression exercises real native controls, resize, trim/disposal and close within the existing fixture deadline. Default/minimum/working-area and small-display scrolling retain P01 regressions.

Composer/button share one guarded async send path. It disables editing during a pending submission, displays UTF-16 remaining length, clears only a successfully confirmed unchanged draft and retains failed/newer text. Unconfirmed-send guidance is stored independently of current connection guidance and remains until a successful confirmed send. Busy command flags prevent repeated click queues; failed cleanup permits explicit Stop/Disconnect retry. Enter sends, Shift+Enter inserts a line, Tab navigates. IME composition suppresses Enter submission while active; physical input/assistive verification remains pending.

## Remaining scope

The accepted-client list and documented local robustness cases are merged through PR #6. P04-004 trusted-LAN verification remains pending. P06 adds final documentation and real capture validation without changing application code. Optional rooms/private messages/history remain deferred. P06 prepares final images, exact published-checkout evidence and submission review. Another computer, physical acceptance and owner approval remain external checks; the final PR stays draft while required evidence is missing. Passing CI does not provide owner acceptance.

See [decisions](DECISIONS.md), [frontend file guide](../frontend/FILE_GUIDE.md) and [testing](../quality/TESTING.md).
