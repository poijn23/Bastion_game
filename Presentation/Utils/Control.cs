using System;
using Microsoft.Xna.Framework;

namespace Bastion.Presentation.Utils;

public abstract class Control
{
    public Rectangle Bounds { get; init; }

    public bool IsEnabled { get; init; } = true;

    public bool IsVisible { get; init; } = true;

    public bool IsHovered { get; protected set; }

    public bool IsFocused { get; set; }

    // Set by validation, which does not exist yet. CU-02 FA-03, FA-04, FA-05 and
    // FA-07 require highlighting the rejected field and stating what it expects.
    public string? Warning { get; set; }

    public bool HasWarning => !string.IsNullOrEmpty(Warning);

    public virtual void Update(InputState input)
    {
        ArgumentNullException.ThrowIfNull(input);

        IsHovered = IsEnabled && IsVisible && Bounds.Contains(input.MousePosition);
    }

    public abstract void Draw(Canvas canvas);
}
