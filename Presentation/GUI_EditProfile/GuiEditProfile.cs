using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Bastion.Presentation.GUI_Profile;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_EditProfile;

public sealed class GuiEditProfile : FormScreen
{
    private const int AvatarSize = 64;
    private const int IconRows = 2;
    private const int IconRowGap = 8;
    private const int AvatarGap = 16;
    private const int SmallButtonWidth = 140;
    private const int SmallButtonGap = 10;
    private const int HintHeight = 20;
    private const int SectionGap = 18;
    private const int ChangeButtonWidth = 120;
    private const int PreviewHeight = 64;
    private const int PreviewAvatar = 40;
    private const int ToggleHeight = 40;
    private const int LinkRowHeight = 46;
    private const int LinkRowGap = 8;
    private const int LinkTypeWidth = 130;
    private const int LinkRemoveWidth = 46;
    private const int MaxLinkLength = 254;
    private const int ButtonsGap = 24;

    private const int IconsRowTop = AvatarSize + 12;
    private const int IconsRowsHeight = (IconRows * Theme.ChipHeight) + ((IconRows - 1) * IconRowGap);
    private const int IconsHintTop = IconsRowTop + IconsRowsHeight + 8;
    private const int AvatarBlockHeight = IconsHintTop + HintHeight;
    private const int NameLabelTop = AvatarBlockHeight + SectionGap;
    private const int NameFieldTop = NameLabelTop + LabelSpace;
    private const int NameHintTop = NameFieldTop + Theme.FieldHeight + 6;
    private const int TitleLabelTop = NameHintTop + HintHeight + SectionGap;
    private const int TitleChipsTop = TitleLabelTop + LabelSpace;
    private const int PreviewTop = TitleChipsTop + Theme.ChipHeight + SectionGap;
    private const int LeftHeight = PreviewTop + PreviewHeight;

    private const int LanguageLabelTop = ToggleHeight + SectionGap;
    private const int LanguageFieldTop = LanguageLabelTop + LabelSpace;
    private const int LinksLabelTop = LanguageFieldTop + Theme.FieldHeight + SectionGap;
    private const int LinksRowsTop = LinksLabelTop + LabelSpace;
    private const int LinksHintTop = LinksRowsTop + (TestProfile.MaxLinks * (LinkRowHeight + LinkRowGap));
    private const int RightHeight = LinksHintTop + HintHeight;

    private const int ContentHeight = RightHeight > LeftHeight ? RightHeight : LeftHeight;
    private const int CardHeight =
        Theme.CardPadding + Theme.PanelBackHeight + Theme.PanelGap + Theme.PanelTitleHeight + Theme.PanelGap
        + ContentHeight + ButtonsGap + Theme.PanelButtonHeight + Theme.CardPadding;

    private readonly Avatar _avatar;
    private readonly Button _uploadButton;
    private readonly Button _iconsButton;
    private readonly ChipRow _iconChips;
    private readonly TextLine _iconsHint;
    private readonly ValueBox _nicknameBox;
    private readonly Button _changeNicknameButton;
    private readonly TextLine _nicknameHint;
    private readonly TextLine _titleLabel;
    private readonly ChipRow _titleChips;
    private readonly PanelBox _previewBox;
    private readonly Avatar _previewAvatar;
    private readonly TextLine _previewLine;
    private readonly TextLine _previewHint;
    private readonly ToggleSwitch _spectatorsSwitch;
    private readonly Selector _languageSelector;
    private readonly TextLine _linksLabel;
    private readonly List<DropDown> _linkTypes = [];
    private readonly List<TextField> _linkFields = [];
    private readonly List<Button> _linkRemoveButtons = [];
    private readonly TextLine _linksHint;
    private readonly Button _saveButton;
    private readonly Button _cancelButton;
    private int _titleIndex;
    private int _iconIndex;
    private bool _hasValidated;

    public GuiEditProfile(INavigator navigator)
        : base(navigator, Theme.PanelCardWidth, CardHeight, ScreenLayout.Panel)
    {
        int top = PanelContentTop;
        int leftX = ContentX;
        int rightX = ContentX + ColumnWidth + Gutter;
        int besideAvatar = leftX + AvatarSize + AvatarGap;
        _titleIndex = TestProfile.TitleIndex;
        _iconIndex = TestProfile.IconIndex;

        _avatar = new Avatar { Bounds = new Rectangle(leftX, top, AvatarSize, AvatarSize) };
        int buttonsTop = top + ((AvatarSize - Theme.SmallButtonHeight) / 2);
        _uploadButton = CreateOutlineButton(new Rectangle(besideAvatar, buttonsTop, SmallButtonWidth, Theme.SmallButtonHeight));
        _uploadButton.IsEnabled = false;
        _iconsButton = CreateOutlineButton(
            new Rectangle(besideAvatar + SmallButtonWidth + SmallButtonGap, buttonsTop, SmallButtonWidth, Theme.SmallButtonHeight));
        _iconChips = new ChipRow { Bounds = new Rectangle(leftX, top + IconsRowTop, ColumnWidth, IconsRowsHeight) };
        _iconChips.ChipChosen += OnIconChosen;
        _iconsHint = new TextLine
        {
            Style = TextLineStyle.Small,
            Bounds = new Rectangle(leftX, top + IconsHintTop, ColumnWidth, HintHeight)
        };

        _nicknameBox = new ValueBox
        {
            Bounds = new Rectangle(leftX, top + NameFieldTop, ColumnWidth - ChangeButtonWidth - SmallButtonGap, Theme.FieldHeight)
        };
        _changeNicknameButton = CreateOutlineButton(
            new Rectangle(leftX + ColumnWidth - ChangeButtonWidth, top + NameFieldTop, ChangeButtonWidth, Theme.FieldHeight));
        _changeNicknameButton.IsEnabled = !TestProfile.HasChangedNickname;
        _changeNicknameButton.Clicked += OnChangeNicknameClicked;
        _nicknameHint = new TextLine
        {
            Style = TextLineStyle.Small,
            Bounds = new Rectangle(leftX, top + NameHintTop, ColumnWidth, HintHeight)
        };

        _titleLabel = new TextLine
        {
            Style = TextLineStyle.Label,
            Bounds = new Rectangle(leftX, top + TitleLabelTop, ColumnWidth, LabelSpace)
        };
        _titleChips = new ChipRow { Bounds = new Rectangle(leftX, top + TitleChipsTop, ColumnWidth, Theme.ChipHeight) };
        _titleChips.ChipChosen += OnTitleChosen;

        var previewArea = new Rectangle(leftX, top + PreviewTop, ColumnWidth, PreviewHeight);
        _previewBox = new PanelBox { Bounds = previewArea };
        _previewAvatar = new Avatar
        {
            Bounds = new Rectangle(previewArea.X + 14, previewArea.Y + ((PreviewHeight - PreviewAvatar) / 2), PreviewAvatar, PreviewAvatar)
        };
        _previewLine = new TextLine
        {
            Bounds = new Rectangle(previewArea.X + 14 + PreviewAvatar + 14, previewArea.Y + 12, ColumnWidth, 22)
        };
        _previewHint = new TextLine
        {
            Style = TextLineStyle.Small,
            Bounds = new Rectangle(previewArea.X + 14 + PreviewAvatar + 14, previewArea.Y + 36, ColumnWidth, HintHeight)
        };

        _spectatorsSwitch = new ToggleSwitch
        {
            IsOn = TestProfile.AllowsSpectators,
            Bounds = new Rectangle(rightX, top, ColumnWidth, ToggleHeight)
        };
        _languageSelector = new Selector
        {
            Options = LanguagePicker.GetNames(),
            SelectedIndex = GetLanguageIndex(TestProfile.PreferredLanguage),
            Bounds = new Rectangle(rightX, top + LanguageFieldTop, ColumnWidth, Theme.FieldHeight)
        };
        _linksLabel = new TextLine
        {
            Style = TextLineStyle.Label,
            Bounds = new Rectangle(rightX, top + LinksLabelTop, ColumnWidth, LabelSpace)
        };

        for (int i = 0; i < TestProfile.MaxLinks; i++)
        {
            int rowTop = top + LinksRowsTop + (i * (LinkRowHeight + LinkRowGap));
            int fieldX = rightX + LinkTypeWidth + SmallButtonGap;
            int fieldWidth = ColumnWidth - LinkTypeWidth - LinkRemoveWidth - (SmallButtonGap * 2);

            var type = new DropDown
            {
                Options = GuiProfile.GetLinkTypeNames(),
                Bounds = new Rectangle(rightX, rowTop, LinkTypeWidth, LinkRowHeight)
            };
            var field = new TextField { MaxLength = MaxLinkLength, Bounds = new Rectangle(fieldX, rowTop, fieldWidth, LinkRowHeight) };
            Button remove = CreateOutlineButton(new Rectangle(fieldX + fieldWidth + SmallButtonGap, rowTop, LinkRemoveWidth, LinkRowHeight));
            int index = i;
            remove.Clicked += (_, _) => RemoveLink(index);

            _linkTypes.Add(type);
            _linkFields.Add(field);
            _linkRemoveButtons.Add(remove);
        }

        _linksHint = new TextLine
        {
            Style = TextLineStyle.Small,
            Bounds = new Rectangle(rightX, top + LinksHintTop, ColumnWidth, HintHeight)
        };

        _saveButton = CreatePrimaryButton(false);
        _cancelButton = CreateOutlineButton(SecondaryButtonBounds);
        _saveButton.Clicked += OnSaveClicked;
        _cancelButton.Clicked += OnCancelClicked;

        Register(_avatar);
        Register(_uploadButton);
        Register(_iconsButton);
        Register(_iconChips);
        Register(_iconsHint);
        Register(_nicknameBox);
        Register(_changeNicknameButton);
        Register(_nicknameHint);
        Register(_titleLabel);
        Register(_titleChips);
        Register(_previewBox);
        Register(_previewAvatar);
        Register(_previewLine);
        Register(_previewHint);
        Register(_spectatorsSwitch);
        Register(_languageSelector);
        Register(_linksLabel);

        for (int i = 0; i < TestProfile.MaxLinks; i++)
        {
            RegisterField(_linkFields[i]);
            Register(_linkRemoveButtons[i]);
        }

        Register(_linksHint);
        Register(_saveButton);
        Register(_cancelButton);

        for (int i = 0; i < TestProfile.MaxLinks; i++)
        {
            Register(_linkTypes[i]);
        }

        LoadLinks();
        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.EditProfileSubtitle;
    }

    public override void Update(InputState input)
    {
        base.Update(input);
        ShowNextEmptyLinkRow();
    }

    protected override void ApplyTexts()
    {
        _avatar.Text = TextCatalog.AvatarPlaceholder;
        _previewAvatar.Text = TextCatalog.AvatarPlaceholder;
        _uploadButton.Title = TextCatalog.EditProfileUploadButton;
        _iconsButton.Title = TextCatalog.EditProfileIconsButton;
        _iconsHint.Text = TextCatalog.EditProfileIconsHint;
        RefreshIconChips();

        _nicknameBox.Label = TextCatalog.EditProfileNicknameLabel;
        _nicknameBox.Value = TestAccount.Nickname;
        _changeNicknameButton.Title = TextCatalog.CommonChangeButton;
        _nicknameHint.Text = TextCatalog.EditProfileNicknameHint;

        _titleLabel.Text = TextCatalog.EditProfileTitleLabel;
        RefreshTitleChips();
        RefreshPreview();
        _previewHint.Text = TextCatalog.EditProfilePreviewHint;

        _spectatorsSwitch.Text = TextCatalog.EditProfileSpectatorsText;
        _languageSelector.Label = TextCatalog.EditProfileLanguageLabel;
        _languageSelector.Options = LanguagePicker.GetNames();
        _linksLabel.Text = TextCatalog.EditProfileLinksLabel;
        IReadOnlyList<string> types = GuiProfile.GetLinkTypeNames();

        for (int i = 0; i < TestProfile.MaxLinks; i++)
        {
            _linkTypes[i].Options = types;
            _linkFields[i].Placeholder = TextCatalog.EditProfileLinkPlaceholder;
            _linkRemoveButtons[i].Title = TextCatalog.EditProfileRemoveLinkButton;
        }

        _linksHint.Text = TextCatalog.EditProfileLinksHint;
        _saveButton.Title = TextCatalog.CommonSaveButton;
        _cancelButton.Title = TextCatalog.CommonCancelButton;

        if (_hasValidated)
        {
            Validate();
        }
    }

    private void OnIconChosen(object? sender, SelectionChangedEventArgs e)
    {
        if (e.SelectedIndex < _iconChips.Items.Count - 1)
        {
            _iconIndex = e.SelectedIndex;
            RefreshIconChips();
        }
    }

    private void OnTitleChosen(object? sender, SelectionChangedEventArgs e)
    {
        _titleIndex = e.SelectedIndex < GuiProfile.GetTitleNames().Count ? e.SelectedIndex : -1;
        RefreshTitleChips();
        RefreshPreview();
    }

    private void OnChangeNicknameClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.ChangeNickname);
    }

    private void OnSaveClicked(object? sender, EventArgs e)
    {
        _hasValidated = true;

        if (!Validate())
        {
            return;
        }

        TestProfile.TitleIndex = _titleIndex;
        TestProfile.IconIndex = _iconIndex;
        TestProfile.AllowsSpectators = _spectatorsSwitch.IsOn;
        TestProfile.Links.Clear();

        for (int i = 0; i < TestProfile.MaxLinks; i++)
        {
            string url = _linkFields[i].Text.Trim();

            if (_linkFields[i].IsVisible && url.Length > 0)
            {
                TestProfile.Links.Add((_linkTypes[i].SelectedIndex, url));
            }
        }

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
        bool isValid = true;

        for (int i = 0; i < TestProfile.MaxLinks; i++)
        {
            _linkFields[i].Warning = _linkFields[i].IsVisible ? GetLinkWarning(_linkFields[i].Text.Trim()) : null;
            isValid &= !_linkFields[i].HasWarning;
        }

        return isValid;
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

    private void LoadLinks()
    {
        for (int i = 0; i < TestProfile.MaxLinks; i++)
        {
            bool has = i < TestProfile.Links.Count;
            _linkTypes[i].SelectedIndex = has ? TestProfile.Links[i].TypeIndex : GuiProfile.GetLinkTypeNames().Count - 1;
            _linkFields[i].SetText(has ? TestProfile.Links[i].Url : string.Empty);
            SetLinkRowVisible(i, has);
        }

        ShowNextEmptyLinkRow();
    }

    private void ShowNextEmptyLinkRow()
    {
        for (int i = 0; i < TestProfile.MaxLinks; i++)
        {
            if (!_linkFields[i].IsVisible)
            {
                bool previousFilled = i == 0 || _linkFields[i - 1].Text.Trim().Length > 0;

                if (previousFilled)
                {
                    SetLinkRowVisible(i, true);
                }

                break;
            }
        }

        for (int i = 0; i < TestProfile.MaxLinks; i++)
        {
            bool filled = _linkFields[i].IsVisible && _linkFields[i].Text.Trim().Length > 0;
            SetVisible(_linkRemoveButtons[i], filled);
        }
    }

    private void RemoveLink(int index)
    {
        for (int i = index; i < TestProfile.MaxLinks - 1; i++)
        {
            _linkFields[i].SetText(_linkFields[i + 1].Text);
            _linkTypes[i].SelectedIndex = _linkTypes[i + 1].SelectedIndex;
            _linkFields[i].Warning = _linkFields[i + 1].Warning;
        }

        int last = TestProfile.MaxLinks - 1;
        _linkFields[last].SetText(string.Empty);
        _linkFields[last].Warning = null;

        for (int i = TestProfile.MaxLinks - 1; i > 0; i--)
        {
            if (_linkFields[i].IsVisible && _linkFields[i].Text.Length == 0 && _linkFields[i - 1].Text.Length == 0)
            {
                SetLinkRowVisible(i, false);
            }
        }
    }

    private void SetLinkRowVisible(int index, bool isVisible)
    {
        SetVisible(_linkTypes[index], isVisible);
        SetVisible(_linkFields[index], isVisible);
        SetVisible(_linkRemoveButtons[index], isVisible);
    }

    private static void SetVisible(Control control, bool isVisible)
    {
        if (isVisible)
        {
            control.Show();
        }
        else
        {
            control.Hide();
        }
    }

    private void RefreshIconChips()
    {
        _iconChips.Items.Clear();
        string[] icons =
        [
            TextCatalog.IconoZorro, TextCatalog.IconoBuho, TextCatalog.IconoOso, TextCatalog.IconoGato,
            TextCatalog.IconoMuro, TextCatalog.IconoPeon, TextCatalog.IconoFaro
        ];

        for (int i = 0; i < icons.Length; i++)
        {
            _iconChips.Items.Add(new Chip { Text = icons[i], IsSelected = i == _iconIndex });
        }

        _iconChips.Items.Add(new Chip { Text = TextCatalog.EditProfileMoreIconsChip, IsDashed = true });
    }

    private void RefreshTitleChips()
    {
        _titleChips.Items.Clear();
        IReadOnlyList<string> titles = GuiProfile.GetTitleNames();

        for (int i = 0; i < titles.Count; i++)
        {
            _titleChips.Items.Add(new Chip { Text = titles[i], IsSelected = i == _titleIndex });
        }

        _titleChips.Items.Add(new Chip { Text = TextCatalog.ProfileNoTitle, IsDashed = _titleIndex >= 0, IsSelected = _titleIndex < 0 });
    }

    private void RefreshPreview()
    {
        IReadOnlyList<string> titles = GuiProfile.GetTitleNames();
        string title = _titleIndex >= 0 && _titleIndex < titles.Count ? titles[_titleIndex] : TextCatalog.ProfileNoTitle;
        _previewLine.Text = string.Format(TextCatalog.EditProfilePreviewFormat, TestAccount.Nickname, title);
    }
}
