using System;
using Microsoft.Xna.Framework;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_Login;

// CU-01 main flow step 1. The identifier accepts either the nickname or the
// email, so it is one field and not two.
public sealed class GuiLogin : FormScreen
{
    private const int CardHeight = 272;
    private const int MaxIdentifierLength = 254;

    // The link sits under a field that validation can reject, so it clears the
    // warning line instead of being drawn on top of it.
    private const int LinkGap = Theme.WarningSpace + 6;
    private const int LinkHeight = 24;
    private const int GuestButtonGap = 26;

    private readonly TextField _identifierField;
    private readonly TextField _passwordField;
    private readonly Button _forgotButton;
    private readonly Button _signInButton;
    private readonly Button _createAccountButton;
    private readonly Button _guestButton;
    private bool _hasValidated;

    public GuiLogin(INavigator navigator)
        : base(navigator, NarrowCardWidth, CardHeight)
    {
        _identifierField = new TextField { MaxLength = MaxIdentifierLength, Bounds = GetRow(0) };
        _passwordField = new TextField { IsPassword = true, Bounds = GetRow(1) };
        _forgotButton = new Button { Style = ButtonStyle.Link, Bounds = GetForgotBounds() };
        _signInButton = CreatePrimaryButton(true);
        _createAccountButton = CreateSecondaryButton();
        _guestButton = new Button { Style = ButtonStyle.Link, Bounds = GetGuestBounds() };

        _signInButton.Clicked += OnSignInClicked;
        _createAccountButton.Clicked += OnCreateAccountClicked;
        _forgotButton.Clicked += OnForgotClicked;
        _guestButton.Clicked += OnGuestClicked;

        RegisterField(_identifierField);
        RegisterField(_passwordField);
        Register(_forgotButton);
        Register(_signInButton);
        Register(_createAccountButton);
        Register(_guestButton);

        ApplyTexts();
        FocusFirstField();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.LoginSubtitle;
    }

    // The connection does not exist yet, so the one test account stands in for
    // the LOGIN answer: a match opens the account hub, anything else is
    // rejected as CU-01 does, with one message for both fields.
    private void OnSignInClicked(object? sender, EventArgs e)
    {
        _hasValidated = true;

        if (!Validate())
        {
            return;
        }

        if (TestAccount.Matches(_identifierField.Text.Trim(), _passwordField.Text))
        {
            Navigator.GoTo(ScreenId.AccountSettings);
            return;
        }

        _passwordField.Warning = TextCatalog.LoginCredentialsRejected;
    }

    private bool Validate()
    {
        _identifierField.Warning = string.IsNullOrWhiteSpace(_identifierField.Text)
            ? TextCatalog.LoginIdentifierRequired
            : null;

        _passwordField.Warning = string.IsNullOrEmpty(_passwordField.Text)
            ? TextCatalog.LoginPasswordRequired
            : null;

        return !_identifierField.HasWarning && !_passwordField.HasWarning;
    }

    private void OnCreateAccountClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.Register);
    }

    private void OnForgotClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.ForgotPassword);
    }

    private void OnGuestClicked(object? sender, EventArgs e)
    {
    }

    protected override void ApplyTexts()
    {
        _identifierField.Label = TextCatalog.LoginIdentifierLabel;
        _identifierField.Placeholder = TextCatalog.LoginIdentifierPlaceholder;
        _passwordField.Label = TextCatalog.LoginPasswordLabel;
        _passwordField.Placeholder = TextCatalog.LoginPasswordPlaceholder;
        _forgotButton.Title = TextCatalog.LoginForgotLink;
        _signInButton.Title = TextCatalog.LoginSignInButton;
        _signInButton.Subtitle = TextCatalog.LoginSignInDetail;
        _createAccountButton.Title = TextCatalog.LoginCreateAccountButton;
        _guestButton.Title = TextCatalog.LoginGuestButton;

        if (_hasValidated)
        {
            Validate();
        }
    }

    private Rectangle GetForgotBounds()
    {
        return new Rectangle(ContentX, GetRow(1).Bottom + LinkGap, ContentWidth, LinkHeight);
    }

    private Rectangle GetGuestBounds()
    {
        return new Rectangle(ContentX, SecondaryButtonBounds.Bottom + GuestButtonGap, ContentWidth, LinkHeight);
    }
}
