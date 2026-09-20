using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Bastion.Presentation.Utils;

// Opens upward on purpose: it sits near the bottom edge, where a list growing
// downward would fall outside the window.
public sealed class DropDown : Control
{
    private const int OptionHeight = 36;
    private const int PanelPadding = 4;
    private const int PanelGap = 6;
    private const int OptionCornerRadius = 8;
    private const int ContentPadding = 14;
    private const int OptionTextPadding = 10;
    private const int ArrowMargin = 18;
    private const int ArrowRows = 5;

    private Point _mousePosition;

    public required IReadOnlyList<string> Options { get; set; }

    public int SelectedIndex { get; set; }

    public bool IsOpen { get; private set; }

    public event EventHandler<SelectionChangedEventArgs>? SelectionChanged;

    public override void Update(InputState input)
    {
        base.Update(input);
        _mousePosition = input.MousePosition;

        if (!IsEnabled || !IsVisible || !input.HasClicked)
        {
            return;
        }

        if (IsHovered)
        {
            IsOpen = !IsOpen;
            return;
        }

        if (!IsOpen)
        {
            return;
        }

        SelectOptionAt(input.MousePosition);
    }

    public override void Draw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        if (!IsVisible)
        {
            return;
        }

        if (IsOpen)
        {
            DrawPanel(canvas);
        }

        DrawClosedBox(canvas);
    }

    private void OnSelectionChanged(int selectedIndex)
    {
        SelectionChanged?.Invoke(this, new SelectionChangedEventArgs { SelectedIndex = selectedIndex });
    }

    private void SelectOptionAt(Point position)
    {
        for (int i = 0; i < Options.Count; i++)
        {
            if (!GetOption(i).Contains(position))
            {
                continue;
            }

            IsOpen = false;

            if (i != SelectedIndex)
            {
                SelectedIndex = i;
                OnSelectionChanged(i);
            }

            return;
        }

        // A click outside the control and outside the list closes it.
        IsOpen = false;
    }

    private Rectangle GetPanel()
    {
        int height = (Options.Count * OptionHeight) + (PanelPadding * 2);

        return new Rectangle(Bounds.X, Bounds.Y - PanelGap - height, Bounds.Width, height);
    }

    private Rectangle GetOption(int index)
    {
        Rectangle panel = GetPanel();

        return new Rectangle(
            panel.X + PanelPadding,
            panel.Y + PanelPadding + (index * OptionHeight),
            panel.Width - (PanelPadding * 2),
            OptionHeight);
    }

    private void DrawClosedBox(Canvas canvas)
    {
        Color fill = IsHovered || IsOpen ? Theme.SecondaryButtonHovered : Theme.SecondaryButton;
        canvas.Shapes.DrawRoundedRectangle(Bounds, Theme.FieldCornerRadius, fill);
        canvas.Shapes.DrawRoundedBorder(
            Bounds,
            BorderStyleFactory.CreateHairline(Theme.FieldCornerRadius, Theme.SecondaryBorder));

        TextStyle style = TextStyleFactory.CreateLabel(canvas.Fonts, Theme.TextLight);
        float y = Bounds.Y + ((Bounds.Height - canvas.Text.GetLineHeight(style)) / 2f);
        canvas.Text.Draw(Options[SelectedIndex], new Vector2(Bounds.X + ContentPadding, MathF.Round(y)), style);

        DrawArrow(canvas, new Point(Bounds.Right - ArrowMargin, Bounds.Center.Y), IsOpen);
    }

    private void DrawPanel(Canvas canvas)
    {
        Rectangle panel = GetPanel();
        canvas.Shapes.DrawRoundedRectangle(panel, Theme.FieldCornerRadius, Theme.SecondaryButton);
        canvas.Shapes.DrawRoundedBorder(
            panel,
            BorderStyleFactory.CreateHairline(Theme.FieldCornerRadius, Theme.SecondaryBorder));

        for (int i = 0; i < Options.Count; i++)
        {
            DrawOption(canvas, i);
        }
    }

    private void DrawOption(Canvas canvas, int index)
    {
        Rectangle area = GetOption(index);

        if (area.Contains(_mousePosition))
        {
            canvas.Shapes.DrawRoundedRectangle(area, OptionCornerRadius, Theme.SecondaryButtonHovered);
        }

        Color color = index == SelectedIndex ? Theme.Accent : Theme.TextLight;
        TextStyle style = TextStyleFactory.CreateLabel(canvas.Fonts, color);
        float y = area.Y + ((area.Height - canvas.Text.GetLineHeight(style)) / 2f);

        canvas.Text.Draw(Options[index], new Vector2(area.X + OptionTextPadding, MathF.Round(y)), style);
    }

    // Five row triangle, pointing down when closed and up when open.
    private static void DrawArrow(Canvas canvas, Point center, bool isPointingUp)
    {
        for (int i = 0; i < ArrowRows; i++)
        {
            int width = 9 - (i * 2);
            int y = isPointingUp ? center.Y + 2 - i : center.Y - 2 + i;
            canvas.Shapes.DrawRectangle(new Rectangle(center.X - (width / 2), y, width, 1), Theme.TextMuted);
        }
    }
}
