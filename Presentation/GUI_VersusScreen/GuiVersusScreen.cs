using System;
using Microsoft.Xna.Framework;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_VersusScreen;

// CU-17 main flow step 6. Shown for a moment before the match opens, so both
// players see who they are about to face.
public sealed class GuiVersusScreen : FormScreen
{
    private const int SampleClockMinutes = 10;

    private const int AvatarSize = 72;
    private const int RowTop = 8;
    private const int NameGap = 6;
    private const int NameHeight = 24;
    private const int EloHeight = 20;
    private const int VersusWidth = 60;
    private const int SettingsGap = 20;
    private const int SettingsHeight = 22;

    private const int SettingsTop = RowTop + AvatarSize + NameGap + NameHeight + NameGap + EloHeight + SettingsGap;
    private const int CardHeight = Theme.CardPadding + SettingsTop + SettingsHeight + Theme.CardPadding;

    private readonly Avatar _leftAvatar;
    private readonly TextLine _leftName;
    private readonly TextLine _leftElo;
    private readonly TextLine _versusLabel;
    private readonly Avatar _rightAvatar;
    private readonly TextLine _rightName;
    private readonly TextLine _rightElo;
    private readonly TextLine _settings;
    private readonly Button _startButton;

    public GuiVersusScreen(INavigator navigator)
        : base(navigator, NarrowCardWidth, CardHeight)
    {
        int top = Card.Y + Theme.CardPadding + RowTop;
        int sideWidth = (ContentWidth - VersusWidth) / 2;
        int leftX = ContentX;
        int rightX = ContentX + sideWidth + VersusWidth;
        int nameTop = top + AvatarSize + NameGap;
        int eloTop = nameTop + NameHeight + NameGap;

        _leftAvatar = new Avatar
        {
            Bounds = new Rectangle(leftX + ((sideWidth - AvatarSize) / 2), top, AvatarSize, AvatarSize)
        };
        _leftName = new TextLine
        {
            IsCentered = true,
            Bounds = new Rectangle(leftX, nameTop, sideWidth, NameHeight)
        };
        _leftElo = new TextLine
        {
            Style = TextLineStyle.Small,
            IsCentered = true,
            Bounds = new Rectangle(leftX, eloTop, sideWidth, EloHeight)
        };

        _versusLabel = new TextLine
        {
            Style = TextLineStyle.Heading,
            IsCentered = true,
            Bounds = new Rectangle(leftX + sideWidth, top + ((AvatarSize - NameHeight) / 2), VersusWidth, NameHeight)
        };

        _rightAvatar = new Avatar
        {
            Bounds = new Rectangle(rightX + ((sideWidth - AvatarSize) / 2), top, AvatarSize, AvatarSize)
        };
        _rightName = new TextLine
        {
            IsCentered = true,
            Bounds = new Rectangle(rightX, nameTop, sideWidth, NameHeight)
        };
        _rightElo = new TextLine
        {
            Style = TextLineStyle.Small,
            IsCentered = true,
            Bounds = new Rectangle(rightX, eloTop, sideWidth, EloHeight)
        };

        _settings = new TextLine
        {
            Style = TextLineStyle.Muted,
            IsCentered = true,
            Bounds = new Rectangle(ContentX, Card.Y + Theme.CardPadding + SettingsTop, ContentWidth, SettingsHeight)
        };

        _startButton = CreatePrimaryButton(true);
        _startButton.Clicked += OnStartClicked;

        Register(_leftAvatar);
        Register(_leftName);
        Register(_leftElo);
        Register(_versusLabel);
        Register(_rightAvatar);
        Register(_rightName);
        Register(_rightElo);
        Register(_settings);
        Register(_startButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.VersusScreenSubtitle;
    }

    private void OnStartClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.Match);
    }

    // Placeholder rivals and settings: the pairing comes from the server.
    protected override void ApplyTexts()
    {
        _leftAvatar.Text = TextCatalog.AvatarPlaceholder;
        _leftName.Text = TestAccount.Nickname;
        _leftElo.Text = string.Format(TextCatalog.VersusScreenEloFormat, TestProfile.CurrentElo.ToString("N0"));

        _versusLabel.Text = TextCatalog.VersusScreenVersusLabel;

        _rightAvatar.Text = TextCatalog.AvatarPlaceholder;
        _rightName.Text = TextCatalog.RankingPlayerSample;
        _rightElo.Text = string.Format(TextCatalog.VersusScreenEloFormat, TestProfile.CurrentElo.ToString("N0"));

        _settings.Text = string.Format(
            TextCatalog.VersusScreenSettingsFormat,
            TextCatalog.ModoClasico,
            string.Format(TextCatalog.VersusScreenClockFormat, SampleClockMinutes));
        _startButton.Title = TextCatalog.VersusScreenStartButton;
    }
}
