using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_ReportReview;

// CU-43 main flow step 5. The reason, the description, the match chat with the
// reported messages standing out, and the three ways to close the report.
public sealed class GuiReportReview : FormScreen
{
    private const int WideCardWidth = 760;
    private const int CardHeight = 384;
    private const int ReasonHeight = 62;
    private const int SectionGap = 18;
    private const int MessageHeight = 46;
    private const int MessageGap = 8;
    private const int VisibleMessages = 3;

    private readonly ValueBox _reasonBox;
    private readonly NoticeBox _description;
    private readonly List<DataRow> _messages = [];
    private readonly Button _sanctionButton;
    private readonly Button _dismissButton;
    private readonly Button _backButton;

    public GuiReportReview(INavigator navigator)
        : base(navigator, WideCardWidth, CardHeight)
    {
        int top = Card.Y + Theme.CardPadding;

        _reasonBox = new ValueBox
        {
            Bounds = new Rectangle(ContentX, top + LabelSpace, ContentWidth, Theme.FieldHeight)
        };

        _description = new NoticeBox
        {
            Bounds = new Rectangle(ContentX, _reasonBox.Bounds.Bottom + SectionGap, ContentWidth, ReasonHeight)
        };

        int messagesTop = _description.Bounds.Bottom + SectionGap;

        for (int i = 0; i < VisibleMessages; i++)
        {
            var message = new DataRow
            {
                IsHighlighted = IsEvenRow(i),
                Bounds = new Rectangle(
                    ContentX,
                    messagesTop + (i * (MessageHeight + MessageGap)),
                    ContentWidth,
                    MessageHeight)
            };

            _messages.Add(message);
            Register(message);
        }

        _sanctionButton = CreatePrimaryButton(false);
        _dismissButton = CreateSecondaryButton();
        _backButton = CreateSecondaryButton();
        LayOutActionsInRow([_sanctionButton, _dismissButton, _backButton]);

        _sanctionButton.Clicked += OnSanctionClicked;
        _dismissButton.Clicked += OnDismissClicked;
        _backButton.Clicked += OnBackClicked;

        Register(_reasonBox);
        Register(_description);
        Register(_sanctionButton);
        Register(_dismissButton);
        Register(_backButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.ReportReviewSubtitle;
    }

    private void OnSanctionClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.ApplySanction);
    }

    private void OnDismissClicked(object? sender, EventArgs e)
    {
    }

    private void OnBackClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    // Placeholder content: the report comes from the server.
    private void ApplyTexts()
    {
        _reasonBox.Label = TextCatalog.ReportReviewReasonLabel;
        _reasonBox.Value = TextCatalog.ModerationQueueReasonSample;
        _description.Text = TextCatalog.ReportReviewDescriptionSample;

        // The highlight marks the reported player, so the author has to match it.
        for (int i = 0; i < _messages.Count; i++)
        {
            _messages[i].Title = TextCatalog.ReportReviewMessageSample;
            _messages[i].Subtitle = IsEvenRow(i)
                ? TextCatalog.ReportReviewAuthorSample
                : TextCatalog.ReportReviewOtherAuthorSample;
        }

        _sanctionButton.Title = TextCatalog.ReportReviewSanctionButton;
        _dismissButton.Title = TextCatalog.ReportReviewDismissButton;
        _backButton.Title = TextCatalog.CommonBackButton;
    }
}
