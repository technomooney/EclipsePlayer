// SPDX-FileCopyrightText: 2026 Marty Mooney
// SPDX-License-Identifier: MPL-2.0

using System.Text.Json.Serialization;

namespace EclipsePlayer.Funscript.Parsing.Dto;

internal sealed record FunscriptChapterDto
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }
    [JsonPropertyName("startTime")]
    public string? StartTime { get; init; }
    [JsonPropertyName("endTime")]
    public string? EndTime { get; init; }
}
