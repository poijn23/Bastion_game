using System;
using System.Globalization;
using Microsoft.Xna.Framework;
using Bastion.Presentation.GUI_Profile;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_EditProfile;

public sealed class GuiEditProfile : FormScreen
{
    private const int MaxLinkLength = 254;
    private const int RowAir = 4;
    private const int EditRowSpacing = LabelSpace + Theme.FieldHeight + Theme.WarningSpace + RowAir;
    private const int BottomPadding = Theme.WarningSpace + RowAir;
    private const int CardHeight =
        Theme.CardPadding + LabelSpace + (2 * EditRowSpacing) + Theme.FieldHeight + BottomPadding;

    private const int NicknameRow = 0;
    private const int TitleRow = 1;
    private const int LanguageRow = 2;

    private readonly ValueBox _nicknameBox;
    private readonly Selector _titleSelector;
    private readonly Selector _languageSelector;
    private readonly TextField _firstLinkField;
    private readonly TextField _secondLinkField;
    private readonly CheckBox _spectatorsCheckBox;
    private readonly Button _saveButton;
    private readonly Button _cancelButton;
    private bool _hasValidated;

    public GuiEditProfile(INavigator navigator)
        : base(navigator, WideCardWidth, CardHeight)
    {
        _nicknameBox = new ValueBox { Bounds = GetCell(NicknameRow, false) };
        _titleSelector = new Selector
        {
            Options = GuiProfile.GetTitleNames(),
            SelectedIndex = TestProfile.TitleIndex,
            Bounds = GetCell(TitleRow, false)
        };
        _languageSelector = new Selector
        {
            Options = LanguagePicker.GetNames(),
            SelectedIndex = GetLanguageIndex(TestProfile.PreferredLanguage),
            Bounds = GetCell(LanguageRow, false)
        };
        _firstLinkField = CreateLinkField(NicknameRow, TestProfile.FirstLink);
        _secondLinkField = CreateLinkField(TitleRow, TestProfile.SecondLink);
        _spectatorsCheckBox = new CheckBox
        {
            IsChecked = TestProfile.AllowsSpectators,
            Bounds = GetCell(LanguageRow, true)
        };

        _saveButton = CreatePrimaryButton(true);
        _cancelButton = CreateSecondaryButton();
        _saveButton.Clicked += OnSaveClicked;
        _cancelButton.Clicked += OnCancelClicked;

        Register(_nicknameBox);
        Register(_titleSelector);
        Register(_languageSelector);
        RegisterField(_firstLinkField);
        RegisterField(_secondLinkField);
        Register(_spectatorsCheckBox);
        Register(_saveButton);
        Register(_cancelButton);

        ApplyTexts();
        FocusFirstField();
    }

    protected override int RowPitch => EditRowSpacing;

    protected override string GetSubtitle()
    {
        return TextCatalog.EditProfileSubtitle;
    }

    protected override void ApplyTexts()
    {
        _nicknameBox.Label = TextCatalog.EditProfileNicknameLabel;
        _nicknameBox.Value = TestAccount.Nickname;
        _titleSelector.Label = TextCatalog.EditProfileTitleLabel;
        _titleSelector.Options = GuiProfile.GetTitleNames();
        _languageSelector.Label = TextCatalog.EditProfileLanguageLabel;
        _languageSelector.Options = LanguagePicker.GetNames();
        _firstLinkField.Label = TextCatalog.EditProfileFirstLinkLabel;
        _firstLinkField.Placeholder = TextCatalog.EditProfileLinkPlaceholder;
        _secondLinkField.Label = TextCatalog.EditProfileSecondLinkLabel;
        _secondLinkField.Placeholder = TextCatalog.EditProfileLinkPlaceholder;
        _spectatorsCheckBox.Text = TextCatalog.EditProfileSpectatorsText;
        _saveButton.Title = TextCatalog.CommonSaveButton;
        _cancelButton.Title = TextCatalog.CommonCancelButton;

        if (_hasValidated)
        {
            Validate();
        }
    }

    private void OnSaveClicked(object? sender, EventArgs e)
    {
        _hasValidated = true;

        if (!Validate())
        {
            return;
        }

        TestProfile.TitleIndex = _titleSelector.SelectedIndex;
        TestProfile.FirstLink = _firstLinkField.Text.Trim();
        TestProfile.SecondLink = _secondLinkField.Text.Trim();
        TestProfile.AllowsSpectators = _spectatorsCheckBox.IsChecked;

        CultureInfo chosen = _languageSelector.SelectedIndex == LanguagePicker.EnglishIndex
            ? Language.English
            : Language.SpanishMexico;

        if (!chosen.Equals(TestProfile.PreferredLanguage))
        {
            TestProfile.PreferredLanguage = chosen;
            LanguagePicker.Apply(_languageSelector.SelectedIndex);
        }

        Navigator.GoTo(ScreenId.Profile);
        Navigator.ShowMessage(DialogTone.Success, TextCatalog.EditProfileSavedBody);
    }

    private void OnCancelClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.Profile);
    }

    private bool Validate()
    {
        _firstLinkField.Warning = GetLinkWarning(_firstLinkField.Text.Trim());
        _secondLinkField.Warning = GetLinkWarning(_secondLinkField.Text.Trim());

        return !_firstLinkField.HasWarning && !_secondLinkField.HasWarning;
    }

    private static string? GetLinkWarning(string link)
    {
        if (link.Length == 0)
        {
            return null;
        }

        if (!InputRules.IsWebAddress(link))
        {
            return TextCatalog.EditProfileLinkInvalid;
        }

        return InputRules.IsShortener(link) ? TextCatalog.EditProfileLinkShortener : null;
    }

    private static int GetLanguageIndex(CultureInfo culture)
    {
        return culture.TwoLetterISOLanguageName == Language.English.TwoLetterISOLanguageName
            ? LanguagePicker.EnglishIndex
            : LanguagePicker.SpanishIndex;
    }

    private TextField CreateLinkField(int row, string value)
    {
        var field = new TextField { MaxLength = MaxLinkLength, Bounds = GetCell(row, true) };
        field.SetText(value);

        return field;
    }
}
