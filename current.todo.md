# EclipsePlayer — current working list

Plain-text task list. Actionable lines are prefixed `TODO:` so Rider's TODO tool
window indexes them (Settings > Editor > File Types: add `*.todo` to "Plain Text"
if items don't show up; or rename this to `current.todo.md` for zero-config).

Last updated: 2026-09-24 (first green test — smoke test proves the model graph holds together)


## DONE this session (2026-09-24)

DONE: First smoke test — `EclipsePlayer.Funscript.Tests/Model/FunscriptDocumentTest.cs`, `TestFunDocWiring`. Hand-built 4 `FunscriptAction`s -> `ImmutableArray.Create` -> `AxisScript` (Stroke) -> `FunscriptMetadata` (object-initializer syntax) -> `FunscriptDocument`. Asserts on `Stroke.Actions.Length`, `Metadata.Creator`, `Metadata.Title`. Passes. Proves the model graph (actions -> axis -> document, with metadata attached) actually holds together end to end. Uncommitted `EclipsePlayer.Funscript.Tests.csproj` `Model\` folder reference now resolves for real.


## DONE previous session (2026-09-17)

DONE: Model/FunscriptDocument.cs — `public sealed record FunscriptDocument(AxisScript Stroke, ImmutableArray<AxisScript> Axes, FunscriptMetadata Metadata);`. Confirmed design: `Stroke` is just another `AxisScript`, not a separate shape — one axis type used everywhere, per FORMAT.md §5.5. Committed.
DONE: Model/FunscriptMetadata.cs extended — Tags/Performers (`List<string>?`) + Chapters/Bookmarks (new `FunscriptChapter`/`FunscriptBookmark` records). Committed.
DONE: FunscriptChapter/FunscriptBookmark's time fields are real `System.TimeSpan`, not raw strings — decided so later editor code gets TimeSpan's arithmetic/comparisons for free instead of re-deriving them.
DONE: FORMAT.md fully reconciled against LIVE primary sources (re-fetched every §2 repo/branch/commit directly, not from memory) — real links added throughout; `range` default corrected 90 → 100 (checked OFS master `Funscript.cpp` which hardcodes 100, the governed schema which declares no default, and launchcontrol which uses 0 to mean something else entirely — 90 matched none of them). Confirmed the OSR2/SR6 TCode gap is still unfixed upstream on both branches.
DONE: FORMAT.md §4.6 added — design commitment for preserving unrecognised top-level/metadata keys on round-trip (motivated by a real, unshipped FIXME found in OFS `dev`'s `Funscript.h`: `JsonOther`). Not built yet — see TODO below.
DONE: Decided `Funscript` (the parser/writer facade, currently an empty stub) will be a `static class` — it's a pure bytes-in/bytes-out transform with nothing to hold between calls, same shape as `Math`/`Convert`/`JsonSerializer`. Not a singleton; no instances at all.
DONE: Worked out the `ParseTimeSpan` algorithm needed for chapters/bookmarks — confirmed empirically (ran it, didn't just assume) that BOTH `TimeSpan.Parse`/`TryParse` AND `System.ComponentModel.TimeSpanConverter` misparse the schema's `timeSpan` string format (`"623"` → 623 days not seconds; `"6:23"` → 6 hours 23 min not 6 min 23 sec). Full algorithm + citations in the Parsing TODO below. Not built yet — it's `Parse`-side work.
DONE: `EclipsePlayer.Funscript.Tests/Model/` folder created (empty, csproj-referenced) ready for the smoke test — see RESUME HERE.

Uncommitted at session end: `EclipsePlayer.Funscript.Tests.csproj` (just the empty `Model\` folder reference — harmless, safe to commit any time, or it'll auto-resolve once the smoke test file below is added to that folder).


## RESUME HERE — next single step

TODO: Go over "preserving unrecognised data" in real depth before/alongside building it (see FORMAT.md §4.6, added 2026-09-17 off OFS dev's own unshipped `JsonOther` FIXME). Needs a real design pass, not just the sketch in §4.6: where exactly `UnknownFields` bags live on `FunscriptDocument`/`FunscriptMetadata`, how `Parse` enforces the known/unknown key disjointness invariant, how `Export`/`ToJson` merges them back in, and whether `JsonElement` is the right carrier type or something else fits the immutable-record model better


## Then, in order (Funscript model + parser)

TODO: Parsing — `Funscript` becomes a `static class` (decided 2026-09-17: it's a pure bytes-in/bytes-out transform with no state to hold between calls, same shape as `Math`/`Convert`/`JsonSerializer` — not a singleton, no instances at all)
TODO: Parsing — LooksLikeFunscript(ReadOnlySpan<byte> head): cheap probe (JSON + "actions" key)
TODO: Parsing — Parse(stream, options) -> { Document?, Errors[], Warnings[] }; strict vs lenient(normalize); takes bytes/stream, NEVER a file path
TODO: Parsing — `ParseTimeSpan(string raw)` helper on `Funscript` (private/internal static) for chapters[].startTime/endTime + bookmarks[].time. CONFIRMED 2026-09-17 by actually running both: `TimeSpan.Parse`/`TryParse` AND `System.ComponentModel.TimeSpanConverter` (same underlying parser) both misread the schema's `^(\d+:)*\d+(\.\d+)?$` format — `"623"` -> 623 DAYS not 623 seconds, `"6:23"` -> 6 HOURS 23 MIN not 6 min 23 sec, `"5.5"` fails/throws outright. Neither is usable as-is. Correct algorithm: split on `:`, last segment = seconds (double, may have a fraction), walking backward: next = whole minutes, next = whole hours; combine as `hours*3600 + minutes*60 + secondsWithFraction` -> `TimeSpan.FromSeconds(total)`. Write-side is NOT symmetric-effort: `TimeSpan.ToString(@"hh\:mm\:ss\.fff")` already round-trips correctly (verified), always emit full HH:MM:SS.fff regardless of input shape — no custom formatter needed, only a custom parser.
TODO: Parsing — strict = base-1.0 write rules (int ms, unique strictly-increasing at, pos 0-100); lenient coerces float/unsorted/dupe/negative + warnings. TDD against the fixture folders (flat-1.0-minimal -> flat-1.0-full -> lenient/ as a [Theory] -> multi-axis).
TODO: Query — GetActionAt(TimeSpan) on AxisScript via binary search; return the bracketing pair. Own [Theory]: before-first, after-last, exact hit, between, empty, single.
TODO: decide whether interpolation lives in Funscript or Engine (FORMAT.md §9)
TODO: Serialization — model -> JSON: single-file 1.1 `axes` output, ids = semantic names, top-level actions always = stroke
TODO: EclipsePlayer.Funscript.csproj — PackageLicenseExpression=MPL-2.0, IsPackable=false, GenerateDocumentationFile=true
TODO: drop LICENSE (MPL-2.0) into EclipsePlayer.Funscript/
TODO: keep SPDX headers on every new .cs (AxisName.cs had lost its once — watch for it)


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
