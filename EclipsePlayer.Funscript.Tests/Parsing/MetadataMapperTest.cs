// SPDX-FileCopyrightText: 2026 Marty Mooney
// SPDX-License-Identifier: MPL-2.0

using System.Text.Json;
using EclipsePlayer.Funscript.Parsing.Dto;

namespace EclipsePlayer.Funscript.Tests.Parsing;

public class MetadataMapperTest
{
    [Fact]
    public void TestMetadataMapperValid()
    {
        var fileOnDisk = Path.Combine(AppContext.BaseDirectory, "TestData", "valid", "flat-1.0-full.funscript");
        var rawData = File.ReadAllText(fileOnDisk);
        using var doc = JsonDocument.Parse(rawData);
        var metadataJson = doc.RootElement.GetProperty("metadata");
        var dto = metadataJson.Deserialize<FunscriptMetadataDto>();
        Assert.NotNull(dto);
        var validFunscriptMetadataObj = MetadataMapper.ToDomain(dto);

        Assert.Equal("synthetic", validFunscriptMetadataObj.Creator);
        Assert.Equal("full flat sample", validFunscriptMetadataObj.Title);
        Assert.Equal(5400.0, validFunscriptMetadataObj.Duration);


    }
}
