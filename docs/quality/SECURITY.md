# Security and data boundaries

Status: P03 implements bounded framing/strict JSON/directions, supported names/text, authoritative server metadata, one pending/accepted peer, cancellation/deadlines and literal message rendering. There is no transcript persistence. These safeguards are not authentication, encryption or a security certification.

## Trust model

Supported use is an explicit local/trusted-LAN course demonstration. TCP is plaintext; usernames are unverified display claims. There are no accounts, passwords, TLS, end-to-end encryption or public internet deployment in the baseline.

A remote peer's bytes are untrusted. Accept a bounded valid frame and permitted packet direction before creating a message. Server derives sender/time/ID; never trust a client's claimed server identity.

Do not scan networks, change firewall/router rules or bind broadly automatically. Loopback is default; LAN binding/endpoint selection is deliberate. An IPv4 address alone is not evidence that a peer is trustworthy.

## Data inventory

| Data | Owner / planned lifetime |
| --- | --- |
| Username | Accepted connection session; display claim, not authentication |
| Peer endpoint / connected time | Server memory while peer is connected; shown for course requirement |
| Message body / ID / server UTC time | Current ordered flow and latest 500 visible messages/window |
| Composer text | Native control until accepted send/user editing; keep on recoverable failure |
| Diagnostic failure | Controlled code/guidance without message bodies, credentials or stack dumps |
| History files | Not implemented; separate optional decision/format/permissions policy required |

No automatic transcript saving, hidden message log, analytics service or remote telemetry exists. Latest-window cards retain message bodies/metadata until trimmed or closed, including across explicit reconnect; only accepted-session live peer data is cleared on disconnect. The pending submission holds one text/TCS until confirmation or failure. Core holds no global data. Tests/wireframes use synthetic content. Public screenshots/issues/PRs must not contain actual private conversations or student numbers.

## Input/resource limits

Implemented connection limits cap frame bytes at 16,384, JSON depth at 8, supported username length at 24 and pending/accepted clients at one. Message text is limited to 2,000 UTF-16 code units and each visible transcript to 500 cards/IDs. Client receive duplicate suppression is also bounded to 500 recent IDs per connection. These sets are not permanent replay ledgers. Validate length before allocation, valid Unicode and required fields before display. Timeouts/cancellation bound incomplete handshakes/frames/writes.

Display plain text, never concatenate received text into RTF/HTML, execute commands, load remote images or open arbitrary links automatically. Preserve literal ampersands. Resource bounds do not authenticate a malicious peer or make a deployed public service safe.

Await UI notifications to apply backpressure instead of posting unlimited callbacks. Dispose removed message controls and own every socket/stream/task/cancellation/font. Clearing/disposal is ordinary reference/resource release, not secure memory erasure.

## Error and shutdown handling

Catch expected validation/socket/I/O/cancellation failures at the owning boundary. A local requested cancel is normal; unrelated failure gets friendly guidance. Never suppress all exceptions to claim stability.

No automatic reconnect/replay: a write may have reached the peer even when the sender cannot confirm it. Keep useful input but require explicit retry. Stop/close must cancel, close owned resources and await tasks without blocking the UI or touching disposed controls.

## Development dependencies and repository

P00 pins SDK/direct test packages, enables all-dependency auditing at low severity and above, and clears inherited package/audit sources in favor of official nuget.org. Actual restore and direct/transitive advisory results are recorded in TESTING/STACK; these are time-bound known-advisory checks, not a vulnerability-free claim. Runtime transport/UI/JSON use the BCL; no API secret or database is required.

Exclude bin/obj/.local/TestResults, personal IDE state, .env/certificates and transcripts. Use transcripts/ or *.chatlog.txt for isolated future test exports; do not ignore arbitrary documentation TXT files. Review staged content and use only owned temporary targets.

## Required verification

Test malformed/oversized/truncated frames, spoofed direction/role, Unicode limits, slow/incomplete peers, extra clients, repeated disconnect/close and UI callbacks during cancellation. Assert no leaked owned tasks/sockets or crash; do not claim a protocol limit is implemented until tests/code exist.

Physical LAN/native UI/high-contrast/another-computer results remain separate from automated loopback tests. See [PROTOCOL](../architecture/PROTOCOL.md), [TESTING](TESTING.md) and [submission](../submission/CHECKLIST.md).
