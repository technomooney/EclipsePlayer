// SPDX-FileCopyrightText: 2026 Marty Mooney
// SPDX-License-Identifier: MPL-2.0
namespace EclipsePlayer.Funscript.Model;
using System.Collections.Immutable;

public sealed record AxisScript(AxisName Axis, bool Inverted, int Range, ImmutableArray<FunscriptAction> Actions);
