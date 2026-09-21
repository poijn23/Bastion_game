using System;
using Microsoft.Xna.Framework;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_ChangeNickname;

public sealed class GuiChangeNickname : FormScreen
{
    private const int MaxNicknameLength = 30;
    private const int FieldGap = 26;
    private const int NoticeHeight = 76;
    private const int ButtonsGap = 24;
    private const int ContentHeight = LabelSpace + Theme.FieldHeight + FieldGap + LabelSpace + Theme.FieldHeight + FieldGap + NoticeHeight;
    private const int CardHeight =
        Theme.CardPadding + Theme.PanelBackHeight + Theme.PanelGap + Theme.PanelTitleHeight + Theme.PanelGap
        + ContentHeight + ButtonsGap + Theme.PanelButtonHeight + Theme.CardPadding;

    private readonly ValueBox _currentBox;
    private readonly TextField _newField;
    private readonly NoticeBox _notice;
    private readonly Button _continueButton;
    private readonly Button _cancelButton;
    private bool _hasValidated;

    public GuiChangeNickname(INavigator navigator)
        : base(navigator, NarrowCardWidth, CardHeight, ScreenLayout.Panel)
    {
        int top = PanelContentTop + LabelSpace;
        _currentBox = new ValueBox { Bounds = new Rectangle(ContentX, top, ContentWidth, Theme.FieldHeight) };

        int newTop = top + Theme.FieldHeight + FieldGap + LabelSpace;
        _newField = new TextField { MaxLength = MaxNicknameLength, Bounds = new Rectangle(ContentX, newTop, ContentWidth, Theme.FieldHeight) };

        int noticeTop = newTop + Theme.FieldHeight + FieldGap;
        _notice = new NoticeBox { IsCritical = true, Bounds = new Rectangle(ContentX, noticeTop, ContentWidth, NoticeHeight) };

        _continueButton = CreatePrimaryButton(true);
        _cancelButton = CreateOutlineButton(SecondaryButtonBounds);
        _continueButton.Clicked += OnContinueClicked;
        _cancelButton.Clicked += OnCancelClicked;

        Register(_currentBox);
        RegisterField(_newField);
        Register(_notice);
        Register(_continueButton);
        Register(_cancelButton);

        ApplyTexts();
        FocusFirstField();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.ChangeNicknameSubtitle;
    }

    protected override void ApplyTexts()
    {
        _currentBox.Label = TextCatalog.ChangeNicknameCurrentLabel;
        _currentBox.Value = TestAccount.Nickname;
        _newField.Label = TextCatalog.ChangeNicknameNewLabel;
        _newField.Placeholder = TextCatalog.ChangeNicknameNewPlaceholder;
        _notice.Text = TextCatalog.ChangeNicknameNotice;
        _continueButton.Title = TextCatalog.CommonContinueButton;
        _cancelButton.Title = TextCatalog.CommonCancelButton;

        if (_hasValidated)
        {
            Validate();
        }
    }

    private void OnContinueClicked(object? sender, EventArgs e)
    {
        _hasValidated = true;

        if (!Validate())
        {
            return;
        }

        string newNickname = _newField.Text.Trim();
        Navigator.ShowConfirm(new ConfirmRequest
        {
            Title = TextCatalog.ChangeNicknameConfirmTitle,
            Body = string.Format(TextCatalog.ChangeNicknameConfirmFormat, TestAccount.Nickname, newNickname),
            Detail = TextCatalog.ChangeNicknameConfirmDetail,
            PrimaryLabel = TextCatalog.ChangeNicknameConfirmButton,
            SecondaryLabel = TextCatalog.CommonCancelButton,
            OnConfirm = () => Apply(newNickname)
        });
    }

    private void OnCancelClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    private void Apply(string newNickname)
    {
        TestAccount.Nickname = newNickname;
        TestProfile.HasChangedNickname = true;
        Navigator.GoTo(ScreenId.Profile);
        Navigator.ShowMessage(DialogTone.Success, TextCatalog.ChangeNicknameDoneBody);
    }

    private bool Validate()
    {
        string nickname = _newField.Text.Trim();

        if (!InputRules.HasNicknameLength(nickname))
        {
            _newField.Warning = TextCatalog.RegisterNicknameLength;
        }
        else if (TestAccount.IsNicknameTaken(nickname))
        {
            _newField.Warning = TextCatalog.RegisterNicknameTaken;
        }
        else
        {
            _newField.Warning = null;
        }

        return !_newField.HasWarning;
    }
}
