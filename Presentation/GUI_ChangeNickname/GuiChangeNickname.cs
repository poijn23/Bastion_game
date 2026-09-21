using System;
using Microsoft.Xna.Framework;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_ChangeNickname;

// CU-13 main flow step 1. The change is irreversible and can only be done once
// (CU-13 RN-01), so the warning is part of the form and not a dialog.
public sealed class GuiChangeNickname : FormScreen
{
    private const int CardHeight = 332;
    private const int NoticeHeight = 76;
    private const int NoticeGap = 26;
    private const int MaxNicknameLength = 30;

    private readonly NoticeBox _notice;
    private readonly ValueBox _currentBox;
    private readonly TextField _newField;
    private readonly Button _changeButton;
    private readonly Button _cancelButton;

    public GuiChangeNickname(INavigator navigator)
        : base(navigator, NarrowCardWidth, CardHeight)
    {
        _notice = new NoticeBox
        {
            IsCritical = true,
            Bounds = new Rectangle(ContentX, Card.Y + Theme.CardPadding, ContentWidth, NoticeHeight)
        };

        int currentTop = _notice.Bounds.Bottom + NoticeGap + LabelSpace;
        _currentBox = new ValueBox
        {
            Bounds = new Rectangle(ContentX, currentTop, ContentWidth, Theme.FieldHeight)
        };

        int newTop = currentTop + RowSpacing;
        _newField = new TextField
        {
            MaxLength = MaxNicknameLength,
            Bounds = new Rectangle(ContentX, newTop, ContentWidth, Theme.FieldHeight)
        };

        _changeButton = CreatePrimaryButton(true);
        _cancelButton = CreateSecondaryButton();
        _changeButton.Clicked += OnChangeClicked;
        _cancelButton.Clicked += OnCancelClicked;

        Register(_notice);
        Register(_currentBox);
        RegisterField(_newField);
        Register(_changeButton);
        Register(_cancelButton);

        ApplyTexts();
        FocusFirstField();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.ChangeNicknameSubtitle;
    }

    private void OnChangeClicked(object? sender, EventArgs e)
    {
    }

    private void OnCancelClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    protected override void ApplyTexts()
    {
        _notice.Text = TextCatalog.ChangeNicknameNotice;
        _currentBox.Label = TextCatalog.ChangeNicknameCurrentLabel;
        _newField.Label = TextCatalog.ChangeNicknameNewLabel;
        _newField.Placeholder = TextCatalog.ChangeNicknameNewPlaceholder;
        _changeButton.Title = TextCatalog.ChangeNicknameSaveButton;
        _cancelButton.Title = TextCatalog.CommonCancelButton;
    }
}
