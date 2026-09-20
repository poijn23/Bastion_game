using System.Globalization;

namespace Bastion.Resources;

/// <summary>
/// Los dos idiomas que el sistema admite y el cambio entre ellos.
///
/// Son exactamente dos, <c>es-MX</c> e <c>en</c>, por la D-21; el esquema los
/// restringe con <c>CK_Usuario_idioma</c> y el CU-02 los vuelve a verificar en
/// el servidor en el paso 9 del flujo normal. Agregar un tercero obliga a
/// tocar tambien esa restriccion y el catalogo de recursos.
/// </summary>
public static class Idioma
{
    public static readonly CultureInfo EspanolMexico = new("es-MX");

    public static readonly CultureInfo Ingles = new("en");

    /// <summary>Idioma con el que arranca el cliente mientras nadie ha elegido otro.</summary>
    public static CultureInfo Predeterminado => Ingles;

    public static CultureInfo Actual => CultureInfo.CurrentUICulture;

    /// <summary>
    /// El idioma vigente es el ingles. Se compara por el codigo de dos letras y
    /// no por el objeto, porque <c>en</c> y <c>en-US</c> son culturas distintas
    /// y las dos deben contar como ingles.
    /// </summary>
    public static bool EsIngles => Actual.TwoLetterISOLanguageName == "en";

    /// <summary>
    /// Cambia el idioma de los textos. Se fija tambien el valor por omision de
    /// los hilos nuevos, para que un hilo creado despues no quede en el idioma
    /// del sistema operativo.
    /// </summary>
    public static void Aplicar(CultureInfo cultura)
    {
        CultureInfo.CurrentUICulture = cultura;
        CultureInfo.DefaultThreadCurrentUICulture = cultura;
    }
}
