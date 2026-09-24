// SPDX-FileCopyrightText: 2026 Marty Mooney
// SPDX-License-Identifier: MPL-2.0

using System.Text.Json;

namespace EclipsePlayer.Funscript.Model;

public sealed record FunscriptMetadata
{
    public string? Creator { get; init; }
    public string? Title { get; init; }
    public string? Description { get; init; }
    public double? Duration { get; init; }
    public string? DurationTime { get; init; }
    public string? License { get; init; }
    public string? Type { get; init; }
    public string? Notes { get; init; }
    public string? ScriptUrl { get; init; }
    public string? TopicUrl { get; init; }
    public string? VideoUrl { get; init; }
    public List<string>? Tags { get; init; }
    public List<string>? Performers { get; init; }
    public List<FunscriptChapter>? Chapters { get; init; }
    public List<FunscriptBookmark>? Bookmarks { get; init; }
    public Dictionary<string, JsonElement>? UnknownFields { get; init; }



}
