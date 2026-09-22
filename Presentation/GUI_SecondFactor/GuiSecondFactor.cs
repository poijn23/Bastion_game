using System;
using Microsoft.Xna.Framework;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_SecondFactor;

// CU-01 FA-13, FA-14. Reached today only from GUI_Menu: GUI_Login does not
// yet branch on doble_factor_habilitado.
public sealed class GuiSecondFactor : FormScreen
{
    private const int CodeLength = 6;
    private const float CodeLifetimeSeconds = 300f;

    private const int HintHeight = 20;
    private const int ResendGap = 10;
    private const int ResendButtonTop = HintHeight + ResendGap;
    private const int FieldTop = ResendButtonTop + Theme.SmallButtonHeight + LabelSpace;
    private const int ExpiresGap = 10;
    private const int ExpiresHeight = 20;
    private const int ContentHeight = FieldTop + Theme.FieldHeight + ExpiresGap + ExpiresHeight;
    private const int CardHeight = Theme.CardPadding + ContentHeight + Theme.CardPadding;

    private readonly TextLine _hint;
    private readonly Button _resendButton;
    private readonly TextField _codeField;
    private readonly TextLine _expiresLine;
    private readonly Button _verifyButton;
    private readonly Button _cancelButton;
    private float _startedAt = -1f;

    public GuiSecondFactor(INavigator navigator)
        : base(navigator, NarrowCardWidth, CardHeight)
    {
        int top = Card.Y + Theme.CardPadding;

        _hint = new TextLine
        {
            Style = TextLineStyle.Small,
            Bounds = new Rectangle(ContentX, top, ContentWidth, HintHeight)
        };

        int resendX = ContentX + ((ContentWidth - Theme.PanelButtonWidth) / 2);
        _resendButton = CreateOutlineButton(
            new Rectangle(resendX, top + ResendButtonTop, Theme.PanelButtonWidth, Theme.SmallButtonHeight));
        _resendButton.Clicked += OnResendClicked;

        _codeField = new TextField
        {
            MaxLength = CodeLength,
            IsCentered = true,
            Bounds = new Rectangle(ContentX, top + FieldTop, ContentWidth, Theme.FieldHeight)
        };

        int expiresTop = top + FieldTop + Theme.FieldHeight + ExpiresGap;
        _expiresLine = new TextLine
        {
            Style = TextLineStyle.Muted,
            Bounds = new Rectangle(ContentX, expiresTop, ContentWidth, ExpiresHeight)
        };

        _verifyButton = CreatePrimaryButton(true);
        _cancelButton = CreateSecondaryButton();
        _verifyButton.Clicked += OnVerifyClicked;
        _cancelButton.Clicked += OnCancelClicked;

        Register(_hint);
        Register(_resendButton);
        RegisterField(_codeField);
        Register(_expiresLine);
        Register(_verifyButton);
        Register(_cancelButton);

        ApplyTexts();
        FocusFirstField();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.SecondFactorSubtitle;
    }

    public override void Update(InputState input)
    {
        base.Update(input);

        if (_startedAt < 0f)
        {
            _startedAt = input.ElapsedSeconds;
        }

        float secondsLeft = MathF.Max(0f, CodeLifetimeSeconds - (input.ElapsedSeconds - _startedAt));
        string clock = TimeSpan.FromSeconds(secondsLeft).ToString(@"m\:ss");
        _expiresLine.Text = string.Format(TextCatalog.SecondFactorExpiresFormat, clock);
    }

    private void OnVerifyClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.MainMenu);
    }

    private void OnCancelClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    private void OnResendClicked(object? sender, EventArgs e)
    {
        _startedAt = -1f;
        Navigator.ShowMessage(DialogTone.Success, TextCatalog.SecondFactorResentBody);
    }

    protected override void ApplyTexts()
    {
        _hint.Text = TextCatalog.SecondFactorHint;
        _resendButton.Title = TextCatalog.SecondFactorResendButton;
        _codeField.Label = TextCatalog.SecondFactorCodeLabel;
        _codeField.Placeholder = TextCatalog.SecondFactorCodePlaceholder;
        _verifyButton.Title = TextCatalog.SecondFactorVerifyButton;
        _cancelButton.Title = TextCatalog.CommonCancelButton;
    }
}
