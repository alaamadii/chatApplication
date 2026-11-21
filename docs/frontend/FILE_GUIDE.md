# Frontend file guide

## Implemented P01 files

| Existing file | Responsibility and owner |
| --- | --- |
| [Server Program](../../src/ChatApplication.Server/Program.cs) | Separate STA/PerMonitorV2 entry point and visual styles |
| [ServerForm](../../src/ChatApplication.Server/Forms/ServerForm.cs) | Listener inputs and empty client-information list; no listener/session |
| [Server layout](../../src/ChatApplication.Server/Forms/ServerForm.Layout.cs) | Sidebar labels, native fields, unavailable actions and empty peer context |
| [Client Program](../../src/ChatApplication.Client/Program.cs) | Separate STA/PerMonitorV2 entry point and visual styles |
| [ClientForm](../../src/ChatApplication.Client/Forms/ClientForm.cs) | Editable username/endpoint inputs; no accepted identity or connection |
| [Client layout](../../src/ChatApplication.Client/Forms/ClientForm.Layout.cs) | Connection sidebar, explicit unavailable actions and explanatory copy |
| [Presentation project](../../src/ChatApplication.Presentation/ChatApplication.Presentation.csproj) | Shared native WinForms library; no Core/socket/service dependency |
| [ChatWindow](../../src/ChatApplication.Presentation/ChatWindow.cs) | Shared Form lifetime, per-window fonts, scrollable canvas and working-area/DPI sizing |
| [Shared window layout](../../src/ChatApplication.Presentation/ChatWindow.Layout.cs) | Header, empty conversation, unavailable composer and inline guidance |
| [UiLayout](../../src/ChatApplication.Presentation/Layout/UiLayout.cs) | Small native label/field/action/column builders; no application actions |
| [ChatTheme](../../src/ChatApplication.Presentation/Theme/ChatTheme.cs) | Semantic canvas/surface/sidebar/text palette and system-color fallback |
| [OwnedFonts](../../src/ChatApplication.Presentation/Theme/OwnedFonts.cs) | Per-window point-unit body/title/section/metadata fonts; disposed after controls |
| [ConnectionBadge](../../src/ChatApplication.Presentation/Controls/ConnectionBadge.cs) | Literal Interface preview label; does not model or change connection state |
| [TranscriptView](../../src/ChatApplication.Presentation/Controls/TranscriptView.cs) | Readable empty conversation shell only; no message collection/cards |
| [App test project](../../tests/ChatApplication.App.Tests/ChatApplication.App.Tests.csproj) | Windows-only integration checks; same pinned tooling as Core.Tests |
| [Interface tests](../../tests/ChatApplication.App.Tests/InterfaceTests.cs) | Layout/reachability, no-op actions, Tab traversal and independent window lifetime |
| [STA helper](../../tests/ChatApplication.App.Tests/StaWindow.cs) | Serial, bounded STA execution and explicit Form disposal through test ownership |

Role Forms inherit ChatWindow. Layout partials organize the same object; they are not additional controllers or windows. Each Form owns its controls and its own OwnedFonts instance. Fonts remain valid while the other executable/window closes. Native system fonts/colors are never disposed.

The shared library contains presentation only. Apps retain their direct Core references from P00 and add Presentation; Presentation does not reference empty Core merely to follow a diagram.

## Future files — not created

| Planned path | Responsibility | Phase |
| --- | --- | --- |
| src/ChatApplication.Server/Forms/ServerForm.Connection.cs | Real Start/Stop, state projection, peer lifecycle and close guard | P02/P04 |
| src/ChatApplication.Server/Forms/ServerForm.Messaging.cs | Awaited operator send and authoritative message display | P03 |
| src/ChatApplication.Client/Forms/ClientForm.Connection.cs | Real Connect/Cancel/Disconnect and accepted identity | P02/P04 |
| src/ChatApplication.Client/Forms/ClientForm.Messaging.cs | Explicit send, server echo rendering and errors | P03 |
| src/ChatApplication.Presentation/Controls/MessageCard.cs | Plain author/time/body with accessible message content | P03 |

There are no empty Connection/Messaging handlers, message models, fake participants or sample conversation. Networking is deferred to P02. Future transcript retention, trim notices, message accessibility and actual async notifications must be implemented and tested with the behavior they serve.

## Design assets and evidence

- [Client wireframe](assets/client-wireframe.svg) and [Server wireframe](assets/server-wireframe.svg) remain labeled proposals with synthetic future content.
- [theme.json](theme.json) is the design inventory; only the P01 subset described in DESIGN_SYSTEM is applied.
- P01 local review captures are ignored under .local, not published submission images.

See [UX](UX.md), [design system](DESIGN_SYSTEM.md), [architecture](../architecture/ARCHITECTURE.md) and [testing](../quality/TESTING.md).
