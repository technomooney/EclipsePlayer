<!-- SPDX-FileCopyrightText: 2026 Marty Mooney -->
<!-- SPDX-License-Identifier: MPL-2.0 -->

# Funscript format — the shape `EclipsePlayer.Funscript` reads and writes

**Status:** working reference, revised 2026-09-09. There is **no formal upstream
funscript specification** — no standards body, no schema, no versioned grammar. This
document is the format definition this library is built against. A change here is a
change to the library's contract and ships as a new release; the library does not
negotiate format variants at runtime.

## 1. Scope

This library, given **bytes / a stream** (never a file path):

- identifies whether the content looks like a funscript (cheap probe),
- parses it into an immutable model,
- validates it (strict) or normalizes it (lenient),
- serialises the model back out,
- answers positional queries (`GetActionAt`).

It does **not**: touch the filesystem, discover or correlate files, apply motion
transforms (offset, speed limiting, device interpolation), or map axes to device
channels. Those live in `EclipsePlayer.IO`, `EclipsePlayer.Engine` and
`EclipsePlayer.Hal` respectively.

## 2. Pinned sources

Re-verified live against source, 2026-09-17 — links resolve as of that date; repos are
third-party and can move or change without notice, hence the pinned commits/branches.

- **[`Eroscripts/OFS`](https://github.com/Eroscripts/OFS)**, branch
  [`dev`](https://github.com/Eroscripts/OFS/tree/dev) @
  [`e9c910e`](https://github.com/Eroscripts/OFS/commit/e9c910e830d628171fb3718fbf294d9578424ef6)
  (single-file multi-axis work landed in commits
  [`c9592c0`](https://github.com/Eroscripts/OFS/commit/c9592c0168c34a1cdbc2ae0f26dfd4f0e4e2f04c)
  "read 2.0" …
  [`b670ff6`](https://github.com/Eroscripts/OFS/commit/b670ff639050641952f7e8b09e0230619cc423e6)
  "preserve topic", 2025-09-27).
- **RFC: single-file multi-axis** —
  [`discuss.eroscripts.com/t/rfc-single-file-multi-axis/267449`](https://discuss.eroscripts.com/t/rfc-single-file-multi-axis/267449)
  (draft as of 2026-09).
- **[`funjack/launchcontrol`](https://github.com/funjack/launchcontrol)** — oldest
  written field description; source file
  [`protocol/funscript/funscript.go`](https://github.com/funjack/launchcontrol/blob/master/protocol/funscript/funscript.go).
  Re-checked 2026-09-17: this is Handy/Launch-specific, not a generic funscript
  reader — `Range` defaults to Go's zero value `0`, which this code treats as "don't
  scale, use position unmodified," a different *meaning* than a percentage default.
  Its own JSON schema copy is at
  [`schemas/funscript.schema.json`](https://github.com/funjack/launchcontrol/blob/master/schemas/funscript.schema.json)
  (older/narrower than the governed schema below; not treated as authoritative where
  they differ).
- **[`Eroscripts/funlib`](https://github.com/Eroscripts/funlib)** —
  [`funscript.schema.json`](https://github.com/Eroscripts/funlib/blob/master/funscript.schema.json)
  (JSON Schema draft-07), the governed schema. Source of the `axisId` enum (§5.6) and
  the metadata fields in §4.5. The repo also has a narrative
  [`docs/funscript-format-specification.md`](https://github.com/Eroscripts/funlib/blob/master/docs/funscript-format-specification.md)
  — looser and less precise than the schema itself (e.g. it says `"vibe"` where the
  schema/this doc use `vib0`/`vib1`/`vib2`); **not treated as an independent
  authority**, the schema file wins on any conflict.
- **[`multiaxis/TCode-Specification`](https://github.com/multiaxis/TCode-Specification)**
  — [`master`](https://github.com/multiaxis/TCode-Specification/tree/master) (v0.3,
  2021-05-10) and [`Dev`](https://github.com/multiaxis/TCode-Specification/tree/Dev)
  (v0.4, draft) branches, **cross-checked against the fuller v0.3 text circulating on
  the T-code Discord** (pasted in full by Marty, 2026-09-11). Re-confirmed 2026-09-17
  directly against both branches' live `README.md`: **both are still missing** the
  section present in the Discord copy — "Multi-Axis Devices → Extra functions on the
  OSR2/SR6" — so the GitHub repo alone is **not** treated as complete; the Discord
  text is the fuller/authoritative one for this section. It documents: `L0-L2`/`R0-R2`
  defined per-axis meanings (matches GitHub); `V0`, `V1` = vibration motor channels
  (OSR2/SR6, undifferentiated by function); `A0` = direct valve position; `A1` =
  "suck algorithm" valve control; `A2` = lube motor speed — all specific to the
  OSR2/SR6 "extra functions," not a universal definition of the `A`/`V` letters for
  every device.
- Field behaviour cross-checked against OpenFunscripter and MultiFunPlayer.

## 3. Base format (`version` "1.0")

A funscript is a JSON object.

| field | type | default | meaning |
|---|---|---|---|
| `actions` | array | — (**required**) | timestamped positions |
| `version` | string | `"1.0"` | informational only — see §4.3 |
| `inverted` | bool | `false` | swap position: 0 ⇄ 100. **Deprecated** per the governed schema (§2) — "not widely supported" — but still real, still parsed |
| `range` | int 0–100 | `100` | fraction of device physical range to use. Same deprecation note as `inverted`. Default re-verified 2026-09-17 against OFS master (`Funscript.cpp`, `json["range"] = 100` hardcoded on write); the governed schema declares no default at all, and `launchcontrol` treats an omitted/zero value as "don't scale" rather than a percentage — a prior version of this doc stated `90`, which matched none of the three sources on inspection |
| `metadata` | object | absent | see §4.5 |
| `channel` | string | absent | human-readable name **hint** for the top-level `actions` array, per the governed schema. Informational only, never identity — always ignored per §4.4, not treated as an axis id |
| `rawActions` | array | absent | editor pre-simplification list — always ignored |

### Action object

```json
{ "at": 1500, "pos": 80 }
```

| field | type | meaning |
|---|---|---|
| `at` | int, milliseconds from media start | timestamp |
| `pos` | int 0–100 | position (0 = retracted, 100 = extended, unless `inverted`) |

### Example

```json
{
  "version": "1.0",
  "inverted": false,
  "range": 100,
  "metadata": { "creator": "someone", "duration": 623 },
  "actions": [
    { "at": 0,    "pos": 0   },
    { "at": 500,  "pos": 100 },
    { "at": 1200, "pos": 20  }
  ]
}
```

## 4. Parsing contract

Two modes, selected by the caller.

### 4.1 Strict

Rejects anything not conforming. Rules:

- root is a JSON object;
- `actions` is present and is an array;
- every action has an **integer** `at` ≥ 0 and an **integer** `pos`;
- `pos` is within 0–100;
- `at` is **strictly increasing** across the array (already sorted, no duplicates);
- if present: `inverted` is bool, `range` is int 0–100, `version` is string;
- unknown top-level keys are **ignored, not rejected** (forward compatibility);
- `axes` / `channels` are recognised (§5), not treated as unknown.

Strict is the shape this library itself **writes** (§6).

### 4.2 Lenient (normalize)

Best-effort recovery of real-world files. Produces a valid model plus a list of
warnings describing what was changed.

| input deviation | action |
|---|---|
| `at` is a float | round to nearest ms |
| `pos` is a float | round, then clamp 0–100 |
| `pos` outside 0–100 | clamp |
| actions out of order | stable sort by `at` |
| duplicate `at` | keep the **last** occurrence (later entry wins) |
| `at` < 0 | drop that action |
| `inverted` / `range` / `version` missing | apply the §3 default |
| `range` outside 0–100 | clamp |
| trailing/leading BOM, UTF-8/UTF-16 | decoded transparently |
| `actions` missing, or root not an object | **still an error** — nothing to recover |
| `actions` present but empty | valid; warning only |

### 4.3 `version` is informational

Never branch on the `version` string. Detect capability by key presence:
`channels` → §5.4; else `axes` → §5.3; else flat §3.

### 4.4 Always tolerated (never an error)

Unknown top-level keys, `rawActions`, and any unrecognised keys inside `metadata`.
None of these fail the parse. They split into two different fates on write, per §4.6:
`rawActions` is a **deliberate exclusion** — an editor's pre-simplification cache,
always discarded, never round-tripped. Unknown top-level keys and unknown `metadata`
keys are **preserved**, not discarded — see §4.6 for the mechanism.

### 4.5 Metadata

`metadata` is optional and free-form (its shape is "what OpenFunscripter writes" plus
what the governed schema, §2, adds: `creator`, `title`, `description`, `duration`
(number, seconds), `durationTime` (timeSpan string, human-readable duration hint),
`license`, `tags`, `performers`, `type`, `notes`, `script_url`, `topic_url`,
`video_url`, `chapters`, `bookmarks`, …). This library parses the keys it recognises
into a typed structure; unrecognised keys are preserved per §4.6, not discarded. A
malformed `metadata` value never fails the parse.

`chapters` and `bookmarks` entries use a **`timeSpan` string** (`HH:MM:SS.ms` or bare
seconds, e.g. `"00:03:00.017"`), not integer ms like an action's `at` — a different
representation living inside the same file. `chapters[]` = `{name, startTime,
endTime}`; `bookmarks[]` = `{name, time}`.

### 4.6 Preserving unrecognised data (round-trip safety)

**Motivation.** OFS's own `dev` branch header (`OFS-lib/Funscript/Funscript.h`) has a
drafted-but-unshipped fix for exactly this gap:

```cpp
// FIXME: OFS should be able to retain metadata injected by other programs without overwriting it
//nlohmann::json JsonOther;
```

OFS writes its file by explicitly listing every field it knows about — anything it
doesn't recognise is simply absent from that list, so a load-then-save round trip
silently destroys data another tool wrote (a custom extension field, a foreign app's
metadata key). The fix was drafted and never wired up — plausibly because `Funscript`
there is a long-lived, mutable, in-editor object, and keeping a raw JSON side-bag in
sync with fields the user is actively editing over a session is a real
staleness/synchronisation hazard in a mutable design.

**This library's design commitment**, made possible by the model being immutable
records rather than a long-lived mutable object (every `FunscriptDocument` is
produced fresh, once, by `Parse` — there is no live session to drift out of sync
during):

- At parse time, every incoming JSON key is sorted into exactly one of two disjoint
  sets: keys the model understands (become typed fields) and everything else (an
  opaque bag). A key must never appear in both — that disjointness is what makes the
  later write-merge unambiguous, and it only has to be enforced once, at parse time.
- The opaque bag is carried on the model itself (not left behind in a parse-result
  wrapper), because serialisation only has the model to work from. Two separate bags:
  one for unknown top-level keys (on `FunscriptDocument`), one for unknown `metadata`
  keys (on `FunscriptMetadata`).
- Each entry's value is an untouched `System.Text.Json.JsonElement` — this library
  doesn't need to understand a key's shape to preserve it byte-for-byte.
- On write, typed fields are emitted as normal, then every entry still sitting in the
  bag is emitted alongside them. Disjointness guarantees no key collision to resolve.
- Scope: top-level keys and `metadata` keys only, per §4.4/§4.5. Does **not** extend
  to unknown fields inside individual `axes`/`channels`/`actions` entries (a separate,
  harder problem, not yet designed), and never applies to `rawActions` (§4.4 —
  deliberately discarded, not preserved).

**Mechanism (decided 2026-09-24): `System.Text.Json`'s built-in
`[JsonExtensionDataAttribute]`, not a hand-rolled disjointness check.** Placed on a
`Dictionary<string, JsonElement>` property, it makes the framework itself sort
unmapped JSON keys into that dictionary during `Deserialize` and splice them back in
as top-level properties during `Serialize` — the disjointness invariant above is
enforced by the BCL, not by code in this library. Constraints confirmed against the
API docs: the dictionary's value type must be `JsonElement` or `object` (keyed by
`string`); only one such property is allowed per type (a second throws
`InvalidOperationException`); it's compatible with a positional-record constructor
(the extension-data property is populated after construction, not passed as a
constructor argument).

This can't sit directly on `FunscriptDocument`/`FunscriptMetadata`, since the domain
model doesn't mirror the wire shape 1:1 (`FunscriptAction.AtMilliSecond`/`ToPosition`
vs. wire `at`/`pos`, TCode alias resolution, strict/lenient coercion, custom
`TimeSpan` parsing). Instead: an internal wire-shape DTO (Data Transfer Object) per
JSON object needing preservation — one for the top level, one for `metadata` — each
carrying a `[JsonExtensionData] Dictionary<string, JsonElement>? Extra` property.
`Parse` deserialises into the DTO first (getting the known/unknown split for free),
hand-maps DTO → domain model as already planned, and carries `Extra` over into the
domain record's bag field. Write reverses this: hand-build a DTO from the domain
model (typed fields + the bag merged back into `Extra`) and serialise that.

**Lifetime, verified empirically (2026-09-24):** the concern was whether a
`JsonElement` captured via `[JsonExtensionData]` stays valid after the call to
`Deserialize` returns, or is secretly tied to an internal `JsonDocument` that could
be disposed — the API docs don't say either way. Probe: deserialise JSON with an
unmapped key into a record using `[JsonExtensionData]`, capture the resulting
`JsonElement`, drop every other reference (including the deserialised object) and
force `GC.Collect()` + `GC.WaitForPendingFinalizers()`, then read the captured
element back. It read correctly — the value is self-contained, not reliant on
anything the caller needs to keep alive. Safe to hold on a long-lived immutable
record.

Not yet built — the mechanism above is decided; the DTO shapes and the `Parse`/write
mapping code are the next implementation step (see `current.todo.md`).

## 5. Multi-axis

### 5.1 Axis identity

An axis is identified by a **stable semantic name**, never by a device-protocol
(TCode) channel id. TCode is a device convention, not a governed standard, and its
axis set has changed across versions and vendors — it does not belong in the file
format or in this library's model.

Vocabulary (well-known constants): `stroke`, `surge`, `sway`, `twist`, `roll`,
`pitch`, `suck`, plus the long tail `vib`, `pump`, `raw`.

The identity type is a small value type over a normalised string. **Unknown names
pass through unchanged** — they are not rejected and not coerced.

`stroke` is the primary axis and is always carried in the **top-level `actions`**
(§5.5), never as an entry inside `axes` / `channels`.

### 5.2 Classic separate-file convention (read)

Historically, each axis is its own file, identified by a filename suffix on the base
name — `movie.funscript` (stroke), `movie.roll.funscript`, `movie.twist.funscript`,
etc. Nothing inside the file identifies the axis. Grouping these files is
`EclipsePlayer.IO`'s job; this library only ever sees one stream at a time and takes
the axis name from the caller.

### 5.3 Single-file `axes` (`version` "1.1") — read **and write**

The form this library **emits** (§6) and the one the RFC is converging on. `axes` is
a JSON **array** (order preserved, which the RFC's "on overlap, take the last" rule
depends on). Each entry is `{ "id": <axis name>, "actions": [ … ] }`.

```json
{
  "version": "1.1",
  "inverted": false,
  "range": 100,
  "metadata": { },
  "actions": [ { "at": 0, "pos": 50 } ],
  "axes": [
    { "id": "twist", "actions": [ { "at": 0, "pos": 50 } ] },
    { "id": "roll",  "actions": [ { "at": 0, "pos": 50 } ] }
  ]
}
```

A classic 1.0 player ignores the unknown `axes` key and plays `actions` — so this is
a backward-compatible extension, not a break.

On read, `axes[].id` is TCode-only, a closed enum:
`L0,L1,L2,R0,R1,R2,A0,A1,A2,V0,V1,V2` (per the governed schema, §2). Semantic names
are never valid here — only as `channels` object keys (§5.4). Strict mode **rejects**
an `axes[].id` outside this enum; lenient mode passes an unrecognised id through
unchanged (§4.2). Recognised ids are normalised to names per §5.6.

### 5.4 Single-file `channels` (`version` "2.0") — read only

The earlier, less-favoured proposal. `channels` is a JSON **object** keyed by axis
name (free-form string, not the `axes` TCode enum): `{ "twist": { "actions": [ … ] } }`.
Read for compatibility; never written.

If both top-level `actions` and `channels.stroke` are present, the schema's stated
intent is: single-axis devices (e.g. Handy) play `actions`, multi-axis devices (e.g.
SR6) play `channels.stroke`. This is a playback/device concern — `Funscript` parses
both and lets the caller (`Engine`/`Hal`) pick; it does not resolve the choice itself.

### 5.5 Top-level `actions` is always `stroke`

In every form — flat, `axes`, `channels` — the top-level `actions` array is the
`stroke` (L0) axis. It is always written, even for a single-axis script, so any
classic consumer can play it.

### 5.6 Inbound TCode → name alias map (versioned)

Applied only when reading an `axes` `id` (or other foreign input) that uses a TCode
id. Output is always names.

| TCode id | name |
|---|---|
| `L0` | `stroke` |
| `L1` | `surge` |
| `L2` | `sway` |
| `R0` | `twist` |
| `R1` | `roll` |
| `R2` | `pitch` |
| `A1` | `suck` |
| `V0` | `vib0` |
| `V1` | `vib1` |
| `V2` | `vib2` |
| `A0` | `valve` |
| `A2` | `lube` |

`A0`/`A1`/`A2`/`V0`/`V1` here come from the T-code v0.3 spec's own "Extra functions on
the OSR2/SR6" text (§2): `A0` = direct valve position, `A1` = "suck algorithm" valve
control (matches the OFS fork's independently-sourced `A1 = suck`), `A2` = lube motor
speed, `V0`/`V1` = the two vibration motor channels, undifferentiated by function —
hence numbered, not named. The spec frames these explicitly as **OSR2/SR6-specific**
extras, not a universal definition of what `A`/`V` mean for every device — a different
multi-axis device could assign them differently, and this table would need a
per-device variant if one is found. `V2` has no citation on any device seen so far;
kept numbered/generic for consistency with `V0`/`V1`.

This table is versioned with the library. It is a **read-side compatibility shim
only** — the reverse direction (name → device channel) is `EclipsePlayer.Hal`'s
concern and is parameterised there by device and protocol dialect.

## 6. What this library writes

- Single file, `version` `"1.1"`.
- `stroke` in top-level `actions`; every other axis as an `{ "id": <name>, "actions": … }`
  entry in the `axes` array, in a stable order.
- Strict shape (§4.1): integer `at` (ms, unique, strictly increasing), integer `pos`
  0–100.
- `metadata` always written as an object (may be empty). `rawActions` never written.

Emitting the classic separate-suffix files instead is an **export option in
`EclipsePlayer.IO`**, not a mode of this library.

## 7. Explicitly out of scope

| concern | owner |
|---|---|
| opening files, byte acquisition, encoding sniffing | `EclipsePlayer.IO` |
| finding/grouping sidecar axis files, script ↔ media matching | `EclipsePlayer.IO` |
| per-media script offset, speed limiting, no-motion gap skipping | `EclipsePlayer.Engine` |
| interpolation for real-time device output | `EclipsePlayer.Engine` (see §9) |
| axis name → device channel, TCode dialects | `EclipsePlayer.Hal` |

## 8. Versioning of this document

Tied to the library version. Any change to the accepted or emitted shape is a
breaking change and ships as a new `EclipsePlayer.Funscript` release with this file
updated and the §2 pins bumped.

## 9. Open questions

- **Interpolation location.** `GetActionAt` returning the bracketing action pair
  (this library) vs. returning an interpolated position. Leaning: this library
  exposes the pair and the raw query; motion-shaping interpolation for devices is
  `Engine`. Decide when the query API is built.
- **In-memory timestamp representation.** On disk it is integer milliseconds. The
  public query API is expected to take `TimeSpan`; internal storage (int ms vs ticks)
  is an implementation detail, not part of this contract.

## Appendix — axis reference

TCode column is **for cross-reference only**; it is not used by this library.

| name | motion | TCode (ref) |
|---|---|---|
| `stroke` | up / down (primary) | L0 |
| `surge` | forward / back | L1 |
| `sway` | left / right | L2 |
| `twist` | rotation (yaw) | R0 |
| `roll` | tilt side to side | R1 |
| `pitch` | tilt front to back | R2 |
| `suck` | suction / air pump (OSR2/SR6: "suck algorithm" valve control) | A1 |
| `valve` | direct valve position (OSR2/SR6) | A0 |
| `lube` | lube motor speed (OSR2/SR6) | A2 |
| `vib0` | vibration motor 0 | V0 |
| `vib1` | vibration motor 1 | V1 |
| `vib2` | vibration motor 2 (no known device yet) | V2 |
| `vib` | vibration (classic single-file suffix convention, §5.2) | — |
| `pump` | pump | — |
| `raw` | passthrough | — |
