// SPDX-FileCopyrightText: 2026 Marty Mooney
// SPDX-License-Identifier: MPL-2.0

using System.Text.Json;
using EclipsePlayer.Funscript.Model;
using EclipsePlayer.Funscript.Parsing.Dto;

namespace EclipsePlayer.Funscript.Tests.Parsing;

public class MetadataMapperTest
{
    private static FunscriptMetadata LoadMetadata(string folder, string filename)
    {
        var fileOnDisk = Path.Combine(AppContext.BaseDirectory,"TestData",folder, filename);
        var rawData = File.ReadAllText(fileOnDisk);
        using var doc =  JsonDocument.Parse(rawData);
        var metadataJson = doc.RootElement.GetProperty("metadata");
        var dto = metadataJson.Deserialize<FunscriptMetadataDto>();
        Assert.NotNull(dto);
        return MetadataMapper.ToDomain(dto);
    }

    [Fact]
    public void TestMetadataMapperScalarsValid()
    {
        var validFunscriptMetadataObj = LoadMetadata("valid", "flat-1.0-full.funscript");
        Assert.Equal("synthetic", validFunscriptMetadataObj.Creator);
        Assert.Equal("full flat sample", validFunscriptMetadataObj.Title);
        Assert.Equal(5400.0, validFunscriptMetadataObj.Duration);
        Assert.Equal("basic",validFunscriptMetadataObj.Type);
        Assert.Equal("every documented wrapper field present", validFunscriptMetadataObj.Notes);
        Assert.Equal("CC0-1.0", validFunscriptMetadataObj.License);
        Assert.Equal("https://example.com/script",  validFunscriptMetadataObj.ScriptUrl);
        Assert.Equal("https://example.com/topic",   validFunscriptMetadataObj.TopicUrl);
        Assert.Equal("https://example.com/video",   validFunscriptMetadataObj.VideoUrl);
    }

    [Fact]
    public void TestMetadataListsValid()
    {
        var validFunscriptMetadataObj = LoadMetadata("valid", "flat-1.0-full.funscript");
        List<string> expectedTagsList = ["test"];
        Assert.Equal(expectedTagsList, validFunscriptMetadataObj.Tags);

        List<string> expectedPerformers = new List<string> { "performer one", "performer two" };
        Assert.Equal(expectedPerformers, validFunscriptMetadataObj.Performers);
    }

    [Fact]
    public void TestMetadataChaptersValid()
    {
        var validFunscriptMetadataObj = LoadMetadata("valid", "flat-1.0-full.funscript");
        List<FunscriptChapter> expectedChaptersList = [
            new FunscriptChapter("minutes-seconds", new TimeSpan(0, 6, 23), new TimeSpan(0,12, 5)),
            new FunscriptChapter("hours-minutes-seconds", new TimeSpan(1, 2, 3), new TimeSpan(1,30,00))
        ];
        Assert.Equal(expectedChaptersList, validFunscriptMetadataObj.Chapters);
    }

    [Fact]
    public void TestMetadataBookmarksValid()
    {
        var validFunscriptMetadataObj = LoadMetadata("valid", "flat-1.0-full.funscript");
        List<FunscriptBookmark> expectedBookmarksList =
        [
            new FunscriptBookmark("bare-seconds", TimeSpan.FromSeconds(623)),
            new FunscriptBookmark("minutes-seconds", new TimeSpan(0, 6, 23))
        ];
        Assert.Equal(expectedBookmarksList, validFunscriptMetadataObj.Bookmarks);
    }

    [Fact]
    public void TestMetadataNullableNull()
    {
        var validNullFunscriptMetadataObj = MetadataMapper.ToDomain(new FunscriptMetadataDto());
        Assert.Null(validNullFunscriptMetadataObj.Creator);
        Assert.Null(validNullFunscriptMetadataObj.Title);
        Assert.Null(validNullFunscriptMetadataObj.Description);
        Assert.Null(validNullFunscriptMetadataObj.Duration);
        Assert.Null(validNullFunscriptMetadataObj.DurationTime);
        Assert.Null(validNullFunscriptMetadataObj.Type);
        Assert.Null(validNullFunscriptMetadataObj.Notes);
        Assert.Null(validNullFunscriptMetadataObj.License);
        Assert.Null(validNullFunscriptMetadataObj.ScriptUrl);
        Assert.Null(validNullFunscriptMetadataObj.TopicUrl);
        Assert.Null(validNullFunscriptMetadataObj.VideoUrl);
        Assert.Null(validNullFunscriptMetadataObj.Tags);
        Assert.Null(validNullFunscriptMetadataObj.Performers);
        Assert.Null(validNullFunscriptMetadataObj.Chapters);
        Assert.Null(validNullFunscriptMetadataObj.Bookmarks);
        Assert.Null(validNullFunscriptMetadataObj.UnknownFields);
    }
}
