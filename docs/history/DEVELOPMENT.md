# Development record

## 2026-11-20 — Documentation/design foundation

Branch: docs/project-foundation. Status: work prepared for owner review, not accepted application functionality.

- Confirmed the requested Chat Application directory and alaamadii/chatApplication repository were empty; remote metadata is public. No ancestor AGENTS.md applied.
- Read the supplied course PDF pages 1, 3 and 5, owner brief and primary technical references. Separated mandatory one-server/one-client behavior from optional rooms/private/history.
- Cloned only the selected repository. Created minimal README/.gitignore main baseline e496391 solely to establish a PR base, then branched for substantive work.
- Inspected installed SDK/Git: 10.0.402 and 2.53.0.windows.1. No application package restore, build, runtime test or listener was started.
- Documented separate Server/Client executables, pure Core, later Networking/shared Presentation, proposed bounded JSON/TCP contract and explicit lifetime/UI dispatch.
- Organized product, technology, architecture, frontend, planning, quality, setup, submission and references. PLAN and TASKS are distinct.
- Added original planned Client/Server SVG layouts and exact tokens. Mockups are labeled proposals, not actual screenshots or evidence.
- Existing completed assignments and other repositories were not modified. Application implementation and optional feature choices remain pending owner review.

Local/hosted documentation check, visual preview, diff and publication receipts are appended only after actual execution.

### Local design/document validation

- Documentation checker passed: 24 required artifacts, 22 Markdown files, 87 local targets, 29 tasks/93 references, 10 contrast pairs and 2 labeled SVGs. Contrast range 5.50 to 15.35; no application coverage is claimed.
- During checker development, XmlDocument.InnerText was not a usable validation source in PowerShell; replaced that check with an explicit labeled SVG text-node query, then reran successfully.
- Rendered original SVG assets through isolated hidden/headless Edge profiles, visually reviewed both roles and refined user-facing footer/limit wording. Rendered previews/profile data are ignored under .local; no application screenshot is claimed.
- App code/behavior/manual acceptance and optional feature approvals remain pending. Publication/hosted results follow confirmed execution.

### Publication and review receipt

- Pushed minimal main baseline e496391 and reviewed foundation commits 06f1150 / bef1ec4 to the selected existing public repository.
- Opened [PR #1](https://github.com/alaamadii/chatApplication/pull/1) for documentation/design review. Hosted documentation run 44729812668 passed.
- Fresh isolated checkout of bef1ec485bb4307d4fe4e073ffbd1d40cfabc45d passed the same documentation checks and stayed clean; no local renderer/profile is required for validation.
- Anonymous README/index/PLAN/TASKS/two-SVG access returned 200. Narrow exclusions, limited credential scan and intended docs-only diff passed.
- Documentation preparation artifacts/checks/publication are complete; owner review remains pending and every application implementation task remains Pending. Final exact-head CI is recorded on PR #1 without self-referential receipt commits.
- No application build, runtime test, listener, merge, visibility change or Classroom submission.

## 2026-11-21 — P00 project initialization

Branch: chore/project-initialization. Foundation PR #1 was confirmed merged at bb9d0fcd4ae4959a4b86177e7497cc01fe45ce89. Inspected a clean tree/correct origin, fetched, fast-forwarded main and created the dedicated phase branch. No owner changes were discarded.

- Added the four-project .slnx: independent empty Core, two separate Windows Forms executables and Core.Tests infrastructure. No Class1, behavioral placeholder, Networking or Presentation project.
- Implemented small native ServerForm/ClientForm initialization windows with separate STA/PerMonitorV2 entry points, visual styles, resizing and truthful unavailable-networking/messaging labels. Native controls own inherited fonts; no custom graphics/resources.
- Pinned verified stable SDK 10.0.402/latestPatch/no prerelease and exact test tooling 18.12.0 / 2.9.3 / 4.0.1. Enabled nullability, deterministic output, warnings as errors and all-dependency NuGet audit using official sources.
- Added repository-scoped doctor/restore/build/test/check/run commands, shared VS Code tasks/role/compound launches and bounded read-only Windows .NET CI. Preserved documentation CI and narrow output/private-data exclusions.
- Reconciled README, actual versus planned architecture/frontend files, stack/setup, requirements, phase/task ledger and evidence. Kept complete frontend/transport/protocol work planned.
- Doctor and development check passed locally: Release build 0 warnings/0 errors, discovery zero cases, format/docs validation. Explicit direct/transitive advisory query reported no known vulnerabilities at official sources. This is not behavioral coverage or a security certification.
- Actual Server and Client EXEs created their expected role/initialization windows and closed via WM_CLOSE with exit 0. Scripted lifecycle evidence is separate from human appearance/F5/DPI acceptance.
- Corrected multi-dotnet PATH selection and the lifecycle probe's premature readiness assumption; reran successfully. Probe/CLI state remain ignored under .local.
- Exact final commit and hosted CI publication results belong in the P00 PR to avoid self-referential receipt commits. P00 owner acceptance, physical UI/F5/DPI and later behavior remain pending.
- No networking/message/username/participant/persistence/optional feature, sibling-project change, merge or Classroom submission.
- Additional local probes confirmed script environment/location restoration after success and injected native exit 23, narrow exclusions, trackable shared configurations, and documentation validation (102 local links, 29 tasks / 92 references). Final diff/staged content is inspected before publication.

### P00 publication receipt

- Published implementation 8bf57d6 and documentation 6b6092181d33637d89f63d0e6e7e955b13eb62d3 to the authorized phase branch and opened [PR #2](https://github.com/alaamadii/chatApplication/pull/2), targeting main.
- Candidate 6b6092181d33637d89f63d0e6e7e955b13eb62d3 passed [Windows .NET run 44870930094](https://github.com/alaamadii/chatApplication/actions/runs/44870930094) and [documentation run 44870930105](https://github.com/alaamadii/chatApplication/actions/runs/44870930105). Inspected Windows job logs: SDK 10.0.402, Release build 0 warnings/0 errors, zero discovered cases and successful docs/format checks.
- Full 37-file phase diff, shared JSON parsing, output exclusions and limited credential-pattern scan passed. Final receipt-commit exact SHA and CI results are maintained in PR #2 after execution. Owner acceptance remains pending; no merge or next-phase work.

## 2026-11-21 — P01 Server and Client interfaces

Branch: feat/server-client-ui. Confirmed owner merge of PR #2 at a4f5acb6fd1e90df55f8c68dd2a1658d24f29599, inspected a clean tree/correct origin, fetched/fast-forwarded main and created the phase branch. Preserved owner changes and sibling assignments.

- Added actual shared Presentation: ChatWindow and layout, UiLayout builders, semantic ChatTheme, per-window OwnedFonts, preview badge and empty TranscriptView.
- Implemented native Server listener settings/empty client list and Client username/endpoint settings; shared empty conversation, disabled composer/actions and persistent P02/P03 guidance. No handlers simulate connections, accepted identity or messages.
- Retained independent STA/PerMonitorV2 executables and empty Core. Apps reference Presentation; Presentation needs no Core reference. Added native App.Tests with the existing exact test-tool pins; no new package/version upgrade.
- Added 15 serial bounded STA integration cases for supported-size layout, small working areas, long literal guidance, unavailable-action behavior, Tab traversal and independently disposed window/font lifetime.
- Resolved initial DPI scaling of incomplete controls, minimum-size scrollbars and long-guidance overlap using suspended construction, native docking/virtual canvas and measured minimum content height. Retained regression assertions. Corrected the fixture's classification of the port spinner's internal editor.
- Doctor and full development check passed: restore, Release build 0 warnings/0 errors, 15 App cases passed, zero Core cases, format and docs validation.
- Started both actual Release EXEs, verified honest empty states/3 disabled actions and normal WM_CLOSE exit 0. Captured and reviewed both roles at default/minimum/wide on the current 125% display, without changing settings. Corrected only the ignored probe's native-class/DPI capture assumptions before final captures.
- Captures/probes remain ignored under .local. Physical keyboard/IME/mnemonics/high contrast/F5, 100%/150%, monitor transitions and another computer remain pending; no owner acceptance was inferred.
- Updated organized frontend, architecture/decisions, setup/stack, requirements, plan/tasks and evidence. Wireframes remain labeled future proposals.
- Publication/hosted receipts are added only after actual execution. No merge, P02 behavior, optional feature, Classroom submission or student number.
- Final local repository/dependency review passed: official direct/transitive advisory query for all six projects found no known vulnerable packages; references, Core independence, narrow exclusions, limited credential scan and diff inspected. Docs check passed with 118 local links and 29 tasks / 92 references.

### Hosted minimum-layout correction

- Published UI implementation 9741c3a and docs candidate 3d0eab5f463e59f9282d0421dd77593ff2b970fc in [PR #3](https://github.com/alaamadii/chatApplication/pull/3). Documentation run 44873823803 passed; Windows run 44873823732 found 2 minimum-layout failures at DPI 96 (13 other native cases passed).
- Required content measured 627 pixels for the 620-pixel client minimum. Reduced title/header gaps by 8 logical pixels without shrinking text or relaxing assertions. Revalidation receipts follow actual execution; physical 100% acceptance is still pending.
- After the spacing fix, full local check passed again (15 native cases, zero Core cases, 0 build warnings/errors, format/docs). Actual EXE lifecycle and final 125% captures were repeated; both closed with exit 0 and the revised layouts were inspected.

### P01 publication receipt

- Fixed implementation d138862c7d4db957bad902de7fc276542c6bddc7 passed [Windows run 44874348238](https://github.com/alaamadii/chatApplication/actions/runs/44874348238) and [documentation run 44874348287](https://github.com/alaamadii/chatApplication/actions/runs/44874348287). Inspected job 136755626271 logs: 0 build warnings/errors, 15 passed native cases, zero Core cases, format/docs success.
- Initial Git HTTPS pushes of the fix encountered transient connectivity failures; an ordinary HTTP/1.1 retry succeeded. No force push, history rewrite, network-policy or TLS weakening.
- Reconciled README/PLAN/TASKS/TESTING with [PR #3](https://github.com/alaamadii/chatApplication/pull/3) and passing implementation checks. Corrected only stale wireframe accessibility descriptions to identify synthetic proposals; wireframe visuals/labels remain unchanged.
- Final documentation receipt is checked/published separately; its exact final SHA and hosted runs belong in PR #3 after execution. P01 owner acceptance and physical F5/keyboard/accessibility/remaining DPI checks stay pending.

## 2026-11-21 — P02 TCP connections and username handshake

Branch: feat/tcp-connections. Confirmed owner merge of PR #3 at e0cb64aa8fc6ff97c782dcd91eb2ec057338a69d, inspected clean tree/remotes, fetched/fast-forwarded main and branched. Resumption preserved that branch; no earlier partial source write existed and no owner changes were discarded.

- Implemented pure UsernameRules and eight-project dependencies: UI-free Networking/Networking.Tests, existing Core/Presentation/Apps/tests retained. No production external package or test-version upgrade.
- Added strict 4-byte signed big-endian framing, payload cap 16 KiB, strict UTF-8/JSON/depth/duplicates/phase-direction checks and hello/welcome/error/disconnect only. No chat/message models or payloads.
- Implemented owned ChatServer/ClientSession/ChatClient lifetimes, one pending/accepted admission slot, serialized bounded rejection, actual accepted identity, cancellation/deadlines and observable/joined tasks.
- Connected native Start/Stop and Connect/Cancel/Disconnect; real status tones/input locking/local controlled feedback. Awaited guarded UI dispatch and normal close join commands/services before controls/fonts. Composer/Send remain unavailable; detailed metadata list stays P04.
- Added meaningful Core/Networking cases and retained/updated native regressions: current total 82 passing cases (19 Core, 46 Networking, 17 App). Release build had 0 warnings/0 errors.
- Found/fixed busy-response reset/discard by bounded initial-frame consumption. Adjusted closed-port fixture for OS refusal versus connect-deadline notification while requiring complete cleanup/no false connection.
- Ran doctor and required check; applied reported whitespace formatting. Final full-check receipt follows reconciled docs. Explicit official direct/transitive audit for all eight projects found no known vulnerable packages at the time.
- Actual separate EXEs on owned loopback port 50634 passed occupied-port guidance, accepted synthetic identity, disconnect/reconnect, server stop/restart, failed endpoint feedback and connected Client close; both exited normally with code 0.
- Reviewed actual connected PrintWindow captures at measured DPI 120 (125%). Corrected only ignored probe provider/selector assumptions before the final complete run; captures/probes stay under ignored .local.
- Updated organized scope, architecture/protocol/decisions, frontend ownership/state, setup/stack, requirements, plan/tasks and exact evidence. Wireframes remain proposals.
- Physical UI/F5/accessibility/remaining DPI/LAN/second-computer checks and owner acceptance remain pending. No P03 messaging, full P04 list, optional feature, merge, sibling change, student number or Classroom submission.
- Final local check passed after formatting and docs reconciliation: all 82 cases, 0 build warnings/errors, format/docs success; docs inventory 127 local links and 29 tasks / 92 references. Project references were inspected and matched the intended platform boundaries.

### P02 publication receipt

- Published networking 179e276, UI integration 612efa6 and documentation candidate efe9b58b06745e4261337cf8a55be3c0ee521321 to feat/tcp-connections; opened [PR #4](https://github.com/alaamadii/chatApplication/pull/4) targeting main.
- Candidate passed [Windows run 44906284347](https://github.com/alaamadii/chatApplication/actions/runs/44906284347) and [documentation run 44906284332](https://github.com/alaamadii/chatApplication/actions/runs/44906284332). Inspected job 136852448835 logs: 0 build warnings/errors, 19 Core + 46 Networking + 17 App cases passed, format/docs success.
- Complete 55-file phase diff, narrow output exclusions, limited credential-pattern scan and clean synchronized tree were verified. No generated/local/private output was published.
- Artifact/check tasks are complete while owner P02 acceptance remains pending. Final documentation receipt SHA and exact hosted runs are recorded in PR #4 after execution; no self-referential receipt commit is needed.
- Physical F5/keyboard/accessibility/remaining DPI/LAN/second-computer evidence remains pending. No merge or P03 messaging.

## 2026-11-21 — P03 real-time timestamped messaging

Branch: feat/realtime-messaging. Verified owner merge of PR #4 at 7e1f95c6167280e017e4a8bf006cfc51cf2f7456, clean tree/correct origin and fast-forwarded main before creating the phase branch.

- Added pure text rules and immutable validated messages; extended strict v1 framing/packets without relaxing P02 limits or directions.
- Added continuous accepted read loops, injectable server UTC clock and one ordered dispatch path for client/operator text. Server owns identity/ID/time; Client renders its own authoritative echo only.
- Added one generation/session-bound pending client submission, bounded recent-ID suppression and cancellation/failed acceptance cleanup. Failure cancels that captured lifetime/socket, joins owned work and never retries automatically.
- Added native message cards with separate literal author/time/body, local accessible date/offset, wrapping and bounded latest-500 retention/disposal/notice. Presentation now references Core, remains socket-free and borrows window-owned fonts.
- Connected both real composers/Send actions, UTF-16 remaining count, shared button/Enter path, Shift+Enter, Tab and IME-state guard. Pending submission locks composition; success clears only unchanged submitted text, while failure/newer input is kept.
- Executed doctor and full development check: restore, Release build 0 warnings/errors, 124 cases (31 Core, 70 Networking, 23 native App), format verification and docs validation, all passing. Ran official direct/transitive advisory query: no known vulnerable packages at execution time. Final-head publication/CI receipts are maintained in the phase PR after execution.
- Exercised actual separate EXEs on owned ephemeral loopback port 57185, repeated after the final source build on 49887: bidirectional Unicode/multiline, explicit author/time, one row per acceptance, equal text twice, reconnect, stop/close during messaging activity and both normal exit 0. Inspected genuine 125% window captures; local probe/images remain ignored.
- Corrected the ignored probe's multiline change-notification, ANSI P/Invoke and post-confirmation readiness assumptions. Initial native font/retention checks exposed empty-label and burst-cost issues; cached width/DPI measurement and verified live native trim/disposal after prefill. No bulk-performance certification is claimed.
- Reconciled organized product, architecture/protocol/decisions, frontend, setup/stack/security, plan/tasks and evidence. Physical keyboard/IME/F5/high contrast/other DPI/monitor/LAN/another computer and final images remain pending.
- Owner acceptance remains pending. No P04/optional scope, student number, merge or Classroom action.

### P03 final local review

- Rejected authoritative timestamps without an explicit zero-offset suffix, with a regression case; validation is independent of the computer's local timezone.
- Full check passed with 124 cases and docs validation: 24 artifacts, 22 Markdown files, 138 local links, 29 tasks/91 references, 10 contrast pairs and 2 labeled SVG proposals.
- Repeated both actual EXE lifecycle/messaging checks after the final source build; both exited 0. Inspected final Unicode/multiline and repeated-message captures at DPI 120.
- Inspected source/diff/references, excluded local/build/TRX output and ran a limited credential-pattern scan with no matches. This is not a comprehensive secret/security certification.

## 2026-11-22 — P04 accepted-client information and local robustness

Branch: fix/connection-lifecycle. Confirmed owner merge of PR #5 at b74825a1fc5b2afad7bde8330d60bad39b41cdee and successful predecessor CI. Inspected clean status/correct origin, fetched, fast-forwarded main and branched without changing sibling assignments.

- Added server-local immutable ConnectedClientInfo from the actual accepted socket and TimeProvider clock after welcome. ConnectionSnapshot carries it without changing wire packets. Source-checked callbacks and joined old lifetimes protect new sessions.
- Completed the native accepted-only information row: name, actual IP/port, local Gregorian date/time with offset and Connected state, measured columns/horizontal scrolling, tooltip and complete accessible description. Pending/rejected/busy peers have no row; disconnect/stop clears live state.
- Reproduced three terminal-callback/Disconnecting-observer failures before fixing them. Explicit Start/Connect now join prior terminal work/sender before replacement, with bounded waits; any Stop/Disconnect failure still aborts I/O.
- Fixed a simultaneous stop/disconnect cancellation escaping the session-final notification. Normal server-lifetime cancellation completes cleanly; unrelated failure remains observable.
- Added command busy guards, cleanup retry and persistent unconfirmed-send guidance independent of status renders/field edits. A successful confirmed send clears that guidance; no reconnect/resend is automated.
- A full visible burst revealed layout cost after an isolated pass. Explicit canvas bounds and batched append/trim layout preserve real native controls/500-card disposal within the existing 20-second fixture deadline. Full-run visible results included 7.57 s and 13.59 s; no throughput certification is claimed.
- Corrected ambiguous numeric calendar display found in actual Arabic-Windows EXEs. Invariant formatting preserves local time/offset; ar-SA metadata rendering is tested.
- Doctor and final ./scripts/dev.ps1 check passed: restore, Release build, 139 cases (Core 31, Networking 80, native App 28), format and documentation checks; 0 failed/skipped and 0 build warnings/errors. Official direct/transitive advisory query reported no known vulnerable packages at execution time. Final-head publication/hosted receipts are maintained in the phase PR after execution.
- Actual separate EXEs on owned loopback port 51276, repeated after final command-input locking on 51230, passed bidirectional Unicode/multiline, exact count/author/time, accepted metadata/removal/fresh reconnect, Stop/restart, intentional owned Client termination, replacement recovery and Server close during activity. Server/replacement Client exited 0; owned port rebind passed. Inspected genuine 125% captures; ignored probes/images remain local.
- Fixture corrections covered IPv4-mapped raw endpoints, OS EOF/reset differences, actual enabled-action readiness and the PowerShell UIA assembly's missing legacy pattern; no production test hooks were added.
- Updated organized product/architecture/protocol/decisions/frontend/setup/security/stack/plan/tasks/evidence. P04-001/002/003 artifacts are Done; P04-004 remains Pending because a second Windows machine/authorized remote target is unavailable. Added a precise deliberate trusted-LAN owner procedure.
- Owner acceptance and physical keyboard/IME/assistive/F5/other-DPI/monitor/LAN evidence remain pending. No P05/P06 implementation, optional feature, student number, merge or Classroom action.

### P04 final local review

- Final development check passed with 139 cases, format verification and documentation validation: 24 artifacts, 22 Markdown files, 144 local links, 29 tasks/106 references, 10 contrast pairs and 2 labeled SVG proposals.
- Verified immediate identity/endpoint locks during owned command preparation and retained failure guidance until successful retry.
- Repeated actual EXE checks after final source changes, including horizontal list scrolling; Server/replacement Client exited 0 and the owned port rebound. The original Client was deliberately terminated to test recovery.
- Inspected references, complete source/diff, narrow exclusions and limited credential-pattern scan (no matches). This is not a comprehensive security/secret certification.
- Trusted-LAN, physical acceptance and owner phase review remain pending; no next phase was begun.

## 2026-11-22 — P06 final-review preparation

Branch: docs/final-submission. Verified PR #6 merged at 1234b22205750059439cb8b5b16349edd4afc95c and successful final-head CI. Inspected clean status/correct origin, fetched/fast-forwarded main and branched without touching sibling assignments.

- Audited mandatory requirements against actual source/tests and preserved the merged runtime baseline, limits and optional-scope deferrals.
- Baseline doctor/check passed: 139 cases (31 Core, 80 Networking, 28 native App), 0 failed/skipped, Release 0 warnings/errors, format/docs success. Official direct/transitive advisory query reported no known vulnerable packages at execution time.
- Actual separate EXEs on owned loopback port 50812 passed occupied/refused error recovery, accepted information, bidirectional Unicode/multiline, intentional equal text twice, disconnect/reconnect, stop/restart, deliberate owned Client termination, replacement recovery and normal Server/replacement Client exit 0; owned port rebind passed.
- Captured three genuine complete application-window PNGs at DPI 120 (125%), from Release source 1234b22205750059439cb8b5b16349edd4afc95c. Visually reviewed synthetic conversation and a scrolled full connection-time view. Copied original bytes into organized frontend assets and embedded them with captions/alt text.
- Added source/build/date/DPI/dimension/image SHA-256 provenance and original assembly fingerprints. Extended check-docs to validate PNG integrity/README embeds; a deliberately incorrect hash was rejected, then original manifest bytes were restored and validation passed.
- Reconciled organized README, requirements/source audit, architecture/protocol/decisions, frontend/file guide, setup/stack/security, plan/tasks and submission evidence.
- The owner reported all requested manual/external checks Not performed on 2026-11-22. Physical input/IME/accessibility/F5/DPI/monitor/LAN and another Windows computer remain Pending. The owner subsequently approved all three concrete screenshot files on 2026-11-22; the manifest and guides record that approval.
- Published-candidate clean-checkout and exact final-head hosted/public-access receipts follow actual execution. Keep the final PR draft while required evidence is missing; no automatic merge/Classroom submission or optional implementation.

- One later full-check repeat hit the existing 20-second native fixture deadline in two send/feedback cases. Both targeted cases and the complete 28-case native suite subsequently passed unchanged; cause was not established. Recorded the observation without increasing deadlines, skipping cases or claiming a runtime fix.

### P06 published candidate and owner screenshot approval

- Published documentation/capture commit 401d1a2 and integrity tooling commit 7c6904219d872df332ff6365249de19970446057; opened draft PR #7 targeting main.
- Exact published candidate passed Windows run 45039966756 and documentation run 45039966757. Windows logs confirm 139 passing cases, 0 failed/skipped, build 0 warnings/errors and format/docs success.
- Cloned exact published candidate over HTTPS into a new isolated ignored folder; doctor/check passed using the tracked configuration. A logged full-check repeat passed unchanged. Actual cloned Release EXEs repeated the controlled error, metadata, messaging, reconnect/restart and recovery scenarios on owned loopback port 55872. Server/replacement Client exited 0 and the port rebound; the original Client was intentionally terminated. The tracked clone remained clean.
- Anonymous HTTP requests to the exact published README, three PNGs and manifest returned 200. Existing public visibility was preserved. Source/tests/build configuration/workflows remain unchanged; generated/local output is excluded.
- Owner explicitly approved all three README screenshot files on 2026-11-22. Updated manifest and evidence consistently; P06-001/002 artifacts are Done. Physical/manual/LAN/second-machine work remains Pending and PR #7 remains draft.
- Final receipt-commit exact SHA, hosted checks and public-access results belong in PR #7 after verification; no self-referential receipt commit or automatic merge.
