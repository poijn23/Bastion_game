using System;
using Microsoft.Xna.Framework;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_Register;

// Step 1 of the CU-02 main flow. Presentation only: it does not validate, does
// not open a connection and does not build the REGISTER_REQUEST.
public sealed class GuiRegister : FormScreen
{
    private const int MaxFirstNameLength = 50;
    private const int MaxLastNameLength = 50;
    private const int MaxNicknameLength = 30;
    private const int MaxEmailLength = 254;
    private const int MaxDayLength = 2;
    private const int MaxMonthLength = 2;
    private const int MaxYearLength = 4;

    private const int WideCardWidth = 900;
    private const int Gutter = 32;

    // A row is its label, its field and the line validation may print under
    // it, plus a little air. Four of them at the shared FormScreen.RowSpacing
    // would push the two buttons past the bottom of a 720 pixel window, so
    // this screen sizes its own rows from what they actually have to hold.
    private const int RowAir = 4;
    private const int RegisterRowSpacing = LabelSpace + Theme.FieldHeight + Theme.WarningSpace + RowAir;

    // The last row prints its warnings into the bottom padding, which therefore
    // cannot be the usual CardPadding.
    private const int BottomPadding = Theme.WarningSpace + RowAir;
    private const int WideCardHeight =
        Theme.CardPadding + LabelSpace + (3 * RegisterRowSpacing) + Theme.FieldHeight + BottomPadding;

    private const int NameRow = 0;
    private const int AccountRow = 1;
    private const int PasswordRow = 2;
    private const int BirthDateRow = 3;

    private const int DayWidth = 92;
    private const int MonthWidth = 92;
    private const int DateGap = 12;

    private readonly TextField _firstNameField;
    private readonly TextField _lastNameField;
    private readonly TextField _nicknameField;
    private readonly TextField _emailField;
    private readonly TextField _passwordField;
    private readonly TextField _confirmationField;
    private readonly TextField _dayField;
    private readonly TextField _monthField;
    private readonly TextField _yearField;
    private readonly DropDown _interfaceLanguageDropDown;
    private readonly CheckBox _termsCheckBox;
    private readonly Button _createButton;
    private readonly Button _cancelButton;
    private bool _hasValidated;

    public GuiRegister(INavigator navigator)
        : base(navigator, WideCardWidth, WideCardHeight)
    {
        _firstNameField = CreateField(NameRow, false, MaxFirstNameLength);
        _lastNameField = CreateField(NameRow, true, MaxLastNameLength);
        _nicknameField = CreateField(AccountRow, false, MaxNicknameLength);
        _emailField = CreateField(AccountRow, true, MaxEmailLength);
        _passwordField = CreatePasswordField(false);
        _confirmationField = CreatePasswordField(true);
        _dayField = CreateDatePart(ContentX, DayWidth, MaxDayLength);
        _monthField = CreateDatePart(ContentX + DayWidth + DateGap, MonthWidth, MaxMonthLength);
        _yearField = CreateDatePart(ContentX + DayWidth + MonthWidth + (DateGap * 2), GetYearWidth(), MaxYearLength);
        _interfaceLanguageDropDown = LanguagePicker.Create();
        _termsCheckBox = CreateTermsCheckBox();
        _createButton = CreatePrimaryButton(true);
        _cancelButton = CreateSecondaryButton();

        _createButton.Clicked += OnCreateAccountClicked;
        _cancelButton.Clicked += OnCancelClicked;
        _interfaceLanguageDropDown.SelectionChanged += OnInterfaceLanguageChanged;

        RegisterField(_firstNameField);
        RegisterField(_lastNameField);
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
        Register(_interfaceLanguageDropDown);

        ApplyTexts();
        FocusFirstField();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.RegisterSubtitle;
    }

    // Reloads the catalog and redraws. What the player typed is kept, because
    // only labels and hints are touched here (CU-02 FA-02 steps 1 and 2).
    private void OnInterfaceLanguageChanged(object? sender, SelectionChangedEventArgs e)
    {
        LanguagePicker.Apply(e.SelectedIndex);
        ApplyTexts();

        // The warnings are catalog strings too, so they follow the language.
        if (_hasValidated)
        {
            Validate();
        }
    }

    // The connection does not exist yet, so a valid form behaves as if the
    // server had answered REGISTER_OK (main flow step 20). The account
    // idioma_preferido of step 22 travels as the interface language in force
    // on this screen, which is why the form no longer asks for it: choosing it
    // twice in the same window only invited the two to disagree.
    private void OnCreateAccountClicked(object? sender, EventArgs e)
    {
        _hasValidated = true;

        if (!Validate())
        {
            return;
        }

        Navigator.GoTo(ScreenId.Login);
        Navigator.ShowMessage(DialogTone.Success, TextCatalog.RegisterSuccessBody);
    }

    // Every field is checked, not only the first bad one, so the player fixes
    // the whole form in one pass (CU-02 FA-03 to FA-05 and FA-07). The
    // duplicate checks stand in for the server answer until it exists.
    private bool Validate()
    {
        _firstNameField.Warning = string.IsNullOrWhiteSpace(_firstNameField.Text)
            ? TextCatalog.RegisterFirstNameRequired
            : null;

        _lastNameField.Warning = string.IsNullOrWhiteSpace(_lastNameField.Text)
            ? TextCatalog.RegisterLastNameRequired
            : null;

        _nicknameField.Warning = GetNicknameWarning(_nicknameField.Text.Trim());
        _emailField.Warning = GetEmailWarning(_emailField.Text.Trim());

        _passwordField.Warning = InputRules.HasPasswordLength(_passwordField.Text)
            ? null
            : TextCatalog.RegisterPasswordTooShort;

        _confirmationField.Warning = _confirmationField.Text == _passwordField.Text
            ? null
            : TextCatalog.RegisterConfirmationMismatch;

        // Only the day box carries the text, or it would be drawn three times.
        _dayField.Warning = GetBirthDateWarning();

        _termsCheckBox.Warning = _termsCheckBox.IsChecked
            ? null
            : TextCatalog.RegisterTermsRequired;

        return !_firstNameField.HasWarning
            && !_lastNameField.HasWarning
            && !_nicknameField.HasWarning
            && !_emailField.HasWarning
            && !_passwordField.HasWarning
            && !_confirmationField.HasWarning
            && !_dayField.HasWarning
            && !_termsCheckBox.HasWarning;
    }

    private static string? GetNicknameWarning(string nickname)
    {
        if (!InputRules.HasNicknameLength(nickname))
        {
            return TextCatalog.RegisterNicknameLength;
        }

        return TestAccount.IsNicknameTaken(nickname) ? TextCatalog.RegisterNicknameTaken : null;
    }

    private static string? GetEmailWarning(string email)
    {
        if (!InputRules.IsEmail(email))
        {
            return TextCatalog.RegisterEmailInvalid;
        }

        return TestAccount.IsEmailTaken(email) ? TextCatalog.RegisterEmailTaken : null;
    }

    private string? GetBirthDateWarning()
    {
        if (!InputRules.TryParseBirthDate(_dayField.Text, _monthField.Text, _yearField.Text, out DateOnly date))
        {
            return TextCatalog.RegisterBirthDateInvalid;
        }

        return InputRules.IsInFuture(date) ? TextCatalog.RegisterBirthDateFuture : null;
    }

    // CU-02 FA-01. Discarding also has to clear the two passwords and uncheck
    // the terms, which belongs to validation and is still pending.
    private void OnCancelClicked(object? sender, EventArgs e)
    {
        Navigator.ShowConfirm(new ConfirmRequest
        {
            Body = TextCatalog.RegisterDiscardBody,
            PrimaryLabel = TextCatalog.RegisterDiscardButton,
            SecondaryLabel = TextCatalog.RegisterKeepEditingButton,
            OnConfirm = GoBackToLogin
        });
    }

    private void GoBackToLogin()
    {
        Navigator.GoTo(ScreenId.Login);
    }

    private void ApplyTexts()
    {
        _firstNameField.Label = TextCatalog.RegisterFirstNameLabel;
        _firstNameField.Placeholder = TextCatalog.RegisterFirstNamePlaceholder;

        _lastNameField.Label = TextCatalog.RegisterLastNameLabel;
        _lastNameField.Placeholder = TextCatalog.RegisterLastNamePlaceholder;

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

        _interfaceLanguageDropDown.Options = LanguagePicker.GetNames();

        _termsCheckBox.Text = TextCatalog.RegisterTermsText;
        _termsCheckBox.LinkText = TextCatalog.RegisterTermsLinkText;

        _createButton.Title = TextCatalog.RegisterCreateButton;
        _createButton.Subtitle = TextCatalog.RegisterCreateButtonDetail;
        _cancelButton.Title = TextCatalog.RegisterCancelButton;
    }

    private int GetColumnWidth()
    {
        return (ContentWidth - Gutter) / 2;
    }

    private int GetYearWidth()
    {
        return GetColumnWidth() - DayWidth - MonthWidth - (DateGap * 2);
    }

    private Rectangle GetCell(int row, bool isRightColumn)
    {
        int columnWidth = GetColumnWidth();
        int x = isRightColumn ? ContentX + columnWidth + Gutter : ContentX;

        return new Rectangle(x, FirstRowTop + (row * RegisterRowSpacing), columnWidth, Theme.FieldHeight);
    }

    private TextField CreateField(int row, bool isRightColumn, int maxLength)
    {
        return new TextField
        {
            MaxLength = maxLength,
            Bounds = GetCell(row, isRightColumn)
        };
    }

    private TextField CreatePasswordField(bool isRightColumn)
    {
        return new TextField
        {
            IsPassword = true,
            Bounds = GetCell(PasswordRow, isRightColumn)
        };
    }

    // Three boxes instead of one field: clearer for an eight year old player
    // (CON-12) and it needs no format to be explained.
    private TextField CreateDatePart(int x, int width, int maxLength)
    {
        return new TextField
        {
            MaxLength = maxLength,
            IsCentered = true,
            Bounds = new Rectangle(x, FirstRowTop + (BirthDateRow * RegisterRowSpacing), width, Theme.FieldHeight)
        };
    }

    // Sits in the free cell beside the date of birth; below them it would need
    // a fifth row the window cannot afford. It takes the whole cell so its box,
    // its text and its warning line up with those of the date boxes.
    private CheckBox CreateTermsCheckBox()
    {
        return new CheckBox
        {
            Bounds = GetCell(BirthDateRow, true)
        };
    }
}
