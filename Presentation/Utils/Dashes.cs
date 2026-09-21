using Microsoft.Xna.Framework;

namespace Bastion.Presentation.Utils;

public static class Dashes
{
    private const int DashLength = 8;
    private const int DashGap = 6;
    private const int Thickness = 2;

    public static void DrawHorizontal(Canvas canvas, int x, int y, int width, Color color)
    {
        for (int i = 0; i < width; i += DashLength + DashGap)
        {
            int length = System.Math.Min(DashLength, width - i);
            canvas.Shapes.DrawRectangle(new Rectangle(x + i, y, length, Thickness), color);
        }
    }

    public static void DrawVertical(Canvas canvas, int x, int y, int height, Color color)
    {
        for (int i = 0; i < height; i += DashLength + DashGap)
        {
            int length = System.Math.Min(DashLength, height - i);
            canvas.Shapes.DrawRectangle(new Rectangle(x, y + i, Thickness, length), color);
        }
    }

    public static void DrawBorder(Canvas canvas, Rectangle bounds, int cornerRadius, Color color)
    {
        int inset = cornerRadius / 2;
        DrawHorizontal(canvas, bounds.X + inset, bounds.Y, bounds.Width - (inset * 2), color);
        DrawHorizontal(canvas, bounds.X + inset, bounds.Bottom - Thickness, bounds.Width - (inset * 2), color);
        DrawVertical(canvas, bounds.X, bounds.Y + inset, bounds.Height - (inset * 2), color);
        DrawVertical(canvas, bounds.Right - Thickness, bounds.Y + inset, bounds.Height - (inset * 2), color);
    }
}
