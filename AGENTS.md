# Project working agreement

## Scope and workflow

- Work only in this Chat Application repository; do not change sibling assignments.
- Current delivery is documentation and design. Do not implement application behavior until the owner reviews this foundation and requests Phase 00.
- Inspect status/remotes and preserve owner edits before changes.
- Use a dedicated branch and one reviewed PR per phase. Main's minimal bootstrap is only the PR base.
- Never force-push, rewrite history, merge automatically or submit to Classroom.
- Publish only within authorization already supplied by the task/session.
- Do not request or publish the student number. Author name supplied: Alaa Madi.
- Distinguish mandatory assignment requirements, owner choices and deferred optional proposals.

## Planned engineering rules

- Core stays independent of Windows UI, drawing, sockets, timers and file dialogs.
- Networking owns TCP, framing, async I/O and connection resources; it never receives controls.
- Server and Client are separate Windows Forms executables.
- Add shared Presentation only when actual UI work needs it; no empty future classes.
- Use programmatic native controls, explicit resource ownership and explainable small classes.
- Use cancellation/awaited tasks; no blocking UI waits or unobserved background work.
- Default to loopback. LAN configuration must be deliberate; no automatic firewall/router changes.
- Multiple-client rooms, private messages and saved history require a later owner decision.

## Evidence and documentation

- Run ./scripts/check-docs.ps1 after documentation/tooling changes.
- After Phase 00 provides scripts/dev.ps1, also run its check after application/configuration changes.
- Add meaningful behavioral tests when behavior exists; no placeholder passing tests.
- Label planned files/designs clearly; do not link to source files that do not exist.
- Record actual work dates and exact local/hosted/manual results separately.
- Mockups are design proposals, never application screenshots or submission evidence.
- Update decisions, requirements, frontend guidance, PLAN and TASKS together when scope changes.
- Exclude build output, local state, transcripts, credentials and personal settings from Git.
