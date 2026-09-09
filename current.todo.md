# EclipsePlayer — current working list

Plain-text task list. Actionable lines are prefixed `TODO:` so Rider's TODO tool
window indexes them (Settings > Editor > File Types: add `*.todo` to "Plain Text"
if items don't show up; or rename this to `current.todo.md` for zero-config).

Last updated: 2026-09-09


## NOW — first backend: EclipsePlayer.Funscript

Rationale: no internal deps, no native interop, no async/threading. Scope is fixed
(validate parsed data against the spec, expose a read-only queryable dataset).
Everything on the haptics side sits on top of it.

TODO: add the solution test project (pick xUnit or NUnit for the whole solution) and wire it into EclipsePlayer.slnx
TODO: Funscript — immutable FunscriptAction (timestamp + position)
TODO: Funscript — immutable AxisScript holding a sorted ImmutableArray<FunscriptAction> + metadata (range, inverted, axis id)
TODO: Funscript — factory: take parsed representation, sort + validate, return built object OR a structured list of validation errors
TODO: Funscript — GetActionAt(TimeSpan) via binary search; return the bracketing pair
TODO: Funscript — decide whether interpolation lives here or in Engine
TODO: Funscript — RESOLVED: strict validator = base-1.0 WRITE contract (int ms, unique strictly-increasing at, pos 0-100); float/unsorted/dupe/negative handling lives in IO normalization + warnings
TODO: Funscript — tests first: boundaries, midpoints, before-first, after-last, empty, single-action
TODO: write docs/funscript-format.md — pin to Eroscripts/OFS@<commit> + RFC 267449 (no formal spec exists); see memory funscript-format-findings
TODO: add two-line SPDX headers as real source files get written (MPL-2.0 for Funscript)


## Backend build sequence (after Funscript)

TODO: IO — read a .funscript file into the parsed representation Funscript validates
TODO: IO — normalization layer for messy real-world funscript files (best-effort cleanup before handoff)
TODO: IO — multi-axis: read 1.1 `axes` array (target) + 2.0 `channels` + classic separate-suffix files (name.roll.funscript)
TODO: IO — axis key map: canonical = TCode id (L0/L1/L2/R0/R1/R2/A1); accept TCode or name on input; user-overridable
TODO: IO — media + script folder scanning; script <-> video matching
TODO: Media — run the libmpv embed spike on the CachyOS box BEFORE any Media code (see memory: media-backend)
TODO: Media — define IMediaPlayer minimally against one working mpv implementation; keep the video-surface control a separate factory/ViewLocator seam
TODO: Hal — device abstraction + one real transport first (which one depends on hardware on hand)
TODO: Engine — sync loop: script position + media clock + device output
TODO: Engine — dedicated high-priority thread + System.Threading.Channels (deferred until profilable)


## Open architecture questions (decide when the relevant lib is built)

TODO: funscript axis key naming — RESOLVED: canonical internal key = TCode id (see IO tasks + memory funscript-format-findings)
TODO: eager vs lazy multi-axis load
TODO: data virtualization strategy for large library views in Avalonia
TODO: plugin/extension loading mechanism — Lua vs out-of-process Python (see memory: extension-scripting)


## Data layer (design settled this session — see memory: moon-edition-db-schema)

TODO: schema — videos (content identity) + video_paths (N paths -> 1 identity)
TODO: schema — fingerprints with per-row sampling params (mode, effective interval) alongside algo_version
TODO: schema — thumbnails table (separate from videos), WebP blobs, invalidation cols (source_size, source_mtime/hash, generator_version)
TODO: schema — videos.merged_into, pending_duplicate_links, merge_log, merge_exclusions
TODO: fingerprint sampling — symmetric tapered cadence (2x under 2min, base 30s, 1/2x 30-90min, 1/4x 90min+) + min-samples floor + optional ~300 backstop
TODO: duplicate handling — tiered-confidence, NO auto-merge; pending links reviewed in a batch UI showing would-be merged totals
TODO: thumbnails — WebP encode via SkiaSharp on a background job, never on the UI thread


## Repo / GitHub chores (carried from memory: todo)

TODO: push to github.com/technomooney/EclipsePlayer (public)
TODO: repo description + topics (funscript, avalonia, dotnet, csharp, media-player, haptics, cross-platform, linux)
TODO: GitHub settings — enable Discussions + private vulnerability reporting
TODO: confirm first CI run is green
TODO: CI — add badge to README
TODO: CI — OS matrix (ubuntu + windows + macos)
TODO: CI — dotnet format --verify-no-changes gate (non-blocking to start)
TODO: CI — test job once the test project exists
TODO: CI — pin GitHub Actions to commit SHAs; CodeQL workflow
TODO: supply chain — enable lock files + --locked-mode in CI
TODO: committed docs/ with architecture rationale (only in the gitignored context doc today)
