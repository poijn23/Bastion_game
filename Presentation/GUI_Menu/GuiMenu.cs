using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_Menu;

public sealed class GuiMenu : FormScreen
{
    private const int Columns = 4;
    private const int EntryHeight = 40;
    private const int EntryGap = 8;
    private const int EntryCount = 31;
    private const int Rows = (EntryCount + Columns - 1) / Columns;
    private const int CardHeight = Theme.CardPadding + (Rows * (EntryHeight + EntryGap)) - EntryGap + Theme.CardPadding;

    private static readonly (Func<string> Title, Action<INavigator> Open)[] Entries =
    [
            (() => TextCatalog.ProfileSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.Profile))),
            (() => TextCatalog.EditProfileSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.EditProfile))),
            (() => TextCatalog.SettingsAccountEntry, (Action<INavigator>)(n => n.GoTo(ScreenId.AccountSettings))),
            (() => TextCatalog.SettingsLanguageEntry, (Action<INavigator>)(n => n.GoTo(ScreenId.SettingsLanguage))),
            (() => TextCatalog.ChangePasswordSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.ChangePassword))),
            (() => TextCatalog.ChangeEmailSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.ChangeEmail))),
            (() => TextCatalog.ChangeNicknameSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.ChangeNickname))),
            (() => TextCatalog.DeleteAccountSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.DeleteAccount))),
            (() => TextCatalog.ActiveSessionsSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.ActiveSessions))),
            (() => TextCatalog.RegisterSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.Register))),
            (() => TextCatalog.RegistrationSuccessSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.RegistrationSuccess, "prueba@bastion.test"))),
            (() => TextCatalog.ForgotPasswordSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.ForgotPassword))),
            (() => TextCatalog.ResetPasswordSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.ResetPassword, "prueba@bastion.test"))),
            (() => TextCatalog.PlayerCardSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.PlayerCard))),
            (() => TextCatalog.FriendsSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.Friends))),
            (() => TextCatalog.AddFriendSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.AddFriend))),
            (() => TextCatalog.MatchHistorySubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.MatchHistory))),
            (() => TextCatalog.CoinHistorySubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.CoinHistory))),
            (() => TextCatalog.RankingSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.Ranking))),
            (() => TextCatalog.ReportSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.Report))),
            (() => TextCatalog.ModerationQueueSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.ModerationQueue))),
            (() => TextCatalog.ReportReviewSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.ReportReview))),
            (() => TextCatalog.ApplySanctionSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.ApplySanction))),
            (() => TextCatalog.AppealSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.Appeal))),
            (() => TextCatalog.AdminPanelSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.AdminPanel))),
            (() => TextCatalog.LogsSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.Logs))),
            (() => TextCatalog.ShopSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.Shop))),
            (() => TextCatalog.PurchaseConfirmSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.PurchaseConfirm))),
            (() => TextCatalog.BoxPurchaseSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.BoxPurchaseConfirm))),
            (() => TextCatalog.CustomizeSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.Customize))),
            (() => TextCatalog.SettingsSignOutButton, (Action<INavigator>)(n => n.GoTo(ScreenId.Login)))
    ];

    private readonly List<Button> _buttons = [];

    public GuiMenu(INavigator navigator)
        : base(navigator, Theme.PanelCardWidth, CardHeight)
    {
        int width = (ContentWidth - ((Columns - 1) * EntryGap)) / Columns;

        for (int i = 0; i < Entries.Length; i++)
        {
            int x = ContentX + ((i % Columns) * (width + EntryGap));
            int y = Card.Y + Theme.CardPadding + ((i / Columns) * (EntryHeight + EntryGap));
            var button = new Button { Style = ButtonStyle.Secondary, IsCompact = true, Bounds = new Rectangle(x, y, width, EntryHeight) };
            int index = i;
            button.Clicked += (_, _) => Entries[index].Open(Navigator);
            _buttons.Add(button);
            Register(button);
        }

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.MenuSubtitle;
    }

    protected override void ApplyTexts()
    {
        for (int i = 0; i < Entries.Length; i++)
        {
            _buttons[i].Title = Entries[i].Title();
        }
    }
}
