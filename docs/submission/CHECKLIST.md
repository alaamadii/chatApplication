# Submission candidate and owner evidence

**P06 draft final-review candidate; not submission-ready.** The owner merged [PR #6](https://github.com/alaamadii/chatApplication/pull/6) at 1234b22205750059439cb8b5b16349edd4afc95c. Runtime code is implemented and tested locally; required external/physical evidence remains missing.

Repository: [alaamadii/chatApplication](https://github.com/alaamadii/chatApplication). Preserve the selected repository/public visibility. Author: **Alaa Madi**. The original sheet requests a student number; the owner directs omission. No number is requested/published and no instructor waiver is claimed.

## Preparation and acceptance

| Item | Current evidence / state |
| --- | --- |
| Separate source repository, .gitignore, staged history | Existing repository and owner-merged phase PRs |
| Mandatory same-computer functionality | Implemented; source/test mapping in REQUIREMENTS; 139 cases and actual separate processes |
| README purpose/stack/setup/recovery/limitations | Reconciled P06 guide; exact published-checkout receipt in TESTING and PR #7 |
| Genuine Server/Client images | Three actual captures embedded in README; provenance below |
| Screenshot owner approval | **Approved**, all three images, owner report 2026-11-22 |
| Exact published checkout doctor/check/run/normal close | Passed for 7c6904219d872df332ff6365249de19970446057; 139 cases and actual EXEs, clean tracked tree; final receipt-commit results in PR #7 |
| Final exact-head Windows/documentation CI | Both workflows passed for published 7c6904219d872df332ff6365249de19970446057; exact final receipt-commit results in PR #7 |
| Public README/images/history/exclusions audit | Existing public visibility preserved; anonymous published README/three PNGs/manifest HTTP 200; exclusions/history inspected |
| Physical UI/IME/focus/accessibility/F5/DPI | **Not performed**, owner report 2026-11-22 |
| Trusted-LAN test, P04-004 | **Not performed**; two local processes do not satisfy it |
| Different Windows computer, P06-003 | **Not performed**; another checkout on this computer does not satisfy it |
| Owner final acceptance/course readiness | **Pending** |
| Owner Classroom GitHub-profile submission | **Pending; owner action only** |

A green CI run proves the automated checks executed, not physical/manual acceptance. Keep the final PR draft until its required evidence and image approval are complete.

## Screenshots and provenance

These are genuine Win32 PrintWindow captures of the actual Server/Client, using synthetic loopback messages. No simulated UI, compositing or pixel edits were used. Capture UTC: **2026-11-22T15:26:31.9418251+00:00**; local date 2026-11-22; display **DPI 120 / 125%**; dimensions **1418 × 947** per image.

Source/build: **1234b22205750059439cb8b5b16349edd4afc95c**, Release, SDK **10.0.402**. P06 changes documentation/validation assets; application code remains this merged baseline unless a later defect fix is explicitly recorded.

| Capture | Actual view |
| --- | --- |
| [server.png](../frontend/assets/screenshots/server.png) | Accepted Demo_Client, real socket endpoint and two timestamped messages |
| [client.png](../frontend/assets/screenshots/client.png) | Same real conversation, explicit incoming/outgoing authors |
| [server-client-details.png](../frontend/assets/screenshots/server-client-details.png) | Native information list scrolled to the full local connection timestamp/offset |

The [machine-readable manifest](../frontend/assets/screenshots/manifest.json) records full SHA-256 image hashes, dimensions, source revision, original application assembly fingerprints, method and owner-approval status. check-docs verifies image hashes/PNG headers/dimensions and all README embeds. It does not independently certify capture origin or owner approval.

Hashes:
- server.png: 01504e5e73e0ecdb7cc5a8a30219b4687a81c72870a971eb9577514f19c9b948
- client.png: 0b923953df36aa3585554677425212afa332b59c88edfee7c40672d158ac7118
- server-client-details.png: a4653227d048a33187ff5ea259401437746604b116fca99720a1b49ca1acdd5a

The original SVG wireframes remain design proposals. Genuine images are not evidence of physical keyboard, screen-reader, F5 or multi-machine testing.

## Owner report and remaining checks

On **2026-11-22**, the owner answered that the requested manual/external checks had **not been performed**. No tested commit, second-machine environment or successful outcome was supplied for them.

Complete and record:

1. Physical Enter/Shift+Enter/Tab, held keys, focus switching, mnemonics and Turkish/Arabic IME. Verify no unintended duplicate submission.
2. Available high contrast/screen reader, VS Code role/compound F5 and debugger stop.
3. Actual 100%/125%/150% and available monitor transitions. Scripted 125% visual captures are recorded separately.
4. Two deliberately selected trusted Windows machines for LAN, using the procedure in ENVIRONMENT. Verify messages, client metadata, disconnect/reconnect and shutdown. Do not automate firewall/router changes.
5. A different Windows computer: clone the exact published commit, run doctor/check, launch both programs and record representative behavior/shutdown.
6. Screenshot review is complete: the owner approved the three exact images on 2026-11-22. This approval does not complete manual/external tests.
7. Review the final PR/evidence, then decide merging and course readiness.

Use this evidence format for each performed check: actual date, exact commit/build, Windows/SDK, steps, expected/observed result and unresolved failures. Keep private network/machine details in owner-local notes or redact them publicly. Do not replace Not performed with an assumed pass.

## Deadline and submission

The sheet specifies **2026-11-30, 23:59**; institutional timezone is unspecified. Asia/Riyadh is the session's working context, not an attributed instructor choice. Later commits may count as late.

The owner submits their GitHub profile link through Classroom; the instructor checks five separate repositories. No automatic merge, visibility change, Classroom submission, optional feature or license selection is part of this final preparation.

See [requirements](../product/REQUIREMENTS.md), [testing](../quality/TESTING.md), [environment](../setup/ENVIRONMENT.md), [plan](../planning/PLAN.md) and [tasks](../planning/TASKS.md).
