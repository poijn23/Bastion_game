using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_MatchHistory;

// CU-36 main flow step 4. One row per match: result, opponent, mode, duration
// and the elo change, signed and colored.
public sealed class GuiMatchHistory : FormScreen
{
    private const int RowHeight = 62;
    private const int RowGap = 10;
    private const int VisibleRows = 5;

    private static readonly int _cardHeight = ComputeListCardHeight(VisibleRows, RowHeight, RowGap);

    private readonly List<DataRow> _rows = [];
    private readonly Button _backButton;

    public GuiMatchHistory(INavigator navigator)
        : base(navigator, WideCardWidth, _cardHeight)
    {
        for (int i = 0; i < VisibleRows; i++)
        {
            var row = new DataRow { Bounds = GetRowBounds(i) };
            _rows.Add(row);
            Register(row);
        }

        _backButton = CreateSecondaryButton();
        _backButton.MoveTo(PrimaryButtonBounds);
        _backButton.Clicked += OnBackClicked;
        Register(_backButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.MatchHistorySubtitle;
    }

    private void OnBackClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    // Placeholder rows: the history comes from the server.
    protected override void ApplyTexts()
    {
        for (int i = 0; i < _rows.Count; i++)
        {
            bool isWin = IsEvenRow(i);
            _rows[i].Title = isWin ? TextCatalog.MatchHistoryWin : TextCatalog.MatchHistoryLoss;
            _rows[i].Subtitle = TextCatalog.MatchHistoryRowSample;
            _rows[i].Value = isWin ? TextCatalog.MatchHistoryGainSample : TextCatalog.MatchHistoryLossSample;
            _rows[i].Tone = isWin ? ValueTone.Positive : ValueTone.Negative;
        }

        _backButton.Title = TextCatalog.CommonBackButton;
    }

    private Rectangle GetRowBounds(int index)
    {
        int top = Card.Y + Theme.CardPadding + (index * (RowHeight + RowGap));

        return new Rectangle(ContentX, top, ContentWidth, RowHeight);
    }
}
