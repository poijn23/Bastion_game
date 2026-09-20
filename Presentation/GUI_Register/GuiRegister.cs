using System;
using Microsoft.Xna.Framework;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_Register;

// Step 1 of the CU-02 main flow. Presentation only: it does not validate, does
// not open a connection and does not build the REGISTER_REQUEST.
public sealed class GuiRegister : FormScreen
{
    private const int MaxNicknameLength = 30;
    private const int MaxEmailLength = 254;
    private const int MaxDayLength = 2;
    private const int MaxMonthLength = 2;
    private const int MaxYearLength = 4;

    private const int WideCardWidth = 760;
    private const int WideCardHeight = 378;
    private const int Gutter = 32;
    private const int CheckBoxHeight = 24;
    private const int CheckBoxGap = 28;

    private const int DayWidth = 92;
    private const int MonthWidth = 92;
    private const int DateGap = 12;

    private readonly TextField _nicknameField;
    private readonly TextField _emailField;
    private readonly TextField _passwordField;
    private readonly TextField _confirmationField;
    private readonly TextField _dayField;
    private readonly TextField _monthField;
    private readonly TextField _yearField;
    private readonly Selector _accountLanguageSelector;
    private readonly DropDown _interfaceLanguageDropDown;
    private readonly CheckBox _termsCheckBox;
    private readonly Button _createButton;
    private readonly Button _cancelButton;

    public GuiRegister(INavigator navigator)
        : base(navigator, WideCardWidth, WideCardHeight)
    {
        _nicknameField = CreateLeftField(0, MaxNicknameLength);
        _emailField = CreateLeftField(1, MaxEmailLength);
        _passwordField = CreatePasswordField(0);
        _confirmationField = CreatePasswordField(1);
        _dayField = CreateDatePart(ContentX, DayWidth, MaxDayLength);
        _monthField = CreateDatePart(ContentX + DayWidth + DateGap, MonthWidth, MaxMonthLength);
        _yearField = CreateDatePart(ContentX + DayWidth + MonthWidth + (DateGap * 2), GetYearWidth(), MaxYearLength);
        _accountLanguageSelector = CreateAccountLanguageSelector();
        _interfaceLanguageDropDown = LanguagePicker.Create();
        _termsCheckBox = CreateTermsCheckBox();
        _createButton = CreatePrimaryButton(true);
        _cancelButton = CreateSecondaryButton();

        _createButton.Clicked += OnCreateAccountClicked;
        _cancelButton.Clicked += OnCancelClicked;
        _interfaceLanguageDropDown.SelectionChanged += OnInterfaceLanguageChanged;

        RegisterField(_nicknameField);
        RegisterField(_passwordField);
        RegisterField(_emailField);
        RegisterField(_confirmationField);
        RegisterField(_dayField);
        RegisterField(_monthField);
        RegisterField(_yearField);
        Register(_accountLanguageSelector);
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
    }

    // Left empty until validation and the connection exist.
    private void OnCreateAccountClicked(object? sender, EventArgs e)
    {
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

        _accountLanguageSelector.Label = TextCatalog.RegisterAccountLanguageLabel;
        _accountLanguageSelector.Options = LanguagePicker.GetNames();
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

        return new Rectangle(x, FirstRowTop + (row * RowSpacing), columnWidth, Theme.FieldHeight);
    }

    private TextField CreateLeftField(int row, int maxLength)
    {
        return new TextField
        {
            MaxLength = maxLength,
            Bounds = GetCell(row, false)
        };
    }

    private TextField CreatePasswordField(int row)
    {
        return new TextField
        {
            IsPassword = true,
            Bounds = GetCell(row, true)
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
            Bounds = new Rectangle(x, FirstRowTop + (2 * RowSpacing), width, Theme.FieldHeight)
        };
    }

    private Selector CreateAccountLanguageSelector()
    {
        return new Selector
        {
            Options = LanguagePicker.GetNames(),
            SelectedIndex = GetStartingLanguageIndex(),
            Bounds = GetCell(2, true)
        };
    }

    private CheckBox CreateTermsCheckBox()
    {
        int top = FirstRowTop + (2 * RowSpacing) + Theme.FieldHeight + CheckBoxGap;

        return new CheckBox
        {
            Bounds = new Rectangle(ContentX, top, ContentWidth, CheckBoxHeight)
        };
    }

    private static int GetStartingLanguageIndex()
    {
        return Language.IsEnglish ? LanguagePicker.EnglishIndex : LanguagePicker.SpanishIndex;
    }
}
