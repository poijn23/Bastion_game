using System;
using Microsoft.Xna.Framework;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_DeleteAccount;

// CU-11 main flow steps 1 and 2. Two steps on purpose: the first states what is
// lost and asks for the password, the second asks the player to type a word.
public sealed class GuiDeleteAccount : FormScreen
{
    private const int CardHeight = 288;
    private const int NoticeHeight = 132;
    private const int NoticeGap = 26;
    private const int ConfirmNoticeHeight = 96;

    private readonly NoticeBox _lossNotice;
    private readonly TextField _passwordField;
    private readonly NoticeBox _confirmNotice;
    private readonly TextField _wordField;
    private readonly Button _primaryAction;
    private readonly Button _cancelButton;

    public GuiDeleteAccount(INavigator navigator)
        : base(navigator, NarrowCardWidth, CardHeight)
    {
        int noticeTop = Card.Y + Theme.CardPadding;
        _lossNotice = new NoticeBox
        {
            IsCritical = true,
            Bounds = new Rectangle(ContentX, noticeTop, ContentWidth, NoticeHeight)
        };

        _confirmNotice = new NoticeBox
        {
            IsCritical = true,
            IsVisible = false,
            Bounds = new Rectangle(ContentX, noticeTop, ContentWidth, ConfirmNoticeHeight)
        };

        int passwordTop = _lossNotice.Bounds.Bottom + NoticeGap + LabelSpace;
        _passwordField = new TextField
        {
            IsPassword = true,
            Bounds = new Rectangle(ContentX, passwordTop, ContentWidth, Theme.FieldHeight)
        };

        int wordTop = _confirmNotice.Bounds.Bottom + NoticeGap + LabelSpace;
        _wordField = new TextField
        {
            IsVisible = false,
            Bounds = new Rectangle(ContentX, wordTop, ContentWidth, Theme.FieldHeight)
        };

        _primaryAction = CreatePrimaryButton(true);
        _cancelButton = CreateSecondaryButton();
        _primaryAction.Clicked += OnPrimaryClicked;
        _cancelButton.Clicked += OnCancelClicked;

        Register(_lossNotice);
        Register(_confirmNotice);
        RegisterField(_passwordField);
        RegisterField(_wordField);
        Register(_primaryAction);
        Register(_cancelButton);

        ApplyTexts();
        FocusFirstField();
    }

    // The second step replaces the contents of the same card instead of opening
    // another screen, so the player keeps the context of what is being deleted.
    private bool IsConfirmStep => _wordField.IsVisible;

    protected override string GetSubtitle()
    {
        return IsConfirmStep ? TextCatalog.DeleteAccountConfirmSubtitle : TextCatalog.DeleteAccountSubtitle;
    }

    private void OnPrimaryClicked(object? sender, EventArgs e)
    {
    }

    private void OnCancelClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    protected override void ApplyTexts()
    {
        _lossNotice.Text = TextCatalog.DeleteAccountNotice;
        _passwordField.Label = TextCatalog.DeleteAccountPasswordLabel;
        _passwordField.Placeholder = TextCatalog.DeleteAccountPasswordPlaceholder;
        _confirmNotice.Text = TextCatalog.DeleteAccountConfirmNotice;
        _wordField.Label = TextCatalog.DeleteAccountWordLabel;
        _wordField.Placeholder = TextCatalog.DeleteAccountWordPlaceholder;
        _primaryAction.Title = TextCatalog.DeleteAccountContinueButton;
        _cancelButton.Title = TextCatalog.CommonCancelButton;
    }
}
