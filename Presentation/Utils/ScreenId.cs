namespace Bastion.Presentation.Utils;

// The screens the navigator can reach by name, so a screen asks for a
// destination without depending on the class that implements it.
public enum ScreenId
{
    Login,
    Register,
    ForgotPassword,
    ResetPassword,
    AccountSettings,
    ChangePassword,
    ChangeEmail,
    ChangeNickname,
    DeleteAccount,
    ActiveSessions,
    RegistrationSuccess,
    Profile,
    EditProfile,
    PlayerCard,
    Friends,
    AddFriend,
    MatchHistory,
    CoinHistory,
    Ranking,
    Report,
    ModerationQueue,
    ReportReview,
    ApplySanction,
    Appeal,
    AdminPanel,
    Logs,
    Shop,
    PurchaseConfirm,
    BoxPurchaseConfirm,
    Customize,
    SettingsLanguage,
    Menu,

    // Not reachable yet from Login or from a session: the server that would
    // trigger them (an unverified account, a sanction, a second factor) does
    // not exist. Reachable from the review menu, GUI_Menu, until it does.
    MainScreen,
    PendingVerification,
    SecondFactor,
    BannedAccount,
    FirstTime,
    LinkAccount,

    // The home hub and the match flow it opens.
    MainMenu,
    SelectMode,
    Matchmaking,
    VersusScreen,
    Match,
    MatchEnd,
    OpponentDisconnected,
    PrivateMatch,
    WaitingRoom,
    AIDifficulty,
    AIMatchEnd,
    Spectator,
    Replay,
    TutorialIndex
}
