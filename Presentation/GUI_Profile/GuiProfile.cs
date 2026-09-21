using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_Profile;

public sealed class GuiProfile : FormScreen
{
    private const int AvatarSize = 96;
    private const int HeaderTextInset = AvatarSize + 20;
    private const int HeadingHeight = 30;
    private const int HeaderLineTop = 38;
    private const int BarTop = 66;
    private const int BarWidth = 300;
    private const int BarHeight = 8;
    private const int EditButtonWidth = 120;
    private const int BlockGap = 20;
    private const int TileHeight = 84;
    private const int TileGap = 16;
    private const int TileCount = 4;
    private const int PanelHeight = 190;
    private const int PanelRowTop = 44;
    private const int PanelRowHeight = 40;
    private const int ModeCount = 3;
    private const int LinksBarHeight = 64;
    private const int LinksLabelWidth = 90;

    private const int TilesTop = AvatarSize + BlockGap;
    private const int PanelsTop = TilesTop + TileHeight + BlockGap;
    private const int LinksTop = PanelsTop + PanelHeight + BlockGap;
    private const int ContentHeight = LinksTop + LinksBarHeight;
    private const int CardHeight =
        Theme.CardPadding + Theme.PanelBackHeight + Theme.PanelGap + ContentHeight + Theme.CardPadding;

    private readonly Avatar _avatar;
    private readonly TextLine _nickname;
    private readonly TextLine _headerLine;
    private readonly ProgressBar _experienceBar;
    private readonly Button _editButton;
    private readonly StatTile _matchesTile;
    private readonly StatTile _winsTile;
    private readonly StatTile _streakTile;
    private readonly StatTile _topEloTile;
    private readonly PanelBox _modesPanel;
    private readonly List<TextLine> _modeNames = [];
    private readonly List<TextLine> _modeStats = [];
    private readonly PanelBox _titlesPanel;
    private readonly ChipRow _titleChips;
    private readonly PanelBox _linksBar;
    private readonly TextLine _linksLabel;
    private readonly ChipRow _linkChips;

    public GuiProfile(INavigator navigator)
        : base(navigator, Theme.PanelCardWidth, CardHeight, ScreenLayout.PanelBare)
    {
        int top = PanelContentTop;

        _avatar = new Avatar { Bounds = new Rectangle(ContentX, top, AvatarSize, AvatarSize) };
        _nickname = new TextLine
        {
            Style = TextLineStyle.Heading,
            Bounds = new Rectangle(ContentX + HeaderTextInset, top, ContentWidth, HeadingHeight)
        };
        _headerLine = new TextLine
        {
            Style = TextLineStyle.Muted,
            Bounds = new Rectangle(ContentX + HeaderTextInset, top + HeaderLineTop, ContentWidth, HeadingHeight)
        };
        _experienceBar = new ProgressBar
        {
            Value = TestProfile.Experience / (float)TestProfile.ExperienceToNextLevel,
            Bounds = new Rectangle(ContentX + HeaderTextInset, top + BarTop, BarWidth, BarHeight)
        };
        _editButton = CreateOutlineButton(
            new Rectangle(Card.Right - Theme.CardPadding - EditButtonWidth, top, EditButtonWidth, Theme.SmallButtonHeight));
        _editButton.Clicked += OnEditClicked;

        _matchesTile = CreateTile(0, top);
        _winsTile = CreateTile(1, top);
        _streakTile = CreateTile(2, top);
        _topEloTile = CreateTile(3, top);

        int panelWidth = (ContentWidth - BlockGap) / 2;
        var modesArea = new Rectangle(ContentX, top + PanelsTop, panelWidth, PanelHeight);
        _modesPanel = new PanelBox { Bounds = modesArea };

        for (int i = 0; i < ModeCount; i++)
        {
            int rowTop = modesArea.Y + PanelRowTop + (i * PanelRowHeight);
            int rowX = modesArea.X + PanelBox.Padding;
            int rowWidth = modesArea.Width - (PanelBox.Padding * 2);
            _modeNames.Add(new TextLine { Bounds = new Rectangle(rowX, rowTop, rowWidth, PanelRowHeight) });
            _modeStats.Add(new TextLine
            {
                Style = TextLineStyle.Muted,
                IsRightAligned = true,
                Bounds = new Rectangle(rowX, rowTop, rowWidth, PanelRowHeight)
            });
        }

        var titlesArea = new Rectangle(ContentX + panelWidth + BlockGap, top + PanelsTop, panelWidth, PanelHeight);
        _titlesPanel = new PanelBox { Bounds = titlesArea };
        _titleChips = new ChipRow
        {
            IsInteractive = false,
            Bounds = new Rectangle(
                titlesArea.X + PanelBox.Padding, titlesArea.Y + PanelRowTop,
                titlesArea.Width - (PanelBox.Padding * 2), PanelHeight - PanelRowTop - PanelBox.Padding)
        };

        var linksArea = new Rectangle(ContentX, top + LinksTop, ContentWidth, LinksBarHeight);
        _linksBar = new PanelBox { Bounds = linksArea };
        _linksLabel = new TextLine
        {
            Style = TextLineStyle.Muted,
            Bounds = new Rectangle(linksArea.X + PanelBox.Padding, linksArea.Y + (LinksBarHeight - 22) / 2, LinksLabelWidth, 22)
        };
        _linkChips = new ChipRow
        {
            Bounds = new Rectangle(
                linksArea.X + PanelBox.Padding + LinksLabelWidth, linksArea.Y + ((LinksBarHeight - Theme.ChipHeight) / 2),
                linksArea.Width - (PanelBox.Padding * 2) - LinksLabelWidth, Theme.ChipHeight)
        };
        _linkChips.ChipChosen += OnLinkChipChosen;

        Register(_avatar);
        Register(_nickname);
        Register(_headerLine);
        Register(_experienceBar);
        Register(_editButton);
        Register(_matchesTile);
        Register(_winsTile);
        Register(_streakTile);
        Register(_topEloTile);
        Register(_modesPanel);

        for (int i = 0; i < ModeCount; i++)
        {
            Register(_modeNames[i]);
            Register(_modeStats[i]);
            Register(new Rule
            {
                Bounds = new Rectangle(
                    modesArea.X + PanelBox.Padding, modesArea.Y + PanelRowTop + ((i + 1) * PanelRowHeight) - 8,
                    modesArea.Width - (PanelBox.Padding * 2), 2)
            });
        }

        Register(_titlesPanel);
        Register(_titleChips);
        Register(_linksBar);
        Register(_linksLabel);
        Register(_linkChips);

        ApplyTexts();
    }

    public static IReadOnlyList<string> GetModeNames()
    {
        return [TextCatalog.ModoClasico, TextCatalog.ModoCuatroJugadores, TextCatalog.ModoRapida];
    }

    public static IReadOnlyList<string> GetTitleNames()
    {
        return [TextCatalog.TituloNovato, TextCatalog.TituloEstratega, TextCatalog.TituloConstructor];
    }

    public static IReadOnlyList<string> GetLinkTypeNames()
    {
        return [TextCatalog.TipoEnlaceTwitch, TextCatalog.TipoEnlaceYoutube, TextCatalog.TipoEnlaceDiscord, TextCatalog.TipoEnlaceOtro];
    }

    public static string GetEquippedTitle()
    {
        IReadOnlyList<string> titles = GetTitleNames();

        return TestProfile.TitleIndex >= 0 && TestProfile.TitleIndex < titles.Count
            ? titles[TestProfile.TitleIndex]
            : TextCatalog.ProfileNoTitle;
    }

    protected override string GetSubtitle()
    {
        return string.Empty;
    }

    protected override void ApplyTexts()
    {
        _avatar.Text = TextCatalog.AvatarPlaceholder;
        _nickname.Text = TestAccount.Nickname;
        _headerLine.Text = string.Format(
            TextCatalog.ProfileHeaderFormat,
            GetEquippedTitle(),
            TestProfile.Level,
            TestProfile.RegisteredOn.ToString("MMM yyyy"));
        _editButton.Title = TextCatalog.CommonEditButton;

        _matchesTile.Value = TestProfile.MatchesPlayed.ToString("N0");
        _matchesTile.Caption = TextCatalog.ProfileMatchesTile;
        _winsTile.Value = TestProfile.WinRate.ToString("P0");
        _winsTile.Caption = TextCatalog.ProfileWinsTile;
        _streakTile.Value = TestProfile.BestStreak.ToString("N0");
        _streakTile.Caption = TextCatalog.ProfileStreakTile;
        _topEloTile.Value = TestProfile.TopElo.ToString("N0");
        _topEloTile.Caption = TextCatalog.ProfileTopEloTile;

        _modesPanel.Title = TextCatalog.ProfileByModeTitle;
        _modesPanel.Footer = string.Format(TextCatalog.ProfileAverageLengthFormat, TestProfile.AverageMoves);
        IReadOnlyList<string> modes = GetModeNames();

        for (int i = 0; i < ModeCount; i++)
        {
            (int matches, double winRate) = TestProfile.ModeStats[i];
            _modeNames[i].Text = modes[i];
            _modeStats[i].Text = string.Format(TextCatalog.ProfileModeStatFormat, matches.ToString("N0"), winRate.ToString("P0"));
        }

        _titlesPanel.Title = TextCatalog.ProfileTitlesTitle;
        _titlesPanel.Footer = TextCatalog.ProfileSeeAllLink;
        _titleChips.Items.Clear();
        IReadOnlyList<string> titles = GetTitleNames();

        for (int i = 0; i < titles.Count; i++)
        {
            _titleChips.Items.Add(new Chip { Text = titles[i], IsSelected = i == TestProfile.TitleIndex });
        }

        _linksLabel.Text = TextCatalog.ProfileLinksTitle;
        _linkChips.Items.Clear();
        IReadOnlyList<string> types = GetLinkTypeNames();

        foreach ((int typeIndex, string url) in TestProfile.Links)
        {
            if (url.Length > 0)
            {
                _linkChips.Items.Add(new Chip { Text = types[typeIndex] });
            }
        }

        _linkChips.Items.Add(new Chip { Text = TextCatalog.ProfileAddLinkChip, IsDashed = true });
    }

    private void OnEditClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.EditProfile);
    }

    private void OnLinkChipChosen(object? sender, SelectionChangedEventArgs e)
    {
        if (e.SelectedIndex == _linkChips.Items.Count - 1)
        {
            Navigator.GoTo(ScreenId.EditProfile);
        }
    }

    private StatTile CreateTile(int index, int top)
    {
        int width = (ContentWidth - ((TileCount - 1) * TileGap)) / TileCount;
        int x = ContentX + (index * (width + TileGap));

        return new StatTile { Bounds = new Rectangle(x, top + TilesTop, width, TileHeight) };
    }
}
