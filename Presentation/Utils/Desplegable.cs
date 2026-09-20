using Microsoft.Xna.Framework;

namespace Bastion.Presentation.Utils;

/// <summary>
/// Lista desplegable de una sola eleccion. Cerrada muestra la opcion vigente y
/// una flechita; al pulsarla despliega las demas.
///
/// Se despliega hacia arriba a proposito: esta pensada para la esquina inferior
/// de la ventana, donde una lista hacia abajo se saldria de la pantalla.
/// </summary>
public sealed class Desplegable : Control
{
    private const int AltoOpcion = 36;
    private const int RellenoPanel = 4;
    private const int SeparacionPanel = 6;

    /// <summary>Posicion del raton del ultimo cuadro, para resaltar la opcion senalada al dibujar.</summary>
    private Point _raton;

    public required IReadOnlyList<string> Opciones { get; set; }

    public int Seleccion { get; set; }

    public bool Abierto { get; private set; }

    /// <summary>Se dispara solo cuando la eleccion cambia, no al abrir ni al cerrar.</summary>
    public event Action<int>? Cambiado;

    /// <summary>Area que ocupa la lista cuando esta desplegada, encima del control.</summary>
    private Rectangle Panel
    {
        get
        {
            int alto = (Opciones.Count * AltoOpcion) + (RellenoPanel * 2);
            return new Rectangle(Limites.X, Limites.Y - SeparacionPanel - alto, Limites.Width, alto);
        }
    }

    private Rectangle Opcion(int indice)
        => new(Panel.X + RellenoPanel,
               Panel.Y + RellenoPanel + (indice * AltoOpcion),
               Panel.Width - (RellenoPanel * 2),
               AltoOpcion);

    public override void Actualizar(Entrada entrada)
    {
        base.Actualizar(entrada);
        _raton = entrada.Raton;

        if (!Habilitado || !Visible || !entrada.Click)
        {
            return;
        }

        if (Encima)
        {
            Abierto = !Abierto;
            return;
        }

        if (!Abierto)
        {
            return;
        }

        for (int i = 0; i < Opciones.Count; i++)
        {
            if (!Opcion(i).Contains(entrada.Raton))
            {
                continue;
            }

            Abierto = false;

            if (i != Seleccion)
            {
                Seleccion = i;
                Cambiado?.Invoke(i);
            }

            return;
        }

        // Un click fuera del control y fuera de la lista la cierra.
        Abierto = false;
    }

    public override void Dibujar(Lienzo lienzo)
    {
        if (!Visible)
        {
            return;
        }

        var lote = lienzo.Lote;

        if (Abierto)
        {
            DibujarPanel(lienzo);
        }

        lienzo.Formas.RectanguloRedondo(
            lote, Limites, Tema.RadioCampo, Encima || Abierto ? Tema.BotonSecundarioHover : Tema.BotonSecundario);
        lienzo.Formas.BordeRedondo(lote, Limites, Tema.RadioCampo, 1, Tema.BordeSecundario);

        float y = Limites.Y + (Limites.Height - DibujoTexto.Alto(lienzo.Negrita, Tema.EscalaEtiqueta)) / 2f;
        DibujoTexto.Dibujar(
            lote,
            lienzo.Negrita,
            Opciones[Seleccion],
            new Vector2(Limites.X + 14, MathF.Round(y)),
            Tema.TextoClaro,
            Tema.EscalaEtiqueta,
            Tema.EspaciadoEtiqueta);

        DibujarFlechita(lienzo, new Point(Limites.Right - 18, Limites.Center.Y), Abierto, Tema.TextoTenue);
    }

    private void DibujarPanel(Lienzo lienzo)
    {
        var lote = lienzo.Lote;

        lienzo.Formas.RectanguloRedondo(lote, Panel, Tema.RadioCampo, Tema.BotonSecundario);
        lienzo.Formas.BordeRedondo(lote, Panel, Tema.RadioCampo, 1, Tema.BordeSecundario);

        for (int i = 0; i < Opciones.Count; i++)
        {
            var area = Opcion(i);
            bool encima = area.Contains(_raton);

            if (encima)
            {
                lienzo.Formas.RectanguloRedondo(lote, area, 8, Tema.BotonSecundarioHover);
            }

            float y = area.Y + (area.Height - DibujoTexto.Alto(lienzo.Negrita, Tema.EscalaEtiqueta)) / 2f;
            DibujoTexto.Dibujar(
                lote,
                lienzo.Negrita,
                Opciones[i],
                new Vector2(area.X + 10, MathF.Round(y)),
                i == Seleccion ? Tema.Acento : Tema.TextoClaro,
                Tema.EscalaEtiqueta,
                Tema.EspaciadoEtiqueta);
        }
    }

    /// <summary>Triangulo de cinco filas, hacia abajo cerrado y hacia arriba abierto.</summary>
    private static void DibujarFlechita(Lienzo lienzo, Point centro, bool haciaArriba, Color color)
    {
        for (int i = 0; i < 5; i++)
        {
            int ancho = 9 - (i * 2);
            int y = haciaArriba ? centro.Y + 2 - i : centro.Y - 2 + i;
            lienzo.Lote.Draw(lienzo.Formas.Pixel, new Rectangle(centro.X - (ancho / 2), y, ancho, 1), color);
        }
    }
}
