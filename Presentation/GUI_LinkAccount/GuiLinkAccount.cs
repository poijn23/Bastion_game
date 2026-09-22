using System;
using Microsoft.Xna.Framework;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_LinkAccount;

// CU-07 main flow step 1. Presentation only, like GUI_Register: it does not
// validate against the server and does not build the LINK request. Unlike
// GUI_Register, there is no name row (D-06 already dropped it there too),
// so two rows are enough. Laid out by hand, like GUI_DeleteAccount, because
// the notice above the fields pushes them lower than FirstRowTop assumes.
public sealed class GuiLinkAccount : FormScreen
{
    private const int MinimumAgeYears = 8;
    private const int MaxNicknameLength = 30;
    private const int MaxEmailLength = 254;
    private const int MaxDayLength = 2;
    private const int MaxMonthLength = 2;
    private const int MaxYearLength = 4;

    private const int NoticeHeight = 20;
    private const int RowAir = 4;
    private const int FieldRowSpacing = LabelSpace + Theme.FieldHeight + Theme.WarningSpace + RowAir;
    private const int FieldsTop = NoticeHeight + RowAir + LabelSpace;
    private const int BottomPadding = Theme.WarningSpace + RowAir;
    private const int WideCardHeight = Theme.CardPadding + FieldsTop + (3 * FieldRowSpacing) + BottomPadding;

    private const int AccountRow = 0;
    private const int PasswordRow = 1;
    private const int BirthDateRow = 2;

    private const int DayWidth = 92;
    private const int MonthWidth = 92;
    private const int DateGap = 12;

    private readonly TextBlock _notice;
    private readonly TextField _nicknameField;
    private readonly TextField _emailField;
    private readonly TextField _passwordField;
    private readonly TextField _confirmationField;
    private readonly TextField _dayField;
    private readonly TextField _monthField;
    private readonly TextField _yearField;
    private readonly CheckBox _termsCheckBox;
    private readonly Button _createButton;
    private readonly Button _cancelButton;
    private bool _hasValidated;

    public GuiLinkAccount(INavigator navigator)
        : base(navigator, WideCardWidth, WideCardHeight)
    {
        int top = Card.Y + Theme.CardPadding;
        int rowsTop = top + FieldsTop;
        int rightX = ContentX + ColumnWidth + Gutter;

        _notice = new TextBlock { IsSmall = true, Bounds = new Rectangle(ContentX, top, ContentWidth, NoticeHeight) };

        _nicknameField = CreateField(rowsTop, AccountRow, ContentX, MaxNicknameLength);
        _emailField = CreateField(rowsTop, AccountRow, rightX, MaxEmailLength);
        _passwordField = CreatePasswordField(rowsTop, ContentX);
        _confirmationField = CreatePasswordField(rowsTop, rightX);

        int dateRowTop = rowsTop + (BirthDateRow * FieldRowSpacing);
        int monthX = ContentX + DayWidth + DateGap;
        int yearX = ContentX + DayWidth + MonthWidth + (DateGap * 2);
        _dayField = CreateDatePart(dateRowTop, ContentX, DayWidth, MaxDayLength);
        _monthField = CreateDatePart(dateRowTop, monthX, MonthWidth, MaxMonthLength);
        _yearField = CreateDatePart(dateRowTop, yearX, GetYearWidth(), MaxYearLength);
        _termsCheckBox = new CheckBox { Bounds = new Rectangle(rightX, dateRowTop, ColumnWidth, Theme.FieldHeight) };

        _createButton = CreatePrimaryButton(true);
        _cancelButton = CreateSecondaryButton();
        _createButton.Clicked += OnCreateAccountClicked;
        _cancelButton.Clicked += OnCancelClicked;

        Register(_notice);
        RegisterField(_nicknameField);
        RegisterField(_emailField);
        RegisterField(_passwordField);
        RegisterField(_confirmationField);
        RegisterField(_dayField);
        RegisterField(_monthField);
        RegisterField(_yearField);
        Register(_termsCheckBox);
        Register(_createButton);
        Register(_cancelButton);

        ApplyTexts();
        FocusFirstField();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.LinkAccountSubtitle;
    }

    private void OnCreateAccountClicked(object? sender, EventArgs e)
    {
        _hasValidated = true;

        if (!Validate())
        {
            return;
        }

        Navigator.GoTo(ScreenId.RegistrationSuccess, _emailField.Text.Trim());
    }

    private void OnCancelClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    private bool Validate()
    {
        _nicknameField.Warning = InputRules.HasNicknameLength(_nicknameField.Text.Trim())
            ? null
            : TextCatalog.RegisterNicknameLength;

        _emailField.Warning = InputRules.IsEmail(_emailField.Text.Trim()) ? null : TextCatalog.RegisterEmailInvalid;

        _passwordField.Warning = InputRules.MeetsPasswordPolicy(_passwordField.Text)
            ? null
            : TextCatalog.PasswordPolicyWarning;

        _confirmationField.Warning = _confirmationField.Text == _passwordField.Text
            ? null
            : TextCatalog.RegisterConfirmationMismatch;

        _dayField.Warning = GetBirthDateWarning();
        _termsCheckBox.Warning = _termsCheckBox.IsChecked ? null : TextCatalog.RegisterTermsRequired;

        return !_nicknameField.HasWarning
            && !_emailField.HasWarning
            && !_passwordField.HasWarning
            && !_confirmationField.HasWarning
            && !_dayField.HasWarning
            && !_termsCheckBox.HasWarning;
    }

    private string? GetBirthDateWarning()
    {
        if (!InputRules.TryParseBirthDate(_dayField.Text, _monthField.Text, _yearField.Text, out DateOnly date))
        {
            return TextCatalog.RegisterBirthDateInvalid;
        }

        if (InputRules.IsInFuture(date))
        {
            return TextCatalog.RegisterBirthDateFuture;
        }

        return InputRules.IsAtLeastYearsOld(date, MinimumAgeYears) ? null : TextCatalog.RegisterBirthDateInvalid;
    }

    protected override void ApplyTexts()
    {
        _notice.Text = TextCatalog.LinkAccountNotice;

        _nicknameField.Label = TextCatalog.RegisterNicknameLabel;
        _nicknameField.Placeholder = TextCatalog.RegisterNicknamePlaceholder;
        _emailField.Label = TextCatalog.RegisterEmailLabel;
        _emailField.Placeholder = TextCatalog.RegisterEmailPlaceholder;

        _passwordField.Label = TextCatalog.RegisterPasswordLabel;
        _passwordField.Placeholder = TextCatalog.RegisterPasswordPlaceholder;
        _confirmationField.Label = TextCatalog.RegisterConfirmationLabel;
        _confirmationField.Placeholder = TextCatalog.RegisterConfirmationPlaceholder;

        _dayField.Label = TextCatalog.RegisterBirthDateLabel;
        _dayField.Placeholder = TextCatalog.RegisterDayPlaceholder;
        _monthField.Placeholder = TextCatalog.RegisterMonthPlaceholder;
        _yearField.Placeholder = TextCatalog.RegisterYearPlaceholder;

        _termsCheckBox.Text = TextCatalog.RegisterTermsText;
        _termsCheckBox.LinkText = TextCatalog.RegisterTermsLinkText;

        _createButton.Title = TextCatalog.RegisterCreateButton;
        _cancelButton.Title = TextCatalog.RegisterCancelButton;

        if (_hasValidated)
        {
            Validate();
        }
    }

    private int GetYearWidth()
    {
        return ColumnWidth - DayWidth - MonthWidth - (DateGap * 2);
    }

    private TextField CreateField(int rowsTop, int row, int x, int maxLength)
    {
        int y = rowsTop + (row * FieldRowSpacing);

        return new TextField { MaxLength = maxLength, Bounds = new Rectangle(x, y, ColumnWidth, Theme.FieldHeight) };
    }

    private TextField CreatePasswordField(int rowsTop, int x)
    {
        return new TextField
        {
            IsPassword = true,
            Bounds = new Rectangle(x, rowsTop + (PasswordRow * FieldRowSpacing), ColumnWidth, Theme.FieldHeight)
        };
    }

    private static TextField CreateDatePart(int rowTop, int x, int width, int maxLength)
    {
        return new TextField
        {
            MaxLength = maxLength,
            IsCentered = true,
            Bounds = new Rectangle(x, rowTop, width, Theme.FieldHeight)
        };
    }
}
