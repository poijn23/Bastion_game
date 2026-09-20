using System.Globalization;

namespace Bastion.Resources;

// Exactly two languages are supported by decision D-21. The schema enforces the
// same pair with CK_Usuario_idioma, so adding a third also changes that check.
public static class Language
{
    private const string EnglishCode = "en";
    private const string SpanishMexicoCode = "es-MX";

    public static readonly CultureInfo SpanishMexico = new(SpanishMexicoCode);

    public static readonly CultureInfo English = new(EnglishCode);

    public static CultureInfo Default => English;

    public static CultureInfo Current => CultureInfo.CurrentUICulture;

    // Compared by two letter code because "en" and "en-US" are different
    // cultures and both must count as English.
    public static bool IsEnglish => Current.TwoLetterISOLanguageName == EnglishCode;

    public static void Apply(CultureInfo culture)
    {
        CultureInfo.CurrentUICulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }
}
