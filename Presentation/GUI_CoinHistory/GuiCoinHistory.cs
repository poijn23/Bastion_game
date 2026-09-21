using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_CoinHistory;

// CU-40 main flow step 4. Balance on top, then the movements with their date,
// description and signed amount.
public sealed class GuiCoinHistory : FormScreen
{
    private const int CardHeight = 352;
    private const int BalanceHeight = 76;
    private const int BalanceGap = 18;
    private const int RowHeight = 58;
    private const int RowGap = 10;
    private const int VisibleRows = 3;

    private readonly StatTile _balanceTile;
    private readonly List<DataRow> _rows = [];
    private readonly Button _backButton;

    public GuiCoinHistory(INavigator navigator)
        : base(navigator, NarrowCardWidth, CardHeight)
    {
        _balanceTile = new StatTile
        {
            Bounds = new Rectangle(ContentX, Card.Y + Theme.CardPadding, ContentWidth, BalanceHeight)
        };

        for (int i = 0; i < VisibleRows; i++)
        {
            var row = new DataRow { Bounds = GetRowBounds(i) };
            _rows.Add(row);
            Register(row);
        }

        _backButton = CreateSecondaryButton();
        _backButton.MoveTo(PrimaryButtonBounds);
        _backButton.Clicked += OnBackClicked;

        Register(_balanceTile);
        Register(_backButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.CoinHistorySubtitle;
    }

    private void OnBackClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    // Placeholder rows: the movements come from the server.
    private void ApplyTexts()
    {
        _balanceTile.Caption = TextCatalog.CoinHistoryBalanceCaption;
        _balanceTile.Value = TextCatalog.CoinHistoryBalanceSample;

        for (int i = 0; i < _rows.Count; i++)
        {
            bool isGain = IsEvenRow(i);
            _rows[i].Title = isGain ? TextCatalog.CoinHistoryGainSample : TextCatalog.CoinHistorySpendSample;
            _rows[i].Subtitle = TextCatalog.CoinHistoryDateSample;
            _rows[i].Value = isGain
                ? TextCatalog.CoinHistoryGainAmountSample
                : TextCatalog.CoinHistorySpendAmountSample;
            _rows[i].Tone = isGain ? ValueTone.Positive : ValueTone.Negative;
        }

        _backButton.Title = TextCatalog.CommonBackButton;
    }

    private Rectangle GetRowBounds(int index)
    {
        int top = Card.Y + Theme.CardPadding + BalanceHeight + BalanceGap + (index * (RowHeight + RowGap));

        return new Rectangle(ContentX, top, ContentWidth, RowHeight);
    }
}
