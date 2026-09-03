# Security Policy

## Supported versions

EclipsePlayer is in early development. There are no released versions yet, and no
formal security support. Fixes land on the default branch.

## Reporting a vulnerability

Please **do not** open a public issue for security problems.

Use GitHub's private vulnerability reporting: the repository's **Security** tab →
**Report a vulnerability**. If that is unavailable, email github@moontronic.com
with details.

Please include:

- what the problem is and the impact you foresee
- steps or a proof of concept to reproduce it
- affected component (`Hal`, `Funscript`, `Media`, the app, a transport, etc.)
- your environment (OS, .NET version, device if relevant)

You will get an acknowledgement as soon as reasonably possible. Since this is a
solo hobby project, response times are best effort. Coordinated disclosure is
appreciated — give a reasonable window for a fix before publishing details.

## Scope notes

- **Device transports** (serial, Bluetooth, HTTP, WebSocket) and **player
  extension scripts** are the highest-risk areas; reports there are especially
  welcome.
- Vulnerabilities in bundled third-party components (libmpv, .NET runtime, NuGet
  dependencies) should also be reported upstream where possible.
