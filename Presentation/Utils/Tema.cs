using Microsoft.Xna.Framework;

namespace Bastion.Presentation.Utils;

/// <summary>
/// Paleta y medidas de la interfaz. Los colores estan tomados directamente del
/// prototipo de alta fidelidad "Bastion Login Screen", para que las pantallas
/// de acceso se vean como una sola y no como dos aproximaciones distintas.
/// Cualquier color nuevo se agrega aqui y no suelto en una pantalla.
/// </summary>
public static class Tema
{
    // --- Fondo y superficies -------------------------------------------------
    public static readonly Color Fondo = new(0x13, 0x11, 0x10);
    public static readonly Color FondoAdorno = new(0x2A, 0x27, 0x24);
    public static readonly Color Tarjeta = new(0xF1, 0xEF, 0xE9);
    public static readonly Color Campo = new(0xE8, 0xE4, 0xDA);
    public static readonly Color CampoEnfocado = new(0xDE, 0xD9, 0xCC);

    // --- Acento --------------------------------------------------------------
    public static readonly Color Acento = new(0xEC, 0x4B, 0x22);
    public static readonly Color AcentoClaro = new(0xEE, 0x61, 0x3D);

    // --- Texto ---------------------------------------------------------------
    public static readonly Color TextoClaro = new(0xF7, 0xF5, 0xF1);
    public static readonly Color TextoTenue = new(0x8F, 0x88, 0x80);
    public static readonly Color TextoOscuro = new(0x2A, 0x27, 0x23);
    public static readonly Color Etiqueta = new(0x6B, 0x65, 0x5C);
    public static readonly Color Marcador = new(0xA2, 0x9B, 0x8F);

    // --- Botones y bordes ----------------------------------------------------
    public static readonly Color BotonSecundario = new(0x1C, 0x18, 0x15);
    public static readonly Color BotonSecundarioHover = new(0x26, 0x21, 0x1D);
    public static readonly Color BordeSecundario = new(0x37, 0x31, 0x2A);
    public static readonly Color BordeCasilla = new(0xB9, 0xB2, 0xA5);

    // --- Medidas -------------------------------------------------------------
    public const int AnchoVentana = 1280;
    public const int AltoVentana = 720;

    public const int RadioTarjeta = 24;
    public const int RadioCampo = 12;
    public const int RadioBoton = 14;
    public const int RadioCasilla = 6;

    public const int AltoCampo = 46;
    public const int AltoBotonPrimario = 60;
    public const int AltoBotonSecundario = 52;
    public const int RellenoTarjeta = 32;

    // --- Escalas de texto ----------------------------------------------------
    // La fuente Regular y la Negrita se generan a 15 px y la de titulo a 40 px;
    // estas escalas salen de ahi.
    public const float EscalaTitulo = 1.0f;
    public const float EscalaCuerpo = 1.0f;
    public const float EscalaEtiqueta = 0.8f;
    public const float EscalaMenuda = 0.75f;

    public const float EspaciadoTitulo = 6f;
    public const float EspaciadoEtiqueta = 1.4f;
}
