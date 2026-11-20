# Validation strategy and current evidence

## Documentation preparation — 2026-11-20

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

All application/manual checks are currently pending. SVG mockups and a docs CI pass cannot complete them.

## Failure/reporting policy

Keep failures reproducible with synthetic content. Record steps, expected/actual behavior and whether the issue belongs to transport, UI or test setup. Fix the underlying defect/fixture and rerun appropriate checks; do not add placeholder tests or quietly skip a failed mandatory scenario.

## Executed local documentation checks — 2026-11-20

- ./scripts/check-docs.ps1 passed: 24 required artifacts, 22 Markdown files, 87 local file links, 29 task definitions/93 references, 10 declared contrast pairs and 2 labeled SVG proposals.
- Selected text-color ratios are 5.50:1 to 15.35:1. This is a token check, not complete UI accessibility evidence.
- git diff --check passed. Original Client/Server SVGs were rendered with an isolated headless Edge profile and visually reviewed; previews remain under ignored .local. These are design previews, not Windows Forms/application checks.
- Application build/behavior tests, physical UI and second-computer results remain not performed because source has not been implemented. Hosted exact-head results are recorded on the foundation PR after execution.
