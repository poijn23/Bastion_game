using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bastion.Presentation.GUI_WaitingRoom;
using Bastion.Presentation.Utils;
using Bastion.Resources;

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

        Assert.AreEqual(Destination.On(ScreenId.SecondFactor), Destination.Of(_navigator));
    }

    [TestMethod]
    public void SignIn_AfterTheSecondFactor_ReachesTheMainMenu()
    {
        GoToTheMainMenu();

        Assert.AreEqual(Destination.On(ScreenId.MainMenu), Destination.Of(_navigator));
    }

    [TestMethod]
    public void SignIn_WithASanctionedAccount_ReachesTheBannedScreen()
    {
        ScreenDriver.Click(_navigator.Current!, TextCatalog.LoginSignInButton);
        ScreenDriver.Type(_navigator.Current!, 0, TestAccount.BannedIdentifier);
        ScreenDriver.Type(_navigator.Current!, 1, TestAccount.Password);

        ScreenDriver.Click(_navigator.Current!, TextCatalog.LoginSignInButton);

        Assert.AreEqual(Destination.On(ScreenId.BannedAccount), Destination.Of(_navigator));
    }

    [TestMethod]
    public void SignIn_WithAnUnverifiedAccount_ReachesTheWaitingScreen()
    {
        ScreenDriver.Click(_navigator.Current!, TextCatalog.LoginSignInButton);
        ScreenDriver.Type(_navigator.Current!, 0, TestAccount.PendingIdentifier);
        ScreenDriver.Type(_navigator.Current!, 1, TestAccount.Password);

        ScreenDriver.Click(_navigator.Current!, TextCatalog.LoginSignInButton);

        Assert.AreEqual(Destination.On(ScreenId.PendingVerification), Destination.Of(_navigator));
    }

    [TestMethod]
    public void SignIn_WithTheWrongPassword_StaysOnTheForm()
    {
        ScreenDriver.Click(_navigator.Current!, TextCatalog.LoginSignInButton);
        ScreenDriver.Type(_navigator.Current!, 0, TestAccount.Nickname);
        ScreenDriver.Type(_navigator.Current!, 1, "not the password");

        ScreenDriver.Click(_navigator.Current!, TextCatalog.LoginSignInButton);

        Assert.AreEqual(Destination.On(ScreenId.Login), Destination.Of(_navigator));
    }

    [TestMethod]
    public void CreateAccount_FromTheForm_ReachesTheRegistrationForm()
    {
        ScreenDriver.Click(_navigator.Current!, TextCatalog.LoginSignInButton);

        ScreenDriver.Click(_navigator.Current!, TextCatalog.LoginCreateAccountButton);

        Assert.AreEqual(Destination.On(ScreenId.Register), Destination.Of(_navigator));
    }

    // CU-02 FA-08: leaving the form asks before throwing the typing away.
    [TestMethod]
    public void CancelRegistration_BeforeConfirming_AsksFirst()
    {
        GoToTheRegistrationForm();

        ScreenDriver.Click(_navigator.Current!, TextCatalog.RegisterCancelButton);

        Assert.AreEqual(Destination.Asking(ScreenId.Register), Destination.Of(_navigator));
    }

    [TestMethod]
    public void CancelRegistration_OnceConfirmed_ReturnsToTheLogin()
    {
        GoToTheRegistrationForm();
        ScreenDriver.Click(_navigator.Current!, TextCatalog.RegisterCancelButton);

        ScreenDriver.ConfirmDialog(_navigator.Current!);

        Assert.AreEqual(Destination.On(ScreenId.Login), Destination.Of(_navigator));
    }

    [TestMethod]
    public void CancelRegistration_OnceDismissed_StaysOnTheForm()
    {
        GoToTheRegistrationForm();
        ScreenDriver.Click(_navigator.Current!, TextCatalog.RegisterCancelButton);

        ScreenDriver.DismissDialog(_navigator.Current!);

        Assert.AreEqual(Destination.On(ScreenId.Register), Destination.Of(_navigator));
    }

    [TestMethod]
    public void ForgotPassword_FromTheForm_ReachesTheResetScreen()
    {
        ScreenDriver.Click(_navigator.Current!, TextCatalog.LoginSignInButton);
        ScreenDriver.Click(_navigator.Current!, TextCatalog.LoginForgotLink);
        ScreenDriver.Type(_navigator.Current!, 0, TestAccount.Email);

        ScreenDriver.Click(_navigator.Current!, TextCatalog.ForgotPasswordSendButton);

        Assert.AreEqual(Destination.On(ScreenId.ResetPassword), Destination.Of(_navigator));
    }

    [TestMethod]
    public void PlayAsGuest_FromTheForm_ReachesTheFirstTimeSetUp()
    {
        ScreenDriver.Click(_navigator.Current!, TextCatalog.LoginSignInButton);

        ScreenDriver.Click(_navigator.Current!, TextCatalog.LoginGuestButton);

        Assert.AreEqual(Destination.On(ScreenId.FirstTime), Destination.Of(_navigator));
    }

    [TestMethod]
    public void FindMatch_FromTheMainMenu_ReachesTheModeChoice()
    {
        GoToTheMainMenu();

        ScreenDriver.Click(_navigator.Current!, TextCatalog.MainMenuFindMatchButton);

        Assert.AreEqual(Destination.On(ScreenId.SelectMode), Destination.Of(_navigator));
    }

    [TestMethod]
    public void PrivateMatch_CreatedFromTheMainMenu_ReachesTheWaitingRoom()
    {
        GoToTheMainMenu();
        ScreenDriver.Click(_navigator.Current!, TextCatalog.MainMenuPrivateButton);

        ScreenDriver.Click(_navigator.Current!, TextCatalog.PrivateMatchCreateButton);

        Assert.AreEqual(Destination.On(ScreenId.WaitingRoom), Destination.Of(_navigator));
    }

    [TestMethod]
    public void StartMatch_AsTheHost_ReachesTheVersusScreen()
    {
        _navigator.GoTo(ScreenId.WaitingRoom, GuiWaitingRoom.HostRole);

        ScreenDriver.Click(_navigator.Current!, TextCatalog.WaitingRoomStartButton);

        Assert.AreEqual(Destination.On(ScreenId.VersusScreen), Destination.Of(_navigator));
    }

    [TestMethod]
    public void Shop_BuyingABox_ReachesItsOwnConfirmation()
    {
        GoToTheMainMenu();
        ScreenDriver.Click(_navigator.Current!, TextCatalog.MainMenuShopButton);

        ScreenDriver.Click(_navigator.Current!, TextCatalog.ShopBoxButton);

        Assert.AreEqual(Destination.On(ScreenId.BoxPurchaseConfirm), Destination.Of(_navigator));
    }

    [TestMethod]
    public void Friends_OpeningSomeoneOnTheList_ReachesTheirCard()
    {
        GoToTheMainMenu();
        ScreenDriver.Click(_navigator.Current!, TextCatalog.MainMenuFriendsButton);

        ScreenDriver.ClickRow(_navigator.Current!, 0);

        Assert.AreEqual(Destination.On(ScreenId.PlayerCard), Destination.Of(_navigator));
    }

    [TestMethod]
    public void PlayerCard_ReportingSomeone_ReachesTheReportForm()
    {
        _navigator.GoTo(ScreenId.PlayerCard);

        ScreenDriver.Click(_navigator.Current!, TextCatalog.PlayerCardReportButton);

        Assert.AreEqual(Destination.On(ScreenId.Report), Destination.Of(_navigator));
    }

    [TestMethod]
    public void Ranking_OpeningSomeoneOnTheBoard_ReachesTheirCard()
    {
        GoToTheMainMenu();
        ScreenDriver.Click(_navigator.Current!, TextCatalog.MainMenuRankingButton);

        ScreenDriver.ClickRow(_navigator.Current!, 0);

        Assert.AreEqual(Destination.On(ScreenId.PlayerCard), Destination.Of(_navigator));
    }

    [TestMethod]
    public void History_OpeningAMatch_ReachesItsReplay()
    {
        GoToTheMainMenu();
        ScreenDriver.Click(_navigator.Current!, TextCatalog.MainMenuHistoryButton);

        ScreenDriver.ClickRow(_navigator.Current!, 0);

        Assert.AreEqual(Destination.On(ScreenId.Replay), Destination.Of(_navigator));
    }

    [TestMethod]
    public void Moderation_FromTheMainMenu_ReachesTheReportQueue()
    {
        GoToTheMainMenu();
        ScreenDriver.Click(_navigator.Current!, TextCatalog.MainMenuModerationButton);

        ScreenDriver.Click(_navigator.Current!, TextCatalog.AdminPanelQueueButton);

        Assert.AreEqual(Destination.On(ScreenId.ModerationQueue), Destination.Of(_navigator));
    }

    [TestMethod]
    public void Settings_FromTheMainMenu_ReachesTheAccountPanel()
    {
        GoToTheMainMenu();

        ScreenDriver.Click(_navigator.Current!, TextCatalog.MainMenuSettingsButton);

        Assert.AreEqual(Destination.On(ScreenId.AccountSettings), Destination.Of(_navigator));
    }

    [TestMethod]
    public void GoBack_AfterTwoSteps_ReturnsToTheScreenBefore()
    {
        GoToTheMainMenu();
        ScreenDriver.Click(_navigator.Current!, TextCatalog.MainMenuShopButton);

        _navigator.GoBack();

        Assert.AreEqual(Destination.On(ScreenId.MainMenu), Destination.Of(_navigator));
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
