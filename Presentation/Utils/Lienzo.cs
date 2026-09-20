using Microsoft.Xna.Framework.Graphics;

namespace Bastion.Presentation.Utils;

/// <summary>
/// Lo que un control necesita para dibujarse: el lote de sprites, las formas y
/// las tres fuentes. Se pasa por parametro en lugar de guardarlo en cada
/// control, para que ninguno se quede con una referencia a un dispositivo que
/// ya se reinicio.
/// </summary>
public sealed class Lienzo(SpriteBatch lote, Formas formas, SpriteFont regular, SpriteFont negrita, SpriteFont titulo)
{
    public SpriteBatch Lote { get; } = lote;
    public Formas Formas { get; } = formas;
    public SpriteFont Regular { get; } = regular;
    public SpriteFont Negrita { get; } = negrita;
    public SpriteFont Titulo { get; } = titulo;
}
