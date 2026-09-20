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

    public static string RegisterFirstNameLabel => GetText(nameof(RegisterFirstNameLabel));

    public static string RegisterFirstNamePlaceholder => GetText(nameof(RegisterFirstNamePlaceholder));

    public static string RegisterLastNameLabel => GetText(nameof(RegisterLastNameLabel));

    public static string RegisterLastNamePlaceholder => GetText(nameof(RegisterLastNamePlaceholder));

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

    public static string RegisterTermsText => GetText(nameof(RegisterTermsText));

    public static string RegisterTermsLinkText => GetText(nameof(RegisterTermsLinkText));

    public static string RegisterCreateButton => GetText(nameof(RegisterCreateButton));

    public static string RegisterCreateButtonDetail => GetText(nameof(RegisterCreateButtonDetail));

    public static string RegisterCancelButton => GetText(nameof(RegisterCancelButton));

    public static string DialogErrorTitle => GetText(nameof(DialogErrorTitle));

    public static string DialogWarningTitle => GetText(nameof(DialogWarningTitle));

    public static string DialogConfirmTitle => GetText(nameof(DialogConfirmTitle));

    public static string DialogSuccessTitle => GetText(nameof(DialogSuccessTitle));

    public static string DialogAcceptButton => GetText(nameof(DialogAcceptButton));

    public static string DialogRetryButton => GetText(nameof(DialogRetryButton));

    public static string DialogConfirmButton => GetText(nameof(DialogConfirmButton));

    public static string DialogCancelButton => GetText(nameof(DialogCancelButton));

    public static string DialogSignInButton => GetText(nameof(DialogSignInButton));

    public static string CommonSaveButton => GetText(nameof(CommonSaveButton));

    public static string CommonCancelButton => GetText(nameof(CommonCancelButton));

    public static string CommonBackButton => GetText(nameof(CommonBackButton));

    public static string LoginSubtitle => GetText(nameof(LoginSubtitle));

    public static string LoginIdentifierLabel => GetText(nameof(LoginIdentifierLabel));

    public static string LoginIdentifierPlaceholder => GetText(nameof(LoginIdentifierPlaceholder));

    public static string LoginPasswordLabel => GetText(nameof(LoginPasswordLabel));

    public static string LoginPasswordPlaceholder => GetText(nameof(LoginPasswordPlaceholder));

    public static string LoginForgotLink => GetText(nameof(LoginForgotLink));

    public static string LoginSignInButton => GetText(nameof(LoginSignInButton));

    public static string LoginSignInDetail => GetText(nameof(LoginSignInDetail));

    public static string LoginCreateAccountButton => GetText(nameof(LoginCreateAccountButton));

    public static string LoginGuestButton => GetText(nameof(LoginGuestButton));

    public static string LoginIdentifierRequired => GetText(nameof(LoginIdentifierRequired));

    public static string LoginPasswordRequired => GetText(nameof(LoginPasswordRequired));

    public static string LoginCredentialsRejected => GetText(nameof(LoginCredentialsRejected));

    public static string ForgotPasswordSubtitle => GetText(nameof(ForgotPasswordSubtitle));

    public static string ForgotPasswordNotice => GetText(nameof(ForgotPasswordNotice));

    public static string ForgotPasswordEmailLabel => GetText(nameof(ForgotPasswordEmailLabel));

    public static string ForgotPasswordEmailPlaceholder => GetText(nameof(ForgotPasswordEmailPlaceholder));

    public static string ForgotPasswordSendButton => GetText(nameof(ForgotPasswordSendButton));

    public static string ResetPasswordSubtitle => GetText(nameof(ResetPasswordSubtitle));

    public static string ResetPasswordCodeLabel => GetText(nameof(ResetPasswordCodeLabel));

    public static string ResetPasswordCodePlaceholder => GetText(nameof(ResetPasswordCodePlaceholder));

    public static string ResetPasswordNewLabel => GetText(nameof(ResetPasswordNewLabel));

    public static string ResetPasswordNewPlaceholder => GetText(nameof(ResetPasswordNewPlaceholder));

    public static string ResetPasswordConfirmLabel => GetText(nameof(ResetPasswordConfirmLabel));

    public static string ResetPasswordConfirmPlaceholder => GetText(nameof(ResetPasswordConfirmPlaceholder));

    public static string ChangePasswordSubtitle => GetText(nameof(ChangePasswordSubtitle));

    public static string ChangePasswordCurrentLabel => GetText(nameof(ChangePasswordCurrentLabel));

    public static string ChangePasswordCurrentPlaceholder => GetText(nameof(ChangePasswordCurrentPlaceholder));

    public static string ChangePasswordNewLabel => GetText(nameof(ChangePasswordNewLabel));

    public static string ChangePasswordNewPlaceholder => GetText(nameof(ChangePasswordNewPlaceholder));

    public static string ChangePasswordConfirmLabel => GetText(nameof(ChangePasswordConfirmLabel));

    public static string ChangePasswordConfirmPlaceholder => GetText(nameof(ChangePasswordConfirmPlaceholder));

    public static string ChangeEmailSubtitle => GetText(nameof(ChangeEmailSubtitle));

    public static string ChangeEmailNewLabel => GetText(nameof(ChangeEmailNewLabel));

    public static string ChangeEmailNewPlaceholder => GetText(nameof(ChangeEmailNewPlaceholder));

    public static string ChangeEmailPasswordLabel => GetText(nameof(ChangeEmailPasswordLabel));

    public static string ChangeEmailPasswordPlaceholder => GetText(nameof(ChangeEmailPasswordPlaceholder));

    public static string ChangeEmailNotice => GetText(nameof(ChangeEmailNotice));

    public static string ChangeNicknameSubtitle => GetText(nameof(ChangeNicknameSubtitle));

    public static string ChangeNicknameNotice => GetText(nameof(ChangeNicknameNotice));

    public static string ChangeNicknameCurrentLabel => GetText(nameof(ChangeNicknameCurrentLabel));

    public static string ChangeNicknameNewLabel => GetText(nameof(ChangeNicknameNewLabel));

    public static string ChangeNicknameNewPlaceholder => GetText(nameof(ChangeNicknameNewPlaceholder));

    public static string ChangeNicknameSaveButton => GetText(nameof(ChangeNicknameSaveButton));

    public static string DeleteAccountSubtitle => GetText(nameof(DeleteAccountSubtitle));

    public static string DeleteAccountNotice => GetText(nameof(DeleteAccountNotice));

    public static string DeleteAccountPasswordLabel => GetText(nameof(DeleteAccountPasswordLabel));

    public static string DeleteAccountPasswordPlaceholder => GetText(nameof(DeleteAccountPasswordPlaceholder));

    public static string DeleteAccountContinueButton => GetText(nameof(DeleteAccountContinueButton));

    public static string DeleteAccountConfirmSubtitle => GetText(nameof(DeleteAccountConfirmSubtitle));

    public static string DeleteAccountConfirmNotice => GetText(nameof(DeleteAccountConfirmNotice));

    public static string DeleteAccountWordLabel => GetText(nameof(DeleteAccountWordLabel));

    public static string DeleteAccountWordPlaceholder => GetText(nameof(DeleteAccountWordPlaceholder));

    public static string DeleteAccountDeleteButton => GetText(nameof(DeleteAccountDeleteButton));

    public static string ActiveSessionsSubtitle => GetText(nameof(ActiveSessionsSubtitle));

    public static string ActiveSessionsCurrentBadge => GetText(nameof(ActiveSessionsCurrentBadge));

    public static string ActiveSessionsCloseButton => GetText(nameof(ActiveSessionsCloseButton));

    public static string ActiveSessionsCloseAllButton => GetText(nameof(ActiveSessionsCloseAllButton));

    public static string ActiveSessionsDeviceSample => GetText(nameof(ActiveSessionsDeviceSample));

    public static string ActiveSessionsLastUseSample => GetText(nameof(ActiveSessionsLastUseSample));

    public static string AccountSettingsSubtitle => GetText(nameof(AccountSettingsSubtitle));

    public static string AccountSettingsNicknameEntry => GetText(nameof(AccountSettingsNicknameEntry));

    public static string AccountSettingsPasswordEntry => GetText(nameof(AccountSettingsPasswordEntry));

    public static string AccountSettingsEmailEntry => GetText(nameof(AccountSettingsEmailEntry));

    public static string AccountSettingsSessionsEntry => GetText(nameof(AccountSettingsSessionsEntry));

    public static string AccountSettingsDeleteEntry => GetText(nameof(AccountSettingsDeleteEntry));

    public static string RegisterDiscardBody => GetText(nameof(RegisterDiscardBody));

    public static string RegisterDiscardButton => GetText(nameof(RegisterDiscardButton));

    public static string RegisterKeepEditingButton => GetText(nameof(RegisterKeepEditingButton));

    public static string RegisterFirstNameRequired => GetText(nameof(RegisterFirstNameRequired));

    public static string RegisterLastNameRequired => GetText(nameof(RegisterLastNameRequired));

    public static string RegisterNicknameLength => GetText(nameof(RegisterNicknameLength));

    public static string RegisterNicknameTaken => GetText(nameof(RegisterNicknameTaken));

    public static string RegisterEmailInvalid => GetText(nameof(RegisterEmailInvalid));

    public static string RegisterEmailTaken => GetText(nameof(RegisterEmailTaken));

    public static string RegisterPasswordTooShort => GetText(nameof(RegisterPasswordTooShort));

    public static string RegisterConfirmationMismatch => GetText(nameof(RegisterConfirmationMismatch));

    public static string RegisterBirthDateInvalid => GetText(nameof(RegisterBirthDateInvalid));

    public static string RegisterBirthDateFuture => GetText(nameof(RegisterBirthDateFuture));

    public static string RegisterTermsRequired => GetText(nameof(RegisterTermsRequired));

    public static string RegisterSuccessBody => GetText(nameof(RegisterSuccessBody));

    // Returning the key when it is missing keeps a typo visible on screen
    // instead of throwing while drawing a frame.
    private static string GetText(string key)
    {
        return _manager.GetString(key, CultureInfo.CurrentUICulture) ?? key;
    }
}
