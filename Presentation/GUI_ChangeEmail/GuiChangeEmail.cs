using System;
using Microsoft.Xna.Framework;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_ChangeEmail;

// CU-09 alternate flow FA-01. The new address starts unverified, so the notice
// warns that the account goes back to a pending state until it is confirmed.
public sealed class GuiChangeEmail : FormScreen
{
    private const int CardHeight = 300;
    private const int NoticeHeight = 64;
    private const int NoticeGap = 24;
    private const int MaxEmailLength = 254;

    private readonly TextField _newEmailField;
    private readonly TextField _passwordField;
    private readonly NoticeBox _notice;
    private readonly Button _saveButton;
    private readonly Button _cancelButton;

    public GuiChangeEmail(INavigator navigator)
        : base(navigator, NarrowCardWidth, CardHeight)
    {
        _newEmailField = new TextField { MaxLength = MaxEmailLength, Bounds = GetRow(0) };
        _passwordField = new TextField { IsPassword = true, Bounds = GetRow(1) };

        int noticeTop = GetRow(1).Bottom + NoticeGap;
        _notice = new NoticeBox
        {
            Bounds = new Rectangle(ContentX, noticeTop, ContentWidth, NoticeHeight)
        };

        _saveButton = CreatePrimaryButton(true);
        _cancelButton = CreateSecondaryButton();
        _saveButton.Clicked += OnSaveClicked;
        _cancelButton.Clicked += OnCancelClicked;

        RegisterField(_newEmailField);
        RegisterField(_passwordField);
        Register(_notice);
        Register(_saveButton);
        Register(_cancelButton);

        ApplyTexts();
        FocusFirstField();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.ChangeEmailSubtitle;
    }

    private void OnSaveClicked(object? sender, EventArgs e)
    {
    }

    private void OnCancelClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    protected override void ApplyTexts()
    {
        _newEmailField.Label = TextCatalog.ChangeEmailNewLabel;
        _newEmailField.Placeholder = TextCatalog.ChangeEmailNewPlaceholder;
        _passwordField.Label = TextCatalog.ChangeEmailPasswordLabel;
        _passwordField.Placeholder = TextCatalog.ChangeEmailPasswordPlaceholder;
        _notice.Text = TextCatalog.ChangeEmailNotice;
        _saveButton.Title = TextCatalog.CommonSaveButton;
        _cancelButton.Title = TextCatalog.CommonCancelButton;
    }
}
