using Microsoft.Xna.Framework;

namespace Bastion.Presentation.Utils;

public static class Hairline
{
    private const int Thickness = 1;

    public static void DrawHorizontal(Canvas canvas, int x, int y, int width, Color color)
    {
        canvas.Shapes.DrawRectangle(new Rectangle(x, y, width, Thickness), color);
    }

    public static void DrawBorder(Canvas canvas, Rectangle bounds, int cornerRadius, Color color)
    {
        canvas.Shapes.DrawRoundedBorder(bounds, BorderStyleFactory.CreateHairline(cornerRadius, color));
    }
}
