using System;
using Microsoft.Xna.Framework;

namespace Bastion.Presentation.Utils;

public sealed class TextLine : Control
{
    public string Text { get; set; } = string.Empty;

    public TextLineStyle Style { get; init; } = TextLineStyle.Body;

    public bool IsRightAligned { get; init; }

    public override void Draw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        if (!IsVisible || string.IsNullOrEmpty(Text))
        {
            return;
        }

        TextStyle style = Style switch
        {
            TextLineStyle.Heading => TextStyleFactory.CreateHeading(canvas.Fonts, Theme.TextDark),
            TextLineStyle.Small => TextStyleFactory.CreateSmall(canvas.Fonts, Theme.Placeholder),
            TextLineStyle.Label => TextStyleFactory.CreateLabel(canvas.Fonts, Theme.Label),
            TextLineStyle.Muted => TextStyleFactory.CreateBody(canvas.Fonts, Theme.Placeholder),
            _ => TextStyleFactory.CreateBody(canvas.Fonts, Theme.TextDark)
        };

        float x = IsRightAligned ? Bounds.Right - canvas.Text.Measure(Text, style) : Bounds.X;
        canvas.Text.Draw(Text, new Vector2(MathF.Round(x), Bounds.Y), style);
    }
}
