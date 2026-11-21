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
