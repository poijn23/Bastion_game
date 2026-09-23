using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bastion.Resources;

namespace Bastion.Presentation.Tests;

// The catalog answers with the key itself when an entry is missing, so a
// forgotten translation reaches the screen instead of failing the build.
// These read every property in both languages to catch it here.
[TestClass]
public sealed class TestTextCatalog
{
    private const int ExpectedReadings = 2;

    private static IEnumerable<PropertyInfo> EveryEntry =>
        typeof(TextCatalog)
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Where(entry => entry.PropertyType == typeof(string));

    [TestMethod]
    public void Read_EveryEntryInEnglish_AnswersWithText()
    {
        Language.Apply(Language.English);

        string empty = NameTheOnesThat(text => string.IsNullOrWhiteSpace(text));

        Assert.AreEqual(string.Empty, empty);
    }

    [TestMethod]
    public void Read_EveryEntryInSpanish_AnswersWithText()
    {
        Language.Apply(Language.SpanishMexico);

        string empty = NameTheOnesThat(text => string.IsNullOrWhiteSpace(text));

        Assert.AreEqual(string.Empty, empty);
    }

    [TestMethod]
    public void Read_EveryEntryInEnglish_AnswersSomethingOtherThanItsKey()
    {
        Language.Apply(Language.English);

        string missing = NameTheOnesNamedAfterThemselves();

        Assert.AreEqual(string.Empty, missing);
    }

    [TestMethod]
    public void Read_EveryEntryInSpanish_AnswersSomethingOtherThanItsKey()
    {
        Language.Apply(Language.SpanishMexico);

        string missing = NameTheOnesNamedAfterThemselves();

        Assert.AreEqual(string.Empty, missing);
    }

    // Two readings of the same key that land in one bucket would mean the
    // language never changed anything.
    [TestMethod]
    public void Apply_ReadingOneKeyInBothLanguages_AnswersTwoDifferentTexts()
    {
        Language.Apply(Language.English);
        string english = TextCatalog.LoginSignInButton;

        Language.Apply(Language.SpanishMexico);

        Assert.AreEqual(ExpectedReadings, CountDistinct(english, TextCatalog.LoginSignInButton));
    }

    [TestMethod]
    public void Apply_ComingBackToEnglish_AnswersWhatItAnsweredBefore()
    {
        Language.Apply(Language.English);
        string english = TextCatalog.LoginSignInButton;
        Language.Apply(Language.SpanishMexico);

        Language.Apply(Language.English);

        Assert.AreEqual(english, TextCatalog.LoginSignInButton);
    }

    [TestCleanup]
    public void LeaveTheCatalogInTheDefaultLanguage()
    {
        Language.Apply(Language.Default);
    }

    private static int CountDistinct(string first, string second)
    {
        return new HashSet<string>(StringComparer.Ordinal) { first, second }.Count;
    }

    private static string NameTheOnesThat(Func<string?, bool> isWrong)
    {
        return string.Join(", ", EveryEntry
            .Where(entry => isWrong((string?)entry.GetValue(null)))
            .Select(entry => entry.Name));
    }

    private static string NameTheOnesNamedAfterThemselves()
    {
        return string.Join(", ", EveryEntry
            .Where(entry => string.Equals((string?)entry.GetValue(null), entry.Name, StringComparison.Ordinal))
            .Select(entry => entry.Name));
    }
}
