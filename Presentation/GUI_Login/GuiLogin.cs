using System;
using Microsoft.Xna.Framework;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_Login;

// CU-01 main flow step 1. The identifier accepts either the nickname or the
// email, so it is one field and not two.
public sealed class GuiLogin : FormScreen
{
    private const int CardHeight = 260;
    private const int MaxIdentifierLength = 254;
    private const int LinkGap = 14;
    private const int LinkHeight = 24;
    private const int GuestButtonGap = 26;

    private readonly TextField _identifierField;
    private readonly TextField _passwordField;
    private readonly Button _forgotButton;
    private readonly Button _signInButton;
    private readonly Button _createAccountButton;
    private readonly Button _guestButton;
    private readonly DropDown _languageDropDown;

    public GuiLogin(INavigator navigator)
        : base(navigator, NarrowCardWidth, CardHeight)
    {
        _identifierField = new TextField { MaxLength = MaxIdentifierLength, Bounds = GetRow(0) };
        _passwordField = new TextField { IsPassword = true, Bounds = GetRow(1) };
        _forgotButton = new Button { Style = ButtonStyle.Link, Bounds = GetForgotBounds() };
        _signInButton = CreatePrimaryButton(true);
        _createAccountButton = CreateSecondaryButton();
        _guestButton = new Button { Style = ButtonStyle.Link, Bounds = GetGuestBounds() };
        _languageDropDown = LanguagePicker.Create();

        _signInButton.Clicked += OnSignInClicked;
        _createAccountButton.Clicked += OnCreateAccountClicked;
        _forgotButton.Clicked += OnForgotClicked;
        _guestButton.Clicked += OnGuestClicked;
        _languageDropDown.SelectionChanged += OnLanguageChanged;

        RegisterField(_identifierField);
        RegisterField(_passwordField);
        Register(_forgotButton);
        Register(_signInButton);
        Register(_createAccountButton);
        Register(_guestButton);
        Register(_languageDropDown);

        ApplyTexts();
        FocusFirstField();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.LoginSubtitle;
    }

    private void OnSignInClicked(object? sender, EventArgs e)
    {
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

    private void OnLanguageChanged(object? sender, SelectionChangedEventArgs e)
    {
        LanguagePicker.Apply(e.SelectedIndex);
        ApplyTexts();
    }

    private void ApplyTexts()
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
        _languageDropDown.Options = LanguagePicker.GetNames();
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
