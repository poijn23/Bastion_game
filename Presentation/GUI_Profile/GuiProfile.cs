using System;
using Microsoft.Xna.Framework;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_Profile;

// CU-15 main flow step 4. Read only: every number comes from the server, so
// what is here is the layout and the captions.
public sealed class GuiProfile : FormScreen
{
    private const int WideCardWidth = 760;
    private const int CardHeight = 396;
    private const int AvatarSize = 96;
    private const int TileCount = 4;
    private const int TileGap = 12;
    private const int TileHeight = 68;
    private const int ChartHeight = 120;
    private const int SectionGap = 20;
    private const int SampleCount = 8;
    private const int HeaderGap = 20;
    private const int NameHeight = 28;
    private const int TitleHeight = 20;
    private const int LevelHeight = 30;
    private const int Threshold = 1200;
    private const float SampleLevelProgress = 0.62f;

    // Crosses the threshold so the marked line is readable.
    private static readonly int[] _sampleEloSeries = [1120, 1160, 1140, 1190, 1230, 1210, 1260, 1240];

    private readonly AvatarBox _avatar;
    private readonly TextLabel _nickname;
    private readonly TextLabel _title;
    private readonly ProgressBar _level;
    private readonly StatTile _matchesTile;
    private readonly StatTile _winRateTile;
    private readonly StatTile _streakTile;
    private readonly StatTile _eloTile;
    private readonly LineChart _chart;
    private readonly Button _editButton;
    private readonly Button _backButton;

    public GuiProfile(INavigator navigator)
        : base(navigator, WideCardWidth, CardHeight)
    {
        _avatar = new AvatarBox
        {
            Bounds = new Rectangle(ContentX, Card.Y + Theme.CardPadding, AvatarSize, AvatarSize)
        };

        int headerX = _avatar.Bounds.Right + HeaderGap;
        int headerWidth = Card.Right - Theme.CardPadding - headerX;

        _nickname = new TextLabel
        {
            Role = TextRole.Heading,
            Bounds = new Rectangle(headerX, _avatar.Bounds.Y, headerWidth, NameHeight)
        };

        _title = new TextLabel
        {
            Role = TextRole.Caption,
            Bounds = new Rectangle(headerX, _avatar.Bounds.Y + NameHeight, headerWidth, TitleHeight)
        };

        _level = new ProgressBar
        {
            Progress = SampleLevelProgress,
            Bounds = new Rectangle(
                headerX,
                _avatar.Bounds.Bottom - LevelHeight,
                headerWidth,
                LevelHeight)
        };

        int tilesTop = _avatar.Bounds.Bottom + SectionGap;
        _matchesTile = CreateTile(0, tilesTop);
        _winRateTile = CreateTile(1, tilesTop);
        _streakTile = CreateTile(2, tilesTop);
        _eloTile = CreateTile(3, tilesTop);

        _chart = new LineChart
        {
            Bounds = new Rectangle(ContentX, tilesTop + TileHeight + SectionGap, ContentWidth, ChartHeight)
        };

        _editButton = CreatePrimaryButton(true);
        _backButton = CreateSecondaryButton();
        _editButton.Clicked += OnEditClicked;
        _backButton.Clicked += OnBackClicked;

        Register(_avatar);
        Register(_nickname);
        Register(_title);
        Register(_level);
        Register(_matchesTile);
        Register(_winRateTile);
        Register(_streakTile);
        Register(_eloTile);
        Register(_chart);
        Register(_editButton);
        Register(_backButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.ProfileSubtitle;
    }

    private void OnEditClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.EditProfile);
    }

    private void OnBackClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    private void ApplyTexts()
    {
        _avatar.IconLabel = TextCatalog.ProfileAvatarPlaceholder;
        _nickname.Text = TextCatalog.ProfileNicknameSample;
        _title.Text = TextCatalog.ProfileTitleSample;
        _level.Caption = TextCatalog.ProfileLevelSample;
        _matchesTile.Value = TextCatalog.ProfileMatchesSample;
        _winRateTile.Value = TextCatalog.ProfileWinRateSample;
        _streakTile.Value = TextCatalog.ProfileStreakSample;
        _eloTile.Value = TextCatalog.RankingEloSample;
        _matchesTile.Caption = TextCatalog.ProfileMatchesCaption;
        _winRateTile.Caption = TextCatalog.ProfileWinRateCaption;
        _streakTile.Caption = TextCatalog.ProfileStreakCaption;
        _eloTile.Caption = TextCatalog.ProfileEloCaption;
        _editButton.Title = TextCatalog.ProfileEditButton;
        _backButton.Title = TextCatalog.CommonBackButton;

        _chart.Values = _sampleEloSeries;
        _chart.Threshold = Threshold;
    }

    private StatTile CreateTile(int index, int top)
    {
        int width = (ContentWidth - (TileGap * (TileCount - 1))) / TileCount;
        int x = ContentX + (index * (width + TileGap));

        return new StatTile
        {
            Bounds = new Rectangle(x, top, width, TileHeight)
        };
    }
}
