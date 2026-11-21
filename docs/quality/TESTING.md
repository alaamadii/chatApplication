# Validation strategy and current evidence

## P00 initialization evidence — 2026-11-21

Status: implemented initialization; owner acceptance pending. Foundation PR #1 is merged. This section supersedes the historical documentation-only current-state statements below.

Environment: Windows 10.0.22631, PowerShell 5.1.22621.6133, Git 2.53.0.windows.1, selected SDK 10.0.402 and Microsoft.WindowsDesktop.App / Microsoft.NETCore.App 10.0.13.

| Evidence category | Actually executed / result |
| --- | --- |
| Local prerequisite check | ./scripts/dev.ps1 doctor passed; selected C:\\Program Files\\dotnet\\dotnet.exe and reported installed SDK/desktop runtime |
| Automated development check | ./scripts/dev.ps1 check passed: restore, Release build (0 warnings / 0 errors), discovery, dotnet format --verify-no-changes and documentation validation |
| Behavioral tests | **Zero discovered cases** in Core.Tests. Empty Core has no domain behavior; no passing sample/placeholder test or coverage claim |
| Dependencies | Exact Microsoft.NET.Test.Sdk 18.12.0, xunit 2.9.3 and adapter 4.0.1 restored; dotnet list ChatApplication.slnx package --vulnerable --include-transitive found no known vulnerable direct/transitive packages against official nuget.org at the time |
| Actual EXE lifecycle | Started each Release Server/Client EXE independently; found its real role window and expected initialization labels using Win32 enumeration; requested WM_CLOSE; both exited normally with code 0 within bounded waits |
| References / targets | Per-project reference queries and project XML inspection: both apps and Core.Tests reference only Core; Core has no project reference. Core/tests net10.0; apps net10.0-windows |
| Hosted CI | Exact final-head runs/results are recorded in the P00 PR after publication; local execution does not imply hosted success |

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
