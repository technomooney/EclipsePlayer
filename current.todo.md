# EclipsePlayer — current working list

Plain-text task list. Actionable lines are prefixed `TODO:` so Rider's TODO tool
window indexes them (Settings > Editor > File Types: add `*.todo` to "Plain Text"
if items don't show up; or rename this to `current.todo.md` for zero-config).

Last updated: 2026-09-09


## NOW — first backend: EclipsePlayer.Funscript

Rationale: no internal deps, no native interop, no async/threading. Scope is fixed
(validate parsed data against the spec, expose a read-only queryable dataset).
Everything on the haptics side sits on top of it.

DONE: EclipsePlayer.Funscript/FORMAT.md — the pinned format spec (base 1.0, strict/lenient contract, multi-axis 1.1 axes, semantic axis names, inbound TCode map)
TODO: assemble a real .funscript test corpus in EclipsePlayer.Funscript.Tests/TestData: OFS export, classic ScriptPlayer, Handy download, a multi-axis set (separate files + a 1.1 axes single-file), plus messy ones (float at, unsorted, dupe at, negative at, missing wrapper fields, empty actions, BOM)
TODO: add the solution test project — recommend xUnit, solution-wide — and wire it into EclipsePlayer.slnx
TODO: confirm Funscript needs ZERO NuGet (System.Text.Json / System.Collections.Immutable / System.Buffers are in-box on net10) — keep Directory.Packages.props untouched for it
TODO: EclipsePlayer.Funscript.csproj — add PackageLicenseExpression=MPL-2.0, IsPackable=false, GenerateDocumentationFile=true
TODO: drop a LICENSE (MPL-2.0) into EclipsePlayer.Funscript/ so the license is unambiguous if the project is ever extracted
TODO: sketch internal layout before scattering files: Model/ (Document, AxisScript, Action, Metadata), Axes/ (AxisName + constants + alias map), Parsing/ (Reader, Options, Result, probe), Serialization/ (writer); internal the JSON DTOs
TODO: Funscript — immutable FunscriptAction (timestamp + position)
TODO: Funscript — immutable AxisScript holding a sorted ImmutableArray<FunscriptAction> + metadata (range, inverted, axis name)
TODO: Funscript — AxisName value-type-over-string + known constants; unknown names pass through
TODO: Funscript — LooksLikeFunscript(ReadOnlySpan<byte> head): cheap probe (JSON + "actions" key)
TODO: Funscript — Parse(stream, options) -> { Document?, Errors[], Warnings[] }; strict vs lenient(normalize) option; takes bytes/stream, NEVER a file path
TODO: Funscript — strict contract = base-1.0 WRITE rules (int ms, unique strictly-increasing at, pos 0-100); lenient coerces float/unsorted/dupe/negative + emits warnings
TODO: Funscript — versioned inbound TCode->name alias map (L0->stroke etc.) lives HERE, not IO
TODO: Funscript — GetActionAt(TimeSpan) via binary search; return the bracketing pair
TODO: Funscript — decide whether interpolation lives here or in Engine
TODO: Funscript — tests first: boundaries, midpoints, before-first, after-last, empty, single-action
TODO: add two-line SPDX headers as real source files get written (MPL-2.0 for Funscript)


## Backend build sequence (after Funscript)

TODO: IO — byte acquisition: async read to stream, BOM/encoding handling; hand streams to Funscript.Parse
TODO: IO — container magic-byte sniffer (static table: ftyp / EBML 1A45DFA3 / RIFF / OGG...), NO Media dependency
TODO: IO — library scan: walk folders, classify each file via Funscript.LooksLikeFunscript + the container sniffer (tier-1, no full parse)
TODO: IO — script<->media correlation: filename matching, axis-suffix + variant detection, orphan warnings
TODO: Funscript — serialize model->JSON: single-file 1.1 `axes` array output, ids = semantic names, top-level actions always = L0/stroke
TODO: Funscript — deserialize: accept 1.1 `axes` + 2.0 `channels` + flat 1.0 from one stream
TODO: IO — sidecar handling: discover `name.<axis>.funscript` files, group them, feed each stream to Funscript; optional classic-sidecar export (write paths + bytes)
TODO: Hal — TCode name->channel map lives ONLY in the device adapter, parameterized by dialect/version + device model
TODO: IO — media + script folder scanning; script <-> video matching
TODO: Media — run the libmpv embed spike on the CachyOS box BEFORE any Media code (see memory: media-backend)
TODO: Media — define IMediaPlayer minimally against one working mpv implementation; keep the video-surface control a separate factory/ViewLocator seam
TODO: Hal — device abstraction + one real transport first (which one depends on hardware on hand)
TODO: Engine — sync loop: script position + media clock + device output
TODO: Engine — dedicated high-priority thread + System.Threading.Channels (deferred until profilable)


## Open architecture questions (decide when the relevant lib is built)

TODO: funscript axis key naming — RESOLVED: canonical = semantic name, never TCode; TCode confined to Hal device adapter (see IO/Hal tasks + memory funscript-format-findings)
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
