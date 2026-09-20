using Microsoft.Xna.Framework;

namespace Bastion.Presentation.Utils;

/// <summary>
/// Selector de una opcion entre pocas, dibujado como segmentos contiguos. El
/// CU-02 lo usa dos veces y no son el mismo: uno elige el idioma de la
/// interfaz (FA-02) y otro el <c>idioma_preferido</c> que se guardara en la
/// cuenta. Las opciones admitidas son <c>es-MX</c> y <c>en</c> (D-21), y el
/// esquema las restringe con CK_Usuario_idioma.
/// </summary>
public sealed class Selector : Control
{
    public required IReadOnlyList<string> Opciones { get; set; }

    public string Etiqueta { get; set; } = string.Empty;

    /// <summary>Version reducida, para el selector de idioma de la esquina.</summary>
    public bool Compacto { get; init; }

    /// <summary>Sobre fondo oscuro cambia los colores del segmento en reposo.</summary>
    public bool SobreFondoOscuro { get; init; }

    public int Seleccion { get; set; }

    public event Action<int>? Cambiado;

    private Rectangle Segmento(int indice)
    {
        int ancho = Limites.Width / Opciones.Count;
        int x = Limites.X + indice * ancho;

        // El ultimo segmento absorbe el residuo de la division entera, para que
        // el grupo termine justo donde termina el control.
        int anchoReal = indice == Opciones.Count - 1 ? Limites.Right - x : ancho;
        return new Rectangle(x, Limites.Y, anchoReal, Limites.Height);
    }

    public override void Actualizar(Entrada entrada)
    {
        base.Actualizar(entrada);

        if (!Encima || !entrada.Click)
        {
            return;
        }

        for (int i = 0; i < Opciones.Count; i++)
        {
            if (Segmento(i).Contains(entrada.Raton) && i != Seleccion)
            {
                Seleccion = i;
                Cambiado?.Invoke(i);
                return;
            }
        }
    }

    public override void Dibujar(Lienzo lienzo)
    {
        if (!Visible)
        {
            return;
        }

        var lote = lienzo.Lote;
        int radio = Compacto ? 8 : Tema.RadioCampo;

        if (!string.IsNullOrEmpty(Etiqueta))
        {
            DibujoTexto.Dibujar(
                lote, lienzo.Negrita, Etiqueta, new Vector2(Limites.X, Limites.Y - 22),
                TieneAviso ? Tema.Acento : Tema.Etiqueta, Tema.EscalaEtiqueta, Tema.EspaciadoEtiqueta);
        }

        var fondo = SobreFondoOscuro ? Tema.BotonSecundario : Tema.Campo;
        lienzo.Formas.RectanguloRedondo(lote, Limites, radio, fondo);

        if (SobreFondoOscuro)
        {
            lienzo.Formas.BordeRedondo(lote, Limites, radio, 1, Tema.BordeSecundario);
        }

        for (int i = 0; i < Opciones.Count; i++)
        {
            var area = Segmento(i);
            bool activo = i == Seleccion;

            if (activo)
            {
                var interior = new Rectangle(area.X + 3, area.Y + 3, area.Width - 6, area.Height - 6);
                lienzo.Formas.RectanguloRedondo(lote, interior, radio - 3, Tema.Acento);
            }

            var color = activo
                ? Tema.TextoClaro
                : SobreFondoOscuro ? Tema.TextoTenue : Tema.Etiqueta;

            DibujoTexto.DibujarCentrado(
                lote,
                lienzo.Negrita,
                Opciones[i],
                area,
                color,
                Compacto ? Tema.EscalaMenuda : Tema.EscalaEtiqueta,
                Tema.EspaciadoEtiqueta);
        }
    }
}
