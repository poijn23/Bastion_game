using System;

namespace Bastion.Presentation.Utils;

public sealed class Rule : Control
{
    public override void Draw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        if (IsVisible)
        {
            Dashes.DrawHorizontal(canvas, Bounds.X, Bounds.Y, Bounds.Width, Theme.CheckBoxBorder);
        }
    }
}
