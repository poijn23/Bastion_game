using System;
using Microsoft.Xna.Framework;
using Bastion.Resources;

namespace Bastion.Presentation.Utils;

// Background and header shared by every full screen, so the wordmark and the
// ornaments are not laid out again in each one.
public static class ScreenChrome
{
    public const int CardTop = 144;

    private const int TitleY = 52;
    private const int SubtitleY = 112;
    private const float SubtitleTracking = 4f;
    private const int OrnamentArm = 15;
    private const int OrnamentHalf = 7;

    private static readonly Point[] _ornaments =
    [
        new(96, 108), new(1122, 88), new(72, 372), new(1180, 420),
        new(152, 516), new(1060, 664), new(620, 40)
    ];

    public static void Draw(Canvas canvas, string subtitle)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        DrawOrnaments(canvas);
        DrawHeader(canvas, subtitle);
    }

    public static void DrawOrnaments(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        foreach (Point ornament in _ornaments)
        {
            var horizontal = new Rectangle(ornament.X - OrnamentHalf, ornament.Y - 1, OrnamentArm, 2);
            var vertical = new Rectangle(ornament.X - 1, ornament.Y - OrnamentHalf, 2, OrnamentArm);
            canvas.Shapes.DrawRectangle(horizontal, Theme.BackgroundOrnament);
            canvas.Shapes.DrawRectangle(vertical, Theme.BackgroundOrnament);
        }
    }

    private static void DrawHeader(Canvas canvas, string subtitle)
    {
        TextStyle titleStyle = TextStyleFactory.CreateTitle(canvas.Fonts, Theme.TextLight);
        string title = TextCatalog.GameTitle;
        float titleWidth = canvas.Text.Measure(title, titleStyle);
        float titleX = MathF.Round((Theme.WindowWidth - titleWidth) / 2f);
        canvas.Text.Draw(title, new Vector2(titleX, TitleY), titleStyle);

        TextStyle subtitleStyle = TextStyleFactory.CreateLabel(canvas.Fonts, Theme.TextMuted);
        subtitleStyle = subtitleStyle with { Tracking = SubtitleTracking };
        float subtitleWidth = canvas.Text.Measure(subtitle, subtitleStyle);
        float subtitleX = MathF.Round((Theme.WindowWidth - subtitleWidth) / 2f);
        canvas.Text.Draw(subtitle, new Vector2(subtitleX, SubtitleY), subtitleStyle);
    }
}
