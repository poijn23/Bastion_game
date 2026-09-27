using System;
using System.Collections.Generic;
using System.Linq;
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

    // A row is its label, its field and the line validation may print under
    // it, plus a little air. Four of them at the shared FormScreen.RowSpacing
    // would push the two buttons past the bottom of a 720 pixel window, so
    // this screen sizes its own rows from what they actually have to hold.
    private const int MinimumAge = 8;

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
    private readonly CheckBox _termsCheckBox;
    private readonly Button _createButton;
    private readonly Button _cancelButton;
    private readonly IAccountGateway _accounts;

    private bool _hasValidated;

    public GuiRegister(INavigator navigator, IAccountGateway accounts)
        : base(navigator, WideCardWidth, WideCardHeight)
    {
        ArgumentNullException.ThrowIfNull(accounts);

        _accounts = accounts;

        _firstNameField = CreateField(NameRow, false, MaxFirstNameLength);
        _lastNameField = CreateField(NameRow, true, MaxLastNameLength);
        _nicknameField = CreateField(AccountRow, false, MaxNicknameLength);
        _emailField = CreateField(AccountRow, true, MaxEmailLength);
        _passwordField = CreatePasswordField(false);
        _confirmationField = CreatePasswordField(true);
        _dayField = CreateDatePart(ContentX, DayWidth, MaxDayLength);
        _monthField = CreateDatePart(ContentX + DayWidth + DateGap, MonthWidth, MaxMonthLength);
        _yearField = CreateDatePart(ContentX + DayWidth + MonthWidth + (DateGap * 2), GetYearWidth(), MaxYearLength);
        _termsCheckBox = CreateTermsCheckBox();
        _createButton = CreatePrimaryButton(true);
        _cancelButton = CreateSecondaryButton();

        _createButton.Clicked += OnCreateAccountClicked;
        _cancelButton.Clicked += OnCancelClicked;

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

        ApplyTexts();
        FocusFirstField();
    }

    protected override int RowPitch => RegisterRowSpacing;

    protected override string GetSubtitle()
    {
        return TextCatalog.RegisterSubtitle;
    }

    private void OnCreateAccountClicked(object? sender, EventArgs e)
    {
        _hasValidated = true;

        if (!Validate())
        {
            Popup.ShowAlert(Navigator, TextCatalog.RegisterCheckTheForm);
            return;
        }

        // The form checked what it can see; the server decides. A nickname
        // free a second ago may be taken by the time the message lands.
        Answer(_accounts.Register(Describe()));
    }

    // CU-02 asks for the birth date in three boxes; Validate already proved
    // they parse, so this reads them again without doubting them.
    private NewAccountRequest Describe()
    {
        InputRules.TryParseBirthDate(ReadBirthDate(), out DateOnly birthDate);

        return new NewAccountRequest
        {
            Nickname = _nicknameField.Text.Trim(),
            Email = _emailField.Text.Trim(),
            Password = _passwordField.Text,
            BirthDate = birthDate,
            AcceptsTerms = _termsCheckBox.IsChecked
        };
    }

    private BirthDateFields ReadBirthDate()
    {
        return new BirthDateFields
        {
            Day = _dayField.Text,
            Month = _monthField.Text,
            Year = _yearField.Text
        };
    }

    private void Answer(RegistrationAnswer answer)
    {
        if (answer == RegistrationAnswer.Registered)
        {
            Navigator.GoTo(ScreenId.RegistrationSuccess, _emailField.Text.Trim());
            return;
        }

        if (ShowOnTheField(answer))
        {
            return;
        }

        Popup.ShowError(Navigator, answer == RegistrationAnswer.Unreachable
            ? TextCatalog.RegisterServerUnreachable
            : TextCatalog.RegisterRejected);
    }

    // What the person can fix is said next to the box that holds it.
    private bool ShowOnTheField(RegistrationAnswer answer)
    {
        switch (answer)
        {
            case RegistrationAnswer.NicknameTaken:
                _nicknameField.Warning = TextCatalog.RegisterNicknameTaken;
                return true;
            case RegistrationAnswer.EmailTaken:
                _emailField.Warning = TextCatalog.RegisterEmailTaken;
                return true;
            case RegistrationAnswer.Underage:
                _dayField.Warning = TextCatalog.RegisterUnderage;
                return true;
            case RegistrationAnswer.TermsNotAccepted:
                _termsCheckBox.Warning = TextCatalog.RegisterTermsRequired;
                return true;
            default:
                return false;
        }
    }

    // Every field is checked, not only the first bad one, so the player fixes
    // the whole form in one pass (CU-02 FA-03 to FA-05 and FA-07). The
    // duplicate checks stand in for the server answer until it exists.
    private bool Validate()
    {
        _firstNameField.Warning = GetNameWarning(
            _firstNameField.Text, TextCatalog.RegisterFirstNameRequired, TextCatalog.RegisterFirstNameInvalid);
        _lastNameField.Warning = GetNameWarning(
            _lastNameField.Text, TextCatalog.RegisterLastNameRequired, TextCatalog.RegisterLastNameInvalid);
        _nicknameField.Warning = GetNicknameWarning(_nicknameField.Text.Trim());
        _emailField.Warning = GetEmailWarning(_emailField.Text.Trim());
        _passwordField.Warning = GetPasswordWarning();
        _confirmationField.Warning = GetConfirmationWarning();

        // Only the day box carries the text, or it would be drawn three times.
        _dayField.Warning = GetBirthDateWarning();
        _termsCheckBox.Warning = GetTermsWarning();

        return !GetCheckedControls().Any(control => control.HasWarning);
    }

    // The order is the reading order of the form, so a warning is answered
    // where the eye already is.
    private IEnumerable<Control> GetCheckedControls()
    {
        return
        [
            _firstNameField,
            _lastNameField,
            _nicknameField,
            _emailField,
            _passwordField,
            _confirmationField,
            _dayField,
            _termsCheckBox
        ];
    }

    // Two answers for one box: it is empty, or what it holds is not a name.
    private static string? GetNameWarning(string text, string whenEmpty, string whenMalformed)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return whenEmpty;
        }

        return InputRules.IsPersonName(text) ? null : whenMalformed;
    }

    // CU-02 RN-03 asks for length and for three of the four character
    // groups, and says which one is missing rather than both at once.
    private string? GetPasswordWarning()
    {
        if (!InputRules.HasPasswordLength(_passwordField.Text))
        {
            return TextCatalog.RegisterPasswordTooShort;
        }

        return InputRules.MeetsPasswordPolicy(_passwordField.Text) ? null : TextCatalog.RegisterPasswordPolicy;
    }

    private string? GetConfirmationWarning()
    {
        return _confirmationField.Text == _passwordField.Text
            ? null
            : TextCatalog.RegisterConfirmationMismatch;
    }

    private string? GetTermsWarning()
    {
        return _termsCheckBox.IsChecked ? null : TextCatalog.RegisterTermsRequired;
    }

    private string? GetNicknameWarning(string nickname)
    {
        if (!InputRules.HasNicknameLength(nickname))
        {
            return TextCatalog.RegisterNicknameLength;
        }

        if (!InputRules.IsNickname(nickname))
        {
            return TextCatalog.RegisterNicknameInvalid;
        }

        return _accounts.IsNicknameTaken(nickname) ? TextCatalog.RegisterNicknameTaken : null;
    }

    private string? GetEmailWarning(string email)
    {
        if (!InputRules.IsEmail(email))
        {
            return TextCatalog.RegisterEmailInvalid;
        }

        return _accounts.IsEmailTaken(email) ? TextCatalog.RegisterEmailTaken : null;
    }

    private string? GetBirthDateWarning()
    {
        if (!InputRules.TryParseBirthDate(ReadBirthDate(), out DateOnly date))
        {
            return TextCatalog.RegisterBirthDateInvalid;
        }

        if (InputRules.IsInFuture(date))
        {
            return TextCatalog.RegisterBirthDateFuture;
        }

        // D-06 and CU-02 RN-05. The server checks it again: this only saves
        // the round trip.
        return InputRules.IsAtLeastYearsOld(date, MinimumAge) ? null : TextCatalog.RegisterUnderage;
    }

    // CU-02 FA-01. Discarding also has to clear the two passwords and uncheck
    // the terms, which belongs to validation and is still pending.
    private void OnCancelClicked(object? sender, EventArgs e)
    {
        Popup.Ask(Navigator, new ConfirmRequest
        {
            Body = TextCatalog.RegisterDiscardBody,
            PrimaryLabel = TextCatalog.RegisterDiscardButton,
            SecondaryLabel = TextCatalog.RegisterKeepEditingButton,
            OnConfirm = GoBackToLogin
        });
    }

    private void GoBackToLogin()
    {
        Navigator.GoBack();
    }

    protected override void ApplyTexts()
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

        _termsCheckBox.Text = TextCatalog.RegisterTermsText;
        _termsCheckBox.LinkText = TextCatalog.RegisterTermsLinkText;

        _createButton.Title = TextCatalog.RegisterCreateButton;
        _createButton.Subtitle = TextCatalog.RegisterCreateButtonDetail;
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
            Bounds = new Rectangle(x, FirstRowTop + (BirthDateRow * RowPitch), width, Theme.FieldHeight)
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
