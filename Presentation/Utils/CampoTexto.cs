using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Bastion.Presentation.Utils;

/// <summary>
/// Campo de captura de una sola linea, con su etiqueta encima y su texto guia
/// dentro. Acepta escritura y borrado, que es comportamiento de la propia
/// interfaz; no comprueba formato, longitud minima ni politica de contrasena:
/// esas son las RN-01 a RN-05 del CU-02 y corresponden a la validacion, que
/// aqui no existe.
/// </summary>
public sealed class CampoTexto : Control
{
    private readonly StringBuilder _texto = new();

    /// <summary>Etiqueta en mayusculas que va encima del campo.</summary>
    public string Etiqueta { get; set; } = string.Empty;

    /// <summary>Texto guia que se ve mientras el campo esta vacio.</summary>
    public string Marcador { get; set; } = string.Empty;

    /// <summary>Oculta lo capturado tras vinetas, para las dos contrasenas.</summary>
    public bool EsContrasena { get; init; }

    /// <summary>
    /// Tope de captura. Se toma del esquema: nickname NVARCHAR(30) y correo
    /// NVARCHAR(254). Es un limite del propio campo, no una validacion.
    /// </summary>
    public int LongitudMaxima { get; init; } = 64;

    /// <summary>Alinea el contenido al centro, para los tres campos de la fecha.</summary>
    public bool Centrado { get; init; }

    public string Texto => _texto.ToString();

    public override void Actualizar(Entrada entrada)
    {
        base.Actualizar(entrada);

        if (!Habilitado || !Visible || !Enfocado)
        {
            return;
        }

        foreach (char caracter in entrada.Caracteres)
        {
            if (caracter == '\b')
            {
                if (_texto.Length > 0)
                {
                    _texto.Length--;
                }

                continue;
            }

            // Se ignoran los de control: el salto de linea, el tabulador y el
            // escape los atiende la pantalla, no el campo.
            if (char.IsControl(caracter))
            {
                continue;
            }

            if (_texto.Length < LongitudMaxima)
            {
                _texto.Append(caracter);
            }
        }

        // Suprimir borra igual que retroceso; en un campo de una linea no hay
        // diferencia porque el cursor siempre esta al final.
        if (entrada.TeclaNueva(Keys.Delete) && _texto.Length > 0)
        {
            _texto.Length--;
        }
    }

    public override void Dibujar(Lienzo lienzo)
    {
        if (!Visible)
        {
            return;
        }

        var lote = lienzo.Lote;

        if (!string.IsNullOrEmpty(Etiqueta))
        {
            DibujoTexto.Dibujar(
                lote,
                lienzo.Negrita,
                Etiqueta,
                new Vector2(Limites.X, Limites.Y - 22),
                TieneAviso ? Tema.Acento : Tema.Etiqueta,
                Tema.EscalaEtiqueta,
                Tema.EspaciadoEtiqueta);
        }

        var relleno = Enfocado ? Tema.CampoEnfocado : Tema.Campo;
        lienzo.Formas.RectanguloRedondo(lote, Limites, Tema.RadioCampo, relleno);

        // El foco y el aviso se ven en el borde. FA-03 pide senalar el campo
        // mismo y no juntar todos los reclamos arriba del formulario.
        if (TieneAviso)
        {
            lienzo.Formas.BordeRedondo(lote, Limites, Tema.RadioCampo, 2, Tema.Acento);
        }
        else if (Enfocado)
        {
            lienzo.Formas.BordeRedondo(lote, Limites, Tema.RadioCampo, 2, Tema.Acento * 0.55f);
        }

        string visible = EsContrasena ? new string('•', _texto.Length) : Texto;
        bool vacio = visible.Length == 0;

        // Con el foco puesto, el texto guia estorba: el cursor quedaria encima
        // de la primera letra.
        string mostrado = vacio ? (Enfocado ? string.Empty : Marcador) : visible;
        var color = vacio ? Tema.Marcador : Tema.TextoOscuro;

        var area = new Rectangle(Limites.X + 16, Limites.Y, Limites.Width - 32, Limites.Height);
        float anchoTexto = DibujoTexto.Ancho(lienzo.Regular, mostrado, Tema.EscalaCuerpo);
        float x = Centrado ? area.X + (area.Width - anchoTexto) / 2f : area.X;
        float y = area.Y + (area.Height - DibujoTexto.Alto(lienzo.Regular, Tema.EscalaCuerpo)) / 2f;

        DibujoTexto.Dibujar(lote, lienzo.Regular, mostrado, new Vector2(MathF.Round(x), MathF.Round(y)), color, Tema.EscalaCuerpo);

        // Cursor: parpadea a medio segundo y se apoya al final de lo capturado.
        if (Enfocado && (int)(EntradaSegundos * 2) % 2 == 0)
        {
            float cursorX = vacio ? x : x + anchoTexto + 1;
            var cursor = new Rectangle((int)cursorX, (int)y + 2, 2, (int)DibujoTexto.Alto(lienzo.Regular, Tema.EscalaCuerpo) - 4);
            lienzo.Formas.Rectangulo(lote, cursor, Tema.TextoOscuro);
        }

        if (TieneAviso)
        {
            DibujoTexto.Dibujar(
                lote,
                lienzo.Regular,
                Aviso!,
                new Vector2(Limites.X + 2, Limites.Bottom + 6),
                Tema.Acento,
                Tema.EscalaMenuda);
        }
    }

    /// <summary>
    /// Reloj para el parpadeo del cursor. Lo pone al dia la pantalla en cada
    /// cuadro; se guarda aqui para no arrastrar el GameTime hasta el dibujo.
    /// </summary>
    public static float EntradaSegundos { get; set; }
}
