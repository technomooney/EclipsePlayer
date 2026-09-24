// SPDX-FileCopyrightText: 2026 Marty Mooney
// SPDX-License-Identifier: MPL-2.0

using System.Linq;
using EclipsePlayer.Funscript.Model;

namespace EclipsePlayer.Funscript.Parsing.Dto;

internal static class MetadataMapper
{
    public static FunscriptMetadata ToDomain(FunscriptMetadataDto dto)
    {
        return new FunscriptMetadata()
        {
            Creator = dto.Creator,
            Title = dto.Title,
            Description = dto.Description,
            Duration = dto.Duration,
            DurationTime = dto.DurationTime,
            License = dto.License,
            Type = dto.Type,
            Notes = dto.Notes,
            ScriptUrl = dto.ScriptUrl,
            TopicUrl = dto.TopicUrl,
            VideoUrl = dto.VideoUrl,
            Tags = dto.Tags,
            Performers = dto.Performers,
            UnknownFields = dto.Extra,
            Chapters = dto.Chapters?.Select(dtoChapter => new FunscriptChapter(
                dtoChapter.Name!,
                Funscript.ParseTimeSpan(dtoChapter.StartTime!),
                Funscript.ParseTimeSpan(dtoChapter.EndTime!))).ToList(),
            Bookmarks = dto.Bookmarks?.Select(dtoBookmarks => new FunscriptBookmark(
                dtoBookmarks.Name!,
                Funscript.ParseTimeSpan(dtoBookmarks.Time!))).ToList()
        };
    }
}
