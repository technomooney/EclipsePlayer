# EclipsePlayer — current working list

Plain-text task list. Actionable lines are prefixed `TODO:` so Rider's TODO tool
window indexes them (Settings > Editor > File Types: add `*.todo` to "Plain Text"
if items don't show up; or rename this to `current.todo.md` for zero-config).

Last updated: 2026-09-24 (metadata DTO + read-direction mapping built and building clean)


## DONE this session (2026-09-24)

DONE: First smoke test — `EclipsePlayer.Funscript.Tests/Model/FunscriptDocumentTest.cs`, `TestFunDocWiring`. Hand-built 4 `FunscriptAction`s -> `ImmutableArray.Create` -> `AxisScript` (Stroke) -> `FunscriptMetadata` (object-initializer syntax) -> `FunscriptDocument`. Asserts on `Stroke.Actions.Length`, `Metadata.Creator`, `Metadata.Title`. Passes. Proves the model graph (actions -> axis -> document, with metadata attached) actually holds together end to end.
DONE: FORMAT.md §4.6 mechanism decided — `[JsonExtensionData]` on an internal wire-shape DTO's `Dictionary<string, JsonElement>? Extra` property, not a hand-rolled known/unknown key split. Verified empirically that a captured `JsonElement` survives after the deserialized object and all other references are dropped and a GC is forced — safe to hold on a long-lived immutable record. Full writeup in FORMAT.md §4.6.
DONE: `Parsing/Dto/FunscriptMetadataDto.cs`, `FunscriptChapterDto.cs`, `FunscriptBookmarkDto.cs` — internal wire-shape DTOs for the `metadata` object only (top-level 1.0/1.1/2.0 DTO(s) deliberately deferred to the full `Parse` work — metadata's shape doesn't vary by version, so it didn't need to wait). Every field uses `[JsonPropertyName]` explicitly since the wire keys mix casing conventions (`duration` vs `durationTime` vs `script_url`) and `System.Text.Json` is case-sensitive by default. `FunscriptMetadataDto.Extra` carries the `[JsonExtensionData]` bag.
DONE: `Model/FunscriptMetadata.cs` gained `UnknownFields` (`Dictionary<string, JsonElement>?`) — the domain-side home for `Extra` per §4.6.
DONE: `Funscript.cs` is now the decided `static class` (was still an empty instance-class stub); gained `internal static TimeSpan ParseTimeSpan(string)` implementing the 2026-09-17 algorithm (split on `:`, walk backward from seconds through minutes/hours using the `^n` index-from-end operator, `0` fallback via the ternary operator when a segment is absent).
DONE: `Parsing/Dto/MetadataMapper.cs` — `internal static FunscriptMetadata ToDomain(FunscriptMetadataDto dto)`. Scalars map straight across; `Chapters`/`Bookmarks` use `dto.X?.Select(...).ToList()` (LINQ projection + null-conditional) combined with `Funscript.ParseTimeSpan` and the `!` null-forgiving operator (deliberately not validated yet — that's strict/lenient work, still ahead). Read direction only; write direction (domain -> DTO) not yet built.
DONE: `dotnet build EclipsePlayer.slnx` — clean, 0 warnings, 0 errors, confirms all of the above actually compiles together.

Not yet done: `MetadataMapper.ToDomain` has never actually been run against real JSON (only compiled) — no test exists yet. Write-direction mapping (domain -> DTO, for the eventual `Write`/`ToJson`) also not started.


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

TODO: A real test for `MetadataMapper.ToDomain` — deserialize actual JSON (e.g. a string literal with a known key, an unrecognised key, and a `chapters`/`bookmarks` entry) into `FunscriptMetadataDto` via `JsonSerializer.Deserialize`, run it through `ToDomain`, assert the typed fields, the `TimeSpan` conversion, and that the unrecognised key landed in `UnknownFields`. It's only ever been compiled so far, never actually run.
TODO: Write-direction mapping — domain `FunscriptMetadata` -> `FunscriptMetadataDto` (merging `UnknownFields` back into `Extra`), for the eventual `Write`/`ToJson` path.


## Then, in order (Funscript model + parser)

TODO: Parsing — build the top-level wire-shape DTO(s) for §4.6, deferred when the metadata DTO was built since it has to handle 1.0 flat/1.1 `axes`/2.0 `channels` — decide then whether that's one DTO with optional fields per version or three separate DTOs picked by `version`
TODO: Parsing — LooksLikeFunscript(ReadOnlySpan<byte> head): cheap probe (JSON + "actions" key)
TODO: Parsing — Parse(stream, options) -> { Document?, Errors[], Warnings[] }; strict vs lenient(normalize); takes bytes/stream, NEVER a file path
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
