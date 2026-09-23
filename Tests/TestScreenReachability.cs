using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bastion.Presentation.GUI_Match;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.Tests;

// Walks the game the way a person would: from the title screen, pressing every
// button, row and menu entry of every screen it finds. What it cannot reach is
// a screen nobody can open, which is the failure this guards against.
[TestClass]
public sealed class TestScreenReachability
{
    private const ScreenId Entry = ScreenId.MainScreen;

    // What a screen needs typed into it before its buttons do anything. A
    // screen with several answers, such as the sign in form, gets one run per
    // answer.
    private static readonly IReadOnlyDictionary<ScreenId, string[][]> _typing =
        new Dictionary<ScreenId, string[][]>
        {
            [ScreenId.Login] =
            [
                [TestAccount.Nickname, TestAccount.Password],
                [TestAccount.BannedIdentifier, TestAccount.Password],
                [TestAccount.PendingIdentifier, TestAccount.Password]
            ],
            [ScreenId.ForgotPassword] = [[TestAccount.Email]],
            [ScreenId.Register] =
            [
                ["Ada", "Stone", "newcomer", "newcomer@bastion.test", "Password1", "Password1", "01", "01", "2000"]
            ]
        };

    // The screens that mean something different depending on what opened them.
    private static readonly IReadOnlyDictionary<ScreenId, string?[]> _arguments =
        new Dictionary<ScreenId, string?[]>
        {
            [ScreenId.Match] = [null, GuiMatch.AiOpponent]
        };

    [TestMethod]
    public void Crawl_FromTheTitleScreen_ReachesEveryScreen()
    {
        IReadOnlySet<ScreenId> reached = Crawl();

        string orphans = Name(Enum.GetValues<ScreenId>().Where(screen => !reached.Contains(screen)));

        Assert.AreEqual(string.Empty, orphans);
    }

    [TestMethod]
    public void Crawl_WithoutTheProvisionalIndex_StillReachesEveryOtherScreen()
    {
        IReadOnlySet<ScreenId> reached = Crawl(ScreenId.Menu);

        string orphans = Name(Enum.GetValues<ScreenId>()
            .Where(screen => screen != ScreenId.Menu && !reached.Contains(screen)));

        Assert.AreEqual(string.Empty, orphans);
    }

    private static string Name(IEnumerable<ScreenId> screens)
    {
        return string.Join(", ", screens);
    }

    private static IReadOnlySet<ScreenId> Crawl(ScreenId? closed = null)
    {
        Language.Apply(Language.English);

        var reached = new HashSet<ScreenId> { Entry };
        var pending = new Queue<ScreenId>();
        pending.Enqueue(Entry);

        while (pending.Count > 0)
        {
            foreach (ScreenId found in Follow(pending.Dequeue(), closed))
            {
                if (reached.Add(found))
                {
                    pending.Enqueue(found);
                }
            }
        }

        return reached;
    }

    private static IEnumerable<ScreenId> Follow(ScreenId from, ScreenId? closed)
    {
        var found = new List<ScreenId>();

        foreach (string? argument in _arguments.TryGetValue(from, out string?[]? arguments) ? arguments : [null])
        {
            for (int variant = 0; variant < CountVariants(from); variant++)
            {
                found.AddRange(FollowOneVariant(new Visit(from, argument, variant)));
            }
        }

        return found.Where(screen => screen != from && screen != closed);
    }

    private static IEnumerable<ScreenId> FollowOneVariant(Visit visit)
    {
        var found = new List<ScreenId>();
        Navigator opened = Open(visit);

        int pressable = ScreenDriver.GetClickables(opened).Count;
        IReadOnlyList<SidebarMenu> menus = ScreenDriver.GetMenus(opened);
        int entries = menus.Count == 0 ? 0 : menus[0].Items.Count;

        for (int index = 0; index < pressable; index++)
        {
            found.Add(Press(visit, index, Gesture.Control));
        }

        for (int index = 0; index < entries; index++)
        {
            found.Add(Press(visit, index, Gesture.MenuEntry));
        }

        return found;
    }

    private static int CountVariants(ScreenId screen)
    {
        return _typing.TryGetValue(screen, out string[][]? variants) ? variants.Length : 1;
    }

    private static Navigator Open(Visit visit)
    {
        var navigator = new Navigator();
        navigator.Start(visit.Screen);

        if (visit.Argument is not null)
        {
            navigator.GoTo(visit.Screen, visit.Argument);
        }

        if (!_typing.TryGetValue(visit.Screen, out string[][]? variants))
        {
            return navigator;
        }

        string[] values = variants[visit.Variant];

        for (int index = 0; index < values.Length; index++)
        {
            ScreenDriver.Type(navigator, index, values[index]);
        }

        ScreenDriver.TickEveryBox(navigator);

        return navigator;
    }

    // Every press starts from a screen built again, because a press may leave
    // the one before it in a state the next press would read.
    private static ScreenId Press(Visit visit, int index, Gesture gesture)
    {
        Navigator navigator = Open(visit);

        switch (gesture)
        {
            case Gesture.MenuEntry:
                ScreenDriver.ClickMenuItem(navigator, index);
                break;
            case Gesture.Control:
            default:
                ScreenDriver.ClickAt(navigator, index);
                break;
        }

        // A press that only asks a question is answered, so the crawl sees
        // where saying yes leads.
        if (ScreenDriver.IsShowingADialog(navigator.Current))
        {
            ScreenDriver.ConfirmDialog(navigator);
        }

        return navigator.CurrentId;
    }

    private enum Gesture
    {
        Control,
        MenuEntry
    }

    private sealed record Visit(ScreenId Screen, string? Argument, int Variant);
}
