// SPDX-FileCopyrightText: 2026 Marty Mooney
// SPDX-License-Identifier: MPL-2.0
namespace EclipsePlayer.Funscript.Model;

public readonly record struct AxisName(string Name)
{
    public static AxisName Stroke => new("stroke");
    public static AxisName Surge => new("surge");
    public static AxisName Sway => new("sway");
    public static AxisName Twist => new("twist");
    public static AxisName Roll => new("roll");
    public static AxisName Pitch => new("pitch");
    public static AxisName Suck => new("suck");
    public static AxisName Vib0 => new("vib0");
    public static AxisName Vib1 => new("vib1");
    public static AxisName Vib2 => new("vib2");
    public static AxisName Valve => new("valve");
    public static AxisName Lube => new("lube");

    public static AxisName From(string raw)
    {
        string processed = raw.Trim();
        processed = processed.ToLowerInvariant();
        processed = processed switch
        {
            "l0"=>"stroke",
            "l1"=>"surge",
            "l2"=>"sway",
            "r0"=>"twist",
            "r1"=>"roll",
            "r2"=>"pitch",
            "a1"=>"suck",
            "v0"=>"vib0",
            "v1"=>"vib1",
            "v2"=>"vib2",
            "a0"=>"valve",
            "a2"=>"lube",

            _=>processed,
        };
        return new AxisName(processed);

    }

}

