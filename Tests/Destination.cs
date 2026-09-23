using System;
using Bastion.Presentation.Utils;

namespace Bastion.Presentation.Tests;

// What a test cares about once a press has landed: which screen the game is
// showing and whether it is asking something before going on. Being a record,
// two destinations are equal when those two answers match and nothing else is
// compared, so a whole flow can end in a single AreEqual.
public sealed record Destination
{
    public required ScreenId Screen { get; init; }

    public required bool IsAsking { get; init; }

    public static Destination On(ScreenId screen)
    {
        return new Destination { Screen = screen, IsAsking = false };
    }

    public static Destination Asking(ScreenId screen)
    {
        return new Destination { Screen = screen, IsAsking = true };
    }

    public static Destination Of(Navigator navigator)
    {
        ArgumentNullException.ThrowIfNull(navigator);

        return new Destination
        {
            Screen = navigator.CurrentId,
            IsAsking = ScreenDriver.IsShowingADialog(navigator.Current)
        };
    }
}
