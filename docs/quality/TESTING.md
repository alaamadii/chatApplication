# Validation strategy and current evidence

## P03 messaging evidence — 2026-11-21

Status: implementation complete, owner acceptance pending. Confirmed [PR #4](https://github.com/alaamadii/chatApplication/pull/4) merged at **7e1f95c6167280e017e4a8bf006cfc51cf2f7456**. Inspected a clean tree/correct origin, fetched, fast-forwarded main and created **feat/realtime-messaging**. Sibling assignments were not changed.

| Evidence | Executed result |
| --- | --- |
| Environment | ./scripts/dev.ps1 doctor passed: Windows 10.0.22631, PowerShell 5.1.22621.6133, Git 2.53.0.windows.1, SDK 10.0.402, .NET/WindowsDesktop 10.0.13 |
| Behavioral suite | ./scripts/dev.ps1 check passed: **124 passed, 0 failed, 0 skipped** — Core 31, Networking 70, native App 23 |
| Build | Release build passed, 0 warnings/0 errors |
| Formatting | dotnet format ChatApplication.slnx --no-restore applied whitespace; final verify is recorded below |
| Dependencies | dotnet list ChatApplication.slnx package --vulnerable --include-transitive used official nuget.org and reported no known vulnerable packages in all eight projects at execution time |
| Full check | ./scripts/dev.ps1 check passed: restore, Release build, all 124 cases, format verification and documentation validation (24 artifacts, 22 Markdown files, 138 local links, 29 tasks/91 references, 10 contrast pairs, 2 labeled SVG proposals) |
| Hosted CI | Exact final commit and Windows/documentation results are maintained in the phase PR after publication; local results are not hosted evidence |

### Coverage and regression evidence

Core verifies verbatim text, 1/2,000/2,001 UTF-16-unit boundaries, emoji pairs, Arabic/Turkish/multiline/tab, whitespace/control/NUL/unpaired-surrogate rejection and immutable model author/ID/UTC validation.

Networking preserves P02 framing/depth/strict encoding/duplicate/direction/deadline/lifecycle coverage. Final review added explicit UTC-suffix validation and a no-zone timestamp rejection case so behavior does not depend on the machine's local timezone. Added chat/message round trips and field rejection, encoded Unicode limits, raw forged identity and server-only packet rejection, real two-way exchange, injectable server clock, identical text with distinct IDs and concurrent operator/client sends with matching endpoint order.

Pending-send cases cover peer close, acceptance timeout, caller cancellation, explicit disconnect, rejection of parallel submissions and fresh-session reconnect. An unrelated Server message does not confirm a client's text; recent duplicate IDs are suppressed. A cancellation-aware in-flight display observer is canceled/joined on acceptance failure. Server Stop joins an active message observer before releasing owned session work.

Native tests retain layout/resource/connection regressions and add separate literal author/body/time, accessible complete time, wrapping/order, duplicate IDs, one pending send, button/synthetic Enter, no optimistic copy, unchanged/newer draft handling, failed text retention, close during pending send and real-Form bidirectional exchange. Retention pre-fills 500 cards before showing the Form, then trims live native cards; it verifies the cap, persistent notice and removed/remaining control disposal.

An empty hidden label initially broke the font-lifetime regression; it now has meaningful zero-count text. The initial full-visible 502-message burst exceeded the fixture's 20-second deadline. Stable-width measurement was cached, and retention validation now separates prefill from the live trim boundary. This is not a bulk-throughput benchmark or physical usability acceptance.

### Actual two-process check

An ignored probe launched the actual Release Server and Client EXEs as separate processes, using synthetic data and an owned port-0 loopback reservation. Native Unicode character messages filled composition, UI Automation invoked actions, and bounded conditions waited for states/counts rather than fixed delays.

The first complete pass used **127.0.0.1:57185**; a repeat after the final source build used **127.0.0.1:49887** and passed the same checks:
1. Real listen/username welcome with Alaa_Test.
2. Client -> Server Arabic/Turkish/emoji, multiline, ampersand and literal markup; Server -> Client reply.
3. Exactly four entries at each window after four sends, including two deliberate sends of repeat.
4. Exact author/body labels and four separate local HH:mm:ss timestamps.
5. Explicit disconnect/reconnect and a fifth message accepted once at both windows.
6. Stop during representative send activity, observed disconnect and explicit server/client restart.
7. Close Client during representative messaging activity, peer removal, then close Server; both exited **0**.

An interrupted send may be delivered or uncertain; these shutdown checks do not assert delivery of interrupted text. The probe waits for the composer to become available after prior confirmation.

Earlier fixture attempts used WM_SETTEXT without a multiline change notification, then an ANSI character-message P/Invoke that corrupted synthetic Unicode. The final fixture uses Unicode WM_CHAR and readiness conditions. Failed attempts cleaned up only their own processes; no production test hook or physical keystroke claim was introduced.

Captured and inspected actual Server/Client windows via PrintWindow at measured **DPI 120 (125%)**. Source: this branch's P03 Release working tree on 2026-11-21. Images/probe remain ignored under .local/p03-review; no published screenshot or owner approval is claimed. The final repeat also captured and reviewed the first Unicode/multiline exchange before later repeated messages, showing literal markup, separate author/time and incoming/outgoing alignment.

### Owner checks still pending

- Review P03 appearance, messaging and PR before P04.
- Physical Enter/Shift+Enter/Tab, held keys, focus switching, Turkish/Arabic IME and mnemonic behavior.
- High contrast/screen reader, actual 100%/150% and monitor transitions.
- VS Code role/compound F5 and debugger stop.
- Actual trusted LAN, another Windows computer, final completed-app screenshots and owner image approval.

The detailed client-information list, broader P04 robustness matrix and optional features are outside this phase.

## Historical P02 connection evidence — 2026-11-21

Status: implemented connection subset; owner acceptance pending. Confirmed [PR #3](https://github.com/alaamadii/chatApplication/pull/3) merged at e0cb64aa8fc6ff97c782dcd91eb2ec057338a69d, inspected a clean tree/correct origin, fast-forwarded main and created feat/tcp-connections.

| Evidence | Executed result |
| --- | --- |
| Prerequisites | ./scripts/dev.ps1 doctor passed: Windows 10.0.22631, PowerShell 5.1.22621.6133, Git 2.53.0.windows.1, SDK 10.0.402, .NET/WindowsDesktop 10.0.13 |
| Core | 19 supported-name/normalization/rejection cases passed |
| Networking | 46 framing/strict JSON/endpoint/deadline/concurrent-write/real loopback lifecycle cases passed |
| Native App | 17 serial STA layout/action/navigation/handshake/reconnect/close/resource cases passed |
| Current total | **82 passed, 0 failed, 0 skipped**; no zero-case/placeholder coverage claim |
| Build | Release build passed with 0 warnings/0 errors |
| Full check | ./scripts/dev.ps1 check passed after reported whitespace corrections: restore, Release build, all 82 tests, format and documentation validation |
| Dependencies | Explicit official nuget.org direct/transitive advisory query for all eight projects reported no known vulnerable packages at execution time; versions remain pinned |
| Hosted CI | Candidate efe9b58b06745e4261337cf8a55be3c0ee521321 passed [Windows run 44906284347](https://github.com/alaamadii/chatApplication/actions/runs/44906284347) and [documentation run 44906284332](https://github.com/alaamadii/chatApplication/actions/runs/44906284332); exact final receipt-commit runs/results are maintained in [PR #4](https://github.com/alaamadii/chatApplication/pull/4) |

### Actual two-process demonstration

Launched the actual Release Server and Client EXEs independently. An isolated ignored probe drove their existing UI using UI Automation/native messages; it added no production test hooks. Used only owned loopback port **50634**, selected through an owned port-0 listener.

Executed and passed:
1. Held that port with the owned listener: Server Start showed friendly occupied-port feedback without crashing.
2. Released the owned listener, started Server and observed actual Listening.
3. Connected Client as synthetic Alaa_Test, observed Connected after welcome and Server's accepted name.
4. Verified native Send disabled and multiline composer disabled/empty; no message or participant metadata rows.
5. Explicit Client Disconnect cleared both live contexts; reconnect succeeded.
6. Server Stop released the listener and Client returned to Disconnected.
7. Connecting to the stopped endpoint showed friendly refused/timed-out feedback.
8. Explicit Server restart and Client reconnect succeeded.
9. Closed the connected Client via WM_CLOSE; it exited 0 and Server cleared its accepted peer.
10. Closed Server via WM_CLOSE; it exited 0. No probe-owned process remained.

Captured and visually reviewed both connected windows using PrintWindow at measured **DPI 120 (125%)**. Images/probes remain ignored under .local/p02-review; they are not published course screenshots or owner approval.

The local UI Automation provider lacked a numeric value pattern, and label/input names could be ambiguous. The probe was corrected to target the owned native port editor and multiline composer rather than an enabled label or locked identity input. Failed probe attempts cleaned up only their own processes. The final complete probe passed; these were fixture corrections, not invented application results.

### Meaningful boundaries and resolved defects

Tests cover fragmented/coalesced frames, invalid prefix values before allocation, exact 16 KiB payload, strict UTF-8/JSON/depth/duplicate/direction rejection, truncated versus clean EOF, serialized writes, frame timeout/read cancellation and write timeout/unwinding. Idle reads wait for caller cancellation rather than a first-byte deadline.

Real loopback tests cover hello/welcome, reserved names, mismatched welcome/unknown error, pending-slot timeout/release, busy rejection, failed/canceled connect, stop during incomplete input, repeated reconnect/restart and completed owned tasks. Short test deadlines are configurable; arrivals/cleanup are verified by events or task completion, not fixed-delay assumptions.

An early busy-rejection test exposed reset/discard when closing with unread hello bytes. The accept loop now sends server_busy and consumes at most one bounded initial frame before closing. Closed-port tests accept the OS's controlled refusal or connect-deadline outcome while still requiring Disconnected and completed owned work. No assertion was relaxed to permit a live/false connection.

P01 layout/lifetime regressions remain. Only obsolete all-actions-disabled/field-only Tab assumptions were updated to actual P02 actions; native tests also exercise live handshake, reconnect, active Client close and occupied-port recovery.

### Final local receipt

After the formatter corrections and documentation reconciliation, ./scripts/dev.ps1 check passed completely: restore, Release build (0 warnings/0 errors), 19 Core + 46 Networking + 17 App cases, format verification and docs validation. The docs checker reported 24 required artifacts, 22 Markdown files, 127 local links, 29 tasks/92 references, 10 contrast pairs and 2 labeled SVG proposals.

Per-project reference inspection confirmed Core and Presentation have no project references; Networking references Core only; Apps reference Core/Networking/Presentation. Local native/demo output remains excluded. Exact publication/hosted results follow execution in the phase PR.

### Remaining owner checks

- Review real P02 connection behavior/appearance and approve the PR before P03.
- Physical keyboard/IME/mnemonics/focus, high contrast/screen reader and VS Code role/compound F5/debug stop.
- Actual 100%/150% scaling, monitor transitions and deliberate trusted-LAN demonstration.
- Another Windows computer and final working-chat screenshots remain later-phase evidence.
- Messaging, server-stamped message time/author/echo, full participant metadata/list and optional features are not implemented or accepted.

## Historical P01 interface evidence — 2026-11-21

Status: presentation implemented; owner acceptance pending. P00 [PR #2](https://github.com/alaamadii/chatApplication/pull/2) was merged at a4f5acb6fd1e90df55f8c68dd2a1658d24f29599 before the phase branch was created. The P00/foundation receipts below are historical.

| Evidence category | Executed result |
| --- | --- |
| Environment | Windows 10.0.22631, PowerShell 5.1.22621.6133, Git 2.53.0.windows.1, selected SDK 10.0.402 and .NET/WindowsDesktop 10.0.13 |
| Prerequisites | ./scripts/dev.ps1 doctor passed |
| Full automated check | ./scripts/dev.ps1 check passed: restore, Release build with 0 warnings/0 errors, test execution/discovery, format verification and existing docs check |
| Native App.Tests | **15 passed, 0 failed, 0 skipped**; serial bounded STA checks, actual Forms owned/disposed by each test |
| Core.Tests | **Zero cases**; no Core behavior was added and no placeholder case/coverage is claimed |
| Layout assertions | Both roles at 1120 x 720, 900 x 620 and 1440 x 900 logical client sizes: reachable controls, nonoverlap, wrapped labels and no unnecessary root scrollbars |
| Small display / long guidance | A supplied 640 x 480 physical working area bounds the actual Form and supports both scroll directions; expanded literal feedback remains reachable without overlapping the transcript/composer |
| Unavailable actions | Local input editing plus PerformClick attempts leave transcript labels unchanged, client rows empty and composer empty; disabled native buttons cannot perform connection/message work |
| Navigation / resource lifetime | Programmatic Tab traversal reaches editable settings and skips unavailable controls; closing/disposal of Server leaves Client fonts, repaint and resizing valid |
| Actual EXE lifecycle | Started each Release executable independently, found its actual role window/empty-state labels, verified all 3 action buttons disabled, requested WM_CLOSE and observed normal exit 0 within bounded waits |
| Hosted CI | Fixed implementation d138862c7d4db957bad902de7fc276542c6bddc7 passed [Windows run 44874348238](https://github.com/alaamadii/chatApplication/actions/runs/44874348238) and [documentation run 44874348287](https://github.com/alaamadii/chatApplication/actions/runs/44874348287); exact final receipt-commit results are maintained in [PR #3](https://github.com/alaamadii/chatApplication/pull/3) |

### Actual visual review

Captured each real executable's window through PrintWindow using an isolated DPI-aware probe, then visually inspected both roles at default/minimum/wide sizes. Existing monitor DPI was **120 (125%)**; no machine display setting was changed.

| Layout | Logical client | Measured physical client at 125% |
| --- | --- | --- |
| Default | 1120 x 720 | 1400 x 900 |
| Minimum | 900 x 620 | 1125 x 775 |
| Wide capture | 1360 x 800 | 1700 x 1000 |

Reviewed clear hierarchy, role/status guidance, native settings, disabled actions, empty conversation and composer, wrapping and available scrolling. The Server sidebar scrolls at minimum height; its empty native metadata list exposes columns through horizontal scrolling. These captures are ignored in .local/p01-review, not published course screenshots or owner approval.

The first capture helper was DPI-unaware and cropped physical pixels; it was corrected to scope/restore its own thread DPI context before recapturing. Native button class detection was also made case-insensitive. Probe failures cleaned up only their owned process; final probes passed. These were probe defects, not accepted UI screenshots.

### Defects found and resolved

Native/visual checks exposed early scaling of an incompletely constructed base Form, unnecessary minimum-size root scrollbars and long-guidance overlap. Layout construction is now suspended until role controls exist. A docked virtual canvas and measured content minimum preserve readable transcript/composer space and grow reachable scrolling for long text.

Regression checks were retained. The navigation fixture was corrected to treat NumericUpDown's internal editor as part of one native setting; final traversal checks pass both roles.

### Hosted minimum-layout regression

Initial P01 candidate 3d0eab5f463e59f9282d0421dd77593ff2b970fc passed documentation CI but failed [Windows run 44873823732](https://github.com/alaamadii/chatApplication/actions/runs/44873823732): 13 native cases passed and the two 900 x 620 minimum-size cases failed at DPI 96. Measured required content height was 627 rather than 620, producing unwanted root scrollbars.

Reduced only the title/header gaps by 8 logical pixels, retaining all font sizes, controls, readable transcript space and regression assertions. After the fix, local full check passed all 15 native cases and fixed implementation d138862c7d4db957bad902de7fc276542c6bddc7 passed both hosted workflows; Windows job 136755626271 logs confirmed 0 build warnings/errors, 15 passing App cases, zero Core cases and successful format/docs checks. This CI geometry result is not physical owner acceptance at 100%.

### Repository and dependency review

- Official nuget.org direct/transitive advisory query passed for all six projects with no known vulnerable packages reported at execution time.
- Inspected actual references/targets: Apps -> Core/Presentation; Presentation and Core have no project references; App.Tests -> both Apps/Presentation. Core source/test infrastructure is unchanged.
- Narrow ignore probes passed for build/obj/TestResults/local captures/personal settings/credentials/transcripts and confirmed source/shared configuration/documentation TXT remain trackable.
- Limited credential-pattern scan and staged source/diff review passed. No generated output, local capture, future Networking project or transport/background implementation is included.
- Updated docs check passed: 24 required artifacts, 22 Markdown files, 118 local file links, 29 task definitions / 92 references, 10 contrast pairs and 2 labeled SVG proposals.

### Remaining owner/manual checks

- Owner review/acceptance of real appearance, editing and resizing before P02.
- Physical Tab/Shift+Tab, Alt mnemonics, focus switching, Turkish/Arabic/IME input and assistive reading.
- Physical high contrast/screen reader; system-color fallback is implemented but not manually accepted.
- Physical VS Code F5 for both roles and compound launch, breakpoints/debug stop.
- Actual 100%/150% scaling and monitor transitions. The 125% capture review does not supply these results.
- Second Windows computer and final working-chat screenshots remain later-phase evidence.

No transport, handshake, connection state machine, message exchange, message rendering or participant tracking was implemented/tested. Scripted native checks and captured-window review are separate from human/physical acceptance.

## Historical P00 initialization evidence — 2026-11-21

Status: implemented initialization; owner acceptance pending. Foundation PR #1 is merged. This section supersedes the historical documentation-only current-state statements below.

Environment: Windows 10.0.22631, PowerShell 5.1.22621.6133, Git 2.53.0.windows.1, selected SDK 10.0.402 and Microsoft.WindowsDesktop.App / Microsoft.NETCore.App 10.0.13.

| Evidence category | Actually executed / result |
| --- | --- |
| Local prerequisite check | ./scripts/dev.ps1 doctor passed; selected `C:\Program Files\dotnet\dotnet.exe` and reported installed SDK/desktop runtime |
| Automated development check | ./scripts/dev.ps1 check passed: restore, Release build (0 warnings / 0 errors), discovery, dotnet format --verify-no-changes and documentation validation |
| Behavioral tests | **Zero discovered cases** in Core.Tests. Empty Core has no domain behavior; no passing sample/placeholder test or coverage claim |
| Dependencies | Exact Microsoft.NET.Test.Sdk 18.12.0, xunit 2.9.3 and adapter 4.0.1 restored; dotnet list ChatApplication.slnx package --vulnerable --include-transitive found no known vulnerable direct/transitive packages against official nuget.org at the time |
| Actual EXE lifecycle | Started each Release Server/Client EXE independently; found its real role window and expected initialization labels using Win32 enumeration; requested WM_CLOSE; both exited normally with code 0 within bounded waits |
| References / targets | Per-project reference queries and project XML inspection: both apps and Core.Tests reference only Core; Core has no project reference. Core/tests net10.0; apps net10.0-windows |
| Hosted CI | Candidate 6b6092181d33637d89f63d0e6e7e955b13eb62d3 passed [Windows .NET run 44870930094](https://github.com/alaamadii/chatApplication/actions/runs/44870930094) and [documentation run 44870930105](https://github.com/alaamadii/chatApplication/actions/runs/44870930105); final receipt-commit SHA/results are maintained in [PR #2](https://github.com/alaamadii/chatApplication/pull/2) |

Lifecycle verification was a scripted Windows check, not human appearance acceptance or a behavioral test case. The first probe relied on WaitForInputIdle and looked for the window too early; it was corrected to poll the actual expected window/label conditions with a deadline. The failed probe's owned process was cleaned up; the corrected probe passed both programs. The probe remains ignored under .local and is not part of application code.

A PATH containing multiple dotnet executables exposed a script-selection issue during initial doctor execution. The script now selects the first application explicitly; doctor/full check subsequently passed. It scopes SDK-root/CLI-home/certificate-generation settings and restores inherited state/location even when a tool fails.

### Additional local tooling/repository checks

- Invoked doctor by absolute script path from another directory and confirmed inherited working directory, DOTNET_ROOT, DOTNET_CLI_HOME and certificate-generation setting were restored.
- Injected a temporary local dotnet command returning exit 23: restore threw as expected and still restored the inherited environment/location. This probe is ignored local tooling verification, not a Core behavioral case.
- Narrow git check-ignore probes passed for bin/obj/.local/TestResults, credentials/personal settings/transcripts, and confirmed source, shared VS Code files and documentation TXT remain trackable.
- Final documentation check passed with 24 required artifacts, 22 Markdown files, 102 local file links, 29 task definitions / 92 references, 10 contrast pairs and 2 labeled SVG proposals. git diff --check passed.
- Inspected project references, target frameworks and shared configuration; no runtime external package, future layer/model or generated output is intended for the commits.

### Remaining P00 owner checks

- Human review of both initialization windows and resizing/default/minimum appearance.
- Physical VS Code F5 (Server, Client and compound), debugger stop, keyboard/focus and normal manual close.
- Display scaling at 100%, 125%, 150% and monitor transitions where available.
- P00 PR review/merge before P01.

No chat functionality, full P01 interface, physical DPI/F5, second-computer verification or genuine application screenshots are claimed. Later acceptance scenarios remain pending.

## Historical documentation preparation — 2026-11-20

Current scope contains documentation, original SVG proposals, tokens and documentation tooling only. Application behavior tests: **not created/not executed**. No app was built, connected, run or visually accepted.

The current check command is ./scripts/check-docs.ps1. It validates document inventory/local links, task ID references, SVG design-proposal labeling/parse safety and ten selected palette contrast pairs. These are documentation assertions, not chat test coverage or a complete accessibility audit.

Local/hosted result receipts and the exact final PR commit are recorded after execution in [DEVELOPMENT](../history/DEVELOPMENT.md) and the foundation PR. Do not invent counts before checks run.

## Planned application checks

| Phase / area | Meaningful evidence |
| --- | --- |
| P00 tools | Doctor/restore/Release build/test discovery/format, correct references, actual Server/Client EXE start/normal close; zero cases reported honestly if no behavior |
| P01 UI | Native STA action states, input labels/tab order/mnemonics, minimum/wide/long-error layout and no fake connected/messages |
| P02 codec | Byte-by-byte/coalesced frames, exact prefix/payload, min/max/negative lengths, malformed UTF-8/JSON, version/type/direction, clean/truncated EOF |
| P02 transport | Ephemeral owned loopback ports, valid/rejected username, handshake timeout, extra-client rejection, connect cancel and repeat start/stop |
| P03 messages | Unicode/Turkish/Arabic/emoji/multiline, validation, trusted sender/time, intentional identical messages retained, one echo rendering, ordered writes |
| P03 UI | Awaited dispatch, responsive send, composer retention on error, no disposed-control updates |
| P04 robustness | Port in use, refused endpoint, abrupt peer drop, normal disconnect, reconnect, stop/close during accept/read/write/dispatch |
| P04 limits | Transcript 500-item trim notice, resource ownership, frame deadlines, no unbounded posted UI queue |
| P04 LAN | Actual intended bind/peer addresses, date/commit/results; no automatic firewall/router modifications |
| P06 delivery | Exact public checkout/check/run/close, current CI, actual screenshots/public links, another Windows computer and owner approval |

Pure tests must not require WinForms. Transport integration uses bounded waits/cancellation, owned processes and ephemeral ports; no fixed sleep as proof that a message arrived. Native UI tests run serial STA windows with synthetic content and deterministic time when appropriate.

## Manual acceptance

Record actual date, exact commit, Windows/SDK and result for each:

1. Two actual processes connect with a username, exchange messages once in both directions and show time/client information.
2. Physical keyboard Enter/Shift+Enter, Tab/mnemonics, Unicode/IME input and readable errors.
3. Failed connect, occupied port, peer/server close and repeated explicit reconnect.
4. Default/minimum/wide windows, 100%/125%/150% and actual monitor transitions available to the owner.
5. Native high contrast/screen reader where available and actual VS Code F5.
6. Normal close during active I/O with no lingering owned process/work.
7. Different Windows computer: clone exact commit, doctor/check/run, repeat representative connection/message/error checks.
8. Two genuine screenshots reviewed by owner with source/date/method/DPI/hashes.

These final chat/physical acceptance checks remain pending. P00 scripted initialization results above do not complete them.

## Failure/reporting policy

Keep failures reproducible with synthetic content. Record steps, expected/actual behavior and whether the issue belongs to transport, UI or test setup. Fix the underlying defect/fixture and rerun appropriate checks; do not add placeholder tests or quietly skip a failed mandatory scenario.

## Executed local documentation checks — 2026-11-20

- ./scripts/check-docs.ps1 passed: 24 required artifacts, 22 Markdown files, 87 local file links, 29 task definitions/93 references, 10 declared contrast pairs and 2 labeled SVG proposals.
- Selected text-color ratios are 5.50:1 to 15.35:1. This is a token check, not complete UI accessibility evidence.
- git diff --check passed. Original Client/Server SVGs were rendered with an isolated headless Edge profile and visually reviewed; previews remain under ignored .local. These are design previews, not Windows Forms/application checks.
- Application build/behavior tests, physical UI and second-computer results remain not performed because source has not been implemented. Hosted exact-head results are recorded on the foundation PR after execution.

## Historical published foundation receipt — 2026-11-20

- [PR #1](https://github.com/alaamadii/chatApplication/pull/1) targets main from docs/project-foundation and was published for owner documentation/design review and is now merged.
- Published tooling candidate bef1ec485bb4307d4fe4e073ffbd1d40cfabc45d passed documentation CI run 44729812668.
- A fresh isolated checkout of that exact candidate passed the same docs check: 24 required artifacts, 22 Markdown files, 87 local targets, 29 tasks/93 references, 10 contrast pairs and 2 labeled SVGs. The clone stayed clean.
- Anonymous README/index/PLAN/TASKS/Client-SVG/Server-SVG URLs returned HTTP 200. Existing public visibility was preserved.
- Final publication-receipt SHA and exact hosted run/results are recorded on PR #1; later edits reconcile documentation status only.

These are documentation and design checks. Application functionality, native WinForms/physical acceptance and another Windows computer remain unperformed. This historical receipt predates owner merge of PR #1; P00 acceptance remains pending.
