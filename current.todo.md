# EclipsePlayer — current working list

Plain-text task list. Actionable lines are prefixed `TODO:` so Rider's TODO tool
window indexes them (Settings > Editor > File Types: add `*.todo` to "Plain Text"
if items don't show up; or rename this to `current.todo.md` for zero-config).

Last updated: 2026-09-11 (end of session — building the Funscript model, one type at a time, teaching pace)


## DONE this session (2026-09-11)

DONE: Found + reconciled the governed schema (Eroscripts/funlib funscript.schema.json) and the actual TCode spec (multiaxis/TCode-Specification, cross-checked against the fuller Discord-circulated v0.3 text — the GitHub mirror is missing a section). FORMAT.md updated: axes[].id is now a closed TCode enum (strict mode rejects non-members), deprecated flags noted, new metadata fields added (durationTime, topic_url, video_url, channel hint, chapters/bookmarks timeSpan shape). See funscript-format-findings memory for the full citation trail.
DONE: AxisName.From extended — V0/V1/V2 -> vib0/vib1/vib2, A0 -> valve, A1 -> suck (unchanged, now double-confirmed), A2 -> lube. Matching static properties added. Committed (amended into the axis-map commit).
DONE: .editorconfig — SPDX file-header enforcement via Roslyn IDE0073, scoped MPL-2.0 (Hal/Funscript + their Tests) vs GPL-3.0-or-later (everything else), per licensing-model. Verified with dotnet format against real files in both a MPL and a GPL project before committing. Answers "can Rider add SPDX headers automatically" — yes, cross-tool, via dotnet format / build warnings, not a Rider-only setting.
DONE: Model/AxisScript.cs — `public sealed record AxisScript(AxisName Axis, bool Inverted, int Range, ImmutableArray<FunscriptAction> Actions);`. New concept covered: ImmutableArray<T>. Build green.
DONE: Model/FunscriptMetadata.cs — scalars-only first pass (init-only properties, not positional — 11 optional fields don't fit a positional record well). New concept covered: record with `{ get; init; }` properties + object-initializer construction. Build green. tags/performers/chapters/bookmarks deliberately deferred (need list-of-string handling + two new nested record types + the timeSpan-string question).

Uncommitted at session end: AxisScript.cs, FunscriptMetadata.cs (Marty's code — his to commit; check `git status --short` first, Rider auto-stages new files — see repo-state memory gotcha).


## RESUME HERE — next single step

TODO: Model/FunscriptDocument.cs — Stroke (AxisScript for top-level actions) + ImmutableArray<AxisScript> for the others + Metadata


## Then, in order (Funscript model + parser)

TODO: Model/FunscriptMetadata.cs — add Tags/Performers (list of string) + Chapters/Bookmarks (new Chapter/Bookmark record types, timeSpan-string field — decide raw string vs parsed TimeSpan) once ready; raw bag for unrecognized keys deferred to the parsing step (needs System.Text.Json.JsonElement, a new concept Marty wants to meet in context rather than pre-explained)
TODO: smoke test — hand-build a FunscriptDocument in a [Fact], assert a property; proves the model is usable. First green test.
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
