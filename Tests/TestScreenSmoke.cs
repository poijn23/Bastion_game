using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.Tests;

// Every screen the navigator knows has to build and to fill its labels. A
// missing catalog key or a bad measurement only shows up when the screen is
// built, which is why this walks all of them.
[TestClass]
public sealed class TestScreenSmoke
{
    private const int ShortestKeyLength = 12;

    public static IEnumerable<object[]> EveryScreen =>
        Enum.GetValues<ScreenId>().Select(screen => new object[] { screen });

    [TestInitialize]
    public void StartInEnglish()
    {
        Language.Apply(Language.English);
    }

    [DataTestMethod]
    [DynamicData(nameof(EveryScreen))]
    public void Start_EveryScreen_LandsOnIt(ScreenId screen)
    {
        var navigator = new Navigator();

        navigator.Start(screen);

        Assert.AreEqual(Destination.On(screen), Destination.Of(navigator));
    }

    [DataTestMethod]
    [DynamicData(nameof(EveryScreen))]
    public void Start_EveryScreenInSpanish_LandsOnIt(ScreenId screen)
    {
        Language.Apply(Language.SpanishMexico);
        var navigator = new Navigator();

        navigator.Start(screen);

        Assert.AreEqual(Destination.On(screen), Destination.Of(navigator));
    }

    [DataTestMethod]
    [DynamicData(nameof(EveryScreen))]
    public void Start_EveryScreen_LeavesNoButtonWithoutALabel(ScreenId screen)
    {
        var navigator = new Navigator();

        navigator.Start(screen);

        string untitled = Join(ScreenDriver
            .GetButtons(navigator)
            .Select((button, index) => (button, index))
            .Where(pair => string.IsNullOrWhiteSpace(pair.button.Title))
            .Select(pair => $"{screen} button {pair.index}"));

        Assert.AreEqual(string.Empty, untitled);
    }

    // A key that is missing from the catalog comes back as its own name, so a
    // label that reads like an identifier means the resx lost an entry.
    [DataTestMethod]
    [DynamicData(nameof(EveryScreen))]
    public void Start_EveryScreen_ShowsNoRawCatalogKey(ScreenId screen)
    {
        var navigator = new Navigator();

        navigator.Start(screen);

        string raw = Join(ScreenDriver
            .GetButtons(navigator)
            .Select(button => button.Title)
            .Where(LooksLikeAKey));

        Assert.AreEqual(string.Empty, raw);
    }

    private static string Join(IEnumerable<string> names)
    {
        return string.Join(", ", names);
    }

    private static bool LooksLikeAKey(string title)
    {
        return !string.IsNullOrEmpty(title)
            && !title.Contains(' ', StringComparison.Ordinal)
            && title.Length > ShortestKeyLength
            && title.Any(char.IsUpper)
            && title.Any(char.IsLower);
    }
}
