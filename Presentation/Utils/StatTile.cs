using System;
using Microsoft.Xna.Framework;

namespace Bastion.Presentation.Utils;

// A single number with its caption, as the profile shows matches, win rate,
// best streak and elo.
public sealed class StatTile : Control
{
    private const int VerticalPadding = 12;
    private const int CaptionGap = 4;

    public string Caption { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public override void Draw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        if (!IsVisible)
        {
            return;
        }

        canvas.Shapes.DrawRoundedRectangle(Bounds, Theme.FieldCornerRadius, Theme.Field);

        TextStyle valueStyle = TextStyleFactory.CreateBoldBody(canvas.Fonts, Theme.TextDark);
        TextStyle captionStyle = TextStyleFactory.CreateSmall(canvas.Fonts, Theme.Placeholder);

        float total = canvas.Text.GetLineHeight(valueStyle) + CaptionGap + canvas.Text.GetLineHeight(captionStyle);
        float y = Bounds.Y + ((Bounds.Height - total) / 2f);

        canvas.Text.DrawCentered(Value, GetLineArea(canvas, y, valueStyle), valueStyle);

        y += canvas.Text.GetLineHeight(valueStyle) + CaptionGap;
        canvas.Text.DrawCentered(Caption, GetLineArea(canvas, y, captionStyle), captionStyle);
    }

    private Rectangle GetLineArea(Canvas canvas, float top, TextStyle style)
    {
        int height = (int)canvas.Text.GetLineHeight(style);

        return new Rectangle(Bounds.X, (int)MathF.Round(top), Bounds.Width, height);
    }
}
