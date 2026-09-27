using Microsoft.Xna.Framework;

namespace Bastion.Presentation.Utils;

public static class Hairline
{
    private const int Thickness = 1;

    // The rectangle gives the start and the length; the height it carries is
    // ignored, because a hairline is always one pixel tall.
    public static void DrawHorizontal(Canvas canvas, Rectangle line, Color color)
    {
        canvas.Shapes.DrawRectangle(new Rectangle(line.X, line.Y, line.Width, Thickness), color);
    }

    public static void DrawBorder(Canvas canvas, Rectangle bounds, BorderStyle style)
    {
        canvas.Shapes.DrawRoundedBorder(bounds, style);
    }
}
