using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_Register;

// Step 1 of the CU-02 main flow. Presentation only: it does not validate, does
// not open a connection and does not build the REGISTER_REQUEST.
public sealed class GuiRegister
{
    private const int SpanishIndex = 0;
    private const int EnglishIndex = 1;

    private const int MaxNicknameLength = 30;
    private const int MaxEmailLength = 254;
    private const int MaxDayLength = 2;
    private const int MaxMonthLength = 2;
    private const int MaxYearLength = 4;

    private const int CardWidth = 760;
    private const int CardX = (Theme.WindowWidth - CardWidth) / 2;
    private const int CardY = 144;
    private const int CardHeight = 378;

    private const int ContentX = CardX + Theme.CardPadding;
    private const int ContentWidth = CardWidth - (Theme.CardPadding * 2);
    private const int Gutter = 32;
    private const int ColumnWidth = (ContentWidth - Gutter) / 2;
    private const int RightColumnX = ContentX + ColumnWidth + Gutter;

    // Row spacing leaves room for the warning line that FA-03 and FA-07 require
    // next to a rejected field; with less it overlaps the next label.
    private const int RowSpacing = 98;
    private const int FirstRowY = CardY + Theme.CardPadding + 22;
    private const int SecondRowY = FirstRowY + RowSpacing;
    private const int ThirdRowY = SecondRowY + RowSpacing;
    private const int CheckBoxRowY = ThirdRowY + Theme.FieldHeight + 28;
    private const int CheckBoxHeight = 24;

    private const int DayWidth = 92;
    private const int MonthWidth = 92;
    private const int YearWidth = ColumnWidth - DayWidth - MonthWidth - 24;

    private const int DropDownMargin = 24;
    private const int DropDownWidth = 210;
    private const int DropDownHeight = 40;

    private const int PrimaryButtonY = CardY + CardHeight + 22;
    private const int SecondaryButtonY = PrimaryButtonY + Theme.PrimaryButtonHeight + 12;

    private const int TitleY = 52;
    private const int SubtitleY = 112;
    private const float SubtitleTracking = 4f;
    private const int OrnamentArm = 15;

    private static readonly Point[] _ornaments =
    [
        new(96, 108), new(1122, 88), new(72, 372), new(1180, 420),
        new(152, 516), new(1060, 664), new(620, 40)
    ];

    private readonly List<Control> _controls = [];
    private readonly List<TextField> _focusableFields = [];
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

    public GuiRegister()
    {
        _nicknameField = CreateNicknameField();
        _emailField = CreateEmailField();
        _passwordField = CreatePasswordField();
        _confirmationField = CreateConfirmationField();
        _dayField = CreateDayField();
        _monthField = CreateMonthField();
        _yearField = CreateYearField();
        _accountLanguageSelector = CreateAccountLanguageSelector();
        _interfaceLanguageDropDown = CreateInterfaceLanguageDropDown();
        _termsCheckBox = CreateTermsCheckBox();
        _createButton = CreateCreateButton();
        _cancelButton = CreateCancelButton();

        _createButton.Clicked += OnCreateAccountClicked;
        _cancelButton.Clicked += OnCancelClicked;
        _interfaceLanguageDropDown.SelectionChanged += OnInterfaceLanguageChanged;

        _focusableFields.AddRange([
            _nicknameField, _passwordField, _emailField, _confirmationField,
            _dayField, _monthField, _yearField
        ]);

        // The drawing order puts the drop down last so its open list stays on top.
        _controls.AddRange([
            _nicknameField, _emailField, _passwordField, _confirmationField,
            _dayField, _monthField, _yearField,
            _accountLanguageSelector, _termsCheckBox,
            _createButton, _cancelButton,
            _interfaceLanguageDropDown
        ]);

        ApplyTexts();
        _nicknameField.IsFocused = true;
    }

    public void Update(InputState input)
    {
        ArgumentNullException.ThrowIfNull(input);

        if (input.HasClicked)
        {
            ResolveFocus(input);
        }

        if (input.IsKeyNewlyPressed(Keys.Tab))
        {
            MoveFocus(IsShiftPressed(input));
        }

        foreach (Control control in _controls)
        {
            control.Update(input);
        }
    }

    public void Draw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        DrawBackground(canvas);
        DrawHeader(canvas);

        var card = new Rectangle(CardX, CardY, CardWidth, CardHeight);
        canvas.Shapes.DrawRoundedRectangle(card, Theme.CardCornerRadius, Theme.Card);

        foreach (Control control in _controls)
        {
            control.Draw(canvas);
        }
    }

    // Reloads the catalog and redraws. What the player typed is kept, because
    // only labels and hints are touched here (CU-02 FA-02 steps 1 and 2).
    private void OnInterfaceLanguageChanged(object? sender, SelectionChangedEventArgs e)
    {
        Language.Apply(e.SelectedIndex == EnglishIndex ? Language.English : Language.SpanishMexico);
        ApplyTexts();
    }

    // Left empty until validation and the connection exist.
    private void OnCreateAccountClicked(object? sender, EventArgs e)
    {
    }

    // Left empty until GUI_MessageConfirm exists (CU-02 FA-01).
    private void OnCancelClicked(object? sender, EventArgs e)
    {
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
        _accountLanguageSelector.Options = GetLanguageNames();
        _interfaceLanguageDropDown.Options = GetLanguageNames();

        _termsCheckBox.Text = TextCatalog.RegisterTermsText;
        _termsCheckBox.LinkText = TextCatalog.RegisterTermsLinkText;

        _createButton.Title = TextCatalog.RegisterCreateButton;
        _createButton.Subtitle = TextCatalog.RegisterCreateButtonDetail;
        _cancelButton.Title = TextCatalog.RegisterCancelButton;
    }

    private void ResolveFocus(InputState input)
    {
        foreach (TextField field in _focusableFields)
        {
            field.IsFocused = field.Bounds.Contains(input.MousePosition);
        }
    }

    private void MoveFocus(bool isBackwards)
    {
        int current = _focusableFields.FindIndex(IsFieldFocused);
        int step = isBackwards ? -1 : 1;
        int next = current < 0 ? 0 : (current + step + _focusableFields.Count) % _focusableFields.Count;

        for (int i = 0; i < _focusableFields.Count; i++)
        {
            _focusableFields[i].IsFocused = i == next;
        }
    }

    private static bool IsFieldFocused(TextField field)
    {
        return field.IsFocused;
    }

    private static bool IsShiftPressed(InputState input)
    {
        return input.IsKeyPressed(Keys.LeftShift) || input.IsKeyPressed(Keys.RightShift);
    }

    private static IReadOnlyList<string> GetLanguageNames()
    {
        return [TextCatalog.SpanishMexicoLanguageName, TextCatalog.EnglishLanguageName];
    }

    private static int GetStartingLanguageIndex()
    {
        return Language.IsEnglish ? EnglishIndex : SpanishIndex;
    }

    private static void DrawBackground(Canvas canvas)
    {
        foreach (Point ornament in _ornaments)
        {
            var horizontal = new Rectangle(ornament.X - 7, ornament.Y - 1, OrnamentArm, 2);
            var vertical = new Rectangle(ornament.X - 1, ornament.Y - 7, 2, OrnamentArm);
            canvas.Shapes.DrawRectangle(horizontal, Theme.BackgroundOrnament);
            canvas.Shapes.DrawRectangle(vertical, Theme.BackgroundOrnament);
        }
    }

    private static void DrawHeader(Canvas canvas)
    {
        TextStyle titleStyle = TextStyleFactory.CreateTitle(canvas.Fonts, Theme.TextLight);
        string title = TextCatalog.GameTitle;
        float titleWidth = canvas.Text.Measure(title, titleStyle);
        float titleX = MathF.Round((Theme.WindowWidth - titleWidth) / 2f);
        canvas.Text.Draw(title, new Vector2(titleX, TitleY), titleStyle);

        TextStyle subtitleStyle = TextStyleFactory.CreateLabel(canvas.Fonts, Theme.TextMuted);
        subtitleStyle = subtitleStyle with { Tracking = SubtitleTracking };
        string subtitle = TextCatalog.RegisterSubtitle;
        float subtitleWidth = canvas.Text.Measure(subtitle, subtitleStyle);
        float subtitleX = MathF.Round((Theme.WindowWidth - subtitleWidth) / 2f);
        canvas.Text.Draw(subtitle, new Vector2(subtitleX, SubtitleY), subtitleStyle);
    }

    private static TextField CreateNicknameField()
    {
        return new TextField
        {
            MaxLength = MaxNicknameLength,
            Bounds = new Rectangle(ContentX, FirstRowY, ColumnWidth, Theme.FieldHeight)
        };
    }

    private static TextField CreateEmailField()
    {
        return new TextField
        {
            MaxLength = MaxEmailLength,
            Bounds = new Rectangle(ContentX, SecondRowY, ColumnWidth, Theme.FieldHeight)
        };
    }

    private static TextField CreatePasswordField()
    {
        return new TextField
        {
            IsPassword = true,
            Bounds = new Rectangle(RightColumnX, FirstRowY, ColumnWidth, Theme.FieldHeight)
        };
    }

    private static TextField CreateConfirmationField()
    {
        return new TextField
        {
            IsPassword = true,
            Bounds = new Rectangle(RightColumnX, SecondRowY, ColumnWidth, Theme.FieldHeight)
        };
    }

    // Three boxes instead of one field: clearer for an eight year old player
    // (CON-12) and it needs no format to be explained.
    private static TextField CreateDayField()
    {
        return new TextField
        {
            MaxLength = MaxDayLength,
            IsCentered = true,
            Bounds = new Rectangle(ContentX, ThirdRowY, DayWidth, Theme.FieldHeight)
        };
    }

    private static TextField CreateMonthField()
    {
        return new TextField
        {
            MaxLength = MaxMonthLength,
            IsCentered = true,
            Bounds = new Rectangle(ContentX + DayWidth + 12, ThirdRowY, MonthWidth, Theme.FieldHeight)
        };
    }

    private static TextField CreateYearField()
    {
        return new TextField
        {
            MaxLength = MaxYearLength,
            IsCentered = true,
            Bounds = new Rectangle(ContentX + DayWidth + MonthWidth + 24, ThirdRowY, YearWidth, Theme.FieldHeight)
        };
    }

    private static Selector CreateAccountLanguageSelector()
    {
        return new Selector
        {
            Options = GetLanguageNames(),
            SelectedIndex = GetStartingLanguageIndex(),
            Bounds = new Rectangle(RightColumnX, ThirdRowY, ColumnWidth, Theme.FieldHeight)
        };
    }

    private static DropDown CreateInterfaceLanguageDropDown()
    {
        return new DropDown
        {
            Options = GetLanguageNames(),
            SelectedIndex = GetStartingLanguageIndex(),
            Bounds = new Rectangle(
                DropDownMargin,
                Theme.WindowHeight - DropDownMargin - DropDownHeight,
                DropDownWidth,
                DropDownHeight)
        };
    }

    private static CheckBox CreateTermsCheckBox()
    {
        return new CheckBox
        {
            Bounds = new Rectangle(ContentX, CheckBoxRowY, ContentWidth, CheckBoxHeight)
        };
    }

    private static Button CreateCreateButton()
    {
        return new Button
        {
            Style = ButtonStyle.Primary,
            HasArrow = true,
            Bounds = new Rectangle(CardX, PrimaryButtonY, CardWidth, Theme.PrimaryButtonHeight)
        };
    }

    private static Button CreateCancelButton()
    {
        return new Button
        {
            Style = ButtonStyle.Secondary,
            Bounds = new Rectangle(CardX, SecondaryButtonY, CardWidth, Theme.SecondaryButtonHeight)
        };
    }
}
