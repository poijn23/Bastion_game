using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.Tests;

// CU-02 refuses a form for eight different reasons and says which one next
// to the box that holds it. Each test fills a good form, spoils one box and
// reads back what that box now says.
[TestClass]
public sealed class TestRegisterValidation
{
    private const int FirstNameBox = 0;
    private const int LastNameBox = 1;
    private const int NicknameBox = 2;
    private const int EmailBox = 3;
    private const int PasswordBox = 4;
    private const int ConfirmationBox = 5;
    private const int DayBox = 6;
    private const int YearBox = 8;

    private static readonly string[] _goodForm =
        ["Ada", "Stone", "newcomer", "newcomer@bastion.test", "Password1", "Password1", "04", "02", "2000"];

    private Navigator _navigator = new();

    // Kept from before the press: a dialog over the form would otherwise
    // be what the navigator answers with, and it holds no boxes.
    private IScreen _form = null!;

    [TestInitialize]
    public void OpenTheFormInEnglish()
    {
        Language.Apply(Language.English);
        _navigator = new Navigator();
        _navigator.Start(ScreenId.Register);
        _form = ScreenDriver.ScreenOf(_navigator);
    }

    [TestMethod]
    public void Validate_AGoodForm_ReachesTheSuccessScreen()
    {
        Fill();

        Send();

        Assert.AreEqual(Destination.On(ScreenId.RegistrationSuccess), Destination.Of(_navigator));
    }

    [TestMethod]
    public void Validate_AFormWithMistakes_StopsAndSaysSo()
    {
        Fill(FirstNameBox, string.Empty);

        Send();

        Assert.AreEqual(Destination.WithDialog(ScreenId.Register), Destination.Of(_navigator));
    }

    [TestMethod]
    public void Validate_AnEmptyFirstName_AsksForIt()
    {
        Fill(FirstNameBox, string.Empty);

        Send();

        Assert.AreEqual(TextCatalog.RegisterFirstNameRequired, WarningOn(FirstNameBox));
    }

    [TestMethod]
    public void Validate_AFirstNameWithDigits_RefusesTheShape()
    {
        Fill(FirstNameBox, "Ada3000");

        Send();

        Assert.AreEqual(TextCatalog.RegisterFirstNameInvalid, WarningOn(FirstNameBox));
    }

    [TestMethod]
    public void Validate_AnAccentedName_IsAccepted()
    {
        Fill(LastNameBox, "Núñez de la O'Higgins");

        Send();

        Assert.AreEqual(null, WarningOn(LastNameBox));
    }

    [TestMethod]
    public void Validate_AShortNickname_AsksForALongerOne()
    {
        Fill(NicknameBox, "ab");

        Send();

        Assert.AreEqual(TextCatalog.RegisterNicknameLength, WarningOn(NicknameBox));
    }

    [TestMethod]
    public void Validate_ANicknameWithSpaces_RefusesTheShape()
    {
        Fill(NicknameBox, "new comer");

        Send();

        Assert.AreEqual(TextCatalog.RegisterNicknameInvalid, WarningOn(NicknameBox));
    }

    [TestMethod]
    public void Validate_ATakenNickname_SaysItIsTaken()
    {
        Fill(NicknameBox, TestAccount.Nickname);

        Send();

        Assert.AreEqual(TextCatalog.RegisterNicknameTaken, WarningOn(NicknameBox));
    }

    [TestMethod]
    public void Validate_AnAddressWithoutADot_RefusesIt()
    {
        Fill(EmailBox, "someone@localhost");

        Send();

        Assert.AreEqual(TextCatalog.RegisterEmailInvalid, WarningOn(EmailBox));
    }

    [TestMethod]
    public void Validate_AnAddressWithASpace_RefusesIt()
    {
        Fill(EmailBox, "some one@bastion.test");

        Send();

        Assert.AreEqual(TextCatalog.RegisterEmailInvalid, WarningOn(EmailBox));
    }

    [TestMethod]
    public void Validate_ATakenAddress_SaysItIsTaken()
    {
        Fill(EmailBox, TestAccount.Email);

        Send();

        Assert.AreEqual(TextCatalog.RegisterEmailTaken, WarningOn(EmailBox));
    }

    [TestMethod]
    public void Validate_AShortPassword_AsksForALongerOne()
    {
        Fill(PasswordBox, "Abc1");

        Send();

        Assert.AreEqual(TextCatalog.RegisterPasswordTooShort, WarningOn(PasswordBox));
    }

    [TestMethod]
    public void Validate_APasswordOfTwoGroups_AsksForAThird()
    {
        Fill(PasswordBox, "passwordonly1");

        Send();

        Assert.AreEqual(TextCatalog.RegisterPasswordPolicy, WarningOn(PasswordBox));
    }

    [TestMethod]
    public void Validate_AConfirmationThatDiffers_SaysTheyDoNotMatch()
    {
        Fill(ConfirmationBox, "Password2");

        Send();

        Assert.AreEqual(TextCatalog.RegisterConfirmationMismatch, WarningOn(ConfirmationBox));
    }

    [TestMethod]
    public void Validate_ADayThatIsNotANumber_RefusesTheDate()
    {
        Fill(DayBox, "ab");

        Send();

        Assert.AreEqual(TextCatalog.RegisterBirthDateInvalid, WarningOn(DayBox));
    }

    [TestMethod]
    public void Validate_ADayOutsideTheMonth_RefusesTheDate()
    {
        // The good form is born in February, so the thirtieth never existed.
        Fill(DayBox, "30");

        Send();

        Assert.AreEqual(TextCatalog.RegisterBirthDateInvalid, WarningOn(DayBox));
    }

    [TestMethod]
    public void Validate_ABirthDateInTheFuture_RefusesIt()
    {
        Fill(YearBox, (DateTime.Today.Year + 1).ToString());

        Send();

        Assert.AreEqual(TextCatalog.RegisterBirthDateFuture, WarningOn(DayBox));
    }

    [TestMethod]
    public void Validate_SomeoneUnderEight_RefusesTheDate()
    {
        Fill(YearBox, (DateTime.Today.Year - 5).ToString());

        Send();

        Assert.AreEqual(TextCatalog.RegisterUnderage, WarningOn(DayBox));
    }

    [TestMethod]
    public void Validate_TermsNotAccepted_AsksForThem()
    {
        Fill();
        Untick();

        Send();

        Assert.AreEqual(TextCatalog.RegisterTermsRequired, WarningOnTheTerms());
    }

    private void Send()
    {
        ScreenDriver.Click(_form, TextCatalog.RegisterCreateButton);
    }

    // A good form with at most one box spoiled, which is what makes each
    // test read as the one rule it is about.
    private void Fill(int box = -1, string? value = null)
    {
        for (int index = 0; index < _goodForm.Length; index++)
        {
            ScreenDriver.Type(_form, index, index == box ? value! : _goodForm[index]);
        }

        Tick();
    }

    private void Tick()
    {
        ScreenDriver.TickEveryBox(_form);
    }

    private void Untick()
    {
        foreach (CheckBox box in ScreenDriver.GetCheckBoxes(_form))
        {
            box.IsChecked = false;
        }
    }

    private string? WarningOn(int box)
    {
        return ScreenDriver.GetFields(_form)[box].Warning;
    }

    private string? WarningOnTheTerms()
    {
        return ScreenDriver.GetCheckBoxes(_form)[0].Warning;
    }
}
