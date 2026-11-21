# Documentation map

Status: P02 connections/username handshake implemented on 2026-11-21; owner review pending. PR #3 is merged. Chat messages and complete participant metadata remain future behavior.

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

Frontend assets are [client](frontend/assets/client-wireframe.svg), [server](frontend/assets/server-wireframe.svg) and [machine-readable tokens](frontend/theme.json). They are planning artifacts.

FILE_GUIDE links existing P02 source/tests and labels future paths as planned. Current file links point to real repository files. Any change to a requirement must update its tasks, design decisions, protocol/UI implications and validation plan.
