using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Bastion.Presentation.Utils;

// Elo over time, with the division threshold marked (CU-15). It plots whatever
// list it is given; the real series arrives from the server.
public sealed class LineChart : Control
{
    private const int Padding = 12;
    private const int DashLength = 6;
    private const int DashGap = 6;
    private const int PointSize = 4;

    public IReadOnlyList<int> Values { get; set; } = [];

    public int Threshold { get; set; }

    public override void Draw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        if (!IsVisible)
        {
            return;
        }

        canvas.Shapes.DrawRoundedRectangle(Bounds, Theme.FieldCornerRadius, Theme.Field);

        if (Values.Count < 2)
        {
            return;
        }

        DrawThreshold(canvas);
        DrawSeries(canvas);
    }

    private int GetLowest()
    {
        int lowest = Math.Min(Values[0], Threshold);

        foreach (int value in Values)
        {
            lowest = Math.Min(lowest, value);
        }

        return lowest;
    }

    private int GetHighest()
    {
        int highest = Math.Max(Values[0], Threshold);

        foreach (int value in Values)
        {
            highest = Math.Max(highest, value);
        }

        return highest;
    }

    private Point GetPoint(int index)
    {
        int lowest = GetLowest();
        int range = Math.Max(GetHighest() - lowest, 1);
        int usableWidth = Bounds.Width - (Padding * 2);
        int usableHeight = Bounds.Height - (Padding * 2);

        int x = Bounds.X + Padding + (index * usableWidth / Math.Max(Values.Count - 1, 1));
        int y = Bounds.Bottom - Padding - ((Values[index] - lowest) * usableHeight / range);

        return new Point(x, y);
    }

    private void DrawThreshold(Canvas canvas)
    {
        int lowest = GetLowest();
        int range = Math.Max(GetHighest() - lowest, 1);
        int usableHeight = Bounds.Height - (Padding * 2);
        int y = Bounds.Bottom - Padding - ((Threshold - lowest) * usableHeight / range);

        for (int x = Bounds.X + Padding; x < Bounds.Right - Padding; x += DashLength + DashGap)
        {
            canvas.Shapes.DrawRectangle(new Rectangle(x, y, DashLength, 1), Theme.Placeholder);
        }
    }

    private void DrawSeries(Canvas canvas)
    {
        for (int i = 1; i < Values.Count; i++)
        {
            canvas.Shapes.DrawLine(GetPoint(i - 1), GetPoint(i), Theme.Accent);
        }

        foreach (Point point in GetPoints())
        {
            var dot = new Rectangle(point.X - (PointSize / 2), point.Y - (PointSize / 2), PointSize, PointSize);
            canvas.Shapes.DrawRoundedRectangle(dot, PointSize / 2, Theme.Accent);
        }
    }

    private IReadOnlyList<Point> GetPoints()
    {
        var points = new List<Point>();

        for (int i = 0; i < Values.Count; i++)
        {
            points.Add(GetPoint(i));
        }

        return points;
    }
}
