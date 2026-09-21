using System;
using Microsoft.Xna.Framework;

namespace Bastion.Presentation.Utils;

public sealed class RadioRow : Control
{
    private const int CircleSize = 26;
    private const int TextGap = 18;

    public string Text { get; set; } = string.Empty;

    public string Tag { get; set; } = string.Empty;

    public bool IsSelected { get; set; }

    public event EventHandler? Chosen;

    public override void Update(InputState input)
    {
        base.Update(input);

        if (IsEnabled && IsHovered && input.HasClicked && !IsSelected)
        {
            Chosen?.Invoke(this, EventArgs.Empty);
        }
    }

    public override void Draw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        if (!IsVisible)
        {
            return;
        }

        var circle = new Rectangle(Bounds.X, Bounds.Y + ((Bounds.Height - CircleSize) / 2), CircleSize, CircleSize);

        if (IsSelected)
        {
            canvas.Shapes.DrawRoundedRectangle(circle, CircleSize / 2, Theme.TextDark);
        }
        else
        {
            canvas.Shapes.DrawRoundedRectangle(circle, CircleSize / 2, Theme.Card);
            canvas.Shapes.DrawRoundedBorder(circle, BorderStyleFactory.CreateThick(CircleSize / 2, Theme.TextDark));
        }

        TextStyle style = TextStyleFactory.CreateBody(canvas.Fonts, Theme.TextDark);
        float y = Bounds.Y + ((Bounds.Height - canvas.Text.GetLineHeight(style)) / 2f);
        canvas.Text.Draw(Text, new Vector2(circle.Right + TextGap, MathF.Round(y)), style);

        if (!string.IsNullOrEmpty(Tag))
        {
            TextStyle tagStyle = TextStyleFactory.CreateSmall(canvas.Fonts, Theme.Placeholder);
            float tagX = Bounds.Right - canvas.Text.Measure(Tag, tagStyle);
            float tagY = Bounds.Y + ((Bounds.Height - canvas.Text.GetLineHeight(tagStyle)) / 2f);
            canvas.Text.Draw(Tag, new Vector2(MathF.Round(tagX), MathF.Round(tagY)), tagStyle);
        }

        Dashes.DrawHorizontal(canvas, Bounds.X, Bounds.Bottom - 2, Bounds.Width, Theme.CheckBoxBorder);
    }
}
