# Proposed TCP protocol v1

Status: target contract for P02/P03, not implemented. Review this contract before transport coding; update DECISIONS if it changes.

## Framing

A frame is a **4-byte signed big-endian payload length**, followed by exactly that many UTF-8 JSON bytes. Length excludes the prefix and must be 1..16,384. Reject zero, negative and oversized values before allocating a payload.

TCP reads may split a prefix/payload or combine multiple frames. Loop until the required bytes are available; do not assume one read equals one message. EOF before any prefix byte is a clean remote close; partial prefix/payload EOF is a truncated-frame error. [ReadExactlyAsync](https://learn.microsoft.com/en-us/dotnet/api/system.io.stream.readexactlyasync?view=net-10.0) illustrates exact buffered reading/cancellation, but initial EOF classification still needs explicit handling.

Use strict UTF-8, one complete JSON object with no duplicate property names, camelCase fields, version=1 and a bounded JSON depth of 8. Reject invalid encoding/JSON, unsupported version/type, wrong-direction packets and missing/invalid required fields. Extra fields may be ignored within those limits; do not execute/render markup.

One reader per stream; an async gate serializes prefix+payload writes as one logical operation. No concurrent partial-frame writes.

## Packet directions

| Type | Direction | Required data | Meaning |
| --- | --- | --- | --- |
| hello | Client -> Server, first packet | version, type, userName | Request display name |
| welcome | Server -> Client | version, type, sessionId, userName | Accepted name/session; now Connected |
| chat | Client -> Server after welcome | version, type, text | Request message submission |
| message | Server -> Client | version, type, messageId, authorRole, authorName, text, sentAtUtc | Authoritative accepted message |
| error | Server -> Client | version, type, code | Controlled failure, no stack/private data |
| disconnect | Either accepted peer | version, type | Best-effort orderly close, not a delivery guarantee |

Reserve the one admission slot before awaiting a handshake; release it on timeout/failure/disconnect, so pending handshakes cannot grow without bound.

Handshake packets/codec arrive in P02; chat/message behavior arrives in P03. Do not present the full target as implemented in earlier phases.

Example payloads below are illustrative, not execution evidence. Prefix values must be calculated from encoded byte length, not characters:

```json
{"version":1,"type":"hello","userName":"Alaa"}
```

```json
{"version":1,"type":"message","messageId":"11111111-1111-4111-8111-111111111111","authorRole":"client","authorName":"Alaa","text":"Hello!","sentAtUtc":"2026-11-20T09:00:00+00:00"}
```

IDs use GUID strings. UTC timestamps accept valid ISO-8601 offset-zero representations. Author role is client or server. Server is the fixed operator display name; reserve Server/System names case-insensitively for control identity, not account authentication.

## Validation and limits

Username: reject control/format characters in the original value, trim ordinary leading/trailing spaces, then require 2..24 UTF-16 code units. Allow BMP letters/digits plus ordinary internal spaces, underscore and hyphen; reject reserved names and unsupported symbols. This supports ordinary Turkish/Arabic names; emoji/supplementary-character usernames are outside v1.

Message: 1..2,000 UTF-16 code units, valid Unicode, not whitespace-only. Preserve actual content; reject NUL/control characters except tab/CR/LF. Emoji is allowed when represented by valid surrogate pairs. Frame byte limit still applies after JSON escaping.

Server derives the client's author from the accepted session. Do not accept forged message identity: a client sending server-only message packets is rejected. Client cannot supply authoritative timestamps/roles.

Proposed controlled error codes: invalid_name, server_busy, invalid_packet, unsupported_version. Map them to local friendly text; do not trust arbitrary remote error strings.

## Session/lifetime policy

Default loopback endpoint: 127.0.0.1:5000. Production UI accepts literal IPv4 endpoints for the baseline; DNS/IPv6 support is deferred. Test servers bind port 0 and publish their actual endpoint.

Target deadlines: connect 5 s, handshake 5 s, frame completion after its first byte 10 s, send 5 s, shutdown 5 s. An idle accepted conversation does not expire merely because no one types. Deadlines are cancellable and must not leave a blocked read/task behind.

Send failure/EOF transitions to Disconnected, clears live-peer state and retains visible text for review. Canceling an intentional local stop is not an error popup. No automatic reconnect/resend; user action is explicit.

## Delivery/UI semantics

Server creates message ID/time once per accepted message and serializes dispatch. Client appends only the server echo for its own send, preventing local-copy/echo duplication. Server operator messages share the dispatch path. Maintain arrival order, not sorted timestamps.

A completed socket write is not a delivery/read acknowledgment. Reconnect does not replay old traffic. Optional future rooms/history require a contract review; do not silently change v1 semantics.

Tests: byte-by-byte prefix/payload, coalesced frames, Unicode/escaping, all boundary lengths, invalid packets/directions, handshake timeout, clean/truncated EOF, cancellation and concurrent writes. See [TESTING](../quality/TESTING.md).
