# Planned frontend file guide

All C# paths below are **planned**, shown as code rather than broken source links. Create them only in the named phase when behavior is needed. The documentation directory is separate from application source.

## App composition and screens

| Planned path | Responsibility | Phase |
| --- | --- | --- |
| src/ChatApplication.Server/Program.cs | STA entry point, DPI/visual styles and ServerForm composition | P00 |
| src/ChatApplication.Server/Forms/ServerForm.cs | Form identity, owned services/fonts/lifetime and disposal | P00/P01 |
| src/ChatApplication.Server/Forms/ServerForm.Layout.cs | Listener/client-info/conversation/composer layout | P01 |
| src/ChatApplication.Server/Forms/ServerForm.Connection.cs | Start/stop, state projection, client information and close guard | P02/P04 |
| src/ChatApplication.Server/Forms/ServerForm.Messaging.cs | Operator send and awaited transcript updates | P03 |
| src/ChatApplication.Client/Program.cs | STA/DPI entry point and ClientForm composition | P00 |
| src/ChatApplication.Client/Forms/ClientForm.cs | Form identity, ownership and disposal | P00/P01 |
| src/ChatApplication.Client/Forms/ClientForm.Layout.cs | Identity/endpoint/transcript/composer controls | P01 |
| src/ChatApplication.Client/Forms/ClientForm.Connection.cs | Connect/cancel/disconnect and state projection | P02/P04 |
| src/ChatApplication.Client/Forms/ClientForm.Messaging.cs | Explicit send, server echo rendering and plain-text errors | P03 |

Partial files organize one Form object; they are not separate controllers/instances. No socket parsing in a layout method and no Control passed to Networking.

## Shared Presentation

| Planned path | Responsibility | Phase |
| --- | --- | --- |
| src/ChatApplication.Presentation/Theme/ChatTheme.cs | Reviewed palette, spacing and high-contrast fallback | P01 |
| src/ChatApplication.Presentation/Theme/OwnedFonts.cs | Per-window point-unit fonts and disposal | P01 |
| src/ChatApplication.Presentation/Controls/ConnectionBadge.cs | Text-backed state badge; no network state mutation | P01 |
| src/ChatApplication.Presentation/Controls/TranscriptView.cs | Scrollable bounded collection of visible message cards | P01 shell / P03 messages |
| src/ChatApplication.Presentation/Controls/MessageCard.cs | Plain sender/time/body and accessible text; no HTML/RTF execution | P03 |

Use native controls/original simple drawing. Measure wrapping against available width, dispose removed cards and keep a visible 500-message trim notice. Validate actual performance/accessibility before accepting the renderer. If a simpler native transcript is needed, record the tradeoff and update the wireframes/UX instead of claiming pixel equivalence.

## State, resources and tests

Forms bind presentation to explicit connection state and immutable notifications. Shared controls do not connect, retry, save or invent messages. Parent Forms own services/cancellation/fonts; controls own only components they create.

Planned native tests: tests/ChatApplication.App.Tests for state/action/layout, focus/mnemonics, literal message content, duplicate-free echo display and close/callback behavior. Run serial STA windows with bounded waits and synthetic content; manual UI acceptance is separate.

Current design assets:

- [Client wireframe](assets/client-wireframe.svg)
- [Server wireframe](assets/server-wireframe.svg)
- [Token source](theme.json)

See [UX](UX.md), [architecture](../architecture/ARCHITECTURE.md) and [tasks](../planning/TASKS.md) for sequencing. No source directory is created by this file inventory.
