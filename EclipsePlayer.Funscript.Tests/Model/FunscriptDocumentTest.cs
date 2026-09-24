// SPDX-FileCopyrightText: 2026 Marty Mooney
// SPDX-License-Identifier: MPL-2.0

using EclipsePlayer.Funscript.Model;
using System.Collections.Immutable;

namespace EclipsePlayer.Funscript.Tests.Model;

public class FunscriptDocumentTest
{
    [Fact]
    public void TestFunDocWiring()
    {
        var first = new FunscriptAction(0, 0);
        var second = new FunscriptAction(1000, 100);
        var third = new FunscriptAction(1500, 50);
        var fourth = new FunscriptAction(2000, 100);

        var actions = ImmutableArray.Create(first, second, third, fourth);

        var stroke = new AxisScript(AxisName.Stroke, false, 100, actions);
        var metadata = new FunscriptMetadata {Title = "Smoke Test",  Description = "Smoke Test using xUnit", Creator = "Technomooney", Duration = 4000};

        var funDoc = new FunscriptDocument(stroke,ImmutableArray<AxisScript>.Empty, metadata);
        Assert.Equal(4, funDoc.Stroke.Actions.Length);
        Assert.Equal("Technomooney", funDoc.Metadata.Creator);
        Assert.Equal("Smoke Test", funDoc.Metadata.Title);



    }
}
