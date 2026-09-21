using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_Settings;

public sealed class GuiSettings : FormScreen
{
    private const int CardHeight = Theme.WindowHeight - (Theme.PanelTop * 2);
    private const int SidebarWidth = 220;
    private const int SidebarGap = 60;
    private const int HeadingHeight = 34;
    private const int HeadingGap = 16;
    private const int TopBoxHeight = 96;
    private const int TopBoxAvatar = 56;
    private const int EditButtonWidth = 100;
    private const int BlockGap = 16;
    private const int RowHeight = 70;
    private const int SignOutHeight = 56;
    private const int SignOutGap = 24;
    private const int DeleteBoxHeight = 76;
    private const int LanguageRowHeight = 56;
    private const int LanguageLabelHeight = 22;
    private const int LanguageRowsGap = 10;

    private const int AccountIndex = 2;
    private const int LanguageIndex = 3;

    private readonly SidebarMenu _sidebar;
    private readonly TextLine _heading;
    private readonly List<Control> _accountControls = [];
    private readonly List<Control> _languageControls = [];

    private readonly PanelBox _topBox;
    private readonly Avatar _avatar;
    private readonly TextLine _nickname;
    private readonly TextLine _email;
    private readonly Button _editButton;
    private readonly SettingRow _passwordRow;
    private readonly SettingRow _emailRow;
    private readonly SettingRow _sessionsRow;
    private readonly SettingRow _friendCodeRow;
    private readonly Button _signOutButton;
    private readonly DashedBox _deleteBox;

    private readonly TextLine _languageLabel;
    private readonly RadioRow _spanishRow;
    private readonly RadioRow _englishRow;
    private readonly TextLine _chatNote;

    public GuiSettings(INavigator navigator, bool showsLanguage)
        : base(navigator, Theme.PanelCardWidth, CardHeight, ScreenLayout.PanelBare)
    {
        int top = PanelContentTop;
        int panelX = ContentX + SidebarWidth + SidebarGap;
        int panelWidth = ContentWidth - SidebarWidth - SidebarGap;

        _heading = new TextLine { Style = TextLineStyle.Heading, Bounds = new Rectangle(ContentX, top, SidebarWidth, HeadingHeight) };
        _sidebar = new SidebarMenu
        {
            SelectedIndex = showsLanguage ? LanguageIndex : AccountIndex,
            Bounds = new Rectangle(ContentX, top + HeadingHeight + HeadingGap, SidebarWidth, 5 * (SidebarMenu.ItemHeight + 4))
        };
        _sidebar.Items.Add(new SidebarItem { IsEnabled = false });
        _sidebar.Items.Add(new SidebarItem { IsEnabled = false });
        _sidebar.Items.Add(new SidebarItem());
        _sidebar.Items.Add(new SidebarItem());
        _sidebar.Items.Add(new SidebarItem { IsEnabled = false });
        _sidebar.ItemChosen += OnSidebarChosen;

        var topArea = new Rectangle(panelX, top, panelWidth, TopBoxHeight);
        _topBox = new PanelBox { Bounds = topArea };
        _avatar = new Avatar { Bounds = new Rectangle(topArea.X + PanelBox.Padding, topArea.Y + ((TopBoxHeight - TopBoxAvatar) / 2), TopBoxAvatar, TopBoxAvatar) };
        int textX = topArea.X + PanelBox.Padding + TopBoxAvatar + 18;
        _nickname = new TextLine { Bounds = new Rectangle(textX, topArea.Y + 24, panelWidth, 24) };
        _email = new TextLine { Style = TextLineStyle.Small, Bounds = new Rectangle(textX, topArea.Y + 52, panelWidth, 20) };
        _editButton = CreateOutlineButton(new Rectangle(
            topArea.Right - PanelBox.Padding - EditButtonWidth, topArea.Y + ((TopBoxHeight - Theme.SmallButtonHeight) / 2), EditButtonWidth, Theme.SmallButtonHeight));
        _editButton.Clicked += OnEditClicked;
        _avatar.Clicked += (_, _) => Navigator.GoTo(ScreenId.Profile);

        int rowsTop = topArea.Bottom + BlockGap;
        _passwordRow = CreateRow(panelX, rowsTop, panelWidth, 0);
        _emailRow = CreateRow(panelX, rowsTop, panelWidth, 1);
        _sessionsRow = CreateRow(panelX, rowsTop, panelWidth, 2);
        _friendCodeRow = CreateRow(panelX, rowsTop, panelWidth, 3);
        _friendCodeRow.IsButtonEnabled = false;
        _passwordRow.Clicked += (_, _) => Navigator.GoTo(ScreenId.ChangePassword);
        _emailRow.Clicked += (_, _) => Navigator.GoTo(ScreenId.ChangeEmail);
        _sessionsRow.Clicked += (_, _) => Navigator.GoTo(ScreenId.ActiveSessions);

        int signOutTop = rowsTop + (4 * RowHeight) + SignOutGap;
        _signOutButton = CreateOutlineButton(new Rectangle(panelX, signOutTop, panelWidth, SignOutHeight));
        _signOutButton.Clicked += OnSignOutClicked;

        _deleteBox = new DashedBox { Bounds = new Rectangle(panelX, signOutTop + SignOutHeight + BlockGap, panelWidth, DeleteBoxHeight) };
        _deleteBox.Clicked += (_, _) => Navigator.GoTo(ScreenId.DeleteAccount);

        _languageLabel = new TextLine { Style = TextLineStyle.Small, Bounds = new Rectangle(panelX, top, panelWidth, LanguageLabelHeight) };
        int rowTop = top + LanguageLabelHeight + LanguageRowsGap;
        _spanishRow = new RadioRow { Bounds = new Rectangle(panelX, rowTop, panelWidth, LanguageRowHeight) };
        _englishRow = new RadioRow { Bounds = new Rectangle(panelX, rowTop + LanguageRowHeight, panelWidth, LanguageRowHeight) };
        _spanishRow.Chosen += (_, _) => ChooseLanguage(LanguagePicker.SpanishIndex);
        _englishRow.Chosen += (_, _) => ChooseLanguage(LanguagePicker.EnglishIndex);
        _chatNote = new TextLine { Style = TextLineStyle.Small, Bounds = new Rectangle(panelX, Card.Bottom - Theme.CardPadding - LanguageLabelHeight, panelWidth, LanguageLabelHeight) };

        Register(_heading);
        Register(_sidebar);

        foreach (Control control in new Control[] { _topBox, _avatar, _nickname, _email, _editButton, _passwordRow, _emailRow, _sessionsRow, _friendCodeRow, _signOutButton, _deleteBox })
        {
            _accountControls.Add(control);
            Register(control);
        }

        foreach (Control control in new Control[] { _languageLabel, _spanishRow, _englishRow, _chatNote })
        {
            _languageControls.Add(control);
            Register(control);
        }

        ShowPanel(showsLanguage);
        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return string.Empty;
    }

    public override void Draw(Canvas canvas)
    {
        base.Draw(canvas);
        int x = ContentX + SidebarWidth + (SidebarGap / 2);
        canvas.Shapes.DrawRectangle(new Rectangle(x, Card.Y + Theme.CardPadding, 2, Card.Height - (Theme.CardPadding * 2)), Theme.TextDark);
    }

    protected override void ApplyTexts()
    {
        _heading.Text = TextCatalog.SettingsHeading;
        _sidebar.Items[0].Text = TextCatalog.SettingsBoardEntry;
        _sidebar.Items[1].Text = TextCatalog.SettingsAudioEntry;
        _sidebar.Items[2].Text = TextCatalog.SettingsAccountEntry;
        _sidebar.Items[3].Text = TextCatalog.SettingsLanguageEntry;
        _sidebar.Items[4].Text = TextCatalog.SettingsAccessibilityEntry;

        _avatar.Text = TextCatalog.AvatarPlaceholder;
        _nickname.Text = TestAccount.Nickname;
        _email.Text = EmailMask.Apply(TestAccount.Email);
        _editButton.Title = TextCatalog.CommonEditButton;

        _passwordRow.Title = TextCatalog.SettingsPasswordRow;
        _passwordRow.Subtitle = string.Format(TextCatalog.SettingsPasswordChangedFormat, TestProfile.PasswordChangedMonthsAgo);
        _passwordRow.ButtonLabel = TextCatalog.CommonChangeButton;
        _emailRow.Title = TextCatalog.SettingsEmailRow;
        _emailRow.Subtitle = TextCatalog.SettingsEmailVerified;
        _emailRow.ButtonLabel = TextCatalog.CommonChangeButton;
        _sessionsRow.Title = TextCatalog.SettingsSessionsRow;
        _sessionsRow.Subtitle = string.Format(TextCatalog.SettingsSessionsCountFormat, TestProfile.ActiveSessionCount);
        _sessionsRow.ButtonLabel = TextCatalog.CommonViewButton;
        _friendCodeRow.Title = TextCatalog.SettingsFriendCodeRow;
        _friendCodeRow.Subtitle = TestAccount.FriendCode;
        _friendCodeRow.ButtonLabel = TextCatalog.CommonCopyButton;
        _signOutButton.Title = TextCatalog.SettingsSignOutButton;
        _deleteBox.Title = TextCatalog.SettingsDeleteTitle;
        _deleteBox.Hint = TextCatalog.SettingsDeleteHint;

        _languageLabel.Text = TextCatalog.SettingsInterfaceLanguageLabel;
        _spanishRow.Text = TextCatalog.SpanishMexicoLanguageName;
        _englishRow.Text = TextCatalog.EnglishLanguageName;
        _spanishRow.IsSelected = !Language.IsEnglish;
        _englishRow.IsSelected = Language.IsEnglish;
        _spanishRow.Tag = Language.IsEnglish ? string.Empty : TextCatalog.SettingsCurrentTag;
        _englishRow.Tag = Language.IsEnglish ? TextCatalog.SettingsCurrentTag : string.Empty;
        _chatNote.Text = TextCatalog.SettingsChatNote;
    }

    private void OnSidebarChosen(object? sender, SelectionChangedEventArgs e)
    {
        ShowPanel(e.SelectedIndex == LanguageIndex);
    }

    private void OnEditClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.EditProfile);
    }

    private void OnSignOutClicked(object? sender, EventArgs e)
    {
        Navigator.ShowConfirm(new ConfirmRequest
        {
            Body = TextCatalog.SettingsSignOutBody,
            PrimaryLabel = TextCatalog.SettingsSignOutButton,
            SecondaryLabel = TextCatalog.CommonCancelButton,
            OnConfirm = () => Navigator.GoTo(ScreenId.Login)
        });
    }

    private void ChooseLanguage(int index)
    {
        LanguagePicker.Apply(index);
        ApplyTexts();
    }

    private void ShowPanel(bool showsLanguage)
    {
        foreach (Control control in _accountControls)
        {
            SetVisible(control, !showsLanguage);
        }

        foreach (Control control in _languageControls)
        {
            SetVisible(control, showsLanguage);
        }
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

    private SettingRow CreateRow(int x, int top, int width, int index)
    {
        return new SettingRow { Bounds = new Rectangle(x, top + (index * RowHeight), width, RowHeight) };
    }
}
