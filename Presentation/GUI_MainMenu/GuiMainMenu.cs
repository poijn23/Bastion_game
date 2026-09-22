using System;
using Microsoft.Xna.Framework;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_MainMenu;

// CU-01 main flow step 19. The home hub once a session is open: the player's
// numbers up top, the three ways to start a match, and the rest of the game
// one tap away. Prototype 3.4.
public sealed class GuiMainMenu : FormScreen
{
    private const int HeaderHeight = 24;
    private const int SectionGap = 20;
    private const int TileHeight = 68;
    private const int TileGap = 14;
    private const int TileCount = 3;
    private const int ActionHeight = 68;
    private const int ActionGapRow = 14;
    private const int ActionCount = 3;
    private const int SmallButtonGap = 12;
    private const int SmallButtonCount = 5;
    private const int FooterGap = 12;

    private const int TilesTop = HeaderHeight + 8;
    private const int ActionsTop = TilesTop + TileHeight + SectionGap;
    private const int SmallTop = ActionsTop + ActionHeight + SectionGap;
    private const int FooterTop = SmallTop + Theme.SecondaryButtonHeight + SectionGap;
    private const int CardHeight = Theme.CardPadding + FooterTop + Theme.SecondaryButtonHeight + Theme.CardPadding;

    private readonly TextLine _header;
    private readonly TextLine _levelLine;
    private readonly StatTile _eloTile;
    private readonly StatTile _streakTile;
    private readonly StatTile _coinsTile;
    private readonly Button _findMatchButton;
    private readonly Button _privateMatchButton;
    private readonly Button _versusAiButton;
    private readonly Button _friendsButton;
    private readonly Button _customizeButton;
    private readonly Button _shopButton;
    private readonly Button _rankingButton;
    private readonly Button _historyButton;
    private readonly Button _howToPlayButton;
    private readonly Button _settingsButton;

    public GuiMainMenu(INavigator navigator)
        : base(navigator, WideCardWidth, CardHeight)
    {
        int top = Card.Y + Theme.CardPadding;

        _header = new TextLine
        {
            Style = TextLineStyle.Heading,
            Bounds = new Rectangle(ContentX, top, ContentWidth, HeaderHeight)
        };
        _levelLine = new TextLine
        {
            Style = TextLineStyle.Muted,
            IsRightAligned = true,
            Bounds = new Rectangle(ContentX, top + 4, ContentWidth, HeaderHeight)
        };

        int tileWidth = (ContentWidth - (TileGap * (TileCount - 1))) / TileCount;
        _eloTile = CreateTile(0, tileWidth, top + TilesTop);
        _streakTile = CreateTile(1, tileWidth, top + TilesTop);
        _coinsTile = CreateTile(2, tileWidth, top + TilesTop);

        int actionWidth = (ContentWidth - (ActionGapRow * (ActionCount - 1))) / ActionCount;
        _findMatchButton = CreateActionButton(0, actionWidth, top + ActionsTop);
        _privateMatchButton = CreateActionButton(1, actionWidth, top + ActionsTop);
        _versusAiButton = CreateActionButton(2, actionWidth, top + ActionsTop);
        _findMatchButton.Clicked += OnFindMatchClicked;
        _privateMatchButton.Clicked += OnPrivateMatchClicked;
        _versusAiButton.Clicked += OnVersusAiClicked;

        int smallWidth = (ContentWidth - (SmallButtonGap * (SmallButtonCount - 1))) / SmallButtonCount;
        _friendsButton = CreateSmallButton(0, smallWidth, top + SmallTop);
        _customizeButton = CreateSmallButton(1, smallWidth, top + SmallTop);
        _shopButton = CreateSmallButton(2, smallWidth, top + SmallTop);
        _rankingButton = CreateSmallButton(3, smallWidth, top + SmallTop);
        _historyButton = CreateSmallButton(4, smallWidth, top + SmallTop);
        _friendsButton.Clicked += OnFriendsClicked;
        _customizeButton.Clicked += OnCustomizeClicked;
        _shopButton.Clicked += OnShopClicked;
        _rankingButton.Clicked += OnRankingClicked;
        _historyButton.Clicked += OnHistoryClicked;

        int footerWidth = (ContentWidth - FooterGap) / 2;
        _howToPlayButton = CreateOutlineButton(
            new Rectangle(ContentX, top + FooterTop, footerWidth, Theme.SecondaryButtonHeight));
        int settingsX = ContentX + footerWidth + FooterGap;
        _settingsButton = CreateOutlineButton(
            new Rectangle(settingsX, top + FooterTop, footerWidth, Theme.SecondaryButtonHeight));
        _howToPlayButton.Clicked += OnHowToPlayClicked;
        _settingsButton.Clicked += OnSettingsClicked;

        Register(_header);
        Register(_levelLine);
        Register(_eloTile);
        Register(_streakTile);
        Register(_coinsTile);
        Register(_findMatchButton);
        Register(_privateMatchButton);
        Register(_versusAiButton);
        Register(_friendsButton);
        Register(_customizeButton);
        Register(_shopButton);
        Register(_rankingButton);
        Register(_historyButton);
        Register(_howToPlayButton);
        Register(_settingsButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.MainMenuSubtitle;
    }

    private void OnFindMatchClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.SelectMode);
    }

    private void OnPrivateMatchClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.PrivateMatch);
    }

    private void OnVersusAiClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.AIDifficulty);
    }

    private void OnFriendsClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.Friends);
    }

    private void OnCustomizeClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.Customize);
    }

    private void OnShopClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.Shop);
    }

    private void OnRankingClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.Ranking);
    }

    private void OnHistoryClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.MatchHistory);
    }

    private void OnHowToPlayClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.TutorialIndex);
    }

    private void OnSettingsClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.AccountSettings);
    }

    // Placeholder numbers: the player state comes from the server.
    protected override void ApplyTexts()
    {
        _header.Text = TestAccount.Nickname;
        _levelLine.Text = string.Format(TextCatalog.MainMenuLevelFormat, TestProfile.Level);

        _eloTile.Value = TestProfile.CurrentElo.ToString("N0");
        _eloTile.Caption = TextCatalog.MainMenuEloCaption;
        _streakTile.Value = TestProfile.BestStreak.ToString("N0");
        _streakTile.Caption = TextCatalog.MainMenuStreakCaption;
        _coinsTile.Value = TestProfile.Coins.ToString("N0");
        _coinsTile.Caption = TextCatalog.MainMenuCoinsCaption;

        _findMatchButton.Title = TextCatalog.MainMenuFindMatchButton;
        _findMatchButton.Subtitle = TextCatalog.MainMenuFindMatchDetail;
        _privateMatchButton.Title = TextCatalog.MainMenuPrivateButton;
        _privateMatchButton.Subtitle = TextCatalog.MainMenuPrivateDetail;
        _versusAiButton.Title = TextCatalog.MainMenuVersusAiButton;
        _versusAiButton.Subtitle = TextCatalog.MainMenuVersusAiDetail;

        _friendsButton.Title = TextCatalog.MainMenuFriendsButton;
        _customizeButton.Title = TextCatalog.MainMenuCustomizeButton;
        _shopButton.Title = TextCatalog.MainMenuShopButton;
        _rankingButton.Title = TextCatalog.MainMenuRankingButton;
        _historyButton.Title = TextCatalog.MainMenuHistoryButton;

        _howToPlayButton.Title = TextCatalog.MainMenuHowToPlayButton;
        _settingsButton.Title = TextCatalog.MainMenuSettingsButton;
    }

    private StatTile CreateTile(int index, int width, int top)
    {
        int x = ContentX + (index * (width + TileGap));

        return new StatTile { Bounds = new Rectangle(x, top, width, TileHeight) };
    }

    private Button CreateActionButton(int index, int width, int top)
    {
        int x = ContentX + (index * (width + ActionGapRow));

        return new Button
        {
            Style = ButtonStyle.Secondary,
            HasArrow = true,
            Bounds = new Rectangle(x, top, width, ActionHeight)
        };
    }

    private Button CreateSmallButton(int index, int width, int top)
    {
        int x = ContentX + (index * (width + SmallButtonGap));

        return new Button
        {
            Style = ButtonStyle.Secondary,
            Bounds = new Rectangle(x, top, width, Theme.SecondaryButtonHeight)
        };
    }
}
