using System;
using Microsoft.Xna.Framework;

namespace Bastion.Presentation.Utils;

public sealed class DashedBox : Control
{
    private const int Padding = 22;
    private const int TextGap = 4;

    public string Title { get; set; } = string.Empty;

    public string Hint { get; set; } = string.Empty;

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

        canvas.Shapes.DrawRoundedRectangle(Bounds, Theme.FieldCornerRadius, IsHovered ? Theme.Field : Theme.Card);
        Dashes.DrawBorder(canvas, Bounds, Theme.FieldCornerRadius, Theme.CheckBoxBorder);

        TextStyle titleStyle = TextStyleFactory.CreateBoldBody(canvas.Fonts, Theme.TextDark);
        TextStyle hintStyle = TextStyleFactory.CreateSmall(canvas.Fonts, Theme.Placeholder);
        float total = canvas.Text.GetLineHeight(titleStyle) + TextGap + canvas.Text.GetLineHeight(hintStyle);
        float y = Bounds.Y + ((Bounds.Height - total) / 2f);
        canvas.Text.Draw(Title, new Vector2(Bounds.X + Padding, MathF.Round(y)), titleStyle);
        y += canvas.Text.GetLineHeight(titleStyle) + TextGap;
        canvas.Text.Draw(Hint, new Vector2(Bounds.X + Padding, MathF.Round(y)), hintStyle);
    }
}
