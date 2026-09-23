using Bastion.Presentation.GUI_WaitingRoom;
using Bastion.Presentation.Utils;
using Bastion.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bastion.Presentation.Tests;

// The paths the use cases describe, pressed button by button. They guard the
// wiring: a handler that stops navigating breaks one of these, not the build.
[TestClass]
public sealed class TestNavigationFlow
{
    private Navigator _navigator = new();

    [TestInitialize]
    public void OpenTheGameInEnglish()
    {
        Language.Apply(Language.English);
        _navigator = new Navigator();
        _navigator.Start(ScreenId.MainScreen);
    }

    [TestMethod]
    public void SignIn_WithTheTestAccount_ReachesTheSecondFactor()
    {
        ScreenDriver.Click(_navigator.Current!, TextCatalog.LoginSignInButton);
        ScreenDriver.Type(_navigator.Current!, 0, TestAccount.Nickname);
        ScreenDriver.Type(_navigator.Current!, 1, TestAccount.Password);

        ScreenDriver.Click(_navigator.Current!, TextCatalog.LoginSignInButton);

        Assert.AreEqual(ScreenId.SecondFactor, _navigator.CurrentId);
    }

    [TestMethod]
    public void SignIn_AfterTheSecondFactor_ReachesTheMainMenu()
    {
        GoToTheMainMenu();

        Assert.AreEqual(ScreenId.MainMenu, _navigator.CurrentId);
    }

    [TestMethod]
    public void SignIn_WithASanctionedAccount_ReachesTheBannedScreen()
    {
        ScreenDriver.Click(_navigator.Current!, TextCatalog.LoginSignInButton);
        ScreenDriver.Type(_navigator.Current!, 0, TestAccount.BannedIdentifier);
        ScreenDriver.Type(_navigator.Current!, 1, TestAccount.Password);

        ScreenDriver.Click(_navigator.Current!, TextCatalog.LoginSignInButton);

        Assert.AreEqual(ScreenId.BannedAccount, _navigator.CurrentId);
    }

    [TestMethod]
    public void SignIn_WithAnUnverifiedAccount_ReachesTheWaitingScreen()
    {
        ScreenDriver.Click(_navigator.Current!, TextCatalog.LoginSignInButton);
        ScreenDriver.Type(_navigator.Current!, 0, TestAccount.PendingIdentifier);
        ScreenDriver.Type(_navigator.Current!, 1, TestAccount.Password);

        ScreenDriver.Click(_navigator.Current!, TextCatalog.LoginSignInButton);

        Assert.AreEqual(ScreenId.PendingVerification, _navigator.CurrentId);
    }

    [TestMethod]
    public void SignIn_WithTheWrongPassword_StaysOnTheForm()
    {
        ScreenDriver.Click(_navigator.Current!, TextCatalog.LoginSignInButton);
        ScreenDriver.Type(_navigator.Current!, 0, TestAccount.Nickname);
        ScreenDriver.Type(_navigator.Current!, 1, "not the password");

        ScreenDriver.Click(_navigator.Current!, TextCatalog.LoginSignInButton);

        Assert.AreEqual(ScreenId.Login, _navigator.CurrentId);
    }

    [TestMethod]
    public void CreateAccount_FromTheForm_ReachesTheRegistrationForm()
    {
        ScreenDriver.Click(_navigator.Current!, TextCatalog.LoginSignInButton);

        ScreenDriver.Click(_navigator.Current!, TextCatalog.LoginCreateAccountButton);

        Assert.AreEqual(ScreenId.Register, _navigator.CurrentId);
    }

    // CU-02 FA-08: leaving the form asks before throwing the typing away.
    [TestMethod]
    public void CancelRegistration_BeforeConfirming_AsksFirst()
    {
        GoToTheRegistrationForm();

        ScreenDriver.Click(_navigator.Current!, TextCatalog.RegisterCancelButton);

        Assert.IsTrue(ScreenDriver.IsShowingADialog(_navigator.Current));
    }

    [TestMethod]
    public void CancelRegistration_OnceConfirmed_ReturnsToTheLogin()
    {
        GoToTheRegistrationForm();
        ScreenDriver.Click(_navigator.Current!, TextCatalog.RegisterCancelButton);

        ScreenDriver.ConfirmDialog(_navigator.Current!);

        Assert.AreEqual(ScreenId.Login, _navigator.CurrentId);
    }

    [TestMethod]
    public void CancelRegistration_OnceDismissed_StaysOnTheForm()
    {
        GoToTheRegistrationForm();
        ScreenDriver.Click(_navigator.Current!, TextCatalog.RegisterCancelButton);

        ScreenDriver.DismissDialog(_navigator.Current!);

        Assert.AreEqual(ScreenId.Register, _navigator.CurrentId);
    }

    [TestMethod]
    public void ForgotPassword_FromTheForm_ReachesTheResetScreen()
    {
        ScreenDriver.Click(_navigator.Current!, TextCatalog.LoginSignInButton);
        ScreenDriver.Click(_navigator.Current!, TextCatalog.LoginForgotLink);
        ScreenDriver.Type(_navigator.Current!, 0, TestAccount.Email);

        ScreenDriver.Click(_navigator.Current!, TextCatalog.ForgotPasswordSendButton);

        Assert.AreEqual(ScreenId.ResetPassword, _navigator.CurrentId);
    }

    [TestMethod]
    public void PlayAsGuest_FromTheForm_ReachesTheFirstTimeSetUp()
    {
        ScreenDriver.Click(_navigator.Current!, TextCatalog.LoginSignInButton);

        ScreenDriver.Click(_navigator.Current!, TextCatalog.LoginGuestButton);

        Assert.AreEqual(ScreenId.FirstTime, _navigator.CurrentId);
    }

    [TestMethod]
    public void FindMatch_FromTheMainMenu_ReachesTheModeChoice()
    {
        GoToTheMainMenu();

        ScreenDriver.Click(_navigator.Current!, TextCatalog.MainMenuFindMatchButton);

        Assert.AreEqual(ScreenId.SelectMode, _navigator.CurrentId);
    }

    [TestMethod]
    public void PrivateMatch_CreatedFromTheMainMenu_ReachesTheWaitingRoom()
    {
        GoToTheMainMenu();
        ScreenDriver.Click(_navigator.Current!, TextCatalog.MainMenuPrivateButton);

        ScreenDriver.Click(_navigator.Current!, TextCatalog.PrivateMatchCreateButton);

        Assert.AreEqual(ScreenId.WaitingRoom, _navigator.CurrentId);
    }

    [TestMethod]
    public void StartMatch_AsTheHost_ReachesTheVersusScreen()
    {
        _navigator.GoTo(ScreenId.WaitingRoom, GuiWaitingRoom.HostRole);

        ScreenDriver.Click(_navigator.Current!, TextCatalog.WaitingRoomStartButton);

        Assert.AreEqual(ScreenId.VersusScreen, _navigator.CurrentId);
    }

    [TestMethod]
    public void Shop_BuyingABox_ReachesItsOwnConfirmation()
    {
        GoToTheMainMenu();
        ScreenDriver.Click(_navigator.Current!, TextCatalog.MainMenuShopButton);

        ScreenDriver.Click(_navigator.Current!, TextCatalog.ShopBoxButton);

        Assert.AreEqual(ScreenId.BoxPurchaseConfirm, _navigator.CurrentId);
    }

    [TestMethod]
    public void Friends_OpeningSomeoneOnTheList_ReachesTheirCard()
    {
        GoToTheMainMenu();
        ScreenDriver.Click(_navigator.Current!, TextCatalog.MainMenuFriendsButton);

        ScreenDriver.ClickRow(_navigator.Current!, 0);

        Assert.AreEqual(ScreenId.PlayerCard, _navigator.CurrentId);
    }

    [TestMethod]
    public void PlayerCard_ReportingSomeone_ReachesTheReportForm()
    {
        _navigator.GoTo(ScreenId.PlayerCard);

        ScreenDriver.Click(_navigator.Current!, TextCatalog.PlayerCardReportButton);

        Assert.AreEqual(ScreenId.Report, _navigator.CurrentId);
    }

    [TestMethod]
    public void Ranking_OpeningSomeoneOnTheBoard_ReachesTheirCard()
    {
        GoToTheMainMenu();
        ScreenDriver.Click(_navigator.Current!, TextCatalog.MainMenuRankingButton);

        ScreenDriver.ClickRow(_navigator.Current!, 0);

        Assert.AreEqual(ScreenId.PlayerCard, _navigator.CurrentId);
    }

    [TestMethod]
    public void History_OpeningAMatch_ReachesItsReplay()
    {
        GoToTheMainMenu();
        ScreenDriver.Click(_navigator.Current!, TextCatalog.MainMenuHistoryButton);

        ScreenDriver.ClickRow(_navigator.Current!, 0);

        Assert.AreEqual(ScreenId.Replay, _navigator.CurrentId);
    }

    [TestMethod]
    public void Moderation_FromTheMainMenu_ReachesTheReportQueue()
    {
        GoToTheMainMenu();
        ScreenDriver.Click(_navigator.Current!, TextCatalog.MainMenuModerationButton);

        ScreenDriver.Click(_navigator.Current!, TextCatalog.AdminPanelQueueButton);

        Assert.AreEqual(ScreenId.ModerationQueue, _navigator.CurrentId);
    }

    [TestMethod]
    public void Settings_FromTheMainMenu_ReachesTheAccountPanel()
    {
        GoToTheMainMenu();

        ScreenDriver.Click(_navigator.Current!, TextCatalog.MainMenuSettingsButton);

        Assert.AreEqual(ScreenId.AccountSettings, _navigator.CurrentId);
    }

    [TestMethod]
    public void GoBack_AfterTwoSteps_ReturnsToTheScreenBefore()
    {
        GoToTheMainMenu();
        ScreenDriver.Click(_navigator.Current!, TextCatalog.MainMenuShopButton);

        _navigator.GoBack();

        Assert.AreEqual(ScreenId.MainMenu, _navigator.CurrentId);
    }

    private void GoToTheRegistrationForm()
    {
        ScreenDriver.Click(_navigator.Current!, TextCatalog.LoginSignInButton);
        ScreenDriver.Click(_navigator.Current!, TextCatalog.LoginCreateAccountButton);
    }

    private void GoToTheMainMenu()
    {
        ScreenDriver.Click(_navigator.Current!, TextCatalog.LoginSignInButton);
        ScreenDriver.Type(_navigator.Current!, 0, TestAccount.Nickname);
        ScreenDriver.Type(_navigator.Current!, 1, TestAccount.Password);
        ScreenDriver.Click(_navigator.Current!, TextCatalog.LoginSignInButton);
        ScreenDriver.Click(_navigator.Current!, TextCatalog.SecondFactorVerifyButton);
    }
}
