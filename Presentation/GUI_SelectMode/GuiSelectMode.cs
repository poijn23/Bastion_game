using System;
using Microsoft.Xna.Framework;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_SelectMode;

// CU-17 main flow step 1. Mode and clock, preselected to the first option
// until a real last choice exists to remember (RN-07).
public sealed class GuiSelectMode : FormScreen
{
    private const int SectionGap = 20;
    private const int ModeChipsTop = LabelSpace;
    private const int ClockLabelTop = ModeChipsTop + Theme.ChipHeight + SectionGap;
    private const int ClockChipsTop = ClockLabelTop + LabelSpace;
    private const int CardHeight = Theme.CardPadding + ClockChipsTop + Theme.ChipHeight + Theme.CardPadding;

    private static readonly int[] ClockMinutes = [3, 5, 10];

    private readonly TextLine _modeLabel;
    private readonly ChipRow _modeChips;
    private readonly TextLine _clockLabel;
    private readonly ChipRow _clockChips;
    private readonly Button _searchButton;
    private readonly Button _backButton;
    private int _modeIndex;
    private int _clockIndex;

    public GuiSelectMode(INavigator navigator)
        : base(navigator, NarrowCardWidth, CardHeight)
    {
        int top = Card.Y + Theme.CardPadding;

        _modeLabel = new TextLine
        {
            Style = TextLineStyle.Label,
            Bounds = new Rectangle(ContentX, top, ContentWidth, LabelSpace)
        };
        _modeChips = new ChipRow
        {
            Bounds = new Rectangle(ContentX, top + ModeChipsTop, ContentWidth, Theme.ChipHeight)
        };
        _modeChips.ChipChosen += OnModeChosen;

        _clockLabel = new TextLine
        {
            Style = TextLineStyle.Label,
            Bounds = new Rectangle(ContentX, top + ClockLabelTop, ContentWidth, LabelSpace)
        };
        _clockChips = new ChipRow
        {
            Bounds = new Rectangle(ContentX, top + ClockChipsTop, ContentWidth, Theme.ChipHeight)
        };
        _clockChips.ChipChosen += OnClockChosen;

        _searchButton = CreatePrimaryButton(true);
        _backButton = CreateSecondaryButton();
        _searchButton.Clicked += OnSearchClicked;
        _backButton.Clicked += OnBackClicked;

        Register(_modeLabel);
        Register(_modeChips);
        Register(_clockLabel);
        Register(_clockChips);
        Register(_searchButton);
        Register(_backButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.SelectModeSubtitle;
    }

    private void OnModeChosen(object? sender, SelectionChangedEventArgs e)
    {
        _modeIndex = e.SelectedIndex;
        RefreshModeChips();
    }

    private void OnClockChosen(object? sender, SelectionChangedEventArgs e)
    {
        _clockIndex = e.SelectedIndex;
        RefreshClockChips();
    }

    private void OnSearchClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.Matchmaking);
    }

    private void OnBackClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    protected override void ApplyTexts()
    {
        _modeLabel.Text = TextCatalog.SelectModeModeLabel;
        _clockLabel.Text = TextCatalog.SelectModeClockLabel;
        RefreshModeChips();
        RefreshClockChips();
        _searchButton.Title = TextCatalog.SelectModeSearchButton;
        _backButton.Title = TextCatalog.CommonBackButton;
    }

    private void RefreshModeChips()
    {
        _modeChips.Items.Clear();
        string[] modes = [TextCatalog.ModoClasico, TextCatalog.ModoCuatroJugadores, TextCatalog.ModoRapida];

        for (int i = 0; i < modes.Length; i++)
        {
            _modeChips.Items.Add(new Chip { Text = modes[i], IsSelected = i == _modeIndex });
        }
    }

    private void RefreshClockChips()
    {
        _clockChips.Items.Clear();

        for (int i = 0; i < ClockMinutes.Length; i++)
        {
            string text = string.Format(TextCatalog.SelectModeClockFormat, ClockMinutes[i]);
            _clockChips.Items.Add(new Chip { Text = text, IsSelected = i == _clockIndex });
        }
    }
}
