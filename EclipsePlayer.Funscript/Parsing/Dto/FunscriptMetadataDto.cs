// SPDX-FileCopyrightText: 2026 Marty Mooney
// SPDX-License-Identifier: MPL-2.0

using System.Text.Json;
using System.Text.Json.Serialization;
namespace EclipsePlayer.Funscript.Parsing.Dto;

internal sealed record FunscriptMetadataDto
{
    [JsonPropertyName("creator")]
    public string? Creator { get; init; }
    [JsonPropertyName("title")]
    public string? Title { get; init; }
    [JsonPropertyName("description")]
    public string? Description { get; init; }
    [JsonPropertyName("duration")]
    public double? Duration { get; init; }
    [JsonPropertyName("durationTime")]
    public string? DurationTime { get; init; }
    [JsonPropertyName("type")]
    public string? Type { get; init; }
    [JsonPropertyName("notes")]
    public string? Notes { get; init; }
    [JsonPropertyName("license")]
    public string? License { get; init; }
    [JsonPropertyName("script_url")]
    public string? ScriptUrl { get; init; }
    [JsonPropertyName("topic_url")]
    public string? TopicUrl { get; init; }
    [JsonPropertyName("video_url")]
    public string? VideoUrl { get; init; }
    [JsonPropertyName("tags")]
    public List<string>? Tags { get; init; }
    [JsonPropertyName("performers")]
    public List<string>? Performers { get; init; }
    [JsonPropertyName("chapters")]
    public List<FunscriptChapterDto>? Chapters { get; init; }
    [JsonPropertyName("bookmarks")]
    public List<FunscriptBookmarkDto>? Bookmarks { get; init; }
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Extra { get; init; }

}
