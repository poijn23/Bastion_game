using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bastion.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bastion.Presentation.Tests;

// The catalog answers with the key itself when an entry is missing, so a
// forgotten translation reaches the screen instead of failing the build.
// These read every property in both languages to catch it here.
[TestClass]
public sealed class TestTextCatalog
{
    private static IEnumerable<PropertyInfo> EveryEntry =>
        typeof(TextCatalog)
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Where(entry => entry.PropertyType == typeof(string));

    [TestMethod]
    public void Read_EveryEntryInEnglish_AnswersWithText()
    {
        Language.Apply(Language.English);

        IEnumerable<string> empty = EveryEntry
            .Where(entry => string.IsNullOrWhiteSpace((string?)entry.GetValue(null)))
            .Select(entry => entry.Name);

        Assert.IsFalse(empty.Any(), $"Empty in English: {string.Join(", ", empty)}.");
    }

    [TestMethod]
    public void Read_EveryEntryInSpanish_AnswersWithText()
    {
        Language.Apply(Language.SpanishMexico);

        IEnumerable<string> empty = EveryEntry
            .Where(entry => string.IsNullOrWhiteSpace((string?)entry.GetValue(null)))
            .Select(entry => entry.Name);

        Assert.IsFalse(empty.Any(), $"Empty in es-MX: {string.Join(", ", empty)}.");
    }

    [TestMethod]
    public void Read_EveryEntryInSpanish_AnswersWithSomethingOtherThanTheKey()
    {
        Language.Apply(Language.SpanishMexico);

        IEnumerable<string> missing = EveryEntry
            .Where(entry => string.Equals((string?)entry.GetValue(null), entry.Name, StringComparison.Ordinal))
            .Select(entry => entry.Name);

        Assert.IsFalse(missing.Any(), $"Missing from the es-MX catalog: {string.Join(", ", missing)}.");
    }

    [TestMethod]
    public void Read_EveryEntryInEnglish_AnswersWithSomethingOtherThanTheKey()
    {
        Language.Apply(Language.English);

        IEnumerable<string> missing = EveryEntry
            .Where(entry => string.Equals((string?)entry.GetValue(null), entry.Name, StringComparison.Ordinal))
            .Select(entry => entry.Name);

        Assert.IsFalse(missing.Any(), $"Missing from the English catalog: {string.Join(", ", missing)}.");
    }

    [TestMethod]
    public void Apply_SwitchingTheLanguage_ChangesWhatTheCatalogAnswers()
    {
        Language.Apply(Language.English);
        string english = TextCatalog.LoginSignInButton;

        Language.Apply(Language.SpanishMexico);

        Assert.AreNotEqual(english, TextCatalog.LoginSignInButton);
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
    public void LeaveTheCatalogInEnglish()
    {
        Language.Apply(Language.Default);
    }
}
