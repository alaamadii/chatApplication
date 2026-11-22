# Documentation map

Status: P06 draft final-review preparation on 2026-11-22; PR #6 is merged. Genuine images and source/test audit are prepared; physical checks, P04-004 trusted LAN, another Windows computer and final owner acceptance remain pending.

Read in the following order. Each file owns one concern so frontend, technology, architecture, planning and evidence stay separate.

| Area / file | Owns | Does not replace |
| --- | --- | --- |
| [product/OVERVIEW](product/OVERVIEW.md) | Idea, users, demonstration and scope | Formal acceptance criteria |
| [product/REQUIREMENTS](product/REQUIREMENTS.md) | Mandatory/optional requirements and task mapping | Execution evidence |
| [technology/STACK](technology/STACK.md) | Technology choices/version policy | Architecture ownership |
| [architecture/ARCHITECTURE](architecture/ARCHITECTURE.md) | Dependencies/classes/lifecycle | Wire contract |
| [architecture/PROTOCOL](architecture/PROTOCOL.md) | Frames/packets/validation/delivery | UI visual design |
| [architecture/DECISIONS](architecture/DECISIONS.md) | Why choices were made and consequences | Task status |
| [frontend/UX](frontend/UX.md) | Screen layout, actions, states and copy | Colors/tokens |
| [frontend/DESIGN_SYSTEM](frontend/DESIGN_SYSTEM.md) | Palette, type, spacing, contrast and DPI | Actual visual acceptance |
| [frontend/FILE_GUIDE](frontend/FILE_GUIDE.md) | Existing interface and planned UI files | Implementation evidence |
| [planning/PLAN](planning/PLAN.md) | Milestones, sequence and review gates | Granular backlog |
| [planning/TASKS](planning/TASKS.md) | Task IDs, dependencies, status and evidence | Strategy |
| [planning/WORKFLOW](planning/WORKFLOW.md) | Branch/PR/validation process | Owner approval |
| [quality/TESTING](quality/TESTING.md) | Executed checks and future test matrix | Assumed coverage |
| [quality/SECURITY](quality/SECURITY.md) | Trust/data/resource boundaries | Authentication/encryption |
| [setup/ENVIRONMENT](setup/ENVIRONMENT.md) | Actual setup, run/debug commands and troubleshooting | Functional chat acceptance |
| [submission/CHECKLIST](submission/CHECKLIST.md) | Final course evidence/owner action | Automatic submission |
| [history/DEVELOPMENT](history/DEVELOPMENT.md) | Actual dated work and publication | Fabricated phase dates |
| [references/SOURCES](references/SOURCES.md) | Course/primary sources and authority | Executable instructions |

Frontend design proposals are [client](frontend/assets/client-wireframe.svg), [server](frontend/assets/server-wireframe.svg) and [tokens](frontend/theme.json). Genuine [Server](frontend/assets/screenshots/server.png), [Client](frontend/assets/screenshots/client.png) and [connection-time](frontend/assets/screenshots/server-client-details.png) captures have a [provenance/hash manifest](frontend/assets/screenshots/manifest.json). Approval and missing external evidence belong in CHECKLIST.

FILE_GUIDE links the implemented source/tests and genuine capture assets and labels future paths as planned. Current file links point to real repository files. Any change to a requirement must update its tasks, design decisions, protocol/UI implications and validation plan.
