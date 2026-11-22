# Development environment

Use Windows, Git, PowerShell 5.1+ and a stable .NET 10 SDK allowed by global.json, with the .NET 10 WindowsDesktop runtime. VS Code with the shared C# recommendations is the intended editor. First restore/advisory checking requires official NuGet access; no database, API keys or extra UI framework is needed.

## Checkout and checks

```powershell
git clone https://github.com/alaamadii/chatApplication.git
cd chatApplication
# While the P06 final-review PR is open:
git switch docs/final-submission
./scripts/dev.ps1 doctor
./scripts/dev.ps1 check
```

After owner merge, use main and a fast-forward pull. The local P06 environment on 2026-11-22 reported SDK 10.0.402, .NET/WindowsDesktop 10.0.13, Windows 10.0.22631, PowerShell 5.1.22621.6133 and Git 2.53.0.windows.1.

| Command | Implemented behavior |
| --- | --- |
| ./scripts/dev.ps1 doctor | Selected PATH SDK/runtime, Windows/PowerShell/Git prerequisites |
| ./scripts/dev.ps1 restore | Restore all eight projects with official-source audit policy |
| ./scripts/dev.ps1 build | Restore and Release build |
| ./scripts/dev.ps1 test | Restore/build and execute Core, Networking and native App tests; ignored TRX output |
| ./scripts/dev.ps1 check | Restore, Release build, tests, format verification and documentation check |
| ./scripts/dev.ps1 run-server | Build/run actual Server until it closes |
| ./scripts/dev.ps1 run-client | Build/run actual Client until it closes |
| ./scripts/check-docs.ps1 | Documentation/link/task/token/proposal validation only |

The script temporarily scopes matching DOTNET_ROOT, ignored .local CLI state and first-run certificate suppression; finally restores inherited environment/location. Native failures fail the command/CI. No persistent profile, firewall, execution policy or display setting is changed.

## Actual two-process loopback demonstration

Open two terminals:

```powershell
./scripts/dev.ps1 run-server
```

```powershell
./scripts/dev.ps1 run-client
```

1. Server: keep bind 127.0.0.1 and an available port (default 5000), then Start.
2. Client: enter a supported synthetic name, the same IPv4/port, then Connect.
3. Client progresses through Connecting/Handshaking and becomes Connected only after welcome. Server shows the accepted name, separately from Listening.
4. Disconnect explicitly, then reconnect. Only one pending/accepted peer is allowed; another client receives server_busy.
5. Stop Server: its listener/session close and Client returns to Disconnected.
6. Close either window normally, including while connected. Normal close cancels I/O, joins owned commands/tasks and disposes service resources before disposing controls/fonts.

Names allow 2–24 BMP letters/digits, ordinary spaces, underscore and hyphen after ordinary outer-space trimming; Server/System are reserved. Control/format characters and supplementary symbols are rejected. A name is not authentication.

Inputs use four decimal IPv4 octets and ports 1–65535. Leading-zero octets are interpreted as decimal, not octal; DNS/IPv6 are not supported. Bind port 0 is available through the service API for isolated tests, not the production UI.

Composer and Send remain disabled. No messages or join events appear. The accepted name/status proves the handshake; the detailed metadata list remains P04.

## Debugging and manual checks

Shared Chat: build/check tasks, separate Server/Client launches and compound launch remain available in VS Code. Native tests cover programmatic Tab traversal and layout. Connected-window captures were reviewed on the existing 125% display.

Physical F5/debug stop, keyboard/IME/mnemonics, high contrast/screen reader, 100%/150%, monitor transitions, actual LAN and another Windows computer remain pending. Automated loopback checks do not complete them.

## Errors and troubleshooting

| Situation | Guidance |
| --- | --- |
| Missing SDK/runtime | Run doctor; install the compatible SDK/WindowsDesktop runtime deliberately |
| Restore/audit unavailable | Inspect official feed connectivity; do not commit credentials or hide failures |
| Invalid name/address | Correct the visible inputs; local validation does not replace server validation |
| Occupied port | Choose another port or stop your own listener; never kill unrelated processes |
| Refused/unavailable endpoint | Confirm actual listener/address/port; the OS may report refusal or the connect deadline may expire |
| server_busy | Disconnect the pending/accepted peer, then retry explicitly |
| Handshake/frame deadline | The incomplete operation is canceled/closed; no hidden reconnect |
| Stop/close deadline failure | Failure is reported; owned completion remains observable, not falsely marked successful |
| Send unavailable | Requires an accepted peer, nonblank valid text and no pending submission |

Production deadlines: connect/handshake/send/shutdown 5 seconds each; frame completion 10 seconds after its first byte. An idle accepted connection does not expire merely because no packet starts. Awaited observers must honor their cancellation token; Forms use cancellable UI dispatch.

TCP is plaintext. Use loopback or a deliberately configured trusted LAN; no discovery/firewall/router change is automated.

```powershell
dotnet list ChatApplication.slnx package --vulnerable --include-transitive
```

See [protocol](../architecture/PROTOCOL.md), [testing](../quality/TESTING.md), [security](../quality/SECURITY.md) and [workflow](../planning/WORKFLOW.md).

## P03 two-process message demonstration

Build/check first, then run Server and Client from separate terminals with the commands above. Choose matching 127.0.0.1 and an available nonzero port. Enter a synthetic supported username, start Server, then Connect Client.

Send Turkish/Arabic, emoji and an ampersand from Client; use Shift+Enter for a second line, then reply from Server. Each accepted send should produce one entry at both windows with the real author and local time. Send the same text twice intentionally: expect two entries, not suppression. A pending submission locks its composer until confirmation/failure.

Disconnect/reconnect explicitly and send again. Stop Server or close a window during representative send activity; verify recovery/shutdown without claiming every interrupted message was delivered. Failed/unconfirmed text remains for review, and no automatic resend occurs. Transcripts retain latest 500 in that window only; closing releases them.

Actual scripted separate-EXE checks on 2026-11-21 used an owned ephemeral loopback port, native Unicode character messages/UI Automation and condition-based waits. This is separate from physical keyboard/IME/F5, actual LAN or another-computer checks. See TESTING for results and capture provenance.

## P04 accepted-client and recovery procedure

After Connect, inspect Server's Connected client list. It must show only the accepted username, actual remote client IP/port, local Gregorian connection date/time with offset and Connected state. Scroll horizontally for all columns, or use the complete row tooltip/accessibility description. Listening describes the server, not a pending client's acceptance.

Disconnect explicitly: the row/context clears. Reconnect: a fresh session supplies the row. Stop/close removes live information. If cleanup reports failure, wait for or retry the explicit Stop/Disconnect action; successful completion is not assumed. A retained unconfirmed-send warning clears only after a successful confirmed send, so inspect the draft before retrying.

### Trusted LAN — owner procedure, not performed

1. Use two deliberately selected Windows computers on the same trusted network. Verify compatible .NET SDK/WindowsDesktop runtime and record the exact tested commit/build and actual date.
2. On Server, inspect the intended adapter's IPv4 in Windows network properties or local ipconfig output. Do not publish the full output. Use that actual address as Bind IPv4 and an available nonzero port such as 5000.
3. Start Server. On the other computer set Client's Server IPv4 to that server adapter address and the same port, enter a synthetic name and Connect. 127.0.0.1 refers to each computer itself and cannot reach the other machine.
4. Verify one accepted row with the real remote client endpoint, name/date/state; exchange Turkish/Arabic/emoji/multiline text both ways and intentionally send identical text twice.
5. Disconnect/reconnect, stop/restart and close each program normally. Record observed state/row removal, error recovery and shutdown results. Interrupted delivery must be recorded as uncertain when unconfirmed.
6. If the network blocks the connection, stop and have the owner/admin review the existing network/firewall policy. Do not disable protection or add broad rules automatically.
7. Record Windows/SDK, commit/build, commands, actual outcomes and pending failures. Keep private addresses in owner-local notes or redact them in public evidence.

P04-004 remains Pending because this session has no second Windows machine/authorized remote target for a genuine two-machine test. Two local EXEs and loopback tests do not satisfy it. The P06 second-computer clean-checkout requirement also remains separate.

## Exact published-checkout review

Use the full candidate SHA recorded in the final-review PR. Create an isolated checkout without resetting the working project or deleting existing owner directories:

```powershell
git clone https://github.com/alaamadii/chatApplication.git chat-review
cd chat-review
git switch --detach FULL_PUBLISHED_SHA_FROM_PR
./scripts/dev.ps1 doctor
./scripts/dev.ps1 check
./scripts/dev.ps1 run-server
```

Run ./scripts/dev.ps1 run-client from another terminal in that same checkout. Use synthetic data and an available matching loopback port. Verify bidirectional text/timestamps, metadata, explicit reconnect, controlled errors and normal shutdown. Record the actual date, SHA, Windows/SDK and outcomes in the review receipt.

Replace FULL_PUBLISHED_SHA_FROM_PR with the actual full commit hash; it is an instruction placeholder, not a test result. Build output/CLI/TRX remain ignored. A fresh checkout must not depend on ignored capture probes. The tracked development/documentation commands are sufficient to restore/build/test and verify PNG hashes/embeds.

This review is on the same computer unless a different machine is explicitly identified. It does not satisfy the owner-reported missing second-computer or trusted-LAN evidence.
