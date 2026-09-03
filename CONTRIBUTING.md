# Contributing to EclipsePlayer

Thanks for your interest. This covers how to submit changes and the terms your
contributions are made under. Questions that aren't bug reports are welcome in
GitHub Discussions or an issue.

## Before you start

- For anything substantial — new subsystems, architectural changes, a new
  dependency, a new device family — open an issue first so the approach can be
  agreed on. Small fixes can go straight to a pull request.
- Keep pull requests focused: one logical change per PR where practical.

## Licensing of contributions

You keep the copyright to your contributions. You do **not** assign copyright and
you do **not** sign a separate CLA. By submitting a contribution you agree to the
two terms below.

### 1. Inbound = outbound

Your contribution is licensed under the license of the project directory it lands in:

| Project | License |
|---|---|
| `EclipsePlayer.Hal` | MPL-2.0 |
| `EclipsePlayer.Funscript` | MPL-2.0 |
| `EclipsePlayer.IO`, `EclipsePlayer.Engine`, `EclipsePlayer.Media`, `EclipsePlayer` (app / UI) | GPL-3.0-or-later |

Every new source file must carry a two-line [SPDX](https://spdx.dev/) header
matching its project's license:

```csharp
// SPDX-FileCopyrightText: <year> <your name or handle>
// SPDX-License-Identifier: MPL-2.0
```

Use `GPL-3.0-or-later` for files in the GPL projects. XML-family files
(`.axaml`, `.csproj`, `.props`) use an XML comment with the same two lines.

### 2. Bounded relicensing grant

You grant the project's maintainers the right to re-release your contribution
under **any OSI-approved open-source license**, in addition to the license above.

This is a fallback so the project cannot become permanently stuck on one license
if it changes hands or direction — for example, moving `Hal` to a more permissive
open-source license for wider adoption. It does **not** allow relicensing under a
proprietary or non-OSI-approved license, and it does not transfer ownership: you
remain free to use your own contribution however you like elsewhere.

### Devices, transports, and funscript conformance

New device support, new transports, and changes to funscript standard
conformance belong **in `EclipsePlayer.Hal` / `EclipsePlayer.Funscript` in this
repository** — this is their canonical home. Please upstream them rather than
carrying an out-of-tree fork, so every consumer of the libraries benefits.

Player *extension scripts* are a separate matter: those are your own work under
any license you choose and are not covered by this section.

## AI-generated and AI-assisted contributions

Using AI tools to help produce a contribution is allowed, but it **must be
disclosed**. (The maintainer's own application code is written without AI
generation — see the README — but outside contributions only need transparency.)

- **In the pull request**, state which AI tools were used and for what.
- **AI-assisted** code (you wrote it with AI help such as completions or
  refactoring suggestions): identify the affected areas in the PR description.
- **AI-generated** code (a tool produced it and you reviewed it): mark it in the
  source. Above a block:

  ```
  // AI-generated (<tool/model>), reviewed by <name>
  ```

  For a file that is entirely AI-generated, add a third header line under the
  SPDX lines:

  ```
  // SPDX-FileCopyrightText: <year> <your name or handle>
  // SPDX-License-Identifier: MPL-2.0
  // AI-generated: <tool/model>; reviewed by <name>
  ```

- **Commit trailer**: add `Assisted-by: <tool>` or `Generated-by: <tool>`.

This ties into the DCO sign-off below: you are certifying you have the right to
submit the code under the project's license. Substantially AI-generated code has
weaker provenance — training-data licensing is unsettled, and purely
machine-generated output is generally not copyrightable — so it has to be
identifiable for review. Undisclosed AI generation discovered after merge is
grounds for reverting the contribution.

## Developer Certificate of Origin

Every commit must be signed off. The sign-off certifies that you wrote the
change, or otherwise have the right to submit it under the project's license.
It is the Developer Certificate of Origin 1.1 (<https://developercertificate.org/>):

> By making a contribution to this project, I certify that:
>
> (a) The contribution was created in whole or in part by me and I have the
>     right to submit it under the open source license indicated in the file; or
>
> (b) The contribution is based upon previous work that, to the best of my
>     knowledge, is covered under an appropriate open source license and I have
>     the right under that license to submit that work with modifications,
>     whether created in whole or in part by me, under the same open source
>     license (unless I am permitted to submit under a different license), as
>     indicated in the file; or
>
> (c) The contribution was provided directly to me by some other person who
>     certified (a), (b) or (c) and I have not modified it.
>
> (d) I understand and agree that this project and the contribution are public
>     and that a record of the contribution (including all personal information
>     I submit with it, including my sign-off) is maintained indefinitely and
>     may be redistributed consistent with this project or the open source
>     license(s) involved.

Add it with `-s`:

```sh
git commit -s -m "Your message"
```

That appends `Signed-off-by: Your Name <your.email@example.com>`, which must be a
real name and match your Git identity. To add sign-off to the last commit:
`git commit --amend -s --no-edit`. For a range: `git rebase --signoff <base>`.

## Development

- .NET 10 SDK. Build the whole solution with `dotnet build EclipsePlayer.slnx`.
- `.editorconfig` is authoritative for formatting.
- Nullable reference types are enabled and nullable warnings are errors — new
  code must be null-clean.
- The non-UI libraries (`Funscript`, `IO`, `Hal`, `Engine`) must not reference a
  UI framework. Keep the dependency direction intact:
  `EclipsePlayer → Engine → { Funscript, IO, Hal, Media }`, `IO → Funscript`.

### Dependencies

- A new or upgraded NuGet package must be **at least 14 days old** when the PR is
  opened. The early days after a release are the main supply-chain-attack window
  (a malicious version is published, then pulled once caught). Dependabot applies
  this cooldown automatically; manual additions must respect it too.
- `NuGetAudit` runs on restore and the build fails on high/critical advisories in
  direct or transitive packages. Don't suppress an advisory to get a build
  through — resolve or replace the dependency.
- Add a dependency only when it clearly earns its place. More packages is more
  attack surface.

## Submitting a pull request

1. Fork and branch from the default branch.
2. Make your change; sign off every commit.
3. Confirm `dotnet build EclipsePlayer.slnx` is clean.
4. Open a PR describing what changed and why.
