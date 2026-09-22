using System;
using Microsoft.Xna.Framework;

namespace Bastion.Presentation.Utils;

public abstract class Control
{
    private Rectangle _bounds;

    // Init for the usual case; screens that reposition a control use MoveTo.
    public Rectangle Bounds
    {
        get { return _bounds; }
        init { _bounds = value; }
    }

    public bool IsEnabled { get; set; } = true;

    private bool _isVisible = true;

    // Init for the usual case, plus named operations for the screens that do
    // swap a control in and out, such as the two steps of GUI_DeleteAccount.
    public bool IsVisible
    {
        get { return _isVisible; }
        init { _isVisible = value; }
    }

    public bool IsHovered { get; protected set; }

    public bool IsFocused { get; set; }

    // Set by validation, which does not exist yet. CU-02 FA-03, FA-04, FA-05 and
    // FA-07 require highlighting the rejected field and stating what it expects.
    public string? Warning { get; set; }

    public bool HasWarning => !string.IsNullOrEmpty(Warning);

    public void Show()
    {
        _isVisible = true;
    }

    public void Hide()
    {
        _isVisible = false;
    }

    public void MoveTo(Rectangle bounds)
    {
        _bounds = bounds;
    }

    public virtual void Update(InputState input)
    {
        ArgumentNullException.ThrowIfNull(input);

        IsHovered = IsEnabled && IsVisible && Bounds.Contains(input.MousePosition);
    }

    public abstract void Draw(Canvas canvas);
}
