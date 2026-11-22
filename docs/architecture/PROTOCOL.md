# TCP protocol v1

Status: P03 connection/message packets are preserved by P04; no packet, field, version or default deadline was added or relaxed. One accepted client and the Server operator form the baseline.

## Framing and parsing

A frame is a **4-byte signed big-endian payload length** followed by exactly that many UTF-8 JSON bytes. Length excludes the prefix and must be 1..16,384. Reject invalid sizes before allocating. TCP fragments/coalesces bytes: read prefix/payload exactly, classifying EOF before the first byte as clean close and partial-frame EOF as failure.

Require strict UTF-8, one JSON object, version=1, camelCase fields and maximum JSON depth 8. Reject recursively duplicated property names, invalid Unicode, malformed fields and wrong directions. Unknown extra fields are ignored within those limits. One reader owns the stream; a semaphore serializes complete-frame writes.

## Packets and permitted phases

| Type | Direction / phase | Required fields besides version/type |
| --- | --- | --- |
| hello | Client -> Server handshake only | userName |
| welcome | Server -> Client handshake only | sessionId, userName |
| error | Server -> Client handshake or accepted | code |
| chat | Client -> Server accepted only | text |
| message | Server -> Client accepted only | messageId, authorRole, authorName, text, sentAtUtc |
| disconnect | Either accepted peer | None |

Examples are illustrative payloads, not execution evidence. Prefixes use encoded byte counts.

```json
{"version":1,"type":"chat","text":"Hello!\nSecond line"}
```

```json
{"version":1,"type":"message","messageId":"11111111-1111-4111-8111-111111111111","authorRole":"client","authorName":"Alaa","text":"Hello!","sentAtUtc":"2026-11-21T12:34:56+00:00"}
```

Client-supplied author/name/time/ID extras in a chat request have no authority. Server derives the client author from the accepted session and creates a new nonempty GUID and UTC timestamp once per accepted message. A client sending a server-only message packet is rejected. Server operator messages use authorRole=server and authorName=Server.

Client validates authoritative nonempty IDs, role client/server, canonical supported client names or fixed Server name, valid text and an ISO timestamp with **zero offset**. Its own echo must match the accepted client name and pending submitted text/session. The protocol does not authenticate names or the server.

## Input limits

Username: reject control/format characters in the original, trim ordinary outer spaces, require 2..24 UTF-16 code units of BMP letters/digits, internal ordinary spaces, underscore or hyphen. Server/System are reserved case-insensitively. Welcome must match the requested canonical name.

Message: 1..2,000 **UTF-16 code units**, not whitespace-only. Preserve content including outer spaces, Turkish/Arabic, paired-surrogate emoji, multiline and tab. Reject unpaired surrogates, NUL and control characters except TAB/CR/LF. JSON escaping still counts toward the 16 KiB frame limit.

Controlled wire error codes: invalid_name, server_busy, invalid_packet, unsupported_version. Remote prose/stack traces are never displayed; Forms map codes to local guidance.

## Admission and lifetime

Default endpoint: 127.0.0.1:5000. UI accepts dotted decimal IPv4 and ports 1..65535; leading-zero octets are decimal. DNS/IPv6 are outside baseline. Tests own port-0 loopback listeners.

Reserve one peer before handshake. Extra clients receive server_busy with bounded initial-frame drain/close. After acceptance, both sides continuously read until close/error/cancellation.

Defaults: connect 5 s; handshake 5 s; frame completion 10 s after its first byte; send 5 s; overall shutdown 5 s. No idle typing timeout. Client send's budget covers both framed write and authoritative echo/display. Server dispatch's budget includes its ordering gate, write and local notification. Observer callbacks must honor cancellation. A timeout/failed write is uncertain and ends the affected session.

## Acceptance, display and duplicates

The server's single dispatch path orders operator/client messages, stamps once, writes once and notifies its local UI once. Client renders only authoritative messages, including its outgoing echo. It does not echo them again or append locally on WriteAsync completion.

Client allows one pending submission. An unrelated Server message cannot confirm it. Clear the submitted draft only after its matching echo has been displayed; retain it on failure. Recent-ID suppression is bounded to 500 IDs per connection; visible-card suppression retains at most the current 500 IDs. Equal text with distinct IDs remains distinct.

Arrival order is preserved regardless of timestamps. Display local HH:mm:ss; expose complete local date/time/offset accessibly. Keep latest 500 visible entries with a persistent trim notice; no hidden saved history.

A completed write/echo is not human receipt or a read acknowledgment. A disconnect can make delivery uncertain. No automatic reconnect, replay or resend, and no exactly-once guarantee across sessions. Closing a session cancels/faults pending acceptance and joins owned work; old callbacks/sends cannot migrate to a new session.

See [architecture](ARCHITECTURE.md), [security](../quality/SECURITY.md) and [executed tests](../quality/TESTING.md).

## P04 metadata and lifecycle boundary — 2026-11-22

ConnectedClientInfo is server-local, immutable accepted-session data, captured from the actual socket endpoint and the server UTC clock only after welcome succeeds. It adds no wire fields or packet types. The Client cannot supply its authoritative endpoint/time/state. Server Listening and accepted-peer Connected are separate concepts.

Explicit Start/Connect join a previous terminal callback/lifetime before creating another session, under a bounded cleanup wait. Disconnect/Stop failures still abort I/O, retain observable owned completion and permit explicit cleanup retry. An expected canceled final peer notification is suppressed only when its server lifetime was canceled.

P04 does not alter delivery guarantees, text/frame limits, recent-ID bounds or protocol deadlines. Local numeric dates/times use invariant formatting after conversion to the user's timezone, so an ISO-shaped date does not silently use a different calendar. No encryption/authentication, replay or automatic recovery protocol is introduced.
