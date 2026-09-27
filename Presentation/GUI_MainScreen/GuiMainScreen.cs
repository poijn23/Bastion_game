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
    private readonly Button _indexButton;

    public GuiMainScreen(INavigator navigator)
        : base(navigator, NarrowCardWidth, CardHeight)
    {
        _intro = new TextBlock
        {
            Bounds = new Rectangle(ContentX, Card.Y + Theme.CardPadding, ContentWidth, IntroHeight)
        };

        _signInButton = CreatePrimaryButton(true);
        // The provisional index of every screen. It goes away with the
        // release; until then it is the only way into GUI_Menu.
        _indexButton = CreateSecondaryButton();
        _signInButton.Clicked += OnSignInClicked;
        _indexButton.Clicked += OnIndexClicked;

        Register(_intro);
        Register(_signInButton);
        Register(_indexButton);

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

    private void OnIndexClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.Menu);
    }

    protected override void ApplyTexts()
    {
        _intro.Text = TextCatalog.MainScreenIntro;
        _signInButton.Title = TextCatalog.LoginSignInButton;
        _indexButton.Title = TextCatalog.MainScreenIndexButton;
    }
}
