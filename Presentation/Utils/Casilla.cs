using Microsoft.Xna.Framework;

namespace Bastion.Presentation.Utils;

/// <summary>
/// Casilla de verificacion. La del CU-02 es la de aceptacion de los terminos de
/// uso, que el FA-05 manda resaltar cuando se intenta continuar sin marcarla y
/// que el FA-02 desmarca al cambiar de idioma, porque el texto aceptado deja de
/// ser el que se esta viendo. Aqui solo se marca y se desmarca.
/// </summary>
public sealed class Casilla : Control
{
    private const int Lado = 22;

    public bool Marcada { get; set; }

    /// <summary>Texto a la derecha de la casilla.</summary>
    public string Texto { get; set; } = string.Empty;

    /// <summary>
    /// Parte final del texto que se pinta en naranja, como enlace a los
    /// terminos. Debe ser el final literal de <see cref="Texto"/>.
    /// </summary>
    public string TextoEnlace { get; set; } = string.Empty;

    public override void Actualizar(Entrada entrada)
    {
        base.Actualizar(entrada);

        if (Encima && entrada.Click)
        {
            Marcada = !Marcada;
        }
    }

    public override void Dibujar(Lienzo lienzo)
    {
        if (!Visible)
        {
            return;
        }

        var lote = lienzo.Lote;
        var caja = new Rectangle(Limites.X, Limites.Y + (Limites.Height - Lado) / 2, Lado, Lado);

        if (Marcada)
        {
            lienzo.Formas.RectanguloRedondo(lote, caja, Tema.RadioCasilla, Tema.Acento);
            DibujarPalomita(lienzo, caja);
        }
        else
        {
            lienzo.Formas.RectanguloRedondo(lote, caja, Tema.RadioCasilla, Tema.Tarjeta);
            lienzo.Formas.BordeRedondo(
                lote, caja, Tema.RadioCasilla, 2, TieneAviso ? Tema.Acento : Tema.BordeCasilla);
        }

        float x = caja.Right + 12;
        float y = Limites.Y + (Limites.Height - DibujoTexto.Alto(lienzo.Regular, Tema.EscalaCuerpo)) / 2f;

        string normal = string.IsNullOrEmpty(TextoEnlace) || !Texto.EndsWith(TextoEnlace, StringComparison.Ordinal)
            ? Texto
            : Texto[..^TextoEnlace.Length];

        DibujoTexto.Dibujar(lote, lienzo.Regular, normal, new Vector2(x, MathF.Round(y)), Tema.Etiqueta, Tema.EscalaCuerpo);

        if (normal.Length != Texto.Length)
        {
            float desplazamiento = DibujoTexto.Ancho(lienzo.Regular, normal, Tema.EscalaCuerpo);
            DibujoTexto.Dibujar(
                lote, lienzo.Negrita, TextoEnlace, new Vector2(x + desplazamiento, MathF.Round(y)),
                Tema.Acento, Tema.EscalaCuerpo);
        }

        if (TieneAviso)
        {
            DibujoTexto.Dibujar(
                lote, lienzo.Regular, Aviso!, new Vector2(x, Limites.Bottom + 2), Tema.Acento, Tema.EscalaMenuda);
        }
    }

    private static void DibujarPalomita(Lienzo lienzo, Rectangle caja)
    {
        var lote = lienzo.Lote;
        var pixel = lienzo.Formas.Pixel;
        var color = Tema.TextoClaro;

        int x = caja.X + 5;
        int y = caja.Y + 11;

        for (int i = 0; i < 4; i++)
        {
            lote.Draw(pixel, new Rectangle(x + i, y + i, 2, 2), color);
        }

        for (int i = 0; i < 7; i++)
        {
            lote.Draw(pixel, new Rectangle(x + 3 + i, y + 3 - i, 2, 2), color);
        }
    }
}
