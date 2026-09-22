using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_Menu;

public sealed class GuiMenu : FormScreen
{
    private const int Columns = 3;
    private const int EntryHeight = 52;
    private const int EntryGap = 12;
    private const int EntryCount = 36;
    private const int Rows = (EntryCount + Columns - 1) / Columns;
    private const int CardHeight = Theme.CardPadding + (Rows * (EntryHeight + EntryGap)) - EntryGap + Theme.CardPadding;

    private static readonly (Func<string> Title, Action<INavigator> Open)[] Entries =
    [
            (() => TextCatalog.ProfileSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.Profile))),
            (() => TextCatalog.SettingsHeading, (Action<INavigator>)(n => n.GoTo(ScreenId.AccountSettings))),
            (() => TextCatalog.FriendsSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.Friends))),
            (() => TextCatalog.MatchHistorySubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.MatchHistory))),
            (() => TextCatalog.RankingSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.Ranking))),
            (() => TextCatalog.ShopSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.Shop))),
            (() => TextCatalog.CustomizeSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.Customize))),
            (() => TextCatalog.CoinHistorySubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.CoinHistory))),
            (() => TextCatalog.ReportSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.Report))),
            (() => TextCatalog.ModerationQueueSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.ModerationQueue))),
            (() => TextCatalog.AppealSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.Appeal))),
            (() => TextCatalog.AdminPanelSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.AdminPanel))),
            (() => TextCatalog.PlayerCardSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.PlayerCard))),
            (() => TextCatalog.LogsSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.Logs))),
            (() => TextCatalog.BoxPurchaseSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.BoxPurchaseConfirm))),

            // Not reachable yet from Login or from a session (see ScreenId.cs).
            (() => TextCatalog.MainScreenSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.MainScreen))),
            (() => TextCatalog.PendingVerificationSubtitle,
                (Action<INavigator>)(n => n.GoTo(ScreenId.PendingVerification))),
            (() => TextCatalog.SecondFactorSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.SecondFactor))),
            (() => TextCatalog.BannedAccountSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.BannedAccount))),
            (() => TextCatalog.FirstTimeSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.FirstTime))),
            (() => TextCatalog.LinkAccountSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.LinkAccount))),

            // The home hub and the match flow it opens.
            (() => TextCatalog.MainMenuSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.MainMenu))),
            (() => TextCatalog.SelectModeSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.SelectMode))),
            (() => TextCatalog.MatchmakingSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.Matchmaking))),
            (() => TextCatalog.VersusScreenSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.VersusScreen))),
            (() => TextCatalog.MatchSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.Match))),
            (() => TextCatalog.MatchEndDefeatTitle, (Action<INavigator>)(n => n.GoTo(ScreenId.MatchEnd))),
            (() => TextCatalog.OpponentDisconnectedTitle,
                (Action<INavigator>)(n => n.GoTo(ScreenId.OpponentDisconnected))),
            (() => TextCatalog.PrivateMatchSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.PrivateMatch))),
            (() => TextCatalog.WaitingRoomSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.WaitingRoom))),
            (() => TextCatalog.AIDifficultySubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.AIDifficulty))),
            (() => TextCatalog.AIMatchEndSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.AIMatchEnd))),
            (() => TextCatalog.SpectatorSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.Spectator))),
            (() => TextCatalog.ReplayTitle, (Action<INavigator>)(n => n.GoTo(ScreenId.Replay))),
            (() => TextCatalog.TutorialIndexSubtitle, (Action<INavigator>)(n => n.GoTo(ScreenId.TutorialIndex))),

            (() => TextCatalog.SettingsSignOutButton, (Action<INavigator>)(n => n.Restart(ScreenId.Login)))
    ];

    private readonly List<Button> _buttons = [];

    public GuiMenu(INavigator navigator)
        : base(navigator, WideCardWidth, CardHeight)
    {
        int width = (ContentWidth - ((Columns - 1) * EntryGap)) / Columns;

        for (int i = 0; i < Entries.Length; i++)
        {
            int x = ContentX + ((i % Columns) * (width + EntryGap));
            int y = Card.Y + Theme.CardPadding + ((i / Columns) * (EntryHeight + EntryGap));
            var button = new Button { Style = ButtonStyle.Secondary, Bounds = new Rectangle(x, y, width, EntryHeight) };
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
            _buttons[i].Title = Entries[i].Title().ToUpper(CultureInfo.CurrentCulture);
        }
    }
}
