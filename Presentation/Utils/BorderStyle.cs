using Microsoft.Xna.Framework;

namespace Bastion.Presentation.Utils;

public sealed record BorderStyle
{
    public required int CornerRadius { get; init; }

    public required Color Color { get; init; }

    public int Thickness { get; init; } = 1;
}
