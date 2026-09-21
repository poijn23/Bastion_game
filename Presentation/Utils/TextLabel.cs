using System;
using Microsoft.Xna.Framework;

namespace Bastion.Presentation.Utils;

// Plain text placed by a screen. Screens draw through the control list, so a
// loose string needs a control of its own.
public sealed class TextLabel : Control
{
    public string Text { get; set; } = string.Empty;

    public TextRole Role { get; init; } = TextRole.Body;

    public override void Draw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        if (!IsVisible)
        {
            return;
        }

        TextStyle style = CreateStyle(canvas.Fonts);
        float y = Bounds.Y + ((Bounds.Height - canvas.Text.GetLineHeight(style)) / 2f);
        canvas.Text.Draw(Text, new Vector2(Bounds.X, MathF.Round(y)), style);
    }

    private TextStyle CreateStyle(FontSet fonts)
    {
        if (Role == TextRole.Heading)
        {
            return TextStyleFactory.CreateBoldBody(fonts, Theme.TextDark);
        }

        if (Role == TextRole.Caption)
        {
            return TextStyleFactory.CreateSmall(fonts, Theme.Placeholder);
        }

        return TextStyleFactory.CreateBody(fonts, Theme.TextDark);
    }
}
