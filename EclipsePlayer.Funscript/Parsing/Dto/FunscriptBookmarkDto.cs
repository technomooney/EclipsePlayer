// SPDX-FileCopyrightText: 2026 Marty Mooney
// SPDX-License-Identifier: MPL-2.0

using System.Text.Json.Serialization;

namespace EclipsePlayer.Funscript.Parsing.Dto;

internal sealed record FunscriptBookmarkDto
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }
    [JsonPropertyName("time")]
    public string? Time { get; init; }
}
