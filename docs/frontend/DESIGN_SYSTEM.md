# Visual design system

Status: proposed tokens, not a claim of implemented accessibility or exact final pixels. [theme.json](theme.json) is the machine-readable design source for current contrast checks. P01 maps reviewed tokens into shared Presentation code and verifies them there.

## Palette

| Token | Hex | Use |
| --- | --- | --- |
| canvas | #F4F6FA | Window background |
| surface | #FFFFFF | Main content/cards |
| sidebar | #F7F9FC | Connection/peer context |
| textPrimary | #17253D | Headings/body |
| textSecondary | #526178 | Descriptions/metadata |
| primary | #315BEA | Enabled Connect/Start/Send |
| primaryHover | #2449C6 | Primary hover |
| primaryPressed | #1F3FA9 | Primary pressed |
| primaryText | #FFFFFF | Text on primary action |
| controlBorder | #64748B | Input/meaningful control outline |
| divider | #E2E8F0 | Decorative separators |
| incoming | #F1F5F9 | Incoming message surface |
| outgoing | #E8EEFF | Outgoing message surface |
| successText / successSurface | #166534 / #DCFCE7 | Connected/listening badge |
| warningText / warningSurface | #92400E / #FEF3C7 | Connecting/stopping/trim notice |
| errorText / errorSurface | #991B1B / #FEE2E2 | Actionable error |
| focus | #1D4ED8 | Visible keyboard focus ring |

Use text labels with state colors. Disabled buttons use native disabled appearance, not an enabled blue button with unreadable text. Divider is decorative, not the only indication of an interactive field.

Current script checks ten declared text/background pairs against a 4.5:1 minimum using the W3C contrast formula. The selected pairs range from 5.50:1 to 15.35:1. This does not certify native focus indicators, every state, high contrast, screen readers or all rendered text. See [contrast guidance](https://www.w3.org/WAI/WCAG22/Understanding/contrast-minimum.html).

## Typography, spacing and geometry

Segoe UI: body/input 10 pt; app title 20 pt bold; section heading 12 pt bold; metadata 9 pt. Respect DPI/text scaling; never shrink text to hide overflow.

Spacing scale: 4, 8, 12, 16, 24, 32 logical pixels. Use 24 outer padding, 16 card padding, 12 related-control gaps and at least 8 between label/input. Sidebar target width 280 logical pixels; conversation takes remaining width.

Controls: target button height at least 36 logical pixels, primary actions no icon-only meaning. Message cards may use a modest 8-pixel corner treatment/original drawing, with readable plain-text content and author/time. Native title bar remains intact; no frameless-window hit testing or external bitmap dependency.

Use shared tokens rather than unrelated hex values in Forms. Fonts/components must have explicit owners; do not share a disposable static Font across independently closing windows.

## DPI, accessibility and content

Use PerMonitorV2 and a 96-DPI design baseline. Layout panels and text measurement reflow messages on resize; actions/status remain visible or reachable through scrolling. Bound effective minimum size to the real working area.

In high contrast, prefer system foreground/background/highlight colors and native focus over decorative palette choices. Physical monitor transitions and assistive review remain pending until implemented.

English shell labels; Turkish/Arabic/English message content. Keep sender, timestamp and body in separate accessible elements to reduce bidi confusion. Do not encode information only in left/right alignment.

The illustrative [Client](assets/client-wireframe.svg) and [Server](assets/server-wireframe.svg) assets are marked DESIGN PROPOSAL. They contain invented demo conversation for layout review, not execution evidence or copied WhatsApp assets.
