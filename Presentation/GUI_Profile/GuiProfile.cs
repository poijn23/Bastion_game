using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_Profile;

public sealed class GuiProfile : FormScreen
{
    private const int HeadingHeight = 30;
    private const int HeaderLineGap = 6;
    private const int HeaderLineHeight = 22;
    private const int BarGap = 10;
    private const int BarHeight = 10;
    private const int BarWidth = 400;
    private const int BlockGap = 22;
    private const int TileHeight = 84;
    private const int TileGap = 16;
    private const int TileCount = 4;
    private const int ModeCount = 3;

    private const int HeaderHeight = HeadingHeight + HeaderLineGap + HeaderLineHeight + BarGap + BarHeight;
    private const int TilesTop = Theme.CardPadding + HeaderHeight + BlockGap;
    private const int ModesTop = TilesTop + TileHeight + BlockGap + LabelSpace;
    private const int CardHeight = ModesTop + Theme.FieldHeight + Theme.CardPadding;

    private readonly TextLine _nickname;
    private readonly TextLine _headerLine;
    private readonly ProgressBar _experienceBar;
    private readonly StatTile _matchesTile;
    private readonly StatTile _winsTile;
    private readonly StatTile _streakTile;
    private readonly StatTile _topEloTile;
    private readonly List<ValueBox> _modeBoxes = [];
    private readonly Button _editButton;
    private readonly Button _backButton;

    public GuiProfile(INavigator navigator)
        : base(navigator, WideCardWidth, CardHeight)
    {
        int y = Card.Y + Theme.CardPadding;
        _nickname = new TextLine { IsHeading = true, Bounds = new Rectangle(ContentX, y, ContentWidth, HeadingHeight) };

        y += HeadingHeight + HeaderLineGap;
        _headerLine = new TextLine { Bounds = new Rectangle(ContentX, y, ContentWidth, HeaderLineHeight) };

        y += HeaderLineHeight + BarGap;
        _experienceBar = new ProgressBar
        {
            Value = TestProfile.Experience / (float)TestProfile.ExperienceToNextLevel,
            Bounds = new Rectangle(ContentX, y, BarWidth, BarHeight)
        };

        _matchesTile = CreateTile(0);
        _winsTile = CreateTile(1);
        _streakTile = CreateTile(2);
        _topEloTile = CreateTile(3);

        for (int i = 0; i < ModeCount; i++)
        {
            _modeBoxes.Add(new ValueBox { Bounds = GetModeBounds(i) });
        }

        _editButton = CreatePrimaryButton(true);
        _backButton = CreateSecondaryButton();
        _editButton.Clicked += OnEditClicked;
        _backButton.Clicked += OnBackClicked;

        Register(_nickname);
        Register(_headerLine);
        Register(_experienceBar);
        Register(_matchesTile);
        Register(_winsTile);
        Register(_streakTile);
        Register(_topEloTile);

        foreach (ValueBox box in _modeBoxes)
        {
            Register(box);
        }

        Register(_editButton);
        Register(_backButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.ProfileSubtitle;
    }

    protected override void ApplyTexts()
    {
        _nickname.Text = TestAccount.Nickname;
        _headerLine.Text = string.Format(
            TextCatalog.ProfileHeaderFormat,
            GetTitleNames()[TestProfile.TitleIndex],
            TestProfile.Level,
            TestProfile.RegisteredOn.ToString("Y"));

        _matchesTile.Value = TestProfile.MatchesPlayed.ToString("N0");
        _matchesTile.Caption = TextCatalog.ProfileMatchesTile;
        _winsTile.Value = TestProfile.WinRate.ToString("P0");
        _winsTile.Caption = TextCatalog.ProfileWinsTile;
        _streakTile.Value = TestProfile.BestStreak.ToString("N0");
        _streakTile.Caption = TextCatalog.ProfileStreakTile;
        _topEloTile.Value = TestProfile.TopElo.ToString("N0");
        _topEloTile.Caption = TextCatalog.ProfileTopEloTile;

        IReadOnlyList<string> modes = GetModeNames();

        for (int i = 0; i < ModeCount; i++)
        {
            (int matches, double winRate) = TestProfile.ModeStats[i];
            _modeBoxes[i].Label = modes[i];
            _modeBoxes[i].Value = string.Format(
                TextCatalog.ProfileModeStatFormat, matches.ToString("N0"), winRate.ToString("P0"));
        }

        _editButton.Title = TextCatalog.ProfileEditButton;
        _backButton.Title = TextCatalog.CommonBackButton;
    }

    public static IReadOnlyList<string> GetModeNames()
    {
        return [TextCatalog.ModoClasico, TextCatalog.ModoCuatroJugadores, TextCatalog.ModoRapida];
    }

    public static IReadOnlyList<string> GetTitleNames()
    {
        return [TextCatalog.TituloNovato, TextCatalog.TituloEstratega, TextCatalog.TituloConstructor];
    }

    private void OnEditClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.EditProfile);
    }

    private void OnBackClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.AccountSettings);
    }

    private StatTile CreateTile(int index)
    {
        int width = (ContentWidth - ((TileCount - 1) * TileGap)) / TileCount;
        int x = ContentX + (index * (width + TileGap));

        return new StatTile { Bounds = new Rectangle(x, Card.Y + TilesTop, width, TileHeight) };
    }

    private Rectangle GetModeBounds(int index)
    {
        int width = (ContentWidth - ((ModeCount - 1) * TileGap)) / ModeCount;
        int x = ContentX + (index * (width + TileGap));

        return new Rectangle(x, Card.Y + ModesTop, width, Theme.FieldHeight);
    }
}
