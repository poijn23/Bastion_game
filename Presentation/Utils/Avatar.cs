using System;
using Microsoft.Xna.Framework;

namespace Bastion.Presentation.Utils;

public sealed class Avatar : Control
{
    public string Text { get; set; } = string.Empty;

    public event EventHandler? Clicked;

    public override void Update(InputState input)
    {
        base.Update(input);

        if (IsEnabled && IsHovered && input.HasClicked)
        {
            Clicked?.Invoke(this, EventArgs.Empty);
        }
    }

    public override void Draw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        if (!IsVisible)
        {
            return;
        }

        int radius = Math.Min(Bounds.Width, Bounds.Height) / 2;
        canvas.Shapes.DrawRoundedRectangle(Bounds, radius, Theme.Field);
        canvas.Shapes.DrawRoundedBorder(Bounds, BorderStyleFactory.CreateThick(radius, Theme.TextDark));

        TextStyle style = Bounds.Width >= 64
            ? TextStyleFactory.CreateBody(canvas.Fonts, Theme.Placeholder)
            : TextStyleFactory.CreateSmall(canvas.Fonts, Theme.Placeholder);
        canvas.Text.DrawCentered(Text, Bounds, style);
    }
}
