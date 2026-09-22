using System;
using System.Collections.Generic;
using System.Globalization;
using Bastion.Resources;

namespace Bastion.Presentation.Utils;

public static class TestProfile
{
    public const int MaxLinks = 4;

    public static int Level { get; set; } = 14;

    public static int Experience { get; set; } = 620;

    public static int ExperienceToNextLevel { get; set; } = 1000;

    public static DateTime RegisteredOn { get; set; } = new(2026, 3, 9);

    public static int MatchesPlayed { get; set; } = 312;

    public static double WinRate { get; set; } = 0.61;

    public static int BestStreak { get; set; } = 7;

    public static int CurrentElo { get; set; } = 1842;

    public static int TopElo { get; set; } = 1902;

    public static int AverageMoves { get; set; } = 34;

    public static int SkinCount { get; set; } = 14;

    public static int Coins { get; set; } = 2350;

    public static int TitleIndex { get; set; } = 1;

    public static int IconIndex { get; set; } = 0;

    // Set once, at GUI_FirstTime (D-13); -1 means the default pawn still applies.
    public static int PawnIndex { get; set; } = -1;

    public static bool HasChangedNickname { get; set; }

    public static int PasswordChangedMonthsAgo { get; set; } = 3;

    public static int ActiveSessionCount { get; set; } = 2;

    public static CultureInfo PreferredLanguage { get; set; } = Language.SpanishMexico;

    public static bool AllowsSpectators { get; set; } = true;

    public static List<(int TypeIndex, string Url)> Links { get; } =
        [(0, "https://twitch.tv/prueba"), (1, "https://youtube.com/@prueba"), (2, "https://discord.gg/prueba")];

    public static (int Matches, double WinRate)[] ModeStats { get; } =
        [(204, 0.63), (48, 0.52), (60, 0.66)];
}
