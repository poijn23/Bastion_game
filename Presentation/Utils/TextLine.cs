using System;
using Microsoft.Xna.Framework;

namespace Bastion.Presentation.Utils;

public sealed class TextLine : Control
{
    private const float HeadingScale = 1.4f;

    public string Text { get; set; } = string.Empty;

    public bool IsHeading { get; init; }

    public override void Draw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        if (!IsVisible)
        {
            return;
        }

        TextStyle style = IsHeading
            ? new TextStyle { Font = canvas.Fonts.Bold, Color = Theme.TextDark, Scale = HeadingScale }
            : TextStyleFactory.CreateBody(canvas.Fonts, Theme.Label);

        canvas.Text.Draw(Text, new Vector2(Bounds.X, Bounds.Y), style);
    }
}
