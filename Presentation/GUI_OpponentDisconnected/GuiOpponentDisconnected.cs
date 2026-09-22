using System;
using Microsoft.Xna.Framework;
using Bastion.Presentation.GUI_MatchEnd;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_OpponentDisconnected;

// CU-26 main flow, and CU-24 FA-06. The use case draws this over the board
// (RN-03); the navigator has no concept yet of a screen over another, so it
// is a full screen, reached today only from GUI_Menu.
public sealed class GuiOpponentDisconnected : FormScreen
{
    private const float ClaimDelaySeconds = 60f;
    private const int ElapsedHeight = 22;
    private const int HintTop = ElapsedHeight + 14;
    private const int HintHeight = 40;
    private const int CardHeight = Theme.CardPadding + HintTop + HintHeight + Theme.CardPadding;

    private readonly TextLine _elapsed;
    private readonly TextBlock _hint;
    private readonly Button _claimButton;
    private readonly Button _backButton;
    private float _startedAt = -1f;

    public GuiOpponentDisconnected(INavigator navigator)
        : base(navigator, NarrowCardWidth, CardHeight)
    {
        int top = Card.Y + Theme.CardPadding;

        _elapsed = new TextLine
        {
            Style = TextLineStyle.Heading,
            IsCentered = true,
            Bounds = new Rectangle(ContentX, top, ContentWidth, ElapsedHeight)
        };
        _hint = new TextBlock
        {
            IsSmall = true,
            Bounds = new Rectangle(ContentX, top + HintTop, ContentWidth, HintHeight)
        };

        _claimButton = CreatePrimaryButton(false);
        _backButton = CreateSecondaryButton();
        _claimButton.IsEnabled = false;
        _claimButton.Clicked += OnClaimClicked;
        _backButton.Clicked += OnBackClicked;

        Register(_elapsed);
        Register(_hint);
        Register(_claimButton);
        Register(_backButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.OpponentDisconnectedTitle;
    }

    public override void Update(InputState input)
    {
        base.Update(input);

        if (_startedAt < 0f)
        {
            _startedAt = input.ElapsedSeconds;
        }

        float elapsed = input.ElapsedSeconds - _startedAt;
        _elapsed.Text = string.Format(
            TextCatalog.OpponentDisconnectedElapsedFormat, TimeSpan.FromSeconds(elapsed).ToString(@"m\:ss"));
        _claimButton.IsEnabled = elapsed >= ClaimDelaySeconds;
    }

    private void OnClaimClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.MatchEnd, GuiMatchEnd.VictoryOutcome);
    }

    private void OnBackClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    protected override void ApplyTexts()
    {
        _hint.Text = TextCatalog.OpponentDisconnectedHint;
        _claimButton.Title = TextCatalog.OpponentDisconnectedClaimButton;
        _backButton.Title = TextCatalog.OpponentDisconnectedBackButton;
    }
}
