using System;
using Bastion.Presentation.Utils;

namespace Bastion.Presentation.Tests;

// What a test cares about once a press has landed: which screen the game is
// showing and whether a dialog is standing over it, asking or telling. Being
// a record, two destinations are equal when those two answers match and
// nothing else is compared, so a whole flow ends in a single AreEqual.
public sealed record Destination
{
    public required ScreenId Screen { get; init; }

    public required bool HasDialog { get; init; }

    public static Destination On(ScreenId screen)
    {
        return new Destination { Screen = screen, HasDialog = false };
    }

    public static Destination WithDialog(ScreenId screen)
    {
        return new Destination { Screen = screen, HasDialog = true };
    }

    public static Destination Of(Navigator navigator)
    {
        ArgumentNullException.ThrowIfNull(navigator);

        return new Destination
        {
            Screen = navigator.CurrentId,
            HasDialog = ScreenDriver.IsShowingADialog(navigator.Current)
        };
    }
}
