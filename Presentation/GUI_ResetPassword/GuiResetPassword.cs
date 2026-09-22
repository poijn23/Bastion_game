using System;
using Microsoft.Xna.Framework;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_ResetPassword;

public sealed class GuiResetPassword : FormScreen
{
    private const int CodeLength = 6;
    private const float ResendSeconds = 300f;
    private const int SentTitleHeight = 28;
    private const int SentLineTop = 34;
    private const int SentLineHeight = 22;
    private const int ResendTop = 66;
    private const int DividerTop = ResendTop + Theme.SmallButtonHeight + 18;
    private const int CodeLabelTop = DividerTop + 16;
    private const int CodeFieldTop = CodeLabelTop + LabelSpace;
    private const int NewLabelTop = CodeFieldTop + Theme.FieldHeight + Theme.WarningSpace + 2;
    private const int NewFieldTop = NewLabelTop + LabelSpace;
    private const int MeterTop = NewFieldTop + Theme.FieldHeight + 8;
    private const int MeterHeight = 8;
    private const int NoteTop = MeterTop + MeterHeight + 6;
    private const int NoteHeight = 20;
    private const int ConfirmLabelTop = NoteTop + NoteHeight + 18;
    private const int ConfirmFieldTop = ConfirmLabelTop + LabelSpace;
    private const int ButtonTop = ConfirmFieldTop + Theme.FieldHeight + Theme.WarningSpace + 2;
    private const int ContentHeight = ButtonTop + Theme.PanelButtonHeight;
    private const int CardHeight =
        Theme.CardPadding + Theme.PanelBackHeight + Theme.PanelGap + Theme.PanelTitleHeight + Theme.PanelGap
        + ContentHeight + Theme.CardPadding;

    private readonly string _email;
    private readonly TextLine _sentTitle;
    private readonly TextLine _sentLine;
    private readonly Button _resendButton;
    private readonly Rule _divider;
    private readonly TextField _codeField;
    private readonly TextField _newPasswordField;
    private readonly StrengthMeter _meter;
    private readonly TextLine _note;
    private readonly TextField _confirmationField;
    private readonly Button _saveButton;
    private float _resendStartedAt = -1f;
    private float _secondsLeft = ResendSeconds;
    private bool _hasValidated;

    public GuiResetPassword(INavigator navigator, string email)
        : base(navigator, PanelNarrowWidth, CardHeight, ScreenLayout.Panel)
    {
        ArgumentNullException.ThrowIfNull(email);

        _email = email;
        int top = PanelContentTop;

        _sentTitle = new TextLine { Style = TextLineStyle.Heading, IsCentered = true, Bounds = new Rectangle(ContentX, top, ContentWidth, SentTitleHeight) };
        _sentLine = new TextLine { Style = TextLineStyle.Muted, IsCentered = true, Bounds = new Rectangle(ContentX, top + SentLineTop, ContentWidth, SentLineHeight) };
        _resendButton = CreateOutlineButton(new Rectangle(
            ContentX + ((ContentWidth - Theme.PanelButtonWidth) / 2), top + ResendTop, Theme.PanelButtonWidth, Theme.SmallButtonHeight));
        _resendButton.IsEnabled = false;
        _resendButton.Clicked += OnResendClicked;
        _divider = new Rule { Bounds = new Rectangle(ContentX, top + DividerTop, ContentWidth, 2) };

        _codeField = new TextField { MaxLength = CodeLength, Bounds = new Rectangle(ContentX, top + CodeFieldTop, ContentWidth, Theme.FieldHeight) };
        _newPasswordField = new TextField { IsPassword = true, Bounds = new Rectangle(ContentX, top + NewFieldTop, ContentWidth, Theme.FieldHeight) };
        _meter = new StrengthMeter { Bounds = new Rectangle(ContentX, top + MeterTop, ContentWidth, MeterHeight) };
        _note = new TextLine { Style = TextLineStyle.Small, Bounds = new Rectangle(ContentX, top + NoteTop, ContentWidth, NoteHeight) };
        _confirmationField = new TextField { IsPassword = true, Bounds = new Rectangle(ContentX, top + ConfirmFieldTop, ContentWidth, Theme.FieldHeight) };

        _saveButton = CreatePrimaryButton(true);
        _saveButton.MoveTo(new Rectangle(ContentX, top + ButtonTop, ContentWidth, Theme.PanelButtonHeight));
        _saveButton.Clicked += OnSaveClicked;

        Register(_sentTitle);
        Register(_sentLine);
        Register(_resendButton);
        Register(_divider);
        RegisterField(_codeField);
        RegisterField(_newPasswordField);
        Register(_meter);
        Register(_note);
        RegisterField(_confirmationField);
        Register(_saveButton);

        ApplyTexts();
        FocusFirstField();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.ResetPasswordSubtitle;
    }

    public override void Update(InputState input)
    {
        base.Update(input);

        if (_resendStartedAt < 0f)
        {
            _resendStartedAt = input.ElapsedSeconds;
        }

        _secondsLeft = MathF.Max(0f, ResendSeconds - (input.ElapsedSeconds - _resendStartedAt));
        _resendButton.IsEnabled = _secondsLeft <= 0f;
        _resendButton.Title = _secondsLeft <= 0f
            ? TextCatalog.ResetPasswordResendButton
            : string.Format(TextCatalog.ResetPasswordResendInFormat, TimeSpan.FromSeconds(_secondsLeft).ToString(@"m\:ss"));
        _meter.Strength = InputRules.PasswordStrength(_newPasswordField.Text);
    }

    protected override void ApplyTexts()
    {
        _sentTitle.Text = TextCatalog.ResetPasswordSentTitle;
        _sentLine.Text = string.Format(TextCatalog.ResetPasswordSentFormat, EmailMask.Apply(_email));
        _codeField.Label = TextCatalog.ResetPasswordCodeLabel;
        _codeField.Placeholder = TextCatalog.ResetPasswordCodePlaceholder;
        _newPasswordField.Label = TextCatalog.ResetPasswordNewLabel;
        _newPasswordField.Placeholder = TextCatalog.ResetPasswordNewPlaceholder;
        _note.Text = TextCatalog.ResetPasswordSessionsNote;
        _confirmationField.Label = TextCatalog.ResetPasswordConfirmLabel;
        _confirmationField.Placeholder = TextCatalog.ResetPasswordConfirmPlaceholder;
        _saveButton.Title = TextCatalog.ResetPasswordSaveButton;

        if (_hasValidated)
        {
            Validate();
        }
    }

    private void OnResendClicked(object? sender, EventArgs e)
    {
        _resendStartedAt = -1f;
        _secondsLeft = ResendSeconds;
    }

    private void OnSaveClicked(object? sender, EventArgs e)
    {
        _hasValidated = true;

        if (!Validate())
        {
            return;
        }

        Navigator.Restart(ScreenId.Login);
        Navigator.ShowMessage(DialogTone.Success, TextCatalog.ResetPasswordDoneBody);
    }

    private bool Validate()
    {
        _codeField.Warning = _codeField.Text.Trim().Length == CodeLength ? null : TextCatalog.ResetPasswordCodeInvalid;
        _newPasswordField.Warning = InputRules.MeetsPasswordPolicy(_newPasswordField.Text) ? null : TextCatalog.PasswordPolicyWarning;
        _confirmationField.Warning = _confirmationField.Text == _newPasswordField.Text ? null : TextCatalog.RegisterConfirmationMismatch;

        return !_codeField.HasWarning && !_newPasswordField.HasWarning && !_confirmationField.HasWarning;
    }
}
