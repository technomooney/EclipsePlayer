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

- **Eroscripts/OFS**, branch `dev` @ `e9c910e` (single-file multi-axis work landed in
  commits `c9592c0`…`b670ff6`, "read 2.0"…"preserve topic", 2025-09-27).
- **RFC: single-file multi-axis** — `discuss.eroscripts.com/t/rfc-single-file-multi-axis/267449`
  (draft as of 2026-09).
- **`funjack/launchcontrol`** — oldest written field description; origin of the
  canonical defaults.
- Field behaviour cross-checked against OpenFunscripter and MultiFunPlayer.

## 3. Base format (`version` "1.0")

A funscript is a JSON object.

| field | type | default | meaning |
|---|---|---|---|
| `actions` | array | — (**required**) | timestamped positions |
| `version` | string | `"1.0"` | informational only — see §4.3 |
| `inverted` | bool | `false` | swap position: 0 ⇄ 100 |
| `range` | int 0–100 | `90` | fraction of device physical range to use |
| `metadata` | object | absent | see §4.5 |
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
  "range": 90,
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

### 4.4 Always ignored

Unknown top-level keys, `rawActions`, and any unrecognised keys inside `metadata`.
Their presence is never an error.

### 4.5 Metadata

`metadata` is optional and free-form (its shape is "what OpenFunscripter writes":
`creator`, `title`, `description`, `duration`, `license`, `tags`, `performers`,
`type`, `notes`, `script_url`, `chapters`, `bookmarks`, …). This library parses the
keys it recognises into a typed structure and passes the rest through untouched. A
malformed `metadata` value never fails the parse.

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
  "range": 90,
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

On read, an `id` may be a semantic name **or** a TCode id (the RFC permits both) —
TCode ids are normalised per §5.6.

### 5.4 Single-file `channels` (`version` "2.0") — read only

The earlier, less-favoured proposal. `channels` is a JSON **object** keyed by axis
name: `{ "twist": { "actions": [ … ] } }`. Read for compatibility; never written.

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
| `suck` | suction / air pump | A1 |
| `vib` | vibration | — |
| `pump` | pump | — |
| `raw` | passthrough | — |
