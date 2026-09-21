using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_ModerationQueue;

// CU-43 main flow step 3. Reports and appeals waiting, with their reason, their
// age and how many reports the reported player already carries.
public sealed class GuiModerationQueue : FormScreen
{
    private const int RowHeight = 62;
    private const int RowGap = 10;
    private const int VisibleRows = 4;

    // Derived, so the last row keeps fitting when the card or the rows change.
    private const int CardHeight =
        (Theme.CardPadding * 2) + (VisibleRows * RowHeight) + ((VisibleRows - 1) * RowGap);

    private readonly List<DataRow> _rows = [];
    private readonly Button _reviewButton;
    private readonly Button _backButton;

    public GuiModerationQueue(INavigator navigator)
        : base(navigator, WideCardWidth, CardHeight)
    {
        for (int i = 0; i < VisibleRows; i++)
        {
            var row = new DataRow { Bounds = GetRowBounds(i) };
            _rows.Add(row);
            Register(row);
        }

        _reviewButton = CreatePrimaryButton(true);
        _backButton = CreateSecondaryButton();
        _reviewButton.Clicked += OnReviewClicked;
        _backButton.Clicked += OnBackClicked;

        Register(_reviewButton);
        Register(_backButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.ModerationQueueSubtitle;
    }

    private void OnReviewClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.ReportReview);
    }

    private void OnBackClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    // Placeholder rows: the queue comes from the server.
    protected override void ApplyTexts()
    {
        foreach (DataRow row in _rows)
        {
            row.Title = TextCatalog.ModerationQueueReasonSample;
            row.Subtitle = TextCatalog.ModerationQueueAgeSample;
            row.Value = TextCatalog.ModerationQueueCountSample;
        }

        _reviewButton.Title = TextCatalog.ModerationQueueReviewButton;
        _backButton.Title = TextCatalog.CommonBackButton;
    }

    private Rectangle GetRowBounds(int index)
    {
        int top = Card.Y + Theme.CardPadding + (index * (RowHeight + RowGap));

        return new Rectangle(ContentX, top, ContentWidth, RowHeight);
    }
}
