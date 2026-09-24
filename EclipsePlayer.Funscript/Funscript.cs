// SPDX-FileCopyrightText: 2026 Marty Mooney
// SPDX-License-Identifier: MPL-2.0

namespace EclipsePlayer.Funscript;

public static class Funscript
{
    internal static TimeSpan ParseTimeSpan(string rawTimeString)
    {
        var parts = rawTimeString.Split(':');

        double seconds = double.Parse(parts[^1]); // ^ in the ^1 is the index-from-end operator.
        int minutes = parts.Length > 1 ? int.Parse(parts[^2]) : 0; // ternary conditional operator saying if the len of the list is 2 or more, take the second to last index and parse as an int otherwise set to 0
        int hours = parts.Length > 2 ? int.Parse(parts[^3]) : 0; // same as above, but len of 3

        double totalSeconds = hours * 3600 + minutes * 60 + seconds;
        return TimeSpan.FromSeconds(totalSeconds);
    }
}
