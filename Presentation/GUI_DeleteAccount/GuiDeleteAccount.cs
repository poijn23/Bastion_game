using System;
using Microsoft.Xna.Framework;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_DeleteAccount;

public sealed class GuiDeleteAccount : FormScreen
{
    private const int LossBoxHeight = 128;
    private const int LossTextTop = 38;
    private const int BlockGap = 24;
    private const int NoteHeight = 22;
    private const int NoteGap = 10;
    private const int ContentHeight = LossBoxHeight + BlockGap + LabelSpace + Theme.FieldHeight + NoteGap + NoteHeight;
    private const int CardHeight =
        Theme.CardPadding + Theme.PanelBackHeight + Theme.PanelGap + Theme.PanelTitleHeight + Theme.PanelGap
        + ContentHeight + BlockGap + Theme.PanelButtonHeight + Theme.CardPadding;

    private readonly PanelBox _lossBox;
    private readonly TextBlock _lossText;
    private readonly TextField _passwordField;
    private readonly TextField _wordField;
    private readonly TextLine _note;
    private readonly Button _primaryButton;
    private readonly Button _cancelButton;
    private bool _hasValidated;

    public GuiDeleteAccount(INavigator navigator)
        : base(navigator, NarrowCardWidth, CardHeight, ScreenLayout.Panel)
    {
        int top = PanelContentTop;
        var lossArea = new Rectangle(ContentX, top, ContentWidth, LossBoxHeight);
        _lossBox = new PanelBox { Bounds = lossArea };
        _lossText = new TextBlock
        {
            Bounds = new Rectangle(lossArea.X + PanelBox.Padding, lossArea.Y + LossTextTop, lossArea.Width - (PanelBox.Padding * 2), LossBoxHeight - LossTextTop)
        };

        int fieldTop = lossArea.Bottom + BlockGap + LabelSpace;
        _passwordField = new TextField { IsPassword = true, Bounds = new Rectangle(ContentX, fieldTop, ContentWidth, Theme.FieldHeight) };
        _wordField = new TextField { IsVisible = false, Bounds = new Rectangle(ContentX, fieldTop, ContentWidth, Theme.FieldHeight) };
        _note = new TextLine
        {
            Style = TextLineStyle.Small,
            Bounds = new Rectangle(ContentX, fieldTop + Theme.FieldHeight + NoteGap + Theme.WarningSpace, ContentWidth, NoteHeight)
        };

        _primaryButton = CreatePrimaryButton(false);
        _cancelButton = CreateSecondaryButton();
        _cancelButton.MoveTo(SecondaryButtonBounds);
        _primaryButton.Clicked += OnPrimaryClicked;
        _cancelButton.Clicked += OnCancelClicked;

        Register(_lossBox);
        Register(_lossText);
        RegisterField(_passwordField);
        RegisterField(_wordField);
        Register(_note);
        Register(_primaryButton);
        Register(_cancelButton);

        ApplyTexts();
        FocusFirstField();
    }

    private bool IsConfirmStep => _wordField.IsVisible;

    protected override string GetSubtitle()
    {
        return TextCatalog.DeleteAccountSubtitle;
    }

    public override void Update(InputState input)
    {
        base.Update(input);

        if (IsConfirmStep)
        {
            _primaryButton.IsEnabled = string.Equals(_wordField.Text.Trim(), TextCatalog.DeleteAccountWord, StringComparison.OrdinalIgnoreCase);
        }
    }

    protected override void ApplyTexts()
    {
        _lossBox.Title = TextCatalog.DeleteAccountLossTitle;
        _lossText.Text = string.Format(
            TextCatalog.DeleteAccountLossFormat,
            TestProfile.CurrentElo.ToString("N0"), TestProfile.MatchesPlayed.ToString("N0"),
            TestProfile.SkinCount.ToString("N0"), TestProfile.Coins.ToString("N0"));
        _passwordField.Label = TextCatalog.DeleteAccountPasswordLabel;
        _passwordField.Placeholder = TextCatalog.DeleteAccountPasswordPlaceholder;
        _wordField.Label = string.Format(TextCatalog.DeleteAccountWordLabelFormat, TextCatalog.DeleteAccountWord);
        _wordField.Placeholder = TextCatalog.DeleteAccountWord;
        _note.Text = TextCatalog.DeleteAccountNotice;
        _primaryButton.Title = IsConfirmStep ? TextCatalog.DeleteAccountDeleteButton : TextCatalog.CommonContinueButton;
        _cancelButton.Title = TextCatalog.CommonCancelButton;

        if (_hasValidated && !IsConfirmStep)
        {
            ValidatePassword();
        }
    }

    private void OnPrimaryClicked(object? sender, EventArgs e)
    {
        if (!IsConfirmStep)
        {
            _hasValidated = true;

            if (!ValidatePassword())
            {
                return;
            }

            _passwordField.Hide();
            _passwordField.IsFocused = false;
            _wordField.Show();
            _wordField.IsFocused = true;
            _primaryButton.IsEnabled = false;
            ApplyTexts();
            return;
        }

        Navigator.GoTo(ScreenId.Login);
        Navigator.ShowMessage(DialogTone.Warning, TextCatalog.DeleteAccountDoneBody);
    }

    private void OnCancelClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    private bool ValidatePassword()
    {
        _passwordField.Warning = string.Equals(_passwordField.Text, TestAccount.Password, StringComparison.Ordinal)
            ? null
            : TextCatalog.DeleteAccountWrongPassword;

        return !_passwordField.HasWarning;
    }
}
