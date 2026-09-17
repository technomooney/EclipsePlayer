// SPDX-FileCopyrightText: 2026 Marty Mooney
// SPDX-License-Identifier: MPL-2.0

using System.Collections.Immutable;

namespace EclipsePlayer.Funscript.Model;

public sealed record FunscriptDocument(AxisScript Stroke, ImmutableArray<AxisScript> Axes, FunscriptMetadata Metadata);

