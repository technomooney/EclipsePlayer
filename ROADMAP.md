# Roadmap

**This is a direction, not a commitment.** There are no dates. Scope, ordering, and
priorities will change as the project develops and as the foundational spikes settle.
EclipsePlayer is in early development — expect churn, and expect items to move between
phases or drop entirely.

Detailed, trackable work lives in GitHub Issues and Milestones. This file is the
high-level shape.

The feature set is informed by prior art the developer has used and modified —
XTPlayer and a personal ScriptPlayer+ fork — reimplemented natively in C# rather than
ported.

## Phase 0 — Foundations

- [x] Solution scaffold, project structure, licensing, contribution setup
- [ ] Media-backend spike (libmpv embedding on Linux) — decides `EclipsePlayer.Media`
- [ ] Real-time sync-loop threading design (dedicated high-priority thread +
      channels), prototyped and profiled

## Phase 1 — Minimal playback

- [ ] Video playback with position querying behind `IMediaPlayer`
- [ ] Funscript parsing and in-memory model (`EclipsePlayer.Funscript`): v1
      single-axis, v1 companion multi-axis, v2 single-file multi-axis
- [ ] Basic sync of one axis to one device during playback
- [ ] Playback UI: transport, seek bar, fullscreen

## Phase 2 — Device breadth

- [ ] The Handy (HTTP)
- [ ] Serial / TCode devices (OSR family), with the interval-parameter fix
- [ ] Intiface / Buttplug routing, including raw TCode
- [ ] Per-media script offset, motion speed limits, idle keep-alive
- [ ] Hardened disconnect / reconnect handling
- [ ] Multi-axis playback and script-variant selection

## Phase 3 — Library and playback workflow

- [ ] Multi-folder library with tree and grid views
- [ ] Playlists: M3U8 open/save with relative-path support, filtering
- [ ] Play tracking: counts, stats, auto-skip after N plays
- [ ] Script matching, recursive script-folder scan, variant auto-select

## Phase 4 — Deduplication

- [ ] Visual (pHash) and audio fingerprinting into SQLite
- [ ] Offset/containment-tolerant matching
- [ ] Duplicate-aware favorites that collapse to the best-quality copy
- [ ] Adaptive, per-volume library scanning

## Phase 5 — Polish

- [ ] Theming (light/dark, token system, custom editor)
- [ ] Popout windows with multi-window state sync
- [ ] Subtitle rendering: embedded extraction, external SRT/VTT/ASS/SSA
- [ ] Extension scripting for user-authored generators and transforms
- [ ] Localization

## Not planned

- In-app script-site browsing
- NAS (WebDAV / FTP) media sources

## Later / undecided

- Auto-update (evaluate Velopack)
- Single-file self-contained distribution per platform
- WebAssembly build (stretch goal, not guaranteed)
- Mobile (Android / iOS) — not currently targeted
