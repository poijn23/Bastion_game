using System;
using Microsoft.Xna.Framework;

namespace Bastion.Presentation.Utils;

// The experience bar of the level shown in the profile (CU-15).
public sealed class ProgressBar : Control
{
    private const int TrackHeight = 8;

    public float Progress { get; set; }

    public string Caption { get; set; } = string.Empty;

    public override void Draw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        if (!IsVisible)
        {
            return;
        }

        TextStyle style = TextStyleFactory.CreateSmall(canvas.Fonts, Theme.Placeholder);
        canvas.Text.Draw(Caption, new Vector2(Bounds.X, Bounds.Y), style);

        int trackTop = Bounds.Bottom - TrackHeight;
        var track = new Rectangle(Bounds.X, trackTop, Bounds.Width, TrackHeight);
        canvas.Shapes.DrawRoundedRectangle(track, TrackHeight / 2, Theme.Field);

        int filled = (int)(Bounds.Width * Math.Clamp(Progress, 0f, 1f));

        if (filled < TrackHeight)
        {
            return;
        }

        var fill = new Rectangle(Bounds.X, trackTop, filled, TrackHeight);
        canvas.Shapes.DrawRoundedRectangle(fill, TrackHeight / 2, Theme.Accent);
    }
}
