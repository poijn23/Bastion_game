using System.Globalization;
using System.Resources;

namespace Bastion.Resources;

/// <summary>
/// Acceso a los catalogos de texto. La RN-14 del CU-02 exige que todo texto
/// visible salga de un catalogo de recursos; esta clase es la unica puerta a
/// ellos, de modo que ninguna pantalla lleve una cadena escrita adentro.
///
/// Se escribe a mano a proposito. El generador de Visual Studio
/// (<c>ResXFileCodeGenerator</c>) no corre en <c>dotnet build</c>, asi que un
/// archivo <c>.Designer.cs</c> solo se regeneraria en la maquina de quien use
/// Visual Studio, y el equipo trabaja en Rider y en VS Code sobre macOS y
/// Linux. Cada clave se toma con <c>nameof</c> de su propia propiedad, asi que
/// el nombre de la propiedad y el de la clave no pueden separarse.
/// </summary>
public static class Textos
{
    private static readonly ResourceManager Gestor =
        new("Bastion.Resources.Textos", typeof(Textos).Assembly);

    /// <summary>
    /// Devuelve el texto de la cultura vigente. Si la clave falta en los tres
    /// catalogos regresa la clave misma: es preferible verla en pantalla, donde
    /// salta a la vista, que reventar con una excepcion al dibujar.
    /// </summary>
    private static string Obtener(string clave)
        => Gestor.GetString(clave, CultureInfo.CurrentUICulture) ?? clave;

    // --- Generales -----------------------------------------------------------
    public static string TituloVentana => Obtener(nameof(TituloVentana));
    public static string TituloJuego => Obtener(nameof(TituloJuego));
    public static string EtiquetaIdiomaInterfaz => Obtener(nameof(EtiquetaIdiomaInterfaz));
    public static string CodigoIdiomaEspanol => Obtener(nameof(CodigoIdiomaEspanol));
    public static string CodigoIdiomaIngles => Obtener(nameof(CodigoIdiomaIngles));
    public static string IdiomaEspanolMexico => Obtener(nameof(IdiomaEspanolMexico));
    public static string IdiomaIngles => Obtener(nameof(IdiomaIngles));

    // --- GUI_Register (CU-02) ------------------------------------------------
    public static string RegistroSubtitulo => Obtener(nameof(RegistroSubtitulo));
    public static string RegistroEtiquetaNickname => Obtener(nameof(RegistroEtiquetaNickname));
    public static string RegistroMarcadorNickname => Obtener(nameof(RegistroMarcadorNickname));
    public static string RegistroEtiquetaCorreo => Obtener(nameof(RegistroEtiquetaCorreo));
    public static string RegistroMarcadorCorreo => Obtener(nameof(RegistroMarcadorCorreo));
    public static string RegistroEtiquetaContrasena => Obtener(nameof(RegistroEtiquetaContrasena));
    public static string RegistroMarcadorContrasena => Obtener(nameof(RegistroMarcadorContrasena));
    public static string RegistroEtiquetaConfirmacion => Obtener(nameof(RegistroEtiquetaConfirmacion));
    public static string RegistroMarcadorConfirmacion => Obtener(nameof(RegistroMarcadorConfirmacion));
    public static string RegistroEtiquetaFechaNacimiento => Obtener(nameof(RegistroEtiquetaFechaNacimiento));
    public static string RegistroMarcadorDia => Obtener(nameof(RegistroMarcadorDia));
    public static string RegistroMarcadorMes => Obtener(nameof(RegistroMarcadorMes));
    public static string RegistroMarcadorAnio => Obtener(nameof(RegistroMarcadorAnio));
    public static string RegistroEtiquetaIdiomaCuenta => Obtener(nameof(RegistroEtiquetaIdiomaCuenta));
    public static string RegistroCasillaTerminos => Obtener(nameof(RegistroCasillaTerminos));
    public static string RegistroEnlaceTerminos => Obtener(nameof(RegistroEnlaceTerminos));
    public static string RegistroBotonCrear => Obtener(nameof(RegistroBotonCrear));
    public static string RegistroBotonCrearDetalle => Obtener(nameof(RegistroBotonCrearDetalle));
    public static string RegistroBotonCancelar => Obtener(nameof(RegistroBotonCancelar));
}
