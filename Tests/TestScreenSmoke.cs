using System;
using System.Collections.Generic;
using System.Linq;
using Bastion.Presentation.Utils;
using Bastion.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bastion.Presentation.Tests;

// Every screen the navigator knows has to build and to fill its labels. A
// missing catalog key or a bad measurement only shows up when the screen is
// built, which is why this walks all of them.
[TestClass]
public sealed class TestScreenSmoke
{
    public static IEnumerable<object[]> EveryScreen =>
        Enum.GetValues<ScreenId>().Select(screen => new object[] { screen });

    [TestInitialize]
    public void StartInEnglish()
    {
        Language.Apply(Language.English);
    }

    [DataTestMethod]
    [DynamicData(nameof(EveryScreen))]
    public void GoTo_EveryScreen_BuildsIt(ScreenId screen)
    {
        var navigator = new Navigator();

        navigator.Start(screen);

        Assert.IsNotNull(navigator.Current, $"{screen} did not build.");
    }

    [DataTestMethod]
    [DynamicData(nameof(EveryScreen))]
    public void GoTo_EveryScreenInSpanish_BuildsIt(ScreenId screen)
    {
        Language.Apply(Language.SpanishMexico);
        var navigator = new Navigator();

        navigator.Start(screen);

        Assert.IsNotNull(navigator.Current, $"{screen} did not build in es-MX.");
    }

    [DataTestMethod]
    [DynamicData(nameof(EveryScreen))]
    public void Build_EveryScreen_LeavesNoButtonWithoutALabel(ScreenId screen)
    {
        var navigator = new Navigator();
        navigator.Start(screen);

        IEnumerable<Button> untitled = ScreenDriver
            .GetButtons(navigator.Current!)
            .Where(button => string.IsNullOrWhiteSpace(button.Title));

        Assert.IsFalse(untitled.Any(), $"{screen} draws a button with no label.");
    }

    // A key that is missing from the catalog comes back as its own name, so a
    // label that reads like an identifier means the resx lost an entry.
    [DataTestMethod]
    [DynamicData(nameof(EveryScreen))]
    public void Build_EveryScreen_ShowsNoRawCatalogKey(ScreenId screen)
    {
        var navigator = new Navigator();
        navigator.Start(screen);

        IEnumerable<string> raw = ScreenDriver
            .GetButtons(navigator.Current!)
            .Select(button => button.Title)
            .Where(LooksLikeAKey);

        Assert.IsFalse(raw.Any(), $"{screen} shows the raw key {string.Join(", ", raw)}.");
    }

    private static bool LooksLikeAKey(string title)
    {
        return !string.IsNullOrEmpty(title)
            && !title.Contains(' ', StringComparison.Ordinal)
            && title.Length > 12
            && title.Any(char.IsUpper)
            && title.Any(char.IsLower);
    }
}
