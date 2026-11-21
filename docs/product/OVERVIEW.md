# Product overview

## Purpose and users

Build an explainable desktop networking assignment: asynchronous TCP communication, OOP responsibility, native controls, connection state and recoverable errors.

The Server operator starts a local listener, sees the connected client, exchanges messages and stops it. The Client user supplies a display name/address/port, connects, sends/receives and disconnects. These are separate executables in one repository.

## Mandatory demonstration journey

1. Start Server on loopback 127.0.0.1, default port 5000.
2. Start Client, enter a valid username and connect to that endpoint.
3. Complete username validation/handshake before showing Connected.
4. Client sends synthetic text; Server displays it with sender and timestamp.
5. Server operator replies; Client displays it with timestamp.
6. Each accepted message appears once per transcript; no local optimistic copy plus server echo duplication.
7. Client disconnects; both interfaces update without crashing. Reconnect is explicit.
8. Stop Server and close both windows normally.
9. Repeat representative checks on a deliberately configured trusted LAN and another Windows computer.

The server operator provides the second conversation endpoint, so this baseline works with two processes. Client-to-client group chat requires the optional multi-client phase.

## Scope

Required: server/one client, username, timestamped text, explicit actions, basic client information, responsive async UI and friendly errors.

Deferred proposals: rooms with multiple clients, private messages and saving history. Search, file attachments, reactions, accounts, presence across sessions, read receipts, dark theme and installers are not silently included.

No database, internet deployment, account service, third-party messaging API or hosted backend is needed. Usernames are display claims, not verified identities.

## Planned experience

A light two-pane desktop layout separates connection/peer context from conversation. Clear state badges, a visible Send action and actionable inline errors replace technical exception dumps. Messages support Turkish/Arabic/English text; the UI/documentation language is English.

Transcript retention is bounded to the latest 500 messages per window, with a visible notice when older messages are removed. History is memory-only unless separately approved later.

## Current state and review

PR #3 is merged. P02 now provides real TCP listening, supported-name handshake, explicit cancellation/disconnect/reconnect and owned async cleanup. Owner P02 review remains pending. Message exchange and detailed participant information in the demonstration journey remain P03/P04; actual LAN/physical acceptance is not inferred from loopback checks.

See [requirements](REQUIREMENTS.md), [frontend UX](../frontend/UX.md), [architecture](../architecture/ARCHITECTURE.md) and [plan](../planning/PLAN.md).
