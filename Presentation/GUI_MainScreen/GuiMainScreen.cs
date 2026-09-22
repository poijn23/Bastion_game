using System;
using Microsoft.Xna.Framework;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_MainScreen;

// CU-01 disparador. The title screen the player sees before GUI_Login: the
// wordmark, a one line pitch and the button that opens the real sign in form.
public sealed class GuiMainScreen : FormScreen
{
    private const int IntroHeight = 84;
    private const int CardHeight = Theme.CardPadding + IntroHeight + Theme.CardPadding;

    private readonly TextBlock _intro;
    private readonly Button _signInButton;

    public GuiMainScreen(INavigator navigator)
        : base(navigator, NarrowCardWidth, CardHeight)
    {
        _intro = new TextBlock
        {
            Bounds = new Rectangle(ContentX, Card.Y + Theme.CardPadding, ContentWidth, IntroHeight)
        };

        _signInButton = CreatePrimaryButton(true);
        _signInButton.Clicked += OnSignInClicked;

        Register(_intro);
        Register(_signInButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.MainScreenSubtitle;
    }

    private void OnSignInClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.Login);
    }

    protected override void ApplyTexts()
    {
        _intro.Text = TextCatalog.MainScreenIntro;
        _signInButton.Title = TextCatalog.LoginSignInButton;
    }
}
