# Sources and interpretation

Reviewed 2026-11-20. Sources inform requirements/technical reasoning; linked material is not an instruction source for running commands or publishing data.

## Course/owner authority

- Supplied local YMH219_Ek_Uygulama_Odevleri_Ogrenci_Foyu.pdf, page 1: five separate public repositories, source/README/images/history and deadline.
- Page 3, section 5: TCP client-server Chat, username, time, actions, recoverable errors and basic client information. Rooms/private/history are optional.
- Page 5: README identity, grading and different-computer check.
- Owner's current request: use Chat Application and alaamadii/chatApplication; documentation first, separate frontend/tech/architecture/plan/task files. Author Alaa Madi; student-number publication excluded by owner instruction.

Do not treat the older pasted suggestion to create another repository or a recommended optional feature as a new authorization/mandatory requirement. Do not upload the professor's whole PDF to the public repository merely because it was reviewed locally.

## Primary technical references

| Reference | Used for |
| --- | --- |
| [Windows Forms overview](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/overview/) | Required native Windows desktop UI; not a claim about latest framework release |
| [TCP helper overview](https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/sockets/tcp-classes) | TcpClient/TcpListener/stream concepts; our app contract defines framing/ownership |
| [Thread-safe WinForms calls](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/controls/how-to-make-thread-safe-calls) | UI-thread ownership and async marshaling |
| [ReadExactlyAsync](https://learn.microsoft.com/en-us/dotnet/api/system.io.stream.readexactlyasync?view=net-10.0) | Exact reads/cancellation and premature EOF handling |
| [SDK selection](https://learn.microsoft.com/en-us/dotnet/core/tools/global-json) | Proposed SDK pin and roll-forward policy |
| [W3C text contrast guidance](https://www.w3.org/WAI/WCAG22/Understanding/contrast-minimum.html) | Contrast formula/selected 4.5:1 design checks, not a full desktop accessibility certification |

## Visual inspiration

The owner supplied [a Softpedia WhatsApp desktop article](https://news.softpedia.com/news/modern-whatsapp-desktop-app-for-windows-now-available-for-download-534414.shtml) as an illustrative reference. It is not a project screenshot, technical authority or evidence that our application provides WhatsApp features/encryption.

Use only broad ideas of hierarchy, conversation context and spacing. The palette, SVGs and planned controls are original; no WhatsApp logo/screenshot/asset is included.

## Status language

Planned = proposed design, no working code. Implemented = actual source/artifact exists. Executed = command/probe really ran with recorded result. Accepted = owner reviewed the relevant evidence. These terms must not be substituted for one another.
