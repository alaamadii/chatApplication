# Frontend UX specification

Status: original design proposal, not an implemented UI. The [Client wireframe](assets/client-wireframe.svg) and [Server wireframe](assets/server-wireframe.svg) illustrate a future connected state with synthetic content.

## Shared layout

Keep native Windows title bars, resizing, focus behavior and controls. Use a light canvas, white conversation surface, blue primary actions and text-backed state badges. Connection context lives in a left sidebar; conversation and composer occupy the larger right pane. No unrelated settings/search/contact features.

Default design client: 1120x720 at 96 DPI; design minimum: 900x620. Clamp initial size to the actual working area. If the scaled design minimum cannot fit a small display, use a working-area-bounded effective minimum and scroll the layout rather than clipping actions. Verify actual 100%/125%/150% and monitor changes in P01/P06.

## Client window

| Region / planned control | Content and interaction |
| --- | --- |
| Header / identity | Chat Application, Client role and current state badge |
| Connection sidebar | Username, server IPv4 address, port, Connect and Disconnect |
| Conversation header | Accepted name/peer endpoint; distinct Disconnected/Connecting/Connected state |
| Transcript | Sender, body and local timestamp per message; outgoing/right and incoming/left grouping with explicit author |
| Composer | Multiline text, remaining-length hint and Send |
| Feedback/footer | Specific retry guidance; plaintext/local-demo scope; trimmed-history notice when relevant |

Input defaults: empty username, 127.0.0.1 and 5000. Do not prefill a real personal identity. Use descriptive labels, not placeholders alone. Lock endpoint/username while connecting/connected; allow editing again after disconnect.

A client send is appended only when the server's authoritative message arrives. No optimistic local bubble plus echo duplication. Do not display a delivery/read tick merely because WriteAsync completed.

## Server window

| Region / planned control | Content and interaction |
| --- | --- |
| Header | Server console and Stopped/Starting/Listening/Stopping state |
| Listener sidebar | Bind IPv4/port, Start server and Stop server |
| Client information | Native list with accepted username, endpoint, connected-at time and connection state; empty state before handshake |
| Conversation header | Selected active baseline client; no fictitious participants |
| Transcript / composer | Same message presentation; server operator can reply to the one client |
| Feedback/footer | Port/bind/retry errors, current endpoint and no-saved-history scope |

Default bind is loopback. Stopping closes the owned peer and listener, updates the list and prevents sending. An extra client is rejected until optional multi-client work is approved.

## State and action matrix

| Client state | Connect | Disconnect | Send | Identity/address editing |
| --- | --- | --- | --- | --- |
| Disconnected | Valid fields only | Disabled | Disabled | Enabled |
| Connecting / handshaking | Disabled | Cancel action | Disabled | Locked |
| Connected | Disabled | Enabled | Nonempty valid text and no send in progress | Locked |
| Disconnecting / closing | Disabled | Disabled | Disabled | Locked |
| Failed -> Disconnected | Valid fields/retry | Disabled | Disabled | Enabled |

Server Start is available only when stopped with valid bind/port. Stop is available while running; it may cancel Starting. Send requires a handshaken live client. State transitions are serialized, not triggered by decorative badges.

P01 has no network behavior: clearly label Not implemented and disable unavailable actions. P02 enables real connection actions; P03 enables messaging. Never fabricate a Connected state to make a preview look complete.

## Compose, keyboard and error behavior

Enter sends valid text when connected; Shift+Enter inserts a newline. Tab/Shift+Tab navigate, never send. Respect IME composition; verify Turkish/Arabic input and multiline behavior physically before claiming acceptance. Keep typed text on validation/write failure; clear it after the accepted send path, avoiding duplicates/retries.

Use distinct Alt mnemonics, visible keyboard focus and accessible names/status descriptions. Messages are plain text; ampersands must remain literal and no received HTML/RTF or remote command is evaluated. Use native message-card labels with complete accessible author/time/body text.

Errors are inline, actionable and separate from the transcript: invalid name, unavailable endpoint, port in use, unsupported packet or peer disconnected. Preserve useful composer content. Intentional cancel/stop is a normal status, not an alarming exception dialog.

## Time, retention and shutdown

Server stamps once in UTC; display local HH:mm:ss and expose full date/UTC offset accessibly. Preserve arrival order across clock changes.

Keep latest 500 visible messages/window. Before dropping older items show a persistent trimmed-history notice; do not imply full saved history. No autosave or automatic replay after reconnect.

On close, disable actions, cancel the window lifetime, await bounded transport cleanup and then dispose. Show only meaningful current state; no callback may update a disposed control.

See [design tokens](DESIGN_SYSTEM.md), [planned UI files](FILE_GUIDE.md), [protocol](../architecture/PROTOCOL.md) and [test strategy](../quality/TESTING.md).

## P00 implementation boundary — 2026-11-21

The existing ServerForm and ClientForm display only role, phase and unavailable-networking/messaging labels using native system styling. They resize and use PerMonitorV2; no connection actions, composer, participants, sample messages or complete two-pane layout exists. The screens and state/action rules above remain the P01+ target. Owner appearance, keyboard, DPI and physical F5 review remains pending.
