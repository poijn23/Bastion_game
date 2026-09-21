using System;
using Microsoft.Xna.Framework;

namespace Bastion.Presentation.Utils;

public sealed class StrengthMeter : Control
{
    private const int Bars = 3;
    private const int BarGap = 8;

    public int Strength { get; set; }

    public override void Draw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        if (!IsVisible)
        {
            return;
        }

        int width = (Bounds.Width - ((Bars - 1) * BarGap)) / Bars;
        int radius = Bounds.Height / 2;

        for (int i = 0; i < Bars; i++)
        {
            var bar = new Rectangle(Bounds.X + (i * (width + BarGap)), Bounds.Y, width, Bounds.Height);

            if (i < Strength)
            {
                canvas.Shapes.DrawRoundedRectangle(bar, radius, Theme.TextDark);
            }
            else
            {
                canvas.Shapes.DrawRoundedRectangle(bar, radius, Theme.Card);
                canvas.Shapes.DrawRoundedBorder(bar, BorderStyleFactory.CreateThick(radius, Theme.TextDark));
            }
        }
    }
}
