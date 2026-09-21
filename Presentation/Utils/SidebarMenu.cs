using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Bastion.Presentation.Utils;

public sealed class SidebarMenu : Control
{
    public const int ItemHeight = 48;
    private const int ItemGap = 4;
    private const int TextInset = 18;

    public List<SidebarItem> Items { get; } = [];

    public int SelectedIndex { get; set; }

    public event EventHandler<SelectionChangedEventArgs>? ItemChosen;

    public override void Update(InputState input)
    {
        base.Update(input);

        if (!IsEnabled || !IsVisible || !input.HasClicked)
        {
            return;
        }

        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].IsEnabled && GetItemBounds(i).Contains(input.MousePosition) && i != SelectedIndex)
            {
                SelectedIndex = i;
                ItemChosen?.Invoke(this, new SelectionChangedEventArgs { SelectedIndex = i });
                return;
            }
        }
    }

    public override void Draw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        if (!IsVisible)
        {
            return;
        }

        for (int i = 0; i < Items.Count; i++)
        {
            Rectangle area = GetItemBounds(i);
            bool isSelected = i == SelectedIndex;

            if (isSelected)
            {
                canvas.Shapes.DrawRoundedRectangle(area, Theme.FieldCornerRadius, Theme.TextDark);
            }

            Color color = isSelected ? Theme.TextLight : Items[i].IsEnabled ? Theme.Label : Theme.Placeholder;
            TextStyle style = TextStyleFactory.CreateBody(canvas.Fonts, color);
            float y = area.Y + ((area.Height - canvas.Text.GetLineHeight(style)) / 2f);
            canvas.Text.Draw(Items[i].Text, new Vector2(area.X + TextInset, MathF.Round(y)), style);
        }
    }

    private Rectangle GetItemBounds(int index)
    {
        return new Rectangle(Bounds.X, Bounds.Y + (index * (ItemHeight + ItemGap)), Bounds.Width, ItemHeight);
    }
}
