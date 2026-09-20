using Microsoft.Xna.Framework;

namespace Bastion.Presentation.Utils;

public enum EstiloBoton
{
    /// <summary>Naranja solido. La accion que el jugador viene a hacer.</summary>
    Primario,

    /// <summary>Oscuro con borde. La salida.</summary>
    Secundario,

    /// <summary>Solo texto en naranja, sin caja. Para los enlaces del formulario.</summary>
    Enlace
}

/// <summary>
/// Boton de la interfaz. Dispara <see cref="Pulsado"/> al soltar el raton
/// encima; que hace esa accion es cosa de la pantalla.
/// </summary>
public sealed class Boton : Control
{
    public string Titulo { get; set; } = string.Empty;

    /// <summary>Segunda linea, mas pequena, como en el boton principal del prototipo.</summary>
    public string Subtitulo { get; set; } = string.Empty;

    public EstiloBoton Estilo { get; init; } = EstiloBoton.Primario;

    /// <summary>Dibuja la flecha a la derecha, como el "iniciar sesion" del prototipo.</summary>
    public bool ConFlecha { get; init; }

    public event Action? Pulsado;

    public override void Actualizar(Entrada entrada)
    {
        base.Actualizar(entrada);

        if (Encima && entrada.Click)
        {
            Pulsado?.Invoke();
        }
    }

    public override void Dibujar(Lienzo lienzo)
    {
        if (!Visible)
        {
            return;
        }

        var lote = lienzo.Lote;

        switch (Estilo)
        {
            case EstiloBoton.Primario:
                lienzo.Formas.RectanguloRedondo(
                    lote, Limites, Tema.RadioBoton, Encima ? Tema.AcentoClaro : Tema.Acento);
                DibujarContenido(lienzo, Tema.TextoClaro, new Color(0xFF, 0xD8, 0xCC));
                break;

            case EstiloBoton.Secundario:
                lienzo.Formas.RectanguloRedondo(
                    lote, Limites, Tema.RadioBoton, Encima ? Tema.BotonSecundarioHover : Tema.BotonSecundario);
                lienzo.Formas.BordeRedondo(lote, Limites, Tema.RadioBoton, 1, Tema.BordeSecundario);
                DibujarContenido(lienzo, Tema.TextoClaro, Tema.TextoTenue);
                break;

            case EstiloBoton.Enlace:
                DibujoTexto.Dibujar(
                    lote,
                    lienzo.Negrita,
                    Titulo,
                    new Vector2(Limites.X, Limites.Y),
                    Encima ? Tema.AcentoClaro : Tema.Acento,
                    Tema.EscalaMenuda,
                    Tema.EspaciadoEtiqueta);
                break;
        }
    }

    private void DibujarContenido(Lienzo lienzo, Color colorTitulo, Color colorSubtitulo)
    {
        var lote = lienzo.Lote;

        if (string.IsNullOrEmpty(Subtitulo))
        {
            DibujoTexto.DibujarCentrado(
                lote, lienzo.Negrita, Titulo, Limites, colorTitulo, Tema.EscalaCuerpo, Tema.EspaciadoEtiqueta);
        }
        else
        {
            float altoTitulo = DibujoTexto.Alto(lienzo.Negrita, Tema.EscalaCuerpo);
            float altoSubtitulo = DibujoTexto.Alto(lienzo.Regular, Tema.EscalaMenuda);
            float y = Limites.Y + (Limites.Height - (altoTitulo + altoSubtitulo + 2)) / 2f;

            DibujoTexto.Dibujar(
                lote, lienzo.Negrita, Titulo, new Vector2(Limites.X + 28, MathF.Round(y)),
                colorTitulo, Tema.EscalaCuerpo, Tema.EspaciadoEtiqueta);

            DibujoTexto.Dibujar(
                lote, lienzo.Regular, Subtitulo, new Vector2(Limites.X + 28, MathF.Round(y + altoTitulo + 2)),
                colorSubtitulo, Tema.EscalaMenuda);
        }

        if (ConFlecha)
        {
            DibujarFlecha(lienzo, new Vector2(Limites.Right - 40, Limites.Center.Y), colorTitulo);
        }
    }

    /// <summary>Flecha hacia la derecha, armada con un trazo y dos diagonales.</summary>
    private static void DibujarFlecha(Lienzo lienzo, Vector2 centro, Color color)
    {
        var lote = lienzo.Lote;
        var pixel = lienzo.Formas.Pixel;

        lienzo.Formas.Rectangulo(lote, new Rectangle((int)centro.X - 9, (int)centro.Y - 1, 18, 2), color);

        for (int i = 0; i < 7; i++)
        {
            lote.Draw(pixel, new Rectangle((int)centro.X + 2 + i, (int)centro.Y - 7 + i, 2, 2), color);
            lote.Draw(pixel, new Rectangle((int)centro.X + 2 + i, (int)centro.Y + 5 - i, 2, 2), color);
        }
    }
}
