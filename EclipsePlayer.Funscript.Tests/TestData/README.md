<!-- SPDX-FileCopyrightText: 2026 Marty Mooney -->
<!-- SPDX-License-Identifier: MPL-2.0 -->

# Funscript test fixtures

**Provenance:** synthetic. Hand-authored against `../../EclipsePlayer.Funscript/FORMAT.md`
(revision 2026-09-09). These are *not* exports from OpenFunscripter, ScriptPlayer, or
any other tool — each file targets one clause of the format contract so a failing
test points at a specific rule.

Real tool-exported scripts should be added over time as a **regression corpus** under
`corpus/` (create it when you have files), kept separate from these contract fixtures.

## Expected outcomes

Strict = `FunscriptParseOptions` strict mode. Lenient = normalize mode (parses, may
emit warnings).

| file | strict | lenient | exercises |
|---|---|---|---|
| `valid/flat-1.0-minimal.funscript` | pass | pass | bare `actions`-only object is valid |
| `valid/flat-1.0-full.funscript` | pass | pass | every documented wrapper field + metadata |
| `valid/single-action.funscript` | pass | pass | one action |
| `valid/empty-actions.funscript` | pass (warn) | pass (warn) | empty `actions` array is valid, not an error |
| `valid/multiaxis-1.1-axes.funscript` | pass | pass | `axes` array, named ids, top-level = stroke |
| `valid/multiaxis-2.0-channels.funscript` | pass | pass | `channels` object read path |
| `valid/unknown-keys-ignored.funscript` | pass | pass | `rawActions`, unknown top-level key, unknown metadata key all ignored |
| `valid/tcode-axis-ids.funscript` | pass | pass | `axes[].id` = `R0`/`L1`/`A1` → normalised to `twist`/`surge`/`suck` (§5.6) |
| `sidecar/scene.funscript` (+ `.roll`, `.twist`) | pass | pass | classic separate-file set; each stream parsed independently, axis name supplied by caller |
| `lenient/float-timestamps.funscript` | **reject** | pass + warn | `at` as float → round to ms |
| `lenient/unsorted-actions.funscript` | **reject** | pass + warn | stable sort by `at` |
| `lenient/duplicate-at.funscript` | **reject** | pass + warn | duplicate `at` → last wins (pos 100 at 500, not 40) |
| `lenient/negative-at.funscript` | **reject** | pass + warn | `at < 0` action dropped |
| `lenient/pos-out-of-range.funscript` | **reject** | pass + warn | `pos` clamped to 0 / 100 |
| `lenient/range-out-of-bounds.funscript` | **reject** | pass + warn | `range` 250 → clamp to 100 |
| `raw/utf8-bom.funscript` | pass | pass | leading UTF-8 BOM (`EF BB BF`) decoded transparently; content is otherwise valid |
| `invalid/not-json.funscript` | reject | reject | not JSON |
| `invalid/root-is-array.funscript` | reject | reject | root is an array, not an object |
| `invalid/no-actions-key.funscript` | reject | reject | no `actions` — nothing to recover |
| `invalid/actions-not-array.funscript` | reject | reject | `actions` is a string |

## Notes

- `raw/utf8-bom.funscript` is the only fixture with a non-UTF-8-plain byte prefix;
  regenerate it with
  `printf '\xEF\xBB\xBF{...}' > raw/utf8-bom.funscript` if it gets normalised by an
  editor.
- All fixtures are deliberately tiny (≤ 6 actions). They test the contract, not
  performance.
- If `FORMAT.md` changes, update this matrix in the same commit.
