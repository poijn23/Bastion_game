using System.Globalization;
using System.Resources;

namespace Bastion.Resources;

// Written by hand on purpose: the Visual Studio generator does not run under
// dotnet build, and the team works on Rider and VS Code over macOS and Linux.
public static class TextCatalog
{
    private static readonly ResourceManager _manager =
        new("Bastion.Resources.TextCatalog", typeof(TextCatalog).Assembly);

    public static string WindowTitle => GetText(nameof(WindowTitle));

    public static string GameTitle => GetText(nameof(GameTitle));

    public static string InterfaceLanguageLabel => GetText(nameof(InterfaceLanguageLabel));

    public static string SpanishLanguageCode => GetText(nameof(SpanishLanguageCode));

    public static string EnglishLanguageCode => GetText(nameof(EnglishLanguageCode));

    public static string SpanishMexicoLanguageName => GetText(nameof(SpanishMexicoLanguageName));

    public static string EnglishLanguageName => GetText(nameof(EnglishLanguageName));

    public static string RegisterSubtitle => GetText(nameof(RegisterSubtitle));

    public static string RegisterNicknameLabel => GetText(nameof(RegisterNicknameLabel));

    public static string RegisterNicknamePlaceholder => GetText(nameof(RegisterNicknamePlaceholder));

    public static string RegisterEmailLabel => GetText(nameof(RegisterEmailLabel));

    public static string RegisterEmailPlaceholder => GetText(nameof(RegisterEmailPlaceholder));

    public static string RegisterPasswordLabel => GetText(nameof(RegisterPasswordLabel));

    public static string RegisterPasswordPlaceholder => GetText(nameof(RegisterPasswordPlaceholder));

    public static string RegisterConfirmationLabel => GetText(nameof(RegisterConfirmationLabel));

    public static string RegisterConfirmationPlaceholder => GetText(nameof(RegisterConfirmationPlaceholder));

    public static string RegisterBirthDateLabel => GetText(nameof(RegisterBirthDateLabel));

    public static string RegisterDayPlaceholder => GetText(nameof(RegisterDayPlaceholder));

    public static string RegisterMonthPlaceholder => GetText(nameof(RegisterMonthPlaceholder));

    public static string RegisterYearPlaceholder => GetText(nameof(RegisterYearPlaceholder));

    public static string RegisterAccountLanguageLabel => GetText(nameof(RegisterAccountLanguageLabel));

    public static string RegisterTermsText => GetText(nameof(RegisterTermsText));

    public static string RegisterTermsLinkText => GetText(nameof(RegisterTermsLinkText));

    public static string RegisterCreateButton => GetText(nameof(RegisterCreateButton));

    public static string RegisterCreateButtonDetail => GetText(nameof(RegisterCreateButtonDetail));

    public static string RegisterCancelButton => GetText(nameof(RegisterCancelButton));

    // Returning the key when it is missing keeps a typo visible on screen
    // instead of throwing while drawing a frame.
    private static string GetText(string key)
    {
        return _manager.GetString(key, CultureInfo.CurrentUICulture) ?? key;
    }
}
