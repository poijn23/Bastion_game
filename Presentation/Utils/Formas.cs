using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bastion.Presentation.Utils;

/// <summary>
/// Primitivas de dibujo que la GUI necesita y que MonoGame no trae: rectangulos
/// redondeados rellenos y sus bordes, con los cantos suavizados.
///
/// Cada textura se genera una sola vez a partir de su tamano, su radio y su
/// grosor de borde, y queda en cache: la pantalla reutiliza pocas medidas
/// distintas, asi que el costo se paga en el primer cuadro y no vuelve.
/// </summary>
public sealed class Formas : IDisposable
{
    private readonly GraphicsDevice _dispositivo;
    private readonly Dictionary<(int, int, int, int), Texture2D> _cache = new();

    /// <summary>Textura de un pixel blanco, base de los rectangulos rectos.</summary>
    public Texture2D Pixel { get; }

    public Formas(GraphicsDevice dispositivo)
    {
        _dispositivo = dispositivo;
        Pixel = new Texture2D(dispositivo, 1, 1);
        Pixel.SetData(new[] { Color.White });
    }

    public void Rectangulo(SpriteBatch lote, Rectangle area, Color color)
        => lote.Draw(Pixel, area, color);

    public void RectanguloRedondo(SpriteBatch lote, Rectangle area, int radio, Color color)
        => lote.Draw(Obtener(area.Width, area.Height, radio, 0), new Vector2(area.X, area.Y), color);

    public void BordeRedondo(SpriteBatch lote, Rectangle area, int radio, int grosor, Color color)
        => lote.Draw(Obtener(area.Width, area.Height, radio, grosor), new Vector2(area.X, area.Y), color);

    /// <summary>
    /// Devuelve la textura de un rectangulo redondeado. Con <paramref name="grosor"/>
    /// en cero es solido; con un grosor mayor es solo el anillo del borde, dibujado
    /// hacia dentro del area.
    /// </summary>
    private Texture2D Obtener(int ancho, int alto, int radio, int grosor)
    {
        var llave = (ancho, alto, radio, grosor);
        if (_cache.TryGetValue(llave, out var guardada))
        {
            return guardada;
        }

        var textura = new Texture2D(_dispositivo, ancho, alto);
        var pixeles = new Color[ancho * alto];

        float mitadAncho = ancho / 2f;
        float mitadAlto = alto / 2f;
        float r = MathHelper.Clamp(radio, 0, MathF.Min(mitadAncho, mitadAlto));

        for (int y = 0; y < alto; y++)
        {
            for (int x = 0; x < ancho; x++)
            {
                // Distancia con signo al borde del rectangulo redondeado: negativa
                // dentro de la figura, positiva fuera. Es lo que permite suavizar
                // el canto sin dientes de sierra.
                float px = MathF.Abs(x + 0.5f - mitadAncho) - mitadAncho + r;
                float py = MathF.Abs(y + 0.5f - mitadAlto) - mitadAlto + r;
                float fuera = MathF.Sqrt(MathF.Max(px, 0) * MathF.Max(px, 0) +
                                         MathF.Max(py, 0) * MathF.Max(py, 0));
                float distancia = fuera + MathF.Min(MathF.Max(px, py), 0) - r;

                float alfa = Math.Clamp(0.5f - distancia, 0f, 1f);
                if (grosor > 0)
                {
                    // El interior se resta, de modo que solo quede el anillo.
                    alfa -= Math.Clamp(0.5f - (distancia + grosor), 0f, 1f);
                }

                pixeles[y * ancho + x] = Color.White * alfa;
            }
        }

        textura.SetData(pixeles);
        _cache[llave] = textura;
        return textura;
    }

    public void Dispose()
    {
        foreach (var textura in _cache.Values)
        {
            textura.Dispose();
        }

        _cache.Clear();
        Pixel.Dispose();
    }
}
