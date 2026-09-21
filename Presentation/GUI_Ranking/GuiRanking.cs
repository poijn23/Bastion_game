using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_Ranking;

// CU-35 main flow step 4. The top three stand out and the player own row is
// pinned at the bottom even when it falls outside the visible page.
public sealed class GuiRanking : FormScreen
{
    private const int WideCardWidth = 760;
    private const int CardHeight = 440;
    private const int RowHeight = 54;
    private const int RowGap = 8;
    private const int VisibleRows = 5;
    private const int PinnedGap = 18;
    private const int PodiumSize = 3;

    private readonly List<DataRow> _rows = [];
    private readonly DataRow _ownRow;
    private readonly Button _backButton;

    public GuiRanking(INavigator navigator)
        : base(navigator, WideCardWidth, CardHeight)
    {
        for (int i = 0; i < VisibleRows; i++)
        {
            var row = new DataRow { IsPodium = i < PodiumSize, Bounds = GetRowBounds(i) };
            _rows.Add(row);
            Register(row);
        }

        int pinnedTop = GetRowBounds(VisibleRows - 1).Bottom + PinnedGap;
        _ownRow = new DataRow
        {
            IsHighlighted = true,
            Bounds = new Rectangle(ContentX, pinnedTop, ContentWidth, RowHeight)
        };

        _backButton = CreateSecondaryButton();
        _backButton.MoveTo(PrimaryButtonBounds);
        _backButton.Clicked += OnBackClicked;

        Register(_ownRow);
        Register(_backButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.RankingSubtitle;
    }

    private void OnBackClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    // Placeholder rows: the table comes from the server.
    private void ApplyTexts()
    {
        for (int i = 0; i < _rows.Count; i++)
        {
            _rows[i].Badge = (i + 1).ToString();
            _rows[i].Title = TextCatalog.RankingPlayerSample;
            _rows[i].Subtitle = TextCatalog.RankingDivisionSample;
            _rows[i].Value = TextCatalog.RankingEloSample;
        }

        _ownRow.Badge = TextCatalog.RankingOwnPositionSample;
        _ownRow.Title = TextCatalog.RankingYouLabel;
        _ownRow.Subtitle = TextCatalog.RankingDivisionSample;
        _ownRow.Value = TextCatalog.RankingEloSample;
        _backButton.Title = TextCatalog.CommonBackButton;
    }

    private Rectangle GetRowBounds(int index)
    {
        int top = Card.Y + Theme.CardPadding + (index * (RowHeight + RowGap));

        return new Rectangle(ContentX, top, ContentWidth, RowHeight);
    }
}
