using System;
using System.Globalization;
using Bastion.Resources;

namespace Bastion.Presentation.Utils;

public static class TestProfile
{
    public static int Level { get; set; } = 14;

    public static int Experience { get; set; } = 620;

    public static int ExperienceToNextLevel { get; set; } = 1000;

    public static DateTime RegisteredOn { get; set; } = new(2026, 3, 9);

    public static int MatchesPlayed { get; set; } = 312;

    public static double WinRate { get; set; } = 0.61;

    public static int BestStreak { get; set; } = 7;

    public static int TopElo { get; set; } = 1902;

    public static int TitleIndex { get; set; } = 1;

    public static CultureInfo PreferredLanguage { get; set; } = Language.SpanishMexico;

    public static string FirstLink { get; set; } = "https://twitch.tv/prueba";

    public static string SecondLink { get; set; } = string.Empty;

    public static bool AllowsSpectators { get; set; } = true;

    public static (int Matches, double WinRate)[] ModeStats { get; } =
        [(204, 0.63), (48, 0.52), (60, 0.66)];
}
