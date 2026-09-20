namespace Bastion.Presentation.Utils;

public sealed class Canvas
{
    public required ShapeRenderer Shapes { get; init; }

    public required TextRenderer Text { get; init; }

    public required FontSet Fonts { get; init; }
}
