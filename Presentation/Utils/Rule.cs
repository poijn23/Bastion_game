using System;
using Microsoft.Xna.Framework;

namespace Bastion.Presentation.Utils;

public sealed class Rule : Control
{
    public override void Draw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        if (IsVisible)
        {
            Hairline.DrawHorizontal(
                canvas, new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, 1), Theme.CheckBoxBorder);
        }
    }
}
