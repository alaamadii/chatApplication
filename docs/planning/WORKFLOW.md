# Development and review workflow

## Repository and scope

Use only alaamadii/chatApplication and this Chat Application workspace. No sibling assignment modifications or replacement repositories. Existing public visibility is preserved.

The initial main commit contains only a minimal README/.gitignore PR base because the remote was empty. Documentation and all subsequent substantive work use dedicated branches.

## Branch sequence

| Work | Suggested branch |
| --- | --- |
| Current foundation | docs/project-foundation |
| P00 | chore/project-initialization |
| P01 | feat/server-client-ui |
| P02 | feat/tcp-connections |
| P03 | feat/realtime-messaging |
| P04 | fix/connection-lifecycle |
| Selected P05 | Separate scoped feature branch |
| P06 | docs/final-submission |

Before each phase: inspect status, preserve owner edits, confirm the previous review/merge, fetch and fast-forward main, then branch. If switching is unsafe, explain the exact conflict without discarding work. Never reset/force-push to simplify history.

## Working loop

1. Read AGENTS, current requirements, design decisions, PLAN and TASKS.
2. Identify the exact task IDs and scope; no speculative future features.
3. Implement small explainable changes with meaningful assertions.
4. Run current docs checks; after P00 run the full dev check too.
5. Inspect actual UI/process behavior where available.
6. Reconcile current features, limits, file guide, tasks and dated evidence.
7. Inspect diff/output exclusions; create meaningful commits.
8. When authorized, push the phase branch and open a PR targeting main.
9. Verify hosted CI for the exact final SHA, resolve change-caused failures.
10. Owner reviews and merges; only then begin the next phase.

A documentation proposal does not require a fabricated runnable scaffold. A successful build or empty test project does not count as feature coverage.

## Pull request content

Lead with concrete behavior/change; include task IDs, implementation/scope, exact local/hosted/manual validation, screenshots/provenance if genuine, risks and remaining owner checks.

Draft is appropriate for missing required evidence. Do not mark completed mandatory checks simply to make a PR ready. Preserve actual development dates and reviewed commit history.

## Authorization and review

Existing explicit task/session authorization for routine commit/push/PR work persists; do not repeatedly request the same approval. Repository visibility changes, optional scope and merging remain separate owner decisions.

Do not send email/chat messages, submit to Classroom, install unrelated software or change firewall/router settings without actual authorization. No license is selected automatically.

See [PLAN](PLAN.md), [TASKS](TASKS.md), [test strategy](../quality/TESTING.md) and [development log](../history/DEVELOPMENT.md).
