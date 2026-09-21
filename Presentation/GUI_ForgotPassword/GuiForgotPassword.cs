using System;
using Microsoft.Xna.Framework;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_ForgotPassword;

// CU-03 main flow step 1. Asks only for the email. The answer is the same
// whether or not the address exists, so the form cannot be used to find out.
public sealed class GuiForgotPassword : FormScreen
{
    private const int CardHeight = 234;
    private const int NoticeHeight = 64;
    private const int NoticeGap = 26;
    private const int MaxEmailLength = 254;

    private readonly NoticeBox _notice;
    private readonly TextField _emailField;
    private readonly Button _sendButton;
    private readonly Button _cancelButton;

    public GuiForgotPassword(INavigator navigator)
        : base(navigator, NarrowCardWidth, CardHeight)
    {
        _notice = new NoticeBox
        {
            Bounds = new Rectangle(ContentX, Card.Y + Theme.CardPadding, ContentWidth, NoticeHeight)
        };

        int fieldTop = _notice.Bounds.Bottom + NoticeGap + LabelSpace;
        _emailField = new TextField
        {
            MaxLength = MaxEmailLength,
            Bounds = new Rectangle(ContentX, fieldTop, ContentWidth, Theme.FieldHeight)
        };

        _sendButton = CreatePrimaryButton(true);
        _cancelButton = CreateSecondaryButton();
        _sendButton.Clicked += OnSendClicked;
        _cancelButton.Clicked += OnCancelClicked;

        Register(_notice);
        RegisterField(_emailField);
        Register(_sendButton);
        Register(_cancelButton);

        ApplyTexts();
        FocusFirstField();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.ForgotPasswordSubtitle;
    }

    private void OnSendClicked(object? sender, EventArgs e)
    {
    }

    private void OnCancelClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    protected override void ApplyTexts()
    {
        _notice.Text = TextCatalog.ForgotPasswordNotice;
        _emailField.Label = TextCatalog.ForgotPasswordEmailLabel;
        _emailField.Placeholder = TextCatalog.ForgotPasswordEmailPlaceholder;
        _sendButton.Title = TextCatalog.ForgotPasswordSendButton;
        _cancelButton.Title = TextCatalog.CommonCancelButton;
    }
}
