// SPDX-FileCopyrightText: 2026 Marty Mooney
// SPDX-License-Identifier: MPL-2.0

using System.Text.Json;
using System.Text.Json.Serialization;

namespace EclipsePlayer.Funscript.Parsing.Dto;

internal sealed record FunscriptDocumentDto
{
    [JsonPropertyName("version")]
    public string? Version { get; init; }
    [JsonPropertyName("inverted")]
    public bool? Inverted { get; init; }
    [JsonPropertyName("range")]
    public int? Range { get; init; }
    [JsonPropertyName("metadata")]
    public FunscriptMetadataDto? Metadata { get; init; }
    [JsonPropertyName("actions")]
    public List<FunscriptActionDto>? Actions { get; init; }
    public Dictionary<string, JsonElement>? Extra { get; init; }
}
