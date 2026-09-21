using System;
using Microsoft.Xna.Framework;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_AccountSettings;

// The hub the account use cases return to. It owns no data of its own: it only
// leads to the screens that change one thing each.
public sealed class GuiAccountSettings : FormScreen
{
    private const int CardHeight = 408;
    private const int EntryHeight = 52;
    private const int EntryGap = 12;

    private readonly Button _profileEntry;
    private readonly Button _nicknameEntry;
    private readonly Button _passwordEntry;
    private readonly Button _emailEntry;
    private readonly Button _sessionsEntry;
    private readonly Button _deleteEntry;
    private readonly Button _backButton;

    public GuiAccountSettings(INavigator navigator)
        : base(navigator, NarrowCardWidth, CardHeight)
    {
        _profileEntry = CreateEntry(0);
        _nicknameEntry = CreateEntry(1);
        _passwordEntry = CreateEntry(2);
        _emailEntry = CreateEntry(3);
        _sessionsEntry = CreateEntry(4);
        _deleteEntry = CreateEntry(5);
        // This screen has no primary action, so the only button takes the top
        // slot instead of leaving a gap above it.
        _backButton = CreateSecondaryButton();
        _backButton.MoveTo(PrimaryButtonBounds);

        _profileEntry.Clicked += OnProfileClicked;
        _nicknameEntry.Clicked += OnNicknameClicked;
        _passwordEntry.Clicked += OnPasswordClicked;
        _emailEntry.Clicked += OnEmailClicked;
        _sessionsEntry.Clicked += OnSessionsClicked;
        _deleteEntry.Clicked += OnDeleteClicked;
        _backButton.Clicked += OnBackClicked;

        Register(_profileEntry);
        Register(_nicknameEntry);
        Register(_passwordEntry);
        Register(_emailEntry);
        Register(_sessionsEntry);
        Register(_deleteEntry);
        Register(_backButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.AccountSettingsSubtitle;
    }

    private void OnProfileClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.Profile);
    }

    private void OnNicknameClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.ChangeNickname);
    }

    private void OnPasswordClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.ChangePassword);
    }

    private void OnEmailClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.ChangeEmail);
    }

    private void OnSessionsClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.ActiveSessions);
    }

    private void OnDeleteClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.DeleteAccount);
    }

    private void OnBackClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    protected override void ApplyTexts()
    {
        _profileEntry.Title = TextCatalog.AccountSettingsProfileEntry;
        _nicknameEntry.Title = TextCatalog.AccountSettingsNicknameEntry;
        _passwordEntry.Title = TextCatalog.AccountSettingsPasswordEntry;
        _emailEntry.Title = TextCatalog.AccountSettingsEmailEntry;
        _sessionsEntry.Title = TextCatalog.AccountSettingsSessionsEntry;
        _deleteEntry.Title = TextCatalog.AccountSettingsDeleteEntry;
        _backButton.Title = TextCatalog.CommonBackButton;
    }

    private Button CreateEntry(int index)
    {
        int top = Card.Y + Theme.CardPadding + (index * (EntryHeight + EntryGap));

        return new Button
        {
            Style = ButtonStyle.Secondary,
            Bounds = new Rectangle(ContentX, top, ContentWidth, EntryHeight)
        };
    }
}
