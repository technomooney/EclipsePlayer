// SPDX-FileCopyrightText: 2026 Marty Mooney
// SPDX-License-Identifier: MPL-2.0

using System.Text.Json.Serialization;

namespace EclipsePlayer.Funscript.Parsing.Dto;

internal sealed record FunscriptActionDto
{
    [JsonPropertyName("at")]
    public double? AtMilliSecond { get; init; }
    [JsonPropertyName("pos")]
    public int? Position { get; init; }
}
